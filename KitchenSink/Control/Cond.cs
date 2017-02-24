using System;
using System.Collections.Generic;

namespace KitchenSink.Control
{
    /// <summary>
    /// A Cond with an incomplete clause and yet unknown return type.
    /// </summary>
    public interface ICondIfInitial
    {
        /// <summary>
        /// Provides a consequent for previous condition.
        /// Return type for Cond is inferred from argument.
        /// </summary>
        ICondThen<TResult> Then<TResult>(Func<TResult> consequent);

        /// <summary>
        /// Provides a consequent for previous condition.
        /// </summary>
        ICondThen Then(Action consequent);
    }

    public interface ICondIf
    {
        /// <summary>
        /// Provides a consequent for previous condition.
        /// </summary>
        ICondThen Then(Action consequent);
    }

    public interface ICondThen
    {
        /// <summary>
        /// Starts a new clause with given condition.
        /// </summary>
        ICondIf If(Func<bool> condition);

        /// <summary>
        /// Evaluates Cond, invoking the given alternative if
        /// no conditions were true.
        /// </summary>
        void Else(Action alternative);
    }

    /// <summary>
    /// A Cond with an imcomplete clause.
    /// </summary>
    public interface ICondIf<TResult>
    {
        /// <summary>
        /// Provides a consequent for previous condition.
        /// </summary>
        ICondThen<TResult> Then(Func<TResult> consequent);
    }

    /// <summary>
    /// A Cond with a list of complete clauses.
    /// </summary>
    public interface ICondThen<TResult>
    {
        /// <summary>
        /// Starts a new clause with given condition.
        /// </summary>
        ICondIf<TResult> If(Func<bool> condition);

        /// <summary>
        /// Evaluates Cond, invoking the given alternative if
        /// no conditions were true.
        /// </summary>
        TResult Else(Func<TResult> alternative);
    }

    /// <summary>
    /// Builds a conditional control structure.
    /// </summary>
    public static class Cond<TResult>
    {
        /// <summary>
        /// Starts a new Cond with a new clause with given condition.
        /// </summary>
        public static ICondIf<TResult> If(Func<bool> condition)
        {
            return Cond.If<TResult>(condition);
        }

        /// <summary>
        /// Starts a new Cond with a new clause with given condition.
        /// </summary>
        public static ICondIf<TResult> If(bool condition)
        {
            return Cond.If<TResult>(() => condition);
        }
    }

    /// <summary>
    /// Builds a conditional control structure.
    /// </summary>
    public static class Cond
    {
        /// <summary>
        /// Starts a new Cond with a new clause with given condition
        /// and yet unknown return type.
        /// </summary>
        public static ICondIfInitial If(Func<bool> condition)
        {
            return new CondBuilderInitial(condition);
        }

        /// <summary>
        /// Starts a new Cond with a new clause with given condition
        /// and yet unknown return type.
        /// </summary>
        public static ICondIfInitial If(bool condition)
        {
            return new CondBuilderInitial(() => condition);
        }

        /// <summary>
        /// Starts a new Cond with a new clause with given condition
        /// and explicit return type.
        /// </summary>
        public static ICondIf<TResult> If<TResult>(Func<bool> condition)
        {
            return new CondBuilder<TResult>().If(condition);
        }

        /// <summary>
        /// Starts a new Cond with a new clause with given condition
        /// and explicit return type.
        /// </summary>
        public static ICondIf<TResult> If<TResult>(bool condition)
        {
            return new CondBuilder<TResult>().If(() => condition);
        }

        private class CondBuilderInitial : ICondIfInitial
        {
            private readonly Func<bool> pending;

            public CondBuilderInitial(Func<bool> initial)
            {
                pending = initial;
            }

            public ICondThen<TResult> Then<TResult>(Func<TResult> consequent)
            {
                return new CondBuilder<TResult>().If(pending).Then(consequent);
            }

            public ICondThen Then(Action consequent)
            {
                return new CondBuilder().If(pending).Then(consequent);
            }
        }

        private class CondBuilder : ICondIf, ICondThen
        {
            private class Clause
            {
                public Func<bool> Condition { get; set; }
                public Action Consequent { get; set; }
            }

            private Func<bool> pending;
            private readonly List<Clause> clauses = new List<Clause>();

            // ReSharper disable once MemberHidesStaticFromOuterClass
            public ICondIf If(Func<bool> condition)
            {
                pending = condition;
                return this;
            }

            public ICondThen Then(Action consequent)
            {
                clauses.Add(new Clause
                {
                    Condition = pending,
                    Consequent = consequent
                });
                return this;
            }

            public void Else(Action alternative)
            {
                foreach (var clause in clauses)
                {
                    if (clause.Condition())
                    {
                        clause.Consequent();
                        return;
                    }
                }

                alternative();
            }
        }

        private class CondBuilder<TResult> : ICondIf<TResult>, ICondThen<TResult>
        {
            private class Clause
            {
                public Func<bool> Condition { get; set; }
                public Func<TResult> Consequent { get; set; }
            }

            private Func<bool> pending;
            private readonly List<Clause> clauses = new List<Clause>();

            // ReSharper disable once MemberHidesStaticFromOuterClass
            public ICondIf<TResult> If(Func<bool> condition)
            {
                pending = condition;
                return this;
            }

            public ICondThen<TResult> Then(Func<TResult> consequent)
            {
                clauses.Add(new Clause
                {
                    Condition = pending,
                    Consequent = consequent
                });
                return this;
            }

            public TResult Else(Func<TResult> alternative)
            {
                foreach (var clause in clauses)
                {
                    if (clause.Condition())
                    {
                        return clause.Consequent();
                    }
                }

                return alternative();
            }
        }

        /// <summary>
        /// Provides a result for previous condition.
        /// </summary>
        public static ICondThen<TResult> Then<TResult>(this ICondIfInitial builder, TResult result)
        {
            return builder.Then(() => result);
        }

        /// <summary>
        /// Does nothing for previous condition.
        /// </summary>
        public static ICondThen Then(this ICondIfInitial builder)
        {
            return builder.Then(() => { });
        }

        /// <summary>
        /// Does nothing for previous condition.
        /// </summary>
        public static ICondThen Then(this ICondIf builder)
        {
            return builder.Then(() => { });
        }

        /// <summary>
        /// Starts a new clause with given condition.
        /// </summary>
        public static ICondIf If(this ICondThen builder, bool condition)
        {
            return builder.If(() => condition);
        }

        /// <summary>
        /// Evaluates Cond, doing nothing if no conditions were true.
        /// </summary>
        public static void Else(this ICondThen builder)
        {
            builder.Else(() => { });
        }

        /// <summary>
        /// Provides a result for previous condition.
        /// </summary>
        public static ICondThen<TResult> Then<TResult>(this ICondIf<TResult> builder, TResult result)
        {
            return builder.Then(() => result);
        }

        /// <summary>
        /// Starts a new clause with given condition.
        /// </summary>
        public static ICondIf<TResult> If<TResult>(this ICondThen<TResult> builder, bool condition)
        {
            return builder.If(() => condition);
        }

        /// <summary>
        /// Provides a result for previous condition.
        /// </summary>
        public static TResult Else<TResult>(this ICondThen<TResult> builder, TResult result)
        {
            return builder.Else(() => result);
        }
    }
}
