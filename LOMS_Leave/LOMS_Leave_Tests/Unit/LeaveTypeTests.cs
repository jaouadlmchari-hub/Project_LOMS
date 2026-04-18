using Xunit;
using FluentAssertions;
using LOMS_Leave_Buisness;
using LOMS_Leave_Shared;

namespace LOMS_Leave_Tests.Unit
{
    public class LeaveTypeTests
    {
        [Fact]
        public void Constructor_ShouldInitializeWithDefaultValues()
        {
            // Act
            var leaveType = new clsLeaveType();

            // Assert (Vérifie que l'initialisation AddNew est correcte)
            leaveType.LeaveTypeDTO.Should().NotBeNull();
            leaveType.LeaveTypeDTO.LeaveTypeID.Should().Be(-1);
            leaveType.LeaveTypeDTO.IsActive.Should().BeTrue();
        }

        [Fact]
        public void LeaveType_ShouldHoldDataCorrectly()
        {
            // Arrange
            var leaveType = new clsLeaveType();

            // Act
            leaveType.LeaveTypeDTO.LeaveName = "Annual Leave";
            leaveType.LeaveTypeDTO.MaxDaysPerYear = 25;

            // Assert
            leaveType.LeaveTypeDTO.LeaveName.Should().Be("Annual Leave");
            leaveType.LeaveTypeDTO.MaxDaysPerYear.Should().Be(25);
        }
    }
}