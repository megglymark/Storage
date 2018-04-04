using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Storage.Models;

namespace Storage.Repository
{
    public class AppUserRepository : IAppUserRepository
    {
        private readonly UserManager<AppUser> _userManager;

        public AppUserRepository(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<AppUser> GetAppUserAsync(string userName)
        {
            return await _userManager.FindByNameAsync(userName);
        }

        public async Task<IdentityResult> InsertAppUserAsync(AppUser appUser, string password)
        {
            return await _userManager.CreateAsync(appUser,password);
        }

        public async Task<IdentityResult> InsertAppRoleIntoAppUserAsync(AppUser appUser, string role)
        {
            return await _userManager.AddToRoleAsync(appUser,role);
        }
    }
}