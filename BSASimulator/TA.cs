using System.Collections.Generic;
using System.Linq;
using System.Windows.Threading;

namespace BSASimulator
{
    /// <summary>
    ///     @Author Chengkangyang @11/11/14
    /// </summary>
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
                case Option.SystemType.Cdma2000:
                    //In this system, you can get mutiple TA at one time
                    for (int i = 0; i < _dataProvider.GetTotalStep(); i++)
                    {
                        UpdateProgress(i, _dataProvider.GetTotalStep());
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
                case Option.SystemType.Wcdma:
                    var posiblePostion = new Dictionary<int, List<double[]>>();
                    for (int i = 0; i < _dataProvider.GetTotalStep(); i++) posiblePostion.Add(i, new List<double[]>());

                    //Generate posible positions
                    if (_option.GetIsUsingExternalData())
                    {
                        var baseInfo = new List<Dictionary<string, int[]>>();
                        int StartIndex = 0,
                            EndIndex = 0;
                        string duringCellid = _dataProvider.GetTaAt(0).Keys.ToArray()[0];
                        int currentBase = 0;
                        var baseItem = new Dictionary<string, int[]>();
                        baseItem.Add(duringCellid, new[] {0, 0});
                        baseInfo.Add(baseItem);

                        for (int i = 0; i < _dataProvider.GetTotalStep(); i++)
                        {
                            string currentCellId = _dataProvider.GetTaAt(i).Keys.ToArray()[0];

                            if (currentCellId.Equals(duringCellid))
                            {
                                EndIndex = i;
                                baseInfo[currentBase][currentCellId][0] = StartIndex;
                                baseInfo[currentBase][currentCellId][1] = EndIndex;
                            }
                            else
                            {
                                StartIndex = i;
                                EndIndex = i;
                                currentBase++;
                                duringCellid = currentCellId;
                                baseItem = new Dictionary<string, int[]>();
                                baseItem.Add(currentCellId, new[] {StartIndex, EndIndex});
                                baseInfo.Add(baseItem);
                            }
                        }

                        for (int i = 0; i + 1 < baseInfo.Count; i++)
                        {
                            Dictionary<string, int[]> info1 = baseInfo[i];
                            Dictionary<string, int[]> info2 = baseInfo[i + 1];
                            double[] position1 = _dataProvider.GetBsPosition(info1.Keys.ToArray()[0]);
                            double[] position2 = _dataProvider.GetBsPosition(info2.Keys.ToArray()[0]);
                            int stepStart = info1.Values.ToArray()[0][0];
                            int stepEnd = info2.Values.ToArray()[0][1];
                            int stepCount = stepEnd - stepStart + 1;
                            double x = position1[0];
                            double y = position1[1];
                            double deltaX = (position2[0] - position1[0])/stepCount;
                            double deltaY = (position2[1] - position1[1])/stepCount;
                            for (int j = stepStart; j <= stepEnd; j++)
                            {
                                posiblePostion[j].Add(new[] {x + deltaX*(j - stepStart), y + deltaY*(j - stepStart)});
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i + 3 < _dataProvider.GetTotalStep(); i++)
                        {
                            UpdateProgress(i, _dataProvider.GetTotalStep());
                            string cellid1 = _dataProvider.GetTaAt(i).Keys.ToArray()[0];
                            string cellid2 = _dataProvider.GetTaAt(i + 3).Keys.ToArray()[0];
                            if (
                                !_dataProvider.GetTaAt(i).Keys.ToArray()[0].Equals(
                                    _dataProvider.GetTaAt(i + 3).Keys.ToArray()[0]) &&
                                !_option.GetIsUsingExternalData())
                            {
                                cellid2 = _dataProvider.GetTaAt(i + 3).Keys.ToArray()[1];
                            }
                            double[] bsPosition1 = _dataProvider.GetBsPosition(cellid1);
                            double[] bsPosition2 = _dataProvider.GetBsPosition(cellid2);
                            double x = bsPosition1[0];
                            double y = bsPosition1[1];
                            double deltaX = (bsPosition2[0] - bsPosition1[0])/4;
                            double deltaY = (bsPosition2[1] - bsPosition1[1])/4;
                            for (int j = 0; j < 4; j++)
                            {
                                posiblePostion[i + j].Add(new[] {x + deltaX*j, y + deltaY*j});
                            }
                        }
                    }

                    for (int i = 0; i < _dataProvider.GetTotalStep(); i++)
                    {
                        _resultPath.Add(GetAvgPosition(posiblePostion[i]));
                    }
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

        private double[] GetAvgPosition(List<double[]> positions)
        {
            int count = positions.Count;
            double sumX = 0;
            double sumY = 0;
            foreach (var t in positions)
            {
                sumX += t[0];
                sumY += t[1];
            }
            return new[] {sumX/count, sumY/count};
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