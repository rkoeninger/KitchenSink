using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ZedSharp
{
    public static class Verify
    {
        /// <summary>Target of this method is not checked. Argument gets passed through unmodified.</summary>
        public static A NoVerify<A>(this A x) where A : class
        {
            return x;
        }

        private static MethodInfo NoVerifyMethodInfo;

        static Verify()
        {
            NoVerifyMethodInfo = GetMethod(() => NoVerify(""));
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
            return Match.On(expr).Return<Expression>()
                .Case<MemberExpression>(x => x.Expression)
                .Case<MethodCallExpression>(x => x.Object != null, x => x.Object)
                .Case<MethodCallExpression>(x => x.Arguments.Any(), x => x.Arguments.First())
                .Else(expr);
        }

        private static Expression MakeRobust(Expression expr)
        {
            if (expr is MemberExpression)
            {
                var mexpr = (MemberExpression) expr;

                return Expression.AndAlso(MakeRobust(mexpr.Expression), NotNullOrFalse(expr));
            }
            else if (expr is MethodCallExpression)
            {
                var mexpr = (MethodCallExpression) expr;

                if (mexpr.Method.GetGenericMethodDefinition() == NoVerifyMethodInfo)
                {
                    return MakeRobust(Skip(mexpr.Arguments.First()));
                }
                else if (mexpr.Object != null)
                {
                    return Expression.AndAlso(MakeRobust(mexpr.Object), NotNullOrFalse(expr));
                }
                else
                {
                    return Expression.AndAlso(MakeRobust(mexpr.Arguments.First()), NotNullOrFalse(expr));
                }
            }
            else if (expr is BinaryExpression)
            {
                var bexpr = (BinaryExpression) expr;

                return Expression.AndAlso(Expression.AndAlso(MakeRobust(bexpr.Left), MakeRobust(bexpr.Right)), expr);
            }

            return NotNullOrFalse(expr);
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
            else
            {
                return Expression.NotEqual(Expression.Constant(null), expr);
            }
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
        }

        private A Value { get; set; }
        private Exception[] ErrorList { get; set; }

        public IEnumerable<Exception> Errors
        {
            get { return ErrorList.AsEnumerable(); }
        }

        public bool HasErrors
        {
            get { return ErrorList.Length > 0; }
        }

        public Validation<A> Is(bool cond, String message = null)
        {
            return new Validation<A>(Value, cond ? ErrorList : ErrorList.Add(new ApplicationException(message ?? "")));
        }

        public Validation<A> Is(Func<A, bool> f, String message = null)
        {
            return new Validation<A>(Value, f(Value) ? ErrorList : ErrorList.Add(new ApplicationException(message ?? "")));
        }

        public Validation<A> Is(bool cond, Exception exc)
        {
            return new Validation<A>(Value, cond ? ErrorList : ErrorList.Add(exc));
        }

        public Validation<A> Is(Func<A, bool> f, Exception exc)
        {
            return new Validation<A>(Value, f(Value) ? ErrorList : ErrorList.Add(exc));
        }

        public Validation<A> Is(Action<A> f)
        {
            try
            {
                f(Value);
                return new Validation<A>(Value, ErrorList);
            }
            catch (Exception exc)
            {
                return new Validation<A>(Value, ErrorList.Add(exc));
            }
        }

        public Maybe<A> ToMaybe()
        {
            return HasErrors ? Maybe.None<A>() : Maybe.Of(Value);
        }
    }
}
