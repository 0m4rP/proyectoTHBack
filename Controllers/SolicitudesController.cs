using Microsoft.AspNetCore.Mvc;
using proyectSystemTh.DTOs;
using proyectSystemTh.Services.Solicitudes;

namespace proyectSystemTh.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SolicitudesController : ControllerBase
    {
        private readonly ISolicitudesService _solicitudesService;

        // Inyectamos la interfaz en lugar de acoplar la conexión directa aquí
        public SolicitudesController(ISolicitudesService solicitudesService)
        {
            _solicitudesService = solicitudesService;
        }

        [HttpPost("crear-solicitud")]
        public async Task<IActionResult> CrearSolicitud([FromBody] SolicitudCambioDatosDto dto)
        {
            if (dto.IdEmpleado <= 0)
                return BadRequest(new { mensaje = "El ID del empleado no es válido." });

            try
            {
                bool resultado = await _solicitudesService.CrearSolicitudCambioAsync(dto);

                if (!resultado)
                    return BadRequest(new { mensaje = "No se pudo registrar la solicitud." });

                return Ok(new { mensaje = "Solicitud de modificación registrada con éxito." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno en el servidor", error = ex.Message });
            }
        }

        [HttpPost("crear-solicitud-vacaciones")]
        public async Task<IActionResult> CrearSolicitudVacaciones([FromBody] SolicitudVacacionesDto dto)
        {
            if (dto == null || dto.IdEmpleado <= 0)
                return BadRequest("Datos de solicitud inválidos.");

            if (dto.FechaInicio >= dto.FechaRetoma)
                return BadRequest("La fecha de inicio no puede ser mayor o igual a la fecha de retorno.");

            var resultado = await _solicitudesService.CrearSolicitudVacacionesAsync(dto);

            if (resultado)
                return Ok(new { mensaje = "Solicitud de vacaciones registrada exitosamente." });

            return StatusCode(500, "No se pudo procesar el registro en la base de datos.");
        }

        [HttpPost("crear-solicitud-beneficio")]
        public async Task<IActionResult> CrearSolicitudBeneficio([FromBody] SolicitudBeneficioDto dto)
        {
            if (dto == null || dto.IdEmpleado <= 0 || dto.IdBeneficio <= 0)
                return BadRequest("Datos de solicitud de beneficio inválidos.");

            var resultado = await _solicitudesService.CrearSolicitudBeneficioAsync(dto);

            if (resultado)
                return Ok(new { mensaje = "Solicitud de beneficio registrada de forma exitosa." });

            return StatusCode(500, "No se pudo procesar el registro en la base de datos.");
        }
    }
}