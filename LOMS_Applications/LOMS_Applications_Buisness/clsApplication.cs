using LOMS_Applications_DataAccess;
using LOMS_Applications_Shared;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LOMS_Applications_Buisness
{
    public class clsApplication
    {
        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode = enMode.AddNew;

        public enum enApplicationStatus { Pending = 1, Approved = 2, Rejected = 3, Cancelled = 4 };

        public ApplicationDTO DTO { get; set; }

        // --- Méthodes Asynchrones pour communiquer avec les autres services ---

        public async Task<string> GetEmployeeFullNameAsync()
        {
            // Utilise le client HTTP créé dans la DAL
            EmployeeServiceClient client = new EmployeeServiceClient();
            return await client.GetEmployeeFullName(this.DTO.EmployeeID);
        }

        public async Task<string> GetActionByUserNameAsync()
        {
            // Vérifie si un utilisateur a effectué une action (évite -1 ou null)
            if (this.DTO.ActionByUserID != -1)
            {
                AuthServiceClient client = new AuthServiceClient();
                return await client.GetUserName(this.DTO.ActionByUserID);
            }
            return "N/A";
        }

        // --- Constructeurs ---

        public clsApplication()
        {
            this.DTO = new ApplicationDTO();
            this.DTO.ApplicationDate = DateTime.Now;
            this.DTO.LastStatusDate = DateTime.Now;
            this.DTO.Status = "Pending"; // Statut par défaut
            this.Mode = enMode.AddNew;
        }

        public clsApplication(ApplicationDTO dto)
        {
            this.DTO = dto;
            this.Mode = enMode.Update;
        }

        // --- Opérations de données ---

        private bool _AddNewApplication()
        {
            // Appelle la couche DataAccess
            this.DTO.ApplicationID = clsApplicationData.AddNewAppliccation(this.DTO);
            return (this.DTO.ApplicationID != -1);
        }

        private bool _UpdateApplication()
        {
            return clsApplicationData.UpdateApplication(this.DTO);
        }

        public virtual bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if (_AddNewApplication())
                    {
                        Mode = enMode.Update;
                        return true;
                    }
                    return false;

                case enMode.Update:
                    return _UpdateApplication();
            }
            return false;
        }

        // --- Méthodes Statiques ---

        public static clsApplication Find(int ApplicationID)
        {
            ApplicationDTO dto = clsApplicationData.GetApplicationInfoByID(ApplicationID);
            return (dto != null) ? new clsApplication(dto) : null;
        }

        public static DataTable GetAllApplications()
        {
            return clsApplicationData.GetAllApplications();
        }

        public bool Delete()
        {
            return clsApplicationData.DeleteApplication(this.DTO.ApplicationID);
        }

        public static bool UpdateStatus(int ApplicationID, short NewStatus)
        {
            return clsApplicationData.UpdateStatus(ApplicationID, NewStatus);
        }
    }
}