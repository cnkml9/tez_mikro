using BasketService.API.IntegrationEvents.EventHandlers;
using BasketService.API.IntegrationEvents.Events;
using EventBus.Base.Abstraction;

namespace BasketService.API.Extensions
{
    public static  class ConfigureSubscription
    {
      
        public static void ConfigureSubscriptions(IServiceProvider serviceProvider)
        {
            var eventBus = serviceProvider.GetRequiredService<IEventBus>();
            eventBus.subscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();
        }
    }
}
