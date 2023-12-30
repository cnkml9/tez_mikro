using EventBus.Base.Abstraction;
using MediatR;
using OrderService.Appliation.Features.Commands.OrderCreate;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using OrderService.Domain.AggregateModels.OrderAggregate;
using OrderService.Appliation.IntegrationEvents;
using OrderService.API.IntegrationEvents.Events;
using OrderService.Infrastructure.Context;
using System.Linq.Expressions;
using OrderService.Infrastructure.Repositories;
using OrderService.Appliation.Abstract;
using OrderService.Domain.AggregateModels.BuyerAggregate;

namespace OrderService.API.IntegrationEvents.EventHandlers
{
    public class OrderCreatedIntegrationEventHandler : IINtegrationEventHandler<OrderCreatedIntegrationEvent>
    {
        private readonly ILogger<OrderCreatedIntegrationEventHandler> logger;
        private readonly IOrderRepository orderRepository;
        private readonly IEventBus eventBus;
        private readonly IBuyerRepository buyerRepository;


        public OrderCreatedIntegrationEventHandler(ILogger<OrderCreatedIntegrationEventHandler> logger, IEventBus eventBus, IOrderRepository orderRepository, IBuyerRepository buyerRepository)
        {
            this.logger = logger;
            this.eventBus = eventBus;
            this.orderRepository = orderRepository;
            this.buyerRepository = buyerRepository;
        }

        public async Task Handle(OrderCreatedIntegrationEvent @event)
        {
            try
            {
                logger.LogInformation("Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})",
                @event.Id,
                typeof(Program).Namespace,
                @event);



                var createOrderCommand = new CreateOrderCommand(@event.Basket.Items,
                                @event.UserId, @event.UserName,
                                @event.City, @event.Street,
                                @event.State, @event.Country, @event.ZipCode,
                                @event.CardNumber, @event.CardHolderName, @event.CardExpiration,
                                @event.CardSecurityNumber, @event.CardTypeId,@event.BuyerId);

                //await mediator.Send(createOrderCommand);
                await Published(createOrderCommand);
                Console.WriteLine("success", createOrderCommand);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.ToString());
            }
        }



        public async Task<bool> Published(CreateOrderCommand request)
        {
            //logger.LogInformation("CreateOrderCommandHandler -> Handle method invoked");

            var addr = new Address(request.Street, request.City, request.State, request.Country, request.ZipCode);

            Guid buyerGuid = Guid.NewGuid();
            request.BuyerId = buyerGuid;

            Order dbOrder = new(request.UserName,
                addr, request.CardTypeId, request.CardNumber, request.CardSecurityNumber, request.CardHolderName, request.CardExpiration,null,request.BuyerId);

            dbOrder.Description = "Success";
            dbOrder.orderStatusId = 1;


            foreach (var orderItem in request.OrderItems)
            {
                dbOrder.AddOrderItem(orderItem.ProductId, orderItem.ProductName, orderItem.UnitPrice, orderItem.PictureUrl, orderItem.Units);
            }

            Buyer dbBuyer = new(request.UserName);

            dbBuyer.Id = dbOrder.BuyerId;

            await buyerRepository.AddAsync(dbBuyer);

            await orderRepository.AddAsync(dbOrder);
            // await orderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            //logger.LogInformation("CreateOrderCommandHandler -> dbOrder saved");

            var orderStartedIntegrationEvent = new OrderService.Appliation.IntegrationEvents.OrderStartedIntegrationEvent(request.UserName, dbOrder.Id);

            eventBus.Publish(orderStartedIntegrationEvent);

            //logger.LogInformation("CreateOrderCommandHandler -> OrderStartedIntegrationEvent fired");

            return true;
        }


    }
}
