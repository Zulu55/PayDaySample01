namespace PayDaySample01.Web.Models
{
    using Domain.Models;

    public class LocalDataContext : DataContext
    {
        public System.Data.Entity.DbSet<PayDaySample01.Domain.Models.User> Users { get; set; }
    }
}