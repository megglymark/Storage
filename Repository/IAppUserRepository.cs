using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

using Storage.Models;

namespace Storage.Repository
{
    public interface IAppUserRepository
    {
        Task<AppUser> GetAppUserAsync(string userName);
        Task<IdentityResult> InsertAppUserAsync(AppUser appUser, string password);
        Task<IdentityResult> InsertAppRoleIntoAppUserAsync(AppUser appUser, string role);
    }
}