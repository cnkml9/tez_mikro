using Microsoft.EntityFrameworkCore;
using Mikroservice.Domain.Entities;
using Mikroservice.Persistence.EntityConfiguration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mikroservice.Persistence.Context
{
    public class CatalogDbContext: DbContext
    {
        public CatalogDbContext(DbContextOptions options) : base(options)
        {
        }


        public const string DEFAULT_SCHEMA = "catalog";

        public DbSet<CatalogItem> CatalogItems { get; set; }
        public DbSet<CatalogBrand> CatalogBrands { get; set; }
        public DbSet<CatalogType> CatalogTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //ase.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new CatalogBrandEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new CatalogItemEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new CatalogTypeEntityTypeConfiguration());

        }

    }
}
