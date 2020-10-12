using FluentValidation.AspNetCore;
using kAttendance.Infrastructure;
using kAttendance.Infrastructure.EntityFramework;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using AutoMapper;
using kAttendance.Infrastructure.Filters;
using kAttendance.Services.Interfaces;
using kAttendance.Services;
using kAttendance.Infrastructure.Mappers;

namespace kAttendance
{
   public class Startup
   {
      public Startup(IHostingEnvironment env)
      {
         var builder = new ConfigurationBuilder()
            .SetBasePath(env.ContentRootPath)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
            .AddEnvironmentVariables();
         Configuration = builder.Build();
      }

      public IConfigurationRoot Configuration { get; }

      // This method gets called by the runtime. Use this method to add services to the container.
      public void ConfigureServices(IServiceCollection services)
      {

         // Add framework services.
         services.AddMvc(setup =>
         {
            //setup.ReturnHttpNotAcceptable = true;
            setup.Filters.Add(typeof(HttpGlobalExceptionFilter));
         }).AddFluentValidation(fv=>fv.RegisterValidatorsFromAssemblyContaining<Startup>());

         AddAutoMapper(services);

         SetupDatabase(services);
        
         services.AddTransient<IGroupService, GroupService>();
         services.AddTransient<IAttendanceService, AttendanceService>();
         services.AddTransient<IPersonService, PersonService>();
         //  services.AddTransient<IPersonAttendanceService, PersonAttendanceService>();
         services.AddTransient<IExportService, ExportService>();

         services.AddScoped<ModelValidationFilter>();

         services.AddCors(options =>
         {
            options.AddPolicy("CorsPolicy",
               builder => builder.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials());
         });

      }

      private void AddAutoMapper(IServiceCollection services)
      {
         var mappingConfig = new MapperConfiguration(mc => { mc.AddProfile(new AutoMapperProfile()); });

         IMapper mapper = mappingConfig.CreateMapper();
         services.AddSingleton(mapper);
      }

      // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
      public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
      {
         app.UseCors("CorsPolicy");
         if (env.IsDevelopment())
         {
            app.UseDeveloperExceptionPage();
         }
         else
         {
            //app.UseExceptionHandler(appBuilder =>
            //{
            //   app.Run(async context =>
            //   {
            //      context.Response.StatusCode = 500;
            //      await context.Response.WriteAsync("An unexpected fault happend. Try again later.");
            //   });
            //});
         }
         loggerFactory.AddConsole(Configuration.GetSection("Logging"));
         loggerFactory.AddDebug();

         app.UseMvc();

         using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>()
            .CreateScope())
         {
            var dbContext = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
            EnsureDatabaseCreated(dbContext);
         }
      }

      public virtual void SetupDatabase(IServiceCollection services)
      {
         //var connection = @"Data Source=kAttendanceDB.db";
         services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(Configuration.GetConnectionString("Sqlite")));
      }

      public virtual void EnsureDatabaseCreated(ApplicationDbContext context)
      {
         //context.Database.OpenConnection();
         //context.Database.EnsureCreated();
         context.Database.Migrate();
         //context.Database.Migrate();
      }
   }
}
