using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Eventos.Web.Models;

public class Evento
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "El título es obligatorio.")]
    [StringLength(100)]
    public string Titulo { get; set; } = string.Empty;

    [Required(ErrorMessage = "La fecha es obligatoria.")]
    [DataType(DataType.DateTime)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-ddTHH:mm}", ApplyFormatInEditMode = true)]
    public DateTime Fecha { get; set; }

    [Required(ErrorMessage = "El lugar es obligatorio.")]
    [StringLength(100)]
    public string Lugar { get; set; } = string.Empty;

    [Range(1, 5000, ErrorMessage = "El aforo debe estar entre 1 y 5000.")]
    public int Aforo { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    [Range(0, 99999999.99, ErrorMessage = "El precio no puede ser negativo.")]
    public decimal Precio { get; set; }

    [Display(Name = "Categoría")]
    [Range(1, int.MaxValue, ErrorMessage = "Seleccione una categoría.")]
    public int CategoriaId { get; set; }

    [Required(ErrorMessage = "La modalidad es obligatoria.")]
    [StringLength(20)]
    [RegularExpression("^(Presencial|Virtual)$", ErrorMessage = "La modalidad debe ser Presencial o Virtual.")]
    public string Modalidad { get; set; } = "Presencial";

    public bool Activo { get; set; } = true;

    [ValidateNever]
    public Categoria? Categoria { get; set; }

    [ValidateNever]
    public ICollection<Inscripcion>? Inscripciones { get; set; }
}
