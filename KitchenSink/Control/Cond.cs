using System;
using System.Collections.Generic;
using System.Linq;

namespace KitchenSink.Control
{
    /// <summary>
    /// A Cond with an incomplete clause and yet unknown return type.
    /// </summary>
    public interface ICondInitial
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

    /// <summary>
    /// A Cond with an imcomplete clause that returns void.
    /// </summary>
    public interface ICondIf
    {
        /// <summary>
        /// Provides a consequent for previous condition.
        /// </summary>
        ICondThen Then(Action consequent);
    }

    /// <summary>
    /// A Cond with a list of complete clauses that returns void.
    /// </summary>
    public interface ICondThen
    {
        /// <summary>
        /// Starts a new clause with given condition.
        /// </summary>
        ICondIf If(Func<bool> condition);
        
        /// <summary>
        /// Evaluates Cond, returning true if one of the
        /// consequents was evaluated, false if none were.
        /// </summary>
        bool End();

        /// <summary>
        /// Adds all the clauses in the given Cond to this one.
        /// </summary>
        ICondThen Absorb(ICondThen builder);
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
        /// Evaluates Cond, returning Some if one of the
        /// consequents was evaluated, None if none were.
        /// </summary>
        Maybe<TResult> End();

        /// <summary>
        /// Adds all the clauses in the given Cond to this one.
        /// </summary>
        ICondThen<TResult> Absorb(ICondThen<TResult> builder);
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
            return If(() => condition);
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
        public static ICondInitial If(Func<bool> condition)
        {
            return new CondBuilderInitial(condition);
        }

        /// <summary>
        /// Starts a new Cond with a new clause with given condition
        /// and yet unknown return type.
        /// </summary>
        public static ICondInitial If(bool condition)
        {
            return If(() => condition);
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
            return If<TResult>(() => condition);
        }

        private interface IClause
        {
            bool Eval();
        }

        private interface IClause<TResult>
        {
            Maybe<TResult> Eval();
        }

        private class ScalarClause : IClause
        {
            public Func<bool> Condition { private get; set; }
            public Action Consequent { private get; set; }

            public bool Eval()
            {
                if (Condition())
                {
                    Consequent();
                    return true;
                }

                return false;
            }
        }

        private class NestedClause : IClause
        {
            public ICondThen Builder { private get; set; }

            public bool Eval()
            {
                return Builder.End();
            }
        }

        private class ScalarClause<TResult> : IClause<TResult>
        {
            public Func<bool> Condition { private get;  set; }
            public Func<TResult> Consequent { private get; set; }

            public Maybe<TResult> Eval()
            {
                return Condition() ? Maybe.Some(Consequent()) : Maybe<TResult>.None;
            }
        }

        private class NestedClause<TResult> : IClause<TResult>
        {
            public ICondThen<TResult> Builder { private get; set; }

            public Maybe<TResult> Eval()
            {
                return Builder.End();
            }
        }

        private class CondBuilderInitial : ICondInitial
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
            private Func<bool> pending;
            private readonly List<IClause> clauses = new List<IClause>();

            // ReSharper disable once MemberHidesStaticFromOuterClass
            public ICondIf If(Func<bool> condition)
            {
                pending = condition;
                return this;
            }

            public ICondThen Then(Action consequent)
            {
                clauses.Add(new ScalarClause
                {
                    Condition = pending,
                    Consequent = consequent
                });
                return this;
            }

            public bool End()
            {
                return clauses.Any(x => x.Eval());
            }

            public ICondThen Absorb(ICondThen builder)
            {
                clauses.Add(new NestedClause { Builder = builder });
                return this;
            }
        }

        private class CondBuilder<TResult> : ICondIf<TResult>, ICondThen<TResult>
        {
            private Func<bool> pending;
            private readonly List<IClause<TResult>> clauses = new List<IClause<TResult>>();

            // ReSharper disable once MemberHidesStaticFromOuterClass
            public ICondIf<TResult> If(Func<bool> condition)
            {
                pending = condition;
                return this;
            }

            public ICondThen<TResult> Then(Func<TResult> consequent)
            {
                clauses.Add(new ScalarClause<TResult>
                {
                    Condition = pending,
                    Consequent = consequent
                });
                return this;
            }

            public Maybe<TResult> End()
            {
                return clauses.FirstSome(x => x.Eval());
            }

            public ICondThen<TResult> Absorb(ICondThen<TResult> builder)
            {
                clauses.Add(new NestedClause<TResult> { Builder = builder });
                return this;
            }
        }

        /// <summary>
        /// Provides a result for previous condition.
        /// </summary>
        public static ICondThen<TResult> Then<TResult>(this ICondInitial builder, TResult result)
        {
            return builder.Then(() => result);
        }

        /// <summary>
        /// Does nothing for previous condition.
        /// </summary>
        public static ICondThen Then(this ICondInitial builder)
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
        /// Evaluates Cond, invoking the given alternative if
        /// no conditions were true.
        /// </summary>
        public static void Else(this ICondThen builder, Action alternative)
        {
            if (!builder.End())
                alternative();
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
        /// Evaluates Cond, invoking the given alternative if
        /// no conditions were true.
        /// </summary>
        public static TResult Else<TResult>(this ICondThen<TResult> builder, Func<TResult> alternative)
        {
            return builder.End().OrElseEval(alternative);
        }

        /// <summary>
        /// Provides a result for previous condition.
        /// </summary>
        public static TResult Else<TResult>(this ICondThen<TResult> builder, TResult alternative)
        {
            return builder.Else(() => alternative);
        }
    }
}
