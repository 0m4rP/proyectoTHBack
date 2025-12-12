
using Microsoft.EntityFrameworkCore;
using proyectSystemTh.Data;
using proyectSystemTh.DTOs;
using proyectSystemTh.Models;

namespace proyectSystemTh.Services.UsuariosSistema
{
    public class UsuariosSistema : IUsuariosSistema
    {
        private readonly ApplicationDbContext _context;

        public UsuariosSistema(ApplicationDbContext context)
        {
            _context = context;
        }

        #region Listar todos los usuarios
        public async Task<List<UsuarioSistemaDTO>> AllUsers()
        {
            //traer lista
            return await _context.UsuarioSistemas
                .Include(u => u.EmpleadoIdEmpleadoNavigation)
                .Select(u => new UsuarioSistemaDTO
                {
                    IdUsuarioSistema = u.IdUsuarioSistema,
                    NombreUsuario = u.NombreUsuario,
                    RolUsuario = u.RolUsuario,
                    EmpleadoIdEmpleado = u.EmpleadoIdEmpleado,
                    NombreEmpleado = u.EmpleadoIdEmpleadoNavigation.NombreEmpleado,
                    ApellidoEmpleado = u.EmpleadoIdEmpleadoNavigation.ApellidoEmpleado,
                    CorreoEmpleado = u.EmpleadoIdEmpleadoNavigation.Correo,
                    CargoEmpleado = u.EmpleadoIdEmpleadoNavigation.Cargo
                }).ToListAsync();
        }
        #endregion

        #region Obtener empleados que no tengan usuarios
        public async Task<List<EmpleadoDTO>> GetEmpleadosSinUsuario()
        {
            var empleadosConUsuario = await _context.UsuarioSistemas
                .Select(u => u.EmpleadoIdEmpleado)
                .ToListAsync();

            return await _context.Empleados
                .Include(e => e.DepartamentoIdDepartamentoNavigation)
                .Where(e => !empleadosConUsuario.Contains(e.IdEmpleado) && e.EstadoEmpleado == 1)
                .Select(e => new EmpleadoDTO
                {
                    IdEmpleado = e.IdEmpleado,
                    NombreEmpleado = e.NombreEmpleado,
                    ApellidoEmpleado = e.ApellidoEmpleado,
                    Correo = e.Correo,
                    Cargo = e.Cargo,
                    FechaContrato = e.FechaContrato,
                    EstadoEmpleado = e.EstadoEmpleado,
                    DepartamentoIdDepartamento = e.DepartamentoIdDepartamento,
                    NombreDepartamento = e.DepartamentoIdDepartamentoNavigation.NombreDepartamento
                })
                .ToListAsync();
        }
        #endregion

        #region Traer usuario por id
        public async Task<UsuarioSistemaDTO> GetUserById(int id)
        {
            var usuario = await _context.UsuarioSistemas.Include(u => u.IdUsuarioSistema).FirstOrDefaultAsync();

            if (usuario == null) return null;

            //mapear a DTO
            return new UsuarioSistemaDTO
            {
                IdUsuarioSistema = usuario.IdUsuarioSistema,
                NombreUsuario = usuario.NombreUsuario,
                RolUsuario = usuario.RolUsuario,
                EmpleadoIdEmpleado = usuario.EmpleadoIdEmpleado,
                NombreEmpleado = usuario.EmpleadoIdEmpleadoNavigation?.NombreEmpleado
            };
        }
        #endregion

        #region agregar/crear Usuarios
        public async Task<int> AddUser(UsuarioSistemaCreateDTO usuarioDTO)
        {
            //validar si el usuario existe
            var validateUser = await _context.Empleados.AnyAsync(e => e.IdEmpleado == usuarioDTO.EmpleadoIdEmpleado);

            if(!validateUser) throw new ArgumentException($"El empleado con ID {usuarioDTO.EmpleadoIdEmpleado} no existe");

            //validar que el empleado no tenga usuario
            var verifyUser = await _context.UsuarioSistemas.AnyAsync(e => e.EmpleadoIdEmpleado == usuarioDTO.EmpleadoIdEmpleado);

            if (verifyUser) throw new ArgumentException("el empleado ya tiene usuario");

            //campos a llenar al crear el usuario
            var newUser = new UsuarioSistema
            {
                NombreUsuario = usuarioDTO.NombreUsuario,
                ContrasenaUsuario = HashPassword(usuarioDTO.ContrasenaUsuario),
                RolUsuario = usuarioDTO.RolUsuario,
                EmpleadoIdEmpleado = usuarioDTO.EmpleadoIdEmpleado,
            };

            //se agrega y se guardan cambios en la BD
            _context.UsuarioSistemas.Add(newUser);
            return await _context.SaveChangesAsync();
        }
        #endregion

        #region Crear usuario y empleado juntos

        public async Task<int> AddUserWithEmployer(UsuarioConEmpleadoCreateDTO dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Crear empleado
                var empleado = new Empleado
                {
                    NombreEmpleado = dto.NombreEmpleado,
                    ApellidoEmpleado = dto.ApellidoEmpleado,
                    FechaNacimiento = dto.FechaNacimiento,
                    Correo = dto.Correo ?? $"{dto.NombreUsuario.ToLower()}@empresa.com",
                    Cargo = dto.Cargo,
                    FechaContrato = dto.FechaContrato,
                    EstadoEmpleado = 1, // Activo
                    DepartamentoIdDepartamento = dto.DepartamentoIdDepartamento
                };

                _context.Empleados.Add(empleado);
                return await _context.SaveChangesAsync();

                // Crear usuario
                var usuario = new UsuarioSistema
                {
                    NombreUsuario = dto.NombreUsuario,
                    ContrasenaUsuario = HashPassword(dto.ContrasenaUsuario),
                    RolUsuario = dto.RolUsuario,
                    EmpleadoIdEmpleado = empleado.IdEmpleado
                };

                _context.UsuarioSistemas.Add(usuario);
                return await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return usuario.IdUsuarioSistema;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        #endregion

        #region Actualizar el usuario
        public async Task<int> UpdateUser(int id, UsuarioSistemaCreateDTO usuario)
        {
            //encontrar el id
            var findUser = await _context.UsuarioSistemas.FirstOrDefaultAsync(u => u.IdUsuarioSistema == id);

            //validar si existe
            if (findUser == null) return 0;

            //actualizar campos
            findUser.NombreUsuario = usuario.NombreUsuario;
            findUser.RolUsuario = usuario.RolUsuario;

            _context.UsuarioSistemas.Update(findUser);
            return await _context.SaveChangesAsync();
        }
        //método para hacer hash de la contraseña
        private string HashPassword(string password)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var bytes = System.Text.Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
        #endregion

        #region eliminar usuario
        public async Task<int> DeleteUser(int id)
        {
            //se busca el usuario
            var findUser = await _context.UsuarioSistemas.FirstOrDefaultAsync(u => u.IdUsuarioSistema == id);

            //validación si existe
            if (findUser == null) return 0;

            //se eliminar usuario encontrado y guardar cambios en tabla
            _context.UsuarioSistemas.Remove(findUser);
            return await _context.SaveChangesAsync();
        }
        #endregion
    }
}
