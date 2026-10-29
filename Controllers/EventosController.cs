using Eventos.Web.Models;
using Eventos.Web.Repositorios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Eventos.Web.Controllers;

public class EventosController : Controller
{
    private static readonly string[] Modalidades = { "Presencial", "Virtual" };

    private readonly EventoRepositorio _eventos;

    public EventosController(EventoRepositorio eventos)
    {
        _eventos = eventos;
    }

    public async Task<IActionResult> Index(int? categoriaId)
    {
        var categorias = await _eventos.ObtenerCategoriasActivasAsync();

        var modelo = new EventoIndexViewModel
        {
            Eventos = await _eventos.ObtenerActivosAsync(categoriaId),
            Categorias = new SelectList(categorias, "Id", "Nombre", categoriaId),
            CategoriaId = categoriaId
        };
        return View(modelo);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var evento = await _eventos.ObtenerResumenAsync(id.Value);
        if (evento is null)
        {
            return NotFound();
        }
        return View(evento);
    }

    public async Task<IActionResult> Create()
    {
        await CargarListasAsync();
        return View(new Evento { Fecha = DateTime.Today.AddDays(1).AddHours(9) });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("Titulo,Fecha,Lugar,Aforo,Precio,CategoriaId,Modalidad")] Evento evento)
    {
        if (ModelState.IsValid && !await _eventos.CategoriaActivaExisteAsync(evento.CategoriaId))
        {
            ModelState.AddModelError(nameof(Evento.CategoriaId), "La categoría seleccionada no es válida.");
        }

        if (!ModelState.IsValid)
        {
            await CargarListasAsync(evento.CategoriaId, evento.Modalidad);
            return View(evento);
        }

        await _eventos.CrearAsync(evento);
        TempData["Mensaje"] = $"El evento \"{evento.Titulo}\" se registró correctamente.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var evento = await _eventos.ObtenerParaEditarAsync(id.Value);
        if (evento is null)
        {
            return NotFound();
        }

        await CargarListasAsync(evento.CategoriaId, evento.Modalidad);
        return View(evento);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id, [Bind("Id,Titulo,Fecha,Lugar,Aforo,Precio,CategoriaId,Modalidad")] Evento evento)
    {
        if (id != evento.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            if (!await _eventos.CategoriaActivaExisteAsync(evento.CategoriaId))
            {
                ModelState.AddModelError(nameof(Evento.CategoriaId), "La categoría seleccionada no es válida.");
            }

            var confirmados = await _eventos.ContarConfirmadosAsync(evento.Id);
            if (evento.Aforo < confirmados)
            {
                ModelState.AddModelError(nameof(Evento.Aforo),
                    $"El aforo no puede ser menor que los inscritos confirmados ({confirmados}).");
            }
        }

        if (!ModelState.IsValid)
        {
            await CargarListasAsync(evento.CategoriaId, evento.Modalidad);
            return View(evento);
        }

        if (!await _eventos.ActualizarAsync(evento))
        {
            return NotFound();
        }

        TempData["Mensaje"] = $"El evento \"{evento.Titulo}\" se actualizó correctamente.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var evento = await _eventos.ObtenerResumenAsync(id.Value);
        if (evento is null)
        {
            return NotFound();
        }
        return View(evento);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (await _eventos.EliminarAsync(id))
        {
            TempData["Mensaje"] = "El evento se eliminó correctamente.";
        }
        else
        {
            TempData["Error"] = "No se encontró el evento a eliminar.";
        }
        return RedirectToAction(nameof(Index));
    }

    private async Task CargarListasAsync(int? categoriaId = null, string? modalidad = null)
    {
        var categorias = await _eventos.ObtenerCategoriasActivasAsync();
        ViewBag.Categorias = new SelectList(categorias, "Id", "Nombre", categoriaId);
        ViewBag.Modalidades = new SelectList(Modalidades, modalidad);
    }
}
