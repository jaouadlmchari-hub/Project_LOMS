using Xunit;
using LOMS_Salary_Buisness;

namespace LOMS_Salary_Tests.Unit
{
    public class clsSalary_UnitTests
    {
        [Fact]
        public void AddNewSalary_ShouldReturnMinusOne_WhenSalaryIsNegative()
        {
            // Arrange
            decimal negativeSalary = -500;
            int employeeId = 1;
            int createdBy = 1;

            // Act
            int result = clsSalary.AddNewSalary(employeeId, negativeSalary, createdBy);

            // Assert
            Assert.Equal(-1, result);
        }
    }
}