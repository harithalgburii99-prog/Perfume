using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PerfumeStore.Models;
using System.Security.Claims;

namespace PerfumeStore.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId)) return RedirectToAction("Login", "Account");

            var cartItems = await _context.Cart
                .Include(c => c.Perfume)
                .Where(c => c.UserId == userId)
                .Select(c => new CartViewModel
                {
                    CartId = c.CartId,
                    PerfumeName = c.Perfume.Name,
                    Price = c.Perfume.Price ?? 0
                })
                .ToListAsync();

            return View(cartItems);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(int perfumeId)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId)) return RedirectToAction("Login", "Account");

            var cart = new Cart
            {
                UserId = userId,
                PerfumeId = perfumeId
            };

            _context.Cart.Add(cart);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int id)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId)) return RedirectToAction("Login", "Account");

            var cartItem = await _context.Cart.FirstOrDefaultAsync(c => c.CartId == id && c.UserId == userId);
            if (cartItem != null)
            {
                _context.Cart.Remove(cartItem);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId)) return RedirectToAction("Login", "Account");

            var cartItems = await _context.Cart.Where(c => c.UserId == userId).ToListAsync();
            
            foreach (var item in cartItems)
            {
                _context.Orders.Add(new Order
                {
                    UserId = item.UserId,
                    PerfumeId = item.PerfumeId,
                    OrderDate = DateTime.Now
                });
            }

            _context.Cart.RemoveRange(cartItems);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Your order has been placed successfully!";
            return RedirectToAction("Index");
        }
    }
}
