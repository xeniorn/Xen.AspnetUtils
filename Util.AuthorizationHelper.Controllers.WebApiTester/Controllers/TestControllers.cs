using Microsoft.Extensions.Options;

namespace Util.AuthorizationHelper.Controllers.WebApiTester.Controllers;

public class ControllerA : TestControllerBase<ControllerA>
{
    public class Convention(IOptions<Convention.MyOptions> options, ILogger<Convention> logger) : ConventionBase(options.Value, logger)
    {
        public class MyOptions : MyOptionsBase
        {
        }
    }
}

public class ControllerB : TestControllerBase<ControllerB>
{
    public class Convention(IOptions<Convention.MyOptions> options, ILogger<Convention> logger) : ConventionBase(options.Value, logger)
    {
        public class MyOptions : MyOptionsBase
        {
        }
    }
}

public class ControllerC : TestControllerBase<ControllerC>
{
    public class Convention(IOptions<Convention.MyOptions> options, ILogger<Convention> logger) : ConventionBase(options.Value, logger)
    {
        public class MyOptions : MyOptionsBase
        {
        }
    }
}

//[Route("[controller]/[action]")]
//[ApiController]
//public class ControllerB : ControllerBase
//{
//    [HttpGet]
//    public IActionResult Get()
//    {
//        return Ok("Yes! From " + GetType().Name);
//    }

//    public class Convention(IOptions<Convention.MyOptions> options) : SpecialControllerConvention<ControllerB>(options.Value)
//    {
//        public class MyOptions : IMyOptions
//        {
//            /// <inheritdoc />
//            public string? DefaultPolicyName => Const.Polici;

//            /// <inheritdoc />
//            public IReadOnlyDictionary<string, string>? PerActionPolicyNames => new Dictionary<string, string> { ["Get"] = Const.Polici2 };

//            /// <inheritdoc />
//            public bool ReplaceExistingFilters => false;
//        }
//    }
//}

//[Route("[controller]/[action]")]
//[ApiController]
//public class ControllerC : ControllerBase
//{
//    [HttpGet]
//    public IActionResult Get()
//    {
//        return Ok("Yes! From " + GetType().Name);
//    }

//    public class Convention(IOptions<Convention.MyOptions> options) : SpecialControllerConvention<ControllerC>(options.Value)
//    {
//        public class MyOptions : IMyOptions
//        {
//            /// <inheritdoc />
//            public string? DefaultPolicyName => Const.Polici;

//            /// <inheritdoc />
//            public IReadOnlyDictionary<string, string>? PerActionPolicyNames => new Dictionary<string, string> { ["Get"] = Const.Polici2 };

//            /// <inheritdoc />
//            public bool ReplaceExistingFilters => false;
//        }
//    }
//}