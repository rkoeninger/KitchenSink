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

    internal enum MatcherState { Uncomplete, Complete, Run }

    public struct MatcherInitial<A>
    {
        internal MatcherInitial(A key) : this()
        {
            Key = key;
        }

        internal A Key { get; private set; }

        /// <summary>
        /// Specifies return type for Match.
        /// After this, call Case or End.
        /// </summary>
        public Matcher<A, B> Return<B>()
        {
            return new Matcher<A,B>(Key, default(B), MatcherState.Uncomplete);
        }

        /// <summary>
        /// Specifies a condition for the following consequent.
        /// Condition will only be evaluated if no previous Case condition was evaluated to true.
        /// After this, call Then.
        /// </summary>
        public MatcherConsequent<A, A> Case(Func<A, bool> f)
        {
            return new MatcherConsequent<A, A>(Key, f(Key) ? MatcherState.Complete : MatcherState.Uncomplete);
        }

        /// <summary>
        /// Specifies a condition for the following consequent.
        /// Condition will only be evaluated if no previous Case condition was evaluated to true.
        /// After this, call Then.
        /// </summary>
        public MatcherConsequent<A, C> Case<C>()
        {
            return new MatcherConsequent<A, C>(Key, Key is C ? MatcherState.Complete : MatcherState.Uncomplete);
        }
    }

    public struct Matcher<A, B>
    {
        internal Matcher(A key, B result, MatcherState state) : this()
        {
            Key = key;
            Result = result;
            State = state;
        }

        internal A Key { get; private set; }
        internal B Result { get; private set; }
        internal MatcherState State { get; private set; }

        public bool IsComplete { get { return State == MatcherState.Complete; } }

        /// <summary>
        /// Specifies a condition for the following consequent.
        /// Condition will only be evaluated if no previous Case condition was evaluated to true.
        /// After this, call Then.
        /// </summary>
        public MatcherConsequent<A, B, A> Case(Func<A, bool> f)
        {
            return new MatcherConsequent<A, B, A>(Key, Result, State == MatcherState.Uncomplete && f(Key) ? MatcherState.Run : State);
        }

        /// <summary>
        /// Specifies a condition for the following consequent.
        /// Condition will only be evaluated if no previous Case condition was evaluated to true.
        /// After this, call Then.
        /// </summary>
        public MatcherConsequent<A, B, C> Case<C>()
        {
            return new MatcherConsequent<A, B, C>(Key, Result, State == MatcherState.Uncomplete && Key is C ? MatcherState.Run : State);
        }

        /// <summary>Gets the result of the Match as an Unsure. Won't have a value if no cases were matched.</summary>
        public Unsure<B> End()
        {
            return State == MatcherState.Complete ? Unsure.Of(Result) : Unsure.None<B>();
        }

        public static implicit operator Unsure<B>(Matcher<A, B> matcher)
        {
            return matcher.End();
        }
    }

    public struct MatcherConsequent<A, C>
    {
        internal MatcherConsequent(A key, MatcherState state) : this()
        {
            Key = key;
            State = state;
        }
        
        internal A Key { get; private set; }
        internal MatcherState State { get; private set; }

        /// <summary>
        /// Specifies consequence if previous Case was the first true Case in this Match.
        /// Also infers return type of Match.
        /// After this, call Case or End.
        /// </summary>
        public Matcher<A, B> Then<B>(Func<C, B> f)
        {
            return State == MatcherState.Run
                ? new Matcher<A, B>(Key, f((C) (Object) Key), MatcherState.Complete)
                : new Matcher<A, B>(Key, default(B), State);
        }
    }

    public struct MatcherConsequent<A, B, C>
    {
        internal MatcherConsequent(A key, B result, MatcherState state) : this()
        {
            Key = key;
            Result = result;
            State = state;
        }

        internal A Key { get; private set; }
        internal B Result { get; private set; }
        internal MatcherState State { get; private set; }

        /// <summary>
        /// Specifies consequence if previous Case was the first true Case in this Match.
        /// After this, call Case or End.
        /// </summary>
        public Matcher<A, B> Then(Func<C, B> f)
        {
            return State == MatcherState.Run
                ? new Matcher<A, B>(Key, f((C)(Object)Key), MatcherState.Complete)
                : new Matcher<A, B>(Key, Result, State);
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
