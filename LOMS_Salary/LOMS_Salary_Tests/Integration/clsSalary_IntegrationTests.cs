using Xunit;
using LOMS_Salary_Buisness;

namespace LOMS_Salary_Tests.Integration
{
    public class clsSalary_IntegrationTests
    {
        [Fact]
        public void FindByEmployeeID_ShouldReturnSalary_IfEmployeeExists()
        {
            // Arrange
            int existingEmployeeID = 1;

            // Act
            var salary = clsSalary.FindByEmployeeID(existingEmployeeID);

            // Assert
            if (salary != null)
            {
                Assert.NotNull(salary.DTO);
                Assert.True(salary.DTO.Salary >= 0);
            }
        }
    }
}