namespace ApiExamen.Models;

public class ConsultaExamenRespuesta : OperacionRespuesta
{
    public List<TblExamen> Data { get; set; } = new List<TblExamen>();
}
