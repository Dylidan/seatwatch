using System.Net.Http.Headers;

public class EmailService(HttpClient http, IConfiguration config, ILogger<EmailService> logger)
{
    public async Task<bool> SendAlertAsync(PriceAlert alert, CancellationToken ct)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "https://api.resend.com/emails");
        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", config["Resend:ApiKey"]);
        request.Content = JsonContent.Create(new
        {
            from = "SeatWatch <onboarding@resend.dev>",
            to = new[] { alert.Email },
            subject = $"Price drop: {alert.Event!.Name}",
            html = $"<p>Tickets for <strong>{alert.Event.Name}</strong> are now " +
                   $"${alert.Event.LowestPrice}, at or below your target of ${alert.TargetPrice}.</p>"
        });

        try
        {
            var response = await http.SendAsync(request, ct);
            if (response.IsSuccessStatusCode)
            {
                logger.LogInformation("Email sent to {Email}", alert.Email);
                return true;
            }

            var body = await response.Content.ReadAsStringAsync(ct);
            logger.LogWarning("Email to {Email} failed: {Status} {Body}",
                alert.Email, (int)response.StatusCode, body);
            return false;
        }
        catch (HttpRequestException ex)
        {
            logger.LogWarning(ex, "Could not reach the email service");
            return false;
        }
    }
}