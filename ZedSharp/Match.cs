using System;
using System.Collections.Generic;
using System.Linq;

namespace ZedSharp
{
    public static class Match
    {
        public static MatcherK0<A> On<A>(A k)
        {
            return new MatcherK0<A>(k);
        }

        public static MatcherKD0<A, B> On<A, B>(A k, Func<A, B> defaultF)
        {
            return new MatcherKD0<A, B>(k, defaultF);
        }

        public static Matcher0<A> From<A>()
        {
            return new Matcher0<A>();
        }
    }

    public static class Match<A>
    {
        public static MatcherK0<A> On(A k)
        {
            return new MatcherK0<A>(k);
        }

        public static MatcherKD0<A, B> On<B>(A k, Func<A, B> defaultF)
        {
            return new MatcherKD0<A, B>(k, defaultF);
        }

        public static Matcher0<A, B> Return<B>()
        {
            return new Matcher0<A, B>();
        }

        public static MatcherD0<A, B> Default<B>(Func<A, B> v)
        {
            return new MatcherD0<A, B>(v);
        }

        public static MatcherD0<A, B> Default<B>(Func<B> v)
        {
            return new MatcherD0<A, B>(_ => v());
        }

        public static MatcherD0<A, B> Default<B>(B v)
        {
            return new MatcherD0<A, B>(_ => v);
        }
        
        public static Matcher1<A, B> Case<B>(Func<A, bool> p, Func<A, B> f)
        {
            return new Matcher1<A, B>(p, f);
        }

        public static Matcher1<A, B> Case<B>(Func<A, bool> p, Func<B> f)
        {
            return new Matcher1<A, B>(p, _ => f());
        }

        public static Matcher1<A, B> Case<B>(Func<A, bool> p, B f)
        {
            return new Matcher1<A, B>(p, _ => f);
        }

        public static Matcher1<A, B> Case<B>(Func<bool> p, Func<A, B> f)
        {
            return new Matcher1<A, B>(_ => p(), f);
        }

        public static Matcher1<A, B> Case<B>(Func<bool> p, Func<B> f)
        {
            return new Matcher1<A, B>(_ => p(), _ => f());
        }

        public static Matcher1<A, B> Case<B>(Func<bool> p, B f)
        {
            return new Matcher1<A, B>(_ => p(), _ => f);
        }

        public static Matcher1<A, B> Case<B>(bool p, Func<A, B> f)
        {
            return new Matcher1<A, B>(_ => p, f);
        }

        public static Matcher1<A, B> Case<B>(bool p, Func<B> f)
        {
            return new Matcher1<A, B>(_ => p, _ => f());
        }

        public static Matcher1<A, B> Case<B>(bool p, B f)
        {
            return new Matcher1<A, B>(_ => p, _ => f);
        }

        public static Matcher1<A, B> Case<B>(A p, Func<A, B> f)
        {
            return new Matcher1<A, B>(x => Object.Equals(x, p), f);
        }

        public static Matcher1<A, B> Case<B>(A p, Func<B> f)
        {
            return new Matcher1<A, B>(x => Object.Equals(x, p), _ => f());
        }

        public static Matcher1<A, B> Case<B>(A p, B f)
        {
            return new Matcher1<A, B>(x => Object.Equals(x, p), _ => f);
        }

        public static Matcher1<A, B> Case<B>(Type p, Func<A, B> f)
        {
            return new Matcher1<A, B>(x => p.IsInstanceOfType(x), f);
        }

        public static Matcher1<A, B> Case<B>(Type p, Func<B> f)
        {
            return new Matcher1<A, B>(x => p.IsInstanceOfType(x), _ => f());
        }

        public static Matcher1<A, B> Case<B>(Type p, B f)
        {
            return new Matcher1<A, B>(x => p.IsInstanceOfType(x), _ => f);
        }
    }

    public static class Match<A, B>
    {
        public static MatcherK0<A, B> On(A k)
        {
            return new MatcherK0<A, B>(k);
        }

        public static MatcherKD0<A, B> On(A k, Func<A, B> defaultF)
        {
            return new MatcherKD0<A, B>(k, defaultF);
        }
    }

    /// <remarks>Key may be ignored, depending on implementation.</remarks>
    internal interface IMatcher<A, B>
    {
        Maybe<B> Eval(A k);
    }

    public class Matcher0<A>
    {
        internal Matcher0()
        {
        }

        public Matcher0<A, B> Return<B>()
        {
            return new Matcher0<A, B>();
        }
        
        public MatcherD0<A, B> Default<B>(Func<A, B> defaultF)
        {
            return new MatcherD0<A, B>(defaultF);
        }

        public MatcherD0<A, B> Default<B>(Func<B> defaultF)
        {
            return new MatcherD0<A, B>(_ => defaultF());
        }

        public MatcherD0<A, B> Default<B>(B defaultF)
        {
            return new MatcherD0<A, B>(_ => defaultF);
        }
        
        public Matcher1<A, B> Case<B>(Func<A, bool> p, Func<A, B> f)
        {
            return new Matcher1<A, B>(p, f);
        }

        public Matcher1<A, B> Case<B>(Func<A, bool> p, Func<B> f)
        {
            return new Matcher1<A, B>(p, _ => f());
        }

        public Matcher1<A, B> Case<B>(Func<A, bool> p, B f)
        {
            return new Matcher1<A, B>(p, _ => f);
        }

        public Matcher1<A, B> Case<B>(Func<bool> p, Func<A, B> f)
        {
            return new Matcher1<A, B>(_ => p(), f);
        }

        public Matcher1<A, B> Case<B>(Func<bool> p, Func<B> f)
        {
            return new Matcher1<A, B>(_ => p(), _ => f());
        }

        public Matcher1<A, B> Case<B>(Func<bool> p, B f)
        {
            return new Matcher1<A, B>(_ => p(), _ => f);
        }

        public Matcher1<A, B> Case<B>(bool p, Func<A, B> f)
        {
            return new Matcher1<A, B>(_ => p, f);
        }

        public Matcher1<A, B> Case<B>(bool p, Func<B> f)
        {
            return new Matcher1<A, B>(_ => p, _ => f());
        }

        public Matcher1<A, B> Case<B>(bool p, B f)
        {
            return new Matcher1<A, B>(_ => p, _ => f);
        }

        public Matcher1<A, B> Case<B>(A p, Func<A, B> f)
        {
            return new Matcher1<A, B>(x => Object.Equals(x, p), f);
        }

        public Matcher1<A, B> Case<B>(A p, Func<B> f)
        {
            return new Matcher1<A, B>(x => Object.Equals(x, p), _ => f());
        }

        public Matcher1<A, B> Case<B>(A p, B f)
        {
            return new Matcher1<A, B>(x => Object.Equals(x, p), _ => f);
        }

        public Matcher1<A, B> Case<B>(Type p, Func<A, B> f)
        {
            return new Matcher1<A, B>(x => p.IsInstanceOfType(x), f);
        }

        public Matcher1<A, B> Case<B>(Type p, Func<B> f)
        {
            return new Matcher1<A, B>(x => p.IsInstanceOfType(x), _ => f());
        }

        public Matcher1<A, B> Case<B>(Type p, B f)
        {
            return new Matcher1<A, B>(x => p.IsInstanceOfType(x), _ => f);
        }
    }

    
    public class Matcher0<A, B> : IMatcher<A, B>
    {
        internal Matcher0()
        {

        }


        
        public Maybe<B> Eval(A k)
        {
            return Maybe<B>.None;
        }

        public Func<A, Maybe<B>> End
        {
            get { return Eval; }
        }
        
        public Func<A, B> Else(Func<A, B> f)
        {
            var me = this;
            return k => me.End(k).OrElseEval(k, f);
        }


        public Func<A, B> Else(Func<B> f)
        {
            var me = this;
            return k => me.End(k).OrElseEval(f);
        }


        public Func<A, B> Else(B f)
        {
            var me = this;
            return k => me.End(k).OrElse(f);
        }

        
        public MatcherK0<A, B> On(A k)
        {
            return new MatcherK0<A, B>(k);
        }

        
        public MatcherD0<A, B> Default(Func<A, B> defaultF)
        {
            return new MatcherD0<A, B>(defaultF);
        }

        public MatcherD0<A, B> Default(Func<B> defaultF)
        {
            return new MatcherD0<A, B>(_ => defaultF());
        }

        public MatcherD0<A, B> Default(B defaultF)
        {
            return new MatcherD0<A, B>(_ => defaultF);
        }
        
        public Matcher1<A, B> Case(Func<A, bool> p, Func<A, B> f)
        {
            return new Matcher1<A, B>(p, f);
        }

        public Matcher1<A, B> Case(Func<A, bool> p, Func<B> f)
        {
            return new Matcher1<A, B>(p, _ => f());
        }

        public Matcher1<A, B> Case(Func<A, bool> p, B f)
        {
            return new Matcher1<A, B>(p, _ => f);
        }

        public Matcher1<A, B> Case(Func<bool> p, Func<A, B> f)
        {
            return new Matcher1<A, B>(_ => p(), f);
        }

        public Matcher1<A, B> Case(Func<bool> p, Func<B> f)
        {
            return new Matcher1<A, B>(_ => p(), _ => f());
        }

        public Matcher1<A, B> Case(Func<bool> p, B f)
        {
            return new Matcher1<A, B>(_ => p(), _ => f);
        }

        public Matcher1<A, B> Case(bool p, Func<A, B> f)
        {
            return new Matcher1<A, B>(_ => p, f);
        }

        public Matcher1<A, B> Case(bool p, Func<B> f)
        {
            return new Matcher1<A, B>(_ => p, _ => f());
        }

        public Matcher1<A, B> Case(bool p, B f)
        {
            return new Matcher1<A, B>(_ => p, _ => f);
        }

        public Matcher1<A, B> Case(A p, Func<A, B> f)
        {
            return new Matcher1<A, B>(x => Object.Equals(x, p), f);
        }

        public Matcher1<A, B> Case(A p, Func<B> f)
        {
            return new Matcher1<A, B>(x => Object.Equals(x, p), _ => f());
        }

        public Matcher1<A, B> Case(A p, B f)
        {
            return new Matcher1<A, B>(x => Object.Equals(x, p), _ => f);
        }

        public Matcher1<A, B> Case(Type p, Func<A, B> f)
        {
            return new Matcher1<A, B>(x => p.IsInstanceOfType(x), f);
        }

        public Matcher1<A, B> Case(Type p, Func<B> f)
        {
            return new Matcher1<A, B>(x => p.IsInstanceOfType(x), _ => f());
        }

        public Matcher1<A, B> Case(Type p, B f)
        {
            return new Matcher1<A, B>(x => p.IsInstanceOfType(x), _ => f);
        }
        
        public Matcher1<A, B> Case<C>(Func<C, bool> p, Func<C, B> f) where C : A
        {
            return new Matcher1<A, B>(x => x is C && p((C) x), x => f((C) x));
        }
       
        public Matcher1<A, B> Case<C>(Func<C, bool> p, Func<B> f) where C : A
        {
            return new Matcher1<A, B>(x => x is C && p((C) x), _ => f());
        }
       
        public Matcher1<A, B> Case<C>(Func<C, bool> p, B f) where C : A
        {
            return new Matcher1<A, B>(x => x is C && p((C) x), _ => f);
        }
       
        public Matcher1<A, B> Case<C>(Func<bool> p, Func<C, B> f) where C : A
        {
            return new Matcher1<A, B>(_ => p(), x => f((C) x));
        }
       
        public Matcher1<A, B> Case<C>(Func<bool> p, Func<B> f) where C : A
        {
            return new Matcher1<A, B>(_ => p(), _ => f());
        }
       
        public Matcher1<A, B> Case<C>(Func<bool> p, B f) where C : A
        {
            return new Matcher1<A, B>(_ => p(), _ => f);
        }
       
        public Matcher1<A, B> Case<C>(bool p, Func<C, B> f) where C : A
        {
            return new Matcher1<A, B>(_ => p, x => f((C) x));
        }
       
        public Matcher1<A, B> Case<C>(bool p, Func<B> f) where C : A
        {
            return new Matcher1<A, B>(_ => p, _ => f());
        }
       
        public Matcher1<A, B> Case<C>(bool p, B f) where C : A
        {
            return new Matcher1<A, B>(_ => p, _ => f);
        }
           
    }

    
    public class Matcher1<A, B> : IMatcher<A, B>
    {
        internal Matcher1(Func<A, bool> p, Func<A, B> f)
        {
            Condition = p;
            Consequent = f;

        }

        private readonly Func<A, bool> Condition;
        private readonly Func<A, B> Consequent;

        
        public Maybe<B> Eval(A k)
        {
            return Maybe.If(k, Condition, Consequent);
        }

        public Func<A, Maybe<B>> End
        {
            get { return Eval; }
        }
        
        public Func<A, B> Else(Func<A, B> f)
        {
            var me = this;
            return k => me.End(k).OrElseEval(k, f);
        }


        public Func<A, B> Else(Func<B> f)
        {
            var me = this;
            return k => me.End(k).OrElseEval(f);
        }


        public Func<A, B> Else(B f)
        {
            var me = this;
            return k => me.End(k).OrElse(f);
        }

        
        public MatcherK1<A, B> On(A k)
        {
            return new MatcherK1<A, B>(k, Condition, Consequent);
        }

        
        public MatcherD1<A, B> Default(Func<A, B> defaultF)
        {
            return new MatcherD1<A, B>(defaultF, Condition, Consequent);
        }

        public MatcherD1<A, B> Default(Func<B> defaultF)
        {
            return new MatcherD1<A, B>(_ => defaultF(), Condition, Consequent);
        }

        public MatcherD1<A, B> Default(B defaultF)
        {
            return new MatcherD1<A, B>(_ => defaultF, Condition, Consequent);
        }
        
        public MatcherN<A, B> Case(Func<A, bool> p, Func<A, B> f)
        {
            return new MatcherN<A, B>(this, p, f);
        }

        public MatcherN<A, B> Case(Func<A, bool> p, Func<B> f)
        {
            return new MatcherN<A, B>(this, p, _ => f());
        }

        public MatcherN<A, B> Case(Func<A, bool> p, B f)
        {
            return new MatcherN<A, B>(this, p, _ => f);
        }

        public MatcherN<A, B> Case(Func<bool> p, Func<A, B> f)
        {
            return new MatcherN<A, B>(this, _ => p(), f);
        }

        public MatcherN<A, B> Case(Func<bool> p, Func<B> f)
        {
            return new MatcherN<A, B>(this, _ => p(), _ => f());
        }

        public MatcherN<A, B> Case(Func<bool> p, B f)
        {
            return new MatcherN<A, B>(this, _ => p(), _ => f);
        }

        public MatcherN<A, B> Case(bool p, Func<A, B> f)
        {
            return new MatcherN<A, B>(this, _ => p, f);
        }

        public MatcherN<A, B> Case(bool p, Func<B> f)
        {
            return new MatcherN<A, B>(this, _ => p, _ => f());
        }

        public MatcherN<A, B> Case(bool p, B f)
        {
            return new MatcherN<A, B>(this, _ => p, _ => f);
        }

        public MatcherN<A, B> Case(A p, Func<A, B> f)
        {
            return new MatcherN<A, B>(this, x => Object.Equals(x, p), f);
        }

        public MatcherN<A, B> Case(A p, Func<B> f)
        {
            return new MatcherN<A, B>(this, x => Object.Equals(x, p), _ => f());
        }

        public MatcherN<A, B> Case(A p, B f)
        {
            return new MatcherN<A, B>(this, x => Object.Equals(x, p), _ => f);
        }

        public MatcherN<A, B> Case(Type p, Func<A, B> f)
        {
            return new MatcherN<A, B>(this, x => p.IsInstanceOfType(x), f);
        }

        public MatcherN<A, B> Case(Type p, Func<B> f)
        {
            return new MatcherN<A, B>(this, x => p.IsInstanceOfType(x), _ => f());
        }

        public MatcherN<A, B> Case(Type p, B f)
        {
            return new MatcherN<A, B>(this, x => p.IsInstanceOfType(x), _ => f);
        }
        
        public MatcherN<A, B> Case<C>(Func<C, bool> p, Func<C, B> f) where C : A
        {
            return new MatcherN<A, B>(this, x => x is C && p((C) x), x => f((C) x));
        }
       
        public MatcherN<A, B> Case<C>(Func<C, bool> p, Func<B> f) where C : A
        {
            return new MatcherN<A, B>(this, x => x is C && p((C) x), _ => f());
        }
       
        public MatcherN<A, B> Case<C>(Func<C, bool> p, B f) where C : A
        {
            return new MatcherN<A, B>(this, x => x is C && p((C) x), _ => f);
        }
       
        public MatcherN<A, B> Case<C>(Func<bool> p, Func<C, B> f) where C : A
        {
            return new MatcherN<A, B>(this, _ => p(), x => f((C) x));
        }
       
        public MatcherN<A, B> Case<C>(Func<bool> p, Func<B> f) where C : A
        {
            return new MatcherN<A, B>(this, _ => p(), _ => f());
        }
       
        public MatcherN<A, B> Case<C>(Func<bool> p, B f) where C : A
        {
            return new MatcherN<A, B>(this, _ => p(), _ => f);
        }
       
        public MatcherN<A, B> Case<C>(bool p, Func<C, B> f) where C : A
        {
            return new MatcherN<A, B>(this, _ => p, x => f((C) x));
        }
       
        public MatcherN<A, B> Case<C>(bool p, Func<B> f) where C : A
        {
            return new MatcherN<A, B>(this, _ => p, _ => f());
        }
       
        public MatcherN<A, B> Case<C>(bool p, B f) where C : A
        {
            return new MatcherN<A, B>(this, _ => p, _ => f);
        }
           
    }

    
    public class MatcherN<A, B> : IMatcher<A, B>
    {
        internal MatcherN(IMatcher<A, B> m, Func<A, bool> p, Func<A, B> f)
        {
            Previous = m;
            Condition = p;
            Consequent = f;

        }

        private readonly IMatcher<A, B> Previous;
        private readonly Func<A, bool> Condition;
        private readonly Func<A, B> Consequent;

        
        public Maybe<B> Eval(A k)
        {
            return Previous.Eval(k).OrIf(k, Condition, Consequent);
        }

        public Func<A, Maybe<B>> End
        {
            get { return Eval; }
        }
        
        public Func<A, B> Else(Func<A, B> f)
        {
            var me = this;
            return k => me.End(k).OrElseEval(k, f);
        }


        public Func<A, B> Else(Func<B> f)
        {
            var me = this;
            return k => me.End(k).OrElseEval(f);
        }


        public Func<A, B> Else(B f)
        {
            var me = this;
            return k => me.End(k).OrElse(f);
        }

        
        public MatcherKN<A, B> On(A k)
        {
            return new MatcherKN<A, B>(k, Previous, Condition, Consequent);
        }

        
        public MatcherDN<A, B> Default(Func<A, B> defaultF)
        {
            return new MatcherDN<A, B>(this, defaultF, Condition, Consequent);
        }

        public MatcherDN<A, B> Default(Func<B> defaultF)
        {
            return new MatcherDN<A, B>(this, _ => defaultF(), Condition, Consequent);
        }

        public MatcherDN<A, B> Default(B defaultF)
        {
            return new MatcherDN<A, B>(this, _ => defaultF, Condition, Consequent);
        }
        
        public MatcherN<A, B> Case(Func<A, bool> p, Func<A, B> f)
        {
            return new MatcherN<A, B>(this, p, f);
        }

        public MatcherN<A, B> Case(Func<A, bool> p, Func<B> f)
        {
            return new MatcherN<A, B>(this, p, _ => f());
        }

        public MatcherN<A, B> Case(Func<A, bool> p, B f)
        {
            return new MatcherN<A, B>(this, p, _ => f);
        }

        public MatcherN<A, B> Case(Func<bool> p, Func<A, B> f)
        {
            return new MatcherN<A, B>(this, _ => p(), f);
        }

        public MatcherN<A, B> Case(Func<bool> p, Func<B> f)
        {
            return new MatcherN<A, B>(this, _ => p(), _ => f());
        }

        public MatcherN<A, B> Case(Func<bool> p, B f)
        {
            return new MatcherN<A, B>(this, _ => p(), _ => f);
        }

        public MatcherN<A, B> Case(bool p, Func<A, B> f)
        {
            return new MatcherN<A, B>(this, _ => p, f);
        }

        public MatcherN<A, B> Case(bool p, Func<B> f)
        {
            return new MatcherN<A, B>(this, _ => p, _ => f());
        }

        public MatcherN<A, B> Case(bool p, B f)
        {
            return new MatcherN<A, B>(this, _ => p, _ => f);
        }

        public MatcherN<A, B> Case(A p, Func<A, B> f)
        {
            return new MatcherN<A, B>(this, x => Object.Equals(x, p), f);
        }

        public MatcherN<A, B> Case(A p, Func<B> f)
        {
            return new MatcherN<A, B>(this, x => Object.Equals(x, p), _ => f());
        }

        public MatcherN<A, B> Case(A p, B f)
        {
            return new MatcherN<A, B>(this, x => Object.Equals(x, p), _ => f);
        }

        public MatcherN<A, B> Case(Type p, Func<A, B> f)
        {
            return new MatcherN<A, B>(this, x => p.IsInstanceOfType(x), f);
        }

        public MatcherN<A, B> Case(Type p, Func<B> f)
        {
            return new MatcherN<A, B>(this, x => p.IsInstanceOfType(x), _ => f());
        }

        public MatcherN<A, B> Case(Type p, B f)
        {
            return new MatcherN<A, B>(this, x => p.IsInstanceOfType(x), _ => f);
        }
        
        public MatcherN<A, B> Case<C>(Func<C, bool> p, Func<C, B> f) where C : A
        {
            return new MatcherN<A, B>(this, x => x is C && p((C) x), x => f((C) x));
        }
       
        public MatcherN<A, B> Case<C>(Func<C, bool> p, Func<B> f) where C : A
        {
            return new MatcherN<A, B>(this, x => x is C && p((C) x), _ => f());
        }
       
        public MatcherN<A, B> Case<C>(Func<C, bool> p, B f) where C : A
        {
            return new MatcherN<A, B>(this, x => x is C && p((C) x), _ => f);
        }
       
        public MatcherN<A, B> Case<C>(Func<bool> p, Func<C, B> f) where C : A
        {
            return new MatcherN<A, B>(this, _ => p(), x => f((C) x));
        }
       
        public MatcherN<A, B> Case<C>(Func<bool> p, Func<B> f) where C : A
        {
            return new MatcherN<A, B>(this, _ => p(), _ => f());
        }
       
        public MatcherN<A, B> Case<C>(Func<bool> p, B f) where C : A
        {
            return new MatcherN<A, B>(this, _ => p(), _ => f);
        }
       
        public MatcherN<A, B> Case<C>(bool p, Func<C, B> f) where C : A
        {
            return new MatcherN<A, B>(this, _ => p, x => f((C) x));
        }
       
        public MatcherN<A, B> Case<C>(bool p, Func<B> f) where C : A
        {
            return new MatcherN<A, B>(this, _ => p, _ => f());
        }
       
        public MatcherN<A, B> Case<C>(bool p, B f) where C : A
        {
            return new MatcherN<A, B>(this, _ => p, _ => f);
        }
           
    }

    
    public class MatcherD0<A, B> : IMatcher<A, B>
    {
        internal MatcherD0(Func<A, B> defaultF)
        {
            Default = defaultF;

        }

        private readonly Func<A, B> Default;

        
        public Maybe<B> Eval(A k)
        {
            return Maybe<B>.None;
        }

        public Func<A, B> End
        {
            get { return Default; }
        }
        
        public MatcherKD0<A, B> On(A k)
        {
            return new MatcherKD0<A, B>(k, Default);
        }

        
        public MatcherD1<A, B> Case(Func<A, bool> p, Func<A, B> f)
        {
            return new MatcherD1<A, B>(Default, p, f);
        }

        public MatcherD1<A, B> Case(Func<A, bool> p, Func<B> f)
        {
            return new MatcherD1<A, B>(Default, p, _ => f());
        }

        public MatcherD1<A, B> Case(Func<A, bool> p, B f)
        {
            return new MatcherD1<A, B>(Default, p, _ => f);
        }

        public MatcherD1<A, B> Case(Func<bool> p, Func<A, B> f)
        {
            return new MatcherD1<A, B>(Default, _ => p(), f);
        }

        public MatcherD1<A, B> Case(Func<bool> p, Func<B> f)
        {
            return new MatcherD1<A, B>(Default, _ => p(), _ => f());
        }

        public MatcherD1<A, B> Case(Func<bool> p, B f)
        {
            return new MatcherD1<A, B>(Default, _ => p(), _ => f);
        }

        public MatcherD1<A, B> Case(bool p, Func<A, B> f)
        {
            return new MatcherD1<A, B>(Default, _ => p, f);
        }

        public MatcherD1<A, B> Case(bool p, Func<B> f)
        {
            return new MatcherD1<A, B>(Default, _ => p, _ => f());
        }

        public MatcherD1<A, B> Case(bool p, B f)
        {
            return new MatcherD1<A, B>(Default, _ => p, _ => f);
        }

        public MatcherD1<A, B> Case(A p, Func<A, B> f)
        {
            return new MatcherD1<A, B>(Default, x => Object.Equals(x, p), f);
        }

        public MatcherD1<A, B> Case(A p, Func<B> f)
        {
            return new MatcherD1<A, B>(Default, x => Object.Equals(x, p), _ => f());
        }

        public MatcherD1<A, B> Case(A p, B f)
        {
            return new MatcherD1<A, B>(Default, x => Object.Equals(x, p), _ => f);
        }

        public MatcherD1<A, B> Case(Type p, Func<A, B> f)
        {
            return new MatcherD1<A, B>(Default, x => p.IsInstanceOfType(x), f);
        }

        public MatcherD1<A, B> Case(Type p, Func<B> f)
        {
            return new MatcherD1<A, B>(Default, x => p.IsInstanceOfType(x), _ => f());
        }

        public MatcherD1<A, B> Case(Type p, B f)
        {
            return new MatcherD1<A, B>(Default, x => p.IsInstanceOfType(x), _ => f);
        }
        
        public MatcherD1<A, B> Case<C>(Func<C, bool> p, Func<C, B> f) where C : A
        {
            return new MatcherD1<A, B>(Default, x => x is C && p((C) x), x => f((C) x));
        }
       
        public MatcherD1<A, B> Case<C>(Func<C, bool> p, Func<B> f) where C : A
        {
            return new MatcherD1<A, B>(Default, x => x is C && p((C) x), _ => f());
        }
       
        public MatcherD1<A, B> Case<C>(Func<C, bool> p, B f) where C : A
        {
            return new MatcherD1<A, B>(Default, x => x is C && p((C) x), _ => f);
        }
       
        public MatcherD1<A, B> Case<C>(Func<bool> p, Func<C, B> f) where C : A
        {
            return new MatcherD1<A, B>(Default, _ => p(), x => f((C) x));
        }
       
        public MatcherD1<A, B> Case<C>(Func<bool> p, Func<B> f) where C : A
        {
            return new MatcherD1<A, B>(Default, _ => p(), _ => f());
        }
       
        public MatcherD1<A, B> Case<C>(Func<bool> p, B f) where C : A
        {
            return new MatcherD1<A, B>(Default, _ => p(), _ => f);
        }
       
        public MatcherD1<A, B> Case<C>(bool p, Func<C, B> f) where C : A
        {
            return new MatcherD1<A, B>(Default, _ => p, x => f((C) x));
        }
       
        public MatcherD1<A, B> Case<C>(bool p, Func<B> f) where C : A
        {
            return new MatcherD1<A, B>(Default, _ => p, _ => f());
        }
       
        public MatcherD1<A, B> Case<C>(bool p, B f) where C : A
        {
            return new MatcherD1<A, B>(Default, _ => p, _ => f);
        }
           
    }

    
    public class MatcherD1<A, B> : IMatcher<A, B>
    {
        internal MatcherD1(Func<A, B> defaultF, Func<A, bool> p, Func<A, B> f)
        {
            Default = defaultF;
            Condition = p;
            Consequent = f;

        }

        private readonly Func<A, B> Default;
        private readonly Func<A, bool> Condition;
        private readonly Func<A, B> Consequent;

        
        public Maybe<B> Eval(A k)
        {
            return Maybe.If(k, Condition, Consequent);
        }

        public Func<A, B> End
        {
            get
            {
                var me = this;
                return k => me.Eval(k).OrElseEval(k, me.Default);
            }
        }
        
        public MatcherKD1<A, B> On(A k)
        {
            return new MatcherKD1<A, B>(k, Default, Condition, Consequent);
        }

        
        public MatcherDN<A, B> Case(Func<A, bool> p, Func<A, B> f)
        {
            return new MatcherDN<A, B>(this, Default, p, f);
        }

        public MatcherDN<A, B> Case(Func<A, bool> p, Func<B> f)
        {
            return new MatcherDN<A, B>(this, Default, p, _ => f());
        }

        public MatcherDN<A, B> Case(Func<A, bool> p, B f)
        {
            return new MatcherDN<A, B>(this, Default, p, _ => f);
        }

        public MatcherDN<A, B> Case(Func<bool> p, Func<A, B> f)
        {
            return new MatcherDN<A, B>(this, Default, _ => p(), f);
        }

        public MatcherDN<A, B> Case(Func<bool> p, Func<B> f)
        {
            return new MatcherDN<A, B>(this, Default, _ => p(), _ => f());
        }

        public MatcherDN<A, B> Case(Func<bool> p, B f)
        {
            return new MatcherDN<A, B>(this, Default, _ => p(), _ => f);
        }

        public MatcherDN<A, B> Case(bool p, Func<A, B> f)
        {
            return new MatcherDN<A, B>(this, Default, _ => p, f);
        }

        public MatcherDN<A, B> Case(bool p, Func<B> f)
        {
            return new MatcherDN<A, B>(this, Default, _ => p, _ => f());
        }

        public MatcherDN<A, B> Case(bool p, B f)
        {
            return new MatcherDN<A, B>(this, Default, _ => p, _ => f);
        }

        public MatcherDN<A, B> Case(A p, Func<A, B> f)
        {
            return new MatcherDN<A, B>(this, Default, x => Object.Equals(x, p), f);
        }

        public MatcherDN<A, B> Case(A p, Func<B> f)
        {
            return new MatcherDN<A, B>(this, Default, x => Object.Equals(x, p), _ => f());
        }

        public MatcherDN<A, B> Case(A p, B f)
        {
            return new MatcherDN<A, B>(this, Default, x => Object.Equals(x, p), _ => f);
        }

        public MatcherDN<A, B> Case(Type p, Func<A, B> f)
        {
            return new MatcherDN<A, B>(this, Default, x => p.IsInstanceOfType(x), f);
        }

        public MatcherDN<A, B> Case(Type p, Func<B> f)
        {
            return new MatcherDN<A, B>(this, Default, x => p.IsInstanceOfType(x), _ => f());
        }

        public MatcherDN<A, B> Case(Type p, B f)
        {
            return new MatcherDN<A, B>(this, Default, x => p.IsInstanceOfType(x), _ => f);
        }
        
        public MatcherDN<A, B> Case<C>(Func<C, bool> p, Func<C, B> f) where C : A
        {
            return new MatcherDN<A, B>(this, Default, x => x is C && p((C) x), x => f((C) x));
        }
       
        public MatcherDN<A, B> Case<C>(Func<C, bool> p, Func<B> f) where C : A
        {
            return new MatcherDN<A, B>(this, Default, x => x is C && p((C) x), _ => f());
        }
       
        public MatcherDN<A, B> Case<C>(Func<C, bool> p, B f) where C : A
        {
            return new MatcherDN<A, B>(this, Default, x => x is C && p((C) x), _ => f);
        }
       
        public MatcherDN<A, B> Case<C>(Func<bool> p, Func<C, B> f) where C : A
        {
            return new MatcherDN<A, B>(this, Default, _ => p(), x => f((C) x));
        }
       
        public MatcherDN<A, B> Case<C>(Func<bool> p, Func<B> f) where C : A
        {
            return new MatcherDN<A, B>(this, Default, _ => p(), _ => f());
        }
       
        public MatcherDN<A, B> Case<C>(Func<bool> p, B f) where C : A
        {
            return new MatcherDN<A, B>(this, Default, _ => p(), _ => f);
        }
       
        public MatcherDN<A, B> Case<C>(bool p, Func<C, B> f) where C : A
        {
            return new MatcherDN<A, B>(this, Default, _ => p, x => f((C) x));
        }
       
        public MatcherDN<A, B> Case<C>(bool p, Func<B> f) where C : A
        {
            return new MatcherDN<A, B>(this, Default, _ => p, _ => f());
        }
       
        public MatcherDN<A, B> Case<C>(bool p, B f) where C : A
        {
            return new MatcherDN<A, B>(this, Default, _ => p, _ => f);
        }
           
    }

    
    public class MatcherDN<A, B> : IMatcher<A, B>
    {
        internal MatcherDN(IMatcher<A, B> m, Func<A, B> defaultF, Func<A, bool> p, Func<A, B> f)
        {
            Previous = m;
            Default = defaultF;
            Condition = p;
            Consequent = f;

        }

        private readonly IMatcher<A, B> Previous;
        private readonly Func<A, B> Default;
        private readonly Func<A, bool> Condition;
        private readonly Func<A, B> Consequent;

        
        public Maybe<B> Eval(A k)
        {
            return Previous.Eval(k).OrIf(k, Condition, Consequent);
        }

        public Func<A, B> End
        {
            get
            {
                var me = this;
                return k => me.Eval(k).OrElseEval(k, me.Default);
            }
        }
        
        public MatcherKDN<A, B> On(A k)
        {
            return new MatcherKDN<A, B>(k, Default, this, Condition, Consequent);
        }

        
        public MatcherDN<A, B> Case(Func<A, bool> p, Func<A, B> f)
        {
            return new MatcherDN<A, B>(this, Default, p, f);
        }

        public MatcherDN<A, B> Case(Func<A, bool> p, Func<B> f)
        {
            return new MatcherDN<A, B>(this, Default, p, _ => f());
        }

        public MatcherDN<A, B> Case(Func<A, bool> p, B f)
        {
            return new MatcherDN<A, B>(this, Default, p, _ => f);
        }

        public MatcherDN<A, B> Case(Func<bool> p, Func<A, B> f)
        {
            return new MatcherDN<A, B>(this, Default, _ => p(), f);
        }

        public MatcherDN<A, B> Case(Func<bool> p, Func<B> f)
        {
            return new MatcherDN<A, B>(this, Default, _ => p(), _ => f());
        }

        public MatcherDN<A, B> Case(Func<bool> p, B f)
        {
            return new MatcherDN<A, B>(this, Default, _ => p(), _ => f);
        }

        public MatcherDN<A, B> Case(bool p, Func<A, B> f)
        {
            return new MatcherDN<A, B>(this, Default, _ => p, f);
        }

        public MatcherDN<A, B> Case(bool p, Func<B> f)
        {
            return new MatcherDN<A, B>(this, Default, _ => p, _ => f());
        }

        public MatcherDN<A, B> Case(bool p, B f)
        {
            return new MatcherDN<A, B>(this, Default, _ => p, _ => f);
        }

        public MatcherDN<A, B> Case(A p, Func<A, B> f)
        {
            return new MatcherDN<A, B>(this, Default, x => Object.Equals(x, p), f);
        }

        public MatcherDN<A, B> Case(A p, Func<B> f)
        {
            return new MatcherDN<A, B>(this, Default, x => Object.Equals(x, p), _ => f());
        }

        public MatcherDN<A, B> Case(A p, B f)
        {
            return new MatcherDN<A, B>(this, Default, x => Object.Equals(x, p), _ => f);
        }

        public MatcherDN<A, B> Case(Type p, Func<A, B> f)
        {
            return new MatcherDN<A, B>(this, Default, x => p.IsInstanceOfType(x), f);
        }

        public MatcherDN<A, B> Case(Type p, Func<B> f)
        {
            return new MatcherDN<A, B>(this, Default, x => p.IsInstanceOfType(x), _ => f());
        }

        public MatcherDN<A, B> Case(Type p, B f)
        {
            return new MatcherDN<A, B>(this, Default, x => p.IsInstanceOfType(x), _ => f);
        }
        
        public MatcherDN<A, B> Case<C>(Func<C, bool> p, Func<C, B> f) where C : A
        {
            return new MatcherDN<A, B>(this, Default, x => x is C && p((C) x), x => f((C) x));
        }
       
        public MatcherDN<A, B> Case<C>(Func<C, bool> p, Func<B> f) where C : A
        {
            return new MatcherDN<A, B>(this, Default, x => x is C && p((C) x), _ => f());
        }
       
        public MatcherDN<A, B> Case<C>(Func<C, bool> p, B f) where C : A
        {
            return new MatcherDN<A, B>(this, Default, x => x is C && p((C) x), _ => f);
        }
       
        public MatcherDN<A, B> Case<C>(Func<bool> p, Func<C, B> f) where C : A
        {
            return new MatcherDN<A, B>(this, Default, _ => p(), x => f((C) x));
        }
       
        public MatcherDN<A, B> Case<C>(Func<bool> p, Func<B> f) where C : A
        {
            return new MatcherDN<A, B>(this, Default, _ => p(), _ => f());
        }
       
        public MatcherDN<A, B> Case<C>(Func<bool> p, B f) where C : A
        {
            return new MatcherDN<A, B>(this, Default, _ => p(), _ => f);
        }
       
        public MatcherDN<A, B> Case<C>(bool p, Func<C, B> f) where C : A
        {
            return new MatcherDN<A, B>(this, Default, _ => p, x => f((C) x));
        }
       
        public MatcherDN<A, B> Case<C>(bool p, Func<B> f) where C : A
        {
            return new MatcherDN<A, B>(this, Default, _ => p, _ => f());
        }
       
        public MatcherDN<A, B> Case<C>(bool p, B f) where C : A
        {
            return new MatcherDN<A, B>(this, Default, _ => p, _ => f);
        }
           
    }

    public class MatcherK0<A>
    {
        internal MatcherK0(A k)
        {
            Key = k;
        }

        private readonly A Key;

        public MatcherK0<A, B> Return<B>()
        {
            return new MatcherK0<A, B>(Key);
        }
        
        public MatcherKD0<A, B> Default<B>(Func<A, B> defaultF)
        {
            return new MatcherKD0<A, B>(Key, defaultF);
        }

        public MatcherKD0<A, B> Default<B>(Func<B> defaultF)
        {
            return new MatcherKD0<A, B>(Key, _ => defaultF());
        }

        public MatcherKD0<A, B> Default<B>(B defaultF)
        {
            return new MatcherKD0<A, B>(Key, _ => defaultF);
        }
        
        public MatcherK1<A, B> Case<B>(Func<A, bool> p, Func<A, B> f)
        {
            return new MatcherK1<A, B>(Key, p, f);
        }

        public MatcherK1<A, B> Case<B>(Func<A, bool> p, Func<B> f)
        {
            return new MatcherK1<A, B>(Key, p, _ => f());
        }

        public MatcherK1<A, B> Case<B>(Func<A, bool> p, B f)
        {
            return new MatcherK1<A, B>(Key, p, _ => f);
        }

        public MatcherK1<A, B> Case<B>(Func<bool> p, Func<A, B> f)
        {
            return new MatcherK1<A, B>(Key, _ => p(), f);
        }

        public MatcherK1<A, B> Case<B>(Func<bool> p, Func<B> f)
        {
            return new MatcherK1<A, B>(Key, _ => p(), _ => f());
        }

        public MatcherK1<A, B> Case<B>(Func<bool> p, B f)
        {
            return new MatcherK1<A, B>(Key, _ => p(), _ => f);
        }

        public MatcherK1<A, B> Case<B>(bool p, Func<A, B> f)
        {
            return new MatcherK1<A, B>(Key, _ => p, f);
        }

        public MatcherK1<A, B> Case<B>(bool p, Func<B> f)
        {
            return new MatcherK1<A, B>(Key, _ => p, _ => f());
        }

        public MatcherK1<A, B> Case<B>(bool p, B f)
        {
            return new MatcherK1<A, B>(Key, _ => p, _ => f);
        }

        public MatcherK1<A, B> Case<B>(A p, Func<A, B> f)
        {
            return new MatcherK1<A, B>(Key, x => Object.Equals(x, p), f);
        }

        public MatcherK1<A, B> Case<B>(A p, Func<B> f)
        {
            return new MatcherK1<A, B>(Key, x => Object.Equals(x, p), _ => f());
        }

        public MatcherK1<A, B> Case<B>(A p, B f)
        {
            return new MatcherK1<A, B>(Key, x => Object.Equals(x, p), _ => f);
        }

        public MatcherK1<A, B> Case<B>(Type p, Func<A, B> f)
        {
            return new MatcherK1<A, B>(Key, x => p.IsInstanceOfType(x), f);
        }

        public MatcherK1<A, B> Case<B>(Type p, Func<B> f)
        {
            return new MatcherK1<A, B>(Key, x => p.IsInstanceOfType(x), _ => f());
        }

        public MatcherK1<A, B> Case<B>(Type p, B f)
        {
            return new MatcherK1<A, B>(Key, x => p.IsInstanceOfType(x), _ => f);
        }

    }

    
    public class MatcherK0<A, B> : IMatcher<A, B>
    {
        internal MatcherK0(A k)
        {
            Key = k;

        }

        private readonly A Key;

        
        public Maybe<B> Eval(A k)
        {
            return Maybe<B>.None;
        }

        public Maybe<B> End
        {
            get { return Eval(Key); }
        }
        
        public B Else(Func<A, B> f)
        {
            
            return End.OrElseEval(Key, f);
        }


        public B Else(Func<B> f)
        {
            
            return End.OrElseEval(f);
        }


        public B Else(B f)
        {
            
            return End.OrElse(f);
        }

        
        public MatcherKD0<A, B> Default(Func<A, B> defaultF)
        {
            return new MatcherKD0<A, B>(Key, defaultF);
        }

        public MatcherKD0<A, B> Default(Func<B> defaultF)
        {
            return new MatcherKD0<A, B>(Key, _ => defaultF());
        }

        public MatcherKD0<A, B> Default(B defaultF)
        {
            return new MatcherKD0<A, B>(Key, _ => defaultF);
        }
        
        public MatcherK1<A, B> Case(Func<A, bool> p, Func<A, B> f)
        {
            return new MatcherK1<A, B>(Key, p, f);
        }

        public MatcherK1<A, B> Case(Func<A, bool> p, Func<B> f)
        {
            return new MatcherK1<A, B>(Key, p, _ => f());
        }

        public MatcherK1<A, B> Case(Func<A, bool> p, B f)
        {
            return new MatcherK1<A, B>(Key, p, _ => f);
        }

        public MatcherK1<A, B> Case(Func<bool> p, Func<A, B> f)
        {
            return new MatcherK1<A, B>(Key, _ => p(), f);
        }

        public MatcherK1<A, B> Case(Func<bool> p, Func<B> f)
        {
            return new MatcherK1<A, B>(Key, _ => p(), _ => f());
        }

        public MatcherK1<A, B> Case(Func<bool> p, B f)
        {
            return new MatcherK1<A, B>(Key, _ => p(), _ => f);
        }

        public MatcherK1<A, B> Case(bool p, Func<A, B> f)
        {
            return new MatcherK1<A, B>(Key, _ => p, f);
        }

        public MatcherK1<A, B> Case(bool p, Func<B> f)
        {
            return new MatcherK1<A, B>(Key, _ => p, _ => f());
        }

        public MatcherK1<A, B> Case(bool p, B f)
        {
            return new MatcherK1<A, B>(Key, _ => p, _ => f);
        }

        public MatcherK1<A, B> Case(A p, Func<A, B> f)
        {
            return new MatcherK1<A, B>(Key, x => Object.Equals(x, p), f);
        }

        public MatcherK1<A, B> Case(A p, Func<B> f)
        {
            return new MatcherK1<A, B>(Key, x => Object.Equals(x, p), _ => f());
        }

        public MatcherK1<A, B> Case(A p, B f)
        {
            return new MatcherK1<A, B>(Key, x => Object.Equals(x, p), _ => f);
        }

        public MatcherK1<A, B> Case(Type p, Func<A, B> f)
        {
            return new MatcherK1<A, B>(Key, x => p.IsInstanceOfType(x), f);
        }

        public MatcherK1<A, B> Case(Type p, Func<B> f)
        {
            return new MatcherK1<A, B>(Key, x => p.IsInstanceOfType(x), _ => f());
        }

        public MatcherK1<A, B> Case(Type p, B f)
        {
            return new MatcherK1<A, B>(Key, x => p.IsInstanceOfType(x), _ => f);
        }
        
        public MatcherK1<A, B> Case<C>(Func<C, bool> p, Func<C, B> f) where C : A
        {
            return new MatcherK1<A, B>(Key, x => x is C && p((C) x), x => f((C) x));
        }
       
        public MatcherK1<A, B> Case<C>(Func<C, bool> p, Func<B> f) where C : A
        {
            return new MatcherK1<A, B>(Key, x => x is C && p((C) x), _ => f());
        }
       
        public MatcherK1<A, B> Case<C>(Func<C, bool> p, B f) where C : A
        {
            return new MatcherK1<A, B>(Key, x => x is C && p((C) x), _ => f);
        }
       
        public MatcherK1<A, B> Case<C>(Func<bool> p, Func<C, B> f) where C : A
        {
            return new MatcherK1<A, B>(Key, _ => p(), x => f((C) x));
        }
       
        public MatcherK1<A, B> Case<C>(Func<bool> p, Func<B> f) where C : A
        {
            return new MatcherK1<A, B>(Key, _ => p(), _ => f());
        }
       
        public MatcherK1<A, B> Case<C>(Func<bool> p, B f) where C : A
        {
            return new MatcherK1<A, B>(Key, _ => p(), _ => f);
        }
       
        public MatcherK1<A, B> Case<C>(bool p, Func<C, B> f) where C : A
        {
            return new MatcherK1<A, B>(Key, _ => p, x => f((C) x));
        }
       
        public MatcherK1<A, B> Case<C>(bool p, Func<B> f) where C : A
        {
            return new MatcherK1<A, B>(Key, _ => p, _ => f());
        }
       
        public MatcherK1<A, B> Case<C>(bool p, B f) where C : A
        {
            return new MatcherK1<A, B>(Key, _ => p, _ => f);
        }
           
    }

    
    public class MatcherK1<A, B> : IMatcher<A, B>
    {
        internal MatcherK1(A k, Func<A, bool> p, Func<A, B> f)
        {
            Key = k;
            Condition = p;
            Consequent = f;

        }

        private readonly A Key;
        private readonly Func<A, bool> Condition;
        private readonly Func<A, B> Consequent;

        
        public Maybe<B> Eval(A k)
        {
            return Maybe.If(Key, Condition, Consequent);
        }

        public Maybe<B> End
        {
            get { return Eval(Key); }
        }
        
        public B Else(Func<A, B> f)
        {
            
            return End.OrElseEval(Key, f);
        }


        public B Else(Func<B> f)
        {
            
            return End.OrElseEval(f);
        }


        public B Else(B f)
        {
            
            return End.OrElse(f);
        }

        
        public MatcherKD1<A, B> Default(Func<A, B> defaultF)
        {
            return new MatcherKD1<A, B>(Key, defaultF, Condition, Consequent);
        }

        public MatcherKD1<A, B> Default(Func<B> defaultF)
        {
            return new MatcherKD1<A, B>(Key, _ => defaultF(), Condition, Consequent);
        }

        public MatcherKD1<A, B> Default(B defaultF)
        {
            return new MatcherKD1<A, B>(Key, _ => defaultF, Condition, Consequent);
        }
        
        public MatcherKN<A, B> Case(Func<A, bool> p, Func<A, B> f)
        {
            return new MatcherKN<A, B>(Key, this, p, f);
        }

        public MatcherKN<A, B> Case(Func<A, bool> p, Func<B> f)
        {
            return new MatcherKN<A, B>(Key, this, p, _ => f());
        }

        public MatcherKN<A, B> Case(Func<A, bool> p, B f)
        {
            return new MatcherKN<A, B>(Key, this, p, _ => f);
        }

        public MatcherKN<A, B> Case(Func<bool> p, Func<A, B> f)
        {
            return new MatcherKN<A, B>(Key, this, _ => p(), f);
        }

        public MatcherKN<A, B> Case(Func<bool> p, Func<B> f)
        {
            return new MatcherKN<A, B>(Key, this, _ => p(), _ => f());
        }

        public MatcherKN<A, B> Case(Func<bool> p, B f)
        {
            return new MatcherKN<A, B>(Key, this, _ => p(), _ => f);
        }

        public MatcherKN<A, B> Case(bool p, Func<A, B> f)
        {
            return new MatcherKN<A, B>(Key, this, _ => p, f);
        }

        public MatcherKN<A, B> Case(bool p, Func<B> f)
        {
            return new MatcherKN<A, B>(Key, this, _ => p, _ => f());
        }

        public MatcherKN<A, B> Case(bool p, B f)
        {
            return new MatcherKN<A, B>(Key, this, _ => p, _ => f);
        }

        public MatcherKN<A, B> Case(A p, Func<A, B> f)
        {
            return new MatcherKN<A, B>(Key, this, x => Object.Equals(x, p), f);
        }

        public MatcherKN<A, B> Case(A p, Func<B> f)
        {
            return new MatcherKN<A, B>(Key, this, x => Object.Equals(x, p), _ => f());
        }

        public MatcherKN<A, B> Case(A p, B f)
        {
            return new MatcherKN<A, B>(Key, this, x => Object.Equals(x, p), _ => f);
        }

        public MatcherKN<A, B> Case(Type p, Func<A, B> f)
        {
            return new MatcherKN<A, B>(Key, this, x => p.IsInstanceOfType(x), f);
        }

        public MatcherKN<A, B> Case(Type p, Func<B> f)
        {
            return new MatcherKN<A, B>(Key, this, x => p.IsInstanceOfType(x), _ => f());
        }

        public MatcherKN<A, B> Case(Type p, B f)
        {
            return new MatcherKN<A, B>(Key, this, x => p.IsInstanceOfType(x), _ => f);
        }
        
        public MatcherKN<A, B> Case<C>(Func<C, bool> p, Func<C, B> f) where C : A
        {
            return new MatcherKN<A, B>(Key, this, x => x is C && p((C) x), x => f((C) x));
        }
       
        public MatcherKN<A, B> Case<C>(Func<C, bool> p, Func<B> f) where C : A
        {
            return new MatcherKN<A, B>(Key, this, x => x is C && p((C) x), _ => f());
        }
       
        public MatcherKN<A, B> Case<C>(Func<C, bool> p, B f) where C : A
        {
            return new MatcherKN<A, B>(Key, this, x => x is C && p((C) x), _ => f);
        }
       
        public MatcherKN<A, B> Case<C>(Func<bool> p, Func<C, B> f) where C : A
        {
            return new MatcherKN<A, B>(Key, this, _ => p(), x => f((C) x));
        }
       
        public MatcherKN<A, B> Case<C>(Func<bool> p, Func<B> f) where C : A
        {
            return new MatcherKN<A, B>(Key, this, _ => p(), _ => f());
        }
       
        public MatcherKN<A, B> Case<C>(Func<bool> p, B f) where C : A
        {
            return new MatcherKN<A, B>(Key, this, _ => p(), _ => f);
        }
       
        public MatcherKN<A, B> Case<C>(bool p, Func<C, B> f) where C : A
        {
            return new MatcherKN<A, B>(Key, this, _ => p, x => f((C) x));
        }
       
        public MatcherKN<A, B> Case<C>(bool p, Func<B> f) where C : A
        {
            return new MatcherKN<A, B>(Key, this, _ => p, _ => f());
        }
       
        public MatcherKN<A, B> Case<C>(bool p, B f) where C : A
        {
            return new MatcherKN<A, B>(Key, this, _ => p, _ => f);
        }
           
    }

    
    public class MatcherKN<A, B> : IMatcher<A, B>
    {
        internal MatcherKN(A k, IMatcher<A, B> m, Func<A, bool> p, Func<A, B> f)
        {
            Key = k;
            Previous = m;
            Condition = p;
            Consequent = f;

        }

        private readonly A Key;
        private readonly IMatcher<A, B> Previous;
        private readonly Func<A, bool> Condition;
        private readonly Func<A, B> Consequent;

        
        public Maybe<B> Eval(A k)
        {
            return Previous.Eval(Key).OrIf(Key, Condition, Consequent);
        }

        public Maybe<B> End
        {
            get { return Eval(Key); }
        }
        
        public B Else(Func<A, B> f)
        {
            
            return End.OrElseEval(Key, f);
        }


        public B Else(Func<B> f)
        {
            
            return End.OrElseEval(f);
        }


        public B Else(B f)
        {
            
            return End.OrElse(f);
        }

        
        public MatcherKDN<A, B> Default(Func<A, B> defaultF)
        {
            return new MatcherKDN<A, B>(Key, defaultF, this, Condition, Consequent);
        }

        public MatcherKDN<A, B> Default(Func<B> defaultF)
        {
            return new MatcherKDN<A, B>(Key, _ => defaultF(), this, Condition, Consequent);
        }

        public MatcherKDN<A, B> Default(B defaultF)
        {
            return new MatcherKDN<A, B>(Key, _ => defaultF, this, Condition, Consequent);
        }
        
        public MatcherKN<A, B> Case(Func<A, bool> p, Func<A, B> f)
        {
            return new MatcherKN<A, B>(Key, this, p, f);
        }

        public MatcherKN<A, B> Case(Func<A, bool> p, Func<B> f)
        {
            return new MatcherKN<A, B>(Key, this, p, _ => f());
        }

        public MatcherKN<A, B> Case(Func<A, bool> p, B f)
        {
            return new MatcherKN<A, B>(Key, this, p, _ => f);
        }

        public MatcherKN<A, B> Case(Func<bool> p, Func<A, B> f)
        {
            return new MatcherKN<A, B>(Key, this, _ => p(), f);
        }

        public MatcherKN<A, B> Case(Func<bool> p, Func<B> f)
        {
            return new MatcherKN<A, B>(Key, this, _ => p(), _ => f());
        }

        public MatcherKN<A, B> Case(Func<bool> p, B f)
        {
            return new MatcherKN<A, B>(Key, this, _ => p(), _ => f);
        }

        public MatcherKN<A, B> Case(bool p, Func<A, B> f)
        {
            return new MatcherKN<A, B>(Key, this, _ => p, f);
        }

        public MatcherKN<A, B> Case(bool p, Func<B> f)
        {
            return new MatcherKN<A, B>(Key, this, _ => p, _ => f());
        }

        public MatcherKN<A, B> Case(bool p, B f)
        {
            return new MatcherKN<A, B>(Key, this, _ => p, _ => f);
        }

        public MatcherKN<A, B> Case(A p, Func<A, B> f)
        {
            return new MatcherKN<A, B>(Key, this, x => Object.Equals(x, p), f);
        }

        public MatcherKN<A, B> Case(A p, Func<B> f)
        {
            return new MatcherKN<A, B>(Key, this, x => Object.Equals(x, p), _ => f());
        }

        public MatcherKN<A, B> Case(A p, B f)
        {
            return new MatcherKN<A, B>(Key, this, x => Object.Equals(x, p), _ => f);
        }

        public MatcherKN<A, B> Case(Type p, Func<A, B> f)
        {
            return new MatcherKN<A, B>(Key, this, x => p.IsInstanceOfType(x), f);
        }

        public MatcherKN<A, B> Case(Type p, Func<B> f)
        {
            return new MatcherKN<A, B>(Key, this, x => p.IsInstanceOfType(x), _ => f());
        }

        public MatcherKN<A, B> Case(Type p, B f)
        {
            return new MatcherKN<A, B>(Key, this, x => p.IsInstanceOfType(x), _ => f);
        }
        
        public MatcherKN<A, B> Case<C>(Func<C, bool> p, Func<C, B> f) where C : A
        {
            return new MatcherKN<A, B>(Key, this, x => x is C && p((C) x), x => f((C) x));
        }
       
        public MatcherKN<A, B> Case<C>(Func<C, bool> p, Func<B> f) where C : A
        {
            return new MatcherKN<A, B>(Key, this, x => x is C && p((C) x), _ => f());
        }
       
        public MatcherKN<A, B> Case<C>(Func<C, bool> p, B f) where C : A
        {
            return new MatcherKN<A, B>(Key, this, x => x is C && p((C) x), _ => f);
        }
       
        public MatcherKN<A, B> Case<C>(Func<bool> p, Func<C, B> f) where C : A
        {
            return new MatcherKN<A, B>(Key, this, _ => p(), x => f((C) x));
        }
       
        public MatcherKN<A, B> Case<C>(Func<bool> p, Func<B> f) where C : A
        {
            return new MatcherKN<A, B>(Key, this, _ => p(), _ => f());
        }
       
        public MatcherKN<A, B> Case<C>(Func<bool> p, B f) where C : A
        {
            return new MatcherKN<A, B>(Key, this, _ => p(), _ => f);
        }
       
        public MatcherKN<A, B> Case<C>(bool p, Func<C, B> f) where C : A
        {
            return new MatcherKN<A, B>(Key, this, _ => p, x => f((C) x));
        }
       
        public MatcherKN<A, B> Case<C>(bool p, Func<B> f) where C : A
        {
            return new MatcherKN<A, B>(Key, this, _ => p, _ => f());
        }
       
        public MatcherKN<A, B> Case<C>(bool p, B f) where C : A
        {
            return new MatcherKN<A, B>(Key, this, _ => p, _ => f);
        }
           
    }

    
    public class MatcherKD0<A, B> : IMatcher<A, B>
    {
        internal MatcherKD0(A k, Func<A, B> defaultF)
        {
            Key = k;
            Default = defaultF;

        }

        private readonly A Key;
        private readonly Func<A, B> Default;

        
        public Maybe<B> Eval(A k)
        {
            return Maybe<B>.None;
        }

        public B End
        {
            get { return Default(Key); }
        }
        
        public MatcherKD1<A, B> Case(Func<A, bool> p, Func<A, B> f)
        {
            return new MatcherKD1<A, B>(Key, Default, p, f);
        }

        public MatcherKD1<A, B> Case(Func<A, bool> p, Func<B> f)
        {
            return new MatcherKD1<A, B>(Key, Default, p, _ => f());
        }

        public MatcherKD1<A, B> Case(Func<A, bool> p, B f)
        {
            return new MatcherKD1<A, B>(Key, Default, p, _ => f);
        }

        public MatcherKD1<A, B> Case(Func<bool> p, Func<A, B> f)
        {
            return new MatcherKD1<A, B>(Key, Default, _ => p(), f);
        }

        public MatcherKD1<A, B> Case(Func<bool> p, Func<B> f)
        {
            return new MatcherKD1<A, B>(Key, Default, _ => p(), _ => f());
        }

        public MatcherKD1<A, B> Case(Func<bool> p, B f)
        {
            return new MatcherKD1<A, B>(Key, Default, _ => p(), _ => f);
        }

        public MatcherKD1<A, B> Case(bool p, Func<A, B> f)
        {
            return new MatcherKD1<A, B>(Key, Default, _ => p, f);
        }

        public MatcherKD1<A, B> Case(bool p, Func<B> f)
        {
            return new MatcherKD1<A, B>(Key, Default, _ => p, _ => f());
        }

        public MatcherKD1<A, B> Case(bool p, B f)
        {
            return new MatcherKD1<A, B>(Key, Default, _ => p, _ => f);
        }

        public MatcherKD1<A, B> Case(A p, Func<A, B> f)
        {
            return new MatcherKD1<A, B>(Key, Default, x => Object.Equals(x, p), f);
        }

        public MatcherKD1<A, B> Case(A p, Func<B> f)
        {
            return new MatcherKD1<A, B>(Key, Default, x => Object.Equals(x, p), _ => f());
        }

        public MatcherKD1<A, B> Case(A p, B f)
        {
            return new MatcherKD1<A, B>(Key, Default, x => Object.Equals(x, p), _ => f);
        }

        public MatcherKD1<A, B> Case(Type p, Func<A, B> f)
        {
            return new MatcherKD1<A, B>(Key, Default, x => p.IsInstanceOfType(x), f);
        }

        public MatcherKD1<A, B> Case(Type p, Func<B> f)
        {
            return new MatcherKD1<A, B>(Key, Default, x => p.IsInstanceOfType(x), _ => f());
        }

        public MatcherKD1<A, B> Case(Type p, B f)
        {
            return new MatcherKD1<A, B>(Key, Default, x => p.IsInstanceOfType(x), _ => f);
        }
        
        public MatcherKD1<A, B> Case<C>(Func<C, bool> p, Func<C, B> f) where C : A
        {
            return new MatcherKD1<A, B>(Key, Default, x => x is C && p((C) x), x => f((C) x));
        }
       
        public MatcherKD1<A, B> Case<C>(Func<C, bool> p, Func<B> f) where C : A
        {
            return new MatcherKD1<A, B>(Key, Default, x => x is C && p((C) x), _ => f());
        }
       
        public MatcherKD1<A, B> Case<C>(Func<C, bool> p, B f) where C : A
        {
            return new MatcherKD1<A, B>(Key, Default, x => x is C && p((C) x), _ => f);
        }
       
        public MatcherKD1<A, B> Case<C>(Func<bool> p, Func<C, B> f) where C : A
        {
            return new MatcherKD1<A, B>(Key, Default, _ => p(), x => f((C) x));
        }
       
        public MatcherKD1<A, B> Case<C>(Func<bool> p, Func<B> f) where C : A
        {
            return new MatcherKD1<A, B>(Key, Default, _ => p(), _ => f());
        }
       
        public MatcherKD1<A, B> Case<C>(Func<bool> p, B f) where C : A
        {
            return new MatcherKD1<A, B>(Key, Default, _ => p(), _ => f);
        }
       
        public MatcherKD1<A, B> Case<C>(bool p, Func<C, B> f) where C : A
        {
            return new MatcherKD1<A, B>(Key, Default, _ => p, x => f((C) x));
        }
       
        public MatcherKD1<A, B> Case<C>(bool p, Func<B> f) where C : A
        {
            return new MatcherKD1<A, B>(Key, Default, _ => p, _ => f());
        }
       
        public MatcherKD1<A, B> Case<C>(bool p, B f) where C : A
        {
            return new MatcherKD1<A, B>(Key, Default, _ => p, _ => f);
        }
           
    }

    
    public class MatcherKD1<A, B> : IMatcher<A, B>
    {
        internal MatcherKD1(A k, Func<A, B> defaultF, Func<A, bool> p, Func<A, B> f)
        {
            Key = k;
            Default = defaultF;
            Condition = p;
            Consequent = f;

        }

        private readonly A Key;
        private readonly Func<A, B> Default;
        private readonly Func<A, bool> Condition;
        private readonly Func<A, B> Consequent;

        
        public Maybe<B> Eval(A k)
        {
            return Maybe.If(Key, Condition, Consequent);
        }

        public B End
        {
            get { return Eval(Key).OrElseEval(Key, Default); }
        }
        
        public MatcherKDN<A, B> Case(Func<A, bool> p, Func<A, B> f)
        {
            return new MatcherKDN<A, B>(Key, Default, this, p, f);
        }

        public MatcherKDN<A, B> Case(Func<A, bool> p, Func<B> f)
        {
            return new MatcherKDN<A, B>(Key, Default, this, p, _ => f());
        }

        public MatcherKDN<A, B> Case(Func<A, bool> p, B f)
        {
            return new MatcherKDN<A, B>(Key, Default, this, p, _ => f);
        }

        public MatcherKDN<A, B> Case(Func<bool> p, Func<A, B> f)
        {
            return new MatcherKDN<A, B>(Key, Default, this, _ => p(), f);
        }

        public MatcherKDN<A, B> Case(Func<bool> p, Func<B> f)
        {
            return new MatcherKDN<A, B>(Key, Default, this, _ => p(), _ => f());
        }

        public MatcherKDN<A, B> Case(Func<bool> p, B f)
        {
            return new MatcherKDN<A, B>(Key, Default, this, _ => p(), _ => f);
        }

        public MatcherKDN<A, B> Case(bool p, Func<A, B> f)
        {
            return new MatcherKDN<A, B>(Key, Default, this, _ => p, f);
        }

        public MatcherKDN<A, B> Case(bool p, Func<B> f)
        {
            return new MatcherKDN<A, B>(Key, Default, this, _ => p, _ => f());
        }

        public MatcherKDN<A, B> Case(bool p, B f)
        {
            return new MatcherKDN<A, B>(Key, Default, this, _ => p, _ => f);
        }

        public MatcherKDN<A, B> Case(A p, Func<A, B> f)
        {
            return new MatcherKDN<A, B>(Key, Default, this, x => Object.Equals(x, p), f);
        }

        public MatcherKDN<A, B> Case(A p, Func<B> f)
        {
            return new MatcherKDN<A, B>(Key, Default, this, x => Object.Equals(x, p), _ => f());
        }

        public MatcherKDN<A, B> Case(A p, B f)
        {
            return new MatcherKDN<A, B>(Key, Default, this, x => Object.Equals(x, p), _ => f);
        }

        public MatcherKDN<A, B> Case(Type p, Func<A, B> f)
        {
            return new MatcherKDN<A, B>(Key, Default, this, x => p.IsInstanceOfType(x), f);
        }

        public MatcherKDN<A, B> Case(Type p, Func<B> f)
        {
            return new MatcherKDN<A, B>(Key, Default, this, x => p.IsInstanceOfType(x), _ => f());
        }

        public MatcherKDN<A, B> Case(Type p, B f)
        {
            return new MatcherKDN<A, B>(Key, Default, this, x => p.IsInstanceOfType(x), _ => f);
        }
        
        public MatcherKDN<A, B> Case<C>(Func<C, bool> p, Func<C, B> f) where C : A
        {
            return new MatcherKDN<A, B>(Key, Default, this, x => x is C && p((C) x), x => f((C) x));
        }
       
        public MatcherKDN<A, B> Case<C>(Func<C, bool> p, Func<B> f) where C : A
        {
            return new MatcherKDN<A, B>(Key, Default, this, x => x is C && p((C) x), _ => f());
        }
       
        public MatcherKDN<A, B> Case<C>(Func<C, bool> p, B f) where C : A
        {
            return new MatcherKDN<A, B>(Key, Default, this, x => x is C && p((C) x), _ => f);
        }
       
        public MatcherKDN<A, B> Case<C>(Func<bool> p, Func<C, B> f) where C : A
        {
            return new MatcherKDN<A, B>(Key, Default, this, _ => p(), x => f((C) x));
        }
       
        public MatcherKDN<A, B> Case<C>(Func<bool> p, Func<B> f) where C : A
        {
            return new MatcherKDN<A, B>(Key, Default, this, _ => p(), _ => f());
        }
       
        public MatcherKDN<A, B> Case<C>(Func<bool> p, B f) where C : A
        {
            return new MatcherKDN<A, B>(Key, Default, this, _ => p(), _ => f);
        }
       
        public MatcherKDN<A, B> Case<C>(bool p, Func<C, B> f) where C : A
        {
            return new MatcherKDN<A, B>(Key, Default, this, _ => p, x => f((C) x));
        }
       
        public MatcherKDN<A, B> Case<C>(bool p, Func<B> f) where C : A
        {
            return new MatcherKDN<A, B>(Key, Default, this, _ => p, _ => f());
        }
       
        public MatcherKDN<A, B> Case<C>(bool p, B f) where C : A
        {
            return new MatcherKDN<A, B>(Key, Default, this, _ => p, _ => f);
        }
           
    }

    
    public class MatcherKDN<A, B> : IMatcher<A, B>
    {
        internal MatcherKDN(A k, Func<A, B> defaultF, IMatcher<A, B> m, Func<A, bool> p, Func<A, B> f)
        {
            Key = k;
            Default = defaultF;
            Previous = m;
            Condition = p;
            Consequent = f;

        }

        private readonly A Key;
        private readonly Func<A, B> Default;
        private readonly IMatcher<A, B> Previous;
        private readonly Func<A, bool> Condition;
        private readonly Func<A, B> Consequent;

        
        public Maybe<B> Eval(A k)
        {
            return Previous.Eval(Key).OrIf(Key, Condition, Consequent);
        }

        public B End
        {
            get { return Eval(Key).OrElseEval(Key, Default); }
        }
        
        public MatcherKDN<A, B> Case(Func<A, bool> p, Func<A, B> f)
        {
            return new MatcherKDN<A, B>(Key, Default, this, p, f);
        }

        public MatcherKDN<A, B> Case(Func<A, bool> p, Func<B> f)
        {
            return new MatcherKDN<A, B>(Key, Default, this, p, _ => f());
        }

        public MatcherKDN<A, B> Case(Func<A, bool> p, B f)
        {
            return new MatcherKDN<A, B>(Key, Default, this, p, _ => f);
        }

        public MatcherKDN<A, B> Case(Func<bool> p, Func<A, B> f)
        {
            return new MatcherKDN<A, B>(Key, Default, this, _ => p(), f);
        }

        public MatcherKDN<A, B> Case(Func<bool> p, Func<B> f)
        {
            return new MatcherKDN<A, B>(Key, Default, this, _ => p(), _ => f());
        }

        public MatcherKDN<A, B> Case(Func<bool> p, B f)
        {
            return new MatcherKDN<A, B>(Key, Default, this, _ => p(), _ => f);
        }

        public MatcherKDN<A, B> Case(bool p, Func<A, B> f)
        {
            return new MatcherKDN<A, B>(Key, Default, this, _ => p, f);
        }

        public MatcherKDN<A, B> Case(bool p, Func<B> f)
        {
            return new MatcherKDN<A, B>(Key, Default, this, _ => p, _ => f());
        }

        public MatcherKDN<A, B> Case(bool p, B f)
        {
            return new MatcherKDN<A, B>(Key, Default, this, _ => p, _ => f);
        }

        public MatcherKDN<A, B> Case(A p, Func<A, B> f)
        {
            return new MatcherKDN<A, B>(Key, Default, this, x => Object.Equals(x, p), f);
        }

        public MatcherKDN<A, B> Case(A p, Func<B> f)
        {
            return new MatcherKDN<A, B>(Key, Default, this, x => Object.Equals(x, p), _ => f());
        }

        public MatcherKDN<A, B> Case(A p, B f)
        {
            return new MatcherKDN<A, B>(Key, Default, this, x => Object.Equals(x, p), _ => f);
        }

        public MatcherKDN<A, B> Case(Type p, Func<A, B> f)
        {
            return new MatcherKDN<A, B>(Key, Default, this, x => p.IsInstanceOfType(x), f);
        }

        public MatcherKDN<A, B> Case(Type p, Func<B> f)
        {
            return new MatcherKDN<A, B>(Key, Default, this, x => p.IsInstanceOfType(x), _ => f());
        }

        public MatcherKDN<A, B> Case(Type p, B f)
        {
            return new MatcherKDN<A, B>(Key, Default, this, x => p.IsInstanceOfType(x), _ => f);
        }
        
        public MatcherKDN<A, B> Case<C>(Func<C, bool> p, Func<C, B> f) where C : A
        {
            return new MatcherKDN<A, B>(Key, Default, this, x => x is C && p((C) x), x => f((C) x));
        }
       
        public MatcherKDN<A, B> Case<C>(Func<C, bool> p, Func<B> f) where C : A
        {
            return new MatcherKDN<A, B>(Key, Default, this, x => x is C && p((C) x), _ => f());
        }
       
        public MatcherKDN<A, B> Case<C>(Func<C, bool> p, B f) where C : A
        {
            return new MatcherKDN<A, B>(Key, Default, this, x => x is C && p((C) x), _ => f);
        }
       
        public MatcherKDN<A, B> Case<C>(Func<bool> p, Func<C, B> f) where C : A
        {
            return new MatcherKDN<A, B>(Key, Default, this, _ => p(), x => f((C) x));
        }
       
        public MatcherKDN<A, B> Case<C>(Func<bool> p, Func<B> f) where C : A
        {
            return new MatcherKDN<A, B>(Key, Default, this, _ => p(), _ => f());
        }
       
        public MatcherKDN<A, B> Case<C>(Func<bool> p, B f) where C : A
        {
            return new MatcherKDN<A, B>(Key, Default, this, _ => p(), _ => f);
        }
       
        public MatcherKDN<A, B> Case<C>(bool p, Func<C, B> f) where C : A
        {
            return new MatcherKDN<A, B>(Key, Default, this, _ => p, x => f((C) x));
        }
       
        public MatcherKDN<A, B> Case<C>(bool p, Func<B> f) where C : A
        {
            return new MatcherKDN<A, B>(Key, Default, this, _ => p, _ => f());
        }
       
        public MatcherKDN<A, B> Case<C>(bool p, B f) where C : A
        {
            return new MatcherKDN<A, B>(Key, Default, this, _ => p, _ => f);
        }
           
    }
}

