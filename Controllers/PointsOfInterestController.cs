using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace ITPE3200.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PointsOfInterestController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetPoints()
        {
            // Returner noen eksempeldata
            var points = new List<string> { "Point1", "Point2", "Point3" };
            return Ok(points);
        }
    }
}
