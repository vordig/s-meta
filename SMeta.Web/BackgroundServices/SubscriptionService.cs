namespace SMeta.Web.BackgroundServices;

public class SubscriptionService : BackgroundService
{
    private readonly PeriodicTimer _periodicTimer = new(TimeSpan.FromMinutes(1));
    private readonly ILogger<SubscriptionService> _logger;

    public SubscriptionService(ILogger<SubscriptionService> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (await _periodicTimer.WaitForNextTickAsync(stoppingToken) && !stoppingToken.IsCancellationRequested) 
        {
            _logger.LogInformation("Subscription check executed");
        }
    }
}
