
using LOMS_Employee_DataAccess;
using LOMS_Employee_Shared;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOMS_Employee_Business
{
    public class clsEmployee
    {
        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode { get; private set; }
        public EmployeeDTO DTO { get; set; }

        public string FullName => $"{DTO.FirstName} {DTO.SecondName} {DTO.ThirdName} {DTO.LastName}".Replace("  ", " ");

        public clsEmployee()
        {
            this.DTO = new EmployeeDTO();
            this.Mode = enMode.AddNew;
        }

        private clsEmployee(EmployeeDTO dto)
        {
            this.DTO = dto;
            this.Mode = enMode.Update;
        }

        private async Task<bool> _AddNewEmployeeAsync()
        {
            // On exécute la DAL SQL dans un thread séparé pour l'async
            return await Task.Run(() => {
                this.DTO.EmployeeID = clsEmployeeData.AddNewEmployee(this.DTO);
                return (this.DTO.EmployeeID != -1);
            });
        }

        private async Task<bool> _UpdateEmployeeAsync()
        {
            return await Task.Run(() => clsEmployeeData.UpdateEmployee(this.DTO));
        }

        public async Task<bool> SaveAsync()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if (await _AddNewEmployeeAsync())
                    {
                        this.Mode = enMode.Update;

                        // 1. Initialisation du Salaire (API Salary)
                        _ = clsSalaryServiceClient.CreateInitialSalaryAsync(
                            this.DTO.EmployeeID,
                            this.DTO.Salary,
                            this.DTO.CreatedByUserID
                        );

                        // 2. Initialisation des Congés (API Leave)
                        _ = clsLeaveServiceClient.InitLeaveBalanceAsync(this.DTO.EmployeeID);

                        return true;
                    }
                    return false;

                case enMode.Update:
                    return await _UpdateEmployeeAsync();
            }
            return false;
        }

        #region Méthodes Statiques

        public static async Task<clsEmployee> FindAsync(int employeeID)
        {
            EmployeeDTO dto = await Task.Run(() => clsEmployeeData.GetEmployeeInfoByID(employeeID));

            if (dto != null)
            {
                // Utilisation de client Salary asynchrone (Port 7179)
                dto.Salary = await clsSalaryServiceClient.GetSalaryByEmployeeIDAsync(employeeID);
                return new clsEmployee(dto);
            }
            return null;
        }

        public static async Task<clsEmployee> FindAsync(string nationalNo)
        {
            EmployeeDTO dto = await Task.Run(() => clsEmployeeData.GetEmployeeInfoByNationalNo(nationalNo));
            if (dto != null)
            {
                dto.Salary = await clsSalaryServiceClient.GetSalaryByEmployeeIDAsync(dto.EmployeeID);
                return new clsEmployee(dto);
            }
            return null;
        }

        public static async Task<DataTable> GetAllEmployeesAsync()
        {
            return await Task.Run(() => clsEmployeeData.GetAllEmployees());
        }

        public static async Task<bool> DeleteEmployeeAsync(int employeeID)
        {
            return await Task.Run(() => clsEmployeeData.DeleteEmployee(employeeID));
        }

        public static async Task<bool> IsEmployeeExistAsync(int employeeID)
        {
            return await Task.Run(() => clsEmployeeData.IsEmployeeExist(employeeID));
        }

        public static async Task<bool> IsEmployeeExistAsync(string nationalNo)
        {
            return await Task.Run(() => clsEmployeeData.IsEmployeeExist(nationalNo));
        }

        #endregion
    }
}