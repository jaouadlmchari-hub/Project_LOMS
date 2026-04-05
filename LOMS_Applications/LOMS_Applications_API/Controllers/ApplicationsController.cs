using Microsoft.AspNetCore.Mvc;
using LOMS_Applications_Buisness;
using LOMS_Applications_Shared;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LOMS_Applications.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationsController : ControllerBase
    {
        // 1. Récupérer toutes les demandes (En utilisant clsHelper)
        [HttpGet("all")]
        public ActionResult<List<Dictionary<string, object>>> GetAllApplications()
        {
            DataTable dt = clsApplication.GetAllApplications();

            if (dt == null || dt.Rows.Count == 0)
                return NotFound(new { Message = "Aucune demande trouvée." });

            // Utilisation de ta méthode centralisée dans la BLL
            var result = clsHelper.DataTableToList(dt);

            return Ok(result);
        }

        // 2. Récupérer une demande spécifique avec les noms (Async)
        [HttpGet("{id}")]
        public async Task<ActionResult<dynamic>> GetApplicationById(int id)
        {
            clsApplication app = clsApplication.Find(id);

            if (app == null)
                return NotFound(new { Message = $"Demande ID {id} introuvable." });

            // Appels asynchrones vers les autres microservices
            string employeeName = await app.GetEmployeeFullNameAsync();
            string actionByName = await app.GetActionByUserNameAsync();

            return Ok(new
            {
                ApplicationDetails = app.DTO,
                EmployeeFullName = employeeName,
                ActionByUserName = actionByName
            });
        }

        // 3. Ajouter une demande
        [HttpPost("add")]
        public ActionResult AddApplication([FromBody] ApplicationDTO newAppDto)
        {
            if (newAppDto == null) return BadRequest("Données invalides.");

            clsApplication app = new clsApplication();

            // Mapping des données
            app.DTO.EmployeeID = newAppDto.EmployeeID;
            app.DTO.ApplicationTypeID = newAppDto.ApplicationTypeID;
            app.DTO.CreatedByUserID = newAppDto.CreatedByUserID;
            app.DTO.Notes = newAppDto.Notes;
            app.DTO.ApplicationDate = DateTime.Now;
            app.DTO.Status = "Pending";
            app.DTO.LastStatusDate = DateTime.Now;

            if (app.Save())
            {
                return CreatedAtAction(nameof(GetApplicationById), new { id = app.DTO.ApplicationID }, app.DTO);
            }

            return StatusCode(500, "Erreur lors de la sauvegarde.");
        }

        // 4. Mettre à jour le statut (PATCH est plus adapté pour une modification partielle)
        [HttpPatch("update-status/{id}")]
        public ActionResult UpdateStatus(int id, [FromQuery] short newStatus)
        {
            if (clsApplication.UpdateStatus(id, newStatus))
            {
                return Ok(new { Message = "Statut mis à jour avec succès." });
            }
            return BadRequest("Échec de la mise à jour du statut.");
        }

        // 5. Supprimer une demande
        [HttpDelete("delete/{id}")]
        public ActionResult DeleteApplication(int id)
        {
            clsApplication app = clsApplication.Find(id);
            if (app == null) return NotFound();

            if (app.Delete())
            {
                return Ok(new { Message = "Demande supprimée." });
            }

            return StatusCode(500, "Erreur lors de la suppression.");
        }
    }
}