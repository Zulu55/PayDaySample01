using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayDaySample01.Domain.Models
{
    public class RecordTime
    {
        [Key]
        public int RecordTimeId { get; set; }

        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [Display(Name = "Fecha/Hora Inicio")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd hh:mm tt}")]
        public DateTime DateStart { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [Display(Name = "Fecha/Hora Fin")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd hh:mm tt}")]
        public DateTime DateEnd { get; set; }

        [Display(Name = "Tiempo")]
        //[DisplayFormat(DataFormatString = "{0:HH:mm}")]
        public TimeSpan Time
        {
            get
            {
                return this.DateEnd - this.DateStart;
            }
        }

        public virtual Employee Employee { get; set; }
    }
}
