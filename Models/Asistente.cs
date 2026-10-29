using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;

namespace Eventos.Web.Models;

[Index(nameof(Dni), IsUnique = true)]
[Index(nameof(Correo), IsUnique = true)]
public class Asistente
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Los nombres son obligatorios.")]
    [StringLength(100)]
    public string Nombres { get; set; } = string.Empty;

    [Required(ErrorMessage = "El DNI es obligatorio.")]
    [RegularExpression(@"^\d{8}$", ErrorMessage = "El DNI debe tener 8 dígitos.")]
    [StringLength(8)]
    public string Dni { get; set; } = string.Empty;

    [Required(ErrorMessage = "El correo es obligatorio.")]
    [EmailAddress(ErrorMessage = "Formato de correo no válido.")]
    [StringLength(150)]
    public string Correo { get; set; } = string.Empty;

    [StringLength(9, ErrorMessage = "El teléfono admite máximo 9 caracteres.")]
    public string? Telefono { get; set; }

    public bool Activo { get; set; } = true;

    [ValidateNever]
    public ICollection<Inscripcion>? Inscripciones { get; set; }
}
