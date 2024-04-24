using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace Basket.API.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ApiController : ControllerBase
    {

    }
}
