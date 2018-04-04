using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using Serilog;
using AutoMapper;

using Storage.Models;
using Storage.ViewModels;
using Storage.Helpers;

namespace Storage.Repository
{
    public static class DbInitializer
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using (var storageContext = new StorageContext(serviceProvider.GetRequiredService<DbContextOptions<StorageContext>>()))
            {
                storageContext.Database.EnsureCreated();
            }

            var appUsers = new List<AppUser>();

            //Create Regular users
            var userRegistrationViewModels = new RegistrationViewModel [] {
                new RegistrationViewModel {
                    Email = "test1@test.com",
                    Firstname = "test",
                    Lastname = "test",
                    Username = "test1",
                    Password = "testtesttest"
                },
                new RegistrationViewModel {
                    Email = "test2@test.com",
                    Firstname = "test",
                    Lastname = "test",
                    Username = "test2",
                    Password = "testtesttest"
                },
                new RegistrationViewModel {
                    Email = "test3@test.com",
                    Firstname = "test",
                    Lastname = "test",
                    Username = "test3",
                    Password = "testtesttest"
                },
                new RegistrationViewModel {
                    Email = "test4@test.com",
                    Firstname = "test",
                    Lastname = "test",
                    Username = "test4",
                    Password = "testtesttest"
                }
            };
            foreach (RegistrationViewModel registrationViewModel in userRegistrationViewModels)
            {
                var user = await EnsureUser(serviceProvider, registrationViewModel);
                appUsers.Add(user);
                await EnsureRole(serviceProvider, user, Constants.Strings.Roles.User);
            }

            //Create Admin Users
            var adminRegistrationViewModels = new RegistrationViewModel [] {
                new RegistrationViewModel {
                    Email = "admin1@test.com",
                    Firstname = "admin",
                    Lastname = "admin",
                    Username = "admin1",
                    Password = "testtesttest"
                }
            };
            foreach (RegistrationViewModel registrationViewModel in adminRegistrationViewModels)
            {
                var user = await EnsureUser(serviceProvider, registrationViewModel);
                appUsers.Add(user);
                await EnsureRole(serviceProvider, user, Constants.Strings.Roles.Admin);
            }
            
            //Create Bricabracs
            var bricabracs = new Bricabrac [] {
                new Bricabrac {
                    Name = "Thing1"
                },
                new Bricabrac {
                    Name = "Thing2"
                },
                new Bricabrac {
                    Name = "Thing3"
                }
            };
            var taskList = new List<Task>();
            foreach (AppUser appUser in appUsers)
            {
                foreach (Bricabrac bricabrac in bricabracs)
                {
                    //Because objects are pass by reference, new bricabracs need to be created each call.
                    //If a new bricabrac is not created from the array, it will try and use the same Id
                    //created from the first outer loop. 
                    var newBricabrac = new Bricabrac {Name = bricabrac.Name};
                    await EnsureBricabrac(serviceProvider, appUser, newBricabrac);
                }
            }
        }

        public static async Task<AppUser> EnsureUser(IServiceProvider serviceProvider,RegistrationViewModel registrationViewModel)
        {
            var mapper = serviceProvider.GetRequiredService<IMapper>();
            var appUserRepository = serviceProvider.GetRequiredService<IAppUserRepository>();
            var ownerRepository = serviceProvider.GetRequiredService<IOwnerRepository>();
            var user = await appUserRepository.GetAppUserAsync(registrationViewModel.Username);
            if(user == null)
            {
                user = mapper.Map<AppUser>(registrationViewModel);
                var result = await appUserRepository.InsertAppUserAsync(user, registrationViewModel.Password);
                if(!result.Succeeded)
                {  
                    return null;
                }
            }
            var owner = await ownerRepository.GetOwnerByAppUserIdAsync(user.Id);
            if(owner == null)
            {
                owner = new Owner { IdentityId = user.Id };
                var newOwner = await ownerRepository.InsertOwnerAsync(owner);
                if(newOwner == null)
                {
                    return null;
                }
            }
            return user;
        }

        public static async Task EnsureRole(IServiceProvider serviceProvider, AppUser user, string role)
        {
            var appRoleRespository = serviceProvider.GetRequiredService<IAppRoleRepository>();
            var appUserRepository = serviceProvider.GetRequiredService<IAppUserRepository>();

            await appRoleRespository.InsertAppRoleAsync(role);
            await appUserRepository.InsertAppRoleIntoAppUserAsync(user, role);
        }

        public static async Task EnsureBricabrac(IServiceProvider serviceProvider, AppUser appUser, Bricabrac bricabrac)
        {
            var ownerRepository = serviceProvider.GetRequiredService<IOwnerRepository>();
            var bricabracRepository = serviceProvider.GetRequiredService<IBricabracRepository>();
            
            var owner = await ownerRepository.GetOwnerByAppUserIdAsync(appUser.Id);

            bricabrac.Owner = owner;

            await bricabracRepository.InsertBricabracAsync(bricabrac);
        }
    }
}