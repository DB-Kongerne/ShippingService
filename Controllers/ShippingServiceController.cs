using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using System.Text;
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
            // Send shippingRequest to message broker
            var factory = new ConnectionFactory() { HostName = "localhost" }; // RabbitMQ
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "shipping_requests",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var json = JsonSerializer.Serialize(shippingRequest);
                var body = Encoding.UTF8.GetBytes(json);

                channel.BasicPublish(exchange: "",
                                     routingKey: "shipping_requests",
                                     basicProperties: null,
                                     body: body);
            }

            return Ok("Shipping request sent.");
        }
    }
}
