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
				// creo un nuovo oggetto di tipo Carrello che pusho nel carrello

				Carrello Prodottocarrello = new Carrello
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

				// creo una lista di carrelli e ci pusho il prodotto
				List<Carrello> cart = new List<Carrello>();
				cart.Add(Prodottocarrello);

				// serializzo il carrello e lo salvo in sessione
				HttpContext.Session.SetString("carrello", JsonConvert.SerializeObject(cart));

				TempData["Message"] = "Hai aggiunto il prodotto al carrello";
				return RedirectToAction("Index", "Prodotto");
			}

			TempData["Errore"] = "Il prodotto non esiste.Riprova";
			return RedirectToAction("Index", "Prodotto");

		}
		// GET: CarrelloController/Details/5
		//public ActionResult Details(int id)
		//{
		//    return View();
		//}

		// GET: CarrelloController/Create
		//public ActionResult Create()
		//{
		//    return View();
		//}

		//// POST: CarrelloController/Create
		//[HttpPost]
		//[ValidateAntiForgeryToken]
		//public ActionResult Create(IFormCollection collection)
		//{
		//    try
		//    {
		//        return RedirectToAction(nameof(Index));
		//    }
		//    catch
		//    {
		//        return View();
		//    }
		//}

		// GET: CarrelloController/Edit/5
		//public ActionResult Edit(int id)
		//{
		//    return View();
		//}

		//// POST: CarrelloController/Edit/5
		//[HttpPost]
		//[ValidateAntiForgeryToken]
		//public ActionResult Edit(int id, IFormCollection collection)
		//{
		//    try
		//    {
		//        return RedirectToAction(nameof(Index));
		//    }
		//    catch
		//    {
		//        return View();
		//    }
		//}

		// GET: CarrelloController/Delete/5
		//public ActionResult Delete(int id)
		//{
		//    return View();
		//}

		//// POST: CarrelloController/Delete/5
		//[HttpPost]
		//[ValidateAntiForgeryToken]
		//public ActionResult Delete(int id, IFormCollection collection)
		//{
		//    try
		//    {
		//        return RedirectToAction(nameof(Index));
		//    }
		//    catch
		//    {
		//        return View();
		//    }
		//}
	}
}
