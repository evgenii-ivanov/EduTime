using EduTime.Foundation.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace EduTime.Api.Controllers
{
    public class VersionController : AppBaseController
    {
	    private readonly VersionOptions _versionOptions;

	    public VersionController(IOptions<VersionOptions> versionOptions)
	    {
		    _versionOptions = versionOptions.Value;
	    }

	    [HttpGet]
	    public ActionResult<VersionOptions> Get()
	    {
		    return Ok(_versionOptions);
	    }
    }
}
