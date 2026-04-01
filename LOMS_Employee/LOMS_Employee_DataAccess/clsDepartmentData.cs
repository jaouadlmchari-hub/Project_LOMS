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
    public class clsDepartmentData
    {
        public static DataTable GetAllDepartment()
        {
            DataTable dt = new DataTable();

            string query = @"
                         SELECT 
                             Departments.DepartmentID, 
                             Departments.DepartmentName, 
                             Departments.Description, 
                             COUNT(Employees.EmployeeID) AS NumberOfEmployees,
                             Departments.IsActive
                         FROM Departments
                         LEFT JOIN Employees ON Departments.DepartmentID = Employees.DepartmentID
                         GROUP BY Departments.DepartmentID, Departments.DepartmentName,
                                  Departments.Description,Departments.IsActive";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, connection))
            {
                try
                {
                    connection.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        dt.Load(reader);
                    }
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            return dt;
        }

        public static DepartmentDTO GetDepartmentInfoByID(int departmentID)
        {

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT * FROM Departments WHERE DepartmentID = @DepartmentID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@DepartmentID", departmentID);

                    try
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (!reader.Read())
                                return null;

                            return new DepartmentDTO
                                {
                                DepartmentID = (int)reader["DepartmentID"],
                                DepartmentName = (string)reader["DepartmentName"],
                                Description = reader["Description"] == DBNull.Value ? "" : (string)reader["Description"],
                                IsActive = (bool)reader["IsActive"]
                            };
                                   
                                
                            }
                        
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                }
            }
        }

        public static DepartmentDTO GetDepartmentInfoByName(string DepartmentName)
        {

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT * FROM Departments WHERE DepartmentName = @DepartmentName";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@DepartmentName", DepartmentName);

                    try
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (!reader.Read())
                                return null;

                            return new DepartmentDTO
                            {
                                DepartmentID = (int)reader["DepartmentID"],
                                DepartmentName = (string)reader["DepartmentName"],
                                Description = reader["Description"] == DBNull.Value ? "" : (string)reader["Description"],
                                IsActive = (bool)reader["IsActive"]
                            };


                        }

                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                }
            }
        }

        public static int AddNewDepartment(DepartmentDTO department)
        {
            int DepartmentID = -1;
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"INSERT INTO Departments (DepartmentName, Description, IsActive) 
                         VALUES (@DepartmentName, @Description, @IsActive); 
                         SELECT SCOPE_IDENTITY();";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@DepartmentName", department.DepartmentName);
                    command.Parameters.AddWithValue("@Description", (object)department.Description ?? DBNull.Value);
                    command.Parameters.AddWithValue("@IsActive", department.IsActive);

                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();

                        if (result != null && int.TryParse(result.ToString(), out int insertedID))
                        {
                            DepartmentID = insertedID;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                }
            }
            return DepartmentID;
        }

        public static bool UpdateDepartment(DepartmentDTO department)
        {
            int rowsAffected = 0;
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"UPDATE Departments 
                         SET DepartmentName = @DepartmentName, 
                             Description = @Description, 
                             IsActive = @IsActive 
                         WHERE DepartmentID = @DepartmentID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@DepartmentID", department.DepartmentID);
                    command.Parameters.AddWithValue("@DepartmentName", department.DepartmentName);
                    command.Parameters.AddWithValue("@Description", clsDataAccessHelper.EnsureValue(department.Description));
                    command.Parameters.AddWithValue("@IsActive", department.IsActive);

                    try
                    {
                        connection.Open();
                        rowsAffected = command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        return false;
                    }
                }
            }

            return (rowsAffected > 0);
        }

        public static bool UpdateStatus(int DepartmentID, bool IsActive)
        {
            int rowsAffected = 0;
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "UPDATE Departments SET IsActive = @IsActive WHERE DepartmentID = @DepartmentID";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@DepartmentID", DepartmentID);
                    command.Parameters.AddWithValue("@IsActive", IsActive);
                    try
                    {
                        connection.Open();
                        rowsAffected = command.ExecuteNonQuery();
                    }
                    catch { return false; }
                }
            }
            return (rowsAffected > 0);
        }

        public static DataTable GetDepartmentSummary()
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    using(SqlCommand cmd = new SqlCommand("select * from View_DepartmentLeaveSummary", connection))
                    {
                        connection.Open();
                        using(SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                dt.Load(reader);
                            }
                            return dt;
                        }
                    }
                }
            }
            catch (Exception ex) 
            {
                
                clsDataAccessSettings.RaiseError("Error in GetDepartmentSummary", ex);

                return null; 
            }
        }
    }
}
