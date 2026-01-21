using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Util.AuthorizationHelper.Controllers.Controllers;

/// <summary>
/// 
/// </summary>
public static class SharedDIHelper
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TController"></typeparam>
    /// <typeparam name="TConvention"></typeparam>
    /// <typeparam name="TOptions"></typeparam>
    /// <param name="builder"></param>
    /// <param name="useAuthorization"></param>
    /// <param name="configureDependencies"></param>
    /// <returns></returns>
    public static OptionsBuilder<TOptions> AddControllerWithConvention<TController, TConvention, TOptions>(
        this IHostApplicationBuilder builder, bool useAuthorization = true, Action<IHostApplicationBuilder>? configureDependencies = null)
        where TController : ControllerBase
        where TConvention : SpecialControllerConvention<TController>
        where TOptions : class
    {
        configureDependencies?.Invoke(builder);

        builder.AddControllerWithConvention<TController, TConvention>(useAuthorization);
        var optBuilder = builder.Services.AddOptions<TOptions>();

        return optBuilder;
    }

    /// <summary>
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="useAuthorizationConventions"></param>
    public static void AddControllerWithConvention<TController, TConvention>(this IHostApplicationBuilder builder, bool useAuthorizationConventions = true)
        where TController : ControllerBase
        where TConvention : SpecialControllerConvention<TController>
    {
        builder.Services.AddControllers()
            .AddApplicationPart(typeof(TController).Assembly);

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.IncludeXmlComments(Assembly.GetAssembly(typeof(TController)));
        });

        if (useAuthorizationConventions)
        {
            // for authorization:
            builder.Services.TryAddSingleton<TConvention>();
            builder.Services.AddSingleton<IConfigureOptions<MvcOptions>, TConvention>(sp => sp.GetRequiredService<TConvention>());
        }
    }
}

