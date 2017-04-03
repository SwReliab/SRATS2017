using SRATS2017AddIn.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRATS2017AddIn.Commons
{
    public class AlreadyExistSheetName : Exception
    {

    }

    abstract public class IOOperation
    {
        protected static IOOperation self = null;

        public static IOOperation GetInstance()
        {
            return self;
        }

        abstract public Object[,] Read(string datarange);
        abstract public void MakeReport(PlotModel model);
    }

}
