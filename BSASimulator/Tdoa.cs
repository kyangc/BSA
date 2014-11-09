using System.Collections.Generic;

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