using BasketService.API.Core.Application.Repository;
using BasketService.API.IntegrationEvents.Events;
using EventBus.Base.Abstraction;

namespace BasketService.API.IntegrationEvents.EventHandlers
{
    public class OrderCreatedIntegrationEventHandler : IINtegrationEventHandler<OrderCreatedIntegrationEvent>
    {
        private readonly IBasketRepository _repository;
        private readonly ILogger<OrderCreatedIntegrationEvent> _logger;
        private readonly IServiceProvider _serviceProvider;


        public OrderCreatedIntegrationEventHandler(IBasketRepository repository, ILogger<OrderCreatedIntegrationEvent> logger, IServiceProvider serviceProvider)
        {
            _repository = repository;
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public async Task Handle(OrderCreatedIntegrationEvent @event)
        {
           
                _logger.LogInformation("----- Handling integration event: {IntegrationEventId} at BasketService.Api - ({@IntegrationEvent})", @event.Id, @event);
               // Console.WriteLine("----- Handling integration event: {IntegrationEventId} at BasketService.Api - ({@IntegrationEvent})", @event.Id, @event);
               // await _repository.DeleteBasketAsync(@event.UserId);
          
            
        }
    }
}
