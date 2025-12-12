using System;
using System.Collections.Generic;

namespace proyectSystemTh.Models;

public partial class EvaluacionDesempeno
{
    public int IdEvaluacionDesempeno { get; set; }

    public DateOnly FechaEvaluacion { get; set; }

    public decimal? PuntuacionEvaluacion { get; set; }

    public string? Observacion { get; set; }

    public int EmpleadoIdEmpleado { get; set; }

    public virtual Empleado EmpleadoIdEmpleadoNavigation { get; set; } = null!;
}
