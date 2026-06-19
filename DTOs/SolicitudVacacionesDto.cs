namespace proyectSystemTh.DTOs
{
    public class SolicitudVacacionesDto
    {
        public int IdEmpleado { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaRetoma { get; set; }
        public string? Motivo { get; set; }
    }
}