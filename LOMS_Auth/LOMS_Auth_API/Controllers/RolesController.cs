using Microsoft.AspNetCore.Mvc;
using LOMS_Auth_Business;
using LOMS_Auth_Shared;
using System.Collections.Generic;
using System.Data;

namespace LOMS_Auth_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolesController : ControllerBase
    {
        // 1. GET: api/roles
        // Récupère tous les rôles (ID et Nom) pour l'UI
        [HttpGet]
        public ActionResult<List<Dictionary<string, object>>> GetAllRoles()
        {
            DataTable dt = clsRole.GetAllRoles();

            if (dt == null || dt.Rows.Count == 0)
            {
                return NotFound("Aucun rôle défini dans la base de données.");
            }

            // pour éviter l'erreur de sérialisation
            return Ok(clsHelper.DataTableToList(dt));
        }

        // 2. POST: api/roles/names
        [HttpPost("names")]
        public ActionResult<List<string>> GetRoleNames([FromBody] List<int> roleIDs)
        {
            if (roleIDs == null || roleIDs.Count == 0)
            {
                return BadRequest("La liste des IDs est vide.");
            }

            List<string> roleNames = clsRole.GetRoleNames(roleIDs);
            return Ok(roleNames);
        }
    }
}