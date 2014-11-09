using System.Collections.Generic;

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
                case Option.SystemType.CDMA2000:
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
    }
}