using Microsoft.AspNetCore.Mvc;
using LOMS_Employee_Business;
using LOMS_Employee_Shared;
using System.Data;

namespace LOMS_Employee_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        // 1. RECHERCHER par ID
        [HttpGet("{id}")]
        public ActionResult<EmployeeDTO> GetEmployeeById(int id)
        {
            clsEmployee employee = clsEmployee.Find(id);
            if (employee == null) return NotFound($"ID {id} introuvable.");
            return Ok(employee.DTO);
        }

        // 2. RECHERCHER par NationalNo 
        [HttpGet("find/{nationalNo}")]
        public ActionResult<EmployeeDTO> GetEmployeeByNationalNo(string nationalNo)
        {
            clsEmployee employee = clsEmployee.Find(nationalNo);
            if (employee == null) return NotFound($"Aucun employé avec le NationalNo : {nationalNo}");
            return Ok(employee.DTO);
        }

        // 3. FILTRER par Département (Ta méthode GetEmployeesByDepartment)
        [HttpGet("department/{deptId}")]
        public IActionResult GetEmployeesByDepartment(int deptId)
        {
            DataTable dt = clsEmployee.GetEmployeesByDepartment(deptId);

            if (dt == null || dt.Rows.Count == 0)
            {
                return NotFound($"Aucun employé trouvé pour le département avec l'ID {deptId}.");
            }

            // Cela transforme la DataTable en List<Dictionary<string, object>>
            var result = clsHelper.DataTableToList(dt);

            return Ok(result);
        }

        [HttpGet("all")]
        public IActionResult GetAllEmployees()
        {
            DataTable dt = clsEmployee.GetAllEmployees();

            if (dt == null || dt.Rows.Count == 0)
                return NoContent();

            return Ok(clsHelper.DataTableToList(dt));
        }

        // 5. CRÉER (Appelle ton Save() avec RequestLeaveInitialization)
        [HttpPost]
        public ActionResult<EmployeeDTO> AddEmployee(EmployeeDTO newEmployeeDTO)
        {
            if (newEmployeeDTO == null) return BadRequest("Données invalides.");

            // Vérifier si le NationalNo existe déjà avant d'essayer d'ajouter (Ta méthode IsEmployeeExist)
            if (clsEmployee.IsEmployeeExist(newEmployeeDTO.NationalNo))
                return Conflict("Un employé avec ce NationalNo existe déjà.");

            clsEmployee employee = new clsEmployee();
            employee.DTO = newEmployeeDTO;

            if (employee.Save())
                return CreatedAtAction(nameof(GetEmployeeById), new { id = employee.DTO.EmployeeID }, employee.DTO);

            return StatusCode(500, "Erreur lors de la création (Vérifiez la connexion au service Leave).");
        }

        // 6. METTRE À JOUR (Appelle ton Save() en mode Update)
        [HttpPut("{id}")]
        public IActionResult UpdateEmployee(int id, EmployeeDTO updatedDTO)
        {
            clsEmployee employee = clsEmployee.Find(id);
            if (employee == null) return NotFound($"Employé {id} introuvable.");

            employee.DTO = updatedDTO;
            employee.DTO.EmployeeID = id;

            if (employee.Save()) return Ok(employee.DTO);
            return BadRequest("Échec de la mise à jour.");
        }

        // 7. SUPPRIMER (Ta méthode DeleteEmployee)
        [HttpDelete("{id}")]
        public IActionResult DeleteEmployee(int id)
        {
            if (!clsEmployee.IsEmployeeExist(id)) return NotFound();

            if (clsEmployee.DeleteEmployee(id))
                return Ok($"Employé {id} supprimé.");

            return BadRequest("Suppression impossible.");
        }

        // 8. VÉRIFIER EXISTENCE (IsEmployeeExist)
        [HttpGet("exists/{id}")]
        public IActionResult CheckExistence(int id)
        {
            return Ok(clsEmployee.IsEmployeeExist(id));
        }
    }
}