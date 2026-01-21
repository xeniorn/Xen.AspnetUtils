using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Util.AuthorizationHelper.Policies;

namespace Util.AuthorizationHelper.Controllers.Controllers;

/// <summary>
/// 
/// </summary>
/// <typeparam name="TController"></typeparam>
/// <typeparam name="TStandard"></typeparam>
/// <param name="options"></param>
public abstract class SpecialControllerConvention<TController, TStandard>(
    SpecialControllerConvention<TController, TStandard>.IMyOptions? options)
    : SpecialControllerConvention<TController>(options)
    where TController : ControllerBase
    where TStandard : IInternalAuthStandard
{
    protected new interface IMyOptions : SpecialControllerConvention<TController>.IMyOptions
    {
        /// <summary>
        /// Strongly typed configuration of a default policy for the controller
        /// </summary>
        InternalPolicyDefinition<TStandard>? DefaultPolicy { get; }

        /// <summary>
        /// Strongly typed configuration of policies for different actions of a controller
        /// </summary>
        IReadOnlyDictionary<string, InternalPolicyDefinition<TStandard>>? PerActionPolicies { get; }

        /// <inheritdoc />
        string? SpecialControllerConvention<TController>.IMyOptions.DefaultPolicyName => DefaultPolicy?.Name;

        /// <inheritdoc />
        IReadOnlyDictionary<string, string>? SpecialControllerConvention<TController>.IMyOptions.PerActionPolicyNames
            => PerActionPolicies?.ToDictionary
            (
                x => x.Key,
                x => x.Value.Name
            );
    }
}


/// <summary>
/// 
/// </summary>
/// <typeparam name="TController"></typeparam>
/// <param name="options"></param>
public abstract class SpecialControllerConvention<TController>(SpecialControllerConvention<TController>.IMyOptions? options)
    : IActionModelConvention, IControllerModelConvention, IConfigureOptions<MvcOptions>
    where TController : ControllerBase
{

    protected abstract class MyOptionsGenericBase : SpecialControllerConvention<TController>.IMyOptions
    {
        /// <summary>
        /// Strongly typed configuration of a default policy for the controller
        /// </summary>
        InternalPolicyDefinition? DefaultPolicy { get; }

        /// <summary>
        /// Strongly typed configuration of policies for different actions of a controller
        /// </summary>
        IReadOnlyDictionary<string, InternalPolicyDefinition>? PerActionPolicies { get; }

        /// <inheritdoc />
        string? SpecialControllerConvention<TController>.IMyOptions.DefaultPolicyName => DefaultPolicy?.Name;

        /// <inheritdoc />
        IReadOnlyDictionary<string, string>? SpecialControllerConvention<TController>.IMyOptions.PerActionPolicyNames
            => PerActionPolicies?.ToDictionary
            (
                x => x.Key,
                x => x.Value.Name
            );

        /// <inheritdoc />
        public abstract bool ReplaceExistingFilters { get; protected set; }
    }

    /// <summary>
    /// 
    /// </summary>
    protected interface IMyOptions
    {
        /// <summary>
        /// policy will be applied to the controller as a whole
        /// </summary>
        string? DefaultPolicyName { get; }

        /// <summary>
        /// policy will be applied to individual actions. ?is additive with controller policy?
        /// </summary>
        IReadOnlyDictionary<string, string>? PerActionPolicyNames { get; }

        /// <summary>
        /// 
        /// </summary>
        bool ReplaceExistingFilters { get; }
    }

    private IMyOptions? Options => options;

    /// <inheritdoc />
    public void Apply(ActionModel action)
    {
        if (Options is null)
            return;

        if (action.Controller.ControllerType != typeof(TController))
            return;

        var actionName = action.ActionName;
        if (Options.PerActionPolicyNames?.GetValueOrDefault(actionName) is not { } matchedPolicy || string.IsNullOrWhiteSpace(matchedPolicy))
            return;

        if (Options.ReplaceExistingFilters)
            action.Filters.Clear();

        action.Filters.Add(new AuthorizeFilter(matchedPolicy));
    }

    /// <inheritdoc />
    public void Apply(ControllerModel controller)
    {
        if (Options is null)
            return;

        if (controller.ControllerType != typeof(TController))
            return;

        var policy = Options.DefaultPolicyName;
        if (string.IsNullOrWhiteSpace(policy))
            return;

        if (Options.ReplaceExistingFilters)
            controller.Filters.Clear();

        controller.Filters.Add(new AuthorizeFilter(policy));
    }

    /// <inheritdoc />
    public void Configure(MvcOptions options)
    {
        options.Conventions.Add((IActionModelConvention)this);
        options.Conventions.Add((IControllerModelConvention)this);
    }
}