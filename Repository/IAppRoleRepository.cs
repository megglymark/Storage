using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

using Storage.Models;

namespace Storage.Repository
{
    public interface IAppRoleRepository
    {
        Task<IdentityResult> InsertAppRoleAsync(string role);
    }
}