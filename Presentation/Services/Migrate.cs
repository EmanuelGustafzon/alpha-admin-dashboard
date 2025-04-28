using Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Update.Internal;

namespace Presentation.Services;

internal class Migrate(IServiceProvider serviceProvider)
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    internal void UpdateDatabase()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

            try
            {
                logger.LogInformation("Applying database migrations...");
                dbContext.Database.Migrate();
                logger.LogInformation("Database migrations applied successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while migrating the database.");
            }
        }
    }
}
