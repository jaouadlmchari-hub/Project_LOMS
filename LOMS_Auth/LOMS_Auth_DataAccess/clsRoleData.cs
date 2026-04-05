using LOMS_Auth_Shared;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOMS_Auth_DataAccess
{
    public  class clsRoleData
    {
        public static DataTable GetAllRoles()
        {
            DataTable dt = new DataTable();
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT * FROM Roles";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    try
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows) dt.Load(reader);
                        }
                    }
                    catch { /* Log error here */ }
                }
            }
            return dt;
        }

        public static List<string> GetRoleNamesByIDs(List<int> RoleIDs)
        {
            List<string> RoleNames = new List<string>();

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string ids = string.Join(",", RoleIDs);
                string query = $"SELECT RoleName FROM Roles WHERE RoleID IN ({ids})";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            RoleNames.Add(reader["RoleName"].ToString());
                        }
                    }
                }
            }
            return RoleNames;
        }
    }
}
