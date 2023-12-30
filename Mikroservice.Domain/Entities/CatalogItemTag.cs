using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mikroservice.Domain.Entities
{
    public class CatalogItemTag
    {
        public int CatalogItemId { get; set; }
        public CatalogItem CatalogItem { get; set; }

        public int TagId { get; set; }
        public Tag Tag { get; set; }
    }
}
