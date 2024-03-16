using ApplicazionePizzeria2._0.data;
using ApplicazionePizzeria2._0.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ApplicazionePizzeria2._0.Controllers
{
	public class LoginController : Controller
	{
		private readonly ApplicationDbContext _db;
		private readonly IAuthenticationSchemeProvider _schemeProvider;
		public LoginController(ApplicationDbContext db, IAuthenticationSchemeProvider schemeProvider)
		{
			_db = db;
			_schemeProvider = schemeProvider;
		}

		public IActionResult Index()
		{
			return View();
		}


		//questo metodo serve per vedere se il nome inserito dall'utente è gia presente nel DB, se lo user è nullo l utente non esiste.
		//Se invece esiste nel DB vengono presi il nome l'id ed il ruolo salvati nel db e inseriti come claim nel cookie di autenticazione 
		[HttpPost]
		public async Task<IActionResult> Login(Utente utente)
		{
			if (utente.Nome != null && utente.Password != null)
			{
				//string sqlQuery = "SELECT * FROM Dipendenti" +
				//    " WHERE NomeUtente = @nome AND Password = @password";

				string sqlQuery = "SELECT * FROM Utenti" +
					" WHERE Nome = @nome AND Password = @password";

				var nomeParam = new SqlParameter("@nome", utente.Nome);
				var passwordParam = new SqlParameter("@password", utente.Password);

				var user = await _db.Utenti.FromSqlRaw(sqlQuery, nomeParam, passwordParam).FirstOrDefaultAsync();

				if (user != null)
				{
					if (user.Nome == utente.Nome && user.Password == utente.Password)
					{
						var claims = new List<Claim>
						{
							new Claim(ClaimTypes.Name, user.Nome),
							new Claim(ClaimTypes.Role, user.Ruolo),
							new Claim(ClaimTypes.NameIdentifier, user.IdUtente.ToString())
						};

						var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

						var authProperties = new AuthenticationProperties();

						await HttpContext.SignInAsync(
					   CookieAuthenticationDefaults.AuthenticationScheme,
					   new ClaimsPrincipal(claimsIdentity),
					   authProperties);

						TempData["Message"] = "Login effettuato con successo";
						return RedirectToAction("Index");
					}
				}
				TempData["Errore"] = "Lo User è nullo.";
				return RedirectToAction("Index", "Home");

			}
			TempData["Errore"] = "Username o password inseriti male.";
			return RedirectToAction("Index", "Home");
		}

		// questo metodo serve per fare il logout, cancella il cookie di autenticazione e reindirizza alla home
		public async Task<IActionResult> Logout()
		{
			await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

			TempData["Message"] = "Logout effettuato.";
			return RedirectToAction("Index", "Home");
		}

		// questo metodo viene attivato quando l'utente clicca sul link "Registrati" nella pagina di login
		// sono presenti alcuni tempdata che vengono salvati e al reindirizzamento in home
		// vengono visualizzato come messaggi utili per l'utente a capire dove inserire i dati per registrarsi
		public IActionResult Registrazione()
		{
			bool MiStoRegistrando;


			MiStoRegistrando = true;
			TempData["MiStoRegistrando"] = MiStoRegistrando;
			TempData["infoRegistrazione"] = "Inserisci qui sotto i tuoi dati di registrazione.";
			return RedirectToAction("Index", "Home");
		}


		// questo metodo serve per registrare un nuovo utente, una volta riempiti i campi ed effettuata la post viene controllato nel db se già esiste un utente con quel nome .
		// se non esiste viene fatta una insert nel db con il nuovo utente altrimenti messaggio errore, "Nome utente gia in uso. Scegline uno diverso";
		[HttpPost]
		public IActionResult Registrazione([Bind("Nome,Password")] Utente utente)
		{
			utente.Ruolo = "utente";
			ModelState.Remove("Ordini");

			try
			{
				var listaNomiGiaPresentiNelDB = _db.Utenti.ToList();

				// Verifica se il nome utente esiste già
				bool nomeEsiste = listaNomiGiaPresentiNelDB.Any(u => u.Nome == utente.Nome);

				if (nomeEsiste)
				{
					TempData["Errore"] = "Nome utente gia in uso. Scegline uno diverso";
					return RedirectToAction("Index", "Home");
				}

				// Se il nome utente non esiste, aggiungi l'utente
				_db.Utenti.Add(utente);
				_db.SaveChanges();
				TempData["Message"] = "Utente creato con successo.";
				return RedirectToAction("Index", "Home");
			}
			catch
			{
				TempData["Errore"] = "Si è verificato un errore. Riprova";
				return RedirectToAction("Index", "Home");
			}
		}

	}
}
