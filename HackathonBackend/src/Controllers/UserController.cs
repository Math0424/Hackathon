using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HackathonBackend.src.Structs;

namespace HackathonBackend.src
{

    [ApiController]
    [Route("user")]
    public class UserController : ControllerBase
    {

        [HttpPost("create")]
        public async Task<IActionResult> CreateUser(User user)
        {
            var userDb = await Database.GetUser(user.username);
            if (userDb.HasValue)
            {
                return Conflict("User already exsists");
            }

            if (user.password == null || user.password.Length < 5)
            {
                return BadRequest("Invalid password");
            }

            await Database.CreateUser(user);
            return Ok();
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetUsers()
        {
            Console.WriteLine("Get all users");
            return Ok(await Database.GetUsers());
        }



    }
}
