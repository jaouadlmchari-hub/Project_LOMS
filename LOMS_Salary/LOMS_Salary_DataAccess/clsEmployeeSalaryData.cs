using LOMS_Salary_Shared;
using Microsoft.Data.SqlClient;
using System;
using System.Data;

namespace LOMS_Salary_DataAccess
{
    public static class clsEmployeeSalaryData
    {
        public static EmployeeSalaryDTO GetSalaryByEmployeeID(int employeeID)
        {
            EmployeeSalaryDTO salaryDTO = null;

            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand("SP_GetSalaryByEmployeeID", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@EmployeeID", employeeID);

                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                salaryDTO = new EmployeeSalaryDTO
                                {
                                    SalaryID = (int)reader["SalaryID"],
                                    EmployeeID = (int)reader["EmployeeID"],
                                    Salary = (decimal)reader["Salary"],
                                    EffectiveDate = (DateTime)reader["EffectiveDate"],
                                    CreatedByUserID = (int)reader["CreatedByUserID"],
                                    CreatedAt = reader["CreatedAt"] != DBNull.Value ? (DateTime)reader["CreatedAt"] : null
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log exception (ex.Message)
            }

            return salaryDTO;
        }

        public static EmployeeSalaryDTO GetSalaryByNationalNo(string nationalNo)
        {
            EmployeeSalaryDTO salaryDTO = null;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    // On suppose ici une jointure pour trouver le salaire via le NationalNo
                    string query = @"SELECT s.* FROM EmployeeSalaries s 
                             JOIN Employees e ON s.EmployeeID = e.EmployeeID 
                             WHERE e.NationalNo = @NationalNo";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@NationalNo", nationalNo);
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                salaryDTO = new EmployeeSalaryDTO
                                {
                                    SalaryID = (int)reader["SalaryID"],
                                    EmployeeID = (int)reader["EmployeeID"],
                                    Salary = (decimal)reader["Salary"],
                                    EffectiveDate = (DateTime)reader["EffectiveDate"]
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception) { /* Log error */ }
            return salaryDTO;
        }
    }
}
