using Microsoft.AspNetCore.Mvc;
using Util.AuthorizationHelper.Controllers.Controllers;

namespace Util.AuthorizationHelper.Controllers.Test;

public class ExpressionBasedMapTests
{
    public class TestController : ControllerBase
    {
        [HttpGet]
        public IActionResult Test() => Ok();
    }

    [Fact]
    public void Test1()
    {
        const string policy = "policy";

        var map = SpecialControllerConventionHelper.ConstructPolicyMap<TestController>(
            (x => x.Test, policy)
        );

        Assert.Equal(1, map.Count);

        var (key, value) = map.Single();

        Assert.Equal(nameof(TestController.Test), key);
        Assert.Equal(policy, value);

    }
}