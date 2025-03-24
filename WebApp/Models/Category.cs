using Microsoft.AspNetCore.Identity;

namespace WebApp.Models
{
    public class Category
    {
        public string? UserId { get; set; }
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }

        public virtual IdentityUser? User { get; set; }
    }
}
