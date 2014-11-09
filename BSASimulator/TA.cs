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

        public Ta StartAlgorithm(Option.SystemType systemType)
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
