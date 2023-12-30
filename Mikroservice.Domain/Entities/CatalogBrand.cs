using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mikroservice.Domain.Entities
{
    public class CatalogBrand
    {
        public int Id { get; set; }
        public string Brand { get; set; }
        public string ?ImgUrl { get; set; }
    }
}
