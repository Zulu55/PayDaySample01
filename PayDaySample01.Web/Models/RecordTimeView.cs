using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PayDaySample01.Web.Models
{
    public class RecordTimeView
    {
        [Display(Name = "Fecha inicio")]
        [DataType(DataType.Date)]
        public DateTime DateStart { get; set; }

        [Display(Name = "Hora inicio")]
        [DataType(DataType.Time)]
        public DateTime TimeStart { get; set; }

        [Display(Name = "Fecha fin")]
        [DataType(DataType.Date)]
        public DateTime DateEnd { get; set; }

        [Display(Name = "Hora fin")]
        [DataType(DataType.Time)]
        public DateTime TimeEnd { get; set; }
    }
}