using Microsoft.AspNetCore.Mvc;

namespace Nesteo.Server.Controllers
{
    [ApiController]
    [Produces("application/json")]
    public abstract class ApiControllerBase : Controller { }
}
