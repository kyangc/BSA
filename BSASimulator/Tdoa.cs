using System.Collections.Generic;

namespace BSASimulator
{
    internal class Tdoa
    {
        private DataProvider _dataProvider;
        private List<double[]> _resultPath;

        public Tdoa NewIncetance()
        {
            return new Tdoa();
        }

        public Tdoa SetDataProvider(DataProvider dataProvider)
        {
            _dataProvider = dataProvider;
            return this;
        }

        public Tdoa StartAlgorithm(Option.SystemType systemType)
        {
            //TODO And real alocation algorithm
            switch (systemType)
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