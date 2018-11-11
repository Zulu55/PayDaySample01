using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayDaySample01.Domain.Models
{
    public class Employee
    {
        [Key]
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [Display(Name = "Cédula")]
        [StringLength(20, ErrorMessage = "El campo {0} no puede tener más de {1} carácteres.")]
        public string Document { get; set; }

        [Display(Name = "Ciudad")]
        public int CityId { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [Display(Name = "Nombres")]
        [StringLength(50, ErrorMessage = "El campo {0} no puede tener más de {1} carácteres.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [Display(Name = "Apellidos")]
        [StringLength(50, ErrorMessage = "El campo {0} no puede tener más de {1} carácteres.")]
        public string LastName { get; set; }

        [Display(Name = "Foto")]
        public string PicturePath { get; set; }

        [Display(Name = "Fecha contratación")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        public DateTime HireIn { get; set; }

        [Display(Name = "Salario")]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public decimal Salary { get; set; }

        [Display(Name = "¿Tiene hijos?")]
        public bool HasChildren { get; set; }

        public virtual City City { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [Display(Name = "Correo")]
        [StringLength(100, ErrorMessage = "El campo {0} no puede tener más de {1} carácteres.")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        public virtual ICollection<RecordTime> RecordTimes { get; set; }

        public virtual ICollection<CalculatedSalary> CalculatedSalaries { get; set; }

        public virtual ICollection<Dependent> Dependents { get; set; }

        [Display(Name = "Nombres y Apellidos")]
        public string FullName { get { return $"{this.FirstName} {this.LastName}"; } }
    }
}
