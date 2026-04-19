using Xunit;
using LOMS_Employee_Business;

namespace LOMS_Employee_Tests.Unit
{
    public class clsEmployee_UnitTests
    {
        [Fact]
        public void FullName_ShouldReturnCorrectConcatenation()
        {
            // Arrange
            var emp = new clsEmployee();
            emp.DTO.FirstName = "Jaouad";
            emp.DTO.LastName = "El'Mchari";

            // Act
            string result = emp.FullName;

            // Assert
            Assert.Contains("Jaouad", result);
            Assert.Contains("El'Mchari", result);
        }
    }
}