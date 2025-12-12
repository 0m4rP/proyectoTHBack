using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;
using proyectSystemTh.Models;

namespace proyectSystemTh.Data;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Beneficio> Beneficios { get; set; }

    public virtual DbSet<Contrato> Contratos { get; set; }

    public virtual DbSet<Departamento> Departamentos { get; set; }

    public virtual DbSet<Empleado> Empleados { get; set; }

    public virtual DbSet<EmpleadoBeneficio> EmpleadoBeneficios { get; set; }

    public virtual DbSet<EvaluacionDesempeno> EvaluacionDesempenos { get; set; }

    public virtual DbSet<UsuarioSistema> UsuarioSistemas { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb3_spanish_ci")
            .HasCharSet("utf8mb3");

        modelBuilder.Entity<Beneficio>(entity =>
        {
            entity.HasKey(e => e.IdBeneficio).HasName("PRIMARY");

            entity.ToTable("beneficio");

            entity.Property(e => e.IdBeneficio).HasColumnName("idBeneficio");
            entity.Property(e => e.DescripcionBeneficio)
                .HasColumnType("text")
                .HasColumnName("descripcion_Beneficio");
            entity.Property(e => e.NombreBeneficio)
                .HasMaxLength(45)
                .HasColumnName("nombre_Beneficio");
            entity.Property(e => e.TipoBeneficio)
                .HasMaxLength(45)
                .HasColumnName("tipo_Beneficio");
        });

        modelBuilder.Entity<Contrato>(entity =>
        {
            entity.HasKey(e => e.IdContratos).HasName("PRIMARY");

            entity.ToTable("contratos");

            entity.HasIndex(e => e.EmpleadoIdEmpleado, "empleado_idEmpleado");

            entity.Property(e => e.IdContratos).HasColumnName("idContratos");
            entity.Property(e => e.EmpleadoIdEmpleado).HasColumnName("empleado_idEmpleado");
            entity.Property(e => e.FechaFin).HasColumnName("fecha_Fin");
            entity.Property(e => e.FechaInicio).HasColumnName("fecha_Inicio");
            entity.Property(e => e.SalarioContrato)
                .HasPrecision(10, 2)
                .HasColumnName("salario_Contrato");
            entity.Property(e => e.TipoContrato)
                .HasMaxLength(45)
                .HasColumnName("tipo_Contrato");

            entity.HasOne(d => d.EmpleadoIdEmpleadoNavigation).WithMany(p => p.Contratos)
                .HasForeignKey(d => d.EmpleadoIdEmpleado)
                .HasConstraintName("contratos_ibfk_1");
        });

        modelBuilder.Entity<Departamento>(entity =>
        {
            entity.HasKey(e => e.IdDepartamento).HasName("PRIMARY");

            entity.ToTable("departamento");

            entity.Property(e => e.IdDepartamento).HasColumnName("idDepartamento");
            entity.Property(e => e.DescripcionDepartamento)
                .HasMaxLength(200)
                .HasColumnName("descripcion_Departamento");
            entity.Property(e => e.NombreDepartamento)
                .HasMaxLength(45)
                .HasColumnName("nombre_Departamento");
        });

        modelBuilder.Entity<Empleado>(entity =>
        {
            entity.HasKey(e => e.IdEmpleado).HasName("PRIMARY");

            entity.ToTable("empleado");

            entity.HasIndex(e => e.Correo, "correo").IsUnique();

            entity.HasIndex(e => e.DepartamentoIdDepartamento, "departamento_idDepartamento");

            entity.Property(e => e.IdEmpleado).HasColumnName("idEmpleado");
            entity.Property(e => e.ApellidoEmpleado)
                .HasMaxLength(45)
                .HasColumnName("apellido_Empleado");
            entity.Property(e => e.Cargo)
                .HasMaxLength(45)
                .HasColumnName("cargo");
            entity.Property(e => e.Correo)
                .HasMaxLength(100)
                .HasColumnName("correo");
            entity.Property(e => e.DepartamentoIdDepartamento).HasColumnName("departamento_idDepartamento");
            entity.Property(e => e.DireccionEmpleado)
                .HasMaxLength(100)
                .HasColumnName("direccion_Empleado");
            entity.Property(e => e.EstadoEmpleado)
                .HasDefaultValueSql("'1'")
                .HasColumnName("estado_Empleado");
            entity.Property(e => e.FechaContrato).HasColumnName("fecha_Contrato");
            entity.Property(e => e.FechaNacimiento).HasColumnName("fecha_Nacimiento");
            entity.Property(e => e.NombreEmpleado)
                .HasMaxLength(45)
                .HasColumnName("nombre_Empleado");
            entity.Property(e => e.TelefonoEmpleado)
                .HasMaxLength(15)
                .HasColumnName("telefono_Empleado");

            entity.HasOne(d => d.DepartamentoIdDepartamentoNavigation).WithMany(p => p.Empleados)
                .HasForeignKey(d => d.DepartamentoIdDepartamento)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("empleado_ibfk_1");
        });

        modelBuilder.Entity<EmpleadoBeneficio>(entity =>
        {
            entity.HasKey(e => new { e.EmpleadoIdEmpleado, e.BeneficioIdBeneficio })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.ToTable("empleado_beneficio");

            entity.HasIndex(e => e.BeneficioIdBeneficio, "beneficio_idBeneficio");

            entity.Property(e => e.EmpleadoIdEmpleado).HasColumnName("empleado_idEmpleado");
            entity.Property(e => e.BeneficioIdBeneficio).HasColumnName("beneficio_idBeneficio");
            entity.Property(e => e.FechaAsignacion)
                .HasDefaultValueSql("curdate()")
                .HasColumnName("fecha_Asignacion");

            entity.HasOne(d => d.BeneficioIdBeneficioNavigation).WithMany(p => p.EmpleadoBeneficios)
                .HasForeignKey(d => d.BeneficioIdBeneficio)
                .HasConstraintName("empleado_beneficio_ibfk_2");

            entity.HasOne(d => d.EmpleadoIdEmpleadoNavigation).WithMany(p => p.EmpleadoBeneficios)
                .HasForeignKey(d => d.EmpleadoIdEmpleado)
                .HasConstraintName("empleado_beneficio_ibfk_1");
        });

        modelBuilder.Entity<EvaluacionDesempeno>(entity =>
        {
            entity.HasKey(e => e.IdEvaluacionDesempeno).HasName("PRIMARY");

            entity.ToTable("evaluacion_desempeno");

            entity.HasIndex(e => e.EmpleadoIdEmpleado, "empleado_idEmpleado");

            entity.Property(e => e.IdEvaluacionDesempeno).HasColumnName("idEvaluacion_Desempeno");
            entity.Property(e => e.EmpleadoIdEmpleado).HasColumnName("empleado_idEmpleado");
            entity.Property(e => e.FechaEvaluacion).HasColumnName("fecha_Evaluacion");
            entity.Property(e => e.Observacion)
                .HasColumnType("text")
                .HasColumnName("observacion");
            entity.Property(e => e.PuntuacionEvaluacion)
                .HasPrecision(3, 1)
                .HasColumnName("puntuacion_Evaluacion");

            entity.HasOne(d => d.EmpleadoIdEmpleadoNavigation).WithMany(p => p.EvaluacionDesempenos)
                .HasForeignKey(d => d.EmpleadoIdEmpleado)
                .HasConstraintName("evaluacion_desempeno_ibfk_1");
        });

        modelBuilder.Entity<UsuarioSistema>(entity =>
        {
            entity.HasKey(e => e.IdUsuarioSistema).HasName("PRIMARY");

            entity.ToTable("usuario_sistema");

            entity.HasIndex(e => e.EmpleadoIdEmpleado, "empleado_idEmpleado").IsUnique();

            entity.HasIndex(e => e.NombreUsuario, "nombre_Usuario").IsUnique();

            entity.Property(e => e.IdUsuarioSistema).HasColumnName("idUsuario_Sistema");
            entity.Property(e => e.ContrasenaUsuario)
                .HasMaxLength(255)
                .HasColumnName("contrasena_Usuario");
            entity.Property(e => e.EmpleadoIdEmpleado).HasColumnName("empleado_idEmpleado");
            entity.Property(e => e.NombreUsuario)
                .HasMaxLength(45)
                .HasColumnName("nombre_Usuario");
            entity.Property(e => e.RolUsuario)
                .HasMaxLength(45)
                .HasDefaultValueSql("'Usuario'")
                .HasColumnName("rol_Usuario");

            entity.HasOne(d => d.EmpleadoIdEmpleadoNavigation).WithOne(p => p.UsuarioSistema)
                .HasForeignKey<UsuarioSistema>(d => d.EmpleadoIdEmpleado)
                .HasConstraintName("usuario_sistema_ibfk_1");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
