using LOMS_Leave_Shared; 
using Microsoft.Data.SqlClient;
using System;
using System.Data;

namespace LOMS_Leave_DataAccess
{
    public class clsApplicationData
    {

        public static ApplicationDTO GetApplicationInfoByID(int ApplicationID)
        {
            ApplicationDTO appDTO = null;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                // On interroge la table miroir/cache que Kafka remplit
                string query = "SELECT * FROM BaseApplications WHERE ApplicationID = @ApplicationID";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ApplicationID", ApplicationID);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        appDTO = new ApplicationDTO
                        {
                            ApplicationID = (int)reader["ApplicationID"],
                            EmployeeID = (int)reader["EmployeeID"],
                            ApplicationDate = (DateTime)reader["ApplicationDate"],
                            ApplicationTypeID = (int)reader["ApplicationTypeID"],
                            Status = reader["Status"].ToString(),
                            LastStatusDate =  reader["LastStatusDate"] != DBNull.Value ? (DateTime?)reader["LastStatusDate"]: null,
                            CreatedByUserID = (int)reader["CreatedByUserID"],
                            Notes = reader["Notes"]?.ToString()
                        };
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    // Log l'erreur ici
                    Console.WriteLine("Erreur DataAccess: " + ex.Message);
                }
            }

            return appDTO;
        }

        public static bool SaveBaseApplication(ApplicationDTO appDTO)
        {
            int rowsAffected = 0;
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                // Utilisation de IF NOT EXISTS pour éviter les erreurs si Kafka renvoie le même message
                string query = @"
                    IF NOT EXISTS (SELECT 1 FROM BaseApplications WHERE ApplicationID = @ApplicationID)
                    BEGIN
                        INSERT INTO BaseApplications 
                        (ApplicationID, EmployeeID, ApplicationDate, Status, ApplicationTypeID, CreatedByUserID, Notes)
                        VALUES 
                        (@ApplicationID, @EmployeeID, @ApplicationDate, @Status, @ApplicationTypeID, @CreatedByUserID, @Notes)
                    END
                    ELSE
                    BEGIN
                        UPDATE BaseApplications SET 
                            Status = @Status, 
                            Notes = @Notes 
                        WHERE ApplicationID = @ApplicationID
                    END";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ApplicationID", appDTO.ApplicationID);
                command.Parameters.AddWithValue("@EmployeeID", appDTO.EmployeeID);
                command.Parameters.AddWithValue("@ApplicationDate", appDTO.ApplicationDate);
                command.Parameters.AddWithValue("@Status", appDTO.Status);
                command.Parameters.AddWithValue("@ApplicationTypeID", appDTO.ApplicationTypeID);
                command.Parameters.AddWithValue("@CreatedByUserID", appDTO.CreatedByUserID);
                command.Parameters.AddWithValue("@Notes", (object)appDTO.Notes ?? DBNull.Value);

                try
                {
                    connection.Open();
                    rowsAffected = command.ExecuteNonQuery();
                }
                catch { return false; }
            }
            return rowsAffected > 0;
        }
    }
}