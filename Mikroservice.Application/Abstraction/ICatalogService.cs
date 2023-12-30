using Mikroservice.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mikroservice.Application.Abstraction
{

    public interface ICatalogService
    {
        Task<IEnumerable<CatalogItem>> GetCatalogItemsAsync(int pageSize, int pageIndex);
        Task<CatalogItem> GetCatalogItemByIdAsync(int id);
        Task<IEnumerable<CatalogBrand>> GetCatalogBrandsAsync();
        Task<IEnumerable<CatalogType>> GetCatalogTypesAsync();
        Task AddCatalogItemAsync(CatalogItem catalogItem);
        Task UpdateCatalogItemAsync(CatalogItem catalogItem);
        Task RemoveCatalogItemAsync(int id);
        Task<IEnumerable<CatalogItem>> GetProductWithCatalogName(string name, int pageSize, int pageIndex);
        Task<IEnumerable<CatalogItem>> GetPopularProductItem();


    }

}
