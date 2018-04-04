using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Storage.Models;

namespace Storage.Repository
{
    public class StorageContext : IdentityDbContext<AppUser, AppRole, Guid>
    {
        public StorageContext(DbContextOptions<StorageContext> options) : base(options) { }
        
        public DbSet<Bricabrac> Bricabracs { get; set; }
        public DbSet<Owner> Owners { get; set; }
        public DbSet<Tote> Totes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AppUser>()
                .HasMany(au => au.Roles)
                .WithOne()
                .HasForeignKey(r => r.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<AppUser>()
                .HasMany(au => au.Logins)
                .WithOne()
                .HasForeignKey(r => r.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<AppUser>()
                .HasMany(au => au.Roles)
                .WithOne()
                .HasForeignKey(r => r.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Bricabrac>()
                .Property(b => b.ToteId)
                .HasDefaultValue(null);
        }
    }
}