using Microsoft.AspNetCore.Mvc;
using LOMS_Employee_Business; // Pour accéder à clsCountry
using System.Data;

namespace LOMS_Employee_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountriesController : ControllerBase
    {
        // GET: api/Countries
        // Cette méthode retourne la liste de tous les pays
        [HttpGet("all")]
        public IActionResult GetAllCountries()
        {
            DataTable dt = clsCountry.GetAllCountries();

            if (dt == null || dt.Rows.Count == 0)
                return NotFound("Aucun pays trouvé.");

            return Ok(clsHelper.DataTableToList(dt));
        }

        // GET: api/Countries/5
        // Cette méthode cherche un pays spécifique par son ID
        [HttpGet("{id}")]
        public ActionResult<clsCountry> GetCountryById(int id)
        {
            if (id <= 0)
                return BadRequest("ID invalide.");

            clsCountry country = clsCountry.Find(id);

            if (country == null)
            {
                return NotFound($"Le pays avec l'ID {id} n'existe pas.");
            }

            return Ok(country);
        }
    }
}