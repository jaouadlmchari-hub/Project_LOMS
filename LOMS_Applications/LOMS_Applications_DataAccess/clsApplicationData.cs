using LOMS_Applications_Shared;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace LOMS_Applications_DataAccess
{
    public class clsApplicationData
    {
        public static ApplicationDTO GetApplicationInfoByID(int ApplicationID)
        {

            ApplicationDTO dto = new ApplicationDTO();

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string query = "SELECT * FROM Application WHERE ApplicationID = @ApplicationID";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@ApplicationID", ApplicationID);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {

                        return new ApplicationDTO
                        {
                            ApplicationID = (int)reader["ApplicationID"],
                            EmployeeID = (int)reader["EmployeeID"],
                            ApplicationDate = (DateTime)reader["ApplicationDate"],
                            ApplicationTypeID = (int)reader["ApplicationTypeID"],
                            Status = (string)reader["Status"],
                            LastStatusDate = (DateTime)reader["LastStatusDate"],
                            CreatedByUserID = (int)reader["CreatedByUserID"],
                            Notes = (reader["Notes"] != DBNull.Value) ? (string)reader["Notes"] : "",
                            ActionByUserID = (reader["ActionByUserID"] != DBNull.Value) ? (int)reader["ActionByUserID"] : -1
                        };



                    }

                    reader.Close();


                }
                catch (Exception ex)
                {
                    throw;
                }
            }

               
          

            return dto;
        }

        public static DataTable GetAllApplications()
        {

            DataTable dt = new DataTable();
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string query = "select * from ApplicationsList_View order by ApplicationDate desc";

            SqlCommand command = new SqlCommand(query, connection);

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
                // Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                connection.Close();
            }

            return dt;

        }

        public static int AddNewAppliccation(ApplicationDTO dto)
        {
            int ApplicationID = -1;

            using(SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"insert into Application (EmployeeID, ApplicationDate,
                             Status, LastStatusDate, CreatedByUserID, ApplicationTypeID,Notes)
                          Values(@EmployeeID, @ApplicationDate, @Status, 
                                @LastStatusDate,@CreatedByUserID, @ApplicationTypeID, @Notes)
                               select scope_identity()";
                using(SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@EmployeeID", dto.EmployeeID);
                    command.Parameters.AddWithValue("@ApplicationDate", dto.ApplicationDate);
                    command.Parameters.AddWithValue("@Status", dto.Status);
                    command.Parameters.AddWithValue("@LastStatusDate", dto.LastStatusDate);
                    command.Parameters.AddWithValue("@CreatedByUserID", dto.CreatedByUserID);
                    command.Parameters.AddWithValue("@ApplicationTypeID", dto.ApplicationTypeID);
                    command.Parameters.AddWithValue("@Notes", (object)dto.Notes ?? DBNull.Value);


                    try
                    {
                        connection.Open();

                        object result = command.ExecuteScalar();

                        if ( result != null && int.TryParse(result.ToString() ,  out int insertedID))
                        {
                            ApplicationID = insertedID;
                        }
                    }
                    catch (Exception ) 
                    {
                        throw;
                    }
                }
            }
            return ApplicationID;
        }

        public static bool UpdateApplication(ApplicationDTO dto)
        {
            int rowsAffected = 0;
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"Update Application
                         set EmployeeID = @EmployeeID,
                             ApplicationDate = @ApplicationDate,
                             ApplicationTypeID = @ApplicationTypeID,
                             Status = @Status, 
                             LastStatusDate = @LastStatusDate,
                             CreatedByUserID = @CreatedByUserID,
                             ActionByUserID = @ActionByUserID, -- الحقل الجديد
                             Notes = @Notes
                         where ApplicationID = @ApplicationID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ApplicationID", dto.ApplicationID);
                    command.Parameters.AddWithValue("@EmployeeID", dto.EmployeeID);
                    command.Parameters.AddWithValue("@ApplicationDate", dto.ApplicationDate);
                    command.Parameters.AddWithValue("@Status", dto.Status);
                    command.Parameters.AddWithValue("@LastStatusDate", dto.LastStatusDate);
                    command.Parameters.AddWithValue("@CreatedByUserID", dto.CreatedByUserID);
                    command.Parameters.AddWithValue("@ApplicationTypeID", dto.ApplicationTypeID);

                    if (dto.ActionByUserID != -1)
                        command.Parameters.AddWithValue("@ActionByUserID", dto.ActionByUserID);
                    else
                        command.Parameters.AddWithValue("@ActionByUserID", DBNull.Value);

                    command.Parameters.AddWithValue("@Notes", (object)dto.Notes ?? DBNull.Value);

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

        public static bool DeleteApplication(int ApplicationID)
        {

            int rowsAffected = 0;

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string query = @"Delete Application
                                where ApplicationID = @ApplicationID";

            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@ApplicationID", ApplicationID);

            try
            {
                connection.Open();

                rowsAffected = command.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                // Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {

                connection.Close();

            }

            return (rowsAffected > 0);

        }

        public static bool IsApplicationExist(int ApplicationID)
        {
            bool isFound = false;

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string query = "SELECT Found=1 FROM Application WHERE ApplicationID = @ApplicationID";

            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@ApplicationID", ApplicationID);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                isFound = reader.HasRows;

                reader.Close();
            }
            catch (Exception ex)
            {
                //Console.WriteLine("Error: " + ex.Message);
                isFound = false;
            }
            finally
            {
                connection.Close();
            }

            return isFound;
        }

        public static bool UpdateStatus(int ApplicationID, short NewStatus)
        {

            int rowsAffected = 0;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string query = @"Update  Application 
                            set 
                                Status = @NewStatus, 
                                LastStatusDate = @LastStatusDate
                            where ApplicationID=@ApplicationID;";

            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@ApplicationID", ApplicationID);
            command.Parameters.AddWithValue("@NewStatus", NewStatus);
            command.Parameters.AddWithValue("LastStatusDate", DateTime.Now);


            try
            {
                connection.Open();
                rowsAffected = command.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                //Console.WriteLine("Error: " + ex.Message);
                return false;
            }

            finally
            {
                connection.Close();
            }

            return (rowsAffected > 0);
        }



    }
}
