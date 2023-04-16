using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Generic;

namespace Microsoft.AspNetCore.Mvc.Infrastructure
{
    public static class ActionDescriptorExtensions
    {
        public static string GetRouteName(this ActionDescriptor actionDescriptor)
        {

            var area = GetValueSafe("area", actionDescriptor.RouteValues);
            var controller = GetValueSafe("controller", actionDescriptor.RouteValues);
            var action = GetValueSafe("action", actionDescriptor.RouteValues);
            return $"{area ?? ""}/{controller}/{action}".TrimStart('/');

        }
        private static string GetValueSafe(string key, IDictionary<string, string> routes)
        {
            routes.TryGetValue(key, out string result);
            return result;
        }

    }
}

