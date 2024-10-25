using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ShippingService.Controllers;

[ApiController]
[Route("[controller]")]
public class ShippingServiceController : ControllerBase
{
    private readonly ILogger<ShippingServiceController> _logger;

    public ShippingServiceController(ILogger<ShippingServiceController> logger)
    {
        _logger = logger;
    }

    // HTTP POST endpoint for at modtage en ShippingRequest
    [HttpPost("opret-leveringsanmodning")]
    public IActionResult CreateShippingRequest([FromBody] ShippingRequest request)
    {
        if (request == null || string.IsNullOrEmpty(request.AfsenderAdresse) ||
            string.IsNullOrEmpty(request.ModtagerAdresse) || request.PakkeVægt <= 0)
        {
            _logger.LogWarning("Ugyldig leveringsanmodning modtaget");
            return BadRequest("Ugyldig leveringsanmodning");
        }

        _logger.LogInformation("Ny leveringsanmodning modtaget");
        // Here should go the logic to process and save the shipping request
        
        return Ok("Leveringsanmodning oprettet");
    }

    // HTTP GET endpoint for at udlevere en Delivery Plan
    [HttpGet("leveringsplan")]
    public IActionResult GetDeliveryPlan()
    {
        // Her burde du implementere logik til at hente og returnere leveringsplanen
        var deliveryPlan = new { Message = "Leveringsplan vil blive tilføjet senere." };
        
        _logger.LogInformation("Leveringsplan forespørgsel modtaget");
        return Ok(deliveryPlan);
    }
}
