using LOMS_Leave_Shared;
using System.Data;
using Microsoft.Data.SqlClient;


namespace LOMS_Leave_DataAccess
{
    public class clsLeaveTypeData
    {
        public static DataTable GettAllLeaveTypes()
        {
            DataTable dt = new DataTable();
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand("SELECT * FROM LeaveTypes", connection))
                {
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
                    catch { throw; }
                }
            }
            return dt;
        }

        public static int AddNewLeaveType(LeaveTypeDTO dTO)
        {
            int LeaveTypeID = -1;
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"INSERT INTO LeaveTypes (LeaveName, IsPaid, MaxDaysPerYear, RequiresDocument, IsActive)
                                 VALUES (@LeaveName, @IsPaid, @MaxDaysPerYear, @RequiresDocument, @IsActive);
                                 SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@LeaveName", dTO.LeaveName);
                    cmd.Parameters.AddWithValue("@IsPaid", dTO.IsPaid);
                    cmd.Parameters.AddWithValue("@MaxDaysPerYear", dTO.MaxDaysPerYear);
                    cmd.Parameters.AddWithValue("@RequiresDocument", dTO.RequiresDocument);
                    cmd.Parameters.AddWithValue("@IsActive", dTO.IsActive);

                    try
                    {
                        connection.Open();
                        object result = cmd.ExecuteScalar();
                        if (result != null && int.TryParse(result.ToString(), out int insertedID))
                        {
                            LeaveTypeID = insertedID;
                        }
                    }
                    catch { throw; }
                }
            }
            return LeaveTypeID;
        }

        public static bool UpdateLeaveType(LeaveTypeDTO dTO)
        {
            int rowsAffected = 0;
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"UPDATE LeaveTypes 
                                 SET LeaveName = @LeaveName, 
                                     IsPaid = @IsPaid, 
                                     MaxDaysPerYear = @MaxDaysPerYear, 
                                     RequiresDocument = @RequiresDocument, 
                                     IsActive = @IsActive
                                 WHERE LeaveTypeID = @LeaveTypeID";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@LeaveTypeID", dTO.LeaveTypeID);
                    cmd.Parameters.AddWithValue("@LeaveName", dTO.LeaveName);
                    cmd.Parameters.AddWithValue("@IsPaid", dTO.IsPaid);
                    cmd.Parameters.AddWithValue("@MaxDaysPerYear", dTO.MaxDaysPerYear);
                    cmd.Parameters.AddWithValue("@RequiresDocument", dTO.RequiresDocument);
                    cmd.Parameters.AddWithValue("@IsActive", dTO.IsActive);

                    try
                    {
                        connection.Open();
                        rowsAffected = cmd.ExecuteNonQuery();
                    }
                    catch { throw; }
                }
            }
            return (rowsAffected > 0);
        }

        public static LeaveTypeDTO GetLeaveTypeByID(int LeaveTypeID)
        {
            LeaveTypeDTO dTO = null;
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT * FROM LeaveTypes WHERE LeaveTypeID = @LeaveTypeID";
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@LeaveTypeID", LeaveTypeID);
                    try
                    {
                        connection.Open();
                        SqlDataReader reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            dTO = new LeaveTypeDTO
                            {
                                LeaveTypeID = (int)reader["LeaveTypeID"],
                                LeaveName = (string)reader["LeaveName"],
                                IsPaid = (bool)reader["IsPaid"],
                                MaxDaysPerYear = Convert.ToInt16(reader["MaxDaysPerYear"]),
                                RequiresDocument = (bool)reader["RequiresDocument"],
                                IsActive = (bool)reader["IsActive"]
                            };
                        }
                        reader.Close();
                    }
                    catch { throw; }
                }
            }
            return dTO;
        }

        public static bool DeleteLeaveType(int LeaveTypeID)
        {
            int rowsAffected = 0;
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "DELETE FROM LeaveTypes WHERE LeaveTypeID = @LeaveTypeID";
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@LeaveTypeID", LeaveTypeID);
                    try
                    {
                        connection.Open();
                        rowsAffected = cmd.ExecuteNonQuery();
                    }
                    catch { throw; }
                }
            }
            return (rowsAffected > 0);
        }
    }
}
