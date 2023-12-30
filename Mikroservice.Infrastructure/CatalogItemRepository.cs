using Microsoft.EntityFrameworkCore;
using Mikroservice.Domain.Entities;
using Mikroservice.Persistence.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mikroservice.Infrastructure
{
    public class CatalogRepository
    {
        private readonly CatalogDbContext _context;

        public CatalogRepository(CatalogDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        //public async Task<List<CatalogItem>> GetCatalogItemsAsync()
        //{
        //    return await _context.CatalogItems.ToListAsync();
        //}

        //public async Task<CatalogItem> GetCatalogItemByIdAsync(int id)
        //{
        //    return await _context.CatalogItems
        //       .Include(item => item.CatalogItemTags)
        //           .ThenInclude(itemTag => itemTag.Tag)
        //       .SingleOrDefaultAsync(item => item.Id == id);
        //}

        //public async Task AddCatalogItemAsync(CatalogItem catalogItem)
        //{
        //    _context.CatalogItems.Add(catalogItem);
        //    await _context.SaveChangesAsync();
        //}

        // Diğer CRUD işlemleri buraya eklenebilir
    }
}
