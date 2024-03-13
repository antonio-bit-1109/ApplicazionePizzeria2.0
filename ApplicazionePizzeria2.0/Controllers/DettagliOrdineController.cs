﻿using ApplicazionePizzeria2._0.data;
using ApplicazionePizzeria2._0.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ApplicazionePizzeria2._0.Controllers
{
	public class DettagliOrdineController : Controller
	{
		private readonly ApplicationDbContext _context;

		public DettagliOrdineController(ApplicationDbContext context)
		{
			_context = context;
		}

		// GET: DettagliOrdine
		public async Task<IActionResult> Index()
		{
			var applicationDbContext = _context.DettagliOrdini.Include(d => d.Ordine).Include(d => d.Prodotto);
			return View(await applicationDbContext.ToListAsync());
		}

		// GET: DettagliOrdine/Details/5
		public async Task<IActionResult> Details(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var dettagliOrdine = await _context.DettagliOrdini
				.Include(d => d.Ordine)
				.Include(d => d.Prodotto)
				.FirstOrDefaultAsync(m => m.IdDettagliOrdine == id);
			if (dettagliOrdine == null)
			{
				return NotFound();
			}

			return View(dettagliOrdine);
		}

		// GET: DettagliOrdine/Create
		public IActionResult Create(int? id)
		{
			ViewData["IdOrdine"] = id;
			//ViewData["IdOrdine"] = new SelectList(_context.Ordini, "IdOrdine", "IdOrdine");
			ViewData["IdProdotto"] = new SelectList(_context.Prodotti, "IdProdotto", "NomeProdotto");

			return View();
		}

		// POST: DettagliOrdine/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("IdOrdine,IdProdotto,Quantita,Prezzo,OrdineEvaso, CostoTotale")] DettagliOrdine dettagliOrdine)
		{
			ModelState.Remove("Prodotto");
			ModelState.Remove("Ordine");

			dettagliOrdine.CostoTotale = dettagliOrdine.Quantita * dettagliOrdine.Prezzo;

			if (ModelState.IsValid)
			{
				_context.Add(dettagliOrdine);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}
			ViewData["IdOrdine"] = new SelectList(_context.Ordini, "IdOrdine", "indirizzoSpedizione", dettagliOrdine.IdOrdine);
			ViewData["IdProdotto"] = new SelectList(_context.Prodotti, "IdProdotto", "FotoProdotto", dettagliOrdine.IdProdotto);
			return View(dettagliOrdine);
		}

		// GET: DettagliOrdine/Edit/5
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var dettagliOrdine = await _context.DettagliOrdini.FindAsync(id);
			if (dettagliOrdine == null)
			{
				return NotFound();
			}
			ViewData["IdOrdine"] = new SelectList(_context.Ordini, "IdOrdine", "indirizzoSpedizione", dettagliOrdine.IdOrdine);
			ViewData["IdProdotto"] = new SelectList(_context.Prodotti, "IdProdotto", "FotoProdotto", dettagliOrdine.IdProdotto);
			return View(dettagliOrdine);
		}

		// POST: DettagliOrdine/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, [Bind("IdDettagliOrdine,IdOrdine,IdProdotto,Quantita,Prezzo,OrdineEvaso")] DettagliOrdine dettagliOrdine)
		{
			if (id != dettagliOrdine.IdDettagliOrdine)
			{
				return NotFound();
			}

			ModelState.Remove("Prodotto");
			ModelState.Remove("Ordine");

			if (ModelState.IsValid)
			{
				try
				{
					_context.Update(dettagliOrdine);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!DettagliOrdineExists(dettagliOrdine.IdDettagliOrdine))
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
			ViewData["IdOrdine"] = new SelectList(_context.Ordini, "IdOrdine", "indirizzoSpedizione", dettagliOrdine.IdOrdine);
			ViewData["IdProdotto"] = new SelectList(_context.Prodotti, "IdProdotto", "FotoProdotto", dettagliOrdine.IdProdotto);
			return View(dettagliOrdine);
		}

		// GET: DettagliOrdine/Delete/5
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var dettagliOrdine = await _context.DettagliOrdini
				.Include(d => d.Ordine)
				.Include(d => d.Prodotto)
				.FirstOrDefaultAsync(m => m.IdDettagliOrdine == id);
			if (dettagliOrdine == null)
			{
				return NotFound();
			}

			return View(dettagliOrdine);
		}

		// POST: DettagliOrdine/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			var dettagliOrdine = await _context.DettagliOrdini.FindAsync(id);
			if (dettagliOrdine != null)
			{
				_context.DettagliOrdini.Remove(dettagliOrdine);
			}

			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		private bool DettagliOrdineExists(int id)
		{
			return _context.DettagliOrdini.Any(e => e.IdDettagliOrdine == id);
		}


		[HttpGet]
		public async Task<IActionResult> FetchCostoprodottoSelezionato(string idProdotto)
		{

			var prezzoProdotto = await _context.Prodotti
				.Where(p => p.IdProdotto == Convert.ToInt32(idProdotto))
				.Select(p => p.PrezzoProdotto)
				.FirstOrDefaultAsync();

			return Json(prezzoProdotto);

		}
	}
}