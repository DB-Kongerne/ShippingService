using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using ShippingService.Models;
using RabbitMQ.Client;
using System.Net;
using System.Linq; // Husk at inkludere Linq for First()

namespace ShippingService.Controllers
{
    [ApiController]
    [Route("api/forsendelse")]
    public class ShippingController : ControllerBase
    {
        private readonly string _Deliverys;
        private readonly ILogger<ShippingController> _logger;
        private readonly string _rabbitHost;
        
        public ShippingController(ILogger<ShippingController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _Deliverys = configuration["Deliverys"] ?? string.Empty; // Hent miljøvariabel
            _rabbitHost = configuration["RabbitHost"] ?? "localhost"; // Hent RabbitHost fra appsettings.json 

            // Hent og log IP-adresse ved opstart
            var hostName = System.Net.Dns.GetHostName();
            var ips = System.Net.Dns.GetHostAddresses(hostName);
            var _ipaddr = ips.First().MapToIPv4().ToString();
            _logger.LogInformation($"XYZ Service responding from {_ipaddr}");
        }

        // HTTP POST til oprettelse af forsendelse
        [HttpPost("anmodning")]
        public async Task<IActionResult> OpretForsendelse([FromBody] ShippingRequest anmodning)
        {
            if (anmodning == null)
            {
                _logger.LogWarning("ShippingRequest mangler i anmodning.");
                return BadRequest("ShippingRequest mangler.");
            }

            // Log detaljer om den modtagne anmodning
            _logger.LogInformation("Opretter forsendelse for MedlemsNavn: {MedlemsNavn}, PakkeId: {PakkeId}", anmodning.MedlemsNavn, anmodning.PakkeId);

            // Konverter ShippingRequest til ShipmentDelivery
            var shipment = new ShipmentDelivery
            {
                MedlemsNavn = anmodning.MedlemsNavn,
                AfhentningsAdresse = anmodning.AfhentningsAdresse,
                PakkeId = anmodning.PakkeId,
                LeveringsAdresse = anmodning.LeveringsAdresse,
            };

            // Publicer ShipmentDelivery til RabbitMQ
            try
            {
                PublishToRabbitMQ(shipment);
                _logger.LogInformation("Forsendelse med PakkeId: {PakkeId} publiceret til RabbitMQ.", shipment.PakkeId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fejl ved publicering af ShipmentDelivery til RabbitMQ.");
                return StatusCode(500, "Fejl ved publicering af forsendelse.");
            }

            return Ok("Forsendelse oprettet og publiceret til kø.");
        }

        // Metode til at publicere en ShipmentDelivery til RabbitMQ
        private void PublishToRabbitMQ(ShipmentDelivery shipment)
        {
            try
            {
                var factory = new ConnectionFactory() { HostName = _rabbitHost };
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

                    _logger.LogInformation("Besked sendt til RabbitMQ kø: {Queue}", "forsendelsesKø");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fejl ved oprettelse af RabbitMQ-forbindelse eller publicering af besked.");
                throw;
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
        } */
    }
}
