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

        public B OrElse(B defaultValue)
        {
            return State == State.Complete ? Result : defaultValue;
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
}
