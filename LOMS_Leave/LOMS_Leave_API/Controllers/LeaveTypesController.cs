using Microsoft.AspNetCore.Mvc;
using LOMS_Leave_Buisness;
using LOMS_Leave_Shared;
using System.Data;

namespace LOMS_Leave_API.Controllers
{
    [ApiController]
    [Route("api/leave-types")]
    public class LeaveTypesController : ControllerBase
    {
        // 1. GET ALL : api/leave-types
        [HttpGet]
        public ActionResult<IEnumerable<LeaveTypeDTO>> GetAllLeaveTypes()
        {
            DataTable dt = clsLeaveType.GetAllLeaveTypes();

            if (dt.Rows.Count == 0)
                return NotFound(new { message = "No leave types found." });

            var leaveTypesList = dt.AsEnumerable().Select(row => new LeaveTypeDTO
            {
                LeaveTypeID = Convert.ToInt32(row["LeaveTypeID"]),
                LeaveName = row["LeaveName"].ToString(),
                MaxDaysPerYear = (short)Convert.ToInt32(row["MaxDaysPerYear"]),
                IsPaid = Convert.ToBoolean(row["IsPaid"]),
                RequiresDocument = Convert.ToBoolean(row["RequiresDocument"]),
                IsActive = Convert.ToBoolean(row["IsActive"])
            }).ToList();

            return Ok(leaveTypesList);
        }

        // 2. GET BY ID : api/leave-types/{id}
        [HttpGet("{id}", Name = "GetLeaveTypeById")]
        public ActionResult<LeaveTypeDTO> GetLeaveTypeById(int id)
        {
            if (id <= 0) return BadRequest("Invalid ID.");

            clsLeaveType leaveType = clsLeaveType.Find(id);

            if (leaveType == null)
                return NotFound($"Leave type with ID {id} not found.");

            return Ok(leaveType.LeaveTypeDTO);
        }

        // 3. POST (CREATE) : api/leave-types
        [HttpPost]
        public ActionResult<LeaveTypeDTO> CreateLeaveType(LeaveTypeDTO leaveTypeDTO)
        {
            if (leaveTypeDTO == null || string.IsNullOrEmpty(leaveTypeDTO.LeaveName))
                return BadRequest("Invalid leave type data.");

            clsLeaveType leaveType = new clsLeaveType();
            
            // Map les données du DTO vers l'objet Business
            leaveType.LeaveTypeDTO.LeaveName = leaveTypeDTO.LeaveName;
            leaveType.LeaveTypeDTO.MaxDaysPerYear = leaveTypeDTO.MaxDaysPerYear;
            leaveType.LeaveTypeDTO.IsPaid = leaveTypeDTO.IsPaid;
            leaveType.LeaveTypeDTO.RequiresDocument = leaveTypeDTO.RequiresDocument;
            leaveType.LeaveTypeDTO.IsActive = leaveTypeDTO.IsActive;

            if (leaveType.Save())
            {
                return CreatedAtRoute("GetLeaveTypeById", new { id = leaveType.LeaveTypeDTO.LeaveTypeID }, leaveType.LeaveTypeDTO);
            }

            return StatusCode(500, "An error occurred while creating the leave type.");
        }

        // 4. PUT (UPDATE) : api/leave-types/{id}
        [HttpPut("{id}")]
        public ActionResult<LeaveTypeDTO> UpdateLeaveType(int id, LeaveTypeDTO leaveTypeDTO)
        {
            if (id <= 0 || leaveTypeDTO == null) return BadRequest("Invalid request.");

            clsLeaveType leaveType = clsLeaveType.Find(id);

            if (leaveType == null)
                return NotFound($"Leave type with ID {id} not found.");

            // Mise à jour des propriétés
            leaveType.LeaveTypeDTO.LeaveName = leaveTypeDTO.LeaveName;
            leaveType.LeaveTypeDTO.MaxDaysPerYear = leaveTypeDTO.MaxDaysPerYear;
            leaveType.LeaveTypeDTO.IsPaid = leaveTypeDTO.IsPaid;
            leaveType.LeaveTypeDTO.RequiresDocument = leaveTypeDTO.RequiresDocument;
            leaveType.LeaveTypeDTO.IsActive = leaveTypeDTO.IsActive;

            if (leaveType.Save())
                return Ok(leaveType.LeaveTypeDTO);

            return StatusCode(500, "An error occurred while updating the leave type.");
        }

        // 5. DELETE : api/leave-types/{id}
        [HttpDelete("{id}")]
        public ActionResult DeleteLeaveType(int id)
        {
            if (id <= 0) return BadRequest("Invalid ID.");

            if (clsLeaveType.Delete(id))
                return Ok(new { message = $"Leave type {id} deleted successfully." });

            return NotFound($"Leave type {id} not found or has linked records.");
        }
    }
}