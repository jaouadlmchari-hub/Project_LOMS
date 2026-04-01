using LOMS_Auth_DataAccess;
using LOMS_Auth_Shared;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOMS_Auth_Business
{
    public class clsRole
    {
        public RoleDTO DTO { get; set; }
        private clsRole(RoleDTO dto)
        {
            this.DTO = dto;

        }
        public static DataTable GetAllRoles()
        {
            return clsRoleData.GetAllRoles();
        }
        public static List<string> GetRoleNames(List<int> RoleIDs)
        {
            if (RoleIDs == null || RoleIDs.Count == 0)
            {
                return new List<string>();
            }

            return clsRoleData.GetRoleNamesByIDs(RoleIDs);
        }
    }
}
