namespace PayDaySample01.Domain.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Relation
    {
        [Key]
        public int RelationId { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [Display(Name = "Relación")]
        [StringLength(50, ErrorMessage = "El campo {0} no puede tener más de {1} carácteres.")]
        public string Name { get; set; }

        public virtual ICollection<Dependent> Dependents { get; set; }
    }
}