using LOMS_Leave_Shared;
using LOMS_Leave_DataAccess;
using System;
using System.Data;

namespace LOMS_Leave_Buisness
{
    public class clsLeaveApplication
    {
        public enum enMode { AddNew = 0, Update = 1 }
        public enMode Mode { get; set; }

        // Données de base reçues (via Kafka ou API)
        public ApplicationDTO BaseApplicationData { get; set; }

        // Données spécifiques au domaine "Leave"
        public LeaveApplicationDTO LeaveDTO { get; set; }

        public string SaveErrorDetails = "";

        // Propriété calculée pour le type de congé (Lazy Loading)
        private clsLeaveType _LeaveType;
        public clsLeaveType LeaveTypeInfo
        {
            get
            {
                if (_LeaveType == null && LeaveDTO.LeaveTypeID != -1)
                    _LeaveType = clsLeaveType.Find(this.LeaveDTO.LeaveTypeID);
                return _LeaveType;
            }
        }

        public string LeaveTypeName => LeaveTypeInfo?.LeaveTypeDTO.LeaveName ?? "Inconnu";

        public clsLeaveApplication()
        {
            this.BaseApplicationData = new ApplicationDTO();
            this.LeaveDTO = new LeaveApplicationDTO();
            this.Mode = enMode.AddNew;
        }

        // Constructeur interne pour le mode Update
        private clsLeaveApplication(ApplicationDTO baseDTO, LeaveApplicationDTO leaveDTO)
        {
            this.BaseApplicationData = baseDTO;
            this.LeaveDTO = leaveDTO;
            this.Mode = enMode.Update;
        }

        /// <summary>
        /// Sauvegarde locale dans la base de données Leave.
        /// </summary>
        public bool Save()
        {
            // 1. Validation du solde (Règle métier critique)
            clsLeaveBalance balance = clsLeaveBalance.FindBalanceByEmployeeAndType(
                this.BaseApplicationData.EmployeeID,
                this.LeaveDTO.LeaveTypeID,
                this.BaseApplicationData.ApplicationDate.Year);

            if (balance == null)
            {
                SaveErrorDetails = "Aucun solde de congé trouvé pour cet employé.";
                return false;
            }

            decimal originalDays = 0;
            if (this.Mode == enMode.Update)
            {
                var oldRecord = clsLeaveApplicationData.GetLeaveApplicationInfoByApplicationID(this.BaseApplicationData.ApplicationID);
                if (oldRecord != null) originalDays = (decimal)oldRecord.NumberOfDays;
            }

            decimal requestedDays = (decimal)this.LeaveDTO.NumberOfDays;

            // Vérification : Nouveau solde = Solde actuel + anciens jours (si update) - nouveaux jours
            if (requestedDays > (balance.LeaveBalanceDTO.RemainingDays + originalDays))
            {
                SaveErrorDetails = $"Solde insuffisant. Disponible : {balance.LeaveBalanceDTO.RemainingDays + originalDays} jours.";
                return false;
            }

            // 2. Enregistrement dans la table locale LeaveApplications
            // L'ID ApplicationID vient déjà de Kafka (Service Applications)
            bool isSaved = false;
            if (this.Mode == enMode.AddNew)
            {
                int resultID = clsLeaveApplicationData.AddNewLeaveApplication(this.LeaveDTO);
                if (resultID != -1)
                {
                    this.LeaveDTO.LeaveApplicationID = resultID;
                    isSaved = true;
                }
            }
            else
            {
                isSaved = clsLeaveApplicationData.UpdateLeaveApplication(this.LeaveDTO);
            }

            // 3. Mise à jour du solde si la sauvegarde a réussi
            if (isSaved)
            {
                balance.LeaveBalanceDTO.UsedDays = (balance.LeaveBalanceDTO.UsedDays - originalDays) + requestedDays;
                if (!balance.Save())
                {
                    SaveErrorDetails = "Le congé est sauvé, mais le solde n'a pas pu être mis à jour.";
                    return false;
                }
                this.Mode = enMode.Update;
                return true;
            }

            return false;
        }

        public static clsLeaveApplication FindByApplicationID(int ApplicationID)
        {
            ApplicationDTO appDTO = clsApplicationData.GetApplicationInfoByID(ApplicationID);

            if (appDTO != null)
            {
                LeaveApplicationDTO leaveDTO = clsLeaveApplicationData.GetLeaveApplicationInfoByApplicationID(ApplicationID);

                if (leaveDTO != null)
                {
                    return new clsLeaveApplication(appDTO, leaveDTO);
                }
            }
            return null;
        
        }

        public static int CalculateLeaveDays(DateTime StartDate, DateTime EndDate)
        {
            int count = 0;
            for (var date = StartDate.Date; date <= EndDate.Date; date = date.AddDays(1))
            {
                if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday) continue;
                if (clsPublicHoliday.IsPublicHoliday(date)) continue;
                count++;
            }
            return count;
        }
    }
}