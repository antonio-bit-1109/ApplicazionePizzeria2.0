using ApplicazionePizzeria2._0.data;
using ApplicazionePizzeria2._0.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Stripe.Checkout;
using System.Security.Claims;

namespace ApplicazionePizzeria2._0.Controllers
{

	public class OrdineController : Controller
	{
		private readonly ApplicationDbContext _context;

		public OrdineController(ApplicationDbContext context)
		{
			_context = context;
		}

		// GET: Ordine
		public async Task<IActionResult> Index()
		{
			var applicationDbContext = _context.Ordini.Include(o => o.Utente);
			return View(await applicationDbContext.ToListAsync());
		}

		// GET: Ordine/Details/5
		public async Task<IActionResult> Details(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var ordine = await _context.Ordini
				.Include(o => o.Utente)
				.FirstOrDefaultAsync(m => m.IdOrdine == id);
			if (ordine == null)
			{
				return NotFound();
			}

			return View(ordine);
		}

		// GET: Ordine/Create
		[Authorize]
		public IActionResult Create()
		{
			//ViewData["IdUtente"] = new SelectList(_context.Utenti, "IdUtente", "Nome");
			var idUtenteLoggato = User.FindFirstValue(ClaimTypes.NameIdentifier);

			ViewData["IdUTENTE"] = Convert.ToInt32(idUtenteLoggato);
			return View();
		}



		// POST: Ordine/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize]

		// in questo metodo viene creato l'ordine e successivamente il relativo dettaglio ordine. 
		// una volta che l'ordine è crato mi salvo da parte il suo id.
		// richiamo il carrello dalla sessione contenente i prdotti salvati 
		//per ogni prodotto inserito nel carrello creo il relativo dettaglio ordine che sarà correlato sempre allo stesso ordine
		//(tutti i dettagli ordini avranno lo stesso idordine) e avrà come idprodotto, nome, foto ecc lo stesso valore del prodotto inserito nel carrello e come quantità la quantità inserita nel carrello.
		public async Task<IActionResult> Create([Bind("indirizzoSpedizione,IdUtente,NoteAggiuntive, DataDellaConsegna")] Ordine ordine)
		{
			ModelState.Remove("Utente");
			ModelState.Remove("DettagliOrdini");

			if (ModelState.IsValid)
			{
				_context.Add(ordine);
				await _context.SaveChangesAsync();


				//reindirizzo alla paigna iniziale con il messaggio di successo
				// devo popolare manualmente la tabella dettagliOrdine con i prodotti presenti nel carrello della session

				//recupero l'id dell'ordine appena creato da inserire nella tabella dettagliOrdine
				var OrdineAppenaCreato = ordine.IdOrdine;
				//recupero la session
				var carrelloSession = HttpContext.Session.GetString("carrello");

				if (carrelloSession != null)
				{
					// deserializzo il carrello
					List<Carrello> cart = JsonConvert.DeserializeObject<List<Carrello>>(carrelloSession);

					//Estraggo i dati dal carrello per ogni prodotto presente nel carrello estraggo i valori di prodotto e quantita
					// e li assegno all'oggetto dettagliOrdine
					foreach (var item in cart)
					{
						int idprodotto = item.Prodotto.IdProdotto;
						string nomeprodotto = item.Prodotto.NomeProdotto;
						string fotoprodotto = item.Prodotto.FotoProdotto;
						double prezzoprodotto = item.Prodotto.PrezzoProdotto;
						int tempoconsegna = item.Prodotto.TempoConsegna;
						string ingredienti = item.Prodotto.Ingredienti;

						int quantita = item.Quantita;

						//creo un nuovo oggetto dettagliOrdine
						DettagliOrdine dettagliOrdine = new DettagliOrdine
						{
							IdOrdine = OrdineAppenaCreato,
							IdProdotto = idprodotto,
							Quantita = quantita,
							Prezzo = prezzoprodotto,
							CostoTotale = prezzoprodotto * quantita
						};

						//aggiungo l'oggetto dettagliOrdine al db
						_context.Add(dettagliOrdine);
						await _context.SaveChangesAsync();


						// svuoto il carrello
						//HttpContext.Session.Remove("carrello");



					}

				}

				//TempData["Message"] = "Ordine creato con successo!";
				//return RedirectToAction("Index", "Login");

			}
			ViewData["IdUtente"] = new SelectList(_context.Utenti, "IdUtente", "Nome", ordine.IdUtente);

			// sessione di pagamento con Stripe 
			var altroNomeCarrello = HttpContext.Session.GetString("carrello");

			if (altroNomeCarrello != null)
			{
				List<Carrello> cart = JsonConvert.DeserializeObject<List<Carrello>>(altroNomeCarrello);

				var domain = "https://localhost:7111/";

				var Options = new SessionCreateOptions
				{
					SuccessUrl = domain + "Login/Index",
					CancelUrl = domain + "Login/Index",
					LineItems = new List<SessionLineItemOptions>(),
					Mode = "payment",
					CustomerEmail = "EmailProva@gmail.it"
				};

				foreach (var item in cart)
				{
					var sessionLineItem = new SessionLineItemOptions
					{
						PriceData = new SessionLineItemPriceDataOptions
						{
							UnitAmount = (long)item.Prodotto.PrezzoProdotto * 100,
							Currency = "eur",
							ProductData = new SessionLineItemPriceDataProductDataOptions
							{
								Name = item.Prodotto.NomeProdotto,
								Description = item.Prodotto.Ingredienti,
								Images = new List<string> { domain + "/imgs/" + item.Prodotto.FotoProdotto }
							}
						},
						Quantity = item.Quantita
					};

					Options.LineItems.Add(sessionLineItem);
				}
				// creo la sessione di Stripe e la invio al client
				var service = new SessionService();
				Session session = service.Create(Options);
				Response.Headers.Add("Location", session.Url);
			}

			// svuoto il carrello
			HttpContext.Session.Remove("carrello");

			// reindirizzo alla pagina di successo
			return new StatusCodeResult(303);
			//return View(ordine);

			//return RedirectToAction(nameof(Index));
		}


		//public IActionResult ProcediAlPagamento()
		//{

		//}

		// GET: Ordine/Edit/5
		[Authorize(Roles = "admin")]
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var ordine = await _context.Ordini.FindAsync(id);
			if (ordine == null)
			{
				return NotFound();
			}
			ViewData["IdUtente"] = new SelectList(_context.Utenti, "IdUtente", "Nome", ordine.IdUtente);
			return View(ordine);
		}

		// POST: Ordine/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = "admin")]
		public async Task<IActionResult> Edit(int id, [Bind("IdOrdine,indirizzoSpedizione,IdUtente,NoteAggiuntive")] Ordine ordine)
		{
			if (id != ordine.IdOrdine)
			{
				return NotFound();
			}

			ModelState.Remove("Utente");
			ModelState.Remove("DettagliOrdini");

			if (ModelState.IsValid)
			{
				try
				{
					_context.Update(ordine);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!OrdineExists(ordine.IdOrdine))
					{
						return NotFound();
					}
					else
					{
						throw;
					}
				}
				return RedirectToAction(nameof(Index));
			}
			ViewData["IdUtente"] = new SelectList(_context.Utenti, "IdUtente", "Nome", ordine.IdUtente);
			return View(ordine);
		}

		// GET: Ordine/Delete/5
		[Authorize(Roles = "admin")]
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var ordine = await _context.Ordini
				.Include(o => o.Utente)
				.FirstOrDefaultAsync(m => m.IdOrdine == id);
			if (ordine == null)
			{
				return NotFound();
			}

			return View(ordine);
		}

		// POST: Ordine/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = "admin")]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			var ordine = await _context.Ordini.FindAsync(id);
			if (ordine != null)
			{
				_context.Ordini.Remove(ordine);
			}

			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		private bool OrdineExists(int id)
		{
			return _context.Ordini.Any(e => e.IdOrdine == id);
		}


		// questo metodo restituisce il riepilogo degli ordini effettuati da un cliente.
		// se l'utente è autenticato recupero il suo idutente.
		// recupero tutti gli ordini effettuati da quell'utente in una lista.
		// per ogni ordine del cliente recupero lidordine e i dettagli ordine associati a quell'ordine.
		// per ogni dettaglio ordine recupero il'id del prodotto associato e il nome del prodotto associato.
		// creoun nuovo oggetto riepilogo ordine che ha come proprietà l'ordine e una lista di tutti i dettagli ordini associati a quell'ordine.
		// passo la lista alla view e ciclo la lista stessa per ricavare tutti i dettagli di ogni singolo ordine.
		[Authorize]
		public IActionResult RiepilogoOrdine()
		{
			if (User.Identity.IsAuthenticated)
			{
				var idUtenteLoggato = User.FindFirst(ClaimTypes.NameIdentifier).Value;

				// select * from ordini where idutente = idutenteLoggato
				var ordiniDelCliente = _context.Ordini.Where(o => o.IdUtente == Convert.ToInt32(idUtenteLoggato)).ToList();

				List<RiepilogoOrdine> riepiloghi = new List<RiepilogoOrdine>();

				foreach (var singoloOrdine in ordiniDelCliente)
				{
					var singoloordine = singoloOrdine.IdOrdine;
					var DettagliOrdine = _context.DettagliOrdini.Where(d => d.IdOrdine == singoloordine).ToList();

					foreach (var singoloDettaglioOrdine in DettagliOrdine)
					{
						var idSingoloProdotto = singoloDettaglioOrdine.IdProdotto;
						var nomeProdottoAssociato = _context.Prodotti.Where(p => p.IdProdotto == idSingoloProdotto).FirstOrDefault().NomeProdotto;
					}

					var riepilogo = new RiepilogoOrdine
					{
						Ordine = singoloOrdine,
						ListaDettagliOrdine = DettagliOrdine
					};

					riepiloghi.Add(riepilogo);
				}


				return View(riepiloghi);

			}

			return NotFound();

		}

		// metodo che restituisce il totale incassato in una data specifica.
		public IActionResult FetchTotIncassatoInData(DateTime data)
		{
			var totale = _context.DettagliOrdini
				.Join(_context.Ordini, dett => dett.IdOrdine, ord => ord.IdOrdine, (dett, ord) => new { dett, ord })
				.Where(x => x.ord.DataDellaConsegna.Date == data)
				.Sum(x => x.dett.CostoTotale);

			return Json(totale);
		}


		// metodo che restituisce il costo totale di un ordine specifico.
		public IActionResult TotCostoOrdine(string idOrdine)
		{
			var totalissimo = _context.DettagliOrdini.Where(d => d.IdOrdine == Convert.ToInt32(idOrdine)).Sum(d => d.CostoTotale);

			return Json(totalissimo);
		}

	}
}
