using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZedSharp
{
    public static class Match
    {
        public static MatchInitial<A> On<A>(A key)
        {
            return new MatchInitial<A>(key);
        }
    }

    internal enum State { Uncomplete, Complete, Run }

    public struct MatchInitial<A>
    {
        internal MatchInitial(A key) : this()
        {
            Key = key;
        }

        internal A Key { get; private set; }

        public MatchTerminal<A, B> Return<B>()
        {
            return new MatchTerminal<A,B>(Key, default(B), State.Uncomplete);
        }

        public MatchConsequent<A, A> Case(Func<A, bool> f)
        {
            return new MatchConsequent<A, A>(Key, f(Key) ? State.Complete : State.Uncomplete);
        }

        public MatchConsequent<A, C> Case<C>()
        {
            return new MatchConsequent<A, C>(Key, Key is C ? State.Complete : State.Uncomplete);
        }
    }

    public struct MatchTerminal<A, B>
    {
        internal MatchTerminal(A key, B result, State state) : this()
        {
            Key = key;
            Result = result;
            State = state;
        }

        internal A Key { get; private set; }
        internal B Result { get; private set; }
        internal State State { get; private set; }

        public MatchConsequent<A, B, A> Case(Func<A, bool> f)
        {
            return new MatchConsequent<A, B, A>(Key, Result, State == State.Uncomplete && f(Key) ? State.Run : State);
        }

        public MatchConsequent<A, B, C> Case<C>()
        {
            return new MatchConsequent<A, B, C>(Key, Result, State == State.Uncomplete && Key is C ? State.Run : State);
        }

        public Unsure<B> End()
        {
            return State == State.Complete ? Unsure.Of(Result) : Unsure.None<B>();
        }
    }

    public struct MatchConsequent<A, C>
    {
        internal MatchConsequent(A key, State state) : this()
        {
            Key = key;
            State = state;
        }
        
        internal A Key { get; private set; }
        internal State State { get; private set; }
        
        public MatchTerminal<A, B> Then<B>(Func<C, B> f)
        {
            return State == State.Run
                ? new MatchTerminal<A, B>(Key, f((C) (Object) Key), State.Complete)
                : new MatchTerminal<A, B>(Key, default(B), State);
        }
    }

    public struct MatchConsequent<A, B, C>
    {
        internal MatchConsequent(A key, B result, State state) : this()
        {
            Key = key;
            Result = result;
            State = state;
        }

        internal A Key { get; private set; }
        internal B Result { get; private set; }
        internal State State { get; private set; }

        public MatchTerminal<A, B> Then(Func<C, B> f)
        {
            return State == State.Run
                ? new MatchTerminal<A, B>(Key, f((C)(Object)Key), State.Complete)
                : new MatchTerminal<A, B>(Key, Result, State);
        }
    }

    public static class MatchTerminal2Exts
    {
        public static MatchConsequent<A, B, A> Case<A, B>(this MatchTerminal<A, B> matcher, A val)
        {
            return matcher.Case(key => Object.Equals(key, val));
        }

        public static MatchConsequent<A, B, A> Case<A, B>(this MatchTerminal<A, B> matcher, bool cond)
        {
            return matcher.Case(_ => cond);
        }

        public static MatchConsequent<A, B, A> Case<A, B>(this MatchTerminal<A, B> matcher, Func<bool> f)
        {
            return matcher.Case(_ => f());
        }

        public static MatchConsequent<A, B, A> Case<A, B>(this MatchTerminal<A, B> matcher, Type type)
        {
            return matcher.Case(key => type.IsInstanceOfType(key));
        }
    }

    public static class MatchConsequent3Exts
    {
        public static MatchTerminal<A, B> Then<A, B, C>(this MatchConsequent<A, B, C> matcher, B val)
        {
            return matcher.Then(_ => val);
        }

        public static MatchTerminal<A, B> Then<A, B, C>(this MatchConsequent<A, B, C> matcher, Func<B> f)
        {
            return matcher.Then(_ => f());
        }
    }

    public static class MatchInitialExts
    {
        public static MatchConsequent<A, A> Case<A>(this MatchInitial<A> matcher, A val)
        {
            return matcher.Case(key => Object.Equals(key, val));
        }

        public static MatchConsequent<A, A> Case<A>(this MatchInitial<A> matcher, bool cond)
        {
            return matcher.Case(_ => cond);
        }

        public static MatchConsequent<A, A> Case<A>(this MatchInitial<A> matcher, Func<bool> f)
        {
            return matcher.Case(_ => f());
        }

        public static MatchConsequent<A, A> Case<A>(this MatchInitial<A> matcher, Type type)
        {
            return matcher.Case(key => type.IsInstanceOfType(key));
        }
    }

    public static class MatchConsequent2Exts
    {
        public static MatchTerminal<A, B> Then<A, B, C>(this MatchConsequent<A, C> matcher, B val)
        {
            return matcher.Then(_ => val);
        }

        public static MatchTerminal<A, B> Then<A, B, C>(this MatchConsequent<A, C> matcher, Func<B> f)
        {
            return matcher.Then(_ => f());
        }
    }
}
