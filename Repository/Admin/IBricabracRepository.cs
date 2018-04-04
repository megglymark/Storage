using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Storage.Models;

namespace Storage.Repository.Admin
{
    public interface IBricabracRepository
    {
        Task<List<Bricabrac>> GetBricabracsAsync();
        Task<Bricabrac> GetBricabracAsync(Guid id);
        Task<Bricabrac> InsertBricabracAsync(Bricabrac bricabrac);
        Task<bool> UpdateBricabracAsync(Bricabrac bricabrac);
        Task<bool> DeleteBricabracAsync(Guid id);
    }
}