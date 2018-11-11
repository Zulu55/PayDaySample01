using PayDaySample01.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PayDaySample01.Web.Models
{
    public class EmployeeView : Employee
    {
        [Display(Name = "Foto")]
        public HttpPostedFileBase PictureFile { get; set; }

        public int CountryId { get; set; }
    }
}