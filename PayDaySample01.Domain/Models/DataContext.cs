using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayDaySample01.Domain.Models
{
    public class DataContext : DbContext
    {
        public DbSet<City> Cities { get; set; }

        public DataContext() : base("DefaultConnection")
        {

        }
    }
}
