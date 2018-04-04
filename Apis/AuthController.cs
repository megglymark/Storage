using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Serilog;

using Storage.Models;
using Storage.ViewModels;
using Storage.Repository;
using Storage.Auth;
using Storage.Helpers;


namespace Storage.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IOwnerRepository _ownerRepository;
        private readonly IJwtFactory _jwtFactory;
        private readonly JwtIssuerOptions _jwtOptions;

        public AuthController(UserManager<AppUser> userManager, IOwnerRepository ownerRepository, IJwtFactory jwtFactory, IOptions<JwtIssuerOptions> jwtOptions)
        {
            _userManager = userManager;
            _ownerRepository = ownerRepository;
            _jwtFactory = jwtFactory;
            _jwtOptions = jwtOptions.Value;
        }

        // POST api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Post([FromBody]CredentialsViewModel credentials)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var identity = await GetClaimsIdentity(credentials.Username, credentials.Password);
            if (identity == null)
            {
                return BadRequest("login_failure");
            }

            //var jwt = await Tokens.GenerateJwt(identity, _jwtFactory, credentials.Username, _jwtOptions, new JsonSerializerSettings { Formatting = Formatting.Indented });
            var response = new
            {
                id = identity.Claims.Single( c => c.Type == "id").Value,
                auth_token = await _jwtFactory.GenerateEncodedToken(credentials.Username,identity),
                expires_in = (int) _jwtOptions.ValidFor.TotalSeconds
            };
            var jsonSettings = new JsonSerializerSettings { Formatting = Formatting.Indented};

            return new OkObjectResult(JsonConvert.SerializeObject(response, jsonSettings));
        }

        private async Task<ClaimsIdentity> GetClaimsIdentity(string userName, string password)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
            {
                return await Task.FromResult<ClaimsIdentity>(null);
            }

            // get the user to verifty
            var userToVerify = await _userManager.FindByNameAsync(userName);

            if (userToVerify == null) 
            {
                return await Task.FromResult<ClaimsIdentity>(null);
            }

            var owner = await _ownerRepository.GetOwnerByAppUserIdAsync(userToVerify.Id);

            // check the credentials
            if (await _userManager.CheckPasswordAsync(userToVerify, password))
            {
                return await Task.FromResult(_jwtFactory.GenerateClaimsIdentity(userName, userToVerify.Id, owner.Id));
            }

            // Credentials are invalid, or account doesn't exist
            return await Task.FromResult<ClaimsIdentity>(null);
        }
    }
}