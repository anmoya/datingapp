using System.Threading.Tasks;
using DatingApp.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DatingApp.Api.Models;
using DatingApp.Api.DTOs;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Configuration;
using System;
using System.IdentityModel.Tokens.Jwt;

namespace DatingApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;

        public AuthController(IAuthRepository repo, IConfiguration config)
        {
            _repo = repo;
            _config = config;
        }

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

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
        {
            // Buscamos usario en repo
            var userFromRepo = await _repo.Login(userForLoginDto.Username.ToLower(), userForLoginDto.Password);

            // Si vuelve null, Unauthorized()
            if(userFromRepo == null)
                return Unauthorized();

            // Si tenemos usuario, creamos claims con id y username
            var claims = new []{
                new Claim(ClaimTypes.NameIdentifier, userFromRepo.Id.ToString()),
                new Claim(ClaimTypes.Name, userFromRepo.Username)
            };

            // Vamos a buscar el token al appsettings y lo hasheamos
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));

            // creamos credenciales firmadas, con la key y el tipo de hasheo (HMAC512)
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            // Generamos descriptor de token, con los claims, la fecha de expiracion y las credenciales firmadas
            var tokenDescriptor = new SecurityTokenDescriptor{
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            // Creamos un tokenHandler: es quien crea el token que enviaremos y que lo escribe en el response
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok(new {
                token = tokenHandler.WriteToken(token)
            });
        }
    }
}