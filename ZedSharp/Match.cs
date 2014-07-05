using System;
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
            return new Matcher<A,B>(Key);
        }

        /// <summary>
        /// Specifies a condition for the following consequent.
        /// Condition will only be evaluated if no previous Case condition was evaluated to true.
        /// After this, call Then.
        /// </summary>
        public MatcherConsequent<A, A> Case(Func<A, bool> f)
        {
            return new MatcherConsequent<A, A>(Key, f);
        }

        /// <summary>
        /// Specifies a condition for the following consequent.
        /// Condition will only be evaluated if no previous Case condition was evaluated to true.
        /// After this, call Then.
        /// </summary>
        public MatcherConsequent<A, C> Case<C>()
        {
            return new MatcherConsequent<A, C>(Key, x => x is C);
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

        private A Key { get; set; }
        private Ref<Matcher<A, B>> Previous { get; set; }
        private Func<A, bool> Predicate { get; set; }
        private Func<A, B> Selector { get; set; }

        /// <summary>
        /// Specifies a condition for the following consequent.
        /// Condition will only be evaluated if no previous Case condition was evaluated to true.
        /// After this, call Then.
        /// </summary>
        public MatcherConsequent<A, B, A> Case(Func<A, bool> f)
        {
            return new MatcherConsequent<A, B, A>(Key, this, f);
        }

        /// <summary>
        /// Specifies a condition for the following consequent.
        /// Condition will only be evaluated if no previous Case condition was evaluated to true.
        /// After this, call Then.
        /// </summary>
        public MatcherConsequent<A, B, C> Case<C>()
        {
            return new MatcherConsequent<A, B, C>(Key, this, x => x is C);
        }

        /// <summary>Gets the result of the Match as an Unsure. Won't have a value if no cases were matched.</summary>
        public Unsure<B> End()
        {
            var me = this;
            var previousResult = Previous.ToUnsure().SelectMany(x => x.End());
            Func<Unsure<B>> thisResult = () => Unsure.If(me.Key, me.Predicate, me.Selector);
            return previousResult.OrEval(thisResult);
        }

        public static implicit operator Unsure<B>(Matcher<A, B> matcher)
        {
            return matcher.End();
        }
    }

    public struct MatcherConsequent<A, C>
    {
        internal MatcherConsequent(A key, Func<A, bool> predicate) : this()
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
            return new Matcher<A, B>(Key, Predicate, x => f((C) (Object) x));
        }
    }

    public struct MatcherConsequent<A, B, C>
    {
        internal MatcherConsequent(A key, Matcher<A, B> previous, Func<A, bool> predicate) : this()
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

    public static class MatcherInitialExts
    {
        /// <summary>
        /// Specifies a condition for the following consequent.
        /// Condition will only be evaluated if no previous Case condition was evaluated to true.
        /// After this, call Then.
        /// </summary>
        public static MatcherConsequent<A, A> Case<A>(this MatcherInitial<A> matcher, A val)
        {
            return matcher.Case(key => Object.Equals(key, val));
        }

        /// <summary>
        /// Specifies a condition for the following consequent.
        /// Condition will only be evaluated if no previous Case condition was evaluated to true.
        /// After this, call Then.
        /// </summary>
        public static MatcherConsequent<A, A> Case<A>(this MatcherInitial<A> matcher, bool cond)
        {
            return matcher.Case(_ => cond);
        }

        /// <summary>
        /// Specifies a condition for the following consequent.
        /// Condition will only be evaluated if no previous Case condition was evaluated to true.
        /// After this, call Then.
        /// </summary>
        public static MatcherConsequent<A, A> Case<A>(this MatcherInitial<A> matcher, Func<bool> f)
        {
            return matcher.Case(_ => f());
        }

        /// <summary>
        /// Specifies a condition for the following consequent.
        /// Condition will only be evaluated if no previous Case condition was evaluated to true.
        /// After this, call Then.
        /// </summary>
        public static MatcherConsequent<A, A> Case<A>(this MatcherInitial<A> matcher, Type type)
        {
            return matcher.Case(key => type.IsInstanceOfType(key));
        }

        /// <summary>
        /// Specifies a condition for the following consequent.
        /// Condition will only be evaluated if no previous Case condition was evaluated to true.
        /// After this, call Then.
        /// </summary>
        public static MatcherConsequent<String, String> Case(this MatcherInitial<String> matcher, Regex regex)
        {
            return matcher.Case(key => regex.IsMatch(key));
        }

        /// <summary>
        /// Specifies the following consequent is to be evaluated if no previous Case condition evaluated to true.
        /// After this, call Then.
        /// </summary>
        public static MatcherConsequent<A, A> Else<A>(this MatcherInitial<A> matcher)
        {
            return matcher.Case(_ => true);
        }
    }

    public static class MatcherBranch2Exts
    {
        /// <summary>
        /// Specifies a condition for the following consequent.
        /// Condition will only be evaluated if no previous Case condition was evaluated to true.
        /// After this, call Then.
        /// </summary>
        public static MatcherConsequent<A, B, A> Case<A, B>(this Matcher<A, B> matcher, A val)
        {
            return matcher.Case(key => Object.Equals(key, val));
        }

        /// <summary>
        /// Specifies a condition for the following consequent.
        /// Condition will only be evaluated if no previous Case condition was evaluated to true.
        /// After this, call Then.
        /// </summary>
        public static MatcherConsequent<A, B, A> Case<A, B>(this Matcher<A, B> matcher, bool cond)
        {
            return matcher.Case(_ => cond);
        }

        /// <summary>
        /// Specifies a condition for the following consequent.
        /// Condition will only be evaluated if no previous Case condition was evaluated to true.
        /// After this, call Then.
        /// </summary>
        public static MatcherConsequent<A, B, A> Case<A, B>(this Matcher<A, B> matcher, Func<bool> f)
        {
            return matcher.Case(_ => f());
        }

        /// <summary>
        /// Specifies a condition for the following consequent.
        /// Condition will only be evaluated if no previous Case condition was evaluated to true.
        /// After this, call Then.
        /// </summary>
        public static MatcherConsequent<A, B, A> Case<A, B>(this Matcher<A, B> matcher, Type type)
        {
            return matcher.Case(key => type.IsInstanceOfType(key));
        }

        /// <summary>
        /// Specifies a condition for the following consequent.
        /// Condition will only be evaluated if no previous Case condition was evaluated to true.
        /// After this, call Then.
        /// </summary>
        public static MatcherConsequent<String, B, String> Case<B>(this Matcher<String, B> matcher, Regex regex)
        {
            return matcher.Case(key => regex.IsMatch(key));
        }

        /// <summary>
        /// Specifies the following consequent is to be evaluated if no previous Case condition evaluated to true.
        /// After this, call Then.
        /// </summary>
        public static MatcherConsequent<A, B, A> Else<A, B>(this Matcher<A, B> matcher)
        {
            return matcher.Case(_ => true);
        }
    }

    public static class MatcherConsequent2Exts
    {
        /// <summary>
        /// Specifies consequence if previous Case was the first true Case in this Match.
        /// Also infers return type of Match.
        /// After this, call Case or End.
        /// </summary>
        public static Matcher<A, B> Then<A, B, C>(this MatcherConsequent<A, C> matcher, B val)
        {
            return matcher.Then(_ => val);
        }

        /// <summary>
        /// Specifies consequence if previous Case was the first true Case in this Match.
        /// Also infers return type of Match.
        /// After this, call Case or End.
        /// </summary>
        public static Matcher<A, B> Then<A, B, C>(this MatcherConsequent<A, C> matcher, Func<B> f)
        {
            return matcher.Then(_ => f());
        }
    }

    public static class MatcherConsequent3Exts
    {
        /// <summary>
        /// Specifies consequence if previous Case was the first true Case in this Match.
        /// After this, call Case or End.
        /// </summary>
        public static Matcher<A, B> Then<A, B, C>(this MatcherConsequent<A, B, C> matcher, B val)
        {
            return matcher.Then(_ => val);
        }

        /// <summary>
        /// Specifies consequence if previous Case was the first true Case in this Match.
        /// After this, call Case or End.
        /// </summary>
        public static Matcher<A, B> Then<A, B, C>(this MatcherConsequent<A, B, C> matcher, Func<B> f)
        {
            return matcher.Then(_ => f());
        }
    }
}
