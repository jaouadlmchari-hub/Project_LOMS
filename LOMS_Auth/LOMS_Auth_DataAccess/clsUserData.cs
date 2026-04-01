using LOMS_Auth_Shared;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOMS_Auth_DataAccess
{
    public class clsUserData
    {
        public static DataTable GetAllUsers()
        {
            DataTable dt = new DataTable();
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT UserID, EmployeeID, UserName, IsActive FROM Users";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    try
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            dt.Load(reader);
                        }
                    }
                    catch { throw; }
                }
            }
            return dt;
        }
        public static int AddNewUser(UserDTO DTO)
        {
            int UserID = -1;

            using(SqlConnection  connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"Insert Into Users(UserName ,PasswordHash, EmployeeID ,IsActive)
                               Values(@UserName, @PasswordHash,@EmployeeID,@IsActive)
                               SELECT SCOPE_IDENTITY();";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@EmployeeID", DTO.EmployeeID);
                    command.Parameters.AddWithValue("@UserName", DTO.UserName);
                    command.Parameters.AddWithValue("@PasswordHash", DTO.Password);
                    command.Parameters.AddWithValue("@IsActive", DTO.IsActive);

                    try
                    {
                        connection.Open();

                        object result = command.ExecuteScalar();
                        if (result != null && int.TryParse(result.ToString(), out int InsertedID))
                        {
                            UserID = InsertedID;
                        }
                    }
                    catch
                    {
                        throw;
                    }
                }  

            }

            return UserID;
        }

        public static bool UpdateUser(UserDTO DTO)
        {
            int rowsAffected = 0;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"UPDATE Users 
                         SET UserName = @UserName, 
                             PasswordHash = @PasswordHash, 
                             IsActive = @IsActive 
                         WHERE UserID = @UserID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", DTO.UserID);
                    command.Parameters.AddWithValue("@UserName", DTO.UserName);
                    command.Parameters.AddWithValue("@PasswordHash", DTO.Password);
                    command.Parameters.AddWithValue("@IsActive", DTO.IsActive);

                    try
                    {
                        connection.Open();
                        rowsAffected = command.ExecuteNonQuery();
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                }
            }

            return (rowsAffected > 0);
        }

        public static bool DeleteUser(int UserID)
        {

            int rowsAffected = 0;

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string query = @"Delete Users 
                                where UserID = @UserID";

            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@UserID", UserID);

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

        public static bool SetUserStatus(int UserID, bool IsActive)
        {

            int rowsAffected = 0;

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string query = @"UPDATE Users 
                 SET IsActive = @IsActive, 
                     DeletedAt = @DeletedAt 
                 WHERE UserID = @UserID";

         

            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@DeletedAt", DateTime.Now);
            command.Parameters.AddWithValue("@UserID", UserID);
            command.Parameters.AddWithValue("@IsActive", IsActive);
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

        public static bool IsUserExist(int UserID)
        {
            bool isFound = false;

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string query = "SELECT Found=1 FROM Users WHERE UserID = @UserID";

            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@UserID", UserID);

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

        public static bool IsUserExist(string UserName)
        {
            bool isFound = false;

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string query = "SELECT Found=1 FROM Users WHERE UserName = @UserName";

            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@UserName", UserName);

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

        public static bool IsUserExistForEmployeeID(int EmployeeID)
        {
            bool isFound = false;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT Found=1 FROM Users WHERE EmployeeID = @EmployeeID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@EmployeeID", EmployeeID);

                    try
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            isFound = reader.HasRows;
                        }
                    }
                    catch (Exception)
                    {
                        isFound = false;
                    }
                }
            }

            return isFound;
        }

        public static bool ChangePassword(int UserID, string NewPassword)
        {
            int rowsAffected = 0;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"UPDATE Users  
                         SET PasswordHash = @PasswordHash
                         WHERE UserID = @UserID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", UserID);
                    command.Parameters.AddWithValue("@PasswordHash", NewPassword);

                    try
                    {
                        connection.Open();
                        rowsAffected = command.ExecuteNonQuery();
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                }
            }

            return (rowsAffected > 0);
        }

        public static UserDTO FindUserByUserID(int UserID)
        {
            UserDTO dto = null;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT * FROM Users WHERE UserID = @UserID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", UserID);

                    try
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                dto = new UserDTO
                                {
                                    UserID = (int)reader["UserID"],
                                    UserName = (string)reader["UserName"],
                                    Password = (string)reader["PasswordHash"], 
                                    EmployeeID = (int)reader["EmployeeID"],
                                    IsActive = (bool)reader["IsActive"],
                                    CreatedAt = (DateTime)reader["CreatedAt"]
                                };
                            }
                        }
                    }
                    catch (Exception)
                    {
                        // يمكن تسجيل الخطأ هنا
                        dto = null;
                    }
                }
            }

            return dto;
        }

        public static UserDTO FindUserByEmployeeID(int EmployeeID)
        {
            UserDTO dto = null;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT * FROM Users WHERE EmployeeID = @EmployeeID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@EmployeeID", EmployeeID);

                    try
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                dto = new UserDTO
                                {
                                    UserID = (int)reader["UserID"],
                                    UserName = (string)reader["UserName"],
                                    Password = (string)reader["PasswordHash"],
                                    EmployeeID = (int)reader["EmployeeID"],
                                    IsActive = (bool)reader["IsActive"],
                                    CreatedAt = (DateTime)reader["CreatedAt"]
                                };
                            }
                        }
                    }
                    catch (Exception)
                    {
                        dto = null;
                    }
                }
            }

            return dto;
        }

        public static UserDTO FindUserByUserNameAndPassword(string UserName , string PasswordHash)
        {
            UserDTO dto = null;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT * FROM Users WHERE PasswordHash = @PasswordHash AND UserName = @UserName";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@PasswordHash", PasswordHash);
                    command.Parameters.AddWithValue("@UserName", UserName);

                    try
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                dto = new UserDTO
                                {
                                    UserID = (int)reader["UserID"],
                                    UserName = (string)reader["UserName"],
                                    Password = (string)reader["PasswordHash"],
                                    EmployeeID = (int)reader["EmployeeID"],
                                    IsActive = (bool)reader["IsActive"],
                                    CreatedAt = (DateTime)reader["CreatedAt"]
                                };
                            }
                        }
                    }
                    catch (Exception)
                    {
                        dto = null;
                    }
                }
            }

            return dto;
        }


    }
}
