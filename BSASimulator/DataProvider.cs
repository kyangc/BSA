﻿using System;
using System.Collections.Generic;

namespace BSASimulator
{
    internal class DataProvider
    {

        private bool _isDataReady = false;
        private Option _options;
        private List<double> _fetchDataInterval;
        private List<double[]> _realPathList; //x,y
        private List<Dictionary<string, double>> _aoaList; //cell-id,aoa 
        private List<Dictionary<string, double>> _rssList; //cell-id,rss 
        private List<Dictionary<string, int>> _taList; //cell-id,ta
        private int[] _offsetX = new int[] { 1, 0, 0, -1, -1, -1 };
        private int[] _offsetY = new int[] { 0, -1, 1, 0, -1, 1 };

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

            double[] currentMovement = new double[4]
            {
                _options.GetStartPosition()[0],
                _options.GetStartPosition()[1],
                _options.GetInitDirection(),
                _options.GetAvgFetchDataInterval()
            };

            while (
                currentMovement[0] >= 0 &&
                currentMovement[0] <= _options.GetMapProportion()[0] &&
                currentMovement[1] >= 0 &&
                currentMovement[1] <= _options.GetMapProportion()[1])
            {
                GetData(currentMovement);
                currentMovement = Move(currentMovement);
            }

            _isDataReady = true;
            return this;
        }

        private double[] Move(double[] currentMovement)
        {
            double newDirection = Utils.GetBiasedAngle(currentMovement[2]) / 180 * Math.PI;
            double fetchDataInterval = Utils.GetBiasedValue(_options.GetAvgFetchDataInterval());
            double moveDistance = fetchDataInterval * Utils.GetBiasedValue(_options.GetAvgVelocity());
            double nowX = currentMovement[0] + moveDistance * Math.Cos(newDirection);
            double nowY = currentMovement[1] + moveDistance * Math.Sin(newDirection);
            return new double[4] { nowX, nowY, newDirection, fetchDataInterval };
        }

        private void GetData(double[] currentMovement)
        {
            //Save the fetch data interval and current real position
            _fetchDataInterval.Add(currentMovement[3]);
            _realPathList.Add(new double[2] { currentMovement[0], currentMovement[1] });

            //Get the region length
            double regionLength = _options.GetBsIntencity();

            //Prepare the holders
            var fetchingDic = new Dictionary<string, double[]>();
            var taDic = new Dictionary<string, int>();
            var rssDic = new Dictionary<string, double>();
            var aoaDic = new Dictionary<string, double>();

            //Get the current region index
            int currentIndexX = 0;
            int currentIndexY = (int)Math.Ceiling(currentMovement[1] / regionLength);
            if (currentIndexY % 2 == 0) currentIndexX = (int)Math.Ceiling((currentMovement[0] + regionLength * 0.5) / regionLength);
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
            foreach (KeyValuePair<string, double[]> kvp in fetchingDic)
            {
                double distance = Utils.GetDistanceBetween(
                    currentMovement[0],
                    currentMovement[1],
                    kvp.Value[0],
                    kvp.Value[1]);

                if (distance <= _options.GetReceiveRadius())
                {
                    taDic.Add(kvp.Key, Utils.GetTa(distance));
                    rssDic.Add(kvp.Key, Utils.GetSignalStrength(
                        currentMovement[0],
                        currentMovement[1],
                        distance,
                        _options.GetSystemType()));
                    aoaDic.Add(kvp.Key, Utils.GetAoa(
                        currentMovement[0],
                        currentMovement[1],
                        kvp.Value[0],
                        kvp.Value[1]));
                }
            }

            //Save to data lists
            _rssList.Add(rssDic);
            _taList.Add(taDic);
            _aoaList.Add(aoaDic);
        }

        /// <summary>
        /// Returns RSSs fetched at the current positon, around 6(if in receive radius)
        ///  stations and the main connected position
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
        /// Returns AOAs fetched at the current positon, around 6(if in receive radius)
        ///  stations and the main connected position
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
        ///  Returns TAs fetched at the current positon, around 6(if in receive radius)
        ///  stations and the main connected position
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
        ///  Returns Intervals in fetching around stations at the current positon,
        ///  around 6(if in receive radius) stations and the main connected position
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

        public double[] GetRealPathAt(int step)
        {
            if (!_isDataReady)
            {
                throw new Exception("数据没有初始化");
            }
            return _realPathList[step];
        }
    }
}