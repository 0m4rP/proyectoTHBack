using System;
using System.Collections.Generic;

namespace proyectSystemTh.Models;

public partial class UsuarioSistema
{
    public int IdUsuarioSistema { get; set; }

    public string NombreUsuario { get; set; } = null!;

    public string ContrasenaUsuario { get; set; } = null!;

    public string RolUsuario { get; set; } = null!;

    public int EmpleadoIdEmpleado { get; set; }

    public virtual Empleado EmpleadoIdEmpleadoNavigation { get; set; } = null!;
}
