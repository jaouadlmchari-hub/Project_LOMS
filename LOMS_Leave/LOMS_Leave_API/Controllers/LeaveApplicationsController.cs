using LOMS_Leave_Buisness;
using LOMS_Leave_DataAccess;
using LOMS_Leave_Shared;
using Microsoft.AspNetCore.Mvc;

namespace LOMS_Leave_API.Controllers
{
    [ApiController]
    [Route("api/leave-applications")]
    public class LeaveApplicationsController : ControllerBase
    {
        // 1. GET : Trouver une demande par son ApplicationID (L'ID global de Kafka)
        [HttpGet("{applicationId}")]
        public ActionResult<clsLeaveApplication> GetLeaveByApplicationID(int applicationId)
        {
            var leaveApp = clsLeaveApplication.FindByApplicationID(applicationId);

            if (leaveApp == null)
                return NotFound($"Demande de congé avec l'ID {applicationId} non trouvée.");

            // On retourne l'objet complet (Base + Leave spécifique)
            return Ok(new
            {
                BaseData = leaveApp.BaseApplicationData,
                LeaveData = leaveApp.LeaveDTO,
                LeaveTypeName = leaveApp.LeaveTypeName
            });
        }

        // 2. POST : Calculer le nombre de jours (Utile pour le formulaire React)
        // Route : api/leave-applications/calculate-days
        [HttpGet("calculate-days")]
        public ActionResult<int> GetLeaveDaysCount(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate) return BadRequest("La date de début doit être avant la date de fin.");

            int days = clsLeaveApplication.CalculateLeaveDays(startDate, endDate);
            return Ok(days);
        }

        // 3. POST : Sauvegarde locale (Utilisée par le Kafka Consumer)
        [HttpPost]
        public ActionResult CreateLeaveApplication(LeaveApplicationDTO leaveDTO)
        {
            if (leaveDTO == null) return BadRequest("Données invalides.");

            clsLeaveApplication leaveApp = new clsLeaveApplication();
            leaveApp.LeaveDTO = leaveDTO;

            // Note: Pour un AddNew, les BaseApplicationData doivent être déjà présentes 
            // dans la table miroir via Kafka.
            var baseData = clsApplicationData.GetApplicationInfoByID(leaveDTO.ApplicationID);
            if (baseData == null)
                return BadRequest("L'application de base n'existe pas encore dans le cache local.");

            leaveApp.BaseApplicationData = baseData;

            if (leaveApp.Save())
            {
                return Ok(new { message = "Congé enregistré et solde mis à jour." });
            }

            // Si le Save échoue (ex: solde insuffisant), on renvoie l'erreur métier
            return BadRequest(new { error = leaveApp.SaveErrorDetails });
        }

        // 4. PUT : Mettre à jour une demande existante
        [HttpPut("{applicationId}")]
        public ActionResult UpdateLeaveApplication(int applicationId, LeaveApplicationDTO leaveDTO)
        {
            if (applicationId != leaveDTO.ApplicationID) return BadRequest("ID mismatch.");

            var leaveApp = clsLeaveApplication.FindByApplicationID(applicationId);
            if (leaveApp == null) return NotFound();

            leaveApp.LeaveDTO = leaveDTO;

            if (leaveApp.Save())
                return Ok(new { message = "Mise à jour réussie." });

            return BadRequest(new { error = leaveApp.SaveErrorDetails });
        }
    }
}