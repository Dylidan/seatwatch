using Microsoft.EntityFrameworkCore;

public class PriceCheckService(IServiceScopeFactory scopeFactory, ILogger<PriceCheckService> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(30));

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                await RunCheckAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Price check failed. Trying again on the next run.");
            }
        }
    }

    private async Task RunCheckAsync(CancellationToken stoppingToken)
    {
        using var scope = scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var emailService = scope.ServiceProvider.GetRequiredService<EmailService>();

        // Part 1: imitate the market by moving each price up or down by up to 10%
        var events = await db.Events.ToListAsync(stoppingToken);
        foreach (var ev in events)
        {
            var change = (decimal)(Random.Shared.NextDouble() * 0.2 - 0.1);
            ev.LowestPrice = Math.Round(ev.LowestPrice * (1 + change), 2);
            logger.LogInformation("{Name} is now ${Price}", ev.Name, ev.LowestPrice);
        }
        await db.SaveChangesAsync(stoppingToken);

        // Part 2: find alerts whose target price has been reached, and email them
        var hitAlerts = await db.PriceAlerts
            .Include(a => a.Event)
            .Where(a => !a.IsTriggered && a.Event!.LowestPrice <= a.TargetPrice)
            .ToListAsync(stoppingToken);

        foreach (var alert in hitAlerts)
        {
            alert.IsTriggered = true;
            logger.LogInformation("ALERT for {Email}: {Name} dropped to ${Price} (target ${Target})",
                alert.Email, alert.Event!.Name, alert.Event.LowestPrice, alert.TargetPrice);

            await emailService.SendAlertAsync(alert, stoppingToken);
        }
        await db.SaveChangesAsync(stoppingToken);
    }
}