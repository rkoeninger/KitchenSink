using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ZedSharp
{
    internal interface IMatcher<A, B>
    {
        /// <summary>Key may be ignored.</summary>
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
        
        internal readonly A Key;

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
        public MatcherInferencePredicate<A, C> Case<C>(Func<C, bool> f) where C : A
        {
            return new MatcherInferencePredicate<A, C>(Key, x => x is C && f((C) x));
        }

        /// <summary>
        /// Specifies a condition for the following consequent.
        /// Condition will only be evaluated if no previous Case condition was evaluated to true.
        /// After this, call Then.
        /// </summary>
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
        public MatcherPredicate<A, B, C> Case<C>(Func<C, bool> f) where C : A
        {
            return new MatcherPredicate<A, B, C>(Key, this, x => x is C && f((C) x));
        }

        /// <summary>
        /// Specifies a condition for the following consequent.
        /// Condition will only be evaluated if no previous Case condition was evaluated to true.
        /// After this, call Then.
        /// </summary>
        public MatcherPredicate<A, B, C> Case<C>() where C : A
        {
            return new MatcherPredicate<A, B, C>(Key, this, x => x is C);
        }

        /// <summary>Gets the result of the Match as an Maybe. Won't have a value if no cases were matched.</summary>
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

        /// <summary>
        /// Specifies consequence if previous Case was the first true Case in this Match.
        /// After this, call Case or End.
        /// </summary>
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

        /// <summary>
        /// Specifies a condition for the following consequent.
        /// Condition will only be evaluated if no previous Case condition was evaluated to true.
        /// After this, call Then.
        /// </summary>
        public MatcherDefaultPredicate<A, B, A> Case(Func<A, bool> f)
        {
            return new MatcherDefaultPredicate<A, B, A>(Key, Default, this, f);
        }

        /// <summary>
        /// Specifies a condition for the following consequent.
        /// Condition will only be evaluated if no previous Case condition was evaluated to true.
        /// After this, call Then.
        /// </summary>
        public MatcherDefaultPredicate<A, B, C> Case<C>(Func<C, bool> f) where C : A
        {
            return new MatcherDefaultPredicate<A, B, C>(Key, Default, this, x => x is C && f((C) x));
        }

        /// <summary>
        /// Specifies a condition for the following consequent.
        /// Condition will only be evaluated if no previous Case condition was evaluated to true.
        /// After this, call Then.
        /// </summary>
        public MatcherDefaultPredicate<A, B, C> Case<C>() where C : A
        {
            return new MatcherDefaultPredicate<A, B, C>(Key, Default, this, x => x is C);
        }

        /// <summary>Gets the result of the Match. Will be default value if no cases were matched.</summary>
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

        /// <summary>
        /// Specifies consequence if previous Case was the first true Case in this Match.
        /// After this, call Case or End.
        /// </summary>
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

        public MatcherFuncDefaultPredicate<A, B, C> Case<C>(Func<C, bool> f) where C : A
        {
            return new MatcherFuncDefaultPredicate<A, B, C>(Default, this, x => x is C && f((C) x));
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
        /// <summary>
        /// Specifies a condition for the following consequent.
        /// Condition will only be evaluated if no previous Case condition was evaluated to true.
        /// After this, call Then.
        /// </summary>
        public static MatcherPredicate<A, B, A> Case<A, B>(this Matcher<A, B> matcher, A val)
        {
            return matcher.Case(val.Eq());
        }

        /// <summary>
        /// Specifies a condition for the following consequent.
        /// Condition is evaluated immediately as it's not in a continuation.
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
        public static Matcher<A, B> Then<A, B, C>(this MatcherPredicate<A, B, C> matcher, B val) where C : A
        {
            return matcher.Then(_ => val);
        }

        /// <summary>
        /// Specifies consequence if previous Case was the first true Case in this Match.
        /// After this, call Case or End.
        /// </summary>
        public static Matcher<A, B> Then<A, B, C>(this MatcherPredicate<A, B, C> matcher, Func<B> f) where C : A
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
            return matcher.Case(val.Eq());
        }

        /// <summary>
        /// Specifies a condition for the following consequent.
        /// Condition is evaluated immediately as it's not in a continuation.
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
        public static Matcher<A, B> Then<A, B, C>(this MatcherInferencePredicate<A, C> matcher, B val) where C : A
        {
            return matcher.Then(_ => val);
        }

        /// <summary>
        /// Specifies consequence if previous Case was the first true Case in this Match.
        /// Also infers return type of Match.
        /// After this, call Case or End.
        /// </summary>
        public static Matcher<A, B> Then<A, B, C>(this MatcherInferencePredicate<A, C> matcher, Func<B> f) where C : A
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
        /// A final maybe of the matcher's return type is the result of Else.
        /// </summary>
        public static B Else<A, B>(this Matcher<A, B> matcher, Func<A, B> f)
        {
            return matcher.End().OrElseEval(matcher.Key, f);
        }

        /// <summary>
        /// Specifies the following consequence should be run if no previous case was true.
        /// No more Case methods can be called after this.
        /// A final maybe of the matcher's return type is the result of Else.
        /// </summary>
        public static B Else<A, B>(this Matcher<A, B> matcher, Func<B> f)
        {
            return matcher.End().OrElseEval(f);
        }

        /// <summary>
        /// Specifies the following consequence should be run if no previous case was true.
        /// No more Case methods can be called after this.
        /// A final maybe of the matcher's return type is the result of Else.
        /// </summary>
        public static B Else<A, B>(this Matcher<A, B> matcher, B val)
        {
            return matcher.End().OrElse(val);
        }
        
        /// <summary>
        /// Specifies the following consequence should be run if no previous case was true.
        /// No more Case methods can be called after this.
        /// A final maybe of the matcher's return type is the result of Else.
        /// </summary>
        public static B Else<A, B>(this MatcherInitial<A> matcher, Func<A, B> f)
        {
            return f(matcher.Key);
        }

        /// <summary>
        /// Specifies the following consequence should be run if no previous case was true.
        /// No more Case methods can be called after this.
        /// A final maybe of the matcher's return type is the result of Else.
        /// </summary>
        public static B Else<A, B>(this MatcherInitial<A> matcher, Func<B> f)
        {
            return f();
        }

        /// <summary>
        /// Specifies the following consequence should be run if no previous case was true.
        /// No more Case methods can be called after this.
        /// A final maybe of the matcher's return type is the result of Else.
        /// </summary>
        public static B Else<A, B>(this MatcherInitial<A> matcher, B val)
        {
            return val;
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
        /// Condition is evaluated immediately as it's not in a continuation.
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
        public static MatcherDefault<A, B> Then<A, B, C>(this MatcherDefaultPredicate<A, B, C> matcher, B val) where C : A
        {
            return matcher.Then(_ => val);
        }

        /// <summary>
        /// Specifies consequence if previous Case was the first true Case in this Match.
        /// After this, call End.
        /// </summary>
        public static MatcherDefault<A, B> Then<A, B, C>(this MatcherDefaultPredicate<A, B, C> matcher, Func<B> f) where C : A
        {
            return matcher.Then(_ => f());
        }
    }

    public static class MatcherFuncExtensions
    {
        public static MatcherFuncPredicate<A, B, A> Case<A, B>(this MatcherFunc<A, B> matcher, Func<bool> f)
        {
            return matcher.Case(_ => f());
        }

        public static MatcherFuncPredicate<A, B, A> Case<A, B>(this MatcherFunc<A, B> matcher, A val)
        {
            return matcher.Case(val.Eq());
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
}
