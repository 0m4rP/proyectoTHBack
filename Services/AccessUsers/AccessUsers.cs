using Microsoft.EntityFrameworkCore;
using proyectSystemTh.Data;
using proyectSystemTh.DTOs;
using proyectSystemTh.Models;

namespace proyectSystemTh.Services.AccessUsers
{
    public class AccessUsers : IAccessUsers
    {
        private readonly ApplicationDbContext _context;
        public AccessUsers(ApplicationDbContext context)
        {
            _context = context;
        }
        #region Login Usuario
        public async Task<AuthDTO> loginUser(LoginDTO request)
        {
            //buscar el usuario
            var usuario = await _context.UsuarioSistemas.FirstOrDefaultAsync(x => x.NombreUsuario == request.UserName);
            //verificar si existe
            if (usuario == null)
            {
                return new AuthDTO
                {
                    Message = "no se encontró el usuario",
                    Success = false
                };
            }

            bool isPasswordValid = VerifyPassword(request.UserPassword, usuario.ContrasenaUsuario);
            //verificar contraseña
            if (!isPasswordValid)
            {
                return new AuthDTO
                {
                    Message = "no coincide el usuario o contraseña",
                    Success = false
                };
            }
            //login existoso
            return new AuthDTO
            {
                Message = "se ha ingresado correctamente",
                Success = true,
                User = new UsuarioSistemaCreateDTO
                {
                    EmpleadoIdEmpleado = usuario.EmpleadoIdEmpleado,
                    NombreUsuario = usuario.NombreUsuario,
                    ContrasenaUsuario = usuario.ContrasenaUsuario,
                    RolUsuario = usuario.RolUsuario
                }
            };
        }
        #endregion

        #region Registrar usuario 
        public async Task<AuthDTO> registerUser(UsuarioSistemaCreateDTO request)
        {
            try
            {
                // Validar si el empleado existe
                var validateUser = await _context.Empleados
                    .AnyAsync(e => e.IdEmpleado == request.EmpleadoIdEmpleado);

                if (!validateUser)
                {
                    return new AuthDTO
                    {
                        Success = false,
                        Message = $"El empleado con ID {request.EmpleadoIdEmpleado} no existe"
                    };
                }

                // Validar que el empleado no tenga usuario
                var verifyUser = await _context.UsuarioSistemas
                    .AnyAsync(e => e.EmpleadoIdEmpleado == request.EmpleadoIdEmpleado);

                if (verifyUser)
                {
                    return new AuthDTO
                    {
                        Success = false,
                        Message = "El empleado ya tiene un usuario registrado"
                    };
                }

                // Validar que el nombre de usuario no exista
                var existingUsername = await _context.UsuarioSistemas
                    .AnyAsync(u => u.NombreUsuario == request.NombreUsuario);

                if (existingUsername)
                {
                    return new AuthDTO
                    {
                        Success = false,
                        Message = "El nombre de usuario ya está en uso"
                    };
                }

                // Crear nuevo usuario
                var newUser = new UsuarioSistema
                {
                    NombreUsuario = request.NombreUsuario,
                    ContrasenaUsuario = HashPassword(request.ContrasenaUsuario),
                    RolUsuario = request.RolUsuario,
                    EmpleadoIdEmpleado = request.EmpleadoIdEmpleado
                };

                // Agregar y guardar
                _context.UsuarioSistemas.Add(newUser);
                var rowsAffected = await _context.SaveChangesAsync();

                if (rowsAffected > 0)
                {
                    // Obtener datos del empleado para la respuesta
                    var empleado = await _context.Empleados
                        .FirstOrDefaultAsync(e => e.IdEmpleado == request.EmpleadoIdEmpleado);

                    return new AuthDTO
                    {
                        Success = true,
                        Message = "Usuario registrado exitosamente",
                        User = new UsuarioSistemaCreateDTO
                        {
                            EmpleadoIdEmpleado = newUser.IdUsuarioSistema,
                            NombreUsuario = newUser.NombreUsuario,
                            RolUsuario = newUser.RolUsuario,
                            ContrasenaUsuario = newUser.ContrasenaUsuario
                        },
                    };
                }
                else
                {
                    return new AuthDTO
                    {
                        Success = false,
                        Message = "No se pudo registrar el usuario"
                    };
                }
            }
            catch (Exception ex)
            {
                // Log del error
                Console.WriteLine($"Error en RegisterUser: {ex.Message}");

                return new AuthDTO
                {
                    Success = false,
                    Message = "Error interno al registrar el usuario"
                };
            }
        }
        #endregion

        #region método para el hash de contraseña
        private string HashPassword(string password)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var bytes = System.Text.Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        private bool VerifyPassword(string inputPassword, string hashedPassword)
        {
            var inputHash = HashPassword(inputPassword);
            return inputHash == hashedPassword;
        }
        #endregion

        #region Validar usuario 
        public Task<bool> UserExists(string userName)
        {
            return _context.UsuarioSistemas.AnyAsync(x => x.NombreUsuario == userName);
        }
        #endregion
    }
}
