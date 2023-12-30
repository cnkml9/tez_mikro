using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mikroservice.Domain.Entities;
using Mikroservice.Infrastructure;
using System.Text.Json.Serialization;
using System.Text.Json;
using Newtonsoft.Json;
using Mikroservice.Infrastructure.Concrete;
using Mikroservice.Application.Abstraction;
using Mikroservice.Application.DTOs;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using CatagolService.API.DTOs;
using Newtonsoft.Json;


namespace CatagolService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class CatalogController : ControllerBase
    {
        private readonly ICatalogService _catalogService;

        public CatalogController(ICatalogService catalogService)
        {
            _catalogService = catalogService ?? throw new ArgumentNullException(nameof(catalogService));
        }


       


        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CatalogItem>), (int)HttpStatusCode.OK )]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]

        public async Task<VM_CatalogItem> Get(int pi = 1, int ps = 10)
        {

            var items = await _catalogService.GetCatalogItemsAsync(pi, ps);

            VM_CatalogItem vm = new()
            {
                PageIndex = pi,
                PageSize = ps,
                CatalogItems = items
            };

            return vm;
        }

        [HttpGet("{typeName}")]
        public async Task<IActionResult> GetWithTypeName(string typeName, [FromQuery] ProductRequestModel requestModel)
        {
            var items = await _catalogService.GetProductWithCatalogName(typeName, requestModel.Pi, requestModel.Ps);
            return Ok(items);
        }

        [HttpGet("Brand")]
        public async Task<IActionResult> GetBrand()
        {
            var response = await _catalogService.GetCatalogBrandsAsync();
            return Ok(response);
        }

        [HttpGet("PopularProduct")]
        public async Task<IActionResult> GetPopularProducts()
        {
            var response = await _catalogService.GetPopularProductItem();
            return Ok(response);
        }


        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            try
            {
                var catalogItem = await _catalogService.GetCatalogItemByIdAsync(id);

                if (catalogItem != null)
                {
                    // Eğer catalogItem null değilse, JSON formatına çevirip Ok döndür
                   // var jsonCatalogItem = JsonConvert.SerializeObject(catalogItem);
                    return Ok(catalogItem);
                }
                else
                {
                    // Eğer catalogItem null ise, NotFound döndür
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                // Hata durumunda Internal Server Error döndür
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

    }
}
