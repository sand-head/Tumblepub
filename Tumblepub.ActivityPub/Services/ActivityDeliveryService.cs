using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tumblepub.ActivityPub.Services;

internal class ActivityDeliveryService : BackgroundService
{
    private readonly ILogger<ActivityDeliveryService> _logger;

    public ActivityDeliveryService(ILogger<ActivityDeliveryService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting ActivityDeliveryService...");
        return base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            // todo: implement
            break;
        }

        _logger.LogInformation("Stopping ActivityDeliveryService...");
    }
}
