using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Storage.Models;

namespace Storage.Repository
{
    public interface IBricabracRepository
    {
        Task<List<Bricabrac>> GetBricabracsAsync(Guid ownerId);
        Task<Bricabrac> GetBricabracAsync(Guid ownerId, Guid id);
        Task<Bricabrac> InsertBricabracAsync(Bricabrac bricabrac);
        Task<bool> UpdateBricabracAsync(Bricabrac bricabrac);
        Task<bool> DeleteBricabracAsync(Guid id);
    }
}