using Microsoft.EntityFrameworkCore;
using proyectSystemTh.Data;
using proyectSystemTh.DTOs;

namespace proyectSystemTh.Services.InfoUsers
{
    public class InforUsers : IInfoUsers
    {
        private readonly ApplicationDbContext _context;
        public InforUsers(ApplicationDbContext context)
        {
            _context = context;
        }

        #region Traer información del usuario por id
        public async Task<EmpleadoDTO> infoEmploy(int id)
        {
            var usuario = await _context.Empleados.FirstOrDefaultAsync(u => u.IdEmpleado == id);

            if (usuario == null) return null;

            //lógica de vacaiones
            DateTime hoy = DateTime.Today;
            // Reemplaza esta línea:
            // DateTime inicio = usuario.FechaContrato;

            // Por esta conversión explícita de DateOnly a DateTime:
            DateTime inicio = usuario.FechaContrato.ToDateTime(TimeOnly.MinValue);

            int tiempoAntiguedad = ((hoy.Year - inicio.Year) * 12) + hoy.Month - inicio.Month;

            if(hoy.Day < inicio.Day)
            {
                tiempoAntiguedad--;
            }

            int diasGanados = Math.Max(0, tiempoAntiguedad);
            int diasTomados = usuario.DiasTomados;



            //mapear a DTO
            return new EmpleadoDTO
            {
                NombreEmpleado = usuario.NombreEmpleado,
                ApellidoEmpleado = usuario.ApellidoEmpleado,
                FechaNacimiento = usuario.FechaNacimiento,
                DireccionEmpleado = usuario.DireccionEmpleado,
                TelefonoEmpleado = usuario.TelefonoEmpleado,
                Correo = usuario.Correo,
                Cargo = usuario.Cargo,
                FechaContrato = usuario.FechaContrato,
                EstadoEmpleado = usuario.EstadoEmpleado,
                DepartamentoIdDepartamento = usuario.DepartamentoIdDepartamento,
                VacacionesDisponibles = Math.Max(0, diasGanados - diasTomados),
                VacacionesTomadas = diasTomados
            };
        }
        #endregion

    }
}
