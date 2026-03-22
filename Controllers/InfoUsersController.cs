using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using proyectSystemTh.DTOs;
using proyectSystemTh.Services.InfoUsers;


namespace proyectSystemTh.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InfoUsersController : ControllerBase
    {
        private readonly IInfoUsers _infoUsers;

        public InfoUsersController(IInfoUsers infoUsers)
        {
            _infoUsers = infoUsers;
        }

        // GET api/<InfoUsersController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EmpleadoDTO>> GetInfo(int id)
        {
            var result = await _infoUsers.infoEmploy(id);
            if (result == null) return BadRequest("no se encontró el usuario");
            return Ok(result);
        }

    }
}
