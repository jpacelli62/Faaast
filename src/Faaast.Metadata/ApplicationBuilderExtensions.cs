﻿using Faaast.Metadata;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ApplicationBuilderExtensions
    {
        public static IServiceCollection AddMetadata(this IServiceCollection services)
        {
            //services.TryAddSingleton<IObjectMapper, DefaultObjectMapper>();
            services.TryAddSingleton<IObjectMapper, EmitObjectMapper>();
            return services;
        }
    }
}
