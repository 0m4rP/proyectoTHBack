namespace proyectSystemTh.DTOs
{
    public class AuthDTO
    {
        public bool Success { get; set; }
        public string Message { get; set; } = null!;
        public string? Token { get; set; }
        public UsuarioSistemaCreateDTO? User { get; set; }
    }
    // Para creación SIN empleado (usando empleado existente)
    public class UsuarioSistemaCreateDTO
    {
        public string NombreUsuario { get; set; }
        public string ContrasenaUsuario { get; set; }
        public string RolUsuario { get; set; }
        public int EmpleadoIdEmpleado { get; set; } // ID del empleado existente
    }

    // Para creación CON empleado (crear ambos)
    public class UsuarioConEmpleadoCreateDTO
    {
        // Datos del usuario
        public string NombreUsuario { get; set; }
        public string ContrasenaUsuario { get; set; }
        public string RolUsuario { get; set; }

        // Datos del empleado
        public string NombreEmpleado { get; set; }
        public string ApellidoEmpleado { get; set; }
        public DateOnly FechaNacimiento { get; set; }
        public string? Correo { get; set; }
        public string Cargo { get; set; }
        public DateOnly FechaContrato { get; set; }
        public int DepartamentoIdDepartamento { get; set; }
    }

    // Para respuesta (lectura)
    public class UsuarioSistemaDTO
    {
        public int IdUsuarioSistema { get; set; }
        public string NombreUsuario { get; set; }
        public string RolUsuario { get; set; }
        public int EmpleadoIdEmpleado { get; set; }
        public string? NombreEmpleado { get; set; }
        public string? ApellidoEmpleado { get; set; }
        public string? NombreCompletoEmpleado =>
            $"{NombreEmpleado} {ApellidoEmpleado}".Trim();
        public string? CorreoEmpleado { get; set; }
        public string? CargoEmpleado { get; set; }
    }
}