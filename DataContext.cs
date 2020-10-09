using Microsoft.EntityFrameworkCore;
using TestAutoMapCollection.Models;

namespace TestAutoMapCollection
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Order> Orders { get; set; }

        public DbSet<OrderLine> OrderLines { get; set; }
    }
}