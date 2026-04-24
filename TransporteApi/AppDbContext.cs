using Domain.Dto;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using System.Reflection.Metadata;

namespace TransporteApi.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

       // public DbSet<Producto> Productos => Set<Producto>();
        public DbSet<Rol> Roles => Set<Rol>();
        public DbSet<Usuario> Usuarios => Set<Usuario>();
        public DbSet<Camion> Camiones => Set<Camion>();
        public DbSet<Conductor> Conductores => Set<Conductor>();
        public DbSet<RolPermiso> Roles_Permiso => Set<RolPermiso>();
        public DbSet<Propietario> Propietario => Set<Propietario>();
        public DbSet<CentroDistribucion> Centro_distribucion => Set<CentroDistribucion>();
        public DbSet<Guia> Guias => Set<Guia>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RolPermiso>()
            .HasOne(d => d.Rol)
            .WithMany(p => p.RolesPermisos)
            .HasForeignKey(d => d.Rol_Id);

            modelBuilder.Entity<Rol>()
           .HasMany(d => d.RolesPermisos)
           .WithOne(p => p.Rol)
           .HasForeignKey(d => d.Rol_Id);

            modelBuilder.Entity<Camion>()
            .HasOne(d => d.Propietario)
            .WithMany(p => p.Camion)
            .HasForeignKey(d => d.Propietario_Id);

            modelBuilder.Entity<Conductor>()
           .HasOne(d => d.Propietario)
           .WithMany(p => p.Conductor)
           .HasForeignKey(d => d.Propietario_Id);

            modelBuilder.Entity<Conductor>()
           .HasOne(d => d.Camion)
           .WithOne(p => p.Conductor)
           .HasForeignKey<Conductor>(d => d.Camion_Id);

            modelBuilder.Entity<Propietario>()
           .HasMany(d => d.Camion)
           .WithOne(p => p.Propietario)
           .HasForeignKey(d => d.Propietario_Id);



            modelBuilder.Entity<Guia>(entity =>
            {
                // Nombre de la tabla
                entity.ToTable("guias");

                // Configurar cada propiedad con su nombre de columna exacto en la BD
                entity.Property(e => e.Id)
                    .HasColumnName("Id")
                    .HasColumnType("int");

                entity.Property(e => e.Numero_guia)
                    .HasColumnName("Numero_guia")
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.Tipo)
                    .HasColumnName("Tipo")
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.Status)
                    .HasColumnName("Status")
                    .HasColumnType("varchar(50)");

                // ¡PARTE IMPORTANTE! Mapear las propiedades con guión bajo
                entity.Property(e => e.Conductor_id)
                    .HasColumnName("Conductor_id")
                    .HasColumnType("int");

                entity.Property(e => e.Camion_id)
                    .HasColumnName("Camion_id")
                    .HasColumnType("int");

                entity.Property(e => e.Origen_id)
                    .HasColumnName("Origen_id")
                    .HasColumnType("int");

                entity.Property(e => e.Destino_id)
                    .HasColumnName("Destino_id")
                    .HasColumnType("int");

                entity.Property(e => e.Fecha)
                    .HasColumnName("Fecha")
                    .HasColumnType("datetime");

                entity.Property(e => e.Created_at)
                    .HasColumnName("Created_at")
                    .HasColumnType("datetime");

                entity.Property(e => e.Updated_at)
                    .HasColumnName("Updated_at")
                    .HasColumnType("datetime");

                // Configurar las relaciones (FOREIGN KEYS)
                entity.HasOne(e => e.Conductor)
                    .WithMany()
                    .HasForeignKey(e => e.Conductor_id)
                    .HasConstraintName("FK_Guias_Conductores")
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Camion)
                    .WithMany()
                    .HasForeignKey(e => e.Camion_id)
                    .HasConstraintName("FK_Guias_Camiones")
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Origen)
                    .WithMany()
                    .HasForeignKey(e => e.Origen_id)
                    .HasConstraintName("FK_Guias_Origen")
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Destino)
                    .WithMany()
                    .HasForeignKey(e => e.Destino_id)
                    .HasConstraintName("FK_Guias_Destino")
                    .OnDelete(DeleteBehavior.Restrict);

                // Índices
                entity.HasIndex(e => e.Numero_guia)
                    .IsUnique()
                    .HasDatabaseName("IX_Guias_NumeroGuia");

                entity.HasIndex(e => e.Conductor_id)
                    .HasDatabaseName("IX_Guias_ConductorId");

                entity.HasIndex(e => e.Camion_id)
                    .HasDatabaseName("IX_Guias_CamionId");

                entity.HasIndex(e => e.Origen_id)
                    .HasDatabaseName("IX_Guias_OrigenId");

                entity.HasIndex(e => e.Destino_id)
                    .HasDatabaseName("IX_Guias_DestinoId");
            });



        }
    }
}
