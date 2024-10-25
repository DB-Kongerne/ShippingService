public class ShippingRequest
{
    public string AfsenderAdresse { get; set; }
    public string ModtagerAdresse { get; set; }
    public double PakkeVægt { get; set; }

    public ShippingRequest(string afsenderAdresse, string modtagerAdresse, double pakkeVægt)
    {
        AfsenderAdresse = afsenderAdresse;
        ModtagerAdresse = modtagerAdresse;
        PakkeVægt = pakkeVægt;
    }
}
