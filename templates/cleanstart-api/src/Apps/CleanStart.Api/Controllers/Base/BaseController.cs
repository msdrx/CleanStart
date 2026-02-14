using System.Net;
using CleanStart.Shared.Api.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace CleanStart.Api.Controllers.Base;

[ProducesResponseType(typeof(IApiResponse), (int)HttpStatusCode.BadRequest)]
[ProducesResponseType(typeof(IApiResponse), (int)HttpStatusCode.InternalServerError)]
[Route("api/[controller]")]
[ApiController]
public abstract class BaseController : ControllerBase
{
}

