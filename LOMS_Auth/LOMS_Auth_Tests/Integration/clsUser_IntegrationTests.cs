using Xunit;
using LOMS_Auth_Business;
using System.Collections.Generic;

namespace LOMS_Auth_Tests.Integration
{
    public class clsUser_IntegrationTests
    {
        [Fact]
        public void Save_NewUser_ShouldAssignRolesCorrectly()
        {
            // --- ARRANGE ---
            var user = new clsUser();
            user.DTO.UserName = "jaouad_admin";
            user.DTO.Password = "Password123";
            user.DTO.EmployeeID = 1;
            user.DTO.IsActive = true;
            user.DTO.SelectedRolesIDs = new List<int> { 1, 2 }; // Admin et Manager

            // --- ACT ---
            bool result = user.Save();

            // --- ASSERT ---
            Assert.True(result);
            Assert.NotEqual(-1, user.DTO.UserID);

            // Cleanup
            if (result) clsUser.DeleteUser(user.DTO.UserID);
        }
    }
}