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
    /// <summary>
    /// Handles all operations related to Pinpoints.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class PinpointController : ControllerBase
    {
        private readonly IPinpointRepository _pinpointRepository;
        private readonly ILogger<PinpointController> _logger;

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        /// <param name="pinpointRepository">Repository for interacting with pinpoints.</param>
        /// <param name="logger">Logger for capturing log messages.</param>
        public PinpointController(IPinpointRepository pinpointRepository, ILogger<PinpointController> logger)
        {
            _pinpointRepository = pinpointRepository;
            _logger = logger;
        }

        /// <summary>
        /// Gets all pinpoints.
        /// </summary>
        /// <returns>A list of pinpoints with details such as name, description, and location.</returns>
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
                _logger.LogError($"Error fetching pinpoints: {ex}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Gets a specific pinpoint by ID.
        /// </summary>
        /// <param name="id">The ID of the pinpoint to retrieve.</param>
        /// <returns>The pinpoint with the specified ID, or 404 if not found.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPinpoint(int id)
        {
            try
            {
                var pinpoint = await _pinpointRepository.GetPinpointById(id);

                if (pinpoint == null)
                {
                    return NotFound();
                }

                var pinpointObject = new
                {
                    pinpoint.PinpointId,
                    pinpoint.Name,
                    pinpoint.ImageUrl,
                    pinpoint.Description,
                    pinpoint.Latitude,
                    pinpoint.Longitude,
                    Username = pinpoint.User?.Username ?? "Anonymous"
                };

                return Ok(pinpointObject);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching pinpoint: {ex}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Creates a new pinpoint.
        /// </summary>
        /// <param name="pinpoint">The pinpoint object to create.</param>
        /// <returns>The created pinpoint with a 201 status code.</returns>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreatePinpoint([FromBody] Pinpoint pinpoint)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    _logger.LogError("User is not authenticated.");
                    return Unauthorized("User is not authenticated.");
                }

                var userId = int.Parse(userIdClaim);
                pinpoint.UserId = userId;
                await _pinpointRepository.Create(pinpoint, userId);
                return CreatedAtAction(nameof(GetPinpoint), new { id = pinpoint.PinpointId }, pinpoint);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating pinpoint: {ex}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Updates an existing pinpoint.
        /// </summary>
        /// <param name="id">The ID of the pinpoint to update.</param>
        /// <param name="pinpoint">The updated pinpoint object.</param>
        /// <returns>204 No Content on success, or 400/500 on failure.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePinpoint(int id, [FromBody] Pinpoint pinpoint)
        {
            if (id != pinpoint.PinpointId)
            {
                return BadRequest("ID mismatch");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _pinpointRepository.Update(pinpoint);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating pinpoint: {ex}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Deletes a pinpoint.
        /// </summary>
        /// <param name="id">The ID of the pinpoint to delete.</param>
        /// <returns>204 No Content on success, or 404/500 on failure.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePinpoint(int id)
        {
            try
            {
                var success = await _pinpointRepository.Delete(id);

                if (!success)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting pinpoint: {ex}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
