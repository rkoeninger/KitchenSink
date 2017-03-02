using System;
using System.Collections.Generic;
using System.Linq;

// TODO: Absorb
// Builders need to implement IClause so they can be properly nested
// for both Cond and Case

namespace KitchenSink.Control
{
    public interface ICaseInitialWhen<TKey>
    {
        ICaseThen<TKey> Then(Action<TKey> consequent);
        ICaseThen<TKey, TResult> Then<TResult>(Func<TKey, TResult> consequent);
    }

    public interface ICaseSubtypeInitialWhen<TKey, out TSubtype> where TSubtype : TKey
    {
        ICaseThen<TKey> Then(Action<TSubtype> consequent);
        ICaseThen<TKey, TResult> Then<TResult>(Func<TSubtype, TResult> consequent);
    }

    public interface ICaseInitialThen<TKey>
    {
        ICaseInitialWhen<TKey> When(Func<TKey, bool> condition);
        ICaseSubtypeInitialWhen<TKey, TSubtype> When<TSubtype>(Func<TSubtype, bool> condition) where TSubtype : TKey;
        ICaseSubtypeInitialWhen<TKey, TSubtype> When<TSubtype>() where TSubtype : TKey;
        ICaseThen<TKey, TResult> Returns<TResult>();
        ICaseDefaultThen<TKey> Default(Action<TKey> alternative);
        ICaseDefaultThen<TKey, TResult> Default<TResult>(Func<TKey, TResult> alternative);
    }

    public interface ICaseWhen<TKey>
    {
        ICaseThen<TKey> Then(Action<TKey> consequent);
    }

    public interface ICaseSubtypeWhen<TKey, out TSubtype> where TSubtype : TKey
    {
        ICaseThen<TKey> Then(Action<TSubtype> consequent);
    }

    public interface ICaseThen<TKey>
    {
        ICaseWhen<TKey> When(Func<TKey, bool> condition);
        ICaseSubtypeWhen<TKey, TSubtype> When<TSubtype>(Func<TSubtype, bool> condition) where TSubtype : TKey;
        ICaseSubtypeWhen<TKey, TSubtype> When<TSubtype>() where TSubtype : TKey;
        ICaseDefaultThen<TKey> Default(Action<TKey> alternative);
        bool End();
    }

    public interface ICaseDefaultWhen<TKey>
    {
        ICaseDefaultThen<TKey> Then(Action<TKey> consequent);
    }

    public interface ICaseDefaultSubtypeWhen<TKey, out TSubtype> where TSubtype : TKey
    {
        ICaseDefaultThen<TKey> Then(Action<TSubtype> consequent);
    }

    public interface ICaseDefaultThen<TKey>
    {
        ICaseDefaultWhen<TKey> When(Func<TKey, bool> condition);
        ICaseDefaultSubtypeWhen<TKey, TSubtype> When<TSubtype>(Func<TSubtype, bool> condition) where TSubtype : TKey;
        ICaseDefaultSubtypeWhen<TKey, TSubtype> When<TSubtype>() where TSubtype : TKey;
        void End();
    }

    public interface ICaseWhen<TKey, TResult>
    {
        ICaseThen<TKey, TResult> Then(Func<TKey, TResult> consequent);
    }
    
    public interface ICaseSubtypeWhen<TKey, out TSubtype, TResult> where TSubtype : TKey
    {
        ICaseThen<TKey, TResult> Then(Func<TSubtype, TResult> consequent);
    }

    public interface ICaseThen<TKey, TResult>
    {
        ICaseWhen<TKey, TResult> When(Func<TKey, bool> condition);
        ICaseSubtypeWhen<TKey, TSubtype, TResult> When<TSubtype>(Func<TSubtype, bool> condition) where TSubtype : TKey;
        ICaseSubtypeWhen<TKey, TSubtype, TResult> When<TSubtype>() where TSubtype : TKey;
        ICaseDefaultThen<TKey, TResult> Default(Func<TKey, TResult> alternative);
        Maybe<TResult> End();
    }

    public interface ICaseDefaultWhen<TKey, TResult>
    {
        ICaseDefaultThen<TKey, TResult> Then(Func<TKey, TResult> consequent);
    }

    public interface ICaseDefaultSubtypeWhen<TKey, out TSubtype, TResult> where TSubtype : TKey
    {
        ICaseDefaultThen<TKey, TResult> Then(Func<TSubtype, TResult> consequent);
    }

    public interface ICaseDefaultThen<TKey, TResult>
    {
        ICaseDefaultWhen<TKey, TResult> When(Func<TKey, bool> condition);
        ICaseDefaultSubtypeWhen<TKey, TSubtype, TResult> When<TSubtype>(Func<TSubtype, bool> condition) where TSubtype : TKey;
        ICaseDefaultSubtypeWhen<TKey, TSubtype, TResult> When<TSubtype>() where TSubtype : TKey;
        TResult End();
    }

    public static class Case<TKey, TResult>
    {
        public static ICaseThen<TKey, TResult> Of(TKey key)
        {
            return Case.Of(key).Returns<TResult>();
        }
    }

    public static class Case<TKey>
    {
        public static ICaseInitialThen<TKey> Of(TKey key)
        {
            return Case.Of(key);
        }

        public static ICaseThen<TKey, TResult> Of<TResult>(TKey key)
        {
            return Case.Of(key).Returns<TResult>();
        }
    }

    public static class Case
    {
        public static ICaseInitialThen<TKey> Of<TKey>(TKey key)
        {
            return new CaseBuilderInitial<TKey>(key);
        }

        public static ICaseThen<TKey, TResult> Of<TKey, TResult>(TKey key)
        {
            return Of(key).Returns<TResult>();
        }

        private interface IClause<in TKey>
        {
            bool Eval(TKey key);
        }

        private interface IClause<in TKey, TResult>
        {
            Maybe<TResult> Eval(TKey key);
        }

        private class ScalarClause<TKey> : IClause<TKey>
        {
            public Func<TKey, bool> Condition { private get; set; }
            public Action<TKey> Consequent { private get; set; }

            public bool Eval(TKey key)
            {
                if (Condition(key))
                {
                    Consequent(key);
                    return true;
                }

                return false;
            }
        }

        private class NestedClause<TKey> : IClause<TKey>
        {
            public ICaseThen<TKey> Builder { private get; set; }

            public bool Eval(TKey key)
            {
                return Builder.End();
            }
        }

        private class ScalarClause<TKey, TResult> : IClause<TKey, TResult>
        {
            public Func<TKey, bool> Condition { private get; set; }
            public Func<TKey, TResult> Consequent { private get; set; }

            public Maybe<TResult> Eval(TKey key)
            {
                return Condition(key) ? Maybe.Some(Consequent(key)) : Maybe<TResult>.None;
            }
        }

        private class NestedClause<TKey, TResult> : IClause<TKey, TResult>
        {
            public ICaseThen<TKey, TResult> Builder { private get; set; }

            public Maybe<TResult> Eval(TKey key)
            {
                return Builder.End();
            }
        }

        private class CaseBuilderInitial<TKey> : ICaseInitialThen<TKey>, ICaseInitialWhen<TKey>
        {
            private readonly TKey key;
            private Func<TKey, bool> pending;

            public CaseBuilderInitial(TKey key)
            {
                this.key = key;
            }

            public ICaseInitialWhen<TKey> When(Func<TKey, bool> condition)
            {
                pending = condition;
                return this;
            }

            public ICaseSubtypeInitialWhen<TKey, TSubtype> When<TSubtype>(Func<TSubtype, bool> condition) where TSubtype : TKey
            {
                return new CaseSubtypeInitialBuilder<TKey, TSubtype>(this, condition);
            }

            public ICaseSubtypeInitialWhen<TKey, TSubtype> When<TSubtype>() where TSubtype : TKey
            {
                return new CaseSubtypeInitialBuilder<TKey, TSubtype>(this, _ => true);
            }

            public ICaseThen<TKey> Then(Action<TKey> consequent)
            {
                return new CaseBuilder<TKey>(key).When(pending).Then(consequent);
            }

            public ICaseThen<TKey, TResult> Then<TResult>(Func<TKey, TResult> consequent)
            {
                return new CaseBuilder<TKey, TResult>(key).When(pending).Then(consequent);
            }

            public ICaseThen<TKey, TResult> Returns<TResult>()
            {
                return new CaseBuilder<TKey, TResult>(key);
            }

            public ICaseDefaultThen<TKey> Default(Action<TKey> alternative)
            {
                return new CaseBuilderDefault<TKey>(key, new IClause<TKey>[0], alternative);
            }

            public ICaseDefaultThen<TKey, TResult> Default<TResult>(Func<TKey, TResult> alternative)
            {
                return new CaseBuilderDefault<TKey, TResult>(key, new IClause<TKey, TResult>[0], alternative);
            }
        }

        private class CaseBuilder<TKey> : ICaseThen<TKey>, ICaseWhen<TKey>
        {
            private readonly TKey key;
            private Func<TKey, bool> pending;
            private readonly List<IClause<TKey>> clauses = new List<IClause<TKey>>();

            public CaseBuilder(TKey key)
            {
                this.key = key;
            }

            public ICaseWhen<TKey> When(Func<TKey, bool> condition)
            {
                pending = condition;
                return this;
            }

            public ICaseSubtypeWhen<TKey, TSubtype> When<TSubtype>(Func<TSubtype, bool> condition) where TSubtype : TKey
            {
                return new CaseSubtypeBuilder<TKey, TSubtype>(this, condition);
            }

            public ICaseSubtypeWhen<TKey, TSubtype> When<TSubtype>() where TSubtype : TKey
            {
                return new CaseSubtypeBuilder<TKey, TSubtype>(this, _ => true);
            }

            public ICaseThen<TKey> Then(Action<TKey> consequent)
            {
                clauses.Add(new ScalarClause<TKey>
                {
                    Condition = pending,
                    Consequent = consequent
                });
                return this;
            }

            public ICaseDefaultThen<TKey> Default(Action<TKey> alternative)
            {
                return new CaseBuilderDefault<TKey>(key, clauses, alternative);
            }

            public bool End()
            {
                return clauses.Any(x => x.Eval(key));
            }
        }

        private class CaseBuilderDefault<TKey> : ICaseDefaultThen<TKey>, ICaseDefaultWhen<TKey>
        {
            private readonly TKey key;
            private Func<TKey, bool> pending;
            private readonly List<IClause<TKey>> clauses = new List<IClause<TKey>>();
            private readonly Action<TKey> alternative;

            public CaseBuilderDefault(TKey key, IEnumerable<IClause<TKey>> clauses, Action<TKey> alternative)
            {
                this.key = key;
                this.clauses.AddRange(clauses);
                this.alternative = alternative;
            }

            public ICaseDefaultWhen<TKey> When(Func<TKey, bool> condition)
            {
                pending = condition;
                return this;
            }

            public ICaseDefaultSubtypeWhen<TKey, TSubtype> When<TSubtype>(Func<TSubtype, bool> condition) where TSubtype : TKey
            {
                return new CaseDefaultSubtypeBuilder<TKey, TSubtype>(this, condition);
            }

            public ICaseDefaultSubtypeWhen<TKey, TSubtype> When<TSubtype>() where TSubtype : TKey
            {
                return new CaseDefaultSubtypeBuilder<TKey, TSubtype>(this, _ => true);
            }

            public ICaseDefaultThen<TKey> Then(Action<TKey> consequent)
            {
                clauses.Add(new ScalarClause<TKey>
                {
                    Condition = pending,
                    Consequent = consequent
                });
                return this;
            }

            public void End()
            {
                if (!clauses.Any(x => x.Eval(key)))
                {
                    alternative(key);
                }
            }
        }

        private class CaseBuilder<TKey, TResult> : ICaseThen<TKey, TResult>, ICaseWhen<TKey, TResult>
        {
            private readonly TKey key;
            private Func<TKey, bool> pending;
            private readonly List<IClause<TKey, TResult>> clauses = new List<IClause<TKey, TResult>>();

            public CaseBuilder(TKey key)
            {
                this.key = key;
            }

            public ICaseWhen<TKey, TResult> When(Func<TKey, bool> condition)
            {
                pending = condition;
                return this;
            }

            public ICaseSubtypeWhen<TKey, TSubtype, TResult> When<TSubtype>(Func<TSubtype, bool> condition) where TSubtype : TKey
            {
                return new CaseSubtypeBuilder<TKey, TSubtype, TResult>(this, condition);
            }

            public ICaseSubtypeWhen<TKey, TSubtype, TResult> When<TSubtype>() where TSubtype : TKey
            {
                return new CaseSubtypeBuilder<TKey, TSubtype, TResult>(this, _ => true);
            }

            public ICaseThen<TKey, TResult> Then(Func<TKey, TResult> consequent)
            {
                clauses.Add(new ScalarClause<TKey, TResult>
                {
                    Condition = pending,
                    Consequent = consequent
                });
                return this;
            }

            public ICaseDefaultThen<TKey, TResult> Default(Func<TKey, TResult> alternative)
            {
                return new CaseBuilderDefault<TKey, TResult>(key, clauses, alternative);
            }

            public Maybe<TResult> End()
            {
                return clauses.FirstSome(x => x.Eval(key));
            }
        }

        private class CaseBuilderDefault<TKey, TResult> : ICaseDefaultThen<TKey, TResult>, ICaseDefaultWhen<TKey, TResult>
        {
            private readonly TKey key;
            private Func<TKey, bool> pending;
            private readonly List<IClause<TKey, TResult>> clauses = new List<IClause<TKey, TResult>>();
            private readonly Func<TKey, TResult> alternative;

            public CaseBuilderDefault(TKey key, IEnumerable<IClause<TKey, TResult>> clauses, Func<TKey, TResult> alternative)
            {
                this.key = key;
                this.clauses.AddRange(clauses);
                this.alternative = alternative;
            }

            public ICaseDefaultWhen<TKey, TResult> When(Func<TKey, bool> condition)
            {
                pending = condition;
                return this;
            }

            public ICaseDefaultSubtypeWhen<TKey, TSubtype, TResult> When<TSubtype>(Func<TSubtype, bool> condition) where TSubtype : TKey
            {
                return new CaseDefaultSubtypeBuilder<TKey, TSubtype, TResult>(this, condition);
            }

            public ICaseDefaultSubtypeWhen<TKey, TSubtype, TResult> When<TSubtype>() where TSubtype : TKey
            {
                return new CaseDefaultSubtypeBuilder<TKey, TSubtype, TResult>(this, _ => true);
            }

            public ICaseDefaultThen<TKey, TResult> Then(Func<TKey, TResult> consequent)
            {
                clauses.Add(new ScalarClause<TKey, TResult>
                {
                    Condition = pending,
                    Consequent = consequent
                });
                return this;
            }

            public TResult End()
            {
                return clauses.FirstSome(x => x.Eval(key)).OrElseEval(key, alternative);
            }
        }

        private class CaseSubtypeInitialBuilder<TKey, TSubtype> : ICaseSubtypeInitialWhen<TKey, TSubtype>
            where TSubtype : TKey
        {
            private readonly ICaseInitialThen<TKey> builder;
            private readonly Func<TSubtype, bool> condition;

            public CaseSubtypeInitialBuilder(ICaseInitialThen<TKey> builder, Func<TSubtype, bool> condition)
            {
                this.builder = builder;
                this.condition = condition;
            }

            public ICaseThen<TKey> Then(Action<TSubtype> consequent)
            {
                return builder
                    .When(x => x is TSubtype && condition((TSubtype) x))
                    .Then(x => consequent((TSubtype) x));
            }

            public ICaseThen<TKey, TResult> Then<TResult>(Func<TSubtype, TResult> consequent)
            {
                return builder
                    .When(x => x is TSubtype && condition((TSubtype)x))
                    .Then(x => consequent((TSubtype)x));
            }
        }

        private class CaseSubtypeBuilder<TKey, TSubtype> : ICaseSubtypeWhen<TKey, TSubtype>
            where TSubtype : TKey
        {
            private readonly ICaseThen<TKey> builder;
            private readonly Func<TSubtype, bool> condition;

            public CaseSubtypeBuilder(ICaseThen<TKey> builder, Func<TSubtype, bool> condition)
            {
                this.builder = builder;
                this.condition = condition;
            }

            public ICaseThen<TKey> Then(Action<TSubtype> consequent)
            {
                return builder
                    .When(x => x is TSubtype && condition((TSubtype) x))
                    .Then(x => consequent((TSubtype) x));
            }
        }

        private class CaseDefaultSubtypeBuilder<TKey, TSubtype> : ICaseDefaultSubtypeWhen<TKey, TSubtype>
            where TSubtype : TKey
        {
            private readonly ICaseDefaultThen<TKey> builder;
            private readonly Func<TSubtype, bool> condition;

            public CaseDefaultSubtypeBuilder(ICaseDefaultThen<TKey> builder, Func<TSubtype, bool> condition)
            {
                this.builder = builder;
                this.condition = condition;
            }

            public ICaseDefaultThen<TKey> Then(Action<TSubtype> consequent)
            {
                return builder
                    .When(x => x is TSubtype && condition((TSubtype)x))
                    .Then(x => consequent((TSubtype)x));
            }
        }

        private class CaseSubtypeBuilder<TKey, TSubtype, TResult> : ICaseSubtypeWhen<TKey, TSubtype, TResult>
            where TSubtype : TKey
        {
            private readonly ICaseThen<TKey, TResult> builder;
            private readonly Func<TSubtype, bool> condition;

            public CaseSubtypeBuilder(ICaseThen<TKey, TResult> builder, Func<TSubtype, bool> condition)
            {
                this.builder = builder;
                this.condition = condition;
            }

            public ICaseThen<TKey, TResult> Then(Func<TSubtype, TResult> consequent)
            {
                return builder
                    .When(x => x is TSubtype && condition((TSubtype)x))
                    .Then(x => consequent((TSubtype)x));
            }
        }

        private class CaseDefaultSubtypeBuilder<TKey, TSubtype, TResult> : ICaseDefaultSubtypeWhen<TKey, TSubtype, TResult>
            where TSubtype : TKey
        {
            private readonly ICaseDefaultThen<TKey, TResult> builder;
            private readonly Func<TSubtype, bool> condition;

            public CaseDefaultSubtypeBuilder(ICaseDefaultThen<TKey, TResult> builder, Func<TSubtype, bool> condition)
            {
                this.builder = builder;
                this.condition = condition;
            }

            public ICaseDefaultThen<TKey, TResult> Then(Func<TSubtype, TResult> consequent)
            {
                return builder
                    .When(x => x is TSubtype && condition((TSubtype)x))
                    .Then(x => consequent((TSubtype)x));
            }
        }

        public static ICaseThen<TKey> Then<TKey>(
            this ICaseInitialWhen<TKey> builder)
        {
            return builder.Then(_ => { });
        }

        public static ICaseThen<TKey, TResult> Then<TKey, TResult>(
            this ICaseInitialWhen<TKey> builder,
            TResult consequent)
        {
            return builder.Then(_ => consequent);
        }

        public static ICaseThen<TKey> Then<TKey, TSubtype>(
            this ICaseSubtypeInitialWhen<TKey, TSubtype> builder) where TSubtype : TKey
        {
            return builder.Then(_ => { });
        }

        public static ICaseThen<TKey, TResult> Then<TKey, TSubtype, TResult>(
            this ICaseSubtypeInitialWhen<TKey, TSubtype> builder,
            TResult consequent) where TSubtype : TKey
        {
            return builder.Then(_ => consequent);
        }

        public static ICaseInitialWhen<TKey> When<TKey>(
            this ICaseInitialThen<TKey> builder,
            bool condition)
        {
            return builder.When(_ => condition);
        }

        public static ICaseInitialWhen<TKey> When<TKey>(this ICaseInitialThen<TKey> builder, TKey condition)
        {
            return builder.When(x => Equals(x, condition));
        }

        public static ICaseDefaultThen<TKey> Default<TKey>(
            this ICaseInitialThen<TKey> builder)
        {
            return builder.Default(_ => { });
        }

        public static ICaseDefaultThen<TKey, TResult> Default<TKey, TResult>(
            this ICaseInitialThen<TKey> builder,
            TResult alternative)
        {
            return builder.Default(_ => alternative);
        }

        public static ICaseThen<TKey> Then<TKey>(
            this ICaseWhen<TKey> builder)
        {
            return builder.Then(_ => { });
        }

        public static ICaseThen<TKey> Then<TKey, TSubtype>(
            this ICaseSubtypeWhen<TKey, TSubtype> builder) where TSubtype : TKey
        {
            return builder.Then(_ => { });
        }

        public static ICaseWhen<TKey> When<TKey>(
            this ICaseThen<TKey> builder,
            bool condition)
        {
            return builder.When(_ => condition);
        }

        public static ICaseWhen<TKey> When<TKey>(this ICaseThen<TKey> builder, TKey condition)
        {
            return builder.When(x => Equals(x, condition));
        }

        public static ICaseDefaultThen<TKey> Default<TKey>(
            this ICaseThen<TKey> builder)
        {
            return builder.Default(_ => { });
        }

        public static void Else<TKey>(
            this ICaseThen<TKey> builder,
            Action<TKey> alternative)
        {
            builder.Default(alternative).End();
        }

        public static void Else<TKey>(
            this ICaseThen<TKey> builder)
        {
            builder.End();
        }

        public static ICaseDefaultThen<TKey> Then<TKey>(
            this ICaseDefaultWhen<TKey> builder)
        {
            return builder.Then(_ => { });
        }

        public static ICaseDefaultWhen<TKey> When<TKey>(
            this ICaseDefaultThen<TKey> builder,
            bool condition)
        {
            return builder.When(_ => condition);
        }

        public static ICaseDefaultWhen<TKey> When<TKey>(this ICaseDefaultThen<TKey> builder, TKey condition)
        {
            return builder.When(x => Equals(x, condition));
        }

        public static ICaseThen<TKey, TResult> Then<TKey, TResult>(
            this ICaseWhen<TKey, TResult> builder,
            TResult consequent)
        {
            return builder.Then(_ => consequent);
        }

        public static ICaseThen<TKey, TResult> Then<TKey, TSubtype, TResult>(
            this ICaseSubtypeWhen<TKey, TSubtype, TResult> builder,
            TResult consequent) where TSubtype : TKey
        {
            return builder.Then(_ => consequent);
        }

        public static ICaseWhen<TKey, TResult> When<TKey, TResult>(
            this ICaseThen<TKey, TResult> builder,
            bool condition)
        {
            return builder.When(_ => condition);
        }

        public static ICaseWhen<TKey, TResult> When<TKey, TResult>(this ICaseThen<TKey, TResult> builder, TKey condition)
        {
            return builder.When(x => Equals(x, condition));
        }

        public static ICaseDefaultThen<TKey, TResult> Default<TKey, TResult>(
            this ICaseThen<TKey, TResult> builder,
            TResult alternative)
        {
            return builder.Default(_ => alternative);
        }

        public static TResult Else<TKey, TResult>(
            this ICaseThen<TKey, TResult> builder,
            TResult alternative)
        {
            return builder.End() | alternative;
        }

        public static ICaseDefaultThen<TKey, TResult> Then<TKey, TResult>(
            this ICaseDefaultWhen<TKey, TResult> builder,
            TResult consequent)
        {
            return builder.Then(_ => consequent);
        }

        public static ICaseDefaultWhen<TKey, TResult> When<TKey, TResult>(
            this ICaseDefaultThen<TKey, TResult> builder,
            bool condition)
        {
            return builder.When(_ => condition);
        }

        public static ICaseDefaultWhen<TKey, TResult> When<TKey, TResult>(this ICaseDefaultThen<TKey, TResult> builder, TKey condition)
        {
            return builder.When(x => Equals(x, condition));
        }
    }
}
