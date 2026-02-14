using Microsoft.AspNetCore.Mvc;
using net_api.DTOs;
using net_api.Services;

namespace net_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            try
            {
                var products = await _productService.GetAllProductsAsync();
                return Ok(ApiResponse<List<ProductResponse>>.Success(products, "Products retrieved successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<List<ProductResponse>>.BadRequest(ex.Message));
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                if (product == null)
                {
                    return NotFound(ApiResponse<ProductResponse>.NotFound("Product not found"));
                }

                return Ok(ApiResponse<ProductResponse>.Success(product, "Product retrieved successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<ProductResponse>.BadRequest(ex.Message));
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] ProductRequest request)
        {
            try
            {
                var product = await _productService.CreateProductAsync(request);
                return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, ApiResponse<ProductResponse>.Created(product, "Product created successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<ProductResponse>.BadRequest(ex.Message));
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductRequest request)
        {
            try
            {
                var product = await _productService.UpdateProductAsync(id, request);
                if (product == null)
                {
                    return NotFound(ApiResponse<ProductResponse>.NotFound("Product not found"));
                }

                return Ok(ApiResponse<ProductResponse>.Success(product, "Product updated successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<ProductResponse>.BadRequest(ex.Message));
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                var success = await _productService.DeleteProductAsync(id);
                if (!success)
                {
                    return NotFound(ApiResponse.NotFound("Product not found"));
                }

                return Ok(ApiResponse.Success("Product deleted successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse.BadRequest(ex.Message));
            }
        }
    }
}
