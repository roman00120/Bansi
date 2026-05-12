using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WsApiExamen.Data;
using WsApiExamen.Models;

namespace WsApiExamen.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExamenController : ControllerBase
{
    private readonly ExamenDbContext _context;

    public ExamenController(ExamenDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Consultar()
    {
        try
        {
            var registros = await _context.Examenes
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .ToListAsync();

            return Ok(new
            {
                ok = true,
                mensaje = "Consulta realizada correctamente",
                data = registros
            });
        }
        catch
        {
            return StatusCode(500, new
            {
                ok = false,
                mensaje = "Error al consultar los registros"
            });
        }
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> ConsultarPorId(int id)
    {
        try
        {
            var registro = await _context.Examenes
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (registro is null)
            {
                return NotFound(new
                {
                    ok = false,
                    mensaje = "No se encontro el registro"
                });
            }

            return Ok(new
            {
                ok = true,
                mensaje = "Consulta realizada correctamente",
                data = registro
            });
        }
        catch
        {
            return StatusCode(500, new
            {
                ok = false,
                mensaje = "Error al consultar el registro"
            });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Agregar([FromBody] TblExamen model)
    {
        if (model.Id <= 0)
        {
            return BadRequest(new
            {
                ok = false,
                mensaje = "El Id es obligatorio"
            });
        }

        if (string.IsNullOrWhiteSpace(model.Nombre))
        {
            return BadRequest(new
            {
                ok = false,
                mensaje = "El nombre es obligatorio"
            });
        }

        if (string.IsNullOrWhiteSpace(model.Descripcion))
        {
            return BadRequest(new
            {
                ok = false,
                mensaje = "La descripcion es obligatoria"
            });
        }

        model.Nombre = model.Nombre.Trim();
        model.Descripcion = model.Descripcion.Trim();

        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var existe = await _context.Examenes.AnyAsync(x => x.Id == model.Id);

            if (existe)
            {
                await transaction.RollbackAsync();

                return Conflict(new
                {
                    ok = false,
                    mensaje = "Ya existe un registro con ese Id"
                });
            }

            _context.Examenes.Add(model);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return Ok(new
            {
                ok = true,
                mensaje = "Registro agregado correctamente",
                data = model
            });
        }
        catch
        {
            await transaction.RollbackAsync();

            return StatusCode(500, new
            {
                ok = false,
                mensaje = "Error al agregar el registro"
            });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Actualizar(int id, [FromBody] TblExamen model)
    {
        if (id <= 0 || model.Id <= 0)
        {
            return BadRequest(new
            {
                ok = false,
                mensaje = "El Id es obligatorio"
            });
        }

        if (id != model.Id)
        {
            return BadRequest(new
            {
                ok = false,
                mensaje = "El Id de la ruta no coincide con el del registro"
            });
        }

        if (string.IsNullOrWhiteSpace(model.Nombre))
        {
            return BadRequest(new
            {
                ok = false,
                mensaje = "El nombre es obligatorio"
            });
        }

        if (string.IsNullOrWhiteSpace(model.Descripcion))
        {
            return BadRequest(new
            {
                ok = false,
                mensaje = "La descripcion es obligatoria"
            });
        }

        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var registro = await _context.Examenes.FirstOrDefaultAsync(x => x.Id == id);

            if (registro is null)
            {
                await transaction.RollbackAsync();

                return NotFound(new
                {
                    ok = false,
                    mensaje = "No se encontro el registro"
                });
            }

            registro.Nombre = model.Nombre.Trim();
            registro.Descripcion = model.Descripcion.Trim();

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return Ok(new
            {
                ok = true,
                mensaje = "Registro actualizado correctamente",
                data = registro
            });
        }
        catch
        {
            await transaction.RollbackAsync();

            return StatusCode(500, new
            {
                ok = false,
                mensaje = "Error al actualizar el registro"
            });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        if (id <= 0)
        {
            return BadRequest(new
            {
                ok = false,
                mensaje = "El Id es obligatorio"
            });
        }

        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var registro = await _context.Examenes.FirstOrDefaultAsync(x => x.Id == id);

            if (registro is null)
            {
                await transaction.RollbackAsync();

                return NotFound(new
                {
                    ok = false,
                    mensaje = "No se encontro el registro"
                });
            }

            _context.Examenes.Remove(registro);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return Ok(new
            {
                ok = true,
                mensaje = "Registro eliminado correctamente"
            });
        }
        catch
        {
            await transaction.RollbackAsync();

            return StatusCode(500, new
            {
                ok = false,
                mensaje = "Error al eliminar el registro"
            });
        }
    }
}
