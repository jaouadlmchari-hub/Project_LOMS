using FluentAssertions;
using LOMS_Leave_Buisness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOMS_Leave_Tests.Integration
{
    public class clsLeaveApplication_IntegrationTests
    {

        [Fact]
        public void Save_ShouldReturnFalse_WhenBalanceIsInsufficient()
        {
            // Arrange
            var app = new clsLeaveApplication();
            app.BaseApplicationData.EmployeeID = 99;
            app.BaseApplicationData.ApplicationDate = DateTime.Now;
            app.LeaveDTO.LeaveTypeID = 1;
            app.LeaveDTO.NumberOfDays = 500;

            // Act
            bool result = app.Save();

            // Assert
            result.Should().BeFalse();
            app.SaveErrorDetails.Should().Contain("Solde insuffisant");
        }
    }
}
