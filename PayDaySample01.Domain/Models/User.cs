using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayDaySample01.Domain.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [Display(Name = "Nombres")]
        [StringLength(50, ErrorMessage = "El campo {0} no puede tener más de {1} carácteres.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [Display(Name = "Apellidos")]
        [StringLength(50, ErrorMessage = "El campo {0} no puede tener más de {1} carácteres.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [Display(Name = "Correo")]
        [StringLength(100, ErrorMessage = "El campo {0} no puede tener más de {1} carácteres.")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Display(Name = "Contraseña")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [DataType(DataType.Password)]
        [StringLength(20, ErrorMessage = "El campo {0} debe tener entre {2} y {1} carácteres.", MinimumLength = 6)]
        [NotMapped]
        public string Password { get; set; }

        [NotMapped]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [Display(Name = "Confirmación")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "La contraseña y la confirmación no son iguales.")]
        public string Confirm { get; set; }

        [Display(Name = "¿Es Administrador?")]
        public bool IsAdmin { get; set; }

        [Display(Name = "¿Es Empleado?")]
        public bool IsEmployee { get; set; }

        [Display(Name = "¿Puede ver?")]
        public bool CanView { get; set; }

        [Display(Name = "¿Puede editar?")]
        public bool CanEdit { get; set; }

        [Display(Name = "¿Puede crear?")]
        public bool CanCreate { get; set; }

        [Display(Name = "¿Puede borrar?")]
        public bool CanDelete { get; set; }
    }
}
