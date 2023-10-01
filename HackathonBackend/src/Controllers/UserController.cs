using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace HackathonBackend.src
{

    [ApiController]
    [Route("user")]
    public class UserController : ControllerBase
    {

        private static string GenerateUserToken(User user)
        {
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Name, user.id.ToString())
                    }),
                Expires = DateTime.UtcNow.AddHours(5),
                SigningCredentials = new SigningCredentials(Utils.key, SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(securityToken);
            return token;
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate(User user)
        {
            var userDb = await Database.GetUser(user.username);
            if (!userDb.HasValue)
            {
                return NotFound($"User not found '{user.username}'");
            }

            if (!Utils.HashPassword(user.password, userDb.Value.salt).Equals(userDb.Value.encriptedPassword))
            {
                return Unauthorized();
            }

            return Ok(GenerateUserToken(user));
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateUser(User user)
        {
            if (await Database.HasUser(user.username))
            {
                return Conflict("User already in database");
            }
            
            if (user.password == null || user.password.Length < 5)
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
