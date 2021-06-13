namespace Valuator
{
    public interface IPublisher
    {
        void Send(string subject, string data);
    }

}