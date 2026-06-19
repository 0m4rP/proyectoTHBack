namespace proyectSystemTh.DTOs
{
    public class NominaPeriodoDto
    {
        public int IdNomina { get; set; }
        public int PeriodoMes { get; set; }
        public int PeriodoAnio { get; set; }
        public string MesNombre => PeriodoMes switch
        {
            1 => "Enero",
            2 => "Febrero",
            3 => "Marzo",
            4 => "Abril",
            5 => "Mayo",
            6 => "Junio",
            7 => "Julio",
            8 => "Agosto",
            9 => "Septiembre",
            10 => "Octubre",
            11 => "Noviembre",
            12 => "Diciembre",
            _ => "Desconocido"
        };
    }
}