using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace WebApp.Models
{
    public class Expense
    {
        public int Id { get; set; }
        [Key]
        public int ExpenseId { get; set; }
        public string UserId { get; set; }
        public decimal Amount { get; set; }
        public int CategoryId { get; set; }
        public DateTime Date { get; set; }
        public string? Description { get; set; }

        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }

        public virtual IdentityUser User { get; set; }
    }
}
