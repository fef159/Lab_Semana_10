using Eventos.Web.Data;
using Eventos.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace Eventos.Web.Repositorios;

public class InscripcionRepositorio
{
    private readonly EventosContext _context;

    public InscripcionRepositorio(EventosContext context)
    {
        _context = context;
    }

    public async Task<List<Inscripcion>> ObtenerTodasAsync()
    {
        return await _context.Inscripciones
            .AsNoTracking()
            .Include(i => i.Evento)
            .Include(i => i.Asistente)
            .OrderByDescending(i => i.FechaInscripcion)
            .ToListAsync();
    }

    public async Task<List<Evento>> ObtenerEventosActivosAsync()
    {
        return await _context.Eventos
            .AsNoTracking()
            .Where(e => e.Activo)
            .OrderBy(e => e.Fecha)
            .ToListAsync();
    }

    public async Task<List<Asistente>> ObtenerAsistentesActivosAsync()
    {
        return await _context.Asistentes
            .AsNoTracking()
            .Where(a => a.Activo)
            .OrderBy(a => a.Nombres)
            .ToListAsync();
    }

    public async Task<Evento?> ObtenerEventoActivoAsync(int eventoId)
    {
        return await _context.Eventos
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == eventoId && e.Activo);
    }

    public async Task<bool> AsistenteActivoExisteAsync(int asistenteId)
    {
        return await _context.Asistentes.AnyAsync(a => a.Id == asistenteId && a.Activo);
    }

    public async Task<bool> YaInscritoAsync(int eventoId, int asistenteId)
    {
        return await _context.Inscripciones.AnyAsync(i =>
            i.EventoId == eventoId &&
            i.AsistenteId == asistenteId &&
            i.Estado == Inscripcion.Confirmada);
    }

    public async Task<int> ContarConfirmadasAsync(int eventoId)
    {
        return await _context.Inscripciones.CountAsync(i =>
            i.EventoId == eventoId &&
            i.Estado == Inscripcion.Confirmada);
    }

    public async Task CrearAsync(Inscripcion inscripcion)
    {
        inscripcion.Estado = Inscripcion.Confirmada;
        inscripcion.FechaInscripcion = DateTime.Now;
        _context.Inscripciones.Add(inscripcion);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> CancelarAsync(int id)
    {
        var inscripcion = await _context.Inscripciones
            .FirstOrDefaultAsync(i => i.Id == id && i.Estado == Inscripcion.Confirmada);
        if (inscripcion is null)
        {
            return false;
        }

        inscripcion.Estado = Inscripcion.Cancelada;
        await _context.SaveChangesAsync();
        return true;
    }
}
