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

		public async Task<IActionResult> Logout()
		{
			await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

			TempData["Message"] = "Logout effettuato.";
			return RedirectToAction("Index", "Home");
		}


		public IActionResult Registrazione()
		{
			bool MiStoRegistrando;


			MiStoRegistrando = true;
			TempData["MiStoRegistrando"] = MiStoRegistrando;
			return RedirectToAction("Index", "Home");
		}

		[HttpPost]
		public IActionResult Registrazione(Utente utente)
		{

			utente.Ruolo = "utente";

			ModelState.Remove("Ordini");

			if (ModelState.IsValid)
			{
				_db.Utenti.Add(utente);
				_db.SaveChanges();
				TempData["Message"] = "Utente creato con successo.";
				return RedirectToAction("Index");
			}

			// controlla l'oggetto utente che ricevi e se non esiste lo aggiunge al db
			// altrimenti ritorna un errore e invita l'utente a riprovare


			TempData["Message"] = "Dati inseriti Errati. RIprova";
			return RedirectToAction("Index");
		}
	}
}
