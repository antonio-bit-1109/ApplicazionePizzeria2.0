using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApplicazionePizzeria2._0.Models
{
	public class DettagliOrdine
	{
		[Key]
		public int IdDettagliOrdine { get; set; }

		[Required]
		[ForeignKey("Ordine")]
		public int IdOrdine { get; set; }

		[Required]
		[ForeignKey("Prodotto")]
		public int IdProdotto { get; set; }

		[Required]
		public int Quantita { get; set; }

		[Required]
		public double Prezzo { get; set; }

		public double? CostoTotale { get; set; }

		[Required]
		public bool OrdineEvaso { get; set; } = false;

		public virtual Prodotto Prodotto { get; set; }

		public virtual Ordine Ordine { get; set; }
	}
}
