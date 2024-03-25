using Microsoft.EntityFrameworkCore;
using WebStoreApi.Models;

namespace WebStoreApi.Data
{
    public class WebStoreDbContext : DbContext
    {
        public WebStoreDbContext(DbContextOptions options) :base(options) { }
       
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<ResetPassword> ResetPassword { get; set; }
        
    }
}
