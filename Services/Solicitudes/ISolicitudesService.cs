using proyectSystemTh.DTOs;

namespace proyectSystemTh.Services.Solicitudes
{
    public interface ISolicitudesService
    {
        Task<bool> CrearSolicitudCambioAsync(SolicitudCambioDatosDto dto);
        Task<bool> CrearSolicitudVacacionesAsync(SolicitudVacacionesDto dto);
        Task<bool> CrearSolicitudBeneficioAsync(SolicitudBeneficioDto dto);
    }
}
