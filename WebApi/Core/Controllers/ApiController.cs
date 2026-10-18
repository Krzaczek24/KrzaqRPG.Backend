using Krzaq.Attributes.ProducesResponse;
using KrzaqRPG.Backend.WebApi.Core.Errors;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace KrzaqRPG.Backend.WebApi.Core.Controllers;

[ApiController]
[Route("api/[controller]")]
[ProducesResponse(HttpStatusCode.OK)]
[ProducesResponse(HttpStatusCode.Created)]
[ProducesResponse(HttpStatusCode.NoContent)]
[ProducesResponse<ErrorResponse>(HttpStatusCode.BadRequest)]
[ProducesResponse<ErrorResponse>(HttpStatusCode.Unauthorized)]
[ProducesResponse<ErrorResponse>(HttpStatusCode.Forbidden)]
[ProducesResponse<ErrorResponse>(HttpStatusCode.NotFound)]
[ProducesResponse<ErrorResponse>(HttpStatusCode.Conflict)]
[ProducesResponse<ErrorResponse>(HttpStatusCode.InternalServerError)]
public class ApiController : ControllerBase
{
    [NonAction]
    public virtual CreatedResult Created(object? value) => Created((string?)null, value);
}
