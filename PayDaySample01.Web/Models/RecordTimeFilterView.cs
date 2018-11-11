using PagedList;
using PayDaySample01.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PayDaySample01.Web.Models
{
    public class RecordTimeFilterView
    {
        [Display(Name = "Empleado")]
        public int EmployeeId { get; set; }

        [Display(Name = "Fecha inical")]
        [DataType(DataType.Date)]
        public DateTime DateStart { get; set; }

        [Display(Name = "Fecha final")]
        [DataType(DataType.Date)]
        public DateTime DateEnd { get; set; }

        public IPagedList<RecordTime> RecordTimes { get; set; }
    }
}