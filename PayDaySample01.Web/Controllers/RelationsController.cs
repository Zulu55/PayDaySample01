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
    [Authorize(Roles = "Admin")]
    public class RelationsController : Controller
    {
        private LocalDataContext db = new LocalDataContext();

        // GET: Relations
        public async Task<ActionResult> Index()
        {
            return View(await db.Relations.OrderBy(r => r.Name).ToListAsync());
        }

        // GET: Relations/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Relation relation = await db.Relations.FindAsync(id);
            if (relation == null)
            {
                return HttpNotFound();
            }
            return View(relation);
        }

        // GET: Relations/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Relations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "RelationId,Name")] Relation relation)
        {
            if (ModelState.IsValid)
            {
                db.Relations.Add(relation);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(relation);
        }

        // GET: Relations/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Relation relation = await db.Relations.FindAsync(id);
            if (relation == null)
            {
                return HttpNotFound();
            }
            return View(relation);
        }

        // POST: Relations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "RelationId,Name")] Relation relation)
        {
            if (ModelState.IsValid)
            {
                db.Entry(relation).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(relation);
        }

        // GET: Relations/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Relation relation = await db.Relations.FindAsync(id);
            if (relation == null)
            {
                return HttpNotFound();
            }
            return View(relation);
        }

        // POST: Relations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Relation relation = await db.Relations.FindAsync(id);
            db.Relations.Remove(relation);
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
