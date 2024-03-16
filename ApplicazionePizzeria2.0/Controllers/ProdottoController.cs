using ApplicazionePizzeria2._0.data;
using ApplicazionePizzeria2._0.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApplicazionePizzeria2._0.Controllers
{
	public class ProdottoController : Controller
	{
		private readonly ApplicationDbContext _context;

		public ProdottoController(ApplicationDbContext context)
		{
			_context = context;
		}

		// GET: Prodottoes
		public async Task<IActionResult> Index()
		{
			return View(await _context.Prodotti.ToListAsync());
		}

		// GET: Prodottoes/Details/5
		public async Task<IActionResult> Details(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var prodotto = await _context.Prodotti
				.FirstOrDefaultAsync(m => m.IdProdotto == id);
			if (prodotto == null)
			{
				return NotFound();
			}

			return View(prodotto);
		}

		// GET: Prodottoes/Create
		[Authorize(Roles = "admin")]
		public IActionResult Create()
		{
			return View();
		}

		// POST: Prodottoes/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("NomeProdotto,FotoProdotto,PrezzoProdotto,TempoConsegna,Ingredienti")] Prodotto prodotto, IFormFile fotoProdotto)
		{
			ModelState.Remove("DettagliOrdini");
			ModelState.Remove("FotoProdotto");

			if (ModelState.IsValid)
			{
				if (fotoProdotto != null && fotoProdotto.Length > 0)
				{
					var fileName = Path.GetFileName(fotoProdotto.FileName);
					//var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);
					var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/imgs", fileName);

					using (var fileStream = new FileStream(filePath, FileMode.Create))
					{
						await fotoProdotto.CopyToAsync(fileStream);
					}

					prodotto.FotoProdotto = fileName;
				}


				_context.Add(prodotto);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}
			return View(prodotto);
		}

		// GET: Prodottoes/Edit/5
		[Authorize(Roles = "admin")]
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var prodotto = await _context.Prodotti.FindAsync(id);
			if (prodotto == null)
			{
				return NotFound();
			}
			return View(prodotto);
		}

		// POST: Prodottoes/Edit/5

		[Authorize(Roles = "admin")]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, [Bind("IdProdotto,NomeProdotto,FotoProdotto,PrezzoProdotto,TempoConsegna,Ingredienti")] Prodotto prodotto)
		{
			if (id != prodotto.IdProdotto)
			{
				return NotFound();
			}

			ModelState.Remove("DettagliOrdini");

			if (ModelState.IsValid)
			{
				try
				{
					_context.Update(prodotto);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!ProdottoExists(prodotto.IdProdotto))
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
			return View(prodotto);
		}

		// GET: Prodottoes/Delete/5
		[Authorize(Roles = "admin")]
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var prodotto = await _context.Prodotti
				.FirstOrDefaultAsync(m => m.IdProdotto == id);
			if (prodotto == null)
			{
				return NotFound();
			}

			return View(prodotto);
		}

		// POST: Prodottoes/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = "admin")]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			var prodotto = await _context.Prodotti.FindAsync(id);
			if (prodotto != null)
			{
				_context.Prodotti.Remove(prodotto);
			}

			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		private bool ProdottoExists(int id)
		{
			return _context.Prodotti.Any(e => e.IdProdotto == id);
		}

	}
}
