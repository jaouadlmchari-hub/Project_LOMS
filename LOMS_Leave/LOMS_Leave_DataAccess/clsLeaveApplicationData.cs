using LOMS_Leave_Shared;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;

namespace LOMS_Leave_DataAccess
{
    public class clsLeaveApplicationData
    {
        public static LeaveApplicationDTO GetLeaveApplicationInfoByApplicationID(int ApplicationID)
        {
            LeaveApplicationDTO dto = null;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT * FROM LeaveApplications WHERE ApplicationID = @ApplicationID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ApplicationID", ApplicationID);

                    try
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                dto = new LeaveApplicationDTO
                                {
                                    LeaveApplicationID = (int)reader["LeaveApplicationID"],
                                    ApplicationID = (int)reader["ApplicationID"],
                                    LeaveTypeID = (int)reader["LeaveTypeID"],
                                    StartDate = (DateTime)reader["FromDate"],
                                    EndDate = (DateTime)reader["ToDate"],
                                    NumberOfDays = (decimal)reader["NumberOfDays"],
                                    Reason = reader["Reason"] == DBNull.Value ? "" : (string)reader["Reason"]
                                };
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        clsDataAccessSettings.RaiseError("Error in GetLeaveApplicationInfoByApplicationID", ex);
                    }
                }
            }
            return dto;
        }

        public static int AddNewLeaveApplication(LeaveApplicationDTO dto)
        {
            int insertedID = -1;
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"INSERT INTO LeaveApplications (ApplicationID, LeaveTypeID, FromDate, ToDate, NumberOfDays, Reason)
                                 VALUES (@ApplicationID, @LeaveTypeID, @FromDate, @ToDate, @NumberOfDays, @Reason);
                                 SELECT SCOPE_IDENTITY();";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ApplicationID", dto.ApplicationID);
                    command.Parameters.AddWithValue("@LeaveTypeID", dto.LeaveTypeID);
                    command.Parameters.AddWithValue("@FromDate", dto.StartDate);
                    command.Parameters.AddWithValue("@ToDate", dto.EndDate);
                    command.Parameters.AddWithValue("@NumberOfDays", dto.NumberOfDays);
                    command.Parameters.AddWithValue("@Reason", (object)dto.Reason ?? DBNull.Value);

                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();
                        if (result != null && int.TryParse(result.ToString(), out int id))
                        {
                            insertedID = id;
                        }
                    }
                    catch (Exception ex)
                    {
                        clsDataAccessSettings.RaiseError("Error in AddNewLeaveApplication", ex);
                    }
                }
            }
            return insertedID;
        }

        public static bool UpdateLeaveApplication(LeaveApplicationDTO dto)
        {
            int rowsAffected = 0;
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"UPDATE LeaveApplications 
                                 SET LeaveTypeID = @LeaveTypeID, 
                                     FromDate = @FromDate, 
                                     ToDate = @ToDate, 
                                     NumberOfDays = @NumberOfDays, 
                                     Reason = @Reason
                                 WHERE LeaveApplicationID = @LeaveApplicationID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@LeaveApplicationID", dto.LeaveApplicationID);
                    command.Parameters.AddWithValue("@LeaveTypeID", dto.LeaveTypeID);
                    command.Parameters.AddWithValue("@FromDate", dto.StartDate);
                    command.Parameters.AddWithValue("@ToDate", dto.EndDate);
                    command.Parameters.AddWithValue("@NumberOfDays", dto.NumberOfDays);
                    command.Parameters.AddWithValue("@Reason", (object)dto.Reason ?? DBNull.Value);

                    try
                    {
                        connection.Open();
                        rowsAffected = command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        clsDataAccessSettings.RaiseError("Error in UpdateLeaveApplication", ex);
                    }
                }
            }
            return rowsAffected > 0;
        }

        public static DataTable GetLeaveHistoryLocally(int EmployeeID, int? Year = null)
        {
            DataTable dt = new DataTable();

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
               
                string query = @"SELECT LA.ApplicationID, LA.LeaveApplicationID, LT.LeaveName, 
                                        LA.FromDate, LA.ToDate, LA.NumberOfDays, LA.Reason
                                 FROM LeaveApplications LA
                                 INNER JOIN LeaveTypes LT ON LA.LeaveTypeID = LT.LeaveTypeID
                                 WHERE LA.ApplicationID IN (
                                     /* Note: Normalement ici on filtre par EmployeeID, mais comme EmployeeID 
                                        est dans la table Applications (autre DB), on passera une liste d'IDs 
                                        depuis la BLL ou on filtrera ici si on a dupliqué l'EmployeeID */
                                     SELECT ApplicationID FROM LeaveApplications -- Filtre simplifié
                                 )
                                 AND (@Year IS NULL OR YEAR(LA.FromDate) = @Year)
                                 ORDER BY LA.FromDate DESC";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Year", (object)Year ?? DBNull.Value);

                    try
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows) dt.Load(reader);
                        }
                    }
                    catch (Exception ex)
                    {
                        clsDataAccessSettings.RaiseError("Error in GetLeaveHistoryLocally", ex);
                    }
                }
            }
            return dt;
        }

        public static bool DeleteLeaveApplication(int LeaveApplicationID)
        {
            int rowsAffected = 0;
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "DELETE FROM LeaveApplications WHERE LeaveApplicationID = @LeaveApplicationID";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@LeaveApplicationID", LeaveApplicationID);
                    try
                    {
                        connection.Open();
                        rowsAffected = command.ExecuteNonQuery();
                    }
                    catch { return false; }
                }
            }
            return rowsAffected > 0;
        }
    }
}