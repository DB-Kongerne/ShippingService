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
                Status = "Under behandling"
            };

            // Send ShipmentDelivery til RabbitMQ
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "forsendelsesKø",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var message = JsonSerializer.Serialize(shipment);
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                                     routingKey: "forsendelsesKø",
                                     basicProperties: null,
                                     body: body);
            }
            
            return Ok("Forsendelse oprettet.");
        }

        // HTTP GET til udlevering af Delivery Plan
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
        }
    }
}
