using EventBus.Base.Abstraction;
using OrderService.API.IntegrationEvents.Events;

namespace OrderService.API.IntegrationEvents.EventHandlers
{
    public class OrderPaymentFailedIntegrationEventHandler : IINtegrationEventHandler<OrderPaymentFailedIntegrationEvent>
    {
        private readonly ILogger<OrderPaymentFailedIntegrationEventHandler> _logger;

        public OrderPaymentFailedIntegrationEventHandler(ILogger<OrderPaymentFailedIntegrationEventHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(OrderPaymentFailedIntegrationEvent @event)
        {

            //send fail notification (sms,email vs.)
            _logger.LogInformation($"Order Payment failed with OrderId:{@event.OrderId}, ErrorMessage: {@event.ErrorMessage} ");
            return Task.CompletedTask;
        }
    }
}
