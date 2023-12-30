using EventBus.Base.Events;
using System;

namespace PaymnetService.Api.IntegrationEvents.Events
{
    public class OrderStartedIntegrationEvent:IntegrationEvent
    {
        public Guid OrderId { get; set; }

        public OrderStartedIntegrationEvent()
        {
            
        }

        public OrderStartedIntegrationEvent(Guid id)
        {
            OrderId = id;
        }
    }
}
