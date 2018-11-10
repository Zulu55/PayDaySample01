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
    public class RecordTimesController : Controller
    {
        private LocalDataContext db = new LocalDataContext();

        [Authorize(Roles = "Admin")]
        public ActionResult CalculatePays()
        {
            var view = new CalculatePayView
            {
                DateStart = DateTime.Today,
                DateEnd = DateTime.Today,
            };

            return View(view);
        }

        [HttpPost]
        public async Task<ActionResult> CalculatePays(CalculatePayView view)
        {
            if (ModelState.IsValid)
            {
                if (view.DateStart <= view.DateEnd)
                {
                    var recordTimes = await db.RecordTimes.
                        Where(r => r.DateStart >= view.DateStart && r.DateStart <= view.DateEnd).
                        OrderBy(r => r.EmployeeId).
                        ToListAsync();

                    var calculatedSalaries = new List<CalculatedSalary>();

                    if (recordTimes.Count != 0)
                    {
                        int i = 0;
                        while (i < recordTimes.Count)
                        {
                            int previousEmployee = recordTimes[i].EmployeeId;
                            TimeSpan totalTime = new TimeSpan();
                            while (i < recordTimes.Count && previousEmployee == recordTimes[i].EmployeeId)
                            {
                                totalTime += recordTimes[i].Time;
                                i++;
                            }

                            var employee = await db.Employees.FindAsync(previousEmployee);

                            calculatedSalaries.Add(new CalculatedSalary
                            {
                                Employee = employee,
                                TotalToPay = this.CalculateSalary(employee, totalTime),
                                WorkedTime = totalTime,
                            });
                        }
                    }

                    ModelState.AddModelError(string.Empty, "No hay registros para liquidar la nómina.");
                }

                ModelState.AddModelError(string.Empty, "La fecha de fin debe ser mayor o igual a la fecha inicial.");
            }

            return View(view);
        }

        private decimal CalculateSalary(Employee employee, TimeSpan totalTime)
        {
            decimal valueForHour = employee.Salary / 180;
            double hours = totalTime.TotalHours;
            decimal salaryToPay = valueForHour * (decimal)hours;
            if (employee.HasChildren)
            {
                salaryToPay *= 1.1M;
            }
            return salaryToPay;
        }

        // GET: RecordTimes
        public async Task<ActionResult> Index()
        {
            var recordTimes = db.RecordTimes.
                Include(r => r.Employee).
                Where(r => r.Employee.Email.Equals(User.Identity.Name)).
                OrderByDescending(r => r.DateStart);
            return View(await recordTimes.ToListAsync());
        }

        // GET: RecordTimes/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RecordTime recordTime = await db.RecordTimes.FindAsync(id);
            if (recordTime == null)
            {
                return HttpNotFound();
            }
            return View(recordTime);
        }

        // GET: RecordTimes/Create
        public ActionResult Create()
        {
            var view = new RecordTimeView
            {
                DateStart = DateTime.Today,
                DateEnd = DateTime.Today,
            };
            return View();
        }

        // POST: RecordTimes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(RecordTimeView view)
        {
            if (ModelState.IsValid)
            {
                var employee = await db.Employees.Where(e => e.Email.Equals(User.Identity.Name)).FirstOrDefaultAsync();
                if (employee != null)
                {
                    var dateTimeStart = view.DateStart.AddHours(view.TimeStart.Hour).AddMinutes(view.TimeStart.Minute);
                    var dateTimeEnd = view.DateEnd.AddHours(view.TimeEnd.Hour).AddMinutes(view.TimeEnd.Minute);

                    if (dateTimeStart <= dateTimeEnd)
                    {
                        var recordTime = new RecordTime
                        {
                            EmployeeId = employee.EmployeeId,
                            DateStart = dateTimeStart,
                            DateEnd = dateTimeEnd,
                        };

                        db.RecordTimes.Add(recordTime);
                        await db.SaveChangesAsync();
                        return RedirectToAction("Index");
                    }

                    ModelState.AddModelError(string.Empty, "La fecha/hora de finalización no puede ser menor que la fecha/hora de inicio.");
                }

                ModelState.AddModelError(string.Empty, "No se ecuentra empleado.");
            }

            return View(view);
        }

        // GET: RecordTimes/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RecordTime recordTime = await db.RecordTimes.FindAsync(id);
            if (recordTime == null)
            {
                return HttpNotFound();
            }
            ViewBag.EmployeeId = new SelectList(db.Employees, "EmployeeId", "Document", recordTime.EmployeeId);
            return View(recordTime);
        }

        // POST: RecordTimes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "RecordTimeId,EmployeeId,DateStart,DateEnd")] RecordTime recordTime)
        {
            if (ModelState.IsValid)
            {
                db.Entry(recordTime).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.EmployeeId = new SelectList(db.Employees, "EmployeeId", "Document", recordTime.EmployeeId);
            return View(recordTime);
        }

        // GET: RecordTimes/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RecordTime recordTime = await db.RecordTimes.FindAsync(id);
            if (recordTime == null)
            {
                return HttpNotFound();
            }
            return View(recordTime);
        }

        // POST: RecordTimes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            RecordTime recordTime = await db.RecordTimes.FindAsync(id);
            db.RecordTimes.Remove(recordTime);
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
