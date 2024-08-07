﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Faaast.SeoRouter
{
    public class RoutingRules
    {
        private readonly Dictionary<string, Dictionary<string, List<RoutingRule>>> _indexByControllerAction = new(StringComparer.OrdinalIgnoreCase);
        private readonly List<RoutingRule> _allRules = new();
        private readonly List<RoutingRule> _dynamicRules = new();

        private readonly Dictionary<string, List<RoutingRule>> _indexByUrl = new();

        private readonly ReaderWriterLock _syncLock = new();

        public IReadOnlyCollection<RoutingRule> Rules => _allRules.ToArray();

        public void Add(RoutingRule rule)
        {
            _syncLock.AcquireWriterLock(5000);
            try
            {
                _allRules.Add(rule);
                if (rule.IsDynamic || rule.Kind == RuleKind.Global)
                {
                    _dynamicRules.Add(rule);
                }

                if (!rule.IsDynamic)
                {
                    var baseUrl = rule.Url.BaseUrl();
                    if (!_indexByUrl.TryGetValue(baseUrl, out var items))
                    {
                        _indexByUrl[baseUrl] = items = new List<RoutingRule>();
                    }

                    items.Add(rule);
                }

                if (rule.CanGenerateUrl && rule.Target != null)
                {
                    _indexByControllerAction.TryAdd(rule.Target.Value.Controller, () => new Dictionary<string, List<RoutingRule>>(StringComparer.OrdinalIgnoreCase));
                    _indexByControllerAction[rule.Target.Value.Controller].TryAdd(rule.Target.Value.Action, () => new List<RoutingRule>());
                    _indexByControllerAction[rule.Target.Value.Controller][rule.Target.Value.Action].Add(rule);
                }
            }
            finally
            {
                _syncLock.ReleaseWriterLock();
            }
        }

        public void AddRange(IEnumerable<RoutingRule> rules)
        {
            if (rules != null)
            {
                foreach (var item in rules)
                {
                    this.Add(item);
                }
            }
        }

        public RoutingRule Find(string requestPath, out RouteValueDictionary values)
        {
            _syncLock.AcquireReaderLock(5000);
            try
            {
                requestPath = requestPath.NormalizeUrl();
                var baseUrl = requestPath.BaseUrl();

                if (_indexByUrl.TryGetValue(baseUrl, out var rules))
                {
                    foreach (var rule in rules)
                    {
                        if (rule.MatchStrict(requestPath, out var requestValues))
                        {
                            values = requestValues;
                            return rule;
                        }
                    }
                }

                var pathString = new PathString("/" + baseUrl);
                foreach (var rule in _dynamicRules)
                {
                    if (rule.MatchDynamic(pathString, out var requestValues))
                    {
                        var cookie = _syncLock.UpgradeToWriterLock(5000);
                        try
                        {
                            if (!_indexByUrl.TryGetValue(baseUrl, out var items))
                            {
                                _indexByUrl[baseUrl] = items = new List<RoutingRule>();
                            }

                            if (!items.Contains(rule))
                            {
                                items.Add(rule);
                            }
                        }
                        finally { _syncLock.DowngradeFromWriterLock(ref cookie); }

                        if (rule.Target.HasValue)
                        {
                            var newValues = new RouteValueDictionary();
                            newValues[Router.ControllerKey] = rule.Target.Value.Controller;
                            newValues[Router.ActionKey] = rule.Target.Value.Action;
                            var constraintsQueryDictionnary = rule.Target.Value.Constraints.GetQueryDictionnary();

                            foreach (var key in constraintsQueryDictionnary.Keys)
                            {
                                newValues[key] = requestValues[key];
                            }

                            foreach (var kvp in rule.Target.Value.DefaultValues)
                            {
                                newValues[kvp.Key] = requestValues.ContainsKey(kvp.Key) ? requestValues[kvp.Key] : kvp.Value;
                            }

                            requestValues = newValues;
                        }

                        values = requestValues;
                        return rule;
                    }
                }

                values = null;
                return null;
            }
            finally
            {
                _syncLock.ReleaseReaderLock();
            }
        }

        public async IAsyncEnumerable<RoutingRule> FindByRouteAsync(RouteValueDictionary values)
        {
            await Task.CompletedTask;
            if (values.TryGetValue("controller", out var controller) &&
                values.TryGetValue("action", out var action) &&
                _indexByControllerAction.TryGetValue(controller.ToString(), out var actions) &&
                actions.TryGetValue(action.ToString(), out var rules))
            {
#pragma warning disable S3267 // loop should be simplified with Linq
                foreach (var rule in rules)
#pragma warning restore S3267// loop should be simplified with Linq
                {
                    if (rule.MatchConstraints(values, RouteDirection.UrlGeneration))
                    {
                        yield return rule;
                    }
                }
            }
        }
    }
}
