using System;
using System.IO;
using System.Reflection;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using ProductApi.Models;
using Azure.Storage.Queues; 

namespace ProductApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddMvc();

            string connection = Configuration["DbConnectionString"];
            services.AddDbContext<AdventureWorks2019Context>(options =>
                options.UseSqlServer(connection));

            services.AddSingleton(sp =>
            {
                var stAccConnectionString = Configuration.GetConnectionString("StorageAccount");
                return new BlobServiceClient(stAccConnectionString);
            });

            services.AddSingleton(sp =>
            {
                var stAccConnectionString = Configuration.GetConnectionString("StorageAccount");
                return new QueueClient(stAccConnectionString, "awfilequeue");
            });

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1",
                    new OpenApiInfo
                    {
                        Title = "Swagger manage products",
                        Description = "REST API for managing products",
                        Version = "v1"
                    });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);

            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API for managing products");
                c.RoutePrefix = string.Empty;
            });
        }
    }
}
