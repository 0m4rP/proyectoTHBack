namespace proyectSystemTh.DTOs
{
    public class ComprobanteDetalleDto
    {
        public int IdNomina { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public string Cargo { get; set; } = string.Empty;
        public int PeriodoMes { get; set; }
        public int PeriodoAnio { get; set; }
        public decimal SalarioBase { get; set; }
        public decimal TotalDevengado { get; set; }
        public decimal TotalDeducciones { get; set; }
        public decimal NetoPagar { get; set; }
        public DateTime FechaPago { get; set; }
    }
}