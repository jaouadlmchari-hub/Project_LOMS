using Microsoft.AspNetCore.Mvc;
using LOMS_Employee_Business;
using LOMS_Employee_Shared;
using System.Data;

namespace LOMS_Employee_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobsController : ControllerBase
    {
        // 1. GET: api/Jobs/5
        [HttpGet("{id}")]
        public ActionResult<JobTitleDTO> GetJobById(int id)
        {
            clsJob job = clsJob.Find(id);

            if (job == null)
            {
                return NotFound($"Titre de poste avec l'ID {id} non trouvé.");
            }

            return Ok(job.DTO);
        }

        // 2. GET: api/Jobs/find/{titleName}
        [HttpGet("find/{titleName}")]
        public ActionResult<JobTitleDTO> GetJobByTitleName(string titleName)
        {
            clsJob job = clsJob.Find(titleName);

            if (job == null)
            {
                return NotFound($"Le titre de poste '{titleName}' n'existe pas.");
            }

            return Ok(job.DTO);
        }

        // 3. GET: api/Jobs/all
        [HttpGet("all")]
        public IActionResult GetAllJobs()
        {
            DataTable dt = clsJob.GetAllJobTitles();

            if (dt == null || dt.Rows.Count == 0)
            {
                return NoContent();
            }

            // Conversion via le Helper pour le JSON
            return Ok(clsHelper.DataTableToList(dt));
        }

        // 4. GET: api/Jobs/department/{deptId}
        [HttpGet("department/{deptId}")]
        public IActionResult GetJobsByDepartment(int deptId)
        {
            DataTable dt = clsJob.GetAllJobTitlesByDepartmentID(deptId);

            if (dt == null || dt.Rows.Count == 0)
            {
                return NotFound($"Aucun titre de poste trouvé pour le département {deptId}.");
            }

            return Ok(clsHelper.DataTableToList(dt));
        }
    }
}