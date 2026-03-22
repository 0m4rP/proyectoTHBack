using proyectSystemTh.DTOs;

namespace proyectSystemTh.Services.InfoUsers
{
    public interface IInfoUsers
    {
        Task<EmpleadoDTO>infoEmploy(int id);
    }
}
