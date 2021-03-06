using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Serilog;

using Microsoft.Extensions.Logging;
using Storage.Models;

namespace Storage.Repository.Admin
{
    public class BricabracRepository : IBricabracRepository
    {
        private readonly StorageContext _storageContext;

        public BricabracRepository(StorageContext storageContext) 
        {
            _storageContext = storageContext;
        }

        public async Task<List<Bricabrac>> GetBricabracsAsync()
        {
            return await _storageContext.Bricabracs.ToListAsync();
        }

        public async Task<Bricabrac> GetBricabracAsync(Guid id)
        {
            return await _storageContext.Bricabracs.SingleOrDefaultAsync(b => b.Id == id);
        }

        public async Task<Bricabrac> InsertBricabracAsync(Bricabrac bricabrac)
        {
            _storageContext.Add(bricabrac);
            try
            {
                await _storageContext.SaveChangesAsync();
            }
            catch(System.Exception ex) 
            {
                //_logger.LogError($"Error in {nameof(InsertBricabracAsync)}: " + ex.Message);
            }
            return bricabrac;
        }

        public async Task<bool> UpdateBricabracAsync(Bricabrac bricabrac)
        {
            _storageContext.Bricabracs.Attach(bricabrac);
            _storageContext.Entry(bricabrac).State = EntityState.Modified;
            try
            {
                return (await _storageContext.SaveChangesAsync() > 0 ? true: false);
            }
            catch(System.Exception ex) 
            {
                //_logger.LogError($"Error in {nameof(UpdateBricabracAsync)}: " + ex.Message);
            }
            return false;
        }

        public async Task<bool> DeleteBricabracAsync(Guid id)
        {
            var bricabrac = await _storageContext.Bricabracs.SingleOrDefaultAsync(b => b.Id == id);
            _storageContext.Remove(bricabrac);
            try
            {
                return (await _storageContext.SaveChangesAsync() > 0 ? true: false);
            }
            catch(System.Exception ex) 
            {
                //_logger.LogError($"Error in {nameof(DeleteBricabracAsync)}: " + ex.Message);
            }
            return false;
        }
    }
}