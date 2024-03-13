using System.ComponentModel.DataAnnotations;

namespace ApplicazionePizzeria2._0.Models
{
	public class Prodotto
	{
		[Key]
		public int IdProdotto { get; set; }

		[Required]
		public string NomeProdotto { get; set; }

		[Required]
		public string FotoProdotto { get; set; }

		[Required]
		public double PrezzoProdotto { get; set; }

		[Required]
		public int TempoConsegna { get; set; }

		[Required]
		public string Ingredienti { get; set; }

		// uno stesso prodotto può essere presente in più Dettagliordini
		public virtual ICollection<DettagliOrdine> DettagliOrdini { get; set; }
	}
}
