using System.Collections.Generic;
using System.Linq;

namespace BSASimulator
{
    internal class Ta
    {
        private DataProvider _dataProvider;
        private Option _option;
        private List<double[]> _resultPath;

        public static Ta NewIncetance()
        {
            return new Ta();
        }

        public Ta SetDataProvider(DataProvider dataProvider)
        {
            _dataProvider = dataProvider;
            return this;
        }

        public Ta SetOption(Option option)
        {
            _option = option;
            return this;
        }

        public Ta StartAlgorithm()
        {
            _resultPath = new List<double[]>();
            //TODO And real alocation algorithm
            switch (_option.GetSystemType())
            {
                case Option.SystemType.CDMA2000:
                    //In this system, you can get mutiple TA at one time
                    for (int i = 0; i < _dataProvider.GetTotalStep(); i++)
                    {
                        double[] searchArea = GetOverlapArea(3, _dataProvider.GetTaAt(i));
                        double[] taVector = GetTaVector(3, _dataProvider.GetTaAt(i));

                        double diff = int.MaxValue;
                        var position = new double[2];

                        for (var j = (int) searchArea[0]; j < (int) searchArea[1]; j += 50)
                        {
                            for (var k = (int) searchArea[2]; k < (int) searchArea[3]; k += 50)
                            {
                                double tmp = Utils.GetEucDisatance(taVector,
                                    GetDistanceVector(j, k, 3, _dataProvider.GetTaAt(i)));
                                if (tmp < diff)
                                {
                                    diff = tmp;
                                    position[0] = j;
                                    position[1] = k;
                                }
                            }
                        }

                        _resultPath.Add(position);
                    }
                    break;
                case Option.SystemType.WCDMA:
                    break;
            }
            return this;
        }

        public List<double[]> GetResultPath()
        {
            //TODO return the allocation result
            return _resultPath;
        }

        private double[] GetOverlapArea(int circleCount, Dictionary<string, int> taDictionary)
        {
            circleCount = circleCount > taDictionary.Count ? taDictionary.Count : circleCount;

            double[] areaSize;
            string[] cellids = taDictionary.Keys.ToArray();

            double marginLeft = 0;
            double marginRight = _option.GetMapProportion()[0];
            double marginTop = _option.GetMapProportion()[1];
            double marginButtom = 0;

            for (int i = 0; i < circleCount; i++)
            {
                double[] pos = _dataProvider.GetBsPosition(cellids[i]);
                double radius = taDictionary[cellids[i]]*_option.GetTaDistance();
                marginLeft = pos[0] - radius > marginLeft ? pos[0] - radius : marginLeft;
                marginRight = pos[0] + radius < marginRight ? pos[0] + radius : marginRight;
                marginButtom = pos[1] - radius > marginButtom ? pos[1] - radius : marginButtom;
                marginTop = pos[1] + radius < marginTop ? pos[1] + radius : marginTop;
            }

            return new[] {marginLeft, marginRight, marginButtom, marginTop};
        }

        private double[] GetDistanceVector(double x, double y, int vectorCount, Dictionary<string, int> taDictionary)
        {
            vectorCount = vectorCount > taDictionary.Count ? taDictionary.Count : vectorCount;

            var vector = new double[vectorCount];
            string[] cellids = taDictionary.Keys.ToArray();

            for (int i = 0; i < vectorCount; i++)
            {
                double[] baseStationPosition = _dataProvider.GetBsPosition(cellids[i]);
                vector[i] = Utils.GetDistanceBetween(x, y, baseStationPosition[0], baseStationPosition[1]);
            }

            return vector;
        }

        private double[] GetTaVector(int vectorCount, Dictionary<string, int> taDictionary)
        {
            vectorCount = vectorCount > taDictionary.Count ? taDictionary.Count : vectorCount;

            var vector = new double[vectorCount];
            int[] ta = taDictionary.Values.ToArray();

            for (int i = 0; i < vectorCount; i++)
            {
                vector[i] = ta[i]*_option.GetTaDistance();
            }

            return vector;
        }
    }
}