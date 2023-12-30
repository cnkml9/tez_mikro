using Mikroservice.Application.Abstraction;
using Mikroservice.Application.DTOs;
using Mikroservice.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Mikroservice.Persistence.Context;

namespace Mikroservice.Infrastructure.Concrete
{
    public class CatalogService : ICatalogService
    {
        private readonly IRepository<CatalogItem> _catalogItemRepository;
        private readonly IRepository<CatalogBrand> _catalogBrandRepository;
        private readonly IRepository<CatalogType> _catalogTypeRepository;
        readonly CatalogDbContext _context;

        public CatalogService(
            IRepository<CatalogItem> catalogItemRepository,
            IRepository<CatalogBrand> catalogBrandRepository,
            IRepository<CatalogType> catalogTypeRepository,
            CatalogDbContext context)
        {
            _catalogItemRepository = catalogItemRepository ?? throw new ArgumentNullException(nameof(catalogItemRepository));
            _catalogBrandRepository = catalogBrandRepository ?? throw new ArgumentNullException(nameof(catalogBrandRepository));
            _catalogTypeRepository = catalogTypeRepository ?? throw new ArgumentNullException(nameof(catalogTypeRepository));
            _context = context;
        }

        public async Task<IEnumerable<CatalogItem>> GetCatalogItemsAsync(int pageSize, int pageIndex)
        {
            var allItems = await _catalogItemRepository.GetAllAsync();
                

            //var product = _productReadRepository.GetAll(false)
            //   //.Include(p => p.productImageFiles)
            //   .Select(p => new
            //   {
            //       p.Id,
            //       p.Name,
            //       p.Stock,
            //       p.Price,
            //       p.CreateDate,
            //       p.UpdatedDate,
            //       p.productImageFiles
            //   }).Skip(request.Page * request.Size).Take(request.Size).ToList();

            var items = allItems.Skip(pageSize * pageIndex).Take(pageSize).ToList();

            return items;


        }

        public async Task<IEnumerable<CatalogItem>> GetProductWithCatalogName(string name, int pageSize, int pageIndex)
        {
            // Assume GetAllAsync returns IQueryable<CatalogItem>
            var itemsQueryable = _context.CatalogItems;

            // Eager load CatalogType before calling ToList()
            var items = await itemsQueryable
                .Include(item => item.CatalogType)
                .Where(item => item.CatalogType != null && item.CatalogType.Type == name).Skip(pageSize * pageIndex).Take(pageSize)
                .ToListAsync();

            return items;
        }

        public async Task<IEnumerable<CatalogItem>> GetPopularProductItem()
        {
            var itemsQueryable = await _context.CatalogItems.ToListAsync();
            return itemsQueryable;

        }

        public async Task<CatalogItem> GetCatalogItemByIdAsync(int id)
        {
            var items = _context.CatalogItems;

            var response = await items.Include(items => items.CatalogType).Include(i => i.CatalogBrand).ToListAsync();

            var res =   response.FirstOrDefault(i=>i.Id== id);

            return res;


        }

        public async Task<IEnumerable<CatalogBrand>> GetCatalogBrandsAsync()
        {
            var items = await _catalogBrandRepository.GetAllAsync();
            var response =  items.Take(4).ToList();

            return response;

        }

        public async Task<IEnumerable<CatalogType>> GetCatalogTypesAsync()
        {
            return await _catalogTypeRepository.GetAllAsync();
        }

     

        public async Task AddCatalogItemAsync(CatalogItem catalogItem)
        {
            await _catalogItemRepository.AddAsync(catalogItem);
        }

        public async Task UpdateCatalogItemAsync(CatalogItem catalogItem)
        {
            _catalogItemRepository.Update(catalogItem);
        }

        public async Task RemoveCatalogItemAsync(int id)
        {
            var catalogItem = await _catalogItemRepository.GetByIdAsync(id);
            if (catalogItem != null)
            {
                _catalogItemRepository.Remove(catalogItem);
            }
        }
    }
}
