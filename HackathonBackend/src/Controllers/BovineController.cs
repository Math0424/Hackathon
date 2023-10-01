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
            var idString = User.FindFirst(ClaimTypes.Name)?.Value;
            Console.WriteLine("UserId with value " + idString);
            return long.TryParse(idString, out id);
        }

        [Authorize]
        [HttpGet("getall")]
        public async Task<IActionResult> GetAllBovine()
        {
            long userId = 0;
            if (!GetUser(ref userId))
                return BadRequest("Invalid Token");

            return Ok(await Database.GetUserBovineDetails(userId));
        }

        [Authorize]
        [HttpGet("get")]
        public async Task<IActionResult> GetBovine([FromQuery] long id)
        {
            long userId = 0;
            if (!GetUser(ref userId))
                return BadRequest("Invalid Token");

            if (!await Database.HasBovine(id))
                return BadRequest("Cannot find bovine");
            var bovine = await Database.GetBovine(id);
            if (bovine.Value.ownerId != userId)
                return BadRequest("Not the owner");

            return Ok(bovine);
        }

        [Authorize]
        [HttpPost("create")]
        public async Task<IActionResult> CreateBovine(Bovine bovine)
        {
            long userId = 0;
            if (!GetUser(ref userId))
                return BadRequest("Invalid Token");
            
            return Ok(new long[] { await Database.CreateCow(userId, bovine) });
        }

        [Authorize]
        [HttpPost("notes")]
        public async Task<IActionResult> CreateBovineNotes(BovineNotes note)
        {
            long userId = 0;
            if (!GetUser(ref userId))
                return BadRequest("Invalid Token");

            if (!await Database.HasBovine(note.bovineId))
                return BadRequest("Cannot find bovine");
            var bovine = await Database.GetBovine(note.bovineId);
            if (bovine.Value.ownerId != userId)
                return BadRequest("Not the owner");

            await Database.CreateNote(note);
            return Ok();
        }

        [Authorize]
        [HttpDelete("notes")]
        public async Task<IActionResult> DeleteBovineNotes(BovineNotes note)
        {
            long userId = 0;
            if (!GetUser(ref userId))
                return BadRequest("Invalid Token");

            if (!await Database.HasBovine(note.bovineId))
                return BadRequest("Cannot find bovine");
            var bovine = await Database.GetBovine(note.bovineId);
            if (bovine.Value.ownerId != userId)
                return BadRequest("Not the owner");

            await Database.DeleteNote(note);
            return Ok();
        }

        [Authorize]
        [HttpGet("notes")]
        public async Task<IActionResult> GetBovineNotes([FromQuery] long id)
        {
            long userId = 0;
            if (!GetUser(ref userId))
                return BadRequest("Invalid Token");

            if (!await Database.HasBovine(id))
                return BadRequest("Cannot find bovine");
            var bovine = await Database.GetBovine(id);
            if (bovine.Value.ownerId != userId)
                return BadRequest("Not the owner");

            return Ok(await Database.GetBovineNotes(id));
        }

        [Authorize]
        [HttpGet("photos")]
        public async Task<IActionResult> GetBovinePhotos([FromQuery] long id)
        {
            long userId = 0;
            if (!GetUser(ref userId))
                return BadRequest("Invalid Token");

            if (!await Database.HasBovine(id))
                return BadRequest("Cannot find bovine");
            var bovine = await Database.GetBovine(id);
            if (bovine.Value.ownerId != userId)
                return BadRequest("Not the owner");

            return Ok(await Database.GetBovinePhotos(id));
        }

        [Authorize]
        [HttpGet("weights")]
        public async Task<IActionResult> GetBovineWeights([FromQuery] long id)
        {
            long userId = 0;
            if (!GetUser(ref userId))
                return BadRequest("Invalid Token");

            if (!await Database.HasBovine(id))
                return BadRequest("Cannot find bovine");
            var bovine = await Database.GetBovine(id);
            if (bovine.Value.ownerId != userId)
                return BadRequest("Not the owner");

            return Ok(await Database.GetBovineWeights(id));
        }

        [Authorize]
        [HttpPost("weights")]
        public async Task<IActionResult> AddBovineWeights(BovineWeight weight)
        {
            long userId = 0;
            if (!GetUser(ref userId))
                return BadRequest("Invalid Token");

            if (!await Database.HasBovine(weight.bovineId))
                return BadRequest("Cannot find bovine");
            var bovine = await Database.GetBovine(weight.bovineId);
            if (bovine.Value.ownerId != userId)
                return BadRequest("Not the owner");

            await Database.AddWeight(weight);
            return Ok();
        }

        [Authorize]
        [HttpGet("conception")]
        public async Task<IActionResult> GetBovineConception([FromQuery] long id)
        {
            long userId = 0;
            if (!GetUser(ref userId))
                return BadRequest("Invalid Token");

            if (!await Database.HasBovine(id))
                return BadRequest("Cannot find bovine");
            var bovine = await Database.GetBovine(id);
            if (bovine.Value.ownerId != userId)
                return BadRequest("Not the owner");

            return Ok(await Database.GetBovineConception(id));
        }

        [Authorize]
        [HttpGet("children")]
        public async Task<IActionResult> GetBovineChildren([FromQuery] long id)
        {
            long userId = 0;
            if (!GetUser(ref userId))
                return BadRequest("Invalid Token");

            if (!await Database.HasBovine(id))
                return BadRequest("Cannot find bovine");
            var bovine = await Database.GetBovine(id);
            if (bovine.Value.ownerId != userId)
                return BadRequest("Not the owner");

            return Ok(await Database.GetBovineChildren(id));
        }

    }
}
