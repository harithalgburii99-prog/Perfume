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
    }
}
