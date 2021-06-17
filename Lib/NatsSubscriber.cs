using NATS.Client;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace Valuator 
{
    public class NatsSubscriber: ISubscriber<MsgHandlerEventArgs>
    {

        private readonly IConnection _connection;
        private IAsyncSubscription _subscr;
        public NatsSubscriber()
        {  
            ConnectionFactory cf = new ConnectionFactory();
            _connection = cf.CreateConnection();
        }
        public void SubscribeAsyncWithQueue(string subject, string queue, EventHandler<MsgHandlerEventArgs> handler)
        {
            _subscr = _connection.SubscribeAsync(subject, queue, handler);
        }
        public void SubscribeAsync(string subject, EventHandler<MsgHandlerEventArgs> handler)
        {
            _subscr = _connection.SubscribeAsync(subject, handler);
        }
        public void Start()
        {
            _subscr.Start();
        }

        public void Unsubscribe()
        {
            _subscr.Unsubscribe();
        }
        
        ~NatsSubscriber()
        {
            _connection.Drain();
            _connection.Close();
        }
    }
}