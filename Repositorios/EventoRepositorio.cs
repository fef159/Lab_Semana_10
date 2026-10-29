using Eventos.Web.Data;
using Eventos.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace Eventos.Web.Repositorios;

public class EventoRepositorio
{
    private readonly EventosContext _context;

    public EventoRepositorio(EventosContext context)
    {
        _context = context;
    }

    public async Task<List<EventoResumen>> ObtenerActivosAsync(int? categoriaId)
    {
        var consulta = _context.Eventos.AsNoTracking().Where(e => e.Activo);

        if (categoriaId.HasValue)
        {
            consulta = consulta.Where(e => e.CategoriaId == categoriaId.Value);
        }

        return await consulta
            .OrderBy(e => e.Fecha)
            .Select(e => new EventoResumen
            {
                Id = e.Id,
                Titulo = e.Titulo,
                Fecha = e.Fecha,
                Lugar = e.Lugar,
                Modalidad = e.Modalidad,
                Aforo = e.Aforo,
                Precio = e.Precio,
                Categoria = e.Categoria!.Nombre,
                Inscritos = e.Inscripciones!.Count(i => i.Estado == Inscripcion.Confirmada)
            })
            .ToListAsync();
    }

    public async Task<EventoResumen?> ObtenerResumenAsync(int id)
    {
        return await _context.Eventos
            .AsNoTracking()
            .Where(e => e.Id == id && e.Activo)
            .Select(e => new EventoResumen
            {
                Id = e.Id,
                Titulo = e.Titulo,
                Fecha = e.Fecha,
                Lugar = e.Lugar,
                Modalidad = e.Modalidad,
                Aforo = e.Aforo,
                Precio = e.Precio,
                Categoria = e.Categoria!.Nombre,
                Inscritos = e.Inscripciones!.Count(i => i.Estado == Inscripcion.Confirmada)
            })
            .FirstOrDefaultAsync();
    }

    public async Task<Evento?> ObtenerParaEditarAsync(int id)
    {
        return await _context.Eventos
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id && e.Activo);
    }

    public async Task<List<Categoria>> ObtenerCategoriasActivasAsync()
    {
        return await _context.Categorias
            .AsNoTracking()
            .Where(c => c.Activo)
            .OrderBy(c => c.Nombre)
            .ToListAsync();
    }

    public async Task<bool> CategoriaActivaExisteAsync(int categoriaId)
    {
        return await _context.Categorias.AnyAsync(c => c.Id == categoriaId && c.Activo);
    }

    public async Task<int> ContarConfirmadosAsync(int eventoId)
    {
        return await _context.Inscripciones
            .CountAsync(i => i.EventoId == eventoId && i.Estado == Inscripcion.Confirmada);
    }

    public async Task CrearAsync(Evento evento)
    {
        evento.Activo = true;
        _context.Eventos.Add(evento);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ActualizarAsync(Evento datos)
    {
        var evento = await _context.Eventos.FirstOrDefaultAsync(e => e.Id == datos.Id && e.Activo);
        if (evento is null)
        {
            return false;
        }

        evento.Titulo = datos.Titulo;
        evento.Fecha = datos.Fecha;
        evento.Lugar = datos.Lugar;
        evento.Aforo = datos.Aforo;
        evento.Precio = datos.Precio;
        evento.CategoriaId = datos.CategoriaId;
        evento.Modalidad = datos.Modalidad;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var evento = await _context.Eventos.FirstOrDefaultAsync(e => e.Id == id && e.Activo);
        if (evento is null)
        {
            return false;
        }

        evento.Activo = false;
        await _context.SaveChangesAsync();
        return true;
    }
}
