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
    /// <summary>
    /// Handles all operations related to Pinpoints.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class PinpointController : ControllerBase
    {
        private readonly IPinpointRepository _pinpointRepository;
        private readonly ILogger<PinpointController> _logger;
        private readonly ApplicationDbContext _dbContext; // Added ApplicationDbContext field

        // Constructor for dependency injection
        public PinpointController(IPinpointRepository pinpointRepository, ILogger<PinpointController> logger, ApplicationDbContext dbContext)
        {
            _pinpointRepository = pinpointRepository;
            _logger = logger;
            _dbContext = dbContext;
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

        // Method for adding comments to a pinpoint
        [HttpPost("{pinpointId}/comments")]
        [Authorize]
        public async Task<IActionResult> AddComment(int pinpointId, [FromBody] string commentText)
        {
            if (string.IsNullOrWhiteSpace(commentText))
            {
                return BadRequest("Comment text cannot be empty.");
            }

            try
            {
                // Get the logged-in user's ID from claims
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    _logger.LogError("User is not authenticated or NameIdentifier claim is missing.");
                    return Unauthorized("User is not authenticated.");
                }

                var userId = int.Parse(userIdClaim);

                // Verify that the pinpoint exists
                var pinpoint = await _pinpointRepository.GetPinpointById(pinpointId);
                if (pinpoint == null)
                {
                    return NotFound("Pinpoint not found.");
                }

                // Create the comment
                var comment = new Comment
                {
                    Text = commentText,
                    PinpointId = pinpointId,
                    UserId = userId
                };

                _dbContext.Comments.Add(comment);
                await _dbContext.SaveChangesAsync();

                return Ok(new
                {
                    comment.CommentId,
                    comment.Text,
                    Username = pinpoint.User?.Username ?? "Anonymous"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error adding comment: {ex}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{pinpointId}/comments")]
public async Task<IActionResult> GetComments(int pinpointId)
{
    try
    {
        _logger.LogInformation($"Fetching comments for PinpointId: {pinpointId}");

        // Fetch comments for the given pinpoint
        var comments = await _dbContext.Comments
            .Where(c => c.PinpointId == pinpointId)
            .Include(c => c.User)
            .ToListAsync();

        if (comments == null || !comments.Any())
        {
            _logger.LogWarning($"No comments found for PinpointId: {pinpointId}");
            return Ok(new List<object>());
        }

        // Transform comments into the desired format
        var result = comments.Select(comment => new
        {
            comment.CommentId,
            comment.Text,
            Username = comment.User?.Username ?? "Anonymous"
        }).ToList();

        return Ok(result); // Explicitly return a plain array
    }
    catch (Exception ex)
    {
        _logger.LogError($"Error fetching comments for PinpointId {pinpointId}: {ex.Message}");
        return StatusCode(500, new { error = "Internal server error", details = ex.Message });
    }
}



        /// <summary>
        /// Updates an existing pinpoint.
        /// </summary>
        /// <param name="id">The ID of the pinpoint to update.</param>
        /// <param name="pinpoint">The updated pinpoint object.</param>
        /// <returns>204 No Content on success, or 400/500 on failure.</returns>
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdatePinpoint(int id, [FromBody] Pinpoint updatedPinpoint)
        {
            if (id != updatedPinpoint.PinpointId)
            {
                return BadRequest("ID mismatch");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var existingPinpoint = await _pinpointRepository.GetPinpointById(id);
                if (existingPinpoint == null)
                {
                    return NotFound("Pinpoint not found.");
                }

                 // Get the logged-in user's ID
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    _logger.LogError("User is not authenticated or NameIdentifier claim is missing.");
                    return Unauthorized("User is not authenticated.");
                }

                var loggedInUserId = int.Parse(userIdClaim);

                // Check if the logged-in user is the owner of the pinpoint
                if (existingPinpoint.UserId != loggedInUserId)
                {
                    return Forbid("You are not authorized to update this pinpoint.");
                }

                // Update properties
                existingPinpoint.Name = updatedPinpoint.Name;
                existingPinpoint.Description = updatedPinpoint.Description;
                existingPinpoint.ImageUrl = updatedPinpoint.ImageUrl;

                await _pinpointRepository.Update(existingPinpoint);
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
        [Authorize]
        public async Task<IActionResult> DeletePinpoint(int id)
        {
    try
    {
        // Fetch the pinpoint from the database
        var pinpoint = await _pinpointRepository.GetPinpointById(id);
        if (pinpoint == null)
        {
            return NotFound("Pinpoint not found.");
        }

        // Get the logged-in user's ID
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdClaim))
        {
            _logger.LogError("User is not authenticated or NameIdentifier claim is missing.");
            return Unauthorized("User is not authenticated.");
        }

        var loggedInUserId = int.Parse(userIdClaim);

        // Check if the logged-in user is the owner of the pinpoint
        if (pinpoint.UserId != loggedInUserId)
        {
            return Forbid("You are not authorized to delete this pinpoint.");
        }

        // Perform the deletion
        var success = await _pinpointRepository.Delete(id);
        if (!success)
        {
            return StatusCode(500, "Failed to delete pinpoint.");
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
