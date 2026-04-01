using LOMS_Employee_Shared;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOMS_Employee_DataAccess
{
    public class clsJobData
    {
        public static DataTable GetAllJobTitles()
        {
            DataTable dt = new DataTable();

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM JobTitles", connection))
            {
                connection.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    dt.Load(reader);
                }
            }

            return dt;
        }

        public static DataTable GetAllJobTitlesByDepartmentID(int DepartmentID)
        {
            DataTable dt = new DataTable();
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT JobTitleID, TitleName FROM JobTitles WHERE DepartmentID = @DeptID";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@DeptID", DepartmentID);
                    try
                    {
                        connection.Open();
                        dt.Load(command.ExecuteReader());
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
            return dt;
        }

        public static JobTitleDTO GetJobInfoByID(int JobTitleID)
        {
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT * FROM JobTitles WHERE JobTitleID = @JobTitleID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@JobTitleID", JobTitleID);

                    try
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new JobTitleDTO
                                {
                                    JobTitleID = (int)reader["JobTitleID"],
                                    TitleName = (string)reader["TitleName"],
                                    IsActive = (bool)reader["IsActive"]
                                };
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Logger.Log(ex); 
                        throw;
                    }
                }
            }
            return null;
        }

        public static JobTitleDTO GetJobInfoByName(string TitleName)
        {
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT * FROM JobTitles WHERE TitleName = @TitleName";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TitleName", TitleName);

                    try
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new JobTitleDTO
                                {
                                    JobTitleID = (int)reader["JobTitleID"],
                                    TitleName = (string)reader["TitleName"],
                                    IsActive = (bool)reader["IsActive"]
                                };
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Logger.Log(ex); 
                        throw;
                    }
                }
            }
            return null;
        }


    }


}
