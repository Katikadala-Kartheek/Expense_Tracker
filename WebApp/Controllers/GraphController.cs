using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;

namespace WebApp.Controllers
{
    [Authorize]
    public class GraphController : Controller
    {

        private readonly ApplicationDbContext _context;

        public GraphController(ApplicationDbContext context)
        {
            _context = context;
        }


        public IActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> GetExpense(DateTime? startDate, DateTime? endDate)
        {
            var expenses = _context.Expenses.AsQueryable();
            if (startDate.HasValue && endDate.HasValue)
            {
                expenses = expenses.Where(e => e.Date >= startDate && e.Date <= endDate);
            }
            var groupedData = await expenses
                .GroupBy(e => e.Category)
                .Select(g => new
                {
                    Category = g.Key,
                    TotalAmount = g.Sum(e => e.Amount)
                })
                .ToListAsync();
            // Debugging: Check if data is returned
            if (!groupedData.Any())
            {
                Console.WriteLine("No data found for the selected date range.");
            }
            return Json(groupedData);
        }



    }
}
