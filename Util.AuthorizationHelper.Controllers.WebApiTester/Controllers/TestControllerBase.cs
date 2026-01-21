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

    public class ConventionBase(ConventionBase.MyOptionsBase options) : SpecialControllerConvention<TController>(options)
    {
        public class MyOptionsBase : IMyOptions
        {
            /// <inheritdoc />
            public string? DefaultPolicyName => Const.Polici;

            /// <inheritdoc />
            public IReadOnlyDictionary<string, string>? PerActionPolicyNames => new Dictionary<string, string> { ["Get"] = Const.Polici2 };

            /// <inheritdoc />
            public bool ReplaceExistingFilters => false;
        }
    }
}