using Xunit;
using LOMS_Employee_Business;
using System.Data;

namespace LOMS_Employee_Tests.Integration
{
    public class clsDepartment_IntegrationTests
    {
        [Fact]
        public void Save_ShouldAddNewDepartment_And_Find_ShouldReturnIt()
        {
            // --- ARRANGE ---
            var dept = new clsDepartment();
            dept.DTO.DepartmentName = "IT Department Test";
            dept.DTO.IsActive = true;

            // --- ACT ---
            bool saveResult = dept.Save();
            int id = dept.DTO.DepartmentID;

            // --- ASSERT ---
            Assert.True(saveResult);
            Assert.True(id > 0);

            var foundDept = clsDepartment.Find(id);
            Assert.NotNull(foundDept);
            Assert.Equal("IT Department Test", foundDept.DTO.DepartmentName);
        }

        [Fact]
        public void GetDepartmentSummary_ShouldReturnData()
        {
            // Act
            DataTable dt = clsDepartment.GetDepartmentSummary();

            // Assert
            Assert.NotNull(dt);
        }
    }
}