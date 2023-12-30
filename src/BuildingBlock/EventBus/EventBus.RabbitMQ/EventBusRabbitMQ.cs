using EventBus.Base;
using EventBus.Base.Events;
using Newtonsoft.Json;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;


namespace EventBus.RabbitMQ
{
    public class EventBusRabbitMQ : BaseEventBus
    {
        RabbitMQPersistentConnection persistentConnection;
        private readonly IConnectionFactory connectionFactory;
        private readonly IModel consumerChannel;

        public EventBusRabbitMQ(EventBusConfig config, IServiceProvider serviceProvider) : base(config, serviceProvider)
        {
             if(config.Connection != null)
            {
                var connJson = JsonConvert.SerializeObject(EventBusConfig, new JsonSerializerSettings()
                {
                    //hata alınmaması için eklendi
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });

             
                connectionFactory = JsonConvert.DeserializeObject<ConnectionFactory>(connJson);

                connectionFactory = new ConnectionFactory
                {
                    HostName = "c_rabbitmq"
                };



            }
            else
                connectionFactory= new ConnectionFactory
                {
                    HostName="c_rabbitmq"
                };

            Console.WriteLine("HostName: " + connectionFactory.Uri);

            persistentConnection = new RabbitMQPersistentConnection(connectionFactory,config.ConnectionRetryCoun);

            consumerChannel = CreateConsumerChannel();

            //remove subscription
            SubsManager.OnEventRemover += SubsManager_OnEventRemover;

        }

        private void SubsManager_OnEventRemover(object sender, string eventName)
        {
            eventName = ProcessEventName(eventName);

            if(!persistentConnection.ISConnected)
            {
                persistentConnection.TryConnect();
            }

            consumerChannel.QueueUnbind(queue: eventName,
                exchange: EventBusConfig.DefaultTopicName,
                routingKey: eventName);

            if(SubsManager.IsEmpty)
            {
                consumerChannel.Close();
            }


        }

        public override void Publish(IntegrationEvent @event)
        {
             if(!persistentConnection.ISConnected)
                persistentConnection.TryConnect();

            var policy = Policy.Handle<BrokerUnreachableException>()
               .Or<SocketException>()
               .WaitAndRetry(EventBusConfig.ConnectionRetryCoun, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
               {
                   // task: add logged
               });

            var eventName = @event.GetType().Name;
            eventName = ProcessEventName(eventName);

            consumerChannel.ExchangeDeclare(exchange: EventBusConfig.DefaultTopicName, type: "direct");

            //consumerChannel.QueueUnbind(queue: GetSubName(eventName),
            // exchange: EventBusConfig.DefaultTopicName,
            // routingKey: eventName);

            var message = JsonConvert.SerializeObject(@event);
            var body = Encoding.UTF8.GetBytes(message);

            policy.Execute(() =>
            {
                var properties = consumerChannel.CreateBasicProperties();
                properties.DeliveryMode = 2;

                //consumerChannel.QueueDeclare(queue: GetSubName(eventName), // Ensure queue exits while publishing
                //    durable: true,
                //    exclusive: false,
                //    autoDelete: false,
                //    arguments: null
                //    );

                //consumerChannel.QueueUnbind(queue: GetSubName(eventName),
                //   exchange: EventBusConfig.DefaultTopicName,
                //   routingKey: eventName);

                consumerChannel.BasicPublish(
                    exchange: EventBusConfig.DefaultTopicName,
                    routingKey: eventName,
                    mandatory: true,
                    basicProperties: properties,
                    body: body
                    );

            });

        }

        public override void subscribe<T, TH>()
        {
            var eventName = typeof(T).Name;
            eventName = ProcessEventName(eventName);

            //eventin daaha önceden subscribe edilip edilmediği bilgisini dönüyor.
           var subsStatus=  SubsManager.HasSubscriptionForEvent(eventName);
            if(!subsStatus)
            {
                if (!persistentConnection.ISConnected)
                {
                    persistentConnection.TryConnect();
                }

                consumerChannel.QueueDeclare(queue: GetSubName(eventName),
                    durable:true,
                    exclusive:false,
                    autoDelete:false,
                    arguments:null
                    );

                consumerChannel.QueueBind(queue: GetSubName(eventName),
                    exchange:EventBusConfig.DefaultTopicName,
                    routingKey:eventName
                    );
            }

            SubsManager.AddSubscription<T, TH>();
            StartBasicConsume(eventName);
        }

        public override void Unsubscribe<T, TH>()
        {
            SubsManager.RemoveSubscription<T, TH>();

        }

        private IModel CreateConsumerChannel()
        {
            if(!persistentConnection.ISConnected)
            {
                persistentConnection.TryConnect();
            }

            var channel = persistentConnection.CreateModel();

            channel.ExchangeDeclare(exchange: EventBusConfig.DefaultTopicName,
                                    type: "direct");

            return channel;
        }

        private void StartBasicConsume(string eventName)
        {
            if(consumerChannel != null)
            {
                var consumer = new EventingBasicConsumer(consumerChannel);
                consumer.Received += Consumer_Received;
                consumerChannel.BasicConsume(
                    queue: GetSubName(eventName),
                    autoAck: false,
                    consumer: consumer);
            }
        }

        private async void Consumer_Received(object sender, BasicDeliverEventArgs eventArgs)
        {
            var eventName = eventArgs.RoutingKey;
            eventName = ProcessEventName(eventName);

            var message = Encoding.UTF8.GetString(eventArgs.Body.Span);

            try
            {
                await ProcessEvent(eventName, message);
            }
            catch (Exception ex)
            {
                // Hata ayrıntılarını logla
                Console.WriteLine($"Hata Oluştu: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");

            }

            consumerChannel.BasicAck(eventArgs.DeliveryTag, multiple: false);
        }
    }
}
