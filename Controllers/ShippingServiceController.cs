using Microsoft.AspNetCore.Mvc;

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

   
}
