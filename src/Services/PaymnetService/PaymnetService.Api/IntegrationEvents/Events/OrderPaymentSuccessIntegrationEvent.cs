using EventBus.Base.Events;
using System;

namespace PaymnetService.Api.IntegrationEvents.Events
{
    public class OrderPaymentSuccessIntegrationEvent:IntegrationEvent
    {
       public Guid OrderId { get; set; }
        public OrderPaymentSuccessIntegrationEvent(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}
