namespace proyectSystemTh.DTOs
{
    public class EmpleadoCreateDTO
    {
        public string NombreEmpleado { get; set; }
        public string ApellidoEmpleado { get; set; }
        public DateOnly FechaNacimiento { get; set; }
        public string? DireccionEmpleado { get; set; }
        public string? TelefonoEmpleado { get; set; }
        public string? Correo { get; set; }
        public string Cargo { get; set; }
        public DateOnly FechaContrato { get; set; }
        public sbyte? EstadoEmpleado { get; set; } = 1; // 1 = Activo por defecto
        public int DepartamentoIdDepartamento { get; set; }
    }

    public class EmpleadoDTO
    {
        public int IdEmpleado { get; set; }
        public string NombreEmpleado { get; set; }
        public string ApellidoEmpleado { get; set; }
        public string NombreCompleto => $"{NombreEmpleado} {ApellidoEmpleado}";
        public DateOnly FechaNacimiento { get; set; }
        public string? DireccionEmpleado { get; set; }
        public string? TelefonoEmpleado { get; set; }
        public string? Correo { get; set; }
        public string Cargo { get; set; }
        public DateOnly FechaContrato { get; set; }
        public sbyte? EstadoEmpleado { get; set; }
        public int DepartamentoIdDepartamento { get; set; }
        public string? NombreDepartamento { get; set; }
    }
}
