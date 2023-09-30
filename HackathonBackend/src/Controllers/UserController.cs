using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackathonBackend.src
{

    [ApiController]
    [Route("user")]
    public class UserController : ControllerBase
    {

        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateUser(User user)
        {
            if (await Database.HasUser(user.username))
            {
                return Conflict("User already in database");
            }

            if (user.password == null || user.password.Length <= 5)
            {
                return BadRequest($"Invalid password '{user.username}' '{user.password}'");
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
