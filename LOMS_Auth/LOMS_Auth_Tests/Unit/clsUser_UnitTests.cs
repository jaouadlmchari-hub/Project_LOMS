using Xunit;
using LOMS_Auth_Business;

namespace LOMS_Auth_Tests.Unit
{
    public class clsUser_UnitTests
    {
        [Fact]
        public void Constructor_ShouldInitializeWithDefaultValues()
        {
            // Act
            var user = new clsUser();

            // Assert
            Assert.Equal(-1, user.DTO.UserID);
            Assert.Equal(clsUser.enMode.AddNew, user.Mode);
            Assert.Null(user.EmployeeInfo);
        }
    }
}