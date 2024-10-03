using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Eksamen2024.DAL;
using Eksamen2024.Models;
using Microsoft.Extensions.Logging;

namespace Eksamen2024.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PinpointController : ControllerBase
    {
        // Dependency injection of the repository
        private readonly IPinpointRepository _pinpointRepository;
        private readonly ILogger<PinpointController> _logger;

        // Constructor for dependency injection
        public PinpointController(IPinpointRepository pinpointRepository, ILogger<PinpointController> logger)
        {
            _pinpointRepository = pinpointRepository;
            _logger = logger;
        }

        // GET method for returning pinpoints asynchronously
        [HttpGet]
        public async Task<IActionResult> GetPinpoints()
        {
            try
            {
                // Fetch pinpoints from the repository
                var pinpoints = await _pinpointRepository.GetAll();
                return Ok(pinpoints);
            }
            catch (Exception ex)
            {
                // Handle and log error
                _logger.LogError($"Error fetching pinpoints: {ex}");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET method for a specific pinpoint by ID
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

                return Ok(pinpoint);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching pinpoint: {ex}");
                return StatusCode(500, "Internal server error");
            }
        }

        // POST method for creating a new pinpoint
        [HttpPost]
        public async Task<IActionResult> CreatePinpoint([FromBody] Pinpoint pinpoint)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _pinpointRepository.Create(pinpoint);
                return CreatedAtAction(nameof(GetPinpoint), new { id = pinpoint.PinpointId }, pinpoint);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating pinpoint: {ex}");
                return StatusCode(500, "Internal server error");
            }
        }

        // PUT method for updating an existing pinpoint
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

        // DELETE method for deleting a pinpoint
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
