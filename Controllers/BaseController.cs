using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MessagingApi.Controllers
{

    [Produces("application/json")]
    [Route("api/v1/")]
    [ApiController]
    [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]

    public class BaseController : ControllerBase
    {
        
    }
}
