using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Eventos.Web.Models;

public class EventoResumen
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
    public string Lugar { get; set; } = string.Empty;
    public string Modalidad { get; set; } = string.Empty;
    public int Aforo { get; set; }
    public decimal Precio { get; set; }
    public string Categoria { get; set; } = string.Empty;

    [Display(Name = "Inscritos")]
    public int Inscritos { get; set; }

    [Display(Name = "Cupos disponibles")]
    public int Cupos => Math.Max(Aforo - Inscritos, 0);
}

public class EventoIndexViewModel
{
    public List<EventoResumen> Eventos { get; set; } = new();
    public SelectList? Categorias { get; set; }
    public int? CategoriaId { get; set; }
}
