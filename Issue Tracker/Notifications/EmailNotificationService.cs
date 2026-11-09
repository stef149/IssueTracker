namespace Issue_Tracker
{
    internal class EmailNotificationService : INotificationService
    {
        public void Notify(User user, string message)
        {
            Console.WriteLine($"[EMAIL catre {user.Email}] {message}");
        }
    }
}
