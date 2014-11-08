using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSASimulator
{
    class TaAoa
    {
        private DataProvider _dataProvider;
        private List<double[]> _resultPath;

        public TaAoa NewIncetance()
        {
            return new TaAoa();
        }

        public TaAoa SetDataProvider(DataProvider dataProvider)
        {
            _dataProvider = dataProvider;
            return this;
        }

        public TaAoa StartAlgorithm()
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
