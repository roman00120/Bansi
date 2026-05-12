using Microsoft.EntityFrameworkCore;
using WsApiExamen.Models;

namespace WsApiExamen.Data;

public class ExamenDbContext(DbContextOptions<ExamenDbContext> options) : DbContext(options)
{
    public DbSet<TblExamen> Examenes => Set<TblExamen>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TblExamen>(entity =>
        {
            entity.ToTable("tblExamen");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).ValueGeneratedNever();
            entity.Property(x => x.Nombre).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Descripcion).HasMaxLength(255).IsRequired();
        });
    }
}
