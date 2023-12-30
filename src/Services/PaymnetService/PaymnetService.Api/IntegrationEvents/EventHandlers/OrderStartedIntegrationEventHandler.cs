using EventBus.Base.Abstraction;
using EventBus.Base.Events;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PaymnetService.Api.IntegrationEvents.Events;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;

namespace PaymnetService.Api.IntegrationEvents.EventHandlers
{
    public class OrderStartedIntegrationEventHandler:IINtegrationEventHandler<OrderStartedIntegrationEvent>
    {
        private readonly IConfiguration configuration;
        private readonly IEventBus eventBus;
        private readonly ILogger<OrderStartedIntegrationEventHandler> logger;

        public OrderStartedIntegrationEventHandler(IConfiguration configuration, IEventBus eventBus, ILogger<OrderStartedIntegrationEventHandler> logger)
        {
            this.configuration = configuration;
            this.eventBus = eventBus;
            this.logger = logger;
        }

        public Task Handle(OrderStartedIntegrationEvent @event)
        {
            string keyword = "PaymentSuccess";
            bool paymentSuccessFlag = configuration.GetValue<bool>(keyword);

            IntegrationEvent paymentEvent = paymentSuccessFlag
                ? new OrderPaymentSuccessIntegrationEvent(@event.OrderId)
                : new OrderPaymentFailedIntegrationEvent(@event.OrderId,"This is a fake error message");

            logger.LogInformation($"OrderStartedIntegrationEventHandler in PaymentService is fired with PaymentSuccess:{paymentSuccessFlag},orderId:{@event.OrderId}");

           // paymentEvent.CorrelationId = @event.CorrelationId;

           // Log.BindProperty("CorrelationId", @event.CorrelationId, false, out LogEventProperty p);

            eventBus.Publish(paymentEvent);

            return Task.CompletedTask;
        }


    }
}
