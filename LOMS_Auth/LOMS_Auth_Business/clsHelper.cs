using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOMS_Auth_Business
{
    public static class clsHelper
    {
        /// <summary>
        /// Convertit une DataTable en une liste de dictionnaires sérialisables par l'API.
        /// Cela règle l'erreur de sérialisation "System.Type".
        /// </summary>
        public static List<Dictionary<string, object>> DataTableToList(DataTable dt)
        {
            var list = new List<Dictionary<string, object>>();

            if (dt == null) return list;

            foreach (DataRow row in dt.Rows)
            {
                var dict = new Dictionary<string, object>();
                foreach (DataColumn col in dt.Columns)
                {
                    // On gère les valeurs NULL de la base de données
                    dict[col.ColumnName] = (row[col] == DBNull.Value) ? null : row[col];
                }
                list.Add(dict);
            }

            return list;
        }
    }

}
