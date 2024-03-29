﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Faaast.SeoRouter
{
    public class Router : IRouter
    {
        public const string ControllerKey = "controller";
        public const string ActionKey = "action";

        public RoutingRules Rules { get; set; }

        public RoutingRules StaticRoutes { get; set; } = new RoutingRules();

        private ILogger Log { get; set; }

        public async Task<RoutingRules> GetRulesAsync(IServiceProvider services)
        {
            this.Log ??= services.GetRequiredService<ILoggerFactory>().CreateLogger("SeoRouter");

            var provider = services.GetRequiredService<IRouteProvider>();
            if (this.Rules == null || await provider.RefreshNeededAsync(this.Rules))
            {
                this.Log.LogDebug("Refreshing rules");
                this.Rules = await provider.GetRulesAsync();
            }

            return this.Rules;
        }

        public Task RouteAsync(RouteContext context)
        {
            var httpContext = context.HttpContext;
            var services = httpContext.RequestServices;
            return this.RouteAsync(httpContext.Request.Path.ToString(), context, services);
        }

        public Task RouteAsync(string url, RouteContext context, IServiceProvider services)
        {
            var rule = this.FollowRoute(url, services, out var origin, out var values);
            var provider = services.GetRequiredService<IRouteProvider>();
            if (rule != null)
            {
                if (rule == origin)
                {
                    var vpd = this.GetVirtualPath(new VirtualPathContext(context.HttpContext, context.RouteData.DataTokens, values));
                    if(vpd != null && !url.NormalizeUrl().Equals(vpd.VirtualPath.NormalizeUrl(), StringComparison.OrdinalIgnoreCase))
                    {
                        this.Log.LogTrace("Url \"{0}\" matches route {1} but no match good url \"{2}\", redirecting..", url, origin.DisplayName, vpd.VirtualPath);
                        return provider.HandleRedirectAsync(context, vpd, true);
                    }

                    var routers = context.RouteData?.Routers;
                    context.RouteData = new RouteData(values);
                    if(routers != null)
                    {
                        foreach (var router in routers)
                        {
                            context.RouteData.Routers.Add(router);
                        }
                    }

                    return provider.HandleAsync(context, rule);
                }
                else
                {
                    context.RouteData = new RouteData(values);
                    var vpd = rule.GetVirtualPath(this, context.RouteData.DataTokens, values);
                    this.Log.LogTrace("Url \"{0}\" matches route {1} and is redirected to {2} (\"{3}\")", url, origin.DisplayName, rule.DisplayName, vpd.VirtualPath);
                    return provider.HandleRedirectAsync(context, vpd, rule.Handler != HandlerType.RedirectTemporary);
                }
            }

            return Task.CompletedTask;
        }

        public RoutingRule FindRouteRule(string url, IServiceProvider provider, out RouteValueDictionary routeValues)
        {
            var requestPath = url.NormalizeUrl();
            var rules = this.GetRulesAsync(provider).Result;
            return this.StaticRoutes.Find(requestPath, out routeValues) ?? rules.Find(requestPath, out routeValues);
        }

        public RoutingRule FollowRoute(string url, IServiceProvider provider, out RoutingRule origin, out RouteValueDictionary values)
        {
            origin = this.FindRouteRule(url, provider, out values);
            if (origin != null)
            {
                this.Log.LogTrace("Url \"{0}\" matches route {1} with handler {2}", url, origin.DisplayName, origin.Handler.ToString());
                switch (origin.Handler)
                {
                    case HandlerType.Auto:
                    case HandlerType.RedirectPermanent:
                    case HandlerType.RedirectTemporary:
                        return this.GetVirtualPathRuleAsync(new RouteValueDictionary(), values, provider, null).ConfigureAwait(false).GetAwaiter().GetResult();

                    case HandlerType.Legacy:
                    case HandlerType.Gone:
                    case HandlerType.NotFound:
                        break;
                }
            }

            return origin;
        }

        public VirtualPathData GetVirtualPath(VirtualPathContext context)
        {
            var httpContext = context.HttpContext;
            return this.GetVirtualPathAsync(context, httpContext.RequestServices).Result;
        }

        public async Task<VirtualPathData> GetVirtualPathAsync(VirtualPathContext context, IServiceProvider services, object sourceOject = null)
        {
            var rule = await this.GetVirtualPathRuleAsync(context.AmbientValues, context.Values, services, sourceOject);
            return rule?.GetVirtualPath(this, context.AmbientValues, context.Values);
        }

        public async Task<RoutingRule> GetVirtualPathRuleAsync(RouteValueDictionary ambiantValues, RouteValueDictionary contextValues, IServiceProvider services, object sourceOject = null)
        {
            var routeProvider = services.GetRequiredService<IRouteProvider>();
            var rules = await this.GetRulesAsync(services);
            await foreach (var rule in this.StaticRoutes.FindByRouteAsync(contextValues))
            {
                if (await routeProvider.ResolveUrlPartsAsync(rule, ambiantValues, contextValues, sourceOject))
                {
                    return rule;
                }
            }

            await foreach (var rule in rules.FindByRouteAsync(contextValues))
            {
                if (await routeProvider.ResolveUrlPartsAsync(rule, ambiantValues, contextValues, sourceOject))
                {
                    return rule;
                }
            }

            return null;
        }
    }
}