using LOMS_Employee_DataAccess;
using LOMS_Employee_Shared;
using System;
using System.Data;

namespace LOMS_Employee_Business
{
    public class clsDepartment
    {
        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode { get; private set; }
        public DepartmentDTO DTO { get; set; }

        // Constructeur par défaut (Mode Ajout)
        public clsDepartment()
        {
            this.DTO = new DepartmentDTO();
            this.Mode = enMode.AddNew;
        }

        // Constructeur interne (Mode Update)
        private clsDepartment(DepartmentDTO departmentDTO)
        {
            this.DTO = departmentDTO;
            this.Mode = enMode.Update;
        }

        private bool _AddNewDepartment()
        {
            this.DTO.DepartmentID = clsDepartmentData.AddNewDepartment(this.DTO);
            return (this.DTO.DepartmentID != -1);
        }

        private bool _UpdateDepartment()
        {
            return clsDepartmentData.UpdateDepartment(this.DTO);
        }

        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if (_AddNewDepartment())
                    {
                        this.Mode = enMode.Update; // Changement de mode réussi
                        return true;
                    }
                    return false;

                case enMode.Update:
                    return _UpdateDepartment();
            }
            return false;
        }

        #region Méthodes Statiques

        public static clsDepartment Find(int departmentID)
        {
            DepartmentDTO dto = clsDepartmentData.GetDepartmentInfoByID(departmentID);
            return (dto != null) ? new clsDepartment(dto) : null;
        }

        public static clsDepartment Find(string departmentName)
        {
            DepartmentDTO dto = clsDepartmentData.GetDepartmentInfoByName(departmentName);
            return (dto != null) ? new clsDepartment(dto) : null;
        }

        public static DataTable GetAllDepartments()
        {
            return clsDepartmentData.GetAllDepartment();
        }

        public static DataTable GetDepartmentSummary()
        {
            return clsDepartmentData.GetDepartmentSummary();
        }

        public static bool SetStatus(int departmentID, bool newStatus)
        {
            return clsDepartmentData.UpdateStatus(departmentID, newStatus);
        }

        #endregion
    }
}