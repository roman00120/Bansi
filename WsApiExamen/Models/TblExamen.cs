using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WsApiExamen.Models;

[Table("tblExamen")]
public class TblExamen
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [Required]
    [StringLength(255)]
    public string Descripcion { get; set; } = string.Empty;
}
