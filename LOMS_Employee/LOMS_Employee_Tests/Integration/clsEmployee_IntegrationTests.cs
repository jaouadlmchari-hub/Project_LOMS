using Xunit;
using LOMS_Employee_Business;
using System.Threading.Tasks;

namespace LOMS_Employee_Tests.Integration
{
    public class clsEmployee_IntegrationTests
    {
        public clsEmployee_IntegrationTests()
        {
            TestDatabaseInitializer.Initialize();
        }

        [Fact]
        public async Task SaveAsync_ShouldCreateEmployeeAndReturnTrue()
        {
            // Arrange
            var emp = new clsEmployee();
            emp.DTO.FirstName = "Test";
            emp.DTO.LastName = "User";
            emp.DTO.NationalNo = "T12345";
            emp.DTO.Email = "test@domain.com";
            emp.DTO.DepartmentID = 1; // Assurez-vous que ces IDs existent dans votre DB de test !
            emp.DTO.JobTitleID = 1;

            // Act
            bool result = await emp.SaveAsync();

            // Assert
            Assert.True(result);
            Assert.NotEqual(-1, emp.DTO.EmployeeID);

            // Cleanup (Attention: si le test échoue avant la fin, le cleanup peut ne pas se faire)
            if (result && emp.DTO.EmployeeID > 0)
            {
                await clsEmployee.DeleteEmployeeAsync(emp.DTO.EmployeeID);
            }
        }
    }
}