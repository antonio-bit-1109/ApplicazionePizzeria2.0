using ApplicazionePizzeria2._0.data;
using ApplicazionePizzeria2._0.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ApplicazionePizzeria2._0.Controllers
{
	[Authorize]
	public class CarrelloController : Controller
	{

		private readonly ApplicationDbContext _context;

		public CarrelloController(ApplicationDbContext context)
		{
			_context = context;
		}
		// GET: CarrelloController

		// per la costruzione del carrello è stato necessario utilizzare una session. Prima di tutto inserire il service builder.Services.AddSession(); nel file Program.cs ed in seguito inserire app.UseSession(); nel file Program.cs
		// dopo di che ho creato un nuovo model carrello che avesse due proprietà: Prodotto e Quantità. In seguito ho creato un metodo per aggiungere un prodotto al carrello e un metodo per rimuovere un prodotto dal carrello.
		// la principale peculiarità del carrello è che , in quanto scritto con core, questo può contenere solo stringhe. Per questo motivo ho utilizzato la libreria Newtonsoft.Json per serializzare e deserializzare il carrello attraverso il metodo serialize e deserialize object.
		public ActionResult Index()
		{
			// Prendo il carrello dalla sessione
			var carrelloSession = HttpContext.Session.GetString("carrello");

			// Se il carrello esiste e non è vuoto
			if (!string.IsNullOrEmpty(carrelloSession))
			{
				// Deserializzo il carrello
				List<Carrello> carrello = JsonConvert.DeserializeObject<List<Carrello>>(carrelloSession);

				// Passo il carrello alla view
				return View(carrello);
			}

			// Se il carrello è vuoto, passo una nuova lista vuota alla view
			return View(new List<Carrello>());
		}

		// per aggiungere un prodotto al carrello questo metodo prende in input id e trova il relaltivo prodotto nel db. 
		// se questo oggetto esiste nel DB , viene ripreso il carrello dalla session, deserializzato, viene fatto un ciclo nel carrello per trovare il prodotto con lo stesso id inserito come input nel metodo,
		// ed aumentata la quantità del prodotto trovato di +1.
		//INfine il carrello viene di nuovo serializzato e salvato in sessione.

		// se il prodotto trovato grazie al parametro invece non esiste nel carrello, viene creato un nuovo oggetto carrello e inserito nel carrello presente in sessione.
		public IActionResult AggiungiAlCarrello(int? id)
		{
			bool prodottoGiaPresenteNelCarrello = false;
			var prodottoSelezionato = _context.Prodotti.Find(id);

			// se il prodotto esiste
			if (prodottoSelezionato != null)
			{
				// prendo il carrello dalla sessione 
				var carrelloSession = HttpContext.Session.GetString("carrello");

				// se il carrello esiste e non è vuoto
				if (carrelloSession != null)
				{
					// deserializzo il carrello 
					List<Carrello> cart = JsonConvert.DeserializeObject<List<Carrello>>(carrelloSession);

					// controllo i prodotti presenti nel carrello e se un prodotto ha gia id uguale a quello in entrata come parametro
					// incremento la quantità di 1

					foreach (var item in cart)
					{
						if (item.Prodotto.IdProdotto == id)
						{
							item.Quantita++;
							prodottoGiaPresenteNelCarrello = true;

							// serializzo di nuovo il carrello e lo salvo in sessione
							HttpContext.Session.SetString("carrello", JsonConvert.SerializeObject(cart));
							TempData["Message"] = "Hai aggiunto una quantità in più";
							return RedirectToAction("Index", "Prodotto");
						}
					}
				}

			}

			// se il prodotto non è presente nel carrello
			if (!prodottoGiaPresenteNelCarrello)
			{

				// creo un nuovo oggetto carrello e ci pusho il prodotto
				Carrello cart = new Carrello
				{
					Prodotto = new Prodotto
					{
						IdProdotto = prodottoSelezionato.IdProdotto,
						NomeProdotto = prodottoSelezionato.NomeProdotto,
						FotoProdotto = prodottoSelezionato.FotoProdotto,
						PrezzoProdotto = prodottoSelezionato.PrezzoProdotto,
						TempoConsegna = prodottoSelezionato.TempoConsegna,
						Ingredienti = prodottoSelezionato.Ingredienti
					},
					Quantita = 1
				};

				//prendiamo il carrello dalla sessione 
				var carrelloSession = HttpContext.Session.GetString("carrello");

				List<Carrello> carrello = new List<Carrello>();

				//se il carrello esiste e non è vuoto lo deserializzo
				if (!string.IsNullOrEmpty(carrelloSession))
				{
					carrello = JsonConvert.DeserializeObject<List<Carrello>>(carrelloSession);
				}

				//aggiungiamo il prodotto al carrello
				carrello.Add(cart);

				//serializziamo il carrello e lo salviamo in sessione
				HttpContext.Session.SetString("carrello", JsonConvert.SerializeObject(carrello));

				//// creo una lista di carrelli e ci pusho il prodotto
				//List<Carrello> cart = new List<Carrello>();
				//cart.Add(Prodottocarrello);

				//// serializzo il carrello e lo salvo in sessione
				//HttpContext.Session.SetString("carrello", JsonConvert.SerializeObject(cart));

				TempData["Message"] = "Hai aggiunto il prodotto al carrello";
				return RedirectToAction("Index", "Prodotto");
			}

			TempData["Errore"] = "Il prodotto non esiste.Riprova";
			return RedirectToAction("Index", "Prodotto");

		}




		//public IActionResult RimuoviItemDalCarrello(int id)
		//{
		//	if (HttpContext.Session.GetString("Carrello") == null)
		//	{
		//		return NotFound();
		//	}

		//	List<Carrello> cartFromSession = JsonConvert.DeserializeObject<List<Carrello>>(
		//		HttpContext.Session.GetString("Carrello")
		//	);
		//	var existingItem = cartFromSession.FirstOrDefault(t => t.Prodotto.IdProdotto == id);
		//	if (existingItem != null)
		//	{
		//		if (existingItem.Quantita > 1)
		//		{
		//			existingItem.Quantita--;
		//		}
		//		else
		//		{
		//			cartFromSession.Remove(existingItem);
		//		}
		//	}
		//	string jsonCart = JsonConvert.SerializeObject(cartFromSession);
		//	HttpContext.Session.SetString("Carrello", jsonCart);

		//	return Ok();
		//}


		// il metodo rimuovi dal carrello prende in input un id, viene ripreso il carrello dalla sessione,
		// il carrello viene deserializzato da stringa a lista e viene rimosso il prodotto con id uguale all'id inserito come parametro nel metodo.
		public IActionResult RimuoviItemDalCarrello(int id)
		{
			var carrelloSession = HttpContext.Session.GetString("carrello");
			if (carrelloSession != null)
			{
				List<Carrello> cart = JsonConvert.DeserializeObject<List<Carrello>>(carrelloSession);

				// Rimuovo tutti gli elementi con l'ID specificato
				cart.RemoveAll(item => item.Prodotto.IdProdotto == id);

				HttpContext.Session.SetString("carrello", JsonConvert.SerializeObject(cart));
				TempData["message"] = "Articolo rimosso dal carrello";
				return RedirectToAction("Index", "Carrello");
			}
			TempData["error"] = "Articolo non trovato";
			return RedirectToAction("Index", "Carrello");
		}

	}
}
