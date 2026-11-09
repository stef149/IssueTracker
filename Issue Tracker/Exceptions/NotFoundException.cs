namespace Issue_Tracker
{
    internal class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message)
        {
        }
    }
}
