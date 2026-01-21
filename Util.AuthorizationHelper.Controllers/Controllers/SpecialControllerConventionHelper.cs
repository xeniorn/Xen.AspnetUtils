using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using System.Reflection;

namespace Util.AuthorizationHelper.Controllers.Controllers;

/// <summary>
/// 
/// </summary>
public static class SpecialControllerConventionHelper
{
    ///// <summary>
    ///// 
    ///// </summary>
    ///// <param name="items"></param>
    ///// <returns></returns>
    //public static IReadOnlyDictionary<string, string> ConstructPolicyMap<T>(params (Func<T, string> actionNameExpression, string policy)[] items)
    //    where T: ControllerBase
    //{
    //    var res = items.ToDictionary
    //    (
    //        x => x.actionName,
    //        x => x.policy
    //    );
    //    return res;
    //}

    /// <summary>
    /// 
    /// </summary>
    /// <param name="items"></param>
    /// <returns></returns>
    public static IReadOnlyDictionary<string, string> ConstructPolicyMap(params (string actionName, string policy)[] items)
    {
        var res = items.ToDictionary
        (
            x => x.actionName,
            x => x.policy
        );
        return res;
    }

    public static IReadOnlyDictionary<string, string> ConstructPolicyMap<TController>(params (Expression<Func<TController, Delegate>> expression, string policy)[] items)
        where TController : ControllerBase
    {
        var res = items.ToDictionary
        (
            x =>
            {
                try
                {
                    // x => x.Method

                    // x.Method
                    var unaryExpression = (UnaryExpression)x.expression.Body;
                    // x.Method
                    var methodCallExpression = (MethodCallExpression)unaryExpression.Operand;
                    // Method
                    var methodInfoExpression = (ConstantExpression)methodCallExpression.Object;
                    var methodInfo = (MethodInfo?)methodInfoExpression.Value;
                    return methodInfo?.Name ?? throw new ArgumentException();
                }
                catch (Exception ex)
                {
                    throw new ArgumentException("Not a member method expression", ex);
                }
            },
            x => x.policy
        );
        return res;
    }
}