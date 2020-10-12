using kAttendance.Infrastructure.EntityFramework;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace kAttendance
{
    public class TestStartup : Startup
    {
       public TestStartup(IHostingEnvironment env) : base((IHostingEnvironment) env)
       {
       }

       public override void SetupDatabase(IServiceCollection services)
       {
          var connectionStringBuilder = new SqliteConnectionStringBuilder { DataSource = ":memory:" };
          var connectionString = connectionStringBuilder.ToString();
          var connection = new SqliteConnection(connectionString);
          services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(connection));
      }

       public override void EnsureDatabaseCreated(ApplicationDbContext context)
       {
          context.Database.OpenConnection();
          context.Database.EnsureCreated();
       }
    }
}
