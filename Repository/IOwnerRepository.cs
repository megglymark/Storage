using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Storage.Models;

namespace Storage.Repository
{
    public interface IOwnerRepository
    {
        Task<Owner> InsertOwnerAsync(Owner owner);
        Task<Owner> GetOwnerByAppUserIdAsync(Guid appUserId);
    }
}