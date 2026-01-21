using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Util.AuthorizationHelper.Policies;

namespace Util.AuthorizationHelper.Controllers.Controllers;

/// <summary>
/// A standard-conforming version of <see cref="SpecialControllerConvention{TController}.IMyOptions"/>
/// Expected to be injected as a <see cref="IConfigureOptions{MvcOptions}"/>
/// A concrete type should be made for each controller
/// </summary>
/// <typeparam name="TController"></typeparam>
/// <typeparam name="TStandard"></typeparam>
/// <param name="options"></param>
public abstract class SpecialControllerConvention<TController, TStandard>(
    SpecialControllerConvention<TController, TStandard>.IMyOptions? options, ILogger? logger = null)
    : SpecialControllerConvention<TController>(options, logger)
    where TController : ControllerBase
    where TStandard : IInternalAuthStandard
{
    /// <summary>
    /// Uses fully qualified, strongly typed and standard-conforming options instead of plain strings like in <see cref="SpecialControllerConvention{TController}.IMyOptions"/>
    /// </summary>
    public new interface IMyOptions : SpecialControllerConvention<TController>.IMyOptions
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
/// When applied to MvcOptions, assigns authorization policies to the target controller type based on the provided options.
/// Expected to be injected as a <see cref="IConfigureOptions{MvcOptions}"/>
/// A concrete type should be made for each controller
/// </summary>
/// <typeparam name="TController"></typeparam>
/// <param name="conventionOptions"></param>
public abstract class SpecialControllerConvention<TController>(SpecialControllerConvention<TController>.IMyOptions? conventionOptions, ILogger? logger)
    : IActionModelConvention, IControllerModelConvention, IConfigureOptions<MvcOptions>
    where TController : ControllerBase
{
    private readonly ILogger _logger = logger ?? NullLogger.Instance;

    /// <summary>
    /// Uses strongly  typed policy definitions instead of strings
    /// </summary>
    public abstract class MyOptionsGenericBase : SpecialControllerConvention<TController>.IMyOptions
    {
        /// <summary>
        /// Strongly typed configuration of a default policy for the controller
        /// </summary>
        public InternalPolicyDefinition? DefaultPolicy { get; set; }

        /// <summary>
        /// Strongly typed configuration of policies for different actions of a controller
        /// </summary>
        public IReadOnlyDictionary<string, InternalPolicyDefinition>? PerActionPolicies { get; set; }

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
    public interface IMyOptions
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

    private IMyOptions? Options => conventionOptions;

    /// <inheritdoc />
    public void Apply(ActionModel action)
    {
        if (Options is null)
        {
            _logger.LogWarning("Null options provided to this convention, it will not do anything ({typeName}).", GetType().Name);
            return;
        }
            

        if (action.Controller.ControllerType != typeof(TController))
            return;

        var actionName = action.ActionName;
        if (Options.PerActionPolicyNames?.GetValueOrDefault(actionName) is not { } matchedPolicy || string.IsNullOrWhiteSpace(matchedPolicy))
            return;

        if (Options.ReplaceExistingFilters)
        {
            action.Filters.Clear();
            _logger.LogWarning("Removed existing filters from action {actionName} in controller {controllerName}", action.ActionName, action.Controller.ControllerName);
        }

        action.Filters.Add(new AuthorizeFilter(matchedPolicy));
        _logger.LogInformation("Added policy requirement {policyName} for action {actionName} in controller {controllerName}", matchedPolicy, action.ActionName, action.Controller.ControllerName);
    }

    /// <inheritdoc />
    public void Apply(ControllerModel controller)
    {
        if (Options is null)
        {
            _logger.LogWarning("Null options provided to this convention, it will not do anything ({typeName}).", GetType().Name);
            return;
        }

        if (controller.ControllerType != typeof(TController))
            return;

        var policy = Options.DefaultPolicyName;
        if (string.IsNullOrWhiteSpace(policy))
            return;

        if (Options.ReplaceExistingFilters)
        {
            controller.Filters.Clear();
            _logger.LogWarning("Removed existing filters from controller {controllerName}", controller.ControllerName);
        }
            

        controller.Filters.Add(new AuthorizeFilter(policy));
        _logger.LogInformation("Added policy requirement {policyName} for controller {controllerName}", policy, controller.ControllerName);
    }

    /// <inheritdoc />
    public void Configure(MvcOptions options)
    {
        options.Conventions.Add((IActionModelConvention)this);
        options.Conventions.Add((IControllerModelConvention)this);
        _logger.LogInformation("This convention was added to the MvcOptions: {conventionType}", GetType().Name);
    }
}