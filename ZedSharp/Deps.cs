using System;

namespace ZedSharp
{
    public class Deps
    {
        public static Deps Of(params Object[] vals)
        {
            var deps = new Deps();

            foreach (var val in vals)
                deps.Tree.Set(val.GetType(), val);

            return deps;
        }

        public Deps()
        {
            Tree = new TypeTree<Object>();
        }

        private readonly TypeTree<Object> Tree;

        public Deps Set<T>(Object impl)
        {
            Tree.Set<T>(impl);
            return this;
        }

        public Maybe<T> Get<T>() where T : class
        {
            var t = typeof(T);
            return Tree.Get(t)
                .OrEvalMany(t, GetDefaultImpl)
                .Cast<T>();
        }

        private static Maybe<Object> GetDefaultImpl(Type @interface)
        {
            return @interface.GetAttribute<DefaultImplementationAttribute>()
                .Select(x => x.ImplementingClass)
                .OrEvalMany(() =>
                    Types.All(t => t.GetAttribute<DefaultImplementationOfAttribute>(a => a.ImplementedInterface == @interface).HasValue)
                    .SingleMaybe())
                .Select(Activator.CreateInstance);
        }
    }
}
