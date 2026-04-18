using LOMS_Salary_DataAccess;
using LOMS_Salary_Shared;

namespace LOMS_Salary_Buisness
{
    public class clsSalary
    {
        public EmployeeSalaryDTO DTO { get; set; }

        private clsSalary(EmployeeSalaryDTO dto)
        {
            this.DTO = dto;
        }

        /// <summary>
        /// Recherche le salaire via l'ID interne de l'employé
        /// </summary>
        public static clsSalary FindByEmployeeID(int employeeID)
        {
            EmployeeSalaryDTO dto = clsEmployeeSalaryData.GetSalaryByEmployeeID(employeeID);

            if (dto != null)
                return new clsSalary(dto);
            else
                return null;
        }

        /// <summary>
        /// Recherche le salaire via le numéro national (CIN)
        /// </summary>
        public static clsSalary FindByNationalNo(string nationalNo)
        {
            EmployeeSalaryDTO dto = clsEmployeeSalaryData.GetSalaryByNationalNo(nationalNo);

            if (dto != null)
                return new clsSalary(dto);
            else
                return null;
        }

        public static int AddNewSalary(int employeeId, decimal salary, int createdBy)
        {
            // Exp: On refuse les salaires inférieurs au SMIG ou négatifs
            if (salary < 0) return -1;

            return clsEmployeeSalaryData.AddNewSalary(employeeId, salary, createdBy);
        }
    }
}