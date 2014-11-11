using System.Collections.Generic;
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
            //TODO And real alocation algorithm
            switch (_option.GetSystemType())
            {
                case Option.SystemType.Cdma2000:
                    break;
                case Option.SystemType.Wcdma:
                    break;
            }
            return this;
        }

        public List<double[]> GetResultPath()
        {
            _resultPath = new List<double[]>();
            //TODO return the allocation result
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