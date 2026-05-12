using System.Data;
using System.Net.Http.Json;
using System.Text.Json;
using ApiExamen.Models;
using Microsoft.Data.SqlClient;

namespace ApiExamen.Services;

public class clsExamen
{
    private readonly string _apiBaseUrl;
    private readonly string _connectionString;
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public bool UseWebService { get; set; }

    public clsExamen(bool useWebService, string apiBaseUrl, string connectionString)
    {
        UseWebService = useWebService;
        _apiBaseUrl = apiBaseUrl?.Trim() ?? string.Empty;
        _connectionString = connectionString?.Trim() ?? string.Empty;
    }

    public async Task<OperacionRespuesta> AgregarExamen(TblExamen examen)
    {
        var validacion = ValidarExamen(examen);
        if (!validacion.Ok)
        {
            return validacion;
        }

        return UseWebService
            ? await AgregarPorApi(examen)
            : await AgregarPorStoredProcedure(examen);
    }

    public async Task<OperacionRespuesta> ActualizarExamen(TblExamen examen)
    {
        var validacion = ValidarExamen(examen);
        if (!validacion.Ok)
        {
            return validacion;
        }

        return UseWebService
            ? await ActualizarPorApi(examen)
            : await ActualizarPorStoredProcedure(examen);
    }

    public async Task<OperacionRespuesta> EliminarExamen(int id)
    {
        var validacion = ValidarId(id);
        if (!validacion.Ok)
        {
            return validacion;
        }

        return UseWebService
            ? await EliminarPorApi(id)
            : await EliminarPorStoredProcedure(id);
    }

    public async Task<ConsultaExamenRespuesta> ConsultarExamen(int? id = null)
    {
        if (id.HasValue && id.Value <= 0)
        {
            return new ConsultaExamenRespuesta
            {
                Ok = false,
                Mensaje = "El Id no es valido"
            };
        }

        return UseWebService
            ? await ConsultarPorApi(id)
            : await ConsultarPorStoredProcedure(id);
    }

    private OperacionRespuesta ValidarExamen(TblExamen? examen)
    {
        if (examen is null)
        {
            return new OperacionRespuesta
            {
                Ok = false,
                Mensaje = "Los datos del examen son obligatorios"
            };
        }

        if (examen.Id <= 0)
        {
            return new OperacionRespuesta
            {
                Ok = false,
                Mensaje = "El Id es obligatorio"
            };
        }

        if (string.IsNullOrWhiteSpace(examen.Nombre))
        {
            return new OperacionRespuesta
            {
                Ok = false,
                Mensaje = "El nombre es obligatorio"
            };
        }

        if (string.IsNullOrWhiteSpace(examen.Descripcion))
        {
            return new OperacionRespuesta
            {
                Ok = false,
                Mensaje = "La descripcion es obligatoria"
            };
        }

        examen.Nombre = examen.Nombre.Trim();
        examen.Descripcion = examen.Descripcion.Trim();

        return new OperacionRespuesta
        {
            Ok = true
        };
    }

    private OperacionRespuesta ValidarId(int id)
    {
        if (id <= 0)
        {
            return new OperacionRespuesta
            {
                Ok = false,
                Mensaje = "El Id es obligatorio"
            };
        }

        return new OperacionRespuesta
        {
            Ok = true
        };
    }

    private HttpClient CrearHttpClient()
    {
        if (string.IsNullOrWhiteSpace(_apiBaseUrl))
        {
            throw new InvalidOperationException("La URL de la WebAPI es obligatoria");
        }

        return new HttpClient
        {
            BaseAddress = new Uri(_apiBaseUrl.EndsWith("/") ? _apiBaseUrl : $"{_apiBaseUrl}/")
        };
    }

    private async Task<OperacionRespuesta> AgregarPorApi(TblExamen examen)
    {
        try
        {
            using var client = CrearHttpClient();
            var response = await client.PostAsJsonAsync("api/Examen", examen);

            return await LeerRespuestaOperacion(response, "Error al agregar el registro");
        }
        catch
        {
            return new OperacionRespuesta
            {
                Ok = false,
                Mensaje = "Error al consumir la WebAPI"
            };
        }
    }

    private async Task<OperacionRespuesta> ActualizarPorApi(TblExamen examen)
    {
        try
        {
            using var client = CrearHttpClient();
            var response = await client.PutAsJsonAsync($"api/Examen/{examen.Id}", examen);

            return await LeerRespuestaOperacion(response, "Error al actualizar el registro");
        }
        catch
        {
            return new OperacionRespuesta
            {
                Ok = false,
                Mensaje = "Error al consumir la WebAPI"
            };
        }
    }

    private async Task<OperacionRespuesta> EliminarPorApi(int id)
    {
        try
        {
            using var client = CrearHttpClient();
            var response = await client.DeleteAsync($"api/Examen/{id}");

            return await LeerRespuestaOperacion(response, "Error al eliminar el registro");
        }
        catch
        {
            return new OperacionRespuesta
            {
                Ok = false,
                Mensaje = "Error al consumir la WebAPI"
            };
        }
    }

    private async Task<ConsultaExamenRespuesta> ConsultarPorApi(int? id)
    {
        try
        {
            using var client = CrearHttpClient();
            var url = id.HasValue ? $"api/Examen/{id.Value}" : "api/Examen";
            var response = await client.GetAsync(url);
            var json = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(json))
            {
                return new ConsultaExamenRespuesta
                {
                    Ok = false,
                    Mensaje = "No se obtuvo respuesta de la WebAPI"
                };
            }

            using var document = JsonDocument.Parse(json);
            var root = document.RootElement;

            var resultado = new ConsultaExamenRespuesta
            {
                Ok = LeerBool(root, "ok"),
                Mensaje = LeerTexto(root, "mensaje")
            };

            if (!root.TryGetProperty("data", out var data))
            {
                return resultado;
            }

            if (data.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in data.EnumerateArray())
                {
                    var examen = item.Deserialize<TblExamen>(_jsonOptions);
                    if (examen is not null)
                    {
                        resultado.Data.Add(examen);
                    }
                }
            }

            if (data.ValueKind == JsonValueKind.Object)
            {
                var examen = data.Deserialize<TblExamen>(_jsonOptions);
                if (examen is not null)
                {
                    resultado.Data.Add(examen);
                }
            }

            return resultado;
        }
        catch
        {
            return new ConsultaExamenRespuesta
            {
                Ok = false,
                Mensaje = "Error al consumir la WebAPI"
            };
        }
    }

    private async Task<OperacionRespuesta> AgregarPorStoredProcedure(TblExamen examen)
    {
        try
        {
            using var connection = CrearConexion();
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();
            using var command = new SqlCommand("spAgregar", connection, transaction);

            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@Id", examen.Id);
            command.Parameters.AddWithValue("@Nombre", examen.Nombre);
            command.Parameters.AddWithValue("@Descripcion", examen.Descripcion);

            var respuesta = await EjecutarOperacion(command);

            if (respuesta.Ok)
            {
                await transaction.CommitAsync();
            }
            else
            {
                await transaction.RollbackAsync();
            }

            return respuesta;
        }
        catch
        {
            return new OperacionRespuesta
            {
                Ok = false,
                Mensaje = "Error al ejecutar spAgregar"
            };
        }
    }

    private async Task<OperacionRespuesta> ActualizarPorStoredProcedure(TblExamen examen)
    {
        try
        {
            using var connection = CrearConexion();
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();
            using var command = new SqlCommand("spActualizar", connection, transaction);

            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@Id", examen.Id);
            command.Parameters.AddWithValue("@Nombre", examen.Nombre);
            command.Parameters.AddWithValue("@Descripcion", examen.Descripcion);

            var respuesta = await EjecutarOperacion(command);

            if (respuesta.Ok)
            {
                await transaction.CommitAsync();
            }
            else
            {
                await transaction.RollbackAsync();
            }

            return respuesta;
        }
        catch
        {
            return new OperacionRespuesta
            {
                Ok = false,
                Mensaje = "Error al ejecutar spActualizar"
            };
        }
    }

    private async Task<OperacionRespuesta> EliminarPorStoredProcedure(int id)
    {
        try
        {
            using var connection = CrearConexion();
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();
            using var command = new SqlCommand("spEliminar", connection, transaction);

            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@Id", id);

            var respuesta = await EjecutarOperacion(command);

            if (respuesta.Ok)
            {
                await transaction.CommitAsync();
            }
            else
            {
                await transaction.RollbackAsync();
            }

            return respuesta;
        }
        catch
        {
            return new OperacionRespuesta
            {
                Ok = false,
                Mensaje = "Error al ejecutar spEliminar"
            };
        }
    }

    private async Task<ConsultaExamenRespuesta> ConsultarPorStoredProcedure(int? id)
    {
        try
        {
            using var connection = CrearConexion();
            await connection.OpenAsync();
            using var command = new SqlCommand("spConsultar", connection);

            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@Id", id.HasValue ? id.Value : DBNull.Value);

            using var reader = await command.ExecuteReaderAsync();

            var respuesta = new ConsultaExamenRespuesta();

            while (await reader.ReadAsync())
            {
                if (string.IsNullOrWhiteSpace(respuesta.Mensaje))
                {
                    respuesta.Ok = reader["Ok"] != DBNull.Value && Convert.ToBoolean(reader["Ok"]);
                    respuesta.Mensaje = reader["Mensaje"]?.ToString() ?? string.Empty;
                }

                if (reader["Id"] == DBNull.Value)
                {
                    continue;
                }

                respuesta.Data.Add(new TblExamen
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    Nombre = reader["Nombre"]?.ToString() ?? string.Empty,
                    Descripcion = reader["Descripcion"]?.ToString() ?? string.Empty
                });
            }

            if (string.IsNullOrWhiteSpace(respuesta.Mensaje))
            {
                respuesta.Ok = false;
                respuesta.Mensaje = "No se obtuvo respuesta del procedimiento";
            }

            return respuesta;
        }
        catch
        {
            return new ConsultaExamenRespuesta
            {
                Ok = false,
                Mensaje = "Error al ejecutar spConsultar"
            };
        }
    }

    private SqlConnection CrearConexion()
    {
        if (string.IsNullOrWhiteSpace(_connectionString))
        {
            throw new InvalidOperationException("La cadena de conexion es obligatoria");
        }

        return new SqlConnection(_connectionString);
    }

    private async Task<OperacionRespuesta> EjecutarOperacion(SqlCommand command)
    {
        using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
        {
            return new OperacionRespuesta
            {
                Ok = false,
                Mensaje = "No se obtuvo respuesta del procedimiento"
            };
        }

        return new OperacionRespuesta
        {
            Ok = reader["Ok"] != DBNull.Value && Convert.ToBoolean(reader["Ok"]),
            Mensaje = reader["Mensaje"]?.ToString() ?? string.Empty
        };
    }

    private async Task<OperacionRespuesta> LeerRespuestaOperacion(HttpResponseMessage response, string mensajeError)
    {
        try
        {
            var json = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(json))
            {
                return new OperacionRespuesta
                {
                    Ok = false,
                    Mensaje = "No se obtuvo respuesta de la WebAPI"
                };
            }

            using var document = JsonDocument.Parse(json);
            var root = document.RootElement;

            return new OperacionRespuesta
            {
                Ok = LeerBool(root, "ok"),
                Mensaje = LeerTexto(root, "mensaje", mensajeError)
            };
        }
        catch
        {
            return new OperacionRespuesta
            {
                Ok = false,
                Mensaje = mensajeError
            };
        }
    }

    private static bool LeerBool(JsonElement element, string propertyName)
    {
        if (!element.TryGetProperty(propertyName, out var property))
        {
            return false;
        }

        return property.ValueKind == JsonValueKind.True || property.ValueKind == JsonValueKind.False
            ? property.GetBoolean()
            : false;
    }

    private static string LeerTexto(JsonElement element, string propertyName, string defaultValue = "")
    {
        if (!element.TryGetProperty(propertyName, out var property))
        {
            return defaultValue;
        }

        return property.GetString() ?? defaultValue;
    }
}
