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
    public class clsPublicHolidayData
    {
        public static bool GetPublicHolidayInfoByID(int HolidayID, ref PublicHolidaysDTO HolidayDTO)
        {
            bool isFound = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = "SELECT * FROM PublicHolidays WHERE HolidayID = @HolidayID";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@HolidayID", HolidayID);

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            isFound = true;
                            HolidayDTO = new PublicHolidaysDTO
                            {
                                HolidayID = (int)reader["HolidayID"],
                                HolidayName = (string)reader["HolidayName"],
                                HolidayDate = (DateTime)reader["HolidayDate"],
                                IsRepeatedAnnually = (bool)reader["IsRepeatedAnnually"]
                            };


                        }
                    }
                }
            }
            catch { isFound = false; }
            return isFound;
        }

        public static int AddNewPublicHoliday(PublicHolidaysDTO HolidayDTO)
        {
            int HolidayID = -1;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @"INSERT INTO PublicHolidays (HolidayName, HolidayDate, IsRepeatedAnnually)
                                     VALUES (@HolidayName, @HolidayDate, @IsRepeatedAnnually);
                                     SELECT SCOPE_IDENTITY();";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@HolidayName", HolidayDTO.HolidayName);
                    command.Parameters.AddWithValue("@HolidayDate", HolidayDTO.HolidayDate);
                    command.Parameters.AddWithValue("@IsRepeatedAnnually", HolidayDTO.IsRepeatedAnnually);

                    connection.Open();
                    object result = command.ExecuteScalar();
                    if (result != null && int.TryParse(result.ToString(), out int insertedID))
                    {
                        HolidayID = insertedID;
                    }
                }
            }
            catch { /* Log error */ }
            return HolidayID;
        }

        public static bool UpdatePublicHoliday(PublicHolidaysDTO HolidayDTO)
        {
            int rowsAffected = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @"UPDATE PublicHolidays 
                                    SET HolidayName = @HolidayName, 
                                        HolidayDate = @HolidayDate, 
                                        IsRepeatedAnnually = @IsRepeatedAnnually
                                    WHERE HolidayID = @HolidayID";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@HolidayID", HolidayDTO.HolidayID);
                    command.Parameters.AddWithValue("@HolidayName", HolidayDTO.HolidayName);
                    command.Parameters.AddWithValue("@HolidayDate", HolidayDTO.HolidayDate);
                    command.Parameters.AddWithValue("@IsRepeatedAnnually", HolidayDTO.IsRepeatedAnnually);

                    connection.Open();
                    rowsAffected = command.ExecuteNonQuery();
                }
            }
            catch { return false; }
            return (rowsAffected > 0);
        }

        public static bool DeletePublicHoliday(int HolidayID)
        {
            int rowsAffected = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = "DELETE FROM PublicHolidays WHERE HolidayID = @HolidayID";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@HolidayID", HolidayID);

                    connection.Open();
                    rowsAffected = command.ExecuteNonQuery();
                }
            }
            catch { return false; }
            return (rowsAffected > 0);
        }

        public static DataTable GetAllPublicHolidays()
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = "SELECT * FROM PublicHolidays ORDER BY HolidayDate DESC";
                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        dt.Load(reader);
                    }
                }
            }
            catch { }
            return dt;
        }

        public static bool IsPublicHoliday(DateTime Date)
        {
            bool isFound = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @"SELECT TOP 1 Found=1 FROM PublicHolidays 
                                     WHERE CAST(HolidayDate AS DATE) = @Date 
                                     OR (IsRepeatedAnnually = 1 AND DAY(HolidayDate) = DAY(@Date) AND MONTH(HolidayDate) = MONTH(@Date))";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Date", Date.Date);

                    connection.Open();
                    object result = command.ExecuteScalar();
                    isFound = (result != null);
                }
            }
            catch { }
            return isFound;
        }
    }
}
