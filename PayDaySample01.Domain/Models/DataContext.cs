namespace PayDaySample01.Domain.Models
{
    using System.Data.Entity;

    public class DataContext : DbContext
    {
        public DbSet<City> Cities { get; set; }

        public DbSet<Employee> Employees { get; set; }

        public DataContext() : base("DefaultConnection")
        {

        }
    }
}
