using Microsoft.AspNetCore.Mvc;
using LOMS_Auth_Business;
using LOMS_Auth_Shared;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LOMS_Auth_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        // 1. GET: api/users (Liste complète fusionnée SQL + API Employee)
        [HttpGet]
        public async Task<ActionResult<List<Dictionary<string, object>>>> GetAllUsers()
        {
            DataTable dt = await clsUser.GetAllUsersWithDetailsAsync();

            if (dt == null || dt.Rows.Count == 0)
                return NotFound("Aucun utilisateur trouvé.");

            // Utilisation  Helper pour la sérialisation propre
            return Ok(clsHelper.DataTableToList(dt));
        }

        // 2. GET: api/users/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<clsUser>> GetUserById(int id)
        {
            var user = await clsUser.FindByUserIDAsync(id);
            if (user == null)
                return NotFound($"Utilisateur avec l'ID {id} non trouvé.");

            return Ok(user);
        }

        // 3. POST: api/users/login
        [HttpPost("login")]
        public async Task<ActionResult<clsUser>> Login([FromBody] LoginRequest login)
        {
            if (string.IsNullOrEmpty(login.UserName) || string.IsNullOrEmpty(login.Password))
                return BadRequest("Nom d'utilisateur ou mot de passe manquant.");

            var user = await clsUser.FindByUserNameAndPasswordAsync(login.UserName, login.Password);

            if (user == null)
                return Unauthorized("Identifiants incorrects.");

            return Ok(user);
        }

        // 4. POST: api/users (Ajout d'un nouvel utilisateur)
        [HttpPost]
        public ActionResult CreateUser([FromBody] UserDTO userDto)
        {
            if (userDto == null) return BadRequest("Données invalides.");

            clsUser newUser = new clsUser();
            newUser.DTO = userDto;

            if (newUser.Save())
            {
                // On retourne 201 Created avec le lien vers l'objet
                return CreatedAtAction(nameof(GetUserById), new { id = newUser.DTO.UserID }, newUser);
            }

            return StatusCode(500, "Erreur interne lors de la création de l'utilisateur.");
        }

        // 5. PUT: api/users/{id} (Mise à jour complète)
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateUser(int id, [FromBody] UserDTO userDto)
        {
            var user = await clsUser.FindByUserIDAsync(id);
            if (user == null) return NotFound();

            user.DTO = userDto;
            user.DTO.UserID = id; // Sécurité : on s'assure que l'ID correspond

            if (user.Save()) return NoContent();

            return StatusCode(500, "Erreur lors de la mise à jour.");
        }

        // 6. PATCH: api/users/{id}/status (Changement d'état actif/inactif)
        [HttpPatch("{id}/status")]
        public ActionResult SetStatus(int id, [FromBody] bool isActive)
        {
            if (clsUser.SetUserStatus(id, isActive)) return Ok();
            return BadRequest("Impossible de modifier le statut.");
        }

        // 7. DELETE: api/users/{id}
        [HttpDelete("{id}")]
        public ActionResult DeleteUser(int id)
        {
            if (clsUser.DeleteUser(id)) return Ok();
            return BadRequest("Échec de la suppression (l'utilisateur possède peut-être des liaisons).");
        }

        // 8. GET: api/users/check-username/{username}
        [HttpGet("check-username/{username}")]
        public ActionResult<bool> IsUsernameTaken(string username)
        {
            return Ok(clsUser.IsUserExist(username));
        }
    }

    // DTO simple pour la réception des données de connexion
    public class LoginRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}