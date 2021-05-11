using NATS.Client;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;


namespace Valuator
{
    public class NatsPublisher: IPublisher
    {
        public void Publish(string subject, byte[] data)
        {
            Task.Factory.StartNew(() => Produce(data, subject));
        }

        static void Produce(byte[] id, string eventName)
        {
            ConnectionFactory cf = new ConnectionFactory();
            using (IConnection c = cf.CreateConnection())
            {
                c.Publish(eventName, id);

                c.Drain();

                c.Close();
            }
        }
    }
}