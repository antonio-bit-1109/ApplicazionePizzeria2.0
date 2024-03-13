using System.ComponentModel.DataAnnotations;

namespace ApplicazionePizzeria2._0.Models
{
	public class Utente
	{
		[Key]
		public int IdUtente { get; set; }

		[Required]
		public string Nome { get; set; }

		[Required]
		public string Password { get; set; }

		[Required]
		public string Ruolo { get; set; }

		public virtual ICollection<Ordine> Ordini { get; set; }
	}
}
