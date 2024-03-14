using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApplicazionePizzeria2._0.Models
{
	public class Ordine
	{
		public Ordine()
		{
			DataDellaConsegna = DateTime.Now;
		}


		[Key]
		public int IdOrdine { get; set; }

		[Required]
		public string indirizzoSpedizione { get; set; }

		[Required]
		[ForeignKey("Utente")]
		public int IdUtente { get; set; }

		public string? NoteAggiuntive { get; set; } = string.Empty;


		public DateTime DataDellaConsegna { get; set; }

		public virtual Utente Utente { get; set; }
		public virtual ICollection<DettagliOrdine> DettagliOrdini { get; set; }
	}
}
