using System.Collections.Generic;
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