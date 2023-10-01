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
    [Route("bovine")]
    public class BovineController : ControllerBase
    {
    
        [NonAction]
        public bool GetUser(ref long id)
        {
            var idString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return long.TryParse(idString, out id);
        }

        [Authorize]
        [HttpGet("getallbovine")]
        public async Task<IActionResult> GetAllBovine()
        {
            long userId = 0;
            if (!GetUser(ref userId))
                return BadRequest("Invalid Token");

            return Ok();
        }

        [Authorize]
        [HttpGet("getbovine")]
        public async Task<IActionResult> GetBovine([FromQuery] long id)
        {
            long userId = 0;
            if (!GetUser(ref userId))
                return BadRequest("Invalid Token");

            return Ok();
        }

        [Authorize]
        [HttpGet("bovinenotes")]
        public async Task<IActionResult> GetBovineNotes([FromQuery] long id)
        {
            long userId = 0;
            if (!GetUser(ref userId))
                return BadRequest("Invalid Token");

            return Ok();
        }

        [Authorize]
        [HttpGet("bovinephotos")]
        public async Task<IActionResult> GetBovinePhotos([FromQuery] long id)
        {
            long userId = 0;
            if (!GetUser(ref userId))
                return BadRequest("Invalid Token");

            return Ok();
        }

        [Authorize]
        [HttpGet("bovineweights")]
        public async Task<IActionResult> GetBovineWeights([FromQuery] long id)
        {
            long userId = 0;
            if (!GetUser(ref userId))
                return BadRequest("Invalid Token");

            return Ok();
        }

        [Authorize]
        [HttpGet("bovineconception")]
        public async Task<IActionResult> GetBovineConception([FromQuery] long id)
        {
            long userId = 0;
            if (!GetUser(ref userId))
                return BadRequest("Invalid Token");

            return Ok();
        }

        [Authorize]
        [HttpGet("bovinechildren")]
        public async Task<IActionResult> GetBovineChildren([FromQuery] long id)
        {
            long userId = 0;
            if (!GetUser(ref userId))
                return BadRequest("Invalid Token");

            return Ok();
        }

    }
}
