using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PayDaySample01.Domain.Models;
using PayDaySample01.Web.Models;

namespace PayDaySample01.Web.Controllers
{
    public class DependentsController : Controller
    {
        private LocalDataContext db = new LocalDataContext();

        // GET: Dependents
        public async Task<ActionResult> Index()
        {
            var dependents = db.Dependents.Include(d => d.Employee).Include(d => d.Relation);
            return View(await dependents.ToListAsync());
        }

        // GET: Dependents/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Dependent dependent = await db.Dependents.FindAsync(id);
            if (dependent == null)
            {
                return HttpNotFound();
            }
            return View(dependent);
        }

        // GET: Dependents/Create
        public ActionResult Create()
        {
            ViewBag.EmployeeId = new SelectList(db.Employees, "EmployeeId", "Document");
            ViewBag.RelationId = new SelectList(db.Relations, "RelationId", "Name");
            return View();
        }

        // POST: Dependents/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "DependentId,Document,RelationId,EmployeeId,FirstName,LastName,Born,IsActive")] Dependent dependent)
        {
            if (ModelState.IsValid)
            {
                db.Dependents.Add(dependent);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.EmployeeId = new SelectList(db.Employees, "EmployeeId", "Document", dependent.EmployeeId);
            ViewBag.RelationId = new SelectList(db.Relations, "RelationId", "Name", dependent.RelationId);
            return View(dependent);
        }

        // GET: Dependents/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Dependent dependent = await db.Dependents.FindAsync(id);
            if (dependent == null)
            {
                return HttpNotFound();
            }
            ViewBag.EmployeeId = new SelectList(db.Employees, "EmployeeId", "Document", dependent.EmployeeId);
            ViewBag.RelationId = new SelectList(db.Relations, "RelationId", "Name", dependent.RelationId);
            return View(dependent);
        }

        // POST: Dependents/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "DependentId,Document,RelationId,EmployeeId,FirstName,LastName,Born,IsActive")] Dependent dependent)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dependent).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.EmployeeId = new SelectList(db.Employees, "EmployeeId", "Document", dependent.EmployeeId);
            ViewBag.RelationId = new SelectList(db.Relations, "RelationId", "Name", dependent.RelationId);
            return View(dependent);
        }

        // GET: Dependents/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Dependent dependent = await db.Dependents.FindAsync(id);
            if (dependent == null)
            {
                return HttpNotFound();
            }
            return View(dependent);
        }

        // POST: Dependents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Dependent dependent = await db.Dependents.FindAsync(id);
            db.Dependents.Remove(dependent);
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
