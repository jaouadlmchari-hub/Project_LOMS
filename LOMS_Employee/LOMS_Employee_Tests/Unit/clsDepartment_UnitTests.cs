using Xunit;
using LOMS_Employee_Business;

namespace LOMS_Employee_Tests.Unit
{
    public class clsDepartment_UnitTests
    {
        [Fact]
        public void Constructor_ShouldInitializeWithAddNewMode()
        {
            // Act
            var dept = new clsDepartment();

            // Assert
            Assert.Equal(clsDepartment.enMode.AddNew, dept.Mode);
            Assert.NotNull(dept.DTO);
        }
    }
}