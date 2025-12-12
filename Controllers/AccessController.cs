using Microsoft.AspNetCore.Mvc;
using proyectSystemTh.DTOs;
using proyectSystemTh.Services.AccessUsers;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace proyectSystemTh.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccessController : ControllerBase
    {
        private readonly IAccessUsers _context;
        public AccessController(IAccessUsers context)
        {
            _context = context;
        }

        // POST api/<AccessUserController>
        [HttpPost("login")]
        public async Task<ActionResult<AuthDTO>> LoginUser(LoginDTO request)
        {
            if (string.IsNullOrEmpty(request.UserName) || string.IsNullOrEmpty(request.UserPassword))
            {
                return BadRequest("es requerido el llenado de los campos");
            }

            var result = await _context.loginUser(request);

            if (result == null)
                return BadRequest(result);

            return Ok(result);
        }

        // POST api/<AccessUserController>
        [HttpPost("register")]
        public async Task<ActionResult<AuthDTO>> RegisterUser(UsuarioSistemaCreateDTO request)
        {
            if (string.IsNullOrEmpty(request.NombreUsuario) || string.IsNullOrEmpty(request.ContrasenaUsuario))
                return BadRequest("el llenado de datos es importante");

            var result = await _context.registerUser(request);

            if (result == null)
                return BadRequest(result);

            return Ok(result);
        }

        // GET api/<AccessUserController>
        [HttpGet("verifyUser/{userName}")]
        public async Task<ActionResult<AuthDTO>> verifyUser(string userName)
        {
            var result = await _context.UserExists(userName);

            return Ok(new { exists = result });
        }
    }
}
