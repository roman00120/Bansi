using ApiExamen.Models;
using ApiExamen.Services;
using FrontExamen.Models;
using Microsoft.AspNetCore.Mvc;

namespace FrontExamen.Controllers;

public class ExamenController : Controller
{
    private readonly IConfiguration _configuration;

    public ExamenController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var model = CrearModeloBase(true);
        model.Resultados = await ConsultarLista(model.UseWebService);
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Index(ExamenViewModel model, string accion)
    {
        var servicio = CrearServicio(model.UseWebService);

        try
        {
            switch (accion)
            {
                case "Agregar":
                    await EjecutarAgregar(model, servicio);
                    break;

                case "Actualizar":
                    await EjecutarActualizar(model, servicio);
                    break;

                case "Eliminar":
                    await EjecutarEliminar(model, servicio);
                    break;

                case "Consultar":
                    await EjecutarConsultar(model, servicio);
                    return View(model);

                default:
                    model.EsExito = false;
                    model.Mensaje = "Accion no valida";
                    break;
            }

            model.Resultados = await ConsultarLista(model.UseWebService);
        }
        catch
        {
            model.EsExito = false;
            model.Mensaje = "Ocurrio un error al procesar la solicitud";
            model.Resultados = await ConsultarLista(model.UseWebService);
        }

        return View(model);
    }

    private clsExamen CrearServicio(bool useWebService)
    {
        var apiBaseUrl = _configuration["ExamenSettings:ApiBaseUrl"] ?? string.Empty;
        var connectionString = _configuration.GetConnectionString("DefaultConnection") ?? string.Empty;

        return new clsExamen(useWebService, apiBaseUrl, connectionString);
    }

    private ExamenViewModel CrearModeloBase(bool useWebService)
    {
        return new ExamenViewModel
        {
            UseWebService = useWebService
        };
    }

    private async Task<List<TblExamen>> ConsultarLista(bool useWebService)
    {
        var servicio = CrearServicio(useWebService);
        var respuesta = await servicio.ConsultarExamen();
        return respuesta.Data;
    }

    private async Task EjecutarAgregar(ExamenViewModel model, clsExamen servicio)
    {
        var respuesta = await servicio.AgregarExamen(CrearExamen(model));
        model.EsExito = respuesta.Ok;
        model.Mensaje = respuesta.Mensaje;
    }

    private async Task EjecutarActualizar(ExamenViewModel model, clsExamen servicio)
    {
        var respuesta = await servicio.ActualizarExamen(CrearExamen(model));
        model.EsExito = respuesta.Ok;
        model.Mensaje = respuesta.Mensaje;
    }

    private async Task EjecutarEliminar(ExamenViewModel model, clsExamen servicio)
    {
        var respuesta = await servicio.EliminarExamen(model.Id ?? 0);
        model.EsExito = respuesta.Ok;
        model.Mensaje = respuesta.Mensaje;
    }

    private async Task EjecutarConsultar(ExamenViewModel model, clsExamen servicio)
    {
        var respuesta = await servicio.ConsultarExamen(model.Id);
        model.EsExito = respuesta.Ok;
        model.Mensaje = respuesta.Mensaje;
        model.Resultados = respuesta.Data;
    }

    private TblExamen CrearExamen(ExamenViewModel model)
    {
        return new TblExamen
        {
            Id = model.Id ?? 0,
            Nombre = model.Nombre ?? string.Empty,
            Descripcion = model.Descripcion ?? string.Empty
        };
    }
}
