using Microsoft.AspNetCore.Mvc;
using LOMS_Employee_Business;
using LOMS_Employee_Shared;
using System.Data;

namespace LOMS_Employee_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {
        // 1. GET: api/Departments/5
        [HttpGet("{id}")]
        public ActionResult<DepartmentDTO> GetDepartmentById(int id)
        {
            clsDepartment dept = clsDepartment.Find(id);
            if (dept == null)
                return NotFound($"Département avec l'ID {id} non trouvé.");

            return Ok(dept.DTO);
        }

        // 2. GET: api/Departments/find/{name}
        [HttpGet("find/{name}")]
        public ActionResult<DepartmentDTO> GetDepartmentByName(string name)
        {
            clsDepartment dept = clsDepartment.Find(name);
            if (dept == null)
                return NotFound($"Département '{name}' non trouvé.");

            return Ok(dept.DTO);
        }

        // 3. GET: api/Departments/all
        [HttpGet("all")]
        public IActionResult GetAllDepartments()
        {
            DataTable dt = clsDepartment.GetAllDepartments();
            if (dt == null || dt.Rows.Count == 0) return NoContent();

            return Ok(clsHelper.DataTableToList(dt));
        }

        // 4. GET: api/Departments/summary
        [HttpGet("summary")]
        public IActionResult GetDepartmentSummary()
        {
            DataTable dt = clsDepartment.GetDepartmentSummary();
            if (dt == null || dt.Rows.Count == 0) return NoContent();

            return Ok(clsHelper.DataTableToList(dt));
        }

        // 5. POST: api/Departments
        [HttpPost]
        public ActionResult<DepartmentDTO> CreateDepartment(DepartmentDTO deptDTO)
        {
            if (deptDTO == null) return BadRequest("Données invalides.");

            clsDepartment dept = new clsDepartment();
            dept.DTO = deptDTO;

            if (dept.Save())
            {
                return CreatedAtAction(nameof(GetDepartmentById), new { id = dept.DTO.DepartmentID }, dept.DTO);
            }

            return BadRequest("Échec de la création du département.");
        }

        // 6. PUT: api/Departments/5
        [HttpPut("{id}")]
        public IActionResult UpdateDepartment(int id, DepartmentDTO updatedDTO)
        {
            clsDepartment dept = clsDepartment.Find(id);
            if (dept == null) return NotFound();

            dept.DTO = updatedDTO;
            dept.DTO.DepartmentID = id;

            if (dept.Save()) return Ok(dept.DTO);

            return BadRequest("Échec de la mise à jour.");
        }

        // 7. PATCH: api/Departments/5/status
        [HttpPatch("{id}/status")]
        public IActionResult SetStatus(int id, [FromBody] bool newStatus)
        {
            if (clsDepartment.SetStatus(id, newStatus))
                return Ok(new { Message = "Statut mis à jour avec succès." });

            return BadRequest("Impossible de mettre à jour le statut.");
        }
    }
}