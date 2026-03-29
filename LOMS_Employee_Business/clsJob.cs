using LOMS_Employee_DataAccess;
using LOMS_Employee_Shared; 
using System;
using System.Data;

namespace LOMS_Employee_Business
{
    public class clsJob
    {
        // Utilisation du DTO partagé
        public JobTitleDTO DTO { get; set; }

        // Constructeur privé pour l'encapsulation
        private clsJob(JobTitleDTO dto)
        {
            this.DTO = dto;
        }

        #region Méthodes Statiques

        public static clsJob Find(int jobTitleID)
        {
            JobTitleDTO dto = clsJobData.GetJobInfoByID(jobTitleID);
            return (dto != null) ? new clsJob(dto) : null;
        }

        public static clsJob Find(string titleName)
        {
            JobTitleDTO dto = clsJobData.GetJobInfoByName(titleName);
            return (dto != null) ? new clsJob(dto) : null;
        }

        public static DataTable GetAllJobTitles()
        {
            return clsJobData.GetAllJobTitles();
        }

        public static DataTable GetAllJobTitlesByDepartmentID(int departmentID)
        {
            return clsJobData.GetAllJobTitlesByDepartmentID(departmentID);
        }

        #endregion
    }
}