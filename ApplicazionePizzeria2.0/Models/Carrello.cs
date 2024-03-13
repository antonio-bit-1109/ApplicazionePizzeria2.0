using System.ComponentModel.DataAnnotations.Schema;

namespace ApplicazionePizzeria2._0.Models
{
    public class Carrello
    {
        [NotMapped]
        public Prodotto Prodotto { get; set; }

        [NotMapped]
        public int Quantita { get; set; }
    }
}
