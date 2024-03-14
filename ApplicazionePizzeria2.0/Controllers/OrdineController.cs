using ApplicazionePizzeria2._0.data;
using ApplicazionePizzeria2._0.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
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
		public async Task<IActionResult> Create([Bind("indirizzoSpedizione,IdUtente,NoteAggiuntive")] Ordine ordine)
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
						HttpContext.Session.Remove("carrello");
					}


				}


				TempData["Message"] = "Ordine creato con successo!";
				return RedirectToAction("Index", "Login");

			}
			ViewData["IdUtente"] = new SelectList(_context.Utenti, "IdUtente", "Nome", ordine.IdUtente);

			//return View(ordine);

			return RedirectToAction(nameof(Index));
		}

		// GET: Ordine/Edit/5
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


		public IActionResult FetchTotOrdniEvasi()
		{



			return Json(true);
		}
	}
}
