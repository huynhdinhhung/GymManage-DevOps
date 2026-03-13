public class ScheduledEmail
{
    public string To { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
    public DateTime SendTime { get; set; }
}

public class ScheduledEmailService : BackgroundService
{
    private readonly IEmailService _email;
    private readonly List<ScheduledEmail> _queue = new();

    public ScheduledEmailService(IEmailService email)
    {
        _email = email;
    }

    public void ScheduleEmail(ScheduledEmail email)
    {
        _queue.Add(email);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTime.Now;

            foreach (var item in _queue.Where(x => x.SendTime <= now).ToList())
            {
                await _email.SendEmailAsync(item.To, item.Subject, item.Body);
                _queue.Remove(item);
            }

            await Task.Delay(1000); // kiểm tra mỗi 1 giây
        }
    }
}
