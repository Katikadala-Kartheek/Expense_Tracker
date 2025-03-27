using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Models;

namespace WebApp.Controllers
{
    [Authorize]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cats = await _context.Categories.Where(e => e.UserId == userId).ToListAsync();
            return View(cats);
        }

        public IActionResult Create()
        {
            return View(new Category());
        }

        [HttpPost]
        public IActionResult Create(Category category)
        {
            if(ModelState.IsValid == false)
            {
                return View(category);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Category category1 = new Category()
            {
                UserId=userId,
                CategoryName = category.CategoryName
            };
            _context.Categories.Add(category1);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        //Edit
        public IActionResult Edit(int id)
        {
            Category cat = _context.Categories.Find(id);
            return View(cat);
        }

        [HttpPost]
        public IActionResult Edit(Category category)
        {
            if (ModelState.IsValid == false)
            {
                return View(category);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Category category1 = new Category()
            {
                UserId = userId,
                CategoryName = category.CategoryName
            };
            _context.Categories.Add(category1);
            _context.SaveChanges();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Delete(int id)
        {
            Category cat = _context.Categories.Find(id);
            if (cat == null)
            {
                return NotFound();
            }
            _context.Categories.Remove(cat);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }


    }
}
