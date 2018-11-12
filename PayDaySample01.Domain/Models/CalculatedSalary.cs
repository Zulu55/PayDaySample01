using Newtonsoft.Json;
using PayDaySample01.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PayDaySample01.Domain.Models
{
    public class CalculatedSalary
    {
        [Key]
        public int CalculatedSalaryId { get; set; }

        public int EmployeeId { get; set; }

        [JsonIgnore]
        public virtual Employee Employee { get; set; }

        [Display(Name = "Horas Trabajadas")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double WorkedHours { get; set; }

        [NotMapped]
        public TimeSpan WorkedTime { get; set; }

        [Display(Name = "Total a Pagar")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal TotalToPay { get; set; }
    }
}