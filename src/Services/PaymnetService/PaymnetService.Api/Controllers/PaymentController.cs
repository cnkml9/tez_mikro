using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace PaymnetService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        [Authorize]
        [HttpGet]

        public async Task<IActionResult> Get()
        {
            return Ok("success");
        }
    }
}
