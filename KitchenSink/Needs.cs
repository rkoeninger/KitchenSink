using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace KitchenSink
{
    /// <summary>
    /// Indicates that a class/component is not thread safe
    /// and is only good for one time use.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class SingleUseAttribute : Attribute
    {
    }

    public class Needs
    {
        public delegate Type Source(Type type);

        private readonly Dictionary<Type, Func<object>> resolvers = new Dictionary<Type, Func<object>>();
        private readonly List<Source> sources = new List<Source>();

        public void Add<T>(T instance)
        {
            Add(typeof(T), instance);
        }

        public void Add(Type type, object instance)
        {
            resolvers[type] = () => instance;
        }

        public void Refer(Type parent)
        {
            Refer(ParentSouce(parent));
        }

        public void Refer(Assembly assembly)
        {
            Refer(AssemblySource(assembly));
        }

        public void Refer(Source source)
        {
            sources.Add(source);
        }

        private static Source ParentSouce(Type parent)
        {
            return type => FindImpl(type, parent.GetNestedTypes());
        }

        private static Source AssemblySource(Assembly assembly)
        {
            return type => FindImpl(type, assembly.GetExportedTypes());
        }

        private static Type FindImpl(Type type, IEnumerable<Type> types)
        {
            return types.FirstOrDefault(t => t.GetInterfaces().Any(x => x == type));
        }

        public T Get<T>()
        {
            return (T) Get(typeof(T));
        }

        public object Get(Type type)
        {
            return GetInternal(type, !IsSingleUse(type));
        }

        private object GetInternal(Type type, bool multiUse)
        {
            Func<object> resolver;

            if (resolvers.TryGetValue(type, out resolver))
            {
                return resolver();
            }

            var implType = sources.Select(s => s(type)).FirstOrDefault();

            if (implType != null)
            {
                return Persist(type, implType, multiUse);
            }

            throw new Exception($"No implementation found for {type}");
        }

        private object Persist(Type type, Type implType, bool multiUse)
        {
            if (IsSingleUse(implType))
            {
                Func<object> resolver = () => New(implType, multiUse);
                resolvers[type] = resolver;
                return resolver();
            }

            var obj = New(implType, multiUse);
            resolvers[type] = () => obj;
            return obj;
        }

        private object New(Type type, bool multiUse)
        {
            var ctors = type.GetConstructors();

            if (ctors.Length != 1)
            {
                throw new Exception($"Type {type} must have exactly 1 constructor, but has {ctors.Length}");
            }

            var ctor = ctors[0];
            var args = ctor.GetParameters()
                .Select(p => GetInternal(p.ParameterType, multiUse))
                .ToArray();

            if (multiUse)
            {
                foreach (var argType in args.Select(x => x.GetType()))
                {
                    if (IsSingleUse(argType))
                    {
                        throw new Exception($"Non-SingleUse object ({type}) cannot depend on SingleUse object ({argType})");
                    }
                }
            }

            return ctor.Invoke(args);
        }

        private static bool IsSingleUse(MemberInfo type)
        {
            return type.GetCustomAttribute(typeof(SingleUseAttribute)) != null;
        }
    }
}
