using OrderService.Domain.AggregateModels.BuyerAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Appliation.Abstract
{
    public interface IBuyerRepository : IGenericRepository<Buyer>
    {
    }
}
