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
    public class clsLeaveBalance
    {
        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode = enMode.AddNew;
        public LeaveBalanceDTO LeaveBalanceDTO { get; set; }

        public clsLeaveBalance()
        {
            this.LeaveBalanceDTO = new LeaveBalanceDTO();
            this.Mode = enMode.AddNew;
        }

        public clsLeaveBalance(LeaveBalanceDTO dto)
        {
            this.LeaveBalanceDTO = dto;
            this.Mode = enMode.Update;
        }

        public static clsLeaveBalance FindBalanceByEmployeeAndType(int employeeID, int leaveTypeID, int year)
        {
            LeaveBalanceDTO dto = clsLeaveBalanceData.GetBalanceByEmployeeAndType(employeeID, leaveTypeID, year);

            if (dto != null)
                return new clsLeaveBalance(dto); 
            else
                return null;
        }

        public static bool InitializeBalances(int EmployeeID, int Year)
        {
            return clsLeaveBalanceData.InitializeBalances(EmployeeID, Year);
        }

        public static bool UpdateUsedDays(int EmployeeID, int LeaveTypeID, int Year, decimal DaysCount)
        {
            clsLeaveBalance balance = clsLeaveBalance.FindBalanceByEmployeeAndType(EmployeeID, LeaveTypeID, Year);

            if (balance == null)
            {
                return false;
            }

            
            if (balance.LeaveBalanceDTO.RemainingDays < DaysCount)
            {
                return false;
            }

            return clsLeaveBalanceData.UpdateUsedDays(EmployeeID, LeaveTypeID, Year, DaysCount);
        }

        public bool Save()
        {
            switch (Mode)
            {
                case enMode.Update:
                    return clsLeaveBalanceData.UpdateBalance(
                        this.LeaveBalanceDTO.LeaveBalanceID,
                        this.LeaveBalanceDTO.EntitledDays,
                        this.LeaveBalanceDTO.UsedDays
                    );
                default:
                    return false;
            }
        }

        public static DataTable GetEmployeeBalances(int EmployeeID, int Year)
        {
            return clsLeaveBalanceData.GetEmployeeBalances(EmployeeID, Year);
        }

        public static clsLeaveBalance FindByID(int LeaveBalanceID)
        {
            LeaveBalanceDTO dto = clsLeaveBalanceData.GetLeaveBalanceByID(LeaveBalanceID);

            if (dto != null)
            {
                return new clsLeaveBalance(dto);
            }
            else
            {
                return null;
            }
        }
    }
}
