namespace IssueTracker.Api
{
    public class ConsoleNotificationService : INotificationService
    {
        public void Notify(User user, string message)
        {
            Console.WriteLine($"[NOTIFICARE] {user.Name}: {message}");
        }
    }
}
