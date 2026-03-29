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

        // Utilisation d'une propriété calculée moderne
        public string FullName => $"{DTO.FirstName} {DTO.SecondName} {DTO.ThirdName} {DTO.LastName}".Replace("  ", " ");

        // Constructeur pour le mode Ajout
        public clsEmployee()
        {
            this.DTO = new EmployeeDTO();
            this.Mode = enMode.AddNew;
        }

        // Constructeur privé pour le mode Update (utilisé par Find)
        private clsEmployee(EmployeeDTO dto)
        {
            this.DTO = dto;
            this.Mode = enMode.Update;
        }

        private bool _AddNewEmployee()
        {
            // Appelle la DAL et récupère le nouvel ID
            this.DTO.EmployeeID = clsEmployeeData.AddNewEmployee(this.DTO);
            return (this.DTO.EmployeeID != -1);
        }

        private bool _UpdateEmployee()
        {
            return clsEmployeeData.UpdateEmployee(this.DTO);
        }

        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if (_AddNewEmployee())
                    {
                        // 1. On bascule TOUJOURS en mode Update après l'insertion en DB
                        this.Mode = enMode.Update;

                        // 2. On tente d'initialiser les congés (via ton futur microservice Leave)
                        // Note : On retourne true même si l'initialisation échoue ? 
                        // À toi de décider si l'échec des congés doit annuler l'employé.
                        return RequestLeaveInitialization(this.DTO.EmployeeID);
                    }
                    return false;

                case enMode.Update:
                    return _UpdateEmployee();
            }

            return false;
        }

        private bool RequestLeaveInitialization(int employeeId)
        {
            using (var client = new System.Net.Http.HttpClient())
            {
                // L'URL de ton futur Microservice Leave
                string url = $"https://localhost:7001/api/LeaveBalances/init/{employeeId}";

                try
                {
                    var response = client.PostAsync(url, null).Result;
                    return response.IsSuccessStatusCode;
                }
                catch
                {
                    // Si le service Leave est éteint, on décide si on bloque 
                    // la création de l'employé ou non.
                    return false;
                }
            }
        }
        #region Méthodes Statiques

        public static clsEmployee Find(int employeeID)
        {
            EmployeeDTO dto = clsEmployeeData.GetEmployeeInfoByID(employeeID);
            return (dto != null) ? new clsEmployee(dto) : null;
        }

        public static clsEmployee Find(string nationalNo)
        {
            EmployeeDTO dto = clsEmployeeData.GetEmployeeInfoByNationalNo(nationalNo);
            return (dto != null) ? new clsEmployee(dto) : null;
        }

        public static DataTable GetAllEmployees()
        {
            return clsEmployeeData.GetAllEmployees();
        }

        public static bool DeleteEmployee(int employeeID)
        {
            return clsEmployeeData.DeleteEmployee(employeeID);
        }

        public static bool IsEmployeeExist(int employeeID)
        {
            return clsEmployeeData.IsEmployeeExist(employeeID);
        }

        public static bool IsEmployeeExist(string nationalNo)
        {
            return clsEmployeeData.IsEmployeeExist(nationalNo);
        }

        public static DataTable GetEmployeesByDepartment(int departmentID)
        {
            return clsEmployeeData.GetEmployeesByDepartment(departmentID);
        }

        #endregion
    }
}