using System.Collections.Generic;
using System.Linq;
using System.Windows.Threading;

namespace BSASimulator
{
    internal class Rss
    {
        private DataProvider _dataProvider;
        private Option _option;
        private List<double[]> _resultPath;

        public static Rss NewIncetance()
        {
            return new Rss();
        }

        public Rss SetDataProvider(DataProvider dataProvider)
        {
            _dataProvider = dataProvider;
            return this;
        }

        public Rss SetOption(Option option)
        {
            _option = option;
            return this;
        }

        public Rss StartAlgorithm()
        {
            _resultPath = new List<double[]>();
            switch (_option.GetSystemType())
            {
                case Option.SystemType.Cdma2000:
                    for (int i = 0; i < _dataProvider.GetTotalStep(); i++)
                    {
                        UpdateProgress(i, _dataProvider.GetTotalStep());
                        string cellId1 = _dataProvider.GetRssAt(i).Keys.ToArray()[0];
                        string cellId2 = _dataProvider.GetRssAt(i).Keys.ToArray()[1];
                        string cellId3 = _dataProvider.GetRssAt(i).Keys.ToArray()[2];
                        double[] bsposition1 = _dataProvider.GetBsPosition(cellId1);
                        double[] bsposition2 = _dataProvider.GetBsPosition(cellId2);
                        double[] bsposition3 = _dataProvider.GetBsPosition(cellId3);
                        double x1 = bsposition1[0];
                        double y1 = bsposition1[1];
                        double x2 = bsposition2[0];
                        double y2 = bsposition2[1];
                        double x3 = bsposition3[0];
                        double y3 = bsposition3[1];

                        double rss1 = _dataProvider.GetRssAt(i).Values.ToArray()[0];
                        double rss2 = _dataProvider.GetRssAt(i).Values.ToArray()[1];
                        double rss3 = _dataProvider.GetRssAt(i).Values.ToArray()[2];

                        double k1 = (rss2 + rss3)/(rss1 + rss2 + rss3)/3;
                        double k2 = (rss1 + rss3)/(rss1 + rss2 + rss3)/3;
                        double k3 = (rss2 + rss1)/(rss1 + rss2 + rss3)/3;

                        double respositionX = k1*x1 + k2*x2 + k3*x3;
                        double respositionY = k1*y1 + k2*y2 + k3*y3;

                        var resposition1 = new double[2];
                        resposition1[0] = respositionX;
                        resposition1[1] = respositionY;


                        _resultPath.Add(resposition1);
                    }
                    break;

                case Option.SystemType.Wcdma:
                    for (int i = 0; i < _dataProvider.GetTotalStep(); i++)
                    {
                        UpdateProgress(i, _dataProvider.GetTotalStep());
                        string cellId1 = _dataProvider.GetRssAt(i).Keys.ToArray()[0];
                        string cellId2 = _dataProvider.GetRssAt(i).Keys.ToArray()[1];
                        string cellId3 = _dataProvider.GetRssAt(i).Keys.ToArray()[2];
                        double[] bsposition1 = _dataProvider.GetBsPosition(cellId1);
                        double[] bsposition2 = _dataProvider.GetBsPosition(cellId2);
                        double[] bsposition3 = _dataProvider.GetBsPosition(cellId3);
                        double x1 = bsposition1[0];
                        double y1 = bsposition1[1];
                        double x2 = bsposition2[0];
                        double y2 = bsposition2[1];
                        double x3 = bsposition3[0];
                        double y3 = bsposition3[1];

                        double rss1 = _dataProvider.GetRssAt(i).Values.ToArray()[0];
                        double rss2 = _dataProvider.GetRssAt(i).Values.ToArray()[1];
                        double rss3 = _dataProvider.GetRssAt(i).Values.ToArray()[2];

                        double k1 = (rss2 + rss3)/(rss1 + rss2 + rss3)/3;
                        double k2 = (rss1 + rss3)/(rss1 + rss2 + rss3)/3;
                        double k3 = (rss2 + rss1)/(rss1 + rss2 + rss3)/3;

                        double respositionX = k1*x1 + k2*x2 + k3*x3;
                        double respositionY = k1*y1 + k2*y2 + k3*y3;

                        var resposition = new double[2];
                        resposition[0] = respositionX;
                        resposition[1] = respositionY;


                        _resultPath.Add(resposition);
                    }
                    break;
            }
            return this;
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