using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nesteo.Server.Data.Entities.Identity;

namespace Nesteo.Server.BackgroundTasks
{
    public class CreateDefaultUserTask : IHostedService
    {
        public const string DefaultUserName = "Admin";
        public const string DefaultFirstName = "Default";
        public const string DefaultLastName = "Admin";
        public const string DefaultPassword = "Admin123";

        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<CreateDefaultUserTask> _logger;

        public CreateDefaultUserTask(IServiceProvider serviceProvider, ILogger<CreateDefaultUserTask> logger)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            // Create a new scope so we can consume scoped services
            using IServiceScope scope = _serviceProvider.CreateScope();

            // Get user manager
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<UserEntity>>();

            // Check if any users exist.
            if (!await userManager.Users.AnyAsync(cancellationToken).ConfigureAwait(false))
            {
                _logger.LogInformation("Database doesn't contain any registered users. A new default user will be created.");

                try
                {
                    // Create a new default user
                    await userManager.CreateAsync(new UserEntity { UserName = DefaultUserName, FirstName = DefaultFirstName, LastName = DefaultLastName },
                        DefaultPassword).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning("Creating default user failed.", ex);
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
    }
}
