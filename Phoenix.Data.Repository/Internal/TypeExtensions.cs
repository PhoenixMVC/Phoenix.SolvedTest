using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Phoenix.Data.Repository.Internal
{
    internal static class TypeExtensions
    {
        public static bool IsEntityConfigure(this Type type)
        {
            return type.IsClass &&
                    type.IsPublic &&
                    !type.IsAbstract &&
                    type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>));
        }

        public static Type ExtractEntityType(this Type type)
        {
            return type.GetInterfaces()
                .First(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>))
                .GetGenericArguments()
                .First();

        }
    }
}
