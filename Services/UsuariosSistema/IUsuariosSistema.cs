using proyectSystemTh.DTOs;

namespace proyectSystemTh.Services.UsuariosSistema
{
    public interface IUsuariosSistema
    {
        // Usuarios
        Task<List<UsuarioSistemaDTO>> AllUsers();
        Task<int> AddUser(UsuarioSistemaCreateDTO usuario);
        Task<int> AddUserWithEmployer(UsuarioConEmpleadoCreateDTO dto);
        Task<int> UpdateUser(int id, UsuarioSistemaCreateDTO usuario);
        Task<int> DeleteUser(int id);
        Task<UsuarioSistemaDTO> GetUserById(int id);

        // Empleados (opcional para la interfaz)
        Task<List<EmpleadoDTO>> GetEmpleadosSinUsuario();
    }
}
