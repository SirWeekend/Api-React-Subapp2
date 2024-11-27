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
        // Dependency injection of the repository
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

        // GET method for returning pinpoints asynchronously
        [HttpGet]
        public async Task<IActionResult> GetPinpoints()
        {
            try
            {
                // Fetch pinpoints from the repository
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
                //Creating a response object
                var pinpointObject = new
                {
                    pinpoint.PinpointId,
                    pinpoint.Name,
                    pinpoint.ImageUrl,
                    pinpoint.Description,
                    pinpoint.Latitude,
                    pinpoint.Longitude,
                    Username = pinpoint.User?.Username ?? "Anonymous" // Using anonymous as default if its no user to handle null values
                };

                return Ok(pinpointObject);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching pinpoint: {ex}");
                return StatusCode(500, "Internal server error");
            }
        }

        // POST method for creating a new pinpoint
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
            
            // Log claims debugging
            _logger.LogInformation("User Claims:");
            foreach (var claim in User.Claims)
            {
            _logger.LogInformation("Claim Type: {Type}, Claim Value: {Value}", claim.Type, claim.Value);
             }

            // Retrieve the logged-in user's ID from claims
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
            {
            _logger.LogError("User is not authenticated or NameIdentifier claim is missing.");
            return Unauthorized("User is not authenticated.");
            }

            var userId = int.Parse(userIdClaim);

            // Set the UserId on the Pinpoint
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