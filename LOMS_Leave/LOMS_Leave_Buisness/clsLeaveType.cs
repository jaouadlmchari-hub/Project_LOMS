using LOMS_Leave_Shared;
using LOMS_Leave_DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOMS_Leave_Buisness
{
    public class clsLeaveType
    {
        public LeaveTypeDTO LeaveTypeDTO { get; set; }
        public enum enMode { AddNew = 0 , Update = 1}

        private enMode _Mode = enMode.AddNew;

        public clsLeaveType()
        {
            this.LeaveTypeDTO = new LeaveTypeDTO();
            this.LeaveTypeDTO.LeaveTypeID = -1;
            this.LeaveTypeDTO.LeaveName = "";
            this.LeaveTypeDTO.MaxDaysPerYear = 0;
            this.LeaveTypeDTO.IsPaid = true;
            this.LeaveTypeDTO.RequiresDocument = false;
            this.LeaveTypeDTO.IsActive = true;

            _Mode = enMode.AddNew;
        }

        private clsLeaveType(LeaveTypeDTO dTO)
        {
            this.LeaveTypeDTO = dTO;
            _Mode = enMode.Update;
        }

        private bool _AddNew()
        {
            this.LeaveTypeDTO.LeaveTypeID = clsLeaveTypeData.AddNewLeaveType(this.LeaveTypeDTO);
            return (this.LeaveTypeDTO.LeaveTypeID != -1);
        }

        private bool _Update()
        {
            return clsLeaveTypeData.UpdateLeaveType(this.LeaveTypeDTO);
        }

        public static clsLeaveType Find(int LeaveTypeID)
        {
            LeaveTypeDTO dTO = clsLeaveTypeData.GetLeaveTypeByID(LeaveTypeID);

            if (dTO != null)
                return new clsLeaveType(dTO);
            else
                return null;
        }

        public static DataTable GetAllLeaveTypes()
        {
            return clsLeaveTypeData.GettAllLeaveTypes();
        }

        public static bool Delete(int LeaveTypeID)
        {
            return clsLeaveTypeData.DeleteLeaveType(LeaveTypeID);
        }

        public bool Save()
        {
            switch (_Mode)
            {
                case enMode.AddNew:
                    if (_AddNew())
                    {
                        _Mode = enMode.Update; 
                        return true;
                    }
                    return false;

                case enMode.Update:
                    return _Update();
            }
            return false;
        }


    }
}
