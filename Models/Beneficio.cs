using System;
using System.Collections.Generic;

namespace proyectSystemTh.Models;

public partial class Beneficio
{
    public int IdBeneficio { get; set; }

    public string NombreBeneficio { get; set; } = null!;

    public string? DescripcionBeneficio { get; set; }

    public string TipoBeneficio { get; set; } = null!;

    public virtual ICollection<EmpleadoBeneficio> EmpleadoBeneficios { get; set; } = new List<EmpleadoBeneficio>();
}
