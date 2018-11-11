using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayDaySample01.Domain.Models
{
    public class Country
    {
        [Key]
        public int CountryId { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [Display(Name = "País")]
        [StringLength(50, ErrorMessage = "El campo {0} no puede tener más de {1} carácteres.")]
        public string Name { get; set; }

        public virtual ICollection<City> Cities { get; set; }
    }
}
