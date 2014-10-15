using System;
using System.Collections.Generic;

namespace ZedSharp
{
    public class MultiMethod<A, B>
    {
        public MultiMethod(Func<A, Object> f0)
        {
            Overloads = new Dictionary<Tuple<Object>, Func<A, B>>();
            Selector0 = f0;
        }

        private Func<A, Object> Selector0 { get; set; }
        private Dictionary<Tuple<Object>, Func<A, B>> Overloads { get; set; }

        public MultiMethod<A, B> Add(Object arg0, Func<A, B> f)
        {
            Overloads.Add(Tuple.Create(arg0), f);
            return this;
        }

        public Func<A, B> Select(A arg0)
        {
            var target0 = Selector0(arg0);
            var targets = Tuple.Create(target0);
            return Overloads[targets];
        }

        public B Apply(A arg0)
        {
            var f = Select(arg0);
            return f(arg0);
        }

        public Func<A, B> AsFunc()
        {
            return Apply;
        }
    }

    public class MultiMethod<A, B, C>
    {
        public MultiMethod(Func<A, Object> f0, Func<B, Object> f1)
        {
            Overloads = new Dictionary<Tuple<Object, Object>, Func<A, B, C>>();
            Selector0 = f0;
            Selector1 = f1;
        }

        private Func<A, Object> Selector0 { get; set; }
        private Func<B, Object> Selector1 { get; set; }
        private Dictionary<Tuple<Object, Object>, Func<A, B, C>> Overloads { get; set; }

        public MultiMethod<A, B, C> Add(Object arg0, Object arg1, Func<A, B, C> f)
        {
            Overloads.Add(Tuple.Create(arg0, arg1), f);
            return this;
        }

        public Func<A, B, C> Select(A arg0, B arg1)
        {
            var target0 = Selector0(arg0);
            var target1 = Selector1(arg1);
            var targets = Tuple.Create(target0, target1);
            return Overloads[targets];
        }

        public C Apply(A arg0, B arg1)
        {
            var f = Select(arg0, arg1);
            return f(arg0, arg1);
        }

        public Func<A, B, C> AsFunc()
        {
            return Apply;
        }
    }

    public class MultiMethod<A, B, C, D>
    {
        public MultiMethod(Func<A, Object> f0, Func<B, Object> f1, Func<C, Object> f2)
        {
            Overloads = new Dictionary<Tuple<Object, Object, Object>, Func<A, B, C, D>>();
            Selector0 = f0;
            Selector1 = f1;
            Selector2 = f2;
        }

        private Func<A, Object> Selector0 { get; set; }
        private Func<B, Object> Selector1 { get; set; }
        private Func<C, Object> Selector2 { get; set; }
        private Dictionary<Tuple<Object, Object, Object>, Func<A, B, C, D>> Overloads { get; set; }

        public MultiMethod<A, B, C, D> Add(Object arg0, Object arg1, Object arg2, Func<A, B, C, D> f)
        {
            Overloads.Add(Tuple.Create(arg0, arg1, arg2), f);
            return this;
        }

        public Func<A, B, C, D> Select(A arg0, B arg1, C arg2)
        {
            var target0 = Selector0(arg0);
            var target1 = Selector1(arg1);
            var target2 = Selector2(arg2);
            var targets = Tuple.Create(target0, target1, target2);
            return Overloads[targets];
        }

        public D Apply(A arg0, B arg1, C arg2)
        {
            var f = Select(arg0, arg1, arg2);
            return f(arg0, arg1, arg2);
        }

        public Func<A, B, C, D> AsFunc()
        {
            return Apply;
        }
    }

    public class MultiMethod<A, B, C, D, E>
    {
        public MultiMethod(Func<A, Object> f0, Func<B, Object> f1, Func<C, Object> f2, Func<D, Object> f3)
        {
            Overloads = new Dictionary<Tuple<Object, Object, Object, Object>, Func<A, B, C, D, E>>();
            Selector0 = f0;
            Selector1 = f1;
            Selector2 = f2;
            Selector3 = f3;
        }

        private Func<A, Object> Selector0 { get; set; }
        private Func<B, Object> Selector1 { get; set; }
        private Func<C, Object> Selector2 { get; set; }
        private Func<D, Object> Selector3 { get; set; }
        private Dictionary<Tuple<Object, Object, Object, Object>, Func<A, B, C, D, E>> Overloads { get; set; }

        public MultiMethod<A, B, C, D, E> Add(Object arg0, Object arg1, Object arg2, Object arg3, Func<A, B, C, D, E> f)
        {
            Overloads.Add(Tuple.Create(arg0, arg1, arg2, arg3), f);
            return this;
        }

        public Func<A, B, C, D, E> Select(A arg0, B arg1, C arg2, D arg3)
        {
            var target0 = Selector0(arg0);
            var target1 = Selector1(arg1);
            var target2 = Selector2(arg2);
            var target3 = Selector3(arg3);
            var targets = Tuple.Create(target0, target1, target2, target3);
            return Overloads[targets];
        }

        public E Apply(A arg0, B arg1, C arg2, D arg3)
        {
            var f = Select(arg0, arg1, arg2, arg3);
            return f(arg0, arg1, arg2, arg3);
        }

        public Func<A, B, C, D, E> AsFunc()
        {
            return Apply;
        }
    }

    public class MultiMethod<A, B, C, D, E, F>
    {
        public MultiMethod(Func<A, Object> f0, Func<B, Object> f1, Func<C, Object> f2, Func<D, Object> f3, Func<E, Object> f4)
        {
            Overloads = new Dictionary<Tuple<Object, Object, Object, Object, Object>, Func<A, B, C, D, E, F>>();
            Selector0 = f0;
            Selector1 = f1;
            Selector2 = f2;
            Selector3 = f3;
            Selector4 = f4;
        }

        private Func<A, Object> Selector0 { get; set; }
        private Func<B, Object> Selector1 { get; set; }
        private Func<C, Object> Selector2 { get; set; }
        private Func<D, Object> Selector3 { get; set; }
        private Func<E, Object> Selector4 { get; set; }
        private Dictionary<Tuple<Object, Object, Object, Object, Object>, Func<A, B, C, D, E, F>> Overloads { get; set; }

        public MultiMethod<A, B, C, D, E, F> Add(Object arg0, Object arg1, Object arg2, Object arg3, Object arg4, Func<A, B, C, D, E, F> f)
        {
            Overloads.Add(Tuple.Create(arg0, arg1, arg2, arg3, arg4), f);
            return this;
        }

        public Func<A, B, C, D, E, F> Select(A arg0, B arg1, C arg2, D arg3, E arg4)
        {
            var target0 = Selector0(arg0);
            var target1 = Selector1(arg1);
            var target2 = Selector2(arg2);
            var target3 = Selector3(arg3);
            var target4 = Selector4(arg4);
            var targets = Tuple.Create(target0, target1, target2, target3, target4);
            return Overloads[targets];
        }

        public F Apply(A arg0, B arg1, C arg2, D arg3, E arg4)
        {
            var f = Select(arg0, arg1, arg2, arg3, arg4);
            return f(arg0, arg1, arg2, arg3, arg4);
        }

        public Func<A, B, C, D, E, F> AsFunc()
        {
            return Apply;
        }
    }

    public class MultiMethod<A, B, C, D, E, F, G>
    {
        public MultiMethod(Func<A, Object> f0, Func<B, Object> f1, Func<C, Object> f2, Func<D, Object> f3, Func<E, Object> f4, Func<F, Object> f5)
        {
            Overloads = new Dictionary<Tuple<Object, Object, Object, Object, Object, Object>, Func<A, B, C, D, E, F, G>>();
            Selector0 = f0;
            Selector1 = f1;
            Selector2 = f2;
            Selector3 = f3;
            Selector4 = f4;
            Selector5 = f5;
        }

        private Func<A, Object> Selector0 { get; set; }
        private Func<B, Object> Selector1 { get; set; }
        private Func<C, Object> Selector2 { get; set; }
        private Func<D, Object> Selector3 { get; set; }
        private Func<E, Object> Selector4 { get; set; }
        private Func<F, Object> Selector5 { get; set; }
        private Dictionary<Tuple<Object, Object, Object, Object, Object, Object>, Func<A, B, C, D, E, F, G>> Overloads { get; set; }

        public MultiMethod<A, B, C, D, E, F, G> Add(Object arg0, Object arg1, Object arg2, Object arg3, Object arg4, Object arg5, Func<A, B, C, D, E, F, G> f)
        {
            Overloads.Add(Tuple.Create(arg0, arg1, arg2, arg3, arg4, arg5), f);
            return this;
        }

        public Func<A, B, C, D, E, F, G> Select(A arg0, B arg1, C arg2, D arg3, E arg4, F arg5)
        {
            var target0 = Selector0(arg0);
            var target1 = Selector1(arg1);
            var target2 = Selector2(arg2);
            var target3 = Selector3(arg3);
            var target4 = Selector4(arg4);
            var target5 = Selector5(arg5);
            var targets = Tuple.Create(target0, target1, target2, target3, target4, target5);
            return Overloads[targets];
        }

        public G Apply(A arg0, B arg1, C arg2, D arg3, E arg4, F arg5)
        {
            var f = Select(arg0, arg1, arg2, arg3, arg4, arg5);
            return f(arg0, arg1, arg2, arg3, arg4, arg5);
        }

        public Func<A, B, C, D, E, F, G> AsFunc()
        {
            return Apply;
        }
    }

}