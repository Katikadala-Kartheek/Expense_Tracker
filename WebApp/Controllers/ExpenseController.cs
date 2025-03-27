using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using WebApp.Data;
using WebApp.Models;

namespace WebApp.Controllers
{
    [Authorize]
    public class ExpenseController : Controller
    {

        private readonly ApplicationDbContext _context;

        public ExpenseController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userExpenses = await _context.Expenses.Where(e => e.UserId == userId).ToListAsync();
            return View(userExpenses);
        }

        public async Task<IActionResult> Create()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var categories = await _context.Categories
                .Where(c => c.UserId == userId)
                .Select(c => new SelectListItem
                {
                    Value = c.CategoryId.ToString(),
                    Text = c.CategoryName
                }).ToListAsync();

            ViewBag.Categories = categories;
            return View(new Expense());
        }

        [HttpPost]
        public IActionResult Create(Expense expense)
        {
            if (expense == null)
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                Expense exp = new Expense()
                {
                    ExpenseId = expense.ExpenseId,
                    Amount = expense.Amount,
                    UserId=userId,
                    CategoryId = expense.CategoryId,
                    Date = expense.Date,
                    Description = expense.Description,
                    Category = _context.Categories.Find(expense.CategoryId)
                };
                _context.Expenses.Add(exp);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            var categories = _context.Categories
                .Select(c => new SelectListItem
                {
                    Value = c.CategoryId.ToString(),
                    Text = c.CategoryName
                }).ToList();
            ViewBag.Categories = categories;
            return View(expense);
        }

        //Edit
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Expense expense = _context.Expenses.Find(id);
            if (expense == null)
            {
                return NotFound();
            }
            var categories = _context.Categories
                .Select(c => new SelectListItem
                {
                    Value = c.CategoryId.ToString(),
                    Text = c.CategoryName
                }).ToList();
            ViewBag.Categories = categories;
            return View(expense);
        }

        [HttpPost]
        public IActionResult Edit(Expense expense)
        {
            if (!ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                expense.UserId = userId;
                _context.Expenses.Update(expense);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            var categories = _context.Categories
                .Select(c => new SelectListItem
                {
                    Value = c.CategoryId.ToString(),
                    Text = c.CategoryName
                }).ToList();
            ViewBag.Categories = categories;
            return View(expense);
        }

        //Delete
        public IActionResult Delete(int id)
        {
            Expense expense = _context.Expenses.Find(id);
            if (expense == null)
            {
                return NotFound();
            }
            _context.Expenses.Remove(expense);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        //Expense Filter
        public IActionResult FilterExpenses(string filterType)
        {
            decimal amount = 0;
            var expenses = _context.Expenses.AsQueryable();
            if (filterType == "week")
            {
                var startDate = DateTime.Today.AddDays(-7);
                amount = expenses.Where(e => e.Date >= startDate).Sum(e=>e.Amount);
            }
            else if (filterType == "month")
            {
                amount = expenses.Where(e => e.Date.Month == DateTime.Today.Month &&
                                               e.Date.Year == DateTime.Today.Year)
                                                    .Sum(e => e.Amount);
            }
            return Json(new {total=amount});
        }


    }
}
