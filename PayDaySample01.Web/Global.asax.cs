namespace PayDaySample01.Web
{
    using System;
    using System.Data.Entity;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using System.Web.Optimization;
    using System.Web.Routing;
    using Helpers;
    using PayDaySample01.Web.Models;

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<
                Models.LocalDataContext, 
                Migrations.Configuration>());
            this.CheckRolesAndSuperUser();
            this.CheckCities();
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        private async Task CheckCities()
        {
            var db = new LocalDataContext();
            var countCities = await db.Cities.CountAsync();
            if (countCities == 0)
            {
                await this.SeedCities(db);
            }

            db.Dispose();
        }

        private async Task SeedCities(LocalDataContext db)
        {
            db.Cities.Add(new Domain.Models.City
            {
                Name = "Panamá"
            });

            db.Cities.Add(new Domain.Models.City
            {
                Name = "Las Tablas"
            });

            db.Cities.Add(new Domain.Models.City
            {
                Name = "David"
            });

            await db.SaveChangesAsync();
        }

        private void CheckRolesAndSuperUser()
        {
            UsersHelper.CheckRole("Admin");
            UsersHelper.CheckRole("Employee");
            UsersHelper.CheckRole("View");
            UsersHelper.CheckRole("Create");
            UsersHelper.CheckRole("Edit");
            UsersHelper.CheckRole("Delete");
            UsersHelper.CheckSuperUser();
        }
    }
}
