using System.IO;
using FunctionDoc.Models;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(FunctionDoc.Startup))]

namespace FunctionDoc
{
    class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            // Setup config
            var config = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory()))
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            // Get connection string from config
            //var connectionString = config.GetValue<string>("Values:DbConnectionString");
            var connectionString =
                "Server=tcp:aw-vm-sql-server.database.windows.net,1433;Initial Catalog=aw-vm-sql_2020-12-12T18-24Z;Persist Security Info=False;User ID=agnia;Password=Epam2020Create;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

            builder.Services.AddDbContext<AdventureWorksFuncContext>(
                options => options.UseSqlServer(connectionString));
        }
    }
}
