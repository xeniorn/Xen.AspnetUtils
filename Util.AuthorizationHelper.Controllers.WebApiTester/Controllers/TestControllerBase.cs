using Microsoft.AspNetCore.Mvc;
using Util.AuthorizationHelper.Controllers.Controllers;

namespace Util.AuthorizationHelper.Controllers.WebApiTester.Controllers;

[Route("[controller]/[action]")]
[ApiController]
public class TestControllerBase<TController> : ControllerBase
    where TController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok("Yes! From " + GetType().Name);
    }

    public class ConventionBase(ConventionBase.MyOptionsBase options, ILogger? logger) : SpecialControllerConvention<TController>(options, logger)
    {
        public class MyOptionsBase : IMyOptions
        {
            /// <inheritdoc />
            public string? DefaultPolicyName => Const.DefaultPerControllerPolicy;

            /// <inheritdoc />
            public IReadOnlyDictionary<string, string>? PerActionPolicyNames => new Dictionary<string, string> { ["Get"] = Const.DefaultPerActionPolicy };

            /// <inheritdoc />
            public bool ReplaceExistingFilters => false;
        }
    }
}