using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ZedSharp
{
    internal interface IMatcher<A, B>
    {
        /// <remarks>Key may be ignored, depending on implementation.</remarks>
        Maybe<B> Eval(A key);
    }

    internal class NullMatcher<A, B> : IMatcher<A, B>
    {
        public static readonly IMatcher<A, B> It = new NullMatcher<A, B>();

        public Maybe<B> Eval(A _)
        {
            return Maybe.None<B>();
        }
    }

    public static class Match
    {
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
        
        internal readonly A Key;

        public Matcher<A, B> Return<B>()
        {
            return new Matcher<A, B>(Key);
        }

        public MatcherDefault<A, B> Default<B>(Func<A, B> f)
        {
            return new MatcherDefault<A, B>(Key, f);
        }

        public MatcherInferencePredicate<A, A> Case(Func<A, bool> f)
        {
            return new MatcherInferencePredicate<A, A>(Key, f);
        }

        public MatcherInferencePredicate<A, C> Case<C>(Func<C, bool> f) where C : A
        {
            return new MatcherInferencePredicate<A, C>(Key, x => x is C && f((C) x));
        }

        public MatcherInferencePredicate<A, C> Case<C>() where C : A
        {
            return new MatcherInferencePredicate<A, C>(Key, x => x is C);
        }
    }

    public struct Matcher<A, B> : IMatcher<A, B>
    {
        internal Matcher(A key) : this()
        {
            Key = key;
            Previous = NullMatcher<A, B>.It;
            Predicate = _ => false;
            Selector = _ => default(B);
        }

        internal Matcher(A key, Func<A, bool> predicate, Func<A, B> selector) : this()
        {
            Key = key;
            Previous = NullMatcher<A, B>.It;
            Predicate = predicate;
            Selector = selector;
        }

        internal Matcher(A key, IMatcher<A, B> previous, Func<A, bool> predicate, Func<A, B> selector) : this()
        {
            Key = key;
            Previous = previous;
            Predicate = predicate;
            Selector = selector;
        }

        internal readonly A Key;
        private readonly IMatcher<A, B> Previous;
        private readonly Func<A, bool> Predicate;
        private readonly Func<A, B> Selector;

        public MatcherPredicate<A, B, A> Case(Func<A, bool> f)
        {
            return new MatcherPredicate<A, B, A>(Key, this, f);
        }

        public MatcherPredicate<A, B, C> Case<C>(Func<C, bool> f) where C : A
        {
            return new MatcherPredicate<A, B, C>(Key, this, x => x is C && f((C) x));
        }

        public MatcherPredicate<A, B, C> Case<C>() where C : A
        {
            return new MatcherPredicate<A, B, C>(Key, this, x => x is C);
        }

        public Maybe<B> End()
        {
            return Eval(Key);
        }

        public static implicit operator Maybe<B>(Matcher<A, B> matcher)
        {
            return matcher.End();
        }

        public Maybe<B> Eval(A _)
        {
            return Previous.Eval(Key).OrIf(Key, Predicate, Selector);
        }
    }

    public struct MatcherInferencePredicate<A, C> where C : A
    {
        internal MatcherInferencePredicate(A key, Func<A, bool> predicate) : this()
        {
            Key = key;
            Predicate = predicate;
        }
        
        private readonly A Key;
        private readonly Func<A, bool> Predicate;

        public Matcher<A, B> Then<B>(Func<C, B> f)
        {
            return new Matcher<A, B>(Key, Predicate, x => f(x.As<C>()));
        }
    }

    public struct MatcherPredicate<A, B, C> where C : A
    {
        internal MatcherPredicate(A key, IMatcher<A, B> previous, Func<A, bool> predicate) : this()
        {
            Key = key;
            Previous = previous;
            Predicate = predicate;
        }

        private readonly A Key;
        private readonly IMatcher<A, B> Previous;
        private readonly Func<A, bool> Predicate;

        public Matcher<A, B> Then(Func<C, B> f)
        {
            return new Matcher<A, B>(Key, Previous, Predicate, x => f((C) x));
        }
    }

    public struct MatcherDefault<A, B> : IMatcher<A, B>
    {
        internal MatcherDefault(A key, Func<A, B> defaultF) : this()
        {
            Key = key;
            Default = defaultF;
            Previous = NullMatcher<A, B>.It;
            Predicate = _ => false;
            Selector = _ => default(B);
        }

        internal MatcherDefault(A key, Func<A, B> defaultF, IMatcher<A, B> previous, Func<A, bool> predicate, Func<A, B> selector) : this()
        {
            Key = key;
            Default = defaultF;
            Previous = previous;
            Predicate = predicate;
            Selector = selector;
        }

        internal readonly A Key;
        private readonly Func<A, B> Default;
        private readonly IMatcher<A, B> Previous;
        private readonly Func<A, bool> Predicate;
        private readonly Func<A, B> Selector;

        public MatcherDefaultPredicate<A, B, A> Case(Func<A, bool> f)
        {
            return new MatcherDefaultPredicate<A, B, A>(Key, Default, this, f);
        }

        public MatcherDefaultPredicate<A, B, C> Case<C>(Func<C, bool> f) where C : A
        {
            return new MatcherDefaultPredicate<A, B, C>(Key, Default, this, x => x is C && f((C) x));
        }

        public MatcherDefaultPredicate<A, B, C> Case<C>() where C : A
        {
            return new MatcherDefaultPredicate<A, B, C>(Key, Default, this, x => x is C);
        }

        public B End()
        {
            return Eval(Key).OrElseEval(Key, Default);
        }

        public static implicit operator B(MatcherDefault<A, B> matcher)
        {
            return matcher.End();
        }

        public Maybe<B> Eval(A _)
        {
            return Previous.Eval(Key).OrIf(Key, Predicate, Selector);
        }
    }

    public struct MatcherDefaultPredicate<A, B, C> where C : A
    {
        internal MatcherDefaultPredicate(A key, Func<A, B> defaultF, IMatcher<A, B> previous, Func<A, bool> predicate) : this()
        {
            Key = key;
            Default = defaultF;
            Previous = previous;
            Predicate = predicate;
        }

        private readonly A Key;
        private readonly Func<A, B> Default;
        private readonly IMatcher<A, B> Previous;
        private readonly Func<A, bool> Predicate;

        public MatcherDefault<A, B> Then(Func<C, B> f)
        {
            return new MatcherDefault<A, B>(Key, Default, Previous, Predicate, x => f(x.As<C>()));
        }
    }

    public static class Match<A>
    {
        public static MatcherFunc<A, B> Return<B>()
        {
            return new MatcherFunc<A, B>(_ => false, _ => default(B));
        }

        public static MatcherFuncDefault<A, B> Default<B>(Func<A, B> f)
        {
            return new MatcherFuncDefault<A, B>(f);
        }

        public static MatcherFuncDefault<A, B> Default<B>(Func<B> f)
        {
            return new MatcherFuncDefault<A, B>(_ => f());
        }

        public static MatcherFuncDefault<A, B> Default<B>(B val)
        {
            return new MatcherFuncDefault<A, B>(_ => val);
        }

        public static MatcherFuncInferencePredicate<A, A> Case(Func<A, bool> f)
        {
            return new MatcherFuncInferencePredicate<A, A>(f);
        }

        public static MatcherFuncInferencePredicate<A, A> Case(Func<bool> f)
        {
            return new MatcherFuncInferencePredicate<A, A>(_ => f());
        }

        public static MatcherFuncInferencePredicate<A, A> Case(A val)
        {
            return new MatcherFuncInferencePredicate<A, A>(val.Eq());
        }

        public static MatcherFuncInferencePredicate<A, C> Case<C>(Func<C, bool> f) where C : A
        {
            return new MatcherFuncInferencePredicate<A, C>(x => x is C && f((C) x));
        }

        public static MatcherFuncInferencePredicate<A, C> Case<C>() where C : A
        {
            return new MatcherFuncInferencePredicate<A, C>(x => x is C);
        }
    }

    public struct MatcherFunc<A, B> : IMatcher<A, B>
    {
        internal MatcherFunc(Func<A, bool> predicate, Func<A, B> selector) : this()
        {
            Previous = NullMatcher<A, B>.It;
            Predicate = predicate;
            Selector = selector;
        }
        
        internal MatcherFunc(IMatcher<A, B> previous, Func<A, bool> predicate, Func<A, B> selector) : this()
        {
            Previous = previous;
            Predicate = predicate;
            Selector = selector;
        }

        private readonly IMatcher<A, B> Previous;
        private readonly Func<A, bool> Predicate;
        private readonly Func<A, B> Selector;

        public MatcherFuncPredicate<A, B, A> Case(Func<A, bool> f)
        {
            return new MatcherFuncPredicate<A, B, A>(this, f);
        }

        public MatcherFuncPredicate<A, B, C> Case<C>(Func<C, bool> f) where C : A
        {
            return new MatcherFuncPredicate<A, B, C>(this, x => x is C && f((C) x));
        }

        public MatcherFuncPredicate<A, B, C> Case<C>() where C : A
        {
            return new MatcherFuncPredicate<A, B, C>(this, x => x is C);
        }

        public Func<A, B> Else(Func<A, B> f)
        {
            var me = this;
            return key => me.Eval(key).OrElseEval(key, f);
        }

        public Matcher<A, B> On(A key)
        {
            return new Matcher<A,B>(key, Previous, Predicate, Selector);
        }

        public Func<A, Maybe<B>> End()
        {
            return Eval;
        }

        public Maybe<B> Eval(A key)
        {
            return Maybe.If(key, Predicate, Selector).OrEval(key, Previous.Eval);
        }
    }

    public struct MatcherFuncPredicate<A, B, C> where C : A
    {
        internal MatcherFuncPredicate(IMatcher<A, B> previous, Func<A, bool> predicate) : this()
        {
            Previous = previous;
            Predicate = predicate;
        }

        private readonly IMatcher<A, B> Previous;
        private readonly Func<A, bool> Predicate;

        public MatcherFunc<A, B> Then(Func<C, B> f)
        {
            return new MatcherFunc<A, B>(Previous, Predicate, x => f(x.As<C>()));
        }
    }

    public struct MatcherFuncInferencePredicate<A, C> where C : A
    {
        internal MatcherFuncInferencePredicate(Func<A, bool> predicate) : this()
        {
            Predicate = predicate;
        }

        private readonly Func<A, bool> Predicate;

        public MatcherFunc<A, B> Then<B>(Func<C, B> f)
        {
            return new MatcherFunc<A, B>(Predicate, x => f(x.As<C>()));
        }
    }

    public struct MatcherFuncDefault<A, B> : IMatcher<A, B>
    {
        internal MatcherFuncDefault(Func<A, B> defaultF)
        {
            Default = defaultF;
            Previous = NullMatcher<A, B>.It;
            Predicate = _ => false;
            Selector = _ => default(B);
        }

        internal MatcherFuncDefault(Func<A, B> defaultF, IMatcher<A, B> previous, Func<A, bool> predicate, Func<A, B> selector)
        {
            Default = defaultF;
            Previous = previous;
            Predicate = predicate;
            Selector = selector;
        }

        private readonly Func<A, B> Default;
        private readonly IMatcher<A, B> Previous;
        private readonly Func<A, bool> Predicate;
        private readonly Func<A, B> Selector;

        public MatcherFuncDefaultPredicate<A, B, A> Case(Func<A, bool> f)
        {
            return new MatcherFuncDefaultPredicate<A, B, A>(Default, this, f);
        }

        public MatcherFuncDefaultPredicate<A, B, C> Case<C>(Func<C, bool> f) where C : A
        {
            return new MatcherFuncDefaultPredicate<A, B, C>(Default, this, x => x is C && f((C) x));
        }

        public MatcherFuncDefaultPredicate<A, B, C> Case<C>() where C : A
        {
            return new MatcherFuncDefaultPredicate<A, B, C>(Default, this, x => x is C);
        }

        public MatcherDefault<A, B> On(A key)
        {
            return new MatcherDefault<A, B>(key, Default, Previous, Predicate, Selector);
        }

        public Func<A, B> End()
        {
            var me = this;
            return key => me.Eval(key).OrElseEval(key, me.Default);
        }

        public Maybe<B> Eval(A key)
        {
            return Previous.Eval(key).OrIf(key, Predicate, Selector);
        }
    }

    public struct MatcherFuncDefaultPredicate<A, B, C> where C : A
    {
        internal MatcherFuncDefaultPredicate(Func<A, B> defaultF, IMatcher<A, B> previous, Func<A, bool> predicate)
        {
            Default = defaultF;
            Previous = previous;
            Predicate = predicate;
        }

        private readonly Func<A, B> Default;
        private readonly IMatcher<A, B> Previous;
        private readonly Func<A, bool> Predicate;

        public MatcherFuncDefault<A, B> Then(Func<C, B> f)
        {
            return new MatcherFuncDefault<A, B>(Default, Previous, Predicate, x => f((C) x));
        }
    }

    public static class MatcherExtensions
    {
        public static MatcherPredicate<A, B, A> Case<A, B>(this Matcher<A, B> matcher, A val)
        {
            return matcher.Case(val.Eq());
        }

        public static MatcherPredicate<A, B, A> Case<A, B>(this Matcher<A, B> matcher, bool cond)
        {
            return matcher.Case(_ => cond);
        }

        public static MatcherPredicate<A, B, A> Case<A, B>(this Matcher<A, B> matcher, Func<bool> f)
        {
            return matcher.Case(_ => f());
        }

        public static MatcherPredicate<A, B, A> Case<A, B>(this Matcher<A, B> matcher, Type type)
        {
            return matcher.Case(key => type.IsInstanceOfType(key));
        }

        public static Matcher<A, B> Then<A, B, C>(this MatcherPredicate<A, B, C> matcher, B val) where C : A
        {
            return matcher.Then(_ => val);
        }

        public static Matcher<A, B> Then<A, B, C>(this MatcherPredicate<A, B, C> matcher, Func<B> f) where C : A
        {
            return matcher.Then(_ => f());
        }
    }

    public static class MatcherInferenceExtensions
    {
        public static MatcherInferencePredicate<A, A> Case<A>(this MatcherInitial<A> matcher, A val)
        {
            return matcher.Case(val.Eq());
        }

        public static MatcherInferencePredicate<A, A> Case<A>(this MatcherInitial<A> matcher, bool cond)
        {
            return matcher.Case(_ => cond);
        }

        public static MatcherInferencePredicate<A, A> Case<A>(this MatcherInitial<A> matcher, Func<bool> f)
        {
            return matcher.Case(_ => f());
        }

        public static MatcherInferencePredicate<A, A> Case<A>(this MatcherInitial<A> matcher, Type type)
        {
            return matcher.Case(key => type.IsInstanceOfType(key));
        }
        
        public static Matcher<A, B> Then<A, B, C>(this MatcherInferencePredicate<A, C> matcher, B val) where C : A
        {
            return matcher.Then(_ => val);
        }

        public static Matcher<A, B> Then<A, B, C>(this MatcherInferencePredicate<A, C> matcher, Func<B> f) where C : A
        {
            return matcher.Then(_ => f());
        }
    }

    public static class MatcherStringExtensions
    {
        public static MatcherPredicate<String, B, String> Case<B>(this Matcher<String, B> matcher, Regex regex)
        {
            return matcher.Case(regex.IsMatch);
        }
        
        public static MatcherInferencePredicate<String, String> Case(this MatcherInitial<String> matcher, Regex regex)
        {
            return matcher.Case(regex.IsMatch);
        }

        /// <summary>
        /// Does case-insensitive equality comparison with the key.
        /// </summary>
        public static MatcherPredicate<String, B, String> CaseInsenstive<B>(this Matcher<String, B> matcher, String s)
        {
            return matcher.Case(s.EqualsIgnoreCase());
        }

        /// <summary>
        /// Does case-insensitive equality comparison with the key.
        /// </summary>
        public static MatcherInferencePredicate<String, String> CaseInsensitive(this MatcherInitial<String> matcher, String s)
        {
            return matcher.Case(s.EqualsIgnoreCase());
        }
    }

    public static class MatcherFuncStringExtensions
    {
        public static MatcherFuncPredicate<String, B, String> Case<B>(this MatcherFunc<String, B> matcher, Regex regex)
        {
            return matcher.Case(regex.IsMatch);
        }

        /// <summary>
        /// Does case-insensitive equality comparison with the key.
        /// </summary>
        public static MatcherFuncPredicate<String, B, String> CaseInsenstive<B>(this MatcherFunc<String, B> matcher, String s)
        {
            return matcher.Case(s.EqualsIgnoreCase());
        }
    }

    public static class MatcherElseExtensions
    {
        public static B Else<A, B>(this Matcher<A, B> matcher, Func<A, B> f)
        {
            return matcher.End().OrElseEval(matcher.Key, f);
        }

        public static B Else<A, B>(this Matcher<A, B> matcher, Func<B> f)
        {
            return matcher.End().OrElseEval(f);
        }

        public static B Else<A, B>(this Matcher<A, B> matcher, B val)
        {
            return matcher.End().OrElse(val);
        }
        
        public static B Else<A, B>(this MatcherInitial<A> matcher, Func<A, B> f)
        {
            return f(matcher.Key);
        }

        public static B Else<A, B>(this MatcherInitial<A> matcher, Func<B> f)
        {
            return f();
        }

        public static B Else<A, B>(this MatcherInitial<A> matcher, B val)
        {
            return val;
        }
    }

    public static class MatcherDefaultExtensions
    {
        public static MatcherDefault<A, B> Default<A, B>(this MatcherInitial<A> matcher, Func<B> f)
        {
            return matcher.Default(_ => f());
        }

        public static MatcherDefault<A, B> Default<A, B>(this MatcherInitial<A> matcher, B val)
        {
            return matcher.Default(_ => val);
        }

        public static MatcherDefaultPredicate<A, B, A> Case<A, B>(this MatcherDefault<A, B> matcher, A val)
        {
            return matcher.Case(val.Eq());
        }

        public static MatcherDefaultPredicate<A, B, A> Case<A, B>(this MatcherDefault<A, B> matcher, bool cond)
        {
            return matcher.Case(_ => cond);
        }

        public static MatcherDefaultPredicate<A, B, A> Case<A, B>(this MatcherDefault<A, B> matcher, Func<bool> f)
        {
            return matcher.Case(_ => f());
        }

        public static MatcherDefaultPredicate<A, B, A> Case<A, B>(this MatcherDefault<A, B> matcher, Type type)
        {
            return matcher.Case(key => type.IsInstanceOfType(key));
        }

        public static MatcherDefault<A, B> Then<A, B, C>(this MatcherDefaultPredicate<A, B, C> matcher, B val) where C : A
        {
            return matcher.Then(_ => val);
        }

        public static MatcherDefault<A, B> Then<A, B, C>(this MatcherDefaultPredicate<A, B, C> matcher, Func<B> f) where C : A
        {
            return matcher.Then(_ => f());
        }
    }

    public static class MatcherFuncExtensions
    {
        public static MatcherFuncPredicate<A, B, A> Case<A, B>(this MatcherFunc<A, B> matcher, A val)
        {
            return matcher.Case(val.Eq());
        }

        public static MatcherFuncPredicate<A, B, A> Case<A, B>(this MatcherFunc<A, B> matcher, bool cond)
        {
            return matcher.Case(_ => cond);
        }

        public static MatcherFuncPredicate<A, B, A> Case<A, B>(this MatcherFunc<A, B> matcher, Func<bool> f)
        {
            return matcher.Case(_ => f());
        }

        public static MatcherFuncPredicate<A, B, A> Case<A, B>(this MatcherFunc<A, B> matcher, Type type)
        {
            return matcher.Case(key => type.IsInstanceOfType(key));
        }

        public static MatcherFunc<A, B> Then<A, B, C>(this MatcherFuncPredicate<A, B, C> matcher, Func<B> f) where C : A
        {
            return matcher.Then(_ => f());
        }

        public static MatcherFunc<A, B> Then<A, B, C>(this MatcherFuncPredicate<A, B, C> matcher, B val) where C : A
        {
            return matcher.Then(_ => val);
        }

        public static MatcherFunc<A, B> Then<A, B, C>(this MatcherFuncInferencePredicate<A, C> matcher, Func<B> f) where C : A
        {
            return matcher.Then(_ => f());
        }

        public static MatcherFunc<A, B> Then<A, B, C>(this MatcherFuncInferencePredicate<A, C> matcher, B val) where C : A
        {
            return matcher.Then(_ => val);
        }
        
        public static Func<A, B> Else<A, B>(this MatcherFunc<A, B> matcher, Func<A, B> f)
        {
            return key => matcher.Eval(key).OrElseEval(key, f);
        }

        public static Func<A, B> Else<A, B>(this MatcherFunc<A, B> matcher, Func<B> f)
        {
            return key => matcher.Eval(key).OrElseEval(f);
        }

        public static Func<A, B> Else<A, B>(this MatcherFunc<A, B> matcher, B val)
        {
            return key => matcher.Eval(key).OrElse(val);
        }
    }

    public static class MatcherFuncDefaultExtensions
    {
        public static MatcherFuncDefaultPredicate<A, B, A> Case<A, B>(this MatcherFuncDefault<A, B> matcher, A val)
        {
            return matcher.Case(val.Eq());
        }

        public static MatcherFuncDefaultPredicate<A, B, A> Case<A, B>(this MatcherFuncDefault<A, B> matcher, bool cond)
        {
            return matcher.Case(_ => cond);
        }

        public static MatcherFuncDefaultPredicate<A, B, A> Case<A, B>(this MatcherFuncDefault<A, B> matcher, Func<bool> f)
        {
            return matcher.Case(_ => f());
        }

        public static MatcherFuncDefaultPredicate<A, B, A> Case<A, B>(this MatcherFuncDefault<A, B> matcher, Type type)
        {
            return matcher.Case(key => type.IsInstanceOfType(key));
        }

        public static MatcherFuncDefault<A, B> Then<A, B, C>(this MatcherFuncDefaultPredicate<A, B, C> matcher, B val) where C : A
        {
            return matcher.Then(_ => val);
        }

        public static MatcherFuncDefault<A, B> Then<A, B, C>(this MatcherFuncDefaultPredicate<A, B, C> matcher, Func<B> f) where C : A
        {
            return matcher.Then(_ => f());
        }
    }
}
