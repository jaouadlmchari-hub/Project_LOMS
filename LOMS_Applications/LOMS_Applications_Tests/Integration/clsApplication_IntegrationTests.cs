using Xunit;
using LOMS_Applications_Buisness;
using System.Threading.Tasks;

namespace LOMS_Applications_Tests.Integration
{
    public class clsApplication_IntegrationTests
    {
        [Fact]
        public async Task SaveAsync_ShouldAddNewApplicationToDatabase()
        {
            // Arrange
            var app = new clsApplication();
            app.DTO.EmployeeID = 1;
            app.DTO.ApplicationTypeID = 1;
            app.DTO.CreatedByUserID = 1;
            app.DTO.Notes = "Test Integration Application";

            // Act
            bool result = await app.SaveAsync();

            // Assert
            Assert.True(result); // Remplace result.Should().BeTrue()
            Assert.NotEqual(-1, app.DTO.ApplicationID); // Remplace .Should().NotBe(-1)

            // Cleanup
            if (result) app.Delete();
        }
    }
}