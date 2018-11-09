namespace PayDaySample01.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Net;
    using System.Web;
    using System.Web.Mvc;
    using Domain.Models;
    using Models;
    using Helpers;

    public class EmployeesController : Controller
    {
        private LocalDataContext db = new LocalDataContext();

        // GET: Employees
        [Authorize(Roles = "View, Admin")]
        public async Task<ActionResult> Index()
        {
            var employees = db.Employees.Include(e => e.City);
            return View(await employees.ToListAsync());
        }

        // GET: Employees/Details/5
        [Authorize(Roles = "View, Admin")]
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var employee = await db.Employees.FindAsync(id);

            if (employee == null)
            {
                return HttpNotFound();
            }

            return View(employee);
        }

        // GET: Employees/Create
        [Authorize(Roles = "Create, Admin")]
        public ActionResult Create()
        {
            ViewBag.CityId = new SelectList(db.Cities.OrderBy(c => c.Name), "CityId", "Name");
            return View();
        }

        // POST: Employees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(EmployeeView view)
        {
            if (ModelState.IsValid)
            {
                var pic = string.Empty;
                var folder = "~/Content/Photos";

                if (view.PictureFile != null)
                {
                    pic = FilesHelper.UploadPhoto(view.PictureFile, folder);
                    pic = $"{folder}/{pic}";
                }

                var employee = this.ToEmployee(view, pic);
                db.Employees.Add(employee);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.CityId = new SelectList(db.Cities.OrderBy(c => c.Name), "CityId", "Name", view.CityId);
            return View(view);
        }

        private Employee ToEmployee(EmployeeView view, string pic)
        {
            return new Employee
            {
                CityId = view.CityId,
                Document = view.Document,
                EmployeeId = view.EmployeeId,
                FirstName = view.FirstName,
                HasChildren = view.HasChildren,
                HireIn = view.HireIn,
                LastName = view.LastName,
                PicturePath = pic,
                Salary = view.Salary,
            };
        }

        // GET: Employees/Edit/5
        [Authorize(Roles = "Edit, Admin")]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var employee = await db.Employees.FindAsync(id);

            if (employee == null)
            {
                return HttpNotFound();
            }

            ViewBag.CityId = new SelectList(db.Cities.OrderBy(c => c.Name), "CityId", "Name", employee.CityId);
            var view = this.ToView(employee);
            return View(view);
        }

        private EmployeeView ToView(Employee employee)
        {
            return new EmployeeView
            {
                CityId = employee.CityId,
                Document = employee.Document,
                EmployeeId = employee.EmployeeId,
                FirstName = employee.FirstName,
                HasChildren = employee.HasChildren,
                HireIn = employee.HireIn,
                LastName = employee.LastName,
                PicturePath = employee.PicturePath,
                Salary = employee.Salary,
            };
        }

        // POST: Employees/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EmployeeView view)
        {
            if (ModelState.IsValid)
            {
                var pic = view.PicturePath;
                var folder = "~/Content/Photos";

                if (view.PictureFile != null)
                {
                    pic = FilesHelper.UploadPhoto(view.PictureFile, folder);
                    pic = $"{folder}/{pic}";
                }

                var employee = this.ToEmployee(view, pic);
                db.Entry(employee).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.CityId = new SelectList(db.Cities.OrderBy(c => c.Name), "CityId", "Name", view.CityId);
            return View(view);
        }

        // GET: Employees/Delete/5
        [Authorize(Roles = "Delete, Admin")]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var employee = await db.Employees.FindAsync(id);

            if (employee == null)
            {
                return HttpNotFound();
            }

            db.Employees.Remove(employee);
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
