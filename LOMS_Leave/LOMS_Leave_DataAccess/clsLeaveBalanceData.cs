using LOMS_Leave_Shared;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOMS_Leave_DataAccess
{
    public class clsLeaveBalanceData
    {
        public static bool InitializeBalances(int EmployeeID, int Year)
        {
            int rowsAffected = 0;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"
                    INSERT INTO EmployeeLeaveBalances 
                        (EmployeeID, LeaveTypeID, Year, EntitledDays, UsedDays)
                    SELECT 
                        @EmployeeID, LeaveTypeID, @Year, MaxDaysPerYear, 0
                    FROM LeaveTypes 
                    WHERE IsActive = 1;";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@EmployeeID", EmployeeID);
                command.Parameters.AddWithValue("@Year", Year);

                try
                {
                    connection.Open();
                    rowsAffected = command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw ex; 
                }
            }

            // إذا أضاف سطراً واحداً على الأقل، العملية ناجحة
            return (rowsAffected > 0);
        }

        public static DataTable GetEmployeeBalances(int EmployeeID, int Year)
        {
            DataTable dt = new DataTable();

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"
            SELECT 
                B.LeaveBalanceID,
                T.LeaveName, 
                B.Year, 
                B.EntitledDays, 
                B.UsedDays, 
                B.RemainingDays
            FROM EmployeeLeaveBalances B
            INNER JOIN LeaveTypes T ON B.LeaveTypeID = T.LeaveTypeID
            WHERE B.EmployeeID = @EmployeeID AND B.Year = @Year";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@EmployeeID", EmployeeID);
                command.Parameters.AddWithValue("@Year", Year);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        dt.Load(reader);
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return dt;
        }

        public static LeaveBalanceDTO GetBalanceByEmployeeAndType(int EmployeeID, int LeaveTypeID, int Year)
        {
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"SELECT * FROM EmployeeLeaveBalances 
                         WHERE EmployeeID = @EmployeeID AND LeaveTypeID = @LeaveTypeID AND Year = @Year";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@EmployeeID", EmployeeID);
                command.Parameters.AddWithValue("@LeaveTypeID", LeaveTypeID);
                command.Parameters.AddWithValue("@Year", Year);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read()) 
                    {
                        return new LeaveBalanceDTO
                        {
                            LeaveBalanceID = (int)reader["LeaveBalanceID"],
                            EmployeeID = (int)reader["EmployeeID"],
                            LeaveTypeID = (int)reader["LeaveTypeID"],
                            Year = (int)reader["Year"],
                            EntitledDays = (decimal)reader["EntitledDays"],
                            UsedDays = (decimal)reader["UsedDays"],
                            RemainingDays = (decimal)reader["RemainingDays"]
                        };
                    }
                    reader.Close();
                }
                catch (Exception)
                {
                    throw;
                }
            }
            return null; 
        }

        public static LeaveBalanceDTO GetLeaveBalanceByID(int LeaveBalanceID)
        {
            LeaveBalanceDTO dto = null;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT * FROM EmployeeLeaveBalances WHERE LeaveBalanceID = @LeaveBalanceID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@LeaveBalanceID", LeaveBalanceID);
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new LeaveBalanceDTO
                            {
                                LeaveBalanceID = (int)reader["LeaveBalanceID"],
                                EmployeeID = (int)reader["EmployeeID"],
                                LeaveTypeID = (int)reader["LeaveTypeID"],
                                Year = (int)reader["Year"],
                                EntitledDays = (decimal)reader["EntitledDays"],
                                UsedDays = (decimal)reader["UsedDays"],
                                RemainingDays = (decimal)reader["RemainingDays"]
                            };
                        }
                    }
                }
            }
            return dto;
        }

        public static bool UpdateUsedDays(int EmployeeID, int LeaveTypeID, int Year, decimal DaysCount)
        {
            int rowsAffected = 0;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"UPDATE EmployeeLeaveBalances
                         SET UsedDays = UsedDays + @DaysCount
                         WHERE EmployeeID = @EmployeeID AND LeaveTypeID = @LeaveTypeID AND Year = @Year";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@EmployeeID", EmployeeID);
                command.Parameters.AddWithValue("@LeaveTypeID", LeaveTypeID);
                command.Parameters.AddWithValue("@Year", Year);
                command.Parameters.AddWithValue("@DaysCount", DaysCount); 

                try
                {
                    connection.Open();
                    rowsAffected = command.ExecuteNonQuery();
                }
                catch (Exception)
                {
                    throw; 
                }
            }
            return (rowsAffected > 0);
        }

        public static bool UpdateBalance(int LeaveBalanceID, decimal EntitledDays, decimal UsedDays)
        {
            int rowsAffected = 0;
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"UPDATE EmployeeLeaveBalances 
                         SET EntitledDays = @EntitledDays, 
                             UsedDays = @UsedDays 
                         WHERE LeaveBalanceID = @LeaveBalanceID";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@EntitledDays", EntitledDays);
                command.Parameters.AddWithValue("@UsedDays", UsedDays);
                command.Parameters.AddWithValue("@LeaveBalanceID", LeaveBalanceID);

                connection.Open();
                rowsAffected = command.ExecuteNonQuery();
            }
            return (rowsAffected > 0);
        }
    }
}
