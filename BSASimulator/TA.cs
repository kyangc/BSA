using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSASimulator
{
    class Ta
    {
        private DataProvider _dataProvider;
        private List<double[]> _resultPath;

        public Ta NewIncetance()
        {
            return new Ta();
        }

        public Ta SetDataProvider(DataProvider dataProvider)
        {
            _dataProvider = dataProvider;
            return this;
        }

        public Ta StartAlgorithm()
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
