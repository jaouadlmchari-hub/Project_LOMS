using Xunit;
using FluentAssertions;
using LOMS_Leave_Buisness;
using System.Data;

namespace LOMS_Leave_Tests.Intergration
{
    public class clsLeaveBalanceTests
    {
        private const int TEST_EMPLOYEE_ID = 9999; 
        private const int TEST_YEAR = 2026;
        private const int LEAVE_TYPE_ANNUAL = 1; //  1 = Congé Annuel

        [Fact]
        public void LeaveBalance_FullFlow_IntegrationTest()
        {
            // --- 1. TEST INITIALISATION ---
            // On s'assure que les soldes sont créés pour l'année
            bool initResult = clsLeaveBalance.InitializeBalances(TEST_EMPLOYEE_ID, TEST_YEAR);
            initResult.Should().BeTrue("L'initialisation des soldes devrait réussir.");

            // --- 2. TEST RECHERCHE (FIND) ---
            var balance = clsLeaveBalance.FindBalanceByEmployeeAndType(TEST_EMPLOYEE_ID, LEAVE_TYPE_ANNUAL, TEST_YEAR);
            balance.Should().NotBeNull("Le solde devrait être trouvable après initialisation.");
            decimal initialRemaining = balance.LeaveBalanceDTO.RemainingDays;

            // --- 3. TEST MISE À JOUR VALIDE (UPDATE) ---
            // On simule la prise de 2 jours de congés
            decimal daysToTake = 2.0m;
            bool updateResult = clsLeaveBalance.UpdateUsedDays(TEST_EMPLOYEE_ID, LEAVE_TYPE_ANNUAL, TEST_YEAR, daysToTake);

            updateResult.Should().BeTrue("La mise à jour devrait réussir car le solde est suffisant.");

            // Vérification après mise à jour
            var updatedBalance = clsLeaveBalance.FindBalanceByEmployeeAndType(TEST_EMPLOYEE_ID, LEAVE_TYPE_ANNUAL, TEST_YEAR);
            updatedBalance.LeaveBalanceDTO.UsedDays.Should().Be(daysToTake);
            updatedBalance.LeaveBalanceDTO.RemainingDays.Should().Be(initialRemaining - daysToTake);

            // --- 4. TEST SÉCURITÉ : SOLDE INSUFFISANT ---
         
            bool invalidUpdate = clsLeaveBalance.UpdateUsedDays(TEST_EMPLOYEE_ID, LEAVE_TYPE_ANNUAL, TEST_YEAR, 50.0m);

            invalidUpdate.Should().BeFalse("Le système doit refuser la mise à jour si le solde est insuffisant.");
        }

        [Fact]
        public void GetEmployeeBalances_ShouldReturnData()
        {
            // Act
            DataTable dt = clsLeaveBalance.GetEmployeeBalances(TEST_EMPLOYEE_ID, TEST_YEAR);

            // Assert
            dt.Should().NotBeNull();
            // Si l'initialisation a été faite par le test précédent ou ici
            // dt.Rows.Count.Should().BeGreaterThan(0);
        }
    }
}