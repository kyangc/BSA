using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Threading;

namespace BSASimulator
{
    internal class Tdoa
    {
        private DataProvider _dataProvider;
        private Option _option;
        private List<double[]> _resultPath;

        public static Tdoa NewIncetance()
        {
            return new Tdoa();
        }

        public Tdoa SetOption(Option option)
        {
            _option = option;
            return this;
        }

        public Tdoa SetDataProvider(DataProvider dataProvider)
        {
            _dataProvider = dataProvider;
            return this;
        }

        public Tdoa StartAlgorithm()
        {
            _resultPath = new List<double[]>();
            //TODO And real alocation algorithm
            switch (_option.GetSystemType())
            {
                case Option.SystemType.Cdma2000:
                    int gridWide = 20;//网格分辨率
                    for (int i = 0; i < _dataProvider.GetTotalStep(); i++)
                    {
                        UpdateProgress(i, _dataProvider.GetTotalStep());
                        Dictionary<string, int> taDictionary = _dataProvider.GetTaAt(i);
                        int[] bsPositionRangeInt = GetBsPositionRange(taDictionary);
                        double[] taDistance = new double[3];
                        double[] pointDistance = new double[3];
                        double euclideanDistance = 10000;
                        int[] ta = taDictionary.Values.ToArray();
                        for (int j = 0; j < 3; j++)
                        {
                            taDistance[j] = ta[j] * _option.GetTaDistance();
                        }

                        double[] position = new double[2];
                        string[] cellids = taDictionary.Keys.ToArray();
                        for (int xc = bsPositionRangeInt[0]; xc < bsPositionRangeInt[2]; xc = xc + gridWide)
                        {
                            for (int yc = bsPositionRangeInt[1]; yc < bsPositionRangeInt[3]; yc = yc + gridWide)
                            {
                                for (int c = 0; c < 3; c++)
                                {
                                    pointDistance[c] = Utils.GetDistanceBetween(xc, yc,
                                        _dataProvider.GetBsPosition(cellids[c])[0], _dataProvider.GetBsPosition(cellids[c])[1]);
                                }
                                if (GetEuclideanDistance(taDistance, pointDistance) < euclideanDistance)
                                {
                                    euclideanDistance = GetEuclideanDistance(taDistance, pointDistance);
                                    position[0] = xc;
                                    position[1] = yc;
                                }
                            }
                        }

                        _resultPath.Add(position);
                    }
                    break;
                case Option.SystemType.Wcdma:
                    break;
            }
            return this;
        }

        public int[] GetBsPositionRange(Dictionary<string, int> taDictionary)
        {
            double[] bsRange = new double[4];//x1,y1,x2,y2
            string[] cellids = taDictionary.Keys.ToArray();
            bsRange[0] = _dataProvider.GetBsPosition(cellids[0])[0];
            bsRange[1] = _dataProvider.GetBsPosition(cellids[0])[1];
            bsRange[2] = _dataProvider.GetBsPosition(cellids[1])[0];
            bsRange[3] = _dataProvider.GetBsPosition(cellids[1])[1];
            for (int i = 0; i < taDictionary.Count; i++)
            {
                double[] bsPosition = _dataProvider.GetBsPosition(cellids[i]);
                bsRange[0] = bsPosition[0] < bsRange[0] ? bsPosition[0] : bsRange[0];
                bsRange[1] = bsPosition[1] < bsRange[1] ? bsPosition[1] : bsRange[1];
                bsRange[2] = bsPosition[0] > bsRange[2] ? bsPosition[0] : bsRange[2];
                bsRange[3] = bsPosition[1] > bsRange[3] ? bsPosition[1] : bsRange[3];
            }

            int[] bsRangeInt = new int[4];
            bsRangeInt[0] = (int)bsRange[0] - 1500;
            bsRangeInt[1] = (int)bsRange[1] - 1500;
            bsRangeInt[2] = (int)bsRange[2] + 1500;
            bsRangeInt[3] = (int)bsRange[3] + 1500;
            return bsRangeInt;
        }

        public double GetEuclideanDistance(double[] x, double[] y)
        {
            return Math.Sqrt(Math.Pow((x[0] - y[0]), 2) + Math.Pow((x[1] - y[1]), 2) + Math.Pow((x[2] - y[2]), 2));
        }

        public List<double[]> GetResultPath()
        {
            return _resultPath;
        }

        private void UpdateProgress(int i, int total)
        {
            _option.GetMainWindow()
                .Dispatcher
                .BeginInvoke(
                    DispatcherPriority.Normal,
                    new ChangeProgress(
                        () => { _option.GetMainWindow().ProgressBar.Value = i/(double) total*1000; }));
        }

        private delegate void ChangeProgress();
    }
}