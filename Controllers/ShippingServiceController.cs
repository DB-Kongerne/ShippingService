using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ShippingService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ShippingServiceController : ControllerBase
    {
        private readonly ILogger<ShippingServiceController> _logger;

        public ShippingServiceController(ILogger<ShippingServiceController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public IActionResult CreateShippingRequest([FromBody] ShippingRequest shippingRequest)
        {
            _logger.LogInformation("Received a new shipping request: {ShippingRequest}", JsonSerializer.Serialize(shippingRequest));

            // Her kunne du implementere yderligere logik til at håndtere shippingRequest.

            return Ok("Shipping request processed successfully.");
        }
    }
}
