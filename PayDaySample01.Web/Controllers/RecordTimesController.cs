namespace PayDaySample01.Web.Controllers
{
    using PayDaySample01.Domain.Models;
    using PayDaySample01.Web.Helpers;
    using PayDaySample01.Web.Models;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Entity;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;
    using PagedList;

    /// <summary>
    /// Record times controller
    /// </summary>
    public class RecordTimesController : Controller
    {
        /// <summary>
        /// The connectio DB
        /// </summary>
        private LocalDataContext db = new LocalDataContext();

        [Authorize(Roles = "Admin")]
        public ActionResult Import()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Import(ImportTimeFileView view)
        {
            if (view.TimeFile == null)
            {
                ModelState.AddModelError(string.Empty, "Se debe seleccionar un archivo.");
                return View();
            }

            var file = this.GetFile(view.TimeFile);
            var response = await this.ProcessFile(file);
            if (!response.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, response.Message);
                return View();
            }

            if (!string.IsNullOrEmpty(response.Message))
            {
                ModelState.AddModelError(string.Empty, $"Se importó el archivo, " +
                    $"pero hubo problema en los siguientes registros: {response.Message}");
                return View();
            }

            return RedirectToAction("Index2");
        }

        private async Task<Response> ProcessFile(string file)
        {
            var path = System.Web.HttpContext.Current.Server.MapPath(file);
            var lines = System.IO.File.ReadAllLines(path);
            var log = string.Empty;

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    int lineNumber = 0;
                    foreach (var line in lines)
                    {
                        try
                        {
                            var fields = line.Split(',');
                            var recordTime = new RecordTime
                            {
                                EmployeeId = int.Parse(fields[0]),
                                DateStart = DateTime.Parse(fields[1]),
                                DateEnd = DateTime.Parse(fields[2]),
                            };

                            var oldRecord = await db.RecordTimes.
                                Where(r => r.EmployeeId == recordTime.EmployeeId &&
                                           r.DateStart == recordTime.DateStart &&
                                           r.DateEnd == recordTime.DateEnd).
                                FirstOrDefaultAsync();

                            if (oldRecord == null)
                            {
                                db.RecordTimes.Add(recordTime);
                            }

                        }
                        catch (Exception ex)
                        {
                            log += $"EROR Línea: {lineNumber}, {ex.Message}\n";
                        }

                        lineNumber++;
                    }

                    await db.SaveChangesAsync();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ex.ToString();
                    return new Response
                    {
                        IsSuccess = false,
                        Message = ex.ToString(),
                    };
                } 
            }

            return new Response
            {
                IsSuccess = true,
                Message = log,
            };
        }

        /// <summary>
        /// Get the file
        /// </summary>
        /// <param name="timeFile">The objct that represents the file</param>
        /// <returns>The path of upload file</returns>
        private string GetFile(HttpPostedFileBase timeFile)
        {
            var file = string.Empty;
            var folder = "~/Content/Files";

            if (timeFile != null)
            {
                file = FilesHelper.UploadFile(timeFile, folder);
                file = $"{folder}/{file}";
            }

            return file;
        }

        public async Task<ActionResult> ShowResults()
        {
            var x = System.Web.HttpContext.Current.Session["calculatedSalaries"];

            var calculatedSalaries = db.CalculatedSalaries.Include(c => c.Employee);
            return View(await calculatedSalaries.ToListAsync());
        }

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
                                EmployeeId = previousEmployee,
                                Employee = employee,
                                TotalToPay = this.CalculateSalary(employee, totalTime),
                                WorkedTime = totalTime,
                                WorkedHours = totalTime.TotalHours,
                            });

                        }

                        System.Web.HttpContext.Current.Session["calculatedSalaries"] = calculatedSalaries;

                        db.CalculatedSalaries.RemoveRange(db.CalculatedSalaries.ToList());
                        db.CalculatedSalaries.AddRange(calculatedSalaries);
                        await db.SaveChangesAsync();

                        return RedirectToAction("ShowResults");
                    }

                    ModelState.AddModelError(string.Empty, "No hay registros para liquidar la nómina.");
                    return View(view);
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

        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Index2(RecordTimeFilterView view, int? page = null)
        {
            page = (page ?? 1);

            if (view.EmployeeId == 0 && view.DateStart == DateTime.MinValue && view.DateEnd == DateTime.MinValue)
            {
                var recordTimes = db.RecordTimes.
                    Include(r => r.Employee).
                    OrderBy(r => r.Employee.EmployeeId).
                    ThenBy(r => r.DateStart);
                view.RecordTimes = recordTimes.ToPagedList((int)page, 10);
            }
            else if (view.EmployeeId != 0 && view.DateStart == DateTime.MinValue && view.DateEnd == DateTime.MinValue)
            {
                var recordTimes = db.RecordTimes.
                    Include(r => r.Employee).
                    Where(r => r.EmployeeId == view.EmployeeId).
                    OrderBy(r => r.Employee.EmployeeId).
                    ThenBy(r => r.DateStart);
                view.RecordTimes = recordTimes.ToPagedList((int)page, 10);
            }
            else if (view.EmployeeId == 0 && view.DateStart != DateTime.MinValue && view.DateEnd == DateTime.MinValue)
            {
                var recordTimes = db.RecordTimes.
                    Include(r => r.Employee).
                    Where(r => r.DateStart >= view.DateStart).
                    OrderBy(r => r.Employee.EmployeeId).
                    ThenBy(r => r.DateStart);
                view.RecordTimes = recordTimes.ToPagedList((int)page, 10);
            }
            else if (view.EmployeeId == 0 && view.DateStart == DateTime.MinValue && view.DateEnd != DateTime.MinValue)
            {
                var recordTimes = db.RecordTimes.
                    Include(r => r.Employee).
                    Where(r => r.DateStart <= view.DateEnd).
                    OrderBy(r => r.Employee.EmployeeId).
                    ThenBy(r => r.DateStart);
                view.RecordTimes = recordTimes.ToPagedList((int)page, 10);
            }
            else if (view.EmployeeId == 0 && view.DateStart != DateTime.MinValue && view.DateEnd != DateTime.MinValue)
            {
                var recordTimes = db.RecordTimes.
                    Include(r => r.Employee).
                    Where(r => r.DateStart >= view.DateStart && r.DateStart <= view.DateEnd).
                    OrderBy(r => r.Employee.EmployeeId).
                    ThenBy(r => r.DateStart);
                view.RecordTimes = recordTimes.ToPagedList((int)page, 10);
            }
            else if (view.EmployeeId != 0 && view.DateStart != DateTime.MinValue && view.DateEnd != DateTime.MinValue)
            {
                var recordTimes = db.RecordTimes.
                    Include(r => r.Employee).
                    Where(r => r.EmployeeId == view.EmployeeId && r.DateStart >= view.DateStart && r.DateStart <= view.DateEnd).
                    OrderBy(r => r.Employee.EmployeeId).
                    ThenBy(r => r.DateStart);
                view.RecordTimes = recordTimes.ToPagedList((int)page, 10);
            }
            else if (view.EmployeeId != 0 && view.DateStart != DateTime.MinValue && view.DateEnd == DateTime.MinValue)
            {
                var recordTimes = db.RecordTimes.
                    Include(r => r.Employee).
                    Where(r => r.EmployeeId == view.EmployeeId && r.DateStart >= view.DateStart).
                    OrderBy(r => r.Employee.EmployeeId).
                    ThenBy(r => r.DateStart);
                view.RecordTimes = recordTimes.ToPagedList((int)page, 10);
            }
            else if (view.EmployeeId != 0 && view.DateStart == DateTime.MinValue && view.DateEnd != DateTime.MinValue)
            {
                var recordTimes = db.RecordTimes.
                    Include(r => r.Employee).
                    Where(r => r.EmployeeId == view.EmployeeId && r.DateStart <= view.DateEnd).
                    OrderBy(r => r.Employee.EmployeeId).
                    ThenBy(r => r.DateStart);
                view.RecordTimes = recordTimes.ToPagedList((int)page, 10);
            }

            var list = await db.Employees.OrderBy(e => e.FirstName).ThenBy(e => e.LastName).ToListAsync();
            list.Insert(0, new Employee
            {
                EmployeeId = 0,
                FirstName = "[Todos los empleados...]",
            });

            ViewBag.EmployeeId = new SelectList(list, "EmployeeId", "FullName");
            return View(view);
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
