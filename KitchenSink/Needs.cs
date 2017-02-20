using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace KitchenSink
{
    /// <summary>
    /// Indicates that a class/component is not thread safe, or has transient state
    /// and is only good for one time use.
    /// Multi-use classes cannot depend on single-use classes.
    /// Classes are multi-use by default.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class SingleUseAttribute : Attribute
    {
    }

    public class Needs
    {
        /// <summary>
        /// Returns a type implementing the given type, or null.
        /// </summary>
        public delegate Type Source(Type contractType);

        /// <summary>
        /// Returns an object implementing an expected type.
        /// </summary>
        public delegate object Factory();

        public delegate object Backup(Type contractType);

        private readonly Dictionary<Type, Factory> factories = new Dictionary<Type, Factory>();
        private readonly List<Source> sources = new List<Source>();
        private readonly List<Backup> backups = new List<Backup>();

        /// <summary>
        /// Specifies an implementing object for a given contract type.
        /// </summary>
        public Needs Add<Contract>(Contract impl)
        {
            return Add(typeof(Contract), impl);
        }

        /// <summary>
        /// Specifies an implementing object for a given contract type.
        /// </summary>
        public Needs Add(Type contractType, object impl)
        {
            return Add(contractType, () => impl);
        }

        /// <summary>
        /// Provides a factory for building an object implementing a given contract type.
        /// </summary>
        public Needs Add(Type contractType, Factory factory)
        {
            factories[contractType] = factory;
            return this;
        }

        /// <summary>
        /// Registers the nested types of the given parent type as a source of implementations.
        /// </summary>
        public Needs Refer(Type parent)
        {
            return Refer(contractType => FindImplType(contractType, parent.GetNestedTypes()));
        }

        /// <summary>
        /// Registers the exported types in the given assembly as a source of implementations.
        /// </summary>
        public Needs Refer(Assembly assembly)
        {
            return Refer(contractType => FindImplType(contractType, assembly.GetExportedTypes()));
        }

        /// <summary>
        /// Registers the given delegate as an arbitrary source of implementations.
        /// </summary>
        public Needs Refer(Source source)
        {
            sources.Add(source);
            return this;
        }

        private static Type FindImplType(Type contractType, IEnumerable<Type> types)
        {
            return types.FirstOrDefault(t => t.GetInterfaces().Any(x => x == contractType));
        }

        /// <summary>
        /// Registers the given Needs as a backup to this Needs.
        /// </summary>
        public Needs Defer(Needs needs)
        {
            return Defer(contractType =>
            {
                try
                {
                    return needs.Get(contractType);
                }
                catch (NotImplementedException)
                {
                    return null;
                }
            });
        }

        /// <summary>
        /// Registers the given delegate as an arbitrary backup to this Needs.
        /// Backup should not throw and should return null to indicate resolution failure.
        /// </summary>
        public Needs Defer(Backup backup)
        {
            backups.Add(backup);
            return this;
        }

        /// <summary>
        /// Resolves an implementing object for the given contract type.
        /// </summary>
        /// <exception cref="NotImplementedException">If no implementation found.</exception>
        public Contract Get<Contract>()
        {
            return (Contract) Get(typeof(Contract));
        }

        /// <summary>
        /// Resolves an implementing object for the given contract type.
        /// </summary>
        /// <exception cref="NotImplementedException">If no implementation found.</exception>
        public object Get(Type contractType)
        {
            return GetInternal(contractType, !IsSingleUse(contractType));
        }

        private object GetInternal(Type contractType, bool multiUse)
        {
            Factory factory;

            if (factories.TryGetValue(contractType, out factory))
            {
                return factory();
            }

            foreach (var source in sources)
            {
                var implType = source(contractType);

                if (implType != null)
                {
                    return Persist(contractType, implType, multiUse);
                }
            }

            foreach (var backup in backups)
            {
                var impl = backup(contractType);

                if (impl != null)
                {
                    return impl;
                }
            }

            throw new NotImplementedException($"No implementation found for {contractType}");
        }

        private object Persist(Type contractType, Type implType, bool multiUse)
        {
            if (IsSingleUse(implType))
            {
                Factory factory = () => New(implType, multiUse);
                factories[contractType] = factory;
                return factory();
            }

            var impl = New(implType, multiUse);
            factories[contractType] = () => impl;
            return impl;
        }

        private object New(Type implType, bool multiUse)
        {
            var ctors = implType.GetConstructors();

            if (ctors.Length != 1)
            {
                throw new Exception($"Type {implType} must have exactly 1 constructor, but has {ctors.Length}");
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
                        throw new Exception($"MultiUse class ({implType}) cannot depend on SingleUse class ({argType})");
                    }
                }
            }

            return ctor.Invoke(args);
        }

        private static bool IsSingleUse(MemberInfo implType)
        {
            return implType.GetCustomAttribute(typeof(SingleUseAttribute)) != null;
        }
    }
}
