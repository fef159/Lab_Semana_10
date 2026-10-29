using Eventos.Web.Models;
using Eventos.Web.Repositorios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Eventos.Web.Controllers;

public class AsistentesController : Controller
{
    private readonly AsistenteRepositorio _asistentes;

    public AsistentesController(AsistenteRepositorio asistentes)
    {
        _asistentes = asistentes;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _asistentes.ObtenerActivosAsync());
    }

    public IActionResult Create()
    {
        return View(new Asistente());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Nombres,Dni,Correo,Telefono")] Asistente asistente)
    {
        asistente.Correo = asistente.Correo?.Trim() ?? string.Empty;

        if (ModelState.IsValid)
        {
            if (await _asistentes.ExisteDniAsync(asistente.Dni))
            {
                ModelState.AddModelError(nameof(Asistente.Dni), "Ya existe un asistente con ese DNI.");
            }

            if (await _asistentes.ExisteCorreoAsync(asistente.Correo))
            {
                ModelState.AddModelError(nameof(Asistente.Correo), "Ya existe un asistente con ese correo.");
            }
        }

        if (!ModelState.IsValid)
        {
            return View(asistente);
        }

        try
        {
            await _asistentes.CrearAsync(asistente);
        }
        catch (DbUpdateException)
        {
            // Respaldo ante una inserción concurrente que viole los índices únicos.
            ModelState.AddModelError(string.Empty, "No se pudo registrar: el DNI o el correo ya existe.");
            return View(asistente);
        }

        TempData["Mensaje"] = $"El asistente \"{asistente.Nombres}\" se registró correctamente.";
        return RedirectToAction(nameof(Index));
    }
}
