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
        public void Send(string subject, string data)
        {
            Task.Factory.StartNew(() => Produce(data, subject));
        }

        static void Produce(string id, string eventName)
        {
            ConnectionFactory cf = new ConnectionFactory();
            using (IConnection c = cf.CreateConnection())
            {
                c.Publish(eventName, Encoding.UTF8.GetBytes(id));

                c.Drain();

                c.Close();
            }
        }
    }
}