using JobAI.Data.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using Microsoft.EntityFrameworkCore;
namespace JobAI.Data.Extensions
{
    public static class DatabaseExtensions
    {
        public static IHost MigrateDatabase(this IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<AppDbContext>();

                    Log.Information("🔄 Starting database migration...");

                    context.Database.Migrate();

                    Log.Information("✅ Database migration completed successfully.");
                }
                catch (Exception ex)
                {
                    Log.Fatal(ex, "❌ An error occurred while migrating the database.");
                    throw;
                }
            }

            return host;
        }
    }
}
