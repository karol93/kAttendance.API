using kAttendance.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace kAttendance.Infrastructure.EntityFramework
{
   public class ApplicationDbContext : DbContext
   {
      public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
         : base((DbContextOptions) options)
      { }

      public DbSet<Group> Groups { get; set; }
      public DbSet<Attendance> Attendances { get; set; }
      public DbSet<Person> People { get; set; }
      public DbSet<PersonAttendance> PersonAttendances { get; set; }
   }


     public class ApplicationContextDbFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
       ApplicationDbContext IDesignTimeDbContextFactory<ApplicationDbContext>.CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), @"..\kAttendance\"))
                .AddJsonFile("appsettings.json")
                .Build();
            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
            var connectionString = configuration.GetConnectionString("Sqlite");
            builder.UseSqlite(connectionString);
            return new ApplicationDbContext(builder.Options);
        }
    }
}
