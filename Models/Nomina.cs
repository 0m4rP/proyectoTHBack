using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace proyectSystemTh.Models
{
    [Table("nomina")]
    public class Nomina
    {
        [Key]
        [Column("idNomina")]
        public int IdNomina { get; set; }

        [Column("idEmpleado")]
        public int IdEmpleado { get; set; }

        [Column("periodoMes")]
        public int PeriodoMes { get; set; }

        [Column("periodoAnio")]
        public int PeriodoAnio { get; set; }

        [Column("salarioBase")]
        public decimal SalarioBase { get; set; }

        [Column("totalDevengado")]
        public decimal TotalDevengado { get; set; }

        [Column("totalDeducciones")]
        public decimal TotalDeducciones { get; set; }

        [Column("netoPagar")]
        public decimal NetoPagar { get; set; }

        [Column("fechaPago")]
        public DateTime FechaPago { get; set; }
    }
}