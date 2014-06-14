using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZedSharp
{
    public static class Match
    {
        public static Matcher<A> On<A>(A key)
        {
            return new Matcher<A>(key);
        }
    }

    internal enum State { Uncomplete, Complete, Run }

    public struct Matcher<A>
    {
        internal Matcher(A key) : this()
        {
            Key = key;
        }

        internal A Key { get; private set; }

        public Matcher<A, B> Return<B>()
        {
            return new Matcher<A,B>(Key, default(B), State.Uncomplete);
        }
    }

    public struct Matcher<A, B>
    {
        internal Matcher(A key, B result, State state) : this()
        {
            Key = key;
            Result = result;
            State = state;
        }
        
        internal A Key { get; private set; }
        internal B Result { get; private set; }
        internal State State { get; private set; }

        public Matcher<A, B, A> Case(Func<A, bool> f)
        {
            return new Matcher<A, B, A>(Key, Result, State == State.Uncomplete && f(Key) ? State.Run : State);
        }

        public Matcher<A, B, C> Case<C>()
        {
            return new Matcher<A, B, C>(Key, Result, State == State.Uncomplete && Key is C ? State.Run : State);
        }

        public Unsure<B> End()
        {
            return State == State.Complete ? Unsure.Of(Result) : Unsure.None<B>();
        }
    }

    public struct Matcher<A, B, C>
    {
        internal Matcher(A key, B result, State state) : this()
        {
            Key = key;
            Result = result;
            State = state;
        }

        internal A Key { get; private set; }
        internal B Result { get; private set; }
        internal State State { get; private set; }

        public Matcher<A, B> Then(Func<C, B> f)
        {
            return State == State.Run
                ? new Matcher<A, B>(Key, f((C) (Object) Key), State.Complete)
                : new Matcher<A, B>(Key, Result, State);
        }
    }

    public static class MatcherExtensions
    {
        public static Matcher<A, B, A> Case<A, B>(this Matcher<A, B> matcher, A val)
        {
            return matcher.Case(key => Object.Equals(key, val));
        }

        public static Matcher<A, B, A> Case<A, B>(this Matcher<A, B> matcher, bool cond)
        {
            return matcher.Case(_ => cond);
        }

        public static Matcher<A, B, A> Case<A, B>(this Matcher<A, B> matcher, Func<bool> f)
        {
            return matcher.Case(_ => f());
        }

        public static Matcher<A, B, A> Case<A, B>(this Matcher<A, B> matcher, Type type)
        {
            return matcher.Case(key => type.IsInstanceOfType(key));
        }

        public static Matcher<A, B> Then<A, B, C>(this Matcher<A, B, C> matcher, B val)
        {
            return matcher.Then(_ => val);
        }

        public static Matcher<A, B> Then<A, B, C>(this Matcher<A, B, C> matcher, Func<B> f)
        {
            return matcher.Then(_ => f());
        }
    }
}
