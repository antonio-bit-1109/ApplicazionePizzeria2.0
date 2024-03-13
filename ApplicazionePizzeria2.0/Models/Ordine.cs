using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApplicazionePizzeria2._0.Models
{
	public class Ordine
	{
		[Key]
		public int IdOrdine { get; set; }

		[Required]
		public string indirizzoSpedizione { get; set; }

		[Required]
		[ForeignKey("Utente")]
		public int IdUtente { get; set; }

		public string? NoteAggiuntive { get; set; } = string.Empty;

		public virtual Utente Utente { get; set; }
		public virtual ICollection<DettagliOrdine> DettagliOrdini { get; set; }
	}
}
