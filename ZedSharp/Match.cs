using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ZedSharp
{
    public static class Match
    {
        /// <summary>
        /// The starting point for the Matcher construct.
        /// After this, call either Return or Case.
        /// </summary>
        public static MatcherInitial<A> On<A>(A key)
        {
            return new MatcherInitial<A>(key);
        }
    }

    public struct MatcherInitial<A>
    {
        internal MatcherInitial(A key) : this()
        {
            Key = key;
        }

        private A Key { get; set; }

        /// <summary>
        /// Specifies return type for Match.
        /// After this, call Case or End.
        /// </summary>
        public Matcher<A, B> Return<B>()
        {
            return new Matcher<A, B>(Key);
        }

        /// <summary>
        /// Specifies a default return value generator.
        /// Else cannot be called at any point after a Default.
        /// After this, call Case or End.
        /// </summary>
        public MatcherDefault<A, B> Default<B>(Func<A, B> f)
        {
            return new MatcherDefault<A, B>(Key, f);
        }

        /// <summary>
        /// Specifies a condition for the following consequent.
        /// Condition will only be evaluated if no previous Case condition was evaluated to true.
        /// After this, call Then.
        /// </summary>
        public MatcherInferencePredicate<A, A> Case(Func<A, bool> f)
        {
            return new MatcherInferencePredicate<A, A>(Key, f);
        }

        /// <summary>
        /// Specifies a condition for the following consequent.
        /// Condition will only be evaluated if no previous Case condition was evaluated to true.
        /// After this, call Then.
        /// </summary>
        public MatcherInferencePredicate<A, C> Case<C>(Func<C, bool> f)
        {
            return new MatcherInferencePredicate<A, C>(Key, x => (x is C) && f(x.As<C>()));
        }

        /// <summary>
        /// Specifies a condition for the following consequent.
        /// Condition will only be evaluated if no previous Case condition was evaluated to true.
        /// After this, call Then.
        /// </summary>
        public MatcherInferencePredicate<A, C> Case<C>()
        {
            return new MatcherInferencePredicate<A, C>(Key, x => x is C);
        }
    }

    public struct Matcher<A, B>
    {
        internal Matcher(A key) : this()
        {
            Key = key;
            Previous = null;
            Predicate = null;
            Selector = null;
        }

        internal Matcher(A key, Func<A, bool> predicate, Func<A, B> selector) : this()
        {
            Key = key;
            Previous = null;
            Predicate = predicate;
            Selector = selector;
        }

        internal Matcher(A key, Matcher<A, B> previous, Func<A, bool> predicate, Func<A, B> selector) : this()
        {
            Key = key;
            Previous = Ref.Of(previous);
            Predicate = predicate;
            Selector = selector;
        }

        internal A Key { get; private set; }
        private Ref<Matcher<A, B>> Previous { get; set; }
        private Func<A, bool> Predicate { get; set; }
        private Func<A, B> Selector { get; set; }

        /// <summary>
        /// Specifies a condition for the following consequent.
        /// Condition will only be evaluated if no previous Case condition was evaluated to true.
        /// After this, call Then.
        /// </summary>
        public MatcherPredicate<A, B, A> Case(Func<A, bool> f)
        {
            return new MatcherPredicate<A, B, A>(Key, this, f);
        }

        /// <summary>
        /// Specifies a condition for the following consequent.
        /// Condition will only be evaluated if no previous Case condition was evaluated to true.
        /// After this, call Then.
        /// </summary>
        public MatcherPredicate<A, B, C> Case<C>(Func<C, bool> f)
        {
            return new MatcherPredicate<A, B, C>(Key, this, x => (x is C) && f(x.As<C>()));
        }

        /// <summary>
        /// Specifies a condition for the following consequent.
        /// Condition will only be evaluated if no previous Case condition was evaluated to true.
        /// After this, call Then.
        /// </summary>
        public MatcherPredicate<A, B, C> Case<C>()
        {
            return new MatcherPredicate<A, B, C>(Key, this, x => x is C);
        }

        /// <summary>Gets the result of the Match as an Maybe. Won't have a value if no cases were matched.</summary>
        public Maybe<B> End()
        {
            var me = this;
            var previousResult = Previous.ToMaybe().SelectMany(x => x.End());
            Func<A, bool> pred = x => (me.Predicate != null) && (me.Selector != null) && me.Predicate(me.Key);
            Func<Maybe<B>> thisResult = () => Maybe.If(me.Key, pred, me.Selector);
            return previousResult.OrEval(thisResult);
        }

        public static implicit operator Maybe<B>(Matcher<A, B> matcher)
        {
            return matcher.End();
        }
    }

    public struct MatcherInferencePredicate<A, C>
    {
        internal MatcherInferencePredicate(A key, Func<A, bool> predicate) : this()
        {
            Key = key;
            Predicate = predicate;
        }
        
        private A Key { get; set; }
        private Func<A, bool> Predicate { get; set; }

        /// <summary>
        /// Specifies consequence if previous Case was the first true Case in this Match.
        /// Also infers return type of Match.
        /// After this, call Case or End.
        /// </summary>
        public Matcher<A, B> Then<B>(Func<C, B> f)
        {
            return new Matcher<A, B>(Key, Predicate, x => f(x.As<C>()));
        }
    }

    public struct MatcherPredicate<A, B, C>
    {
        internal MatcherPredicate(A key, Matcher<A, B> previous, Func<A, bool> predicate) : this()
        {
            Key = key;
            Previous = previous;
            Predicate = predicate;
        }

        private A Key { get; set; }
        private Matcher<A, B> Previous { get; set; }
        private Func<A, bool> Predicate { get; set; }

        /// <summary>
        /// Specifies consequence if previous Case was the first true Case in this Match.
        /// After this, call Case or End.
        /// </summary>
        public Matcher<A, B> Then(Func<C, B> f)
        {
            return new Matcher<A, B>(Key, Previous, Predicate, x => f((C) (Object) x));
        }
    }

    public struct MatcherElsePredicate<A, B>
    {
        internal MatcherElsePredicate(Matcher<A, B> previous) : this()
        {
            Key = previous.Key;
            Previous = previous;
        }

        private A Key { get; set; }
        private Matcher<A, B> Previous { get; set; }

        /// <summary>
        /// After this, call End.
        /// </summary>
        public MatcherElse<A, B> Then(Func<A, B> f)
        {
            return new MatcherElse<A, B>(Key, Previous, x => f(x));
        }
    }

    public struct MatcherElse<A, B>
    {
        internal MatcherElse(A key, Matcher<A, B> previous, Func<A, B> selector) : this()
        {
            Key = key;
            Previous = previous;
            Selector = selector;
        }

        private A Key { get; set; }
        private Matcher<A, B> Previous { get; set; }
        private Func<A, B> Selector { get; set; }
        
        /// <summary>Gets the result of the Match as an Maybe.</summary>
        public B End()
        {
            var me = this;
            var previousResult = Previous.End();
            return previousResult.OrElseEval(() => me.Selector(me.Key));
        }

        public static implicit operator B(MatcherElse<A, B> matcher)
        {
            return matcher.End();
        }
    }

    public struct MatcherDefault<A, B>
    {
        internal MatcherDefault(A key, Func<A, B> selector) : this()
        {
            Key = key;
            Previous = null;
            Predicate = null;
            Selector = selector;
        }

        internal MatcherDefault(A key, MatcherDefault<A, B> previous, Func<A, bool> predicate, Func<A, B> selector) : this()
        {
            Key = key;
            Previous = Ref.Of(previous);
            Predicate = predicate;
            Selector = selector;
        }

        internal A Key { get; private set; }
        private Ref<MatcherDefault<A, B>> Previous { get; set; }
        private Func<A, bool> Predicate { get; set; }
        private Func<A, B> Selector { get; set; }

        /// <summary>
        /// Specifies a condition for the following consequent.
        /// Condition will only be evaluated if no previous Case condition was evaluated to true.
        /// After this, call Then.
        /// </summary>
        public MatcherDefaultPredicate<A, B, A> Case(Func<A, bool> f)
        {
            return new MatcherDefaultPredicate<A, B, A>(Key, this, f);
        }

        /// <summary>
        /// Specifies a condition for the following consequent.
        /// Condition will only be evaluated if no previous Case condition was evaluated to true.
        /// After this, call Then.
        /// </summary>
        public MatcherDefaultPredicate<A, B, C> Case<C>(Func<C, bool> f)
        {
            return new MatcherDefaultPredicate<A, B, C>(Key, this, x => (x is C) && f(x.As<C>()));
        }

        /// <summary>
        /// Specifies a condition for the following consequent.
        /// Condition will only be evaluated if no previous Case condition was evaluated to true.
        /// After this, call Then.
        /// </summary>
        public MatcherDefaultPredicate<A, B, C> Case<C>()
        {
            return new MatcherDefaultPredicate<A, B, C>(Key, this, x => x is C);
        }

        private B RunDefault(A key)
        {
            return Previous == null ? Selector(key) : Previous.Value.RunDefault(key);
        }

        private Maybe<B> EndNoDefault()
        {
            var me = this;
            var previousResult = Previous.ToMaybe().SelectMany(x => x.EndNoDefault());
            Func<A, bool> pred = x => (me.Predicate != null) && (me.Selector != null) && me.Predicate(me.Key);
            Func<Maybe<B>> thisResult = () => Maybe.If(me.Key, pred, me.Selector);
            return previousResult.OrEval(thisResult);
        }

        /// <summary>Gets the result of the Match. Will be default value if no cases were matched.</summary>
        public B End()
        {
            var me = this;
            return EndNoDefault().OrElseEval(() => me.RunDefault(me.Key));
        }

        public static implicit operator B(MatcherDefault<A, B> matcher)
        {
            return matcher.End();
        }
    }

    public struct MatcherDefaultPredicate<A, B, C>
    {
        internal MatcherDefaultPredicate(A key, MatcherDefault<A, B> previous, Func<A, bool> predicate) : this()
        {
            Key = key;
            Previous = previous;
            Predicate = predicate;
        }

        private A Key { get; set; }
        private MatcherDefault<A, B> Previous { get; set; }
        private Func<A, bool> Predicate { get; set; }

        /// <summary>
        /// Specifies consequence if previous Case was the first true Case in this Match.
        /// After this, call Case or End.
        /// </summary>
        public MatcherDefault<A, B> Then(Func<C, B> f)
        {
            return new MatcherDefault<A, B>(Key, Previous, Predicate, x => f(x.As<C>()));
        }
    }

    public static class MatcherExtensions
    {
        /// <summary>
        /// Specifies a condition for the following consequent.
        /// Condition will only be evaluated if no previous Case condition was evaluated to true.
        /// After this, call Then.
        /// </summary>
        public static MatcherPredicate<A, B, A> Case<A, B>(this Matcher<A, B> matcher, A val)
        {
            return matcher.Case(key => Object.Equals(key, val));
        }

        /// <summary>
        /// Specifies a condition for the following consequent.
        /// Condition will only be evaluated if no previous Case condition was evaluated to true.
        /// After this, call Then.
        /// </summary>
        public static MatcherPredicate<A, B, A> Case<A, B>(this Matcher<A, B> matcher, bool cond)
        {
            return matcher.Case(_ => cond);
        }

        /// <summary>
        /// Specifies a condition for the following consequent.
        /// Condition will only be evaluated if no previous Case condition was evaluated to true.
        /// After this, call Then.
        /// </summary>
        public static MatcherPredicate<A, B, A> Case<A, B>(this Matcher<A, B> matcher, Func<bool> f)
        {
            return matcher.Case(_ => f());
        }

        /// <summary>
        /// Specifies a condition for the following consequent.
        /// Condition will only be evaluated if no previous Case condition was evaluated to true.
        /// After this, call Then.
        /// </summary>
        public static MatcherPredicate<A, B, A> Case<A, B>(this Matcher<A, B> matcher, Type type)
        {
            return matcher.Case(key => type.IsInstanceOfType(key));
        }

        /// <summary>
        /// Specifies consequence if previous Case was the first true Case in this Match.
        /// After this, call Case or End.
        /// </summary>
        public static Matcher<A, B> Then<A, B, C>(this MatcherPredicate<A, B, C> matcher, B val)
        {
            return matcher.Then(_ => val);
        }

        /// <summary>
        /// Specifies consequence if previous Case was the first true Case in this Match.
        /// After this, call Case or End.
        /// </summary>
        public static Matcher<A, B> Then<A, B, C>(this MatcherPredicate<A, B, C> matcher, Func<B> f)
        {
            return matcher.Then(_ => f());
        }
    }

    public static class MatcherInferenceExtensions
    {
        /// <summary>
        /// Specifies a condition for the following consequent.
        /// Condition will only be evaluated if no previous Case condition was evaluated to true.
        /// After this, call Then.
        /// </summary>
        public static MatcherInferencePredicate<A, A> Case<A>(this MatcherInitial<A> matcher, A val)
        {
            return matcher.Case(key => Object.Equals(key, val));
        }

        /// <summary>
        /// Specifies a condition for the following consequent.
        /// Condition will only be evaluated if no previous Case condition was evaluated to true.
        /// After this, call Then.
        /// </summary>
        public static MatcherInferencePredicate<A, A> Case<A>(this MatcherInitial<A> matcher, bool cond)
        {
            return matcher.Case(_ => cond);
        }

        /// <summary>
        /// Specifies a condition for the following consequent.
        /// Condition will only be evaluated if no previous Case condition was evaluated to true.
        /// After this, call Then.
        /// </summary>
        public static MatcherInferencePredicate<A, A> Case<A>(this MatcherInitial<A> matcher, Func<bool> f)
        {
            return matcher.Case(_ => f());
        }

        /// <summary>
        /// Specifies a condition for the following consequent.
        /// Condition will only be evaluated if no previous Case condition was evaluated to true.
        /// After this, call Then.
        /// </summary>
        public static MatcherInferencePredicate<A, A> Case<A>(this MatcherInitial<A> matcher, Type type)
        {
            return matcher.Case(key => type.IsInstanceOfType(key));
        }
        
        /// <summary>
        /// Specifies consequence if previous Case was the first true Case in this Match.
        /// Also infers return type of Match.
        /// After this, call Case or End.
        /// </summary>
        public static Matcher<A, B> Then<A, B, C>(this MatcherInferencePredicate<A, C> matcher, B val)
        {
            return matcher.Then(_ => val);
        }

        /// <summary>
        /// Specifies consequence if previous Case was the first true Case in this Match.
        /// Also infers return type of Match.
        /// After this, call Case or End.
        /// </summary>
        public static Matcher<A, B> Then<A, B, C>(this MatcherInferencePredicate<A, C> matcher, Func<B> f)
        {
            return matcher.Then(_ => f());
        }
    }

    public static class MatcherStringExtensions
    {
        /// <summary>
        /// Specifies a condition for the following consequent.
        /// Condition will only be evaluated if no previous Case condition was evaluated to true.
        /// After this, call Then.
        /// </summary>
        public static MatcherPredicate<String, B, String> Case<B>(this Matcher<String, B> matcher, Regex regex)
        {
            return matcher.Case(key => regex.IsMatch(key));
        }

        /// <summary>
        /// Specifies a condition for the following consequent.
        /// Condition will only be evaluated if no previous Case condition was evaluated to true.
        /// After this, call Then.
        /// </summary>
        public static MatcherInferencePredicate<String, String> Case(this MatcherInitial<String> matcher, Regex regex)
        {
            return matcher.Case(key => regex.IsMatch(key));
        }

        /// <summary>
        /// Does case-insensitive equality comparison with the key.
        /// </summary>
        public static MatcherPredicate<String, B, String> CaseInsenstive<B>(this Matcher<String, B> matcher, String s)
        {
            return matcher.Case(key => String.Equals(key, s, StringComparison.InvariantCultureIgnoreCase));
        }

        /// <summary>
        /// Does case-insensitive equality comparison with the key.
        /// </summary>
        public static MatcherInferencePredicate<String, String> CaseInsensitive(this MatcherInitial<String> matcher, String s)
        {
            return matcher.Case(key => String.Equals(key, s, StringComparison.InvariantCultureIgnoreCase));
        }
    }

    public static class MatcherElseExtensions
    {
        /// <summary>
        /// Specifies the following consequence should be run if no previous case was true.
        /// No more Case methods can be called after this.
        /// After this, call Then.
        /// </summary>
        public static MatcherElsePredicate<A, B> Else<A, B>(this Matcher<A, B> matcher)
        {
            return new MatcherElsePredicate<A, B>(matcher);
        }
        
        /// <summary>
        /// Specifies consequence if previous Case was the first true Case in this Match.
        /// Also infers return type of Match.
        /// After this, call End.
        /// </summary>
        public static MatcherElse<A, B> Then<A, B>(this MatcherElsePredicate<A, B> matcher, B val)
        {
            return matcher.Then(_ => val);
        }

        /// <summary>
        /// Specifies consequence if previous Case was the first true Case in this Match.
        /// Also infers return type of Match.
        /// After this, call End.
        /// </summary>
        public static MatcherElse<A, B> Then<A, B>(this MatcherElsePredicate<A, B> matcher, Func<B> f)
        {
            return matcher.Then(_ => f());
        }
    }

    public static class MatcherDefaultExtensions
    {
        /// <summary>
        /// Specifies a default return value generator.
        /// Else cannot be called at any point after a Default.
        /// After this, call Case or End.
        /// </summary>
        public static MatcherDefault<A, B> Default<A, B>(this MatcherInitial<A> matcher, Func<B> f)
        {
            return matcher.Default(_ => f());
        }

        /// <summary>
        /// Specifies a default return value.
        /// Else cannot be called at any point after a Default.
        /// After this, call Case or End.
        /// </summary>
        public static MatcherDefault<A, B> Default<A, B>(this MatcherInitial<A> matcher, B val)
        {
            return matcher.Default(_ => val);
        }

        /// <summary>
        /// Specifies a condition for the following consequent.
        /// Condition will only be evaluated if no previous Case condition was evaluated to true.
        /// After this, call Then.
        /// </summary>
        public static MatcherDefaultPredicate<A, B, A> Case<A, B>(this MatcherDefault<A, B> matcher, A val)
        {
            return matcher.Case(val.Eq());
        }

        /// <summary>
        /// Specifies a condition for the following consequent.
        /// Condition will only be evaluated if no previous Case condition was evaluated to true.
        /// After this, call Then.
        /// </summary>
        public static MatcherDefaultPredicate<A, B, A> Case<A, B>(this MatcherDefault<A, B> matcher, bool cond)
        {
            return matcher.Case(_ => cond);
        }

        /// <summary>
        /// Specifies a condition for the following consequent.
        /// Condition will only be evaluated if no previous Case condition was evaluated to true.
        /// After this, call Then.
        /// </summary>
        public static MatcherDefaultPredicate<A, B, A> Case<A, B>(this MatcherDefault<A, B> matcher, Func<bool> f)
        {
            return matcher.Case(_ => f());
        }

        /// <summary>
        /// Specifies a condition for the following consequent.
        /// Condition will only be evaluated if no previous Case condition was evaluated to true.
        /// After this, call Then.
        /// </summary>
        public static MatcherDefaultPredicate<A, B, A> Case<A, B>(this MatcherDefault<A, B> matcher, Type type)
        {
            return matcher.Case(key => type.IsInstanceOfType(key));
        }

        /// <summary>
        /// Specifies consequence if previous Case was the first true Case in this Match.
        /// After this, call End.
        /// </summary>
        public static MatcherDefault<A, B> Then<A, B, C>(this MatcherDefaultPredicate<A, B, C> matcher, B val)
        {
            return matcher.Then(_ => val);
        }

        /// <summary>
        /// Specifies consequence if previous Case was the first true Case in this Match.
        /// After this, call End.
        /// </summary>
        public static MatcherDefault<A, B> Then<A, B, C>(this MatcherDefaultPredicate<A, B, C> matcher, Func<B> f)
        {
            return matcher.Then(_ => f());
        }
    }
}
