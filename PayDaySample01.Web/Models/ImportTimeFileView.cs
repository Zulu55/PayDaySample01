using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PayDaySample01.Web.Models
{
    public class ImportTimeFileView
    {
        [Display(Name = "Archivo plano")]
        public HttpPostedFileBase TimeFile { get; set; }
    }
}