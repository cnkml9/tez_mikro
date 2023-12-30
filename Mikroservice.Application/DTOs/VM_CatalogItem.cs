using Mikroservice.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mikroservice.Application.DTOs
{
    public class VM_CatalogItem
    {
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
        public IEnumerable<CatalogItem> CatalogItems { get; set; }
    }
}
