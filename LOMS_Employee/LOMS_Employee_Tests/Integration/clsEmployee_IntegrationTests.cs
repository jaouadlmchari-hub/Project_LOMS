using Xunit;
using LOMS_Employee_Business;
using System.Threading.Tasks;

namespace LOMS_Employee_Tests.Integration
{
    public class clsEmployee_IntegrationTests
    {
        [Fact]
        public async Task SaveAsync_ShouldCreateEmployeeAndReturnTrue()
        {
            // Arrange
            var emp = new clsEmployee();
            emp.DTO.FirstName = "Test";
            emp.DTO.LastName = "User";
            emp.DTO.NationalNo = "T12345";
            emp.DTO.Email = "test@domain.com";
            emp.DTO.DepartmentID = 1;
            emp.DTO.JobTitleID = 1;

            // Act
            bool result = await emp.SaveAsync();

            // Assert
            Assert.True(result);
            Assert.NotEqual(-1, emp.DTO.EmployeeID);

            // Cleanup
            if (result) await clsEmployee.DeleteEmployeeAsync(emp.DTO.EmployeeID);
        }
    }
}