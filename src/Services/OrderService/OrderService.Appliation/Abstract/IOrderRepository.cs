using OrderService.Domain.AggregateModels.OrderAggregate;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Appliation.Abstract
{
    public interface IOrderRepository : IGenericRepository<Order>
    {


      
    }
}
