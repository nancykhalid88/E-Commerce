using E_Commerce.Application.Contracts;
using E_Commerce.Application.DTOS;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.API.Controllers
{
    public class ProductsController(IProductService productService) : ApiBaseController
    {
        #region GetAllProducts
        [HttpGet]
        [ProducesResponseType(typeof(ProductDto),StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<ProductDto>>> GetAllProducts(CancellationToken ct)
        {
            var products=await productService.GetAllProductsAsync(ct);
            return ToActionResult(products);
        }
        #endregion
        #region GetProductById
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
        [ProducesResponseType( StatusCodes.Status404NotFound)]
        public async Task <ActionResult<ProductDto>>GetProduct(int id,CancellationToken ct)
        {
            var product= await productService.GetProductAsync(id,ct);
            return ToActionResult(product);
        }

        #endregion
        #region GetAllBrands
        [HttpGet("brands")]
        public async Task<ActionResult<IReadOnlyList<BrandDto>>> GetAllBrands(CancellationToken ct)
        =>ToActionResult( await productService.GetAllBrandsAsync(ct));
        #endregion
        #region GetAllTypes
        [HttpGet("types")]
        public async Task<ActionResult<IReadOnlyList<TypeDto>>> GetAllTypes(CancellationToken ct)
        => ToActionResult(await productService.GetAllTypesAsync(ct));
        #endregion
    }
}
