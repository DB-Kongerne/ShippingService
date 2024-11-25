using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using ShippingService.Models;
using RabbitMQ.Client;




namespace ShippingService.Controllers
{
    [ApiController]
    [Route("api/forsendelse")]
    public class ShippingController : ControllerBase
    {
        private readonly string _Deliverys;
        private readonly ILogger<ShippingController> _logger;
        public ShippingController(ILogger<ShippingController> logger, IConfiguration configuration)
    {
        _logger = logger;
        _Deliverys = configuration["Deliverys"] ?? string.Empty; // Hent miljøvariabel
    }

        // HTTP POST til oprettelse af forsendelse
        [HttpPost("anmodning")]
        public async Task<IActionResult> OpretForsendelse([FromBody] ShippingRequest anmodning)
        {
            if (anmodning == null)
            {
                return BadRequest("ShippingRequest mangler.");
            }

            // Konverter ShippingRequest til ShipmentDelivery
            var shipment = new ShipmentDelivery
            {
                MedlemsNavn = anmodning.MedlemsNavn,
                AfhentningsAdresse = anmodning.AfhentningsAdresse,
                PakkeId = anmodning.PakkeId,
                LeveringsAdresse = anmodning.LeveringsAdresse,
        
            };

            // Publicer ShipmentDelivery til RabbitMQ
            PublishToRabbitMQ(shipment);
            
            return Ok("Forsendelse oprettet og publiceret til kø.");
        }

        // Metode til at publicere en ShipmentDelivery til RabbitMQ
        private void PublishToRabbitMQ(ShipmentDelivery shipment)
        {
            var factory = new ConnectionFactory() { HostName = "localhost:15672" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                // Deklarer køen, hvis den ikke allerede eksisterer
                channel.QueueDeclare(queue: "forsendelsesKø",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                // Serialiser ShipmentDelivery til JSON
                var message = JsonSerializer.Serialize(shipment);
                var body = Encoding.UTF8.GetBytes(message);

                // Send beskeden til køen
                channel.BasicPublish(exchange: "",
                                     routingKey: "forsendelsesKø",
                                     basicProperties: null,
                                     body: body);
            }
        }

        /*// HTTP GET til udlevering af Delivery Plan
        [HttpGet("leveringsplan")]
        public async Task<IActionResult> HentLeveringsplan()
        {
            var filnavn = "DeliveryPlan-20240924.csv";

            if (!System.IO.File.Exists(filnavn))
            {
                return NotFound("Leveringsplanen findes ikke.");
            }

            try
            {
                var leveringsPlanData = await System.IO.File.ReadAllTextAsync(filnavn);
                return Ok(leveringsPlanData);
            }
            catch (IOException ex)
            {
                return StatusCode(500, $"Fejl under læsning af leveringsplan: {ex.Message}");
            }
        }*/
    }
}
