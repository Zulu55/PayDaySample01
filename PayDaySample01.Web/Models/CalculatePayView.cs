using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PayDaySample01.Web.Models
{
    public class CalculatePayView
    {
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [Display(Name = "Fecha inicio")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime DateStart { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Fecha fin")]
        [DataType(DataType.Date)]
        public DateTime DateEnd { get; set; }
    }
}