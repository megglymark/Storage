using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Serilog;
using Storage.Models;
using Storage.Repository;
using Storage.Helpers;


// TODO: add owner filters to all bricabrac repos

namespace Storage.Apis
{
    [Route("api/[controller]")]
    [Authorize(Policy = "ApiUser")]
    public class BricabracsController : Controller
    {
        private readonly IBricabracRepository _bricabracReposistory;
        private readonly IOwnerRepository _ownerRepository;
        private readonly UserManager<AppUser> _userManager;
        private readonly Guid _ownerId;

        public BricabracsController(IBricabracRepository bricabracRepository, IOwnerRepository ownerRepository, UserManager<AppUser> userManager)
        {
            _bricabracReposistory = bricabracRepository;
            _ownerRepository = ownerRepository;
            _userManager = userManager;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<Bricabrac>), 200)]
        [ProducesResponseType(typeof(List<Bricabrac>), 404)]
        public async Task<ActionResult> Bricabracs()
        {
            var ownerId = new Guid(User.FindFirst(Constants.Strings.ClaimIdentitier.OwnerId).Value);
            var bricabracs = await _bricabracReposistory.GetBricabracsAsync(ownerId);
            if(bricabracs == null)
            {
                return NotFound();
            }
            return Ok(bricabracs);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Bricabrac), 200)]
        [ProducesResponseType(typeof(Bricabrac), 404)]
        public async Task<ActionResult> Bricabrac(Guid id)
        {
            var ownerId = new Guid(User.FindFirst(Constants.Strings.ClaimIdentitier.OwnerId).Value);
            var bricabrac = await _bricabracReposistory.GetBricabracAsync(ownerId, id);
            if(bricabrac == null)
            {
                return NotFound();
            }
            return Ok(bricabrac);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Bricabrac), 201)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<ActionResult> PostBricabrac([FromBody]Bricabrac bricabrac)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }
            var ownerId = new Guid(User.FindFirst(Constants.Strings.ClaimIdentitier.OwnerId).Value);

            var userId = new Guid(User.FindFirst(Constants.Strings.ClaimIdentitier.Id).Value);
            var owner = await _ownerRepository.GetOwnerByAppUserIdAsync(userId);
            bricabrac.Owner = owner;

            var newBricabrac = await _bricabracReposistory.InsertBricabracAsync(bricabrac);
            if(newBricabrac == null)
            {
                return BadRequest("Unable to insert bricabrac");
            }
            return CreatedAtRoute("GetBricabracsRoute", new { id = newBricabrac.Id}, newBricabrac);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(bool), 200)]
        [ProducesResponseType(typeof(bool), 400)]
        public async Task<ActionResult> PutBricabrac(Guid id,[FromBody]Bricabrac newBricabrac)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }
            var status = false;

            var ownerId = new Guid(User.FindFirst(Constants.Strings.ClaimIdentitier.OwnerId).Value);
            var oldBricabrac = await _bricabracReposistory.GetBricabracAsync(ownerId,id);
            if(oldBricabrac != null)
            {
                status = await _bricabracReposistory.UpdateBricabracAsync(newBricabrac);
            }

            if(!status)
            {
                return BadRequest("Unable to update bricabrac");
            }
            return Ok(status);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(bool), 200)]
        [ProducesResponseType(typeof(bool), 404)]
        public async Task<ActionResult> DeleteBricabrac(Guid id)
        {
            var status = false;

            var ownerId = new Guid(User.FindFirst(Constants.Strings.ClaimIdentitier.OwnerId).Value);
            var bricabrac = await _bricabracReposistory.GetBricabracAsync(ownerId,id);
            if(bricabrac != null)
            {
                status = await _bricabracReposistory.DeleteBricabracAsync(id);
            }

            if(!status)
            {
                return BadRequest("Unable to delete bricabrac");
            }
            return Ok(status);
        }
    }
}