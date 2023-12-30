using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderService.API.Services;
using OrderService.Appliation.Abstract;
using OrderService.Appliation.Features.Commands.OrderCreate;
using OrderService.Appliation.Features.Queries.GetOrderDetailWithId;
using OrderService.Domain.AggregateModels.OrderAggregate;
using OrderService.Infrastructure.Repositories;
using System.Security.Claims;

namespace OrderService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository orderRepository;
        private readonly IBuyerRepository buyerRepository;
        private readonly IIdentityService identityService;

        public OrderController(IOrderRepository orderRepository, IBuyerRepository buyerRepository, IIdentityService identityService)
        {
            this.orderRepository = orderRepository;
            this.buyerRepository = buyerRepository;
            this.identityService = identityService;
        }

        [HttpGet("GetOrder")]
        public async Task<IActionResult> GetOrderDetailsById()
        {
            //var buyer = await buyerRepository.GetById(id)

            // var order = await orderRepository.GetByIdAsync(id, i => i.OrderItems);

            var buyerId = identityService.GetUserName().ToString();

            var order = await orderRepository.GetAll();

            var buyer = await buyerRepository.GetAll();

            var matchingBuyers = buyer.Where(u => u.Name == buyerId).ToList();

            var response = order.Where(o => matchingBuyers.Any(b => b.Id == o.BuyerId)).ToList();








            return Ok(response);



        }


    }
}
