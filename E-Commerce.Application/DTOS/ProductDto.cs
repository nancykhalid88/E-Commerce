using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.DTOS
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string description { get; set; } = default!;
        public string PictureUrl { get; set; } = default!;
        public string ProductBrand { get; set; } = default!;
        public string ProductType { get; set; } = default!;
        public decimal Price { get; set; }
    }
}
