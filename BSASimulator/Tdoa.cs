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

        public Tdoa StartAlgorithm()
        {
            //TODO And real alocation algorithm
            return this;
        }

        public List<double[]> GetResultPath()
        {
            //TODO return the allocation result
            return _resultPath;
        }
    }
}