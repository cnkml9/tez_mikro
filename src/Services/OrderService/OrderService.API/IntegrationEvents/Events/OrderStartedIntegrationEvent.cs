using EventBus.Base.Events;

namespace OrderService.API.IntegrationEvents.Events
{
    public class OrderStartedIntegrationEvent : IntegrationEvent
    {
        public OrderStartedIntegrationEvent(Guid orderId)
        {
            OrderId = orderId;
        }


        public Guid OrderId { get; private set; }

    }
}
