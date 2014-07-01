using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ZedSharp
{
    public static class Match
    {
        public static MatcherInitial<A> On<A>(A key)
        {
            return new MatcherInitial<A>(key);
        }
    }

    internal enum State { Uncomplete, Complete, Run }

    public struct MatcherInitial<A>
    {
        internal MatcherInitial(A key) : this()
        {
            Key = key;
        }

        internal A Key { get; private set; }

        public MatcherBranch<A, B> Return<B>()
        {
            return new MatcherBranch<A,B>(Key, default(B), State.Uncomplete);
        }

        public MatcherConsequent<A, A> Case(Func<A, bool> f)
        {
            return new MatcherConsequent<A, A>(Key, f(Key) ? State.Complete : State.Uncomplete);
        }

        public MatcherConsequent<A, C> Case<C>()
        {
            return new MatcherConsequent<A, C>(Key, Key is C ? State.Complete : State.Uncomplete);
        }
    }

    public struct MatcherBranch<A, B>
    {
        internal MatcherBranch(A key, B result, State state) : this()
        {
            Key = key;
            Result = result;
            State = state;
        }

        internal A Key { get; private set; }
        internal B Result { get; private set; }
        internal State State { get; private set; }

        public MatcherConsequent<A, B, A> Case(Func<A, bool> f)
        {
            return new MatcherConsequent<A, B, A>(Key, Result, State == State.Uncomplete && f(Key) ? State.Run : State);
        }

        public MatcherConsequent<A, B, C> Case<C>()
        {
            return new MatcherConsequent<A, B, C>(Key, Result, State == State.Uncomplete && Key is C ? State.Run : State);
        }

        public Unsure<B> End()
        {
            return State == State.Complete ? Unsure.Of(Result) : Unsure.None<B>();
        }
    }

    public struct MatcherConsequent<A, C>
    {
        internal MatcherConsequent(A key, State state) : this()
        {
            Key = key;
            State = state;
        }
        
        internal A Key { get; private set; }
        internal State State { get; private set; }
        
        public MatcherBranch<A, B> Then<B>(Func<C, B> f)
        {
            return State == State.Run
                ? new MatcherBranch<A, B>(Key, f((C) (Object) Key), State.Complete)
                : new MatcherBranch<A, B>(Key, default(B), State);
        }
    }

    public struct MatcherConsequent<A, B, C>
    {
        internal MatcherConsequent(A key, B result, State state) : this()
        {
            Key = key;
            Result = result;
            State = state;
        }

        internal A Key { get; private set; }
        internal B Result { get; private set; }
        internal State State { get; private set; }

        public MatcherBranch<A, B> Then(Func<C, B> f)
        {
            return State == State.Run
                ? new MatcherBranch<A, B>(Key, f((C)(Object)Key), State.Complete)
                : new MatcherBranch<A, B>(Key, Result, State);
        }
    }

    public static class MatcherInitialExts
    {
        public static MatcherConsequent<A, A> Case<A>(this MatcherInitial<A> matcher, A val)
        {
            return matcher.Case(key => Object.Equals(key, val));
        }

        public static MatcherConsequent<A, A> Case<A>(this MatcherInitial<A> matcher, bool cond)
        {
            return matcher.Case(_ => cond);
        }

        public static MatcherConsequent<A, A> Case<A>(this MatcherInitial<A> matcher, Func<bool> f)
        {
            return matcher.Case(_ => f());
        }

        public static MatcherConsequent<A, A> Case<A>(this MatcherInitial<A> matcher, Type type)
        {
            return matcher.Case(key => type.IsInstanceOfType(key));
        }

        public static MatcherConsequent<String, String> Case(this MatcherInitial<String> matcher, Regex regex)
        {
            return matcher.Case(key => regex.IsMatch(key));
        }

        public static MatcherConsequent<A, A> Else<A>(this MatcherInitial<A> matcher)
        {
            return matcher.Case(_ => true);
        }
    }

    public static class MatcherBranch2Exts
    {
        public static MatcherConsequent<A, B, A> Case<A, B>(this MatcherBranch<A, B> matcher, A val)
        {
            return matcher.Case(key => Object.Equals(key, val));
        }

        public static MatcherConsequent<A, B, A> Case<A, B>(this MatcherBranch<A, B> matcher, bool cond)
        {
            return matcher.Case(_ => cond);
        }

        public static MatcherConsequent<A, B, A> Case<A, B>(this MatcherBranch<A, B> matcher, Func<bool> f)
        {
            return matcher.Case(_ => f());
        }

        public static MatcherConsequent<A, B, A> Case<A, B>(this MatcherBranch<A, B> matcher, Type type)
        {
            return matcher.Case(key => type.IsInstanceOfType(key));
        }

        public static MatcherConsequent<String, B, String> Case<B>(this MatcherBranch<String, B> matcher, Regex regex)
        {
            return matcher.Case(key => regex.IsMatch(key));
        }

        public static MatcherConsequent<A, B, A> Else<A, B>(this MatcherBranch<A, B> matcher)
        {
            return matcher.Case(_ => true);
        }
    }

    public static class MatcherConsequent3Exts
    {
        public static MatcherBranch<A, B> Then<A, B, C>(this MatcherConsequent<A, B, C> matcher, B val)
        {
            return matcher.Then(_ => val);
        }

        public static MatcherBranch<A, B> Then<A, B, C>(this MatcherConsequent<A, B, C> matcher, Func<B> f)
        {
            return matcher.Then(_ => f());
        }
    }

    public static class MatcherConsequent2Exts
    {
        public static MatcherBranch<A, B> Then<A, B, C>(this MatcherConsequent<A, C> matcher, B val)
        {
            return matcher.Then(_ => val);
        }

        public static MatcherBranch<A, B> Then<A, B, C>(this MatcherConsequent<A, C> matcher, Func<B> f)
        {
            return matcher.Then(_ => f());
        }
    }
}
