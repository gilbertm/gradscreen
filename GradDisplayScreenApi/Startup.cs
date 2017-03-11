using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.AspNetCore.StaticFiles;

using GradDisplayScreenApi.Models;

namespace GradDisplayScreenApi
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Set the Context Connectionstring
            GraduateDbContext.ConnectionString = Configuration["Data:CeremonialProfile:ConnectionString"];
            TelepromptDbContext.ConnectionString = Configuration["Data:CeremonialProfile:ConnectionString"];
            GradConfigDbContext.ConnectionString = Configuration["Data:CeremonialProfile:ConnectionString"];
            QueueDbContext.ConnectionString = Configuration["Data:CeremonialProfile:ConnectionString"];


            // Add framework services.
            services.AddEntityFramework()
                .AddEntityFrameworkSqlServer()
                .AddDbContext<GraduateDbContext>()
                .AddDbContext<GradConfigDbContext>()
                .AddDbContext<QueueDbContext>()
                .AddDbContext<TelepromptDbContext>();

            // Add framework services.
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            // Serve my app-specific default file, if present.
            DefaultFilesOptions options = new DefaultFilesOptions();
            options.DefaultFileNames.Clear();
            options.DefaultFileNames.Add("index.html");
            app.UseDefaultFiles(options);
            app.UseStaticFiles();

            app.UseMvc();
        }
    }
}
