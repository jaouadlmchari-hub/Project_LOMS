using Microsoft.AspNetCore.Mvc;
using LOMS_Leave_Buisness;
using LOMS_Leave_Shared;
using System.Data;

namespace LOMS_Leave_API.Controllers
{
    [ApiController]
    [Route("api/leave-balances")]
    public class LeaveBalancesController : ControllerBase
    {
        
        [HttpGet("employee/{employeeID}/year/{year}")]
        public ActionResult<IEnumerable<LeaveBalanceDTO>> GetEmployeeBalances(int employeeID, int year)
        {
            if (employeeID <= 0 || year < 2000) return BadRequest("Invalid parameters.");

            DataTable dt = clsLeaveBalance.GetEmployeeBalances(employeeID, year);

            if (dt.Rows.Count == 0)
                return NotFound(new { message = "No balances found for this employee in the specified year." });

            var balancesList = dt.AsEnumerable().Select(row => new LeaveBalanceDTO
            {
                LeaveBalanceID = Convert.ToInt32(row["LeaveBalanceID"]),
                EmployeeID = Convert.ToInt32(row["EmployeeID"]),
                LeaveTypeID = Convert.ToInt32(row["LeaveTypeID"]),
                Year = Convert.ToInt32(row["Year"]),
                EntitledDays = Convert.ToDecimal(row["EntitledDays"]),
                UsedDays = Convert.ToDecimal(row["UsedDays"]),
                RemainingDays = Convert.ToDecimal(row["RemainingDays"])
            }).ToList();

            return Ok(balancesList);
        }

        // 2. GET : Récupérer un solde spécifique par son ID
        [HttpGet("{id}")]
        public ActionResult<LeaveBalanceDTO> GetBalanceByID(int id)
        {
            clsLeaveBalance balance = clsLeaveBalance.FindByID(id);

            if (balance == null)
                return NotFound($"Balance with ID {id} not found.");

            return Ok(balance.LeaveBalanceDTO);
        }

        // 3. POST : Initialiser les soldes pour un nouvel employé ou une nouvelle année
        // Route : api/leave-balances/initialize
        [HttpPost("initialize")]
        public ActionResult Initialize(int employeeID, int year)
        {
            if (employeeID <= 0 || year < 2000) return BadRequest("Invalid parameters.");

            if (clsLeaveBalance.InitializeBalances(employeeID, year))
            {
                return Ok(new { message = $"Balances initialized for Employee {employeeID} for year {year}." });
            }

            return StatusCode(500, "Error during initialization. Balances might already exist.");
        }

        // 4. PATCH/PUT : Mettre à jour les jours utilisés (très utile pour  UI)
        // Route : api/leave-balances/update-used-days
        [HttpPut("update-used-days")]
        public ActionResult UpdateUsedDays(int employeeID, int leaveTypeID, int year, decimal daysCount)
        {
            if (daysCount <= 0) return BadRequest("Days count must be greater than 0.");

            // La BLL clsLeaveBalance.UpdateUsedDays vérifie déjà si le solde est suffisant
            if (clsLeaveBalance.UpdateUsedDays(employeeID, leaveTypeID, year, daysCount))
            {
                return Ok(new { message = "Used days updated successfully." });
            }

            return BadRequest("Could not update used days. Check if the balance is sufficient or exists.");
        }

        // 5. PUT : Mise à jour manuelle (Admin) d'un solde (Entitled ou Used)
        [HttpPut("{id}")]
        public ActionResult UpdateBalance(int id, LeaveBalanceDTO dto)
        {
            if (id != dto.LeaveBalanceID) return BadRequest("ID mismatch.");

            clsLeaveBalance balance = clsLeaveBalance.FindByID(id);
            if (balance == null) return NotFound();

            balance.LeaveBalanceDTO.EntitledDays = dto.EntitledDays;
            balance.LeaveBalanceDTO.UsedDays = dto.UsedDays;

            if (balance.Save())
                return Ok(balance.LeaveBalanceDTO);

            return StatusCode(500, "Error updating balance.");
        }
    }
}