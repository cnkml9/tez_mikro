using EventBus.Base.Abstraction;
using OrderService.API.IntegrationEvents.Events;
using OrderService.Appliation.Abstract;

namespace OrderService.API.IntegrationEvents.EventHandlers
{
    public class OrderPaymentSuccessIntegrationEventHandler : IINtegrationEventHandler<OrderPaymentSuccessIntegrationEvent>
    {
        private readonly ILogger<OrderPaymentSuccessIntegrationEvent> _logger;
        private readonly IOrderRepository orderRepository;

        public OrderPaymentSuccessIntegrationEventHandler(ILogger<OrderPaymentSuccessIntegrationEvent> logger, IOrderRepository orderRepository)
        {
            _logger = logger;
            this.orderRepository = orderRepository;
        }

        public async   Task Handle(OrderPaymentSuccessIntegrationEvent @event)
        {
            try
            {

           
            var order = await orderRepository.GetById(@event.OrderId);

          
               await orderRepository.DeleteAsync(order);
            

            //send fail notification (sms,email vs.)
            _logger.LogInformation($"Order Payment Success with OrderId:{@event.OrderId} ");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.ToString());
            }
        }
    }
}
