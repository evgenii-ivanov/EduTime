using DigitalSkynet.DotnetCore.Api.Controllers;
using Microsoft.AspNetCore.Mvc;
using System;

namespace EduTime.Api.Controllers
{
	[Route("api/[controller]")]
    public abstract class AppBaseController : BaseController<Guid>
    {
    }
}
