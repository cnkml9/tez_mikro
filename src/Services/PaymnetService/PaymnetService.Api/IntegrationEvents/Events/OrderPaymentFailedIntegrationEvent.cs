using EventBus.Base.Events;
using System;

namespace PaymnetService.Api.IntegrationEvents.Events
{
    public class OrderPaymentFailedIntegrationEvent :IntegrationEvent
    {
        public Guid OrderId { get; set; }

        public string ErrorMessage { get; }


        public OrderPaymentFailedIntegrationEvent(Guid orderId, string errorMessage)
        {
            OrderId = orderId;
            errorMessage = ErrorMessage;
        }
    }
}
