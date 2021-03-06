﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace BSASimulator
{
    /// <summary>
    ///     Author Chengkangyang @8/11/14
    /// </summary>
    internal class DataProvider
    {
        private readonly int[] _offsetX = { 1, 0, 0, -1, -1, -1 };
        private readonly int[] _offsetY = { 0, -1, 1, 0, -1, 1 };
        private List<Dictionary<string, double>> _aoaList; //cell-id,aoa 
        private List<double> _fetchDataInterval;
        private bool _isDataReady;
        private Option _options;
        private List<double[]> _realPathList; //x,y
        private List<Dictionary<string, double>> _rssList; //cell-id,rss 
        private List<Dictionary<string, int>> _taList; //cell-id,ta
        private Dictionary<string, double[]> _bsList;//cell-id,position 

        public static DataProvider NewInstance()
        {
            return new DataProvider();
        }

        public DataProvider PrepareData(Option options)
        {
            _isDataReady = false;
            _options = options;
            _fetchDataInterval = new List<double>();
            _realPathList = new List<double[]>();
            _taList = new List<Dictionary<string, int>>();
            _rssList = new List<Dictionary<string, double>>();
            _aoaList = new List<Dictionary<string, double>>();
            _bsList = new Dictionary<string, double[]>();

            if (_options.GetIsUsingExternalData())
            {
                //Read datas from .csv
                string baseInfo = Utils.ReadFileDialog("读入基站信息");
                if (ReadBaseData(baseInfo))
                {
                    string testData = Utils.ReadFileDialog("读入实测数据");
                    if (ReadTestData(testData))
                    {
                        _isDataReady = true;
                        return this;
                    }
                }
            }
            else
            {
                //Generate data by myself
                int currentIntervalIndex = 0;

                var currentMovement = new[]
                {
                    _options.GetStartPosition()[0],
                    _options.GetStartPosition()[1],
                    _options.GetInitDirection(),
                    _options.GetIntervalPattern()[currentIntervalIndex]
                };

                while (
                    currentMovement[0] >= 0 &&
                    currentMovement[0] <= _options.GetMapProportion()[0] &&
                    currentMovement[1] >= 0 &&
                    currentMovement[1] <= _options.GetMapProportion()[1])
                {
                    GetData(currentMovement);
                    currentIntervalIndex = (currentIntervalIndex + 1) % options.GetIntervalPattern().Length;
                    currentMovement = Move(currentMovement, currentIntervalIndex);
                }

                _isDataReady = true;
                return this;
            }

            _isDataReady = false;
            return this;
        }

        private double[] Move(double[] currentMovement, int currentIntervalIndex)
        {
            double newDirection = Utils.GetBiasedAngle(currentMovement[2]) / 180 * Math.PI;
            double fetchDataInterval = Utils.GetBiasedValue(
                Utils.GetAvgIntervalBasedOnVelocity(
                _options.GetIntervalPattern()[currentIntervalIndex],
                _options.GetAvgVelocity()));
            double moveDistance = fetchDataInterval * Utils.GetBiasedValue(_options.GetAvgVelocity());
            double nowX = currentMovement[0] + moveDistance * Math.Cos(newDirection);
            double nowY = currentMovement[1] + moveDistance * Math.Sin(newDirection);
            return new[] { nowX, nowY, newDirection, fetchDataInterval };
        }

        private void GetData(double[] currentMovement)
        {
            //Save the fetch data interval and current real position
            _fetchDataInterval.Add(currentMovement[3]);
            _realPathList.Add(new[] { currentMovement[0], currentMovement[1] });

            //Get the region length
            double regionLength = _options.GetBsIntencity();

            //Prepare the holders
            var fetchingDic = new Dictionary<string, double[]>();
            var taDic = new Dictionary<string, int>();
            var rssDic = new Dictionary<string, double>();
            var aoaDic = new Dictionary<string, double>();

            //Get the current region index
            int currentIndexX;
            var currentIndexY = (int)Math.Ceiling(currentMovement[1] / regionLength);
            if (currentIndexY % 2 == 0)
                currentIndexX = (int)Math.Ceiling((currentMovement[0] + regionLength * 0.5) / regionLength);
            else currentIndexX = (int)Math.Ceiling(currentMovement[0] / regionLength);

            //Get the base stations' cell ids and add it to fetching list
            string cellId = currentIndexX + "," + currentIndexY;
            fetchingDic.Add(cellId, Utils.GetBsPosition(currentIndexX, currentIndexY, regionLength));

            //Get the base station positions in the around regions and add them to fetching list
            for (int i = 0; i < 6; i++)
            {
                int aroundIndexX = currentIndexX + _offsetX[i];
                int aroundIndexY = currentIndexY + _offsetY[i];

                if (aroundIndexX > 0 && aroundIndexY > 0)
                {
                    cellId = aroundIndexX + "," + aroundIndexY;
                    fetchingDic.Add(cellId, Utils.GetBsPosition(aroundIndexX, aroundIndexY, regionLength));
                }
            }

            //Fetch data from base stations
            foreach (var kvp in fetchingDic)
            {
                double distance = Utils.GetDistanceBetween(
                    currentMovement[0],
                    currentMovement[1],
                    kvp.Value[0],
                    kvp.Value[1]);

                if (distance <= _options.GetReceiveRadius())
                {
                    taDic.Add(kvp.Key, Utils.GetTa(distance, _options.GetSystemType()));
                    rssDic.Add(kvp.Key, Utils.GetSignalStrength(
                        currentMovement[0],
                        currentMovement[1],
                        distance,
                        _options.GetSystemType()));
                    aoaDic.Add(kvp.Key, Utils.GetAoa(
                        currentMovement[0],
                        currentMovement[1],
                        kvp.Value[0],
                        kvp.Value[1],
                        _options.GetHeight()));
                }
            }

            //Save to data lists
            _rssList.Add(rssDic);
            _taList.Add(taDic);
            _aoaList.Add(aoaDic);
        }

        /// <summary>
        ///     Return the total fetching data times
        /// </summary>
        /// <returns></returns>
        public int GetTotalStep()
        {
            return _realPathList.Count;
        }

        /// <summary>
        ///     Returns RSSs fetched at the current positon, around 6(if in receive radius)
        ///     stations and the main connected position
        /// </summary>
        /// <param name="step"></param>
        /// <returns></returns>
        public Dictionary<string, double> GetRssAt(int step)
        {
            if (!_isDataReady)
            {
                throw new Exception("数据没有初始化");
            }
            return _rssList[step];
        }

        /// <summary>
        ///     Returns AOAs fetched at the current positon, around 6(if in receive radius)
        ///     stations and the main connected position
        /// </summary>
        /// <param name="step"></param>
        /// <returns></returns>
        public Dictionary<string, double> GetAoaAt(int step)
        {
            if (!_isDataReady)
            {
                throw new Exception("数据没有初始化");
            }
            return _aoaList[step];
        }

        /// <summary>
        ///     Returns TAs fetched at the current positon, around 6(if in receive radius)
        ///     stations and the main connected position
        /// </summary>
        /// <param name="step"></param>
        /// <returns></returns>
        public Dictionary<string, int> GetTaAt(int step)
        {
            if (!_isDataReady)
            {
                throw new Exception("数据没有初始化");
            }
            return _taList[step];
        }

        /// <summary>
        ///     Returns Intervals in fetching around stations at the current positon(interval from last fetching),
        ///     around 6(if in receive radius) stations and the main connected position
        /// </summary>
        /// <param name="step"></param>
        /// <returns></returns>
        public double GetFetchDataIntervalAt(int step)
        {
            if (!_isDataReady)
            {
                throw new Exception("数据没有初始化");
            }
            return _fetchDataInterval[step];
        }

        /// <summary>
        ///     Get the real position at the given step
        /// </summary>
        /// <param name="step"></param>
        /// <returns></returns>
        public double[] GetRealPathAt(int step)
        {
            if (!_isDataReady)
            {
                throw new Exception("数据没有初始化");
            }
            return _realPathList[step];
        }

        /// <summary>
        ///     Get base stations' positon by their cell id
        /// </summary>
        /// <returns></returns>
        public double[] GetBsPosition(string cellId)
        {
            if (_options.GetIsUsingExternalData()) return _bsList[cellId];

            int x, y;
            try
            {
                string[] tmp = cellId.Split(',');
                x = int.Parse(tmp[0]);
                y = int.Parse(tmp[1]);
            }
            catch (Exception)
            {
                throw new Exception("整形转换错误");
            }

            return Utils.GetBsPosition(x, y, _options.GetBsIntencity());
        }

        public List<double[]> GetRealPathAll()
        {
            if (!_isDataReady)
            {
                throw new Exception("数据没有初始化");
            }
            return _realPathList;
        }

        private bool ReadBaseData(string storagePath)
        {
            //Null or empty path -> false
            if (string.IsNullOrEmpty(storagePath)) return false;

            //Reading data, if success, return true.
            var fi = new FileInfo(storagePath.ToString(CultureInfo.InvariantCulture));
            if (!fi.Exists) return false;

            //Reading data from .csv
            try
            {
                var sreader = new StreamReader(storagePath);
                string rline;
                while ((rline = sreader.ReadLine()) != null)
                {
                    if (rline.Length == 0) continue;
                    string[] readArray = rline.Split(',');
                    //Save base station infos
                    _bsList.Add(readArray[0], new[]
                    {
                        Convert.ToDouble(readArray[4]),
                        Convert.ToDouble(readArray[5])
                    });
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        private bool ReadTestData(string storagePath)
        {
            //Null or empty path -> false
            if (string.IsNullOrEmpty(storagePath)) return false;

            //Reading data, if success, return true.
            var fi = new FileInfo(storagePath.ToString(CultureInfo.InvariantCulture));
            if (!fi.Exists) return false;

            //Reading data from .csv
            try
            {
                var sreader = new StreamReader(storagePath);
                string rline;
                while ((rline = sreader.ReadLine()) != null)
                {
                    if (rline.Length == 0) continue;
                    string[] readArray = rline.Split(',');
                    //Save real path
                    _realPathList.Add(new[]
                    {
                        Convert.ToDouble(readArray[3]),
                        Convert.ToDouble(readArray[4])
                    });
                    //Save TA
                    var addItem = new Dictionary<string, int>();
                    addItem.Add(readArray[2], Convert.ToInt16(readArray[5]));
                    _taList.Add(addItem);
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public bool GetIsDataReady()
        {
            return _isDataReady;
        }
    }
}