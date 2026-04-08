using LOMS_Leave_DataAccess;
using LOMS_Leave_Shared;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOMS_Leave_Buisness
{
    public class clsPublicHoliday
    {
        public enum enMode { AddNew = 0, Update = 1 };

        public enMode Mode = enMode.AddNew;

        public PublicHolidaysDTO DTO { get; set; }

        public clsPublicHoliday()
        {
            this.DTO = new PublicHolidaysDTO();
            this.DTO.HolidayID = -1;
            this.DTO.HolidayName = "";
            this.DTO.HolidayDate = DateTime.Now;
            this.DTO.IsRepeatedAnnually = false;

            Mode = enMode.AddNew;
        }

        private clsPublicHoliday(PublicHolidaysDTO holidayDTO)
        {
            this.DTO = holidayDTO;
            Mode = enMode.Update;
        }

        private bool _AddNew()
        {
            this.DTO.HolidayID = clsPublicHolidayData.AddNewPublicHoliday(this.DTO);
            return (this.DTO.HolidayID != -1);
        }

        private bool _Update()
        {
            return clsPublicHolidayData.UpdatePublicHoliday(this.DTO);
        }

        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if (_AddNew())
                    {
                        Mode = enMode.Update;
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                case enMode.Update:
                    return _Update();
            }

            return false;
        }

        public static bool Delete(int holidayID)
        {
            return clsPublicHolidayData.DeletePublicHoliday(holidayID);
        }

        public static DataTable GetAllPublicHolidays()
        {
            return clsPublicHolidayData.GetAllPublicHolidays();
        }

        public static bool IsPublicHoliday(DateTime date)
        {
            return clsPublicHolidayData.IsPublicHoliday(date);
        }

        public static clsPublicHoliday Find(int holidayID)
        {
            PublicHolidaysDTO holidayDTO = new PublicHolidaysDTO();

            if (clsPublicHolidayData.GetPublicHolidayInfoByID(holidayID, ref holidayDTO))
                return new clsPublicHoliday(holidayDTO);
            else
                return null;
        }
    }
}
