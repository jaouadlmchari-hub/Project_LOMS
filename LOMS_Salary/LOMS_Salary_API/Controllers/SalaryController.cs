using Microsoft.AspNetCore.Mvc;
using LOMS_Salary_Buisness;
using LOMS_Salary_Shared;

namespace LOMS_Salary_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalaryController : ControllerBase
    {
        // 1. Récupérer par EmployeeID
        [HttpGet("by-employee/{employeeId}")]
        public ActionResult<EmployeeSalaryDTO> GetSalaryByEmployeeId(int employeeId)
        {
            var salary = clsSalary.FindByEmployeeID(employeeId);

            if (salary == null)
            {
                return NotFound($"Aucun enregistrement de salaire trouvé pour l'ID employé : {employeeId}");
            }

            return Ok(salary.DTO);
        }

        // 2. Récupérer par NationalNo (CIN)
        [HttpGet("by-national-no/{nationalNo}")]
        public ActionResult<EmployeeSalaryDTO> GetSalaryByNationalNo(string nationalNo)
        {
            var salary = clsSalary.FindByNationalNo(nationalNo);

            if (salary == null)
            {
                return NotFound($"Aucun enregistrement de salaire trouvé pour le matricule : {nationalNo}");
            }

            return Ok(salary.DTO);
        }

        [HttpPost("add")]
        public IActionResult AddSalary([FromBody] EmployeeSalaryDTO request)
        {
            int newId = clsSalary.AddNewSalary(request.EmployeeID, request.Salary, request.CreatedByUserID);

            if (newId != -1) return Ok(new { SalaryID = newId });
            return BadRequest("Impossible de créer le salaire.");
        }
    }
}