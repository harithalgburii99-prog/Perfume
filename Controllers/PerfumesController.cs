using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PerfumeStore.Models;

namespace PerfumeStore.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PerfumesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PerfumesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Dashboard()
        {
            var perfumes = await _context.Perfumes.ToListAsync();
            return View(perfumes);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Perfume perfume)
        {
            if (ModelState.IsValid)
            {
                _context.Perfumes.Add(perfume);
                await _context.SaveChangesAsync();
                return RedirectToAction("Dashboard");
            }
            return View(perfume);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var perfume = await _context.Perfumes.FindAsync(id);
            if (perfume != null)
            {
                _context.Perfumes.Remove(perfume);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Dashboard");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var perfume = await _context.Perfumes.FindAsync(id);
            if (perfume == null)
            {
                return NotFound();
            }
            return View(perfume);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Perfume perfume)
        {
            if (id != perfume.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(perfume);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PerfumeExists(perfume.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Dashboard));
            }
            return View(perfume);
        }

        private bool PerfumeExists(int id)
        {
            return _context.Perfumes.Any(e => e.Id == id);
        }
    }
}
