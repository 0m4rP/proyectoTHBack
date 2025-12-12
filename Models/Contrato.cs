using System;
using System.Collections.Generic;

namespace proyectSystemTh.Models;

public partial class Contrato
{
    public int IdContratos { get; set; }

    public string TipoContrato { get; set; } = null!;

    public DateOnly FechaInicio { get; set; }

    public DateOnly? FechaFin { get; set; }

    public decimal SalarioContrato { get; set; }

    public int EmpleadoIdEmpleado { get; set; }

    public virtual Empleado EmpleadoIdEmpleadoNavigation { get; set; } = null!;
}
