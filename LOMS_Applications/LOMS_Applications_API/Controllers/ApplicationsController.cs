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
        // 1. Récupérer toutes les demandes
        [HttpGet("all")]
        public ActionResult<List<Dictionary<string, object>>> GetAllApplications()
        {
            DataTable dt = clsApplication.GetAllApplications();

            if (dt == null || dt.Rows.Count == 0)
                return NotFound(new { Message = "Aucune demande trouvée." });

            var result = clsHelper.DataTableToList(dt);
            return Ok(result);
        }

        // 2. Récupérer une demande spécifique (Détails complets)
        [HttpGet("{id}")]
        public async Task<ActionResult<dynamic>> GetApplicationById(int id)
        {
            clsApplication app = clsApplication.Find(id);

            if (app == null)
                return NotFound(new { Message = $"Demande ID {id} introuvable." });

            string employeeName = await app.GetEmployeeFullNameAsync();
            string actionByName = await app.GetActionByUserNameAsync();

            return Ok(new
            {
                ApplicationDetails = app.DTO,
                EmployeeFullName = employeeName,
                ActionByUserName = actionByName
            });
        }

        // 3. (POST) - Maintenant ASYNC
        [HttpPost("add")]
        public async Task<ActionResult> AddApplication([FromBody] ApplicationDTO newAppDto)
        {
            if (newAppDto == null) return BadRequest("Données invalides.");

            clsApplication app = new clsApplication();

            // Mapping des données
            app.DTO.EmployeeID = newAppDto.EmployeeID;
            app.DTO.ApplicationTypeID = newAppDto.ApplicationTypeID;
            app.DTO.CreatedByUserID = newAppDto.CreatedByUserID;
            app.DTO.Notes = newAppDto.Notes;

            // Utilisation de AWAIT ici pour ne pas bloquer le serveur
            if (await app.SaveAsync())
            {
                return CreatedAtAction(nameof(GetApplicationById),
                    new { id = app.DTO.ApplicationID }, app.DTO);
            }

            return StatusCode(500, "Erreur lors de la création ou de la notification.");
        }

        // 4. Mettre à jour une demande existante (PUT) 
        [HttpPut("update/{id}")]
        public async Task<ActionResult> UpdateApplication(int id, [FromBody] ApplicationDTO updateDto)
        {
            clsApplication app = clsApplication.Find(id);
            if (app == null) return NotFound($"Demande {id} introuvable.");

            // Mise à jour des champs autorisés
            app.DTO.Notes = updateDto.Notes;
            app.DTO.Status = updateDto.Status;
            app.DTO.LastStatusDate = DateTime.Now;
            app.DTO.ActionByUserID = updateDto.ActionByUserID;

            if (await app.SaveAsync())
            {
                return Ok(new { Message = "Demande mise à jour.", Data = app.DTO });
            }

            return StatusCode(500, "Erreur lors de la mise à jour.");
        }

        [HttpPatch("update-status/{id}")]
        public ActionResult UpdateStatus(int id, [FromQuery] short newStatus)
        {
            if (clsApplication.UpdateStatus(id, newStatus))
            {
                return Ok(new { Message = "Statut mis à jour avec succès." });
            }
            return BadRequest("Échec de la mise à jour du statut.");
        }

        // 6. Supprimer une demande
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