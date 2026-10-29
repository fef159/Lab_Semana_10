using Eventos.Web.Models;
using Eventos.Web.Repositorios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Eventos.Web.Controllers;

public class InscripcionesController : Controller
{
    private readonly InscripcionRepositorio _inscripciones;

    public InscripcionesController(InscripcionRepositorio inscripciones)
    {
        _inscripciones = inscripciones;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _inscripciones.ObtenerTodasAsync());
    }

    public async Task<IActionResult> Create()
    {
        await CargarListasAsync();
        return View(new Inscripcion());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("EventoId,AsistenteId")] Inscripcion inscripcion)
    {
        if (ModelState.IsValid)
        {
            var evento = await _inscripciones.ObtenerEventoActivoAsync(inscripcion.EventoId);
            if (evento is null)
            {
                ModelState.AddModelError(nameof(Inscripcion.EventoId), "El evento seleccionado no existe.");
            }
            else
            {
                if (evento.Fecha < DateTime.Now)
                {
                    ModelState.AddModelError(nameof(Inscripcion.EventoId), "El evento ya pasó, no se admiten inscripciones.");
                }

                if (await _inscripciones.ContarConfirmadasAsync(evento.Id) >= evento.Aforo)
                {
                    ModelState.AddModelError(nameof(Inscripcion.EventoId), "El aforo del evento está completo.");
                }
            }

            if (!await _inscripciones.AsistenteActivoExisteAsync(inscripcion.AsistenteId))
            {
                ModelState.AddModelError(nameof(Inscripcion.AsistenteId), "El asistente seleccionado no existe.");
            }
            else if (evento is not null && await _inscripciones.YaInscritoAsync(inscripcion.EventoId, inscripcion.AsistenteId))
            {
                ModelState.AddModelError(nameof(Inscripcion.AsistenteId), "El asistente ya está inscrito en este evento.");
            }
        }

        if (!ModelState.IsValid)
        {
            await CargarListasAsync(inscripcion.EventoId, inscripcion.AsistenteId);
            return View(inscripcion);
        }

        await _inscripciones.CrearAsync(inscripcion);
        TempData["Mensaje"] = "La inscripción se registró correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancelar(int id)
    {
        if (await _inscripciones.CancelarAsync(id))
        {
            TempData["Mensaje"] = "La inscripción fue cancelada.";
        }
        else
        {
            TempData["Error"] = "No se encontró una inscripción confirmada para cancelar.";
        }
        return RedirectToAction(nameof(Index));
    }

    private async Task CargarListasAsync(int? eventoId = null, int? asistenteId = null)
    {
        var eventos = (await _inscripciones.ObtenerEventosActivosAsync())
            .Select(e => new { e.Id, Texto = $"{e.Titulo} ({e.Fecha:dd/MM/yyyy HH:mm})" });
        var asistentes = (await _inscripciones.ObtenerAsistentesActivosAsync())
            .Select(a => new { a.Id, Texto = $"{a.Nombres} - DNI {a.Dni}" });

        ViewBag.Eventos = new SelectList(eventos, "Id", "Texto", eventoId);
        ViewBag.Asistentes = new SelectList(asistentes, "Id", "Texto", asistenteId);
    }
}
