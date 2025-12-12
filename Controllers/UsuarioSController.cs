using Microsoft.AspNetCore.Mvc;
using proyectSystemTh.DTOs;
using proyectSystemTh.Models;
using proyectSystemTh.Services.UsuariosSistema;

namespace proyectSystemTh.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioSController : ControllerBase
    {
        private readonly IUsuariosSistema _usuariosSistema;
        public UsuarioSController(IUsuariosSistema usuariosSistema)
        {
            _usuariosSistema = usuariosSistema;
        }



        // Obtener lista de todos los usuarios
        // GET: api/<UsuarioSController>
        // 200 || 201: correcto
        // 400: no se encontró
        // 500: problemas de conexión con el servidor
        [HttpGet("getUsers")]
        public async Task<IActionResult> getAllUsers()
        {
            var result = await _usuariosSistema.AllUsers();

            //validación de usuarios
            if(result == null)
                return BadRequest("usuarios no encontrados");

            return Ok(result);
        }

        // Obtener usuario por id
        // GET api/<UsuarioSController>/5
        // 200 || 201: correcto
        // 400: no se encontró
        // 500: problemas de conexión con el servidor
        [HttpGet("getUsers/{id}")]
        public async Task<IActionResult> getUserById(int id)
        {
            var result = await _usuariosSistema.GetUserById(id);

            if (result == null) return BadRequest("no se encontró el usuario");

            return Ok(result);
        }

        // Crear usuario o agregar usuario
        // POST api/<UsuarioSController>
        // 200 || 201: correcto
        // 400: no se encontró
        // 500: problemas de conexión con el servidor
        [HttpPost("createUser")]
        public async Task<IActionResult> createUser([FromBody] UsuarioSistemaCreateDTO usuariosSistema)
        {
            var result = await _usuariosSistema.AddUser(usuariosSistema);

            if (result == null) return BadRequest("no se pudo guardar el usuario");

            return Ok("se ha guardado el usuario");
        }

        // Actualizar usuarios
        // PUT api/<UsuarioSController>/5
        // 200 || 201: correcto
        // 400: no se encontró
        // 500: problemas de conexión con el servidor
        [HttpPut("updateUsers/{id}")]
        public async Task<IActionResult> updateUsers(int id, [FromBody] UsuarioSistemaCreateDTO usuario)
        {
            var result = await _usuariosSistema.UpdateUser(id, usuario);

            if (result > 0) return BadRequest("no se pudo actualizar");

            return Ok("se ha actualizado el usuario");
        }

        // Eliminar Usuariosz
        // DELETE api/<UsuarioSController>/5
        // 200 || 201: correcto
        // 400: no se encontró
        // 500: problemas de conexión con el servidor
        [HttpDelete("deleteUser/{id}")]
        public async Task<IActionResult> deleteUsers(int id)
        {
            var result = await _usuariosSistema.DeleteUser(id);

            if (result == null) return BadRequest("no se encontró el usuario");

            return Ok("se ha eliminado el usuario");
        }
    }
}
