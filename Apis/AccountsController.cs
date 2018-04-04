using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using AutoMapper;
using Storage.Models;
using Storage.ViewModels;
using Storage.Repository;


namespace Storage.Controllers
{
    [Route("api/[controller]")]
    public class AccountsController : Controller
    {
        private IMapper _mapper;
        private UserManager<AppUser> _userManager;
        private IAppUserRepository _appUserRepo;
        private IOwnerRepository _ownerRepo;

        public AccountsController(IMapper mapper,UserManager<AppUser> userManager,IOwnerRepository ownerRepository, IAppUserRepository appUserRepository)
        {
            _mapper = mapper;
            _userManager = userManager;
            _appUserRepo = appUserRepository;
            _ownerRepo = ownerRepository;
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody]RegistrationViewModel registrationViewModel)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userIdentity = _mapper.Map<AppUser>(registrationViewModel);

            var result = await _appUserRepo.InsertAppUserAsync(userIdentity, registrationViewModel.Password);

            if (!result.Succeeded) 
            {
                return BadRequest("Unable to create account");
            }

            var owner = new Owner { IdentityId = userIdentity.Id };

            var newOwner = await _ownerRepo.InsertOwnerAsync(owner);
            if(newOwner == null)
            {
                return BadRequest("Unable to create account");
            }

            return new OkObjectResult("Account created");
        }
    }
}