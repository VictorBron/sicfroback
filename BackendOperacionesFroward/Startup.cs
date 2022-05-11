using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using BackendOperacionesFroward.Logger;
using BackendOperacionesFroward.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System;
using BackendOperacionesFroward.Shared.Utilities;

namespace BackendOperacionesFroward
{
    public class Startup
    {
        private readonly string _MyCors = "MyCors";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(options => { options.AllowEmptyInputInBodyModelBinding = true; });
            services.AddCors(options =>
            {
                options.AddPolicy(name: _MyCors, builder =>
                {
                    builder
                    .SetIsOriginAllowed(origin => new Uri(origin).Host == "localhost")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowAnyOrigin();
                });
            });
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            /*
             *  DATABASE 
            */

            ILogger logger = LoggerServiceConsole.CreateInstanceLoger("BackendOperacionesFroward");
            services.AddSingleton(logger);
            logger.WithClass("Setup");

            services.AddDbContext<PofDbContext>
                (options => options.UseSqlServer(Configuration.GetConnectionString("Db_Connection")));
            logger.WriteLine(LOG_TYPES.INFO, (Configuration.GetConnectionString("Db_Connection")));

            // Method layer access
            services.AddTransient<PofDbContext>();
            services.AddTransient<PofDbContextClient>();
            services.AddTransient<PofDbContextDriver>();
            services.AddTransient<PofDbContextHistory>();
            services.AddTransient<PofDbContextRequest>();
            services.AddTransient<PofDbContextSchedule>();
            services.AddTransient<PofDbContextUser>();
            services.AddTransient<PofDbContextVehicleType>();
            services.AddTransient<PofDbContextVehicle>();
            services.AddTransient<PofDbContextLogin>();


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, PofDbContext dbContext, ILogger logger)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseCors(_MyCors);
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            dbContext.SetUpUserConfiguration();


            IConfigurationRoot configuration = AppSettings.GetAppSettings();
            string addresDb = configuration.GetConnectionString("Db_Connection").Split(';')[0].Split('=')[1];
            logger.WriteLine(LOG_TYPES.INFO, $"CONNECTING TO DB IN ADDRESS {addresDb}");

//#if DEBUG
//            logger.WriteLine(LOG_TYPES.INFO, "BEGIN REBUILD DB");
//            dbContext.Database.EnsureDeleted();
//            dbContext.Database.EnsureCreated();
//            dbContext.InitialiceDB();
//            logger.WriteLine(LOG_TYPES.INFO, "END REBUILD DB");
//#endif
            dbContext.Database.EnsureCreated();
                
        }
    }
}
