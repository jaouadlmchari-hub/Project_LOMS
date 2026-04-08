using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOMS_Leave_DataAccess
{
    public  class clsDataAccessHelper
    {
        public static object EnsureValue(object value)
        {
            return value ?? DBNull.Value;
        }
    }
}
