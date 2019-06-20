using System.Threading.Tasks;
using DatingApp.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly DataContext context;

        public UserController(DataContext _context) => this.context = _context;

        [HttpGet]
        public async Task<ActionResult> GetAsync()
        {
            var values = await context.Users.ToListAsync();

            return Ok(values);
        }
    }
}