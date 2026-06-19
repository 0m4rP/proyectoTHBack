using Microsoft.EntityFrameworkCore; // 👈 Esencial para ejecutar SQL Raw asíncrono
using proyectSystemTh.Data;          // 👈 Tu carpeta de datos para el ApplicationDbContext
using proyectSystemTh.DTOs;
using System.Data;

namespace proyectSystemTh.Services.Solicitudes
{
    public class SolicitudesService : ISolicitudesService
    {
        private readonly ApplicationDbContext _context;

        public SolicitudesService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CrearSolicitudCambioAsync(SolicitudCambioDatosDto dto)
        {
            // Usamos parámetros posicionales ({0}, {1}...) para mantener la consulta segura contra SQL Injection
            string query = @"INSERT INTO solicitud_cambio_datos (idEmpleado, nuevaDireccion, nuevoTelefono, nuevoCorreo, estado) 
                             VALUES ({0}, {1}, {2}, {3}, 'Pendiente')";

            try
            {
                // Ejecuta el SQL directamente aprovechando la conexión de EF Core
                int rowsAffected = await _context.Database.ExecuteSqlRawAsync(query,
                    dto.IdEmpleado,
                    dto.NuevaDireccion ?? (object)DBNull.Value,
                    dto.NuevoTelefono ?? (object)DBNull.Value,
                    dto.NuevoCorreo ?? (object)DBNull.Value);

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error en SolicitudesService]: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> CrearSolicitudVacacionesAsync(SolicitudVacacionesDto dto)
        {
            // Usamos parámetros posicionales ({0}, {1}...) para evitar inyecciones SQL dañinas
            string query = @"INSERT INTO solicitud_vacaciones (idEmpleado, fechaInicio, fechaRetoma, motivo, estado) 
                     VALUES ({0}, {1}, {2}, {3}, 'Pendiente')";

            try
            {
                int rowsAffected = await _context.Database.ExecuteSqlRawAsync(query,
                    dto.IdEmpleado,
                    dto.FechaInicio,
                    dto.FechaRetoma,
                    dto.Motivo ?? (object)DBNull.Value);

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error en SolicitudesService - Vacaciones]: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> CrearSolicitudBeneficioAsync(SolicitudBeneficioDto dto)
        {
            string query = @"INSERT INTO solicitud_beneficios (idEmpleado, idBeneficio, observacion, estado) 
                     VALUES ({0}, {1}, {2}, 'Pendiente')";
            try
            {
                int rowsAffected = await _context.Database.ExecuteSqlRawAsync(query,
                    dto.IdEmpleado,
                    dto.IdBeneficio,
                    dto.Observacion ?? (object)DBNull.Value);

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error en SolicitudesService - Beneficios]: {ex.Message}");
                throw;
            }
        }
    }
}