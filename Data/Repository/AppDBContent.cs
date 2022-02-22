using Microsoft.EntityFrameworkCore;
using Order_Board.Data.Models;
using Order_Board.Models;

namespace Order_Board.Data.Repository
{
    public class AppDBContent : DbContext
    {
        public AppDBContent(DbContextOptions<AppDBContent> options)
                : base(options)
        {
        }
        //public DbSet<Product> Products { get; set; }
        //public DbSet<Department> Departments { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Order> Orders { get; set; }
    }
}
