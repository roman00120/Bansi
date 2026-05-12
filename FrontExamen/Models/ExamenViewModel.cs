using ApiExamen.Models;

namespace FrontExamen.Models;

public class ExamenViewModel
{
    public int? Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public bool UseWebService { get; set; } = true;
    public string Mensaje { get; set; } = string.Empty;
    public bool EsExito { get; set; }
    public List<TblExamen> Resultados { get; set; } = new List<TblExamen>();
}
