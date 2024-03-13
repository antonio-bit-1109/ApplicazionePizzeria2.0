using Microsoft.EntityFrameworkCore;

namespace ApplicazionePizzeria2._0.data
{
	public class ApplicationDbContext : DbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
		{
		}


		// inserisci le tabelle del database
		public DbSet<Models.Utente> Utenti { get; set; }
		public DbSet<Models.Prodotto> Prodotti { get; set; }
		public DbSet<Models.Ordine> Ordini { get; set; }
		public DbSet<Models.DettagliOrdine> DettagliOrdini { get; set; }
	}
}
