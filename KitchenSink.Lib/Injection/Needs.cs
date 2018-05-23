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
    /// specified, by discovering them in registered assemblies and classes,
    /// or by deferring to backup containers.
    /// </summary>
    public class Needs
    {
        // Resolvers use an interface instead of Func so it can
        // be determined if a given Resolver concerns a decorator
        private interface IResolver
        {
            object Resolve();
        }

        private class SingleUseResolver : IResolver
        {
            private Needs Container { get; }
            private Type ContractType { get; }
            private Type ImplementationType { get; }

            public SingleUseResolver(Needs needs, Type contractType, Type implType)
            {
                Container = needs;
                ContractType = contractType;
                ImplementationType = implType;
            }

            public object Resolve() => Container.New(ContractType, ImplementationType);
        }

        private class MultiUseResolver : IResolver
        {
            private Lazy<object> Instance { get; }

            public MultiUseResolver(object impl)
            {
                Instance = new Lazy<object>(() => impl);
            }

            public object Resolve() => Instance.Value;
        }

        // Decorators that are multi-use must
        // decorate objects that are multi-use.
        // This DecoratorResolver is for SingleUseResolvers.
        // MultiUseResolvers just get replaced with another
        // MultiUseResolver returning a decorated instance.
        private class SingleUseDecoratorResolver : IResolver
        {
            private IResolver Resolver { get; }
            private Func<object, object> Decorate { get; }

            public SingleUseDecoratorResolver(IResolver resolver, Func<object, object> decorate)
            {
                Resolver = resolver;
                Decorate = decorate;
            }

            public object Resolve()
            {
                var inner = Resolver.Resolve();
                var outer = Decorate(inner);
                CheckReliablitiy(inner.GetType(), outer.GetType(), inner.GetType());
                return outer;
            }
        }

        /// <summary>
        /// Returns a type implementing the given type, or null.
        /// </summary>
        public delegate Maybe<Type> Source(Type contractType);

        /// <summary>
        /// A backup resolver that this Needs can defer to if it is unable to
        /// resolve a dependency.
        /// </summary>
        public delegate Maybe<object> Backup(Type contractType);

        private readonly Dictionary<Type, IResolver> resolvers = new Dictionary<Type, IResolver>();
        private readonly List<Source> sources = new List<Source>();
        private readonly List<Backup> backups = new List<Backup>();

        /// <summary>
        /// Specifies an implementing object for a given contract type.
        /// </summary>
        public Needs Add<TContract>(TContract impl) => Add(typeof(TContract), impl);

        /// <summary>
        /// Specifies a multi-use implementing object for a given contract type.
        /// </summary>
        public Needs Add(Type contractType, object impl)
        {
            if (!contractType.IsInstanceOfType(impl))
            {
                throw new InvalidImplementationException(contractType, impl.GetType());
            }

            resolvers[contractType] = new MultiUseResolver(impl);
            return this;
        }

        /// <summary>
        /// Specifies an implementing type for a given contract type.
        /// </summary>
        public Needs Add<TContract>(Type implType) => Add(typeof(TContract), implType);

        /// <summary>
        /// Specifies an implementing type for a given contract type.
        /// </summary>
        public Needs Add<TContract, TImplementation>() => Add(typeof(TContract), typeof(TImplementation));

        /// <summary>
        /// Specifies an implementing type for a given contract type.
        /// </summary>
        public Needs Add(Type contractType, Type implType)
        {
            if (!contractType.IsAssignableFrom(implType))
            {
                throw new InvalidImplementationException(contractType, implType);
            }

            Persist(contractType, implType);
            return this;
        }

        /// <summary>
        /// Decorates specified contract type with decorator type.
        /// </summary>
        public Needs Decorate<TContract, TDecorator>() where TDecorator : TContract =>
            Decorate(typeof(TContract), typeof(TDecorator));

        /// <summary>
        /// Decorates specified contract type with decorator type.
        /// </summary>
        public Needs Decorate(Type contractType, Type decoratorType)
        {
            CheckReliablitiy(contractType, decoratorType, contractType);
            return Decorate(contractType, x => Activator.CreateInstance(decoratorType, x));
        }

        /// <summary>
        /// Decorates specified contract type with decorator function.
        /// </summary>
        public Needs Decorate<TContract>(Func<TContract, TContract> decorator) =>
            Decorate(typeof(TContract), x => decorator((TContract) x));

        /// <summary>
        /// Decorates specified contract type with decorator function.
        /// </summary>
        public Needs Decorate(Type contractType, Func<object, object> decorate)
        {
            if (resolvers.TryGetValue(contractType, out var resolver))
            {
                if (resolver is MultiUseResolver)
                {
                    resolvers[contractType] = new MultiUseResolver(decorate(resolver.Resolve()));
                }
                else
                {
                    resolvers[contractType] = new SingleUseDecoratorResolver(resolver, decorate);
                }
            }
            else
            {
                var inner = GetMaybe(contractType).OrElseThrow(new ImplementationUnresolvedException(contractType));
                var outer = decorate(inner);
                CheckReliablitiy(contractType, outer.GetType(), inner.GetType());
                resolvers[contractType] = new MultiUseResolver(outer);
            }

            return this;
        }

        /// <summary>
        /// Registers the given types as sources of implementations.
        /// </summary>
        public Needs Refer(params Type[] impls) => Refer(SourceFrom(impls));

        /// <summary>
        /// Registers the given types as sources of implementations.
        /// </summary>
        public Needs Refer(IEnumerable<Type> impls) => Refer(SourceFrom(impls));

        /// <summary>
        /// Registers the nested types of the given parent type as a source of implementations.
        /// </summary>
        public Needs ReferChildren(Type parent) => Refer(SourceFrom(parent.GetNestedTypes()));

        /// <summary>
        /// Registers the exported types in the given assembly as a source of implementations.
        /// </summary>
        public Needs Refer(Assembly assembly) => Refer(assembly, _ => true);

        /// <summary>
        /// Registers the exported types in the given assembly as a source of implementations, filtered per given function.
        /// </summary>
        public Needs Refer(Assembly assembly, Func<Type, bool> filter) =>
            Refer(SourceFrom(assembly.GetExportedTypes().Where(filter)));

        /// <summary>
        /// Registers the given delegate as an arbitrary source of implementations.
        /// </summary>
        public Needs Refer(Source source)
        {
            sources.Add(source);
            return this;
        }

        /// <summary>
        /// Registers the given Needs as a backup to this Needs.
        /// </summary>
        public Needs Defer(Needs needs) => Defer(needs.GetMaybe);

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
        public TContract Get<TContract>() => (TContract) Get(typeof(TContract));

        /// <summary>
        /// Resolves an implementing object for the given contract type.
        /// </summary>
        /// <exception cref="ImplementationUnresolvedException">If no implementation found.</exception>
        public object Get(Type contractType) =>
            GetMaybe(contractType).OrElseThrow(new ImplementationUnresolvedException(contractType));

        /// <summary>
        /// Resolves Some implementing object for the given contract type.
        /// Returns None if no implementation found.
        /// </summary>
        public Maybe<object> GetMaybe(Type contractType) => GetInternal(contractType);

        // Resolves dependency by checking for existing Factory,
        // then checking list of Sources, then checking list of Backups.
        // Throws ImplementationUnresolvedException if impl is not found.
        private Maybe<object> GetInternal(Type contractType)
        {
            return resolvers.GetMaybe(contractType).Select(r => r.Resolve())
                .OrDo(() => sources.FirstSome(s => s(contractType)).Select(t => Persist(contractType, t)))
                .OrDo(() => backups.FirstSome(b => b(contractType)));
        }

        // Create and store Resolver. Resolver returns same instance for each call if
        // implType is multi-use, returns new instance on each call if single-use.
        private object Persist(Type contractType, Type implType)
        {
            if (IsSingleUse(implType))
            {
                var resolver = new SingleUseResolver(this, contractType, implType);
                resolvers[contractType] = resolver;
                return resolver.Resolve();
            }

            var impl = New(contractType, implType);
            resolvers[contractType] = new MultiUseResolver(impl);
            return impl;
        }

        // Resolve all nested dependencies and create instance.
        private object New(Type contractType, Type implType)
        {
            var ctor = GetConstructor(implType);
            var args = new List<object>();

            foreach (var param in ctor.GetParameters())
            {
                var arg = GetInternal(param.ParameterType)
                    .OrElseThrow(new ImplementationUnresolvedException(param.ParameterType));
                var argType = arg.GetType();
                CheckReliablitiy(contractType, implType, argType);
                args.Add(arg);
            }

            return ctor.Invoke(args.ToArray());
        }

        // Explicitly reject decorators
        private static Source SourceFrom(IEnumerable<Type> types) =>
            contractType =>
                types.FirstMaybe(t =>
                    t.GetInterfaces().Contains(contractType) && !IsDecoratorOf(t, contractType));

        private static ConstructorInfo GetConstructor(Type type)
        {
            var ctors = type.GetConstructors();

            if (ctors.Length != 1)
            {
                throw new MultipleConstructorsException(type, ctors.Length);
            }

            return ctors[0];
        }

        private static void CheckReliablitiy(Type contractType, Type implType, Type argType)
        {
            if (Not(IsSingleUse(implType)) && IsSingleUse(argType))
            {
                throw new ImplementationReliabilityException(contractType, implType, argType);
            }
        }

        private static bool IsSingleUse(Type type) =>
            type.HasAttribute<SingleUse>()
            || type.GetMembers(BindingFlags.NonPublic).Any(m => m.Name.IsSimilar("DeclareSingleUse"));

        private static bool IsDecoratorOf(Type decoratorType, Type contractType) =>
            decoratorType.GetConstructors().Any(c => c.GetParameters().Any(p => p.ParameterType == contractType));
    }
}
