using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using AutoMapper;
using Serilog;

using Storage.Repository;
using Storage.Repository.Admin;
using Storage.Models;
using Storage.Auth;
using Storage.Helpers;

namespace Storage
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                              .SetBasePath(env.ContentRootPath)
                              .AddJsonFile("appsettings.json")
                              .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddAutoMapper();

            services.AddEntityFrameworkNpgsql().AddDbContext<StorageContext>(opt =>
            {
                opt.UseNpgsql(Configuration.GetConnectionString("StorageConnection"));
            });

            services.AddScoped<Storage.Repository.IBricabracRepository, Storage.Repository.BricabracRepository>();
            services.AddScoped<Storage.Repository.Admin.IBricabracRepository, Storage.Repository.Admin.BricabracRepository>();
            services.AddScoped<IOwnerRepository, OwnerRepository>();
            services.AddScoped<IAppUserRepository, AppUserRepository>();
            services.AddScoped<IAppRoleRepository, AppRoleRepository>();

            services.AddSingleton<IJwtFactory, JwtFactory>();
            var jwtAppSettingsOptions = Configuration.GetSection("JwtIssuerOptions").Get<JwtIssuerOptions>();

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtAppSettingsOptions.Key));

            services.Configure<JwtIssuerOptions>(options => 
            {
                options.Issuer = jwtAppSettingsOptions.Issuer;
                options.Audience = jwtAppSettingsOptions.Audience;
                options.SigningCredentials = new SigningCredentials(key,SecurityAlgorithms.HmacSha256);
            });

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtAppSettingsOptions.Issuer,

                ValidateAudience = true,
                ValidAudience = jwtAppSettingsOptions.Audience,

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,

                RequireExpirationTime = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            services.AddAuthentication(options => 
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(configureOptions => 
            {
                configureOptions.ClaimsIssuer = jwtAppSettingsOptions.Issuer;
                configureOptions.TokenValidationParameters = tokenValidationParameters;
                configureOptions.SaveToken = true;
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("ApiUser", policy => policy.RequireClaim(Constants.Strings.ClaimIdentitier.Rol,Constants.Strings.Claims.ApiAccess));
            });

            var builder = services.AddIdentityCore<AppUser>(o => 
            {
                o.Password.RequireDigit = false;
                o.Password.RequireLowercase = false;
                o.Password.RequireUppercase = false;
                o.Password.RequireNonAlphanumeric = false;
                o.Password.RequiredLength = 8;
                o.User.RequireUniqueEmail = true;
            });
            builder = new IdentityBuilder(builder.UserType, typeof(AppRole), builder.Services);
            builder.AddEntityFrameworkStores<StorageContext>()
                   .AddDefaultTokenProviders()
                   .AddRoleValidator<RoleValidator<AppRole>>()
                   .AddRoleManager<RoleManager<AppRole>>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
