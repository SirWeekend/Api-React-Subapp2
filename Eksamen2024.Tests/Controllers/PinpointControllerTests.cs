using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Eksamen2024.Controllers;
using Eksamen2024.DAL;
using Eksamen2024.Models;
using Microsoft.VisualBasic;

namespace Eksamen2024.Controllers;

public class PinpointControllerTests
{
    [Fact]
public async Task GetPinpoints_ReturnsViewWithPinpoints()
{
     // Arrange: Mock the dependencies
            var mockRepo = new Mock<IPinpointRepository>();
            var mockLogger = new Mock<ILogger<PinpointController>>();
            
            // Create an instance of the controller with the mocked dependencies
            var controller = new PinpointController(mockRepo.Object, mockLogger.Object);
            
            // Set up the repository mock to return a list of pinpoints
            var pinpoints = new List<Pinpoint> { new Pinpoint { PinpointId = 1, Name = "Test" } };
            mockRepo.Setup(repo => repo.GetAll()).ReturnsAsync(pinpoints);
            
            // Act: Call the GetPinpoints method
            var result = await controller.GetPinpoints();

            // Assert: Ensure the result is of type OkObjectResult and contains the expected data
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<Pinpoint>>(okResult.Value);
            Assert.NotEmpty(returnValue);  // Ensure it returns some pinpoints
}

}