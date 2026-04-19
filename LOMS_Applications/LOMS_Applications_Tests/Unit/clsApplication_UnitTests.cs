using Xunit;
using LOMS_Applications_Buisness;
using System;

namespace LOMS_Applications_Tests.Unit
{
    public class clsApplication_UnitTests
    {
        [Fact]
        public void Constructor_ShouldSetDefaultValues()
        {
            // Act
            var app = new clsApplication();

            // Assert
            Assert.Equal("Pending", app.DTO.Status);
            Assert.Equal(clsApplication.enMode.AddNew, app.Mode);
            Assert.Equal(DateTime.Now.Date, app.DTO.ApplicationDate.Date);
        }
    }
}