using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSASimulator
{
    class Rss
    {
        private DataProvider _dataProvider;
        private List<double[]> _resultPath; 

        public Rss NewIncetance()
        {
            return new Rss();
        }

        public Rss SetDataProvider(DataProvider dataProvider)
        {
            _dataProvider = dataProvider;
            return this;
        }

        public Rss StartAlgorithm(Option.SystemType systemType)
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
