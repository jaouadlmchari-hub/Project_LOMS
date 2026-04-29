using Xunit;
using LOMS_Auth_Business;
using System.Collections.Generic;
using System.Data;

namespace LOMS_Auth_Tests.Integration
{
    public class clsRole_IntegrationTests
    {
        [Fact]
        public void GetAllRoles_ShouldReturnData()
        {
            // Act
            DataTable dt = clsRole.GetAllRoles();

            // Assert
            Assert.NotNull(dt);
        }

        [Fact]
        public void GetRoleNames_ShouldReturnList_WhenIDsAreProvided()
        {
            // Arrange
            var roleIDs = new List<int> { 1 }; 

            // Act
            var names = clsRole.GetRoleNames(roleIDs);

            // Assert
            Assert.NotNull(names);
            
        }

        [Fact]
        public void GetRoleNames_ShouldReturnEmptyList_WhenInputIsEmpty()
        {
            // Act
            var names = clsRole.GetRoleNames(new List<int>());

            // Assert
            Assert.Empty(names);
        }
    }
}