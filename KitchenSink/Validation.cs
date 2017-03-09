using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using KitchenSink.Collections;
using KitchenSink.Extensions;
using static KitchenSink.Operators;

// TODO: some of this is made unnecessary by C# 6.0 (?.) operator

namespace KitchenSink
{
    public static class Verify
    {
        public static Validation<A> That<A>(A obj)
        {
            return new Validation<A>(obj);
        }

        /// <summary>Target of this method is not checked. Argument gets passed through unmodified.</summary>
        public static A SkipVerify<A>(this A x) where A : class
        {
            return x;
        }

        /// <summary>All sub-expressions of this method are not checked. Argument gets passed through unmodified.</summary>
        public static A NoVerify<A>(this A x) where A : class
        {
            return x;
        }

        private static readonly MethodInfo SkipVerifyMethodInfo;
        private static readonly MethodInfo VerifyNoneMethodInfo;
        private static readonly Expression TrueExpr = Expression.Constant(true);

        static Verify()
        {
            SkipVerifyMethodInfo = GetMethod(() => SkipVerify(""));
            VerifyNoneMethodInfo = GetMethod(() => NoVerify(""));
        }

        private static MethodInfo GetMethod<A>(Expression<Func<A>> expr)
        {
            return ((MethodCallExpression) expr.Body).Method.GetGenericMethodDefinition();
        }

        public static A Null<A>() where A : class
        {
            return null;
        }

        public static bool That<A>(Expression<Func<A>> expr)
        {
            return BoolThunk(MakeRobust(expr.Body)).Compile().Invoke();
        }

        private static Expression Skip(Expression expr)
        {
            var memberExpr = expr as MemberExpression;
            var methodExpr = expr as MethodCallExpression;

            if (memberExpr != null)
            {
                return memberExpr.Expression;
            }

            if (methodExpr != null)
            {
                if (methodExpr.Object != null)
                {
                    return methodExpr.Object;
                }

                if (methodExpr.Arguments.Any())
                {
                    return methodExpr.Arguments.First();
                }
            }

            return expr;
        }

        private static Expression MakeRobust(Expression expr)
        {
            if (expr is MemberExpression)
            {
                var mexpr = (MemberExpression) expr;

                if (mexpr.Expression != null) // local/static member with no subexpression
                {
                    return AndAlso(MakeRobust(mexpr.Expression), NotNullOrFalse(expr));
                }
            }

            if (expr is MethodCallExpression)
            {
                var mexpr = (MethodCallExpression)expr;

                if (mexpr.Method.GetGenericMethodDefinition() == SkipVerifyMethodInfo)
                {
                    return MakeRobust(Skip(mexpr.Arguments.First()));
                }

                if (mexpr.Method.GetGenericMethodDefinition() == VerifyNoneMethodInfo)
                {
                    return TrueExpr;
                }

                if (mexpr.Object != null) // instance method
                {
                    return AndAlso(MakeRobust(mexpr.Object), NotNullOrFalse(expr));
                }

                return AndAlso(mexpr.Arguments.Select(MakeRobust), NotNullOrFalse(expr)); // static or extension method
            }
            
            if (expr is BinaryExpression)
            {
                var bexpr = (BinaryExpression) expr;

                return AndAlso(MakeRobust(bexpr.Left), MakeRobust(bexpr.Right), expr);
            }

            return NotNullOrFalse(expr);
        }

        private static Expression AndAlso(IEnumerable<Expression> exprs0, params Expression[] exprs1)
        {
            return AndAlso(exprs0.Concat(exprs1));
        }

        private static Expression AndAlso(IEnumerable<Expression> exprs)
        {
            return AndAlso(exprs.ToArray());
        }

        private static Expression AndAlso(params Expression[] exprs)
        {
            switch (exprs.Length)
            {
                case 0: return TrueExpr;
                case 1: return exprs.Single();
                default: return exprs.Aggregate(Expression.AndAlso);
            }
        }

        private static Expression<Func<bool>> BoolThunk(Expression expr)
        {
            return Expression.Lambda<Func<bool>>(expr);
        }

        private static Expression NotNullOrFalse(Expression expr)
        {
            if (expr.Type == typeof(bool))
            {
                return Expression.IsTrue(expr);
            }

            return Expression.NotEqual(Expression.Constant(null), expr);
        }
    }

    public static class Validation
    {
        public static Validation<A> Of<A>(A value)
        {
            return new Validation<A>(value);
        }
    }

    public struct Validation<A>
    {
        internal Validation(A value) : this(value, Enumerable.Empty<Exception>())
        {
        }

        private Validation(A value, IEnumerable<Exception> errors) : this()
        {
            Value = value;
            ErrorList = errors.ToArray();
            Done = false;
        }

        private Validation(A value, IEnumerable<Exception> errors, bool done) : this()
        {
            Value = value;
            ErrorList = errors.ToArray();
            Done = done;
        }

        private A Value { get; }
        private Exception[] ErrorList { get; }
        private bool Done { get; }

        public IEnumerable<Exception> Errors => ErrorList.AsEnumerable();
        public bool HasErrors => ErrorList.Length > 0;

        public Validation<A> Cut()
        {
            return new Validation<A>(Value, Errors, HasErrors);
        }

        public Validation<A> Try(Action<A> f)
        {
            if (Done)
                return this;

            try
            {
                f(Value);
                return this;
            }
            catch (Exception e)
            {
                return new Validation<A>(Value, ErrorList.Concat(e));
            }
        }

        public Validation<A> Is(bool cond, string message = null)
        {
            if (Done)
                return this;

            return new Validation<A>(Value, cond ? ErrorList : ErrorList.Concat(new ApplicationException(message ?? "")));
        }

        public Validation<A> Is(Func<A, bool> f, string message = null)
        {
            if (Done)
                return this;

            return new Validation<A>(Value, f(Value) ? ErrorList : ErrorList.Concat(new ApplicationException(message ?? "")));
        }

        public Validation<A> Is(bool cond, Exception exc)
        {
            if (Done)
                return this;

            return new Validation<A>(Value, cond ? ErrorList : ErrorList.Concat(exc));
        }

        public Validation<A> Is(Func<A, bool> f, Exception exc)
        {
            if (Done)
                return this;

            return new Validation<A>(Value, f(Value) ? ErrorList : ErrorList.Concat(exc));
        }

        public Validation<A> Is(Action<A> f)
        {
            if (Done)
                return this;

            try
            {
                f(Value);
                return new Validation<A>(Value, ErrorList);
            }
            catch (Exception exc)
            {
                return new Validation<A>(Value, ErrorList.Concat(exc));
            }
        }

        public Maybe<A> ToMaybe()
        {
            return HasErrors ? Maybe<A>.None : MaybeOf(Value);
        }
    }
}
