using Microsoft.AspNetCore.Mvc;
using LOMS_Leave_Buisness;
using LOMS_Leave_Shared;
using System.Data;

namespace LOMS_Leave_API.Controllers
{
    [ApiController]
    [Route("api/holidays")] 
    public class PublicHolidaysController : ControllerBase
    {
        // 1. GET ALL : api/holidays
        [HttpGet]
        public ActionResult<IEnumerable<PublicHolidaysDTO>> GetAllHolidays()
        {
            DataTable dt = clsPublicHoliday.GetAllPublicHolidays();

            if (dt.Rows.Count == 0)
                return NotFound("No public holidays found.");

            // Conversion DataTable vers List<DTO> pour le format JSON
            var holidaysList = dt.AsEnumerable().Select(row => new PublicHolidaysDTO
            {
                HolidayID = Convert.ToInt32(row["HolidayID"]),
                HolidayName = row["HolidayName"].ToString(),
                HolidayDate = Convert.ToDateTime(row["HolidayDate"]),
                IsRepeatedAnnually = Convert.ToBoolean(row["IsRepeatedAnnually"])
            }).ToList();

            return Ok(holidaysList);
        }

        // 2. GET BY ID : api/holidays/{id}
        [HttpGet("{id}", Name = "GetHolidayById")]
        public ActionResult<PublicHolidaysDTO> GetHolidayById(int id)
        {
            if (id <= 0) return BadRequest("Invalid ID.");

            clsPublicHoliday holiday = clsPublicHoliday.Find(id);

            if (holiday == null)
                return NotFound($"Holiday with ID {id} not found.");

            return Ok(holiday.DTO);
        }

        // 3. POST (CREATE) : api/holidays
        [HttpPost]
        public ActionResult<PublicHolidaysDTO> CreateHoliday(PublicHolidaysDTO holidayDTO)
        {
            if (holidayDTO == null || string.IsNullOrEmpty(holidayDTO.HolidayName))
                return BadRequest("Invalid holiday data.");

            clsPublicHoliday holiday = new clsPublicHoliday();
            holiday.DTO.HolidayName = holidayDTO.HolidayName;
            holiday.DTO.HolidayDate = holidayDTO.HolidayDate;
            holiday.DTO.IsRepeatedAnnually = holidayDTO.IsRepeatedAnnually;

            if (holiday.Save())
            {
                // Retourne 201 Created avec l'URL de la nouvelle ressource
                return CreatedAtRoute("GetHolidayById", new { id = holiday.DTO.HolidayID }, holiday.DTO);
            }

            return StatusCode(500, "An error occurred while saving the holiday.");
        }

        // 4. PUT (UPDATE) : api/holidays/{id}
        [HttpPut("{id}")]
        public ActionResult<PublicHolidaysDTO> UpdateHoliday(int id, PublicHolidaysDTO holidayDTO)
        {
            if (id <= 0 || holidayDTO == null) return BadRequest("Invalid parameters.");

            clsPublicHoliday holiday = clsPublicHoliday.Find(id);

            if (holiday == null)
                return NotFound($"Holiday with ID {id} not found.");

            // Mise à jour des données
            holiday.DTO.HolidayName = holidayDTO.HolidayName;
            holiday.DTO.HolidayDate = holidayDTO.HolidayDate;
            holiday.DTO.IsRepeatedAnnually = holidayDTO.IsRepeatedAnnually;

            if (holiday.Save())
                return Ok(holiday.DTO);

            return StatusCode(500, "An error occurred while updating the holiday.");
        }

        // 5. DELETE : api/holidays/{id}
        [HttpDelete("{id}")]
        public ActionResult DeleteHoliday(int id)
        {
            if (id <= 0) return BadRequest("Invalid ID.");

            if (!clsPublicHoliday.Delete(id))
                return NotFound($"Holiday with ID {id} not found or could not be deleted.");

            return Ok(new { message = $"Holiday {id} deleted successfully." });
        }

        // 6. CHECK DATE : api/holidays/is-holiday/{date}
        [HttpGet("is-holiday/{date}")]
        public ActionResult<bool> CheckIfHoliday(DateTime date)
        {
            bool isHoliday = clsPublicHoliday.IsPublicHoliday(date);
            return Ok(isHoliday);
        }
    }
}