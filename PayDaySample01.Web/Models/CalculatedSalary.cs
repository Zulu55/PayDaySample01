using PayDaySample01.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PayDaySample01.Web.Models
{
    public class CalculatedSalary
    {
        public Employee Employee { get; set; }

        public TimeSpan WorkedTime { get; set; }

        public decimal TotalToPay { get; set; }
    }
}