namespace PayDaySample01.Domain.Models
{
    using System.Data.Entity;
    using System.Data.Entity.ModelConfiguration.Conventions;

    public class DataContext : DbContext
    {
        public DbSet<City> Cities { get; set; }

        public DbSet<Employee> Employees { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<RecordTime> RecordTimes { get; set; }

        public DbSet<CalculatedSalary> CalculatedSalaries { get; set; }

        public DbSet<Relation> Relations { get; set; }

        public DbSet<Dependent> Dependents { get; set; }

        public DataContext() : base("DefaultConnection")
        {

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
        }
    }
}
