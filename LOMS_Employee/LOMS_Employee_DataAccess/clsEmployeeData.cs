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
    public class clsEmployeeData
    {
        public static EmployeeDTO GetEmployeeInfoByID(int employeeID)
        {
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"SELECT * FROM Employees WHERE EmployeeID = @EmployeeID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@EmployeeID", employeeID);

                    try
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (!reader.Read()) return null;

                            return new EmployeeDTO
                            {
                                EmployeeID = (int)reader["EmployeeID"],
                                NationalNo = reader["NationalNo"] as string ?? "",
                                FirstName = reader["FirstName"] as string ?? "",

                                // Ces colonnes sont NULL dans ton SELECT, donc "as string ?? """ est obligatoire ici :
                                SecondName = reader["SecondName"] as string ?? "",
                                ThirdName = reader["ThirdName"] as string ?? "",
                                LastName = reader["LastName"] as string ?? "",

                                DateOfBirth = (DateTime)reader["DateOfBirth"],
                                Gender = Convert.ToInt16(reader["Gender"]),

                                // Sécurisation des autres champs NULL :
                                Address = reader["Address"] as string ?? "",
                                Phone = reader["Phone"] as string ?? "",
                                Email = reader["Email"] as string ?? "",
                                ImagePath = reader["ImagePath"] as string ?? "",

                                NationalityCountryID = (int)reader["NationalityCountryID"],
                                HireDate = (DateTime)reader["HireDate"],
                                IsActive = (bool)reader["IsActive"],
                                CreatedAt = (DateTime)reader["CreatedAt"],

                                // Pour les entiers qui sont NULL (DepartmentID et JobTitleID) :
                                DepartmentID = reader["DepartmentID"] == DBNull.Value ? 0 : (int)reader["DepartmentID"],
                                JobTitleID = reader["JobTitleID"] == DBNull.Value ? 0 : (int)reader["JobTitleID"],

                                Salary = 0 // Le salaire sera récupéré plus tard via l'API (Port 7200)
                            };
                        }
                    }
                    catch { throw; }
                }
            }
        }

        public static EmployeeDTO GetEmployeeInfoByNationalNo(string NationalNo)
        {
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"SELECT * FROM Employees WHERE NationalNo = @NationalNo";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@NationalNo", NationalNo);

                    try
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (!reader.Read()) return null;

                            return new EmployeeDTO
                            {
                                EmployeeID = (int)reader["EmployeeID"],
                                NationalNo = reader["NationalNo"] as string ?? "",
                                FirstName = reader["FirstName"] as string ?? "",

                                SecondName = reader["SecondName"] as string ?? "",
                                ThirdName = reader["ThirdName"] as string ?? "",
                                LastName = reader["LastName"] as string,

                                DateOfBirth = (DateTime)reader["DateOfBirth"],
                                Gender = Convert.ToInt16(reader["Gender"]),

                                // Sécurisation des autres champs NULL :
                                Address = reader["Address"] as string ?? "",
                                Phone = reader["Phone"] as string ?? "",
                                Email = reader["Email"] as string ?? "",
                                ImagePath = reader["ImagePath"] as string ?? "",

                                NationalityCountryID = (int)reader["NationalityCountryID"],
                                HireDate = (DateTime)reader["HireDate"],
                                IsActive = (bool)reader["IsActive"],
                                CreatedAt = (DateTime)reader["CreatedAt"],

                                // Pour les entiers qui sont NULL (DepartmentID et JobTitleID) :
                                DepartmentID = reader["DepartmentID"] == DBNull.Value ? 0 : (int)reader["DepartmentID"],
                                JobTitleID = reader["JobTitleID"] == DBNull.Value ? 0 : (int)reader["JobTitleID"],

                                Salary = 0 // Le salaire sera récupéré plus tard via l'API (Port 7200)
                            };
                        }
                    }
                    catch { throw; }
                }
            }
        }

        public static DataTable GetEmployeesByDepartment(int DepartmentID)
        {
            DataTable dt = new DataTable();
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {


                string query = @"SELECT 
                    Employees.EmployeeID, 
                    Employees.NationalNo,
                    (Employees.FirstName + ' ' + Employees.LastName) AS FullName,
                    CASE WHEN Employees.Gender = 1 THEN 'Male' ELSE 'Female' END AS Gender,
                    Employees.Phone, 
                    Employees.Email,
                    Departments.DepartmentName, 
                    JobTitles.TitleName,
                    CASE WHEN Employees.IsActive = 1 THEN 'Active' ELSE 'Inactive' END AS Status
                 FROM Employees
                 INNER JOIN Departments ON Employees.DepartmentID = Departments.DepartmentID
                 INNER JOIN JobTitles ON Employees.JobTitleID = JobTitles.JobTitleID
                 WHERE Employees.DepartmentID = @DepartmentID
                 ORDER BY Employees.FirstName";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@DepartmentID", DepartmentID);

                    try
                    {
                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                dt.Load(reader);
                            }
                        }
                    }
                    catch
                    {
                        // Log si nécessaire
                        throw;
                    }
                }
            }
            return dt;

        }

        public static int AddNewEmployee(EmployeeDTO employee)
        {
            int newID = -1;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"
            BEGIN TRY
                BEGIN TRANSACTION;

                INSERT INTO Employees 
                (NationalNo, FirstName, SecondName, ThirdName, LastName, 
                 DateOfBirth, Gender, Address, Phone, Email, 
                 NationalityCountryID, ImagePath, HireDate, IsActive, 
                 CreatedAt, DepartmentID, JobTitleID)
                VALUES 
                (@NationalNo, @FirstName, @SecondName, @ThirdName, @LastName, 
                 @DateOfBirth, @Gender, @Address, @Phone, @Email, 
                 @NationalityCountryID, @ImagePath, @HireDate, @IsActive, 
                 @CreatedAt, @DepartmentID, @JobTitleID);

                DECLARE @InsertedEmployeeID INT = SCOPE_IDENTITY();

                INSERT INTO EmployeeSalaries
                (EmployeeID, Salary, EffectiveDate, CreatedByUserID, CreatedAt)
                VALUES
                (@InsertedEmployeeID, @SalaryAmount, @EffectiveDate, @CreatedByUserID, @CreatedAt);

                COMMIT TRANSACTION;

                SELECT @InsertedEmployeeID;

            END TRY
            BEGIN CATCH
                IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
                THROW;
            END CATCH";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@NationalNo", employee.NationalNo);
                    command.Parameters.AddWithValue("@FirstName", employee.FirstName);
                    command.Parameters.AddWithValue("@SecondName", employee.SecondName);
                    command.Parameters.AddWithValue("@ThirdName", clsDataAccessHelper.EnsureValue(employee.ThirdName));
                    command.Parameters.AddWithValue("@LastName", employee.LastName);
                    command.Parameters.AddWithValue("@DateOfBirth", employee.DateOfBirth);
                    command.Parameters.AddWithValue("@Gender", employee.Gender);
                    command.Parameters.AddWithValue("@Address", employee.Address);
                    command.Parameters.AddWithValue("@Phone", employee.Phone);
                    command.Parameters.AddWithValue("@Email", clsDataAccessHelper.EnsureValue(employee.Email));
                    command.Parameters.AddWithValue("@NationalityCountryID", employee.NationalityCountryID);
                    command.Parameters.AddWithValue("@ImagePath", clsDataAccessHelper.EnsureValue(employee.ImagePath));
                    command.Parameters.AddWithValue("@HireDate", employee.HireDate);
                    command.Parameters.AddWithValue("@IsActive", employee.IsActive);
                    command.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
                    command.Parameters.AddWithValue("@DepartmentID", employee.DepartmentID);
                    command.Parameters.AddWithValue("@JobTitleID", employee.JobTitleID);

                    command.Parameters.AddWithValue("@SalaryAmount", employee.Salary);
                    command.Parameters.AddWithValue("@EffectiveDate", employee.HireDate);
                    command.Parameters.AddWithValue("@CreatedByUserID", employee.CreatedByUserID);

                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();

                        if (result != null && int.TryParse(result.ToString(), out int insertedID))
                        {
                            newID = insertedID;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                }
            }
            return newID;
        }

        public static bool UpdateEmployee(EmployeeDTO employee)
        {
            int rowsAffected = 0;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"UPDATE Employees 
                         SET NationalNo = @NationalNo, 
                             FirstName = @FirstName, 
                             SecondName = @SecondName, 
                             ThirdName = @ThirdName, 
                             LastName = @LastName, 
                             DateOfBirth = @DateOfBirth, 
                             Gender = @Gender, 
                             Address = @Address, 
                             Phone = @Phone, 
                             Email = @Email, 
                             NationalityCountryID = @NationalityCountryID, 
                             ImagePath = @ImagePath, 
                             HireDate = @HireDate, 
                             IsActive = @IsActive, 
                             DepartmentID = @DepartmentID, 
                             JobTitleID = @JobTitleID
                         WHERE EmployeeID = @EmployeeID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@EmployeeID", employee.EmployeeID);

                    command.Parameters.AddWithValue("@NationalNo", employee.NationalNo);
                    command.Parameters.AddWithValue("@FirstName", employee.FirstName);
                    command.Parameters.AddWithValue("@SecondName", employee.SecondName);
                    command.Parameters.AddWithValue("@ThirdName", clsDataAccessHelper.EnsureValue(employee.ThirdName));
                    command.Parameters.AddWithValue("@LastName", employee.LastName);
                    command.Parameters.AddWithValue("@DateOfBirth", employee.DateOfBirth);
                    command.Parameters.AddWithValue("@Gender", employee.Gender);
                    command.Parameters.AddWithValue("@Address", employee.Address);
                    command.Parameters.AddWithValue("@Phone", employee.Phone);
                    command.Parameters.AddWithValue("@Email", clsDataAccessHelper.EnsureValue(employee.Email));
                    command.Parameters.AddWithValue("@NationalityCountryID", employee.NationalityCountryID);
                    command.Parameters.AddWithValue("@ImagePath", clsDataAccessHelper.EnsureValue(employee.ImagePath));
                    command.Parameters.AddWithValue("@HireDate", employee.HireDate);
                    command.Parameters.AddWithValue("@IsActive", employee.IsActive);
                    command.Parameters.AddWithValue("@DepartmentID", employee.DepartmentID);
                    command.Parameters.AddWithValue("@JobTitleID", employee.JobTitleID);

                    try
                    {
                        connection.Open();
                        rowsAffected = command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        // Log(ex);
                        throw;
                    }
                }
            }

            return (rowsAffected > 0);
        }

        public static DataTable GetAllEmployees()
        {
            DataTable dt = new DataTable();

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"SELECT 
                    Employees.EmployeeID, 
                    Employees.NationalNo,
                    (Employees.FirstName + ' ' + Employees.LastName) AS FullName,
                    CASE WHEN Employees.Gender = 1 THEN 'Male' ELSE 'Female' END AS Gender,
                    Employees.Phone, 
                    Employees.Email,
                    Departments.DepartmentName, 
                    JobTitles.TitleName,
                    CASE WHEN Employees.IsActive = 1 THEN 'Active' ELSE 'Inactive' END AS Status
                 FROM Employees
                 INNER JOIN Departments ON Employees.DepartmentID = Departments.DepartmentID
                 INNER JOIN JobTitles ON Employees.JobTitleID = JobTitles.JobTitleID
                 ORDER BY Employees.FirstName";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    try
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                dt.Load(reader);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log(ex);
                        throw;
                    }
                }
            }
            return dt;
        }

        public static bool DeleteEmployee(int employeeID)
        {
            int rowsAffected = 0;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "DELETE FROM Employees WHERE EmployeeID = @EmployeeID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@EmployeeID", employeeID);

                    try
                    {
                        connection.Open();
                        rowsAffected = command.ExecuteNonQuery();
                    }
                    catch
                    {
                        throw;
                    }
                }
            }

            return (rowsAffected > 0);
        }

        public static bool IsEmployeeExist(int employeeID)
        {

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "Select  1  FROM Employees WHERE EmployeeID = @EmployeeID";


                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@EmployeeID", employeeID);

                    try
                    {
                        connection.Open();

                        object result = command.ExecuteScalar();
                        return result != null;
                    }
                    catch
                    {
                        throw;
                    }
                }
            }

        }

        public static bool IsEmployeeExist(string NationalNo)
        {

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "Select 1  FROM Employees WHERE NationalNo = @NationalNo";


                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@NationalNo", NationalNo);

                    try
                    {
                        connection.Open();

                        object result = command.ExecuteScalar();
                        return result != null;
                    }
                    catch
                    {
                        throw;
                    }
                }
            }

        }

    }
}
