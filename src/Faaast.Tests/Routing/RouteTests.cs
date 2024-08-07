﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Faaast.SeoRouter;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Faaast.Tests.Routing
{
    public class RouteTests : IClassFixture<RouterFixture>
    {
        public RouterFixture Fixture { get; set; }

        public IServiceProvider ServiceProvider { get; set; }

        public IRouter Router { get; set; }

        public RouteTests(RouterFixture fixture)
        {
            this.Fixture = fixture;
            this.ServiceProvider = RouterFixture.BuildProvider(x => { });
            this.Router = new Moq.Mock<IRouter>().Object;
        }

        [Fact]
        public void MatchStrictSimple()
        {
            var route = new RoutingRule(this.ServiceProvider, "test", RuleKind.Strict, HandlerType.Auto, "recettes/fiche_recette_v3.aspx", new MvcAction("recipe", "details", string.Empty));
            var rules = new RoutingRules();
            rules.Add(route);
            Assert.Equal("test", route.DisplayName);
            Assert.NotNull(rules.Find("recettes/fiche_recette_v3.aspx", out _));
            Assert.NotNull(rules.Find("/recettes/fiche_recette_v3.aspx", out _));
            Assert.Null(rules.Find("recettes/fiche_recette_v3.aspx?foo=bar", out _));
            Assert.Equal(route.Url, route.GetVirtualPath(this.Router, new RouteValueDictionary(), route.Target.ToRouteValueDictionary()).VirtualPath.NormalizeUrl());

        }

        [Fact]
        public void MatchStrictWithParameters()
        {
            var route = new RoutingRule(this.ServiceProvider, "test", RuleKind.Strict, HandlerType.Auto, "recettes/fiche_recette_v3.aspx?foo=bar", new MvcAction("recipe", "details", string.Empty));
            var rules = new RoutingRules();
            rules.Add(route);
            Assert.Null(rules.Find("recettes/fiche_recette_v3.aspx", out _));
            Assert.Null(rules.Find("/recettes/fiche_recette_v3.aspx", out _));
            Assert.NotNull(rules.Find("recettes/fiche_recette_v3.aspx?foo=bar", out _));
            Assert.NotNull(rules.Find("/recettes/fiche_recette_v3.aspx?foo=bar", out _));
            Assert.Equal(route.Url, route.GetVirtualPath(this.Router, new RouteValueDictionary(), route.Target.ToRouteValueDictionary()).VirtualPath.NormalizeUrl());
        }

        [Fact]
        public void MatchStrictStartWithSlashSimple()
        {
            var route = new RoutingRule(this.ServiceProvider, "test", RuleKind.Strict, HandlerType.Auto, "/recettes/fiche_recette_v3.aspx", new MvcAction("recipe", "details", string.Empty));
            var rules = new RoutingRules();
            rules.Add(route);
            Assert.NotNull(rules.Find("recettes/fiche_recette_v3.aspx", out _));
            Assert.NotNull(rules.Find("/recettes/fiche_recette_v3.aspx", out _));
            Assert.Null(rules.Find("recettes/fiche_recette_v3.aspx?foo=bar", out _));
            Assert.Equal(route.Url, route.GetVirtualPath(this.Router, new RouteValueDictionary(), route.Target.ToRouteValueDictionary()).VirtualPath.NormalizeUrl());
        }

        [Fact]
        public void MatchStrictStartWithSlashWithParameters()
        {
            var route = new RoutingRule(this.ServiceProvider, "test", RuleKind.Strict, HandlerType.Auto, "/recettes/fiche_recette_v3.aspx?foo=bar", new MvcAction("recipe", "details", string.Empty));
            var rules = new RoutingRules();
            rules.Add(route);
            Assert.Null(rules.Find("recettes/fiche_recette_v3.aspx", out _));
            Assert.Null(rules.Find("/recettes/fiche_recette_v3.aspx", out _));
            Assert.NotNull(rules.Find("recettes/fiche_recette_v3.aspx?foo=bar", out _));
            Assert.NotNull(rules.Find("/recettes/fiche_recette_v3.aspx?foo=bar", out _));
            Assert.Equal(route.Url, route.GetVirtualPath(this.Router, new RouteValueDictionary(), route.Target.ToRouteValueDictionary()).VirtualPath.NormalizeUrl());
        }

        [Fact]
        public void MatchGlobalNoDynamic()
        {
            var route = new RoutingRule(this.ServiceProvider, "test", RuleKind.Global, HandlerType.Auto, "/recettes/fiche_recette_v3.aspx", new MvcAction("recipe", "details", string.Empty));
            var rules = new RoutingRules();
            rules.Add(route);
            Assert.NotNull(rules.Find("recettes/fiche_recette_v3.aspx", out _));
            Assert.NotNull(rules.Find("/recettes/fiche_recette_v3.aspx", out _));
            Assert.NotNull(rules.Find("recettes/fiche_recette_v3.aspx?foo=bar", out _));
            Assert.NotNull(rules.Find("/recettes/fiche_recette_v3.aspx?foo=bar", out _));
            Assert.Equal(route.Url, route.GetVirtualPath(this.Router, new RouteValueDictionary(), route.Target.ToRouteValueDictionary()).VirtualPath.NormalizeUrl());
        }

        [Fact]
        public void MatchGlobalNoDynamicWithParameters()
        {
            var route = new RoutingRule(this.ServiceProvider, "test", RuleKind.Global, HandlerType.Auto, "/recettes/fiche_recette_v3.aspx?foo=bar", new MvcAction("recipe", "details", string.Empty));
            var rules = new RoutingRules();
            rules.Add(route);
            Assert.Null(rules.Find("recettes/fiche_recette_v3.aspx", out _));
            Assert.Null(rules.Find("/recettes/fiche_recette_v3.aspx", out _));
            Assert.NotNull(rules.Find("recettes/fiche_recette_v3.aspx?foo=bar", out _));
            Assert.NotNull(rules.Find("/recettes/fiche_recette_v3.aspx?foo=bar", out _));
            var values = route.Target.ToRouteValueDictionary();
            Assert.NotNull(rules.FindByRouteAsync(values));
            var virtualpath = route.GetVirtualPath(this.Router, new RouteValueDictionary(), values);
            Assert.Equal("recettes/fiche_recette_v3.aspx", virtualpath.VirtualPath.NormalizeUrl());
        }

        [Fact]
        public void MatchGlobalWithConstraints()
        {
            var route = new RoutingRule(this.ServiceProvider, "test", RuleKind.Global, HandlerType.Auto, "{parent}/{name}-REC{id}", new MvcAction("recipe", "details", string.Empty, "id=^[0-9]+$&name=^[abcd-]+$"));
            var rules = new RoutingRules();
            rules.Add(route);
            Assert.Null(rules.Find("recettes/fiche_recette_v3.aspx", out _));
            Assert.Null(rules.Find("/tesst-recdsdfsdfdsf", out _));
            Assert.Null(rules.Find("/abcd-recdsdfsdfdsf", out _));
            Assert.Null(rules.Find("abcd-aaa-rec3", out _));
            Assert.NotNull(rules.Find("test/abcd-aaa-rec3", out _));
            Assert.NotNull(rules.Find("test/a-b-rec3", out var values));
            Assert.Equal("recipe", values["controller"]);
            Assert.Equal("details", values["action"]);
            Assert.Equal("a-b", values["name"]);
            Assert.Equal("3", values["id"]);
            Assert.False(values.ContainsKey("parent"));
        }

        [Fact]
        public void MatchGlobalWithParameter()
        {
            var route = new RoutingRule(this.ServiceProvider, "test", RuleKind.Global, HandlerType.Auto, "recettes/fiche_recette_v3.aspx", new MvcAction("Recipe", "Details", string.Empty));
            var rules = new RoutingRules();
            rules.Add(route);
            Assert.NotNull(rules.Find("recettes/fiche_recette_v3.aspx?id=91480", out var values));
            Assert.Equal("recipe", values["controller"]);
            Assert.Equal("details", values["action"]);
        }

        [Fact]
        public void TestGenerateDynamicUrlWithParameters()
        {
            var route = new RoutingRule(this.ServiceProvider, "test", RuleKind.Global, HandlerType.Auto, "{name}_p{id}", new MvcAction("Portal", "Details", null, "id=^[0-9]+"));
            var path = route.GetVirtualPath(this.Router, new RouteValueDictionary(), new RouteValueDictionary(new { controller = "Portal", action = "Details", Id = "123", name = "HelloWorld", slug = "blah" }));
            Assert.Equal("/HelloWorld_p123?slug=blah", path.VirtualPath);
        }

        [Fact]
        public void TestGenerateDynamicUrlWithMissingParameters()
        {
            var route = new RoutingRule(this.ServiceProvider, "test", RuleKind.Global, HandlerType.Auto, "{name}_p{id}", new MvcAction("Portal", "Details", null, "id=^[0-9]+"));
            var path = route.GetVirtualPath(this.Router, new RouteValueDictionary(), new RouteValueDictionary(new { controller = "Portal", action = "Details", Id = "123", slug = "blah" }));
            Assert.Null(path);
        }

        [Fact]
        public void TestGenerateDynamicUrlWithOptionalParameters()
        {
            var route = new RoutingRule(this.ServiceProvider, "test", RuleKind.Global, HandlerType.Auto, "p{id}.{ext?}", new MvcAction("Portal", "Details", null, "id=^[0-9]+"));
            var path = route.GetVirtualPath(this.Router, new RouteValueDictionary(), new RouteValueDictionary(new { controller = "Portal", action = "Details", Id = "123", slug = "blah" }));
            Assert.Equal("/p123?slug=blah", path.VirtualPath);
        }

        [Fact]
        public void TestGenerateDynamicUrlWithRouteProvider()
        {
            var rule = new RoutingRule(this.ServiceProvider, "test", RuleKind.Global, HandlerType.Auto, "{name}_p{id}", new MvcAction("Portal", "Details", null, "id=^[0-9]+"));
            var rules = new RoutingRules();
            rules.Add(rule);

            var provider = new Mock<IRouteProvider>();
            provider.Setup(x => x.ResolveUrlPartsAsync(It.IsAny<RoutingRule>(), It.IsAny<RouteValueDictionary>(), It.IsAny<RouteValueDictionary>(), It.IsAny<object>()))
                .Callback<RoutingRule, RouteValueDictionary, RouteValueDictionary, object>((rule, ambiant, values, src) => values.TryAdd("name", "wonderful"))
                .ReturnsAsync(true);
            provider.Setup(x => x.GetRulesAsync()).ReturnsAsync(rules);

            var router = RouterFixture.BuildRouterWith(provider.Object, out var services);
            var values = new RouteValueDictionary(new { controller = "Portal", action = "Details", Id = "123", slug = "blah" });

            var matchingRule = router.GetVirtualPathRuleAsync(new RouteValueDictionary(), values, services).Result;
            Assert.Equal(rule, matchingRule);

            var context = new VirtualPathContext(services.GetService<IHttpContextAccessor>().HttpContext, new RouteValueDictionary(), values);
            var vpd = router.GetVirtualPath(context);
            Assert.NotNull(vpd);
            Assert.Equal("/wonderful_p123?slug=blah", vpd.VirtualPath);
        }

        [Fact]
        public void TestFollowRoutesRedirect()
        {
            var source = new RoutingRule(this.ServiceProvider, "empty", RuleKind.Global, HandlerType.RedirectPermanent, "category/empty", new MvcAction("Category", "Details", string.Empty));
            var target = new RoutingRule(this.ServiceProvider, "target", RuleKind.Global, HandlerType.Auto, "category", new MvcAction("Category", "Details", string.Empty));
            var rules = new RoutingRules();
            rules.Add(source);
            rules.Add(target);
            var provider = new Mock<IRouteProvider>();
            provider.Setup(x => x.GetRulesAsync()).ReturnsAsync(rules);
            provider.Setup(x => x.ResolveUrlPartsAsync(It.IsAny<RoutingRule>(), It.IsAny<RouteValueDictionary>(), It.IsAny<RouteValueDictionary>(), It.IsAny<object>())).ReturnsAsync(true);

            var router = RouterFixture.BuildRouterWith(provider.Object, out var services);
            var result = router.FollowRoute("category/empty", services, out var origin, out var values);
            Assert.NotEqual(result, origin);
            Assert.Equal(target, result);
        }

        [Fact]
        public void TestFollowRedirectToGoodRoute()
        {
            var source = new RoutingRule(this.ServiceProvider, "empty", RuleKind.Global, HandlerType.Auto, "category/empty", new MvcAction("Category", "Details", string.Empty));
            var duplicate = new RoutingRule(this.ServiceProvider, "target", RuleKind.Global, HandlerType.Auto, "category", new MvcAction("Category", "Details", string.Empty));
            var rules = new RoutingRules();
            rules.AddRange(new[] { source, duplicate });
            var provider = new Mock<IRouteProvider>();
            provider.Setup(x => x.GetRulesAsync()).ReturnsAsync(rules);
            provider.Setup(x => x.ResolveUrlPartsAsync(It.IsAny<RoutingRule>(), It.IsAny<RouteValueDictionary>(), It.IsAny<RouteValueDictionary>(), It.IsAny<object>())).ReturnsAsync(true);

            var router = RouterFixture.BuildRouterWith(provider.Object, out var services);
            var result = router.FollowRoute("category", services, out var origin, out var values);
            Assert.NotEqual(result, origin);
            Assert.Equal(source, result);
        }

        [Fact]
        public void TestMatchStrictConstraints()
        {
            var rule = new RoutingRule(this.ServiceProvider, "empty", RuleKind.Strict, HandlerType.Auto, "category/empty?foo=bar", new MvcAction("Category", "Details", "id=5"));
            var rules = new RoutingRules();
            rules.Add(rule);
            var provider = new Mock<IRouteProvider>();
            provider.Setup(x => x.GetRulesAsync()).ReturnsAsync(rules);
            provider.Setup(x => x.ResolveUrlPartsAsync(It.IsAny<RoutingRule>(), It.IsAny<RouteValueDictionary>(), It.IsAny<RouteValueDictionary>(), It.IsAny<object>())).ReturnsAsync(true);
            var router = RouterFixture.BuildRouterWith(provider.Object, out var services);

            var values1 = new RouteValueDictionary(new { controller = "Category", action = "Details", foo = "fizz" });
            Assert.Null(router.GetVirtualPathAsync(new VirtualPathContext(services.GetService<IHttpContextAccessor>().HttpContext, new RouteValueDictionary(), values1), services).Result);

            var values2 = new RouteValueDictionary(new { controller = "Category", action = "Details", foo = "bar" });
            Assert.Null(router.GetVirtualPathAsync(new VirtualPathContext(services.GetService<IHttpContextAccessor>().HttpContext, new RouteValueDictionary(), values2), services).Result);

            var values3 = new RouteValueDictionary(new { controller = "Category", action = "Details", foo = "bar", id = 5 });
            Assert.Null(router.GetVirtualPathAsync(new VirtualPathContext(services.GetService<IHttpContextAccessor>().HttpContext, new RouteValueDictionary(), values3), services).Result);

        }

        [Fact]
        public void MatchStrictWithTooMuchParameters()
        {
            var route = new RoutingRule(this.ServiceProvider, "test", RuleKind.Strict, HandlerType.Auto, "actualites/chaud-devant/", new MvcAction("portal", "details", "id=123"));
            var rules = new RoutingRules();
            rules.Add(route);
            Assert.Null(rules.Find("actualites/chaud-devant/?foo=bar", out _));
            Assert.NotNull(rules.Find("actualites/chaud-devant/", out _));

            Assert.NotEmpty(Enumerate(rules.FindByRouteAsync(new RouteValueDictionary(new { controller = "portal", action = "details", id = "123" }))));
            Assert.Empty(Enumerate(rules.FindByRouteAsync(new RouteValueDictionary(new { controller = "portal", action = "details", id = "345" }))));
            Assert.Empty(Enumerate(rules.FindByRouteAsync(new RouteValueDictionary(new { controller = "portal", action = "details", id = "123", page = 2 }))));
        }

        [Fact]
        public void MatchGlobalWithTooMuchParameters()
        {
            var route = new RoutingRule(this.ServiceProvider, "test", RuleKind.Global, HandlerType.Auto, "actualites/chaud-devant/{page}/", new MvcAction("portal", "details", "Id=123&page=", "Id=^[0-9]+$&page=^[0-9]+$"));
            var rules = new RoutingRules();
            rules.Add(route);
            Assert.NotNull(rules.Find("actualites/chaud-devant/3/", out _));
            Assert.NotNull(rules.Find("actualites/chaud-devant/3/?utm=abc", out _));
            Assert.Null(rules.Find("actualites/chaud-devant/", out _));
            Assert.Null(rules.Find("actualites/chaud-devant/test", out _));

            Assert.NotEmpty(Enumerate(rules.FindByRouteAsync(new RouteValueDictionary(new { controller = "portal", action = "details", id = "123", page = "2" }))));

            Assert.Empty(Enumerate(rules.FindByRouteAsync(new RouteValueDictionary(new { controller = "portal", action = "details", id = "123" }))));
            Assert.Empty(Enumerate(rules.FindByRouteAsync(new RouteValueDictionary(new { controller = "portal", action = "details", id = "345", page = "2" }))));
        }

        private ICollection<T> Enumerate<T>(IAsyncEnumerable<T> task)
        {
            var result = new List<T>();
            Task.Run(async () =>
            {
                await foreach (var item in task)
                {
                    result.Add(item);
                }

            }).ConfigureAwait(false).GetAwaiter().GetResult();

            return result;
        }
    }
}
