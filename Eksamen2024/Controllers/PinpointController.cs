using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Eksamen2024.DAL;
using Eksamen2024.Models;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Eksamen2024.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PinpointController : ControllerBase
    {
        private readonly IPinpointRepository _pinpointRepository;
        private readonly ILogger<PinpointController> _logger;
        private readonly ApplicationDbContext _dbContext;

        public PinpointController(IPinpointRepository pinpointRepository, ILogger<PinpointController> logger, ApplicationDbContext dbContext)
        {
            _pinpointRepository = pinpointRepository;
            _logger = logger;
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetPinpoints()
        {
            try
            {
                var pinpoints = await _pinpointRepository.GetAll();
                var pinpointObjectAll = pinpoints.Select(pinpoint => new
                {
                    pinpoint.PinpointId,
                    pinpoint.Name,
                    pinpoint.Description,
                    pinpoint.Latitude,
                    pinpoint.Longitude,
                    pinpoint.ImageUrl,
                    Username = pinpoint.User?.Username ?? "Anonymous"
                });
                return Ok(pinpointObjectAll);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching pinpoints: {ex.Message}");
                return StatusCode(500, "An error occurred while fetching pinpoints.");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPinpoint(int id)
        {
            try
            {
                var pinpoint = await _pinpointRepository.GetPinpointById(id);

                if (pinpoint == null)
                {
                    return NotFound(new { Message = "Pinpoint not found." });
                }

                var pinpointObject = new
                {
                    pinpoint.PinpointId,
                    pinpoint.Name,
                    pinpoint.Description,
                    pinpoint.Latitude,
                    pinpoint.Longitude,
                    pinpoint.ImageUrl,
                    Username = pinpoint.User?.Username ?? "Anonymous"
                };

                return Ok(pinpointObject);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching pinpoint: {ex.Message}");
                return StatusCode(500, "An error occurred while fetching the pinpoint.");
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
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    return Unauthorized(new { Message = "User is not authenticated." });
                }

                var userId = int.Parse(userIdClaim);
                pinpoint.UserId = userId;
                await _pinpointRepository.Create(pinpoint, userId);

                return CreatedAtAction(nameof(GetPinpoint), new { id = pinpoint.PinpointId }, pinpoint);
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
                return BadRequest(new { Message = "ID mismatch." });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new { Message = "Invalid data." });
            }

            try
            {
                var existingPinpoint = await _pinpointRepository.GetPinpointById(id);
                if (existingPinpoint == null)
                {
                    return NotFound(new { Message = "Pinpoint not found." });
                }

                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    return Unauthorized(new { Message = "User is not authenticated." });
                }

                var loggedInUserId = int.Parse(userIdClaim);

                if (existingPinpoint.UserId != loggedInUserId)
                {
                    return Forbid();
                }

                existingPinpoint.Name = updatedPinpoint.Name;
                existingPinpoint.Description = updatedPinpoint.Description;
                existingPinpoint.ImageUrl = updatedPinpoint.ImageUrl;

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
                var pinpoint = await _pinpointRepository.GetPinpointById(id);
                if (pinpoint == null)
                {
                    return NotFound(new { Message = "Pinpoint not found." });
                }

                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    return Unauthorized(new { Message = "User is not authenticated." });
                }

                var loggedInUserId = int.Parse(userIdClaim);

                if (pinpoint.UserId != loggedInUserId)
                {
                    return Forbid();
                }

                var success = await _pinpointRepository.Delete(id);
                if (!success)
                {
                    return StatusCode(500, "Failed to delete the pinpoint.");
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
