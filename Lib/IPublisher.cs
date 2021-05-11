namespace Valuator
{
    public interface IPublisher
    {
        void Publish(string subject, byte[] data);
    }

}