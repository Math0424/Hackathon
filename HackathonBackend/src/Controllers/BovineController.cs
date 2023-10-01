using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace HackathonBackend.src
{
    [ApiController]
    [Route("Bovine")]
    internal class BovineController : ControllerBase
    {
    
        [Authorize]
        [HttpGet("userbovineids")]
        public async Task<IActionResult> GetUserBovineIds()
        {
            var idString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (long.TryParse(idString, out long id))
            {
                //var result = await GetUserBovineIds(id); // Assume this method is implemented
                return Ok();// result);
            }
            return BadRequest("Invalid user ID in claims");
        }
    
    
    }
}
