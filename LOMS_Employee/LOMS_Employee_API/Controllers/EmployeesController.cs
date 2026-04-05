using Microsoft.AspNetCore.Mvc;
using LOMS_Employee_Business;
using LOMS_Employee_Shared;
using System.Data;
using System.Threading.Tasks;

namespace LOMS_Employee_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {

        [HttpGet("{employeeID}/fullname")]
        public async Task<ActionResult<string>> GetFullName(int employeeID)
        {
            var employee = await clsEmployee.FindAsync(employeeID);

            if (employee == null)
            {
                return NotFound("Employee not found");
            }

            return Ok(employee.FullName);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeeDTO>> GetEmployeeById(int id)
        {
            clsEmployee employee = await clsEmployee.FindAsync(id);
            if (employee == null) return NotFound($"ID {id} introuvable.");
            return Ok(employee.DTO);
        }

        [HttpGet("find/{nationalNo}")]
        public async Task<ActionResult<EmployeeDTO>> GetEmployeeByNationalNo(string nationalNo)
        {
            clsEmployee employee = await clsEmployee.FindAsync(nationalNo);
            if (employee == null) return NotFound($"Aucun employé avec le NationalNo : {nationalNo}");
            return Ok(employee.DTO);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllEmployees()
        {
            DataTable dt = await clsEmployee.GetAllEmployeesAsync();
            if (dt == null || dt.Rows.Count == 0) return NoContent();
            return Ok(clsHelper.DataTableToList(dt));
        }

        [HttpPost]
        public async Task<ActionResult<EmployeeDTO>> AddEmployee(EmployeeDTO newEmployeeDTO)
        {
            if (newEmployeeDTO == null) return BadRequest("Données invalides.");

            if (await clsEmployee.IsEmployeeExistAsync(newEmployeeDTO.NationalNo))
                return Conflict("Un employé avec ce NationalNo existe déjà.");

            clsEmployee employee = new clsEmployee();
            employee.DTO = newEmployeeDTO;

            if (await employee.SaveAsync())
                return CreatedAtAction(nameof(GetEmployeeById), new { id = employee.DTO.EmployeeID }, employee.DTO);

            return StatusCode(500, "Erreur lors de la création.");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, EmployeeDTO updatedDTO)
        {
            clsEmployee employee = await clsEmployee.FindAsync(id);
            if (employee == null) return NotFound($"Employé {id} introuvable.");

            employee.DTO = updatedDTO;
            employee.DTO.EmployeeID = id;

            if (await employee.SaveAsync()) return Ok(employee.DTO);
            return BadRequest("Échec de la mise à jour.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            if (!await clsEmployee.IsEmployeeExistAsync(id)) return NotFound();

            if (await clsEmployee.DeleteEmployeeAsync(id))
                return Ok($"Employé {id} supprimé.");

            return BadRequest("Suppression impossible.");
        }
    }
}