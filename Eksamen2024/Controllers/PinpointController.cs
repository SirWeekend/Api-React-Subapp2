using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Eksamen2024.DAL;
using Eksamen2024.Models;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

namespace Eksamen2024.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PinpointController : ControllerBase
    {
        private readonly IPinpointRepository _pinpointRepository;
        private readonly ILogger<PinpointController> _logger;

        public PinpointController(IPinpointRepository pinpointRepository, ILogger<PinpointController> logger)
        {
            _pinpointRepository = pinpointRepository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetPinpoints()
        {
            try
            {
                var pinpoints = await _pinpointRepository.GetAll();
                if (pinpoints == null || !pinpoints.Any())
                {
                    _logger.LogInformation("No pinpoints found.");
                    return Ok(new List<Pinpoint>()); // Return an empty list
                }

                _logger.LogInformation($"Fetched {pinpoints.Count()} pinpoints.");
                return Ok(pinpoints);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching pinpoints: {ex.Message}");
                return StatusCode(500, "An error occurred while fetching pinpoints.");
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreatePinpoint([FromBody] Pinpoint pinpoint)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Message = "Invalid data." });
            }

            try
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                {
                    return Unauthorized(new { Message = "User is not authenticated." });
                }

                pinpoint.UserId = userId;

                if (pinpoint.PinpointId > 0)
                {
                    pinpoint.PinpointId = 0; // EF Core should generate a new ID
                }

                await _pinpointRepository.Create(pinpoint, userId);
                return CreatedAtAction(nameof(GetPinpoints), new { id = pinpoint.PinpointId }, pinpoint);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating pinpoint: {ex.Message}");
                return StatusCode(500, "An error occurred while creating the pinpoint.");
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdatePinpoint(int id, [FromBody] Pinpoint updatedPinpoint)
        {
            if (id != updatedPinpoint.PinpointId)
            {
                return BadRequest(new { Message = "Mismatched ID." });
            }

            try
            {
                var existingPinpoint = await _pinpointRepository.GetPinpointById(id);
                if (existingPinpoint == null)
                {
                    return NotFound(new { Message = "Pinpoint not found." });
                }

                existingPinpoint.Name = updatedPinpoint.Name;
                existingPinpoint.Description = updatedPinpoint.Description;
                existingPinpoint.Latitude = updatedPinpoint.Latitude;
                existingPinpoint.Longitude = updatedPinpoint.Longitude;

                await _pinpointRepository.Update(existingPinpoint);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating pinpoint: {ex.Message}");
                return StatusCode(500, "An error occurred while updating the pinpoint.");
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeletePinpoint(int id)
        {
            try
            {
                var deleted = await _pinpointRepository.Delete(id);
                if (!deleted)
                {
                    return NotFound(new { Message = "Pinpoint not found." });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting pinpoint: {ex.Message}");
                return StatusCode(500, "An error occurred while deleting the pinpoint.");
            }
        }
    }
}
