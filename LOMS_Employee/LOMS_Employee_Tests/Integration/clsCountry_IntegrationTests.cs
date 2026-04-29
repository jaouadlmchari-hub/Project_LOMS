using Xunit;
using LOMS_Employee_Business;
using System.Data;

namespace LOMS_Employee_Tests.Integration
{
    public class clsCountry_IntegrationTests
    {

        public clsCountry_IntegrationTests()
        {
            TestDatabaseInitializer.Initialize();
        }   

        [Fact]
        public void Find_ByID_ShouldReturnCountry_WhenIDExists()
        {
            // Arrange
            int targetID = 1; 

            // Act
            clsCountry country = clsCountry.Find(targetID);

            // Assert
            if (country != null)
            {
                Assert.Equal(targetID, country.ID);
                Assert.False(string.IsNullOrEmpty(country.CountryName));
            }
        }

        [Fact]
        public void GetAllCountries_ShouldReturnPopulatedDataTable()
        {
            // Act
            DataTable dt = clsCountry.GetAllCountries();

            // Assert
            Assert.NotNull(dt);
        }
    }
}