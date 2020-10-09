using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TestAutoMapCollection
{
    public class ContextFactory : IDesignTimeDbContextFactory<DataContext>
    {
        public DataContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DataContext>();
            optionsBuilder.UseSqlServer(@"Data Source=.\SQLEXPRESS; Initial Catalog=TEST_AUTOMAP2;Integrated Security=True;");

            return new DataContext(optionsBuilder.Options);
        }
    }
}