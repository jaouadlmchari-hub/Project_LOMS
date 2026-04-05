using LOMS_Auth_DataAccess;
using LOMS_Auth_DataAccess.LOMS_Auth_DataAccess;
using LOMS_Auth_Shared;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LOMS_Auth_Business
{
    public class clsUser
    {
        public enum enMode { AddNew = 0, Update = 1 }
        public enMode Mode { get; private set; }

        public UserDTO DTO { get; set; }

        // Contiendra les infos (Nom, Email) récupérées via API
        public dynamic EmployeeInfo { get; private set; }

        public clsUser()
        {
            this.DTO = new UserDTO();
            this.DTO.UserID = -1;
            this.Mode = enMode.AddNew;
            this.EmployeeInfo = null;
        }

        private clsUser(UserDTO dto, object employeeInfo)
        {
            this.DTO = dto;
            this.EmployeeInfo = employeeInfo;
            this.Mode = enMode.Update;
        }

        #region Private Methods (Internal Logic)

        private bool _AddNewUser()
        {
            // Insertion en base locale AuthDB
            this.DTO.UserID = clsUserData.AddNewUser(this.DTO);

            if (this.DTO.UserID != -1)
            {
                // Gestion des rôles (Table locale UserRoles)
                foreach (int RoleID in this.DTO.SelectedRolesIDs)
                {
                    clsUserRolesData.AddRoleToUser(this.DTO.UserID, RoleID);
                }
                return true;
            }
            return false;
        }

        private bool _UpdateUser()
        {
            // Mise à jour locale
            bool isUserUpdated = clsUserData.UpdateUser(this.DTO);

            if (isUserUpdated)
            {
                // Refresh des rôles
                clsUserRolesData.DeleteAllRolesByUserID(this.DTO.UserID);

                foreach (int RoleID in this.DTO.SelectedRolesIDs)
                {
                    clsUserRolesData.AddRoleToUser(this.DTO.UserID, RoleID);
                }
                return true;
            }
            return false;
        }

        #endregion

        #region Public Methods (Operations)

        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if (_AddNewUser())
                    {
                        Mode = enMode.Update;
                        return true;
                    }
                    return false;

                case enMode.Update:
                    return _UpdateUser();
            }
            return false;
        }

        // Récupère la liste pour le DataGridView avec agrégation API
        public static async Task<DataTable> GetAllUsersWithDetailsAsync()
        {
            // 1. SQL Local
            DataTable dtUsers = clsUserData.GetAllUsers();

            // 2. Préparation des colonnes externes
            if (!dtUsers.Columns.Contains("FullName")) dtUsers.Columns.Add("FullName", typeof(string));
            if (!dtUsers.Columns.Contains("Email")) dtUsers.Columns.Add("Email", typeof(string));

            // 3. Appel API pour chaque ligne (Agrégation)
            foreach (DataRow row in dtUsers.Rows)
            {
                int empID = Convert.ToInt32(row["EmployeeID"]);
                var emp = await clsEmployeeServiceClient.GetEmployeeBasicInfoAsync(empID);

                if (emp != null)
                {
                    row["FullName"] = emp.FirstName + " " + emp.LastName;
                    row["Email"] = emp.Email;
                }
                else
                {
                    row["FullName"] = "[Non trouvé]";
                    row["Email"] = "-";
                }
            }
            return dtUsers;
        }

        public static async Task<clsUser> FindByUserIDAsync(int UserID)
        {
            UserDTO dto = clsUserData.FindUserByUserID(UserID);

            if (dto == null)
            {
                // Si on entre ici, c'est que le problème est dans la DAL (SQL)
                return null;
            }

            try
            {
                var empInfo = await clsEmployeeServiceClient.GetEmployeeBasicInfoAsync(dto.EmployeeID);
                return new clsUser(dto, empInfo);
            }
            catch (Exception ex)
            {
                throw ex; // remonte l'erreur vers Swagger
            }
        }
        public static async Task<clsUser> FindByUserNameAndPasswordAsync(string UserName, string Password)
        {
            UserDTO dto = clsUserData.FindUserByUserNameAndPassword(UserName, Password);
            if (dto != null)
            {
                dto.SelectedRolesIDs = clsUserRolesData.GetUserRolesIDs(dto.UserID);
                var empInfo = await clsEmployeeServiceClient.GetEmployeeBasicInfoAsync(dto.EmployeeID);
                return new clsUser(dto, empInfo);
            }
            return null;
        }

        // Méthodes synchrones (uniquement AuthDB locale)
        public static bool DeleteUser(int UserID)
        {
            clsUserRolesData.DeleteAllRolesByUserID(UserID);
            return clsUserData.DeleteUser(UserID);
        }

        public static bool SetUserStatus(int UserID, bool IsActive)
        {
            return clsUserData.SetUserStatus(UserID, IsActive);
        }

        public static bool IsUserExist(string UserName)
        {
            return clsUserData.IsUserExist(UserName);
        }

        public static bool ChangePassword(int UserID, string NewPassword)
        {
            return clsUserData.ChangePassword(UserID, NewPassword);
        }

        #endregion
    }
}