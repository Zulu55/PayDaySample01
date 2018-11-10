namespace PayDaySample01.Web.Helpers
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Models;
    using System;
    using System.Web.Configuration;

    public class UsersHelper : IDisposable
    {
        private static ApplicationDbContext userContext = new ApplicationDbContext();
        private static LocalDataContext db = new LocalDataContext();

        public static bool DeleteUser(string userName, string roleName)
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(userContext));
            var userASP = userManager.FindByEmail(userName);
            if (userASP == null)
            {
                return false;
            }

            var response = userManager.RemoveFromRole(userASP.Id, roleName);
            return response.Succeeded;
        }

        public static bool UpdateUserName(string currentUserName, string newUserName)
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(userContext));
            var userASP = userManager.FindByEmail(currentUserName);
            if (userASP == null)
            {
                return false;
            }

            userASP.UserName = newUserName;
            userASP.Email = newUserName;
            var response = userManager.Update(userASP);
            return response.Succeeded;
        }

        public static void CheckRole(string roleName)
        {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(userContext));

            // Check to see if Role Exists, if not create it
            if (!roleManager.RoleExists(roleName))
            {
                roleManager.Create(new IdentityRole(roleName));
            }
        }

        public static void CheckSuperUser()
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(userContext));
            var email = WebConfigurationManager.AppSettings["AdminUser"];
            var password = WebConfigurationManager.AppSettings["AdminPassWord"];
            var userASP = userManager.FindByName(email);
            if (userASP == null)
            {
                CreateUserASP(email, "Admin", password);
                AddRoleToUser(email, "View");
                AddRoleToUser(email, "Create");
                AddRoleToUser(email, "Edit");
                AddRoleToUser(email, "Delete");
                return;
            }
        }

        /// <summary>
        /// Assing the role to user
        /// </summary>
        /// <param name="email">The user email</param>
        /// <param name="role">The role to be assing</param>
        public static void AddRoleToUser(string email, string role)
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(userContext));
            var userASP = userManager.FindByEmail(email);
            if (userASP == null)
            {
                return;
            }

            userManager.AddToRole(userASP.Id, role);
        }

        public static bool CreateUserASP(string email, string roleName, string password)
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(userContext));

            var userASP = new ApplicationUser
            {
                Email = email,
                UserName = email,
            };

            var result = userManager.Create(userASP, password);
            if (result.Succeeded)
            {
                userManager.AddToRole(userASP.Id, roleName);
            }

            return result.Succeeded;
        }

        public static bool CreateUserASP(string email, string password)
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(userContext));

            var userASP = new ApplicationUser
            {
                Email = email,
                UserName = email,
            };

            var result = userManager.Create(userASP, password);
            return result.Succeeded;
        }

        /// <summary>
        /// Unassign the rote from user
        /// </summary>
        /// <param name="email">The email</param>
        /// <param name="role">The role</param>
        public static void RemoveRoleToUser(string email, string role)
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(userContext));
            var userASP = userManager.FindByEmail(email);
            if (userASP == null)
            {
                return;
            }

            userManager.RemoveFromRole(userASP.Id, role);
        }

        public void Dispose()
        {
            userContext.Dispose();
            db.Dispose();
        }
    }
}