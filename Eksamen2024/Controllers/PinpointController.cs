using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Eksamen2024.DAL;
using Eksamen2024.Models;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

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
                _logger.LogInformation("Pinpoints successfully fetched."); // Logging for pinpoint-henting
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
                _logger.LogWarning("Invalid ModelState detected during pinpoint creation.");
                return BadRequest(new { Message = "Invalid data." });
            }

            try
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    _logger.LogWarning("User is not authenticated.");
                    return Unauthorized(new { Message = "User is not authenticated." });
                }

                pinpoint.UserId = int.Parse(userIdClaim);
                await _pinpointRepository.Create(pinpoint);

                _logger.LogInformation($"Pinpoint created: {pinpoint.Name} by User {userIdClaim}"); // Logging for oppretting
                return CreatedAtAction(nameof(GetPinpoints), new { id = pinpoint.PinpointId }, pinpoint);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating pinpoint: {ex.Message}");
                return StatusCode(500, "An error occurred while creating the pinpoint.");
            }
        }
    }
}
