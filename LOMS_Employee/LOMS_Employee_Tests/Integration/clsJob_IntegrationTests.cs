using Xunit;
using LOMS_Employee_Business;
using System.Data;

namespace LOMS_Employee_Tests.Integration
{
    public class clsJob_IntegrationTests
    {

        public clsJob_IntegrationTests()
        {
            TestDatabaseInitializer.Initialize();
        }

        [Fact]
        public void Find_By_ID_Should_Return_Job_If_Exists()
        {
            // Arrange
            int existingID = 1;

            // Act
            clsJob job = clsJob.Find(existingID);

            // Assert
            if (job != null)
            {
                Assert.Equal(existingID, job.DTO.JobTitleID);
                Assert.NotNull(job.DTO.TitleName);
            }
        }

        [Fact]
        public void GetAllJobTitles_Should_Return_Data()
        {
            // Act
            DataTable dt = clsJob.GetAllJobTitles();

            // Assert
            Assert.NotNull(dt);
        }
    }
}