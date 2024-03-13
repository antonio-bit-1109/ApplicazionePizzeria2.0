using System.ComponentModel.DataAnnotations.Schema;

namespace ApplicazionePizzeria2._0.Models
{
	public class RiepilogoOrdine
	{
		[NotMapped]
		public List<DettagliOrdine> ListaDettagliOrdine { get; set; }

		[NotMapped]
		public Ordine Ordine { get; set; }
	}
}
