using FluentAssertions;
using LOMS_Leave_Buisness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOMS_Leave_Tests.Unit
{
    public class clsLeaveApplication_UnitTests
    {
        [Fact]
        public void CalculateLeaveDays_ShouldExcludeWeekends()
        {
            // Lundi au Dimanche (7 jours calendaires, mais 5 jours ouvrables)
            DateTime start = new DateTime(2026, 04, 20);
            DateTime end = new DateTime(2026, 04, 26);

            int days = clsLeaveApplication.CalculateLeaveDays(start, end);

            days.Should().Be(5);
        }
    }
}
