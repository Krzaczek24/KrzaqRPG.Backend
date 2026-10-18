using KrzaqRPG.Backend.WebApi.Core.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace KrzaqRPG.Backend.WebApi.Controllers;

[Route("index")]
public class IndexController : ApiController
{
    [HttpGet("/")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public RedirectResult RedirectToJson() => Redirect(Program.ENDPOINT);
}
