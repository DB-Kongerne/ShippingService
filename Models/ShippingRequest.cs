namespace ShippingService.Models
{
    public class ShippingRequest
    {
        public string MedlemsNavn { get; set; }
        public string AfhentningsAdresse { get; set; }
        public string PakkeId { get; set; }
        public string LeveringsAdresse { get; set; }
    }
}
