namespace IssueTracker.Api
{
    public interface INotificationService
    {
        void Notify(User user, string message);
    }
}
