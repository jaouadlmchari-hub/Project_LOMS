using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOMS_Employee_DataAccess
{
    public static class clsDataAccessSettings
    {
        public static string ConnectionString { get; set; }

        public delegate void DataErrorEventHandler(string message, Exception ex);

        public static event DataErrorEventHandler OnDataError;

        public static void RaiseError(string message, Exception ex)
        {
            OnDataError?.Invoke(message, ex);
        }
    }
}
