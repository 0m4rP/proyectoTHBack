using System;
using System.Collections.Generic;

namespace proyectSystemTh.Models;

public partial class EmpleadoBeneficio
{
    public int EmpleadoIdEmpleado { get; set; }

    public int BeneficioIdBeneficio { get; set; }

    public DateOnly FechaAsignacion { get; set; }

    public virtual Beneficio BeneficioIdBeneficioNavigation { get; set; } = null!;

    public virtual Empleado EmpleadoIdEmpleadoNavigation { get; set; } = null!;
}
