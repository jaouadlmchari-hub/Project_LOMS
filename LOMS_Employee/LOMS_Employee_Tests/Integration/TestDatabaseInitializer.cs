using System;
using LOMS_Employee_DataAccess;

namespace LOMS_Employee_Tests.Integration
{
    public static class TestDatabaseInitializer
    {
        public static void Initialize()
        {
            string connString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");

            if (string.IsNullOrEmpty(connString))
            {
                connString = "Server=localhost;Database=LOMS_EmployeeDB;User Id=sa;Password=JJ.1213famass;TrustServerCertificate=True;";
            }

            // 3. Initialiser la classe statique de la DAL
            clsDataAccessSettings.ConnectionString = connString;

        }
    }
}