using proyectSystemTh.DTOs;

namespace proyectSystemTh.Services.AccessUsers
{
    public interface IAccessUsers
    {
        Task<AuthDTO> registerUser(UsuarioSistemaCreateDTO request);
        Task<AuthDTO> loginUser(LoginDTO request);
        Task<bool> UserExists(string userName);
    }
}
