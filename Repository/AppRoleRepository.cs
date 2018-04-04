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
    public class AppRoleRepository : IAppRoleRepository
    {
        private readonly RoleManager<AppRole> _roleManager;
        public AppRoleRepository(RoleManager<AppRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<IdentityResult> InsertAppRoleAsync(string role)
        {
            IdentityResult identityResult = null;
            if(!await _roleManager.RoleExistsAsync(role))
            {
                identityResult = await _roleManager.CreateAsync(new AppRole { Name = role});
            }
            return identityResult;
        }
    }
}