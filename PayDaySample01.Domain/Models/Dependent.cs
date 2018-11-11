using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayDaySample01.Domain.Models
{
    public class Dependent
    {
        [Key]
        public int DependentId { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [Display(Name = "Documento")]
        [StringLength(20, ErrorMessage = "El campo {0} no puede tener más de {1} carácteres.")]
        public string Document { get; set; }

        [Display(Name = "Relación")]
        [Range(1, int.MaxValue, ErrorMessage = "Debes de seleccionar una relación.")]
        public int RelationId { get; set; }

        [Display(Name = "Empleado")]
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [Display(Name = "Nombres")]
        [StringLength(50, ErrorMessage = "El campo {0} no puede tener más de {1} carácteres.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [Display(Name = "Apellidos")]
        [StringLength(50, ErrorMessage = "El campo {0} no puede tener más de {1} carácteres.")]
        public string LastName { get; set; }

        [Display(Name = "Fecha nacimiento")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        public DateTime Born { get; set; }
        
        [Display(Name = "¿Está activo?")]
        public bool IsActive { get; set; }

        public virtual Relation Relation { get; set; }

        public virtual Employee Employee { get; set; }
    }
}
