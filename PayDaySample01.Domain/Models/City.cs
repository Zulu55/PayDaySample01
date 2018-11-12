using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayDaySample01.Domain.Models
{
    public class City
    {
        [Key]
        public int CityId { get; set; }

        [Display(Name = "País")]
        public int? CountryId { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [Display(Name = "Ciudad")]
        [StringLength(50, ErrorMessage = "El campo {0} no puede tener más de {1} carácteres.")]
        public string Name { get; set; }

        public virtual ICollection<Employee> Employees { get; set; }

        public virtual Country Country { get; set; }
    }
}
