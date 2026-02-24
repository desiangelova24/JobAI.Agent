using JobAI.Data.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace JobAI.Data.Initializers
{
    public class DatabaseInitializer(IServiceProvider serviceProvider) : IHostedService
    {
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            Log.Information("🚀 [DatabaseInitializer] Check for new migrations...");

            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            try
            {
                await context.Database.MigrateAsync(cancellationToken);
                Log.Information("✅ [DatabaseInitializer] The database is ready.");
            }
            catch (Exception ex)
            {
                Log.Fatal("❌ [DatabaseInitializer] Error preparing database: {Message}", ex.Message);
                throw;
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
