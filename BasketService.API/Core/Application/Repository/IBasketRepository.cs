using BasketService.API.Core.Domain.Models;

namespace BasketService.API.Core.Application.Repository
{
    public interface IBasketRepository
    {
        Task<CustomerBasket> GetBasketAsync(string customerId);

        IEnumerable<string> GetUsers();

        Task<CustomerBasket> UpdateBasketAsync(CustomerBasket basket);

        Task<bool> DeleteBasketAsync(string id);
        Task RemoveItemAsync(string buyerId, string itemId);
    }
}
