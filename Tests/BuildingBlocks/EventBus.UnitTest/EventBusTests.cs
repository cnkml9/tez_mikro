using EventBus.Base;
using EventBus.Base.Abstraction;
using EventBus.Factory;
using EventBus.UnitTest.Events.EventHandlers;
using EventBus.UnitTest.Events.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using RabbitMQ.Client;

namespace EventBus.UnitTest
{
    public class EventBusTests
    {
        private ServiceCollection services;

        public EventBusTests()
        {
            this.services = new ServiceCollection();
            services.AddLogging(configure => configure.AddConsole());
        }

        [Test]
        public void subscribe_event_on_rabbirmq_test()
        {
            services.AddSingleton<IEventBus>(sp =>
            {
                EventBusConfig config = new()
                {
                    ConnectionRetryCoun = 5,
                    SubscriberClientAppNmae = "EventBus.UnitTest",
                    DefaultTopicName = "MikroserviceTezTopicName",
                    EventBusType = EventBusType.RabbitMQ,
                    EventNameSuffix = "IntegrationEvent",
                    Connection = new ConnectionFactory()
                    {
                        HostName = "localhost",
                        Port = 15672,
                        UserName = "guest",
                        Password = "guest"

                    }
                };

                return EventBusFactory.Create(config, sp);

            }
            );


            var sp = services.BuildServiceProvider();

            var eventBus = sp.GetRequiredService<IEventBus>();
            eventBus.subscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();
           // eventBus.Unsubscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();

            //Assert.Pass();
        }



        [Test]
        public void send_message_to_rabbitmq()
        {
            services.AddSingleton<IEventBus>(sp=>
            {
                return EventBusFactory.Create(GetRabbitMQConfig(), sp);
            });

            var sp = services.BuildServiceProvider();

            var eventBus = sp.GetRequiredService<IEventBus>();

            eventBus.Publish(new OrderCreatedIntegrationEvent(1));

        }

       


        private EventBusConfig GetRabbitMQConfig()
        {
            return new EventBusConfig()
            {
                ConnectionRetryCoun = 5,
                SubscriberClientAppNmae = "EventBus.UnitTest",
                DefaultTopicName = "MikroserviceTezTopicName",
                EventBusType = EventBusType.RabbitMQ,
                EventNameSuffix = "IntegrationEvent",
                //Connection=new ConnectionFactory()
                //{
                //    HostName = "localhost",
                //    Port= 5672,
                //    UserName = "guest",
                //    Password= "guest"

                //}
            };
        }

    }
}