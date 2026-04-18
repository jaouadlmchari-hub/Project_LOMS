using Xunit;
using LOMS_Leave_Buisness;
using System;

namespace LOMS_Leave_Tests.Intergration
{
    public class clsPublicHolidayTests
    {
        [Fact] 
        public void Should_Save_And_Delete_PublicHoliday_Integration()
        {
            // --- ARRANGE ---
            var holiday = new clsPublicHoliday();
            holiday.DTO.HolidayName = "Fête du Trône";
            holiday.DTO.HolidayDate = new DateTime(2026, 07, 30);
            holiday.DTO.IsRepeatedAnnually = true;

            // --- ACT ---
            bool saveResult = holiday.Save();
            int id = holiday.DTO.HolidayID;

            // --- ASSERT ---
            Assert.True(saveResult, "La sauvegarde aurait dû réussir.");
            Assert.True(id > 0, "L'ID généré devrait être supérieur à 0.");

            // --- CLEANUP 
            if (saveResult)
            {
                clsPublicHoliday.Delete(id);
            }
        }
    }
}