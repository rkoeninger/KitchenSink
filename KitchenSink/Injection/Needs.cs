using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using KitchenSink.Extensions;
using static KitchenSink.Operators;

namespace KitchenSink.Injection
{
    /// <summary>
    /// A simple IoC container. Resolves dependencies that are explicity
    /// registered, by discovering them in assemblies and parent classes,
    /// or by deferring to backup providers.
    /// </summary>
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

        /// <summary>
        /// A backup resolver that this Needs can defer to if it is unable to
        /// resolve a dependency.
        /// </summary>
        public delegate Maybe<object> Backup(Type contractType);

        private readonly Dictionary<Type, Factory> factories = new Dictionary<Type, Factory>();
        private readonly List<Source> sources = new List<Source>();
        private readonly List<Backup> backups = new List<Backup>();

        /// <summary>
        /// Specifies an implementing object for a given contract type.
        /// </summary>
        public Needs Add<TContract>(TContract impl)
        {
            return Add(typeof(TContract), impl);
        }

        /// <summary>
        /// Specifies an implementing object for a given contract type.
        /// </summary>
        public Needs Add(Type contractType, object impl)
        {
            return Add(contractType, () => impl);
        }

        /// <summary>
        /// Specifies an implementing type for a given contract type.
        /// </summary>
        public Needs Add<TContract>(Type implType)
        {
            return Add(typeof(TContract), implType);
        }

        /// <summary>
        /// Specifies an implementing type for a given contract type.
        /// </summary>
        public Needs Add<TContract, TImplementation>()
        {
            return Add(typeof(TContract), typeof(TImplementation));
        }

        /// <summary>
        /// Specifies an implementing type for a given contract type.
        /// </summary>
        public Needs Add(Type contractType, Type implType)
        {
            Persist(contractType, implType, !implType.HasAttribute<SingleUse>());
            return this;
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
            return Refer(SourceFrom(parent.GetNestedTypes()));
        }

        /// <summary>
        /// Registers the exported types in the given assembly as a source of implementations.
        /// </summary>
        public Needs Refer(Assembly assembly)
        {
            return Refer(SourceFrom(assembly.GetExportedTypes()));
        }

        /// <summary>
        /// Registers the given delegate as an arbitrary source of implementations.
        /// </summary>
        public Needs Refer(Source source)
        {
            sources.Add(source);
            return this;
        }

        private static Source SourceFrom(IEnumerable<Type> types)
        {
            return contractType => types.FirstOrDefault(t => t.GetInterfaces().Contains(contractType));
        }

        /// <summary>
        /// Registers the given Needs as a backup to this Needs.
        /// </summary>
        public Needs Defer(Needs needs)
        {
            return Defer(needs.GetMaybe);
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
        /// <exception cref="ImplementationUnresolvedException">If no implementation found.</exception>
        public TContract Get<TContract>()
        {
            return (TContract) Get(typeof(TContract));
        }

        /// <summary>
        /// Resolves an implementing object for the given contract type.
        /// </summary>
        /// <exception cref="ImplementationUnresolvedException">If no implementation found.</exception>
        public object Get(Type contractType)
        {
            return GetMaybe(contractType).OrElseThrow(new ImplementationUnresolvedException(contractType));
        }

        /// <summary>
        /// Resolves Some implementing object for the given contract type.
        /// Returns None if no implementation found.
        /// </summary>
        public Maybe<object> GetMaybe(Type contractType)
        {
            return GetInternal(contractType, !contractType.HasAttribute<SingleUse>());
        }

        // Resolves dependency by checking for existing Factory,
        // then checking list of Sources, then checking list of Backups.
        // Throws NotImplementedException if impl is not found.
        private Maybe<object> GetInternal(Type contractType, bool multiUse)
        {
            Factory factory;

            if (factories.TryGetValue(contractType, out factory))
            {
                return some(factory());
            }

            foreach (var source in sources)
            {
                var implType = source(contractType);

                if (implType != null)
                {
                    return some(Persist(contractType, implType, multiUse));
                }
            }

            foreach (var backup in backups)
            {
                var implMaybe = backup(contractType);

                if (implMaybe.HasValue)
                {
                    return implMaybe;
                }
            }

            return none<object>();
        }

        // Create and store Factory. Factory returns singleton instance if
        // implType is multi-use, returns new instance on each call if single-use.
        private object Persist(Type contractType, Type implType, bool multiUse)
        {
            if (implType.HasAttribute<SingleUse>())
            {
                Factory factory = () => New(contractType, implType, multiUse);
                factories[contractType] = factory;
                return factory();
            }

            var impl = New(contractType, implType, multiUse);
            factories[contractType] = () => impl;
            return impl;
        }

        // Resolve all nested dependencies and create instance.
        private object New(Type contractType, Type implType, bool multiUse)
        {
            var ctors = implType.GetConstructors();

            if (ctors.Length != 1)
            {
                throw new MultipleConstructorsException(contractType, implType, ctors.Length);
            }

            var ctor = ctors[0];
            var args = ctor.GetParameters()
                .Select(p => GetInternal(p.ParameterType, multiUse))
                .Select(m => m.OrElseThrow(new ImplementationUnresolvedException(m.InnerType)))
                .ToArray();

            if (multiUse)
            {
                foreach (var arg in args)
                {
                    var argType = arg.GetType();

                    if (argType.HasAttribute<SingleUse>())
                    {
                        throw new ImplementationReliabilityException(contractType, implType, argType);
                    }
                }
            }

            return ctor.Invoke(args);
        }
    }
}
