namespace PayDaySample01.Web.Controllers
{
    using Domain.Models;
    using Helpers;
    using Models;
    using System;
    using System.Data.Entity;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private LocalDataContext db = new LocalDataContext();

        // GET: Users
        public async Task<ActionResult> Index()
        {
            return View(await db.Users.ToListAsync());
        }

        // GET: Users/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = await db.Users.FindAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // GET: Users/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(User user)
        {
            if (ModelState.IsValid)
            {
                var response = UsersHelper.CreateUserASP(user.Email, user.Password);
                if (response)
                {
                    if (user.IsAdmin)
                    {
                        UsersHelper.AddRoleToUser(user.Email, "Admin");
                    }

                    if (user.CanCreate)
                    {
                        UsersHelper.AddRoleToUser(user.Email, "Create");
                    }

                    if (user.CanDelete)
                    {
                        UsersHelper.AddRoleToUser(user.Email, "Delete");
                    }

                    if (user.CanEdit)
                    {
                        UsersHelper.AddRoleToUser(user.Email, "Edit");
                    }

                    if (user.CanView)
                    {
                        UsersHelper.AddRoleToUser(user.Email, "View");
                    }

                    db.Users.Add(user);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }

                ModelState.AddModelError(string.Empty, "Ya existe un usuario registrado con ese correo.");
            }

            return View(user);
        }

        // GET: Users/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var user = await db.Users.FindAsync(id);

            if (user == null)
            {
                return HttpNotFound();
            }

            user.Password = "000000";
            user.Confirm = "000000";
            return View(user);
        }

        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(User user)
        {
            if (ModelState.IsValid)
            {
                this.CheckPermissions(user);
                db.Entry(user).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(user);
        }

        /// <summary>
        /// This method verifies the user permissions
        /// </summary>
        /// <param name="user">The user</param>
        private void CheckPermissions(User user)
        {
            if (user.IsAdmin)
            {
                UsersHelper.AddRoleToUser(user.Email, "Admin");
            }
            else
            {
                UsersHelper.RemoveRoleToUser(user.Email, "Admin");
            }

            if (user.CanCreate)
            {
                UsersHelper.AddRoleToUser(user.Email, "Create");
            }
            else
            {
                UsersHelper.RemoveRoleToUser(user.Email, "Create");
            }

            if (user.CanDelete)
            {
                UsersHelper.AddRoleToUser(user.Email, "Delete");
            }
            else
            {
                UsersHelper.RemoveRoleToUser(user.Email, "Delete");
            }

            if (user.CanEdit)
            {
                UsersHelper.AddRoleToUser(user.Email, "Edit");
            }
            else
            {
                UsersHelper.RemoveRoleToUser(user.Email, "Edit");
            }

            if (user.CanView)
            {
                UsersHelper.AddRoleToUser(user.Email, "View");
            }
            else
            {
                UsersHelper.RemoveRoleToUser(user.Email, "View");
            }
        }

        // GET: Users/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = await db.Users.FindAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            User user = await db.Users.FindAsync(id);
            db.Users.Remove(user);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
