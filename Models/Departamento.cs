using System;
using System.Collections.Generic;

namespace proyectSystemTh.Models;

public partial class Departamento
{
    public int IdDepartamento { get; set; }

    public string NombreDepartamento { get; set; } = null!;

    public string? DescripcionDepartamento { get; set; }

    public virtual ICollection<Empleado> Empleados { get; set; } = new List<Empleado>();
}
