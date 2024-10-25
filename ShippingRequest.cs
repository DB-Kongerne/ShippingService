public class ShippingRequest
{
    public string AfsenderAdresse { get; set; }
    public string ModtagerAdresse { get; set; }
    public double PakkeVægt { get; set; }
    public DateTime? Leveringsdato { get; set; } // Nullable
    public double? Laengde { get; set; }
    public double? Bredde { get; set; }
    public double? Hoejde { get; set; }
    public string Prioritet { get; set; } // e.g. "Standard", "Express"
    public string Noter { get; set; }

    public ShippingRequest(string afsenderAdresse, string modtagerAdresse, double pakkeVægt,
                           DateTime? leveringsdato = null, double? laengde = null, 
                           double? bredde = null, double? hoejde = null, 
                           string prioritet = "Standard", string noter = "")
    {
        AfsenderAdresse = afsenderAdresse;
        ModtagerAdresse = modtagerAdresse;
        PakkeVægt = pakkeVægt;
        Leveringsdato = leveringsdato;
        Laengde = laengde;
        Bredde = bredde;
        Hoejde = hoejde;
        Prioritet = prioritet;
        Noter = noter; // Rettet fra 'Not ret' til 'Noter'
    }
}
