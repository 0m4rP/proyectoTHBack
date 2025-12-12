using System;
using System.Collections.Generic;

namespace proyectSystemTh.Models;

public partial class Empleado
{
    public int IdEmpleado { get; set; }

    public string NombreEmpleado { get; set; } = null!;

    public string ApellidoEmpleado { get; set; } = null!;

    public DateOnly FechaNacimiento { get; set; }

    public string? DireccionEmpleado { get; set; }

    public string? TelefonoEmpleado { get; set; }

    public string? Correo { get; set; }

    public string Cargo { get; set; } = null!;

    public DateOnly FechaContrato { get; set; }

    public sbyte? EstadoEmpleado { get; set; }

    public int DepartamentoIdDepartamento { get; set; }

    public virtual ICollection<Contrato> Contratos { get; set; } = new List<Contrato>();

    public virtual Departamento DepartamentoIdDepartamentoNavigation { get; set; } = null!;

    public virtual ICollection<EmpleadoBeneficio> EmpleadoBeneficios { get; set; } = new List<EmpleadoBeneficio>();

    public virtual ICollection<EvaluacionDesempeno> EvaluacionDesempenos { get; set; } = new List<EvaluacionDesempeno>();

    public virtual UsuarioSistema? UsuarioSistema { get; set; }
}
