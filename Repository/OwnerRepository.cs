using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Storage.Models;

namespace Storage.Repository
{
    public class OwnerRepository : IOwnerRepository
    {
        private readonly StorageContext _storageContext;

        public OwnerRepository(StorageContext storageContext)
        {
            _storageContext = storageContext;
            //_logger = loggerFactory.CreateLogger("OwnerRepository");;
        }

        public async Task<Owner> InsertOwnerAsync(Owner owner)
        {
            _storageContext.Add(owner);
            try
            {
                await _storageContext.SaveChangesAsync();
            }
            catch(System.Exception ex)
            {
                //_logger.LogError($"Error in {nameof(InsertOwnerAsync)}: " + ex.Message);
            }
            return owner;
        }

        public async Task<Owner> GetOwnerAsync(Guid id)
        {
            return await _storageContext.Owners.SingleOrDefaultAsync(o => o.Id == id);
        }

        public async Task<Owner> GetOwnerByAppUserIdAsync(Guid appUserId)
        {
            return await _storageContext.Owners.SingleOrDefaultAsync(o => o.IdentityId == appUserId);
        }
    }
}