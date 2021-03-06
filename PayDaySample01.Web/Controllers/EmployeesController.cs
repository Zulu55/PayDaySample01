﻿namespace PayDaySample01.Web.Controllers
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
    using System.Data.Entity.Validation;
    using System.Collections;
    using PagedList;

    public class EmployeesController : Controller
    {
        private LocalDataContext db = new LocalDataContext();

        public async Task<ActionResult> DeleteDependent(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var dependent = await db.Dependents.FindAsync(id);

            if (dependent == null)
            {
                return HttpNotFound();
            }

            db.Dependents.Remove(dependent);
            await db.SaveChangesAsync();
            return RedirectToAction($"Details/{dependent.EmployeeId}");
        }

        public async Task<ActionResult> EditDependent(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var dependent = await db.Dependents.FindAsync(id);

            if (dependent == null)
            {
                return HttpNotFound();
            }

            ViewBag.RelationId = new SelectList(await this.GetRelations(), "RelationId", "Name", dependent.RelationId);
            return View(dependent);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditDependent(Dependent dependent)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dependent).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction($"Details/{dependent.EmployeeId}");
            }

            ViewBag.RelationId = new SelectList(await this.GetRelations(), "RelationId", "Name", dependent.RelationId);
            return View(dependent);
        }

        public async Task<ActionResult> AddDependent(int? id)
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

            ViewBag.RelationId = new SelectList(await this.GetRelations(), "RelationId", "Name");
            var view = new Dependent
            {
                EmployeeId = employee.EmployeeId,
            };

            return View(view);
        }

        private async Task<List<Relation>> GetRelations()
        {
            var relations = await db.Relations.OrderBy(r => r.Name).ToListAsync();
            relations.Insert(0, new Relation
            {
                RelationId = 0,
                Name = "[Seleccione una relación...]",
            });

            return relations;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddDependent(Dependent dependent)
        {
            if (ModelState.IsValid)
            {
                db.Dependents.Add(dependent);
                await db.SaveChangesAsync();
                return RedirectToAction($"Details/{dependent.EmployeeId}");
            }

            ViewBag.RelationId = new SelectList(await this.GetRelations(), "RelationId", "Name", dependent.RelationId);
            return View(dependent);
        }

        // GET: Employees
        [Authorize(Roles = "View, Admin")]
        public ActionResult Index(int? page = null)
        {
            page = (page ?? 1);
            var employees = db.Employees.
                OrderBy(e => e.FirstName).
                ThenBy(e => e.LastName).
                Include(e => e.City);
            return View(employees.ToPagedList((int)page, 4));
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
            ViewBag.CountryId = new SelectList(
                db.Countries.OrderBy(c => c.Name), 
                "CountryId", 
                "Name");
            ViewBag.CityId = new SelectList(
                db.Cities.
                    Where(c => c.CountryId == db.Countries.OrderBy(co => co.Name).FirstOrDefault().CountryId).
                    OrderBy(c => c.Name), 
                "CityId", 
                "Name");
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
                    pic = FilesHelper.UploadFile(view.PictureFile, folder);
                    pic = $"{folder}/{pic}";
                }

                var employee = this.ToEmployee(view, pic);
                db.Employees.Add(employee);

                UsersHelper.CreateUserASP(view.Email, "Employee", view.Document);
                var user = this.ToUser(employee);
                db.Users.Add(user);

                try
                {
                    await db.SaveChangesAsync();
                }
                catch (DbEntityValidationException e)
                {
                    var errors = string.Empty;
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        errors += $"Entity of type \"{eve.Entry.Entity.GetType().Name}\" in " +
                            $"state \"{eve.Entry.State}\" has the following validation errors:";
                        foreach (var ve in eve.ValidationErrors)
                        {
                            errors += $"- Property: \"{ve.PropertyName}\", Error: \"{ve.ErrorMessage}\"";
                        }
                    }
                }

                return RedirectToAction("Index");
            }

            ViewBag.CityId = new SelectList(db.Cities.OrderBy(c => c.Name), "CityId", "Name", view.CityId);
            return View(view);
        }

        private User ToUser(Employee employee)
        {
            return new User
            {
                Confirm = employee.Document,
                Email = employee.Email,
                FirstName = employee.FirstName,
                IsEmployee = true,
                LastName = employee.LastName,
                Password = employee.Document,
            };
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
                Email = view.Email,
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
                Email = employee.Email,
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
                    pic = FilesHelper.UploadFile(view.PictureFile, folder);
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
            try
            {
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("DELETE"))
                {
                    ModelState.AddModelError(string.Empty, "No se puede borrar empleado, porque tiene registros relacionados.");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            return RedirectToAction("Index");
        }

        public JsonResult GetCities(int countryId)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var cities = db.Cities.Where(c => c.CountryId == countryId);
            return Json(cities);
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
