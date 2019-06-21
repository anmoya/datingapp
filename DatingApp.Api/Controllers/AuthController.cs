using System.Threading.Tasks;
using DatingApp.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DatingApp.Api.Models;
using DatingApp.Api.DTOs;

namespace DatingApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;

        public AuthController(IAuthRepository repo) => _repo = repo;

        [HttpPost("register")]
        public async Task<ActionResult> Register(UserForRegisterDTO user)
        {
            // validate request

            user.Username = user.Username.ToLower();

            if(await _repo.UserExists(user.Username))
                return BadRequest("Username already exist");

            var userToCreate = new User{    Username = user.Username     };

            var createdUser = await _repo.Register(userToCreate, user.Password);

            return StatusCode(201);
        }
    }
}