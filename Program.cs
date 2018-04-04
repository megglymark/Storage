using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System;
using Serilog;

using Storage.Repository;

namespace Storage
{
    public class Program
    {
        public static int Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();
            var host = BuildWebHost(args);

            try
            {
                using (var scope = host.Services.CreateScope())
                {
                    var serviceProvider = scope.ServiceProvider;
                    var storageContext = serviceProvider.GetRequiredService<StorageContext>();

                    //Applying DB Migrations
                    storageContext.Database.Migrate();

                    try
                    {
                        DbInitializer.Initialize(serviceProvider).Wait();
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Error: Seeding database failed");
                        throw ex;
                    }
                    
                }

                Log.Information("Starting Web Host");
                host.Run();
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseSerilog()
                .Build();
    }
}
