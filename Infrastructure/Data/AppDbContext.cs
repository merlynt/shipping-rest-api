using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Rol> Roles { get; set; }
        public DbSet<Estado> Estados { get; set; }
        public DbSet<Departamento> Departamentos { get; set; }
        public DbSet<Distrito> Distritos { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Administrador> Administradores { get; set; }
        public DbSet<Empresa> Empresas { get; set; }
        public DbSet<Piloto> Pilotos { get; set; }
        public DbSet<Destinatario> Destinatarios { get; set; }
        public DbSet<Envio> Envios { get; set; }
        public DbSet<Evidencia> Evidencias { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de propiedades
            modelBuilder.Entity<Envio>()
                .Property(e => e.Peso)
                .HasColumnType("decimal(18,2)");

            // HACER QUE EL CÓDIGO DE TRACKING SEA ÚNICO E IRREPETIBLE
            modelBuilder.Entity<Envio>()
                .HasIndex(e => e.CodigoTracking)
                .IsUnique();

            modelBuilder.Entity<Envio>()
                .HasMany(e => e.Evidencias)
                .WithOne(ev => ev.Envio)
                .HasForeignKey(ev => ev.EnvioId)
                .OnDelete(DeleteBehavior.Cascade);

            // Filtros globales para Soft Delete (Ocultamiento en cascada)
            modelBuilder.Entity<Empresa>().HasQueryFilter(e => e.Activo);
            modelBuilder.Entity<Envio>().HasQueryFilter(e => e.Empresa.Activo);
            modelBuilder.Entity<Evidencia>().HasQueryFilter(ev => ev.Envio.Empresa.Activo);

            // Relaciones estrictas 1 a 1 (Perfiles de Usuario)
            modelBuilder.Entity<Administrador>()
                .HasOne(a => a.Usuario).WithOne().HasForeignKey<Administrador>(a => a.UsuarioId);

            modelBuilder.Entity<Empresa>()
                .HasOne(e => e.Usuario).WithOne().HasForeignKey<Empresa>(e => e.UsuarioId);

            modelBuilder.Entity<Piloto>()
                .HasOne(p => p.Usuario).WithOne().HasForeignKey<Piloto>(p => p.UsuarioId);

            // Restricciones de borrado para evitar rutas de cascada múltiples 
            modelBuilder.Entity<Envio>()
                .HasOne(e => e.Empresa).WithMany().HasForeignKey(e => e.EmpresaId).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Envio>()
                .HasOne(e => e.Destinatario).WithMany().HasForeignKey(e => e.DestinatarioId).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Envio>()
                .HasOne(e => e.Estado).WithMany().HasForeignKey(e => e.EstadoId).OnDelete(DeleteBehavior.Restrict);

            // 1. Catálogos Base (DEBEN IR PRIMERO)
            modelBuilder.Entity<Departamento>().HasData(new Departamento { Id = 1, Nombre = "San Salvador" });
            modelBuilder.Entity<Distrito>().HasData(new Distrito { Id = 1, Nombre = "San Salvador", DepartamentoId = 1 });

            modelBuilder.Entity<Estado>().HasData(
                new Estado { Id = 1, Nombre = "Recolectado" },
                new Estado { Id = 2, Nombre = "En Bodega" },
                new Estado { Id = 3, Nombre = "En Ruta" },
                new Estado { Id = 4, Nombre = "Entregado" },
                new Estado { Id = 5, Nombre = "Devolucion" }
            );

            modelBuilder.Entity<Rol>().HasData(
                new Rol { Id = 1, Nombre = "Administrador" },
                new Rol { Id = 2, Nombre = "Empresa" },
                new Rol { Id = 3, Nombre = "Piloto" }
            );

            // 2. Usuarios base
            modelBuilder.Entity<Usuario>().HasData(
                new Usuario { Id = 1, Email = "admin@sistema.com", Password = "123456", RolId = 1 },
                new Usuario { Id = 2, Email = "empresa@logistica.com", Password = "123456", RolId = 2 },
                new Usuario { Id = 3, Email = "piloto@logistica.com", Password = "123456", RolId = 3 }
            );

            // 3. Perfiles vinculados (APUNTANDO AL DISTRITO 1)
            modelBuilder.Entity<Empresa>().HasData(
                new Empresa
                {
                    Id = 1,
                    NombreEmpresa = "Logística Global",
                    UsuarioId = 2,
                    Activo = true,
                    DistritoId = 1,
                    Codigo = "LOG001",
                    Telefono = "2222-2222",
                    Direccion = "San Salvador"
                }
            );

            modelBuilder.Entity<Piloto>().HasData(
                new Piloto
                {
                    Id = 1,
                    Nombre = "Juan Piloto",
                    UsuarioId = 3,
                    Telefono = "7777-7777",
                    Vehiculo = "Moto",
                    NumeroLicencia = "L123"
                }
            );

            // 4. Perfil del Administrador Master (Super Admin)
            modelBuilder.Entity<Administrador>().HasData(
                new Administrador
                {
                    Id = 1,
                    Nombre = "Admin",
                    Apellido = "Central",
                    Direccion = "Oficina Principal",
                    Telefono = "2222-0000",
                    EsMaster = true,
                    UsuarioId = 1,   
                    DistritoId = 1 
                }
            );
        }
    }
}