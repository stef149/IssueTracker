namespace Issue_Tracker
{
    internal interface INotificationService
    {
        void Notify(User user, string message);
    }
}
