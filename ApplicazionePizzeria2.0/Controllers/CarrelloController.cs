using ApplicazionePizzeria2._0.data;
using ApplicazionePizzeria2._0.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ApplicazionePizzeria2._0.Controllers
{
	public class CarrelloController : Controller
	{

		private readonly ApplicationDbContext _context;

		public CarrelloController(ApplicationDbContext context)
		{
			_context = context;
		}
		// GET: CarrelloController
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
