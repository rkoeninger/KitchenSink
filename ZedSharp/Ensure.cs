using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZedSharp
{
    public static class Ensure
    {
        public static Ensure<A> Of<A>(A value)
        {
            return new Ensure<A>(value);
        }
    }

    public struct Ensure<A>
    {
        internal Ensure(A value) : this(value, Enumerable.Empty<Exception>())
        {
        }

        private Ensure(A value, IEnumerable<Exception> errors) : this()
        {
            Value = value;
            ErrorList = errors.ToList();
        }

        private A Value { get; set; }
        private List<Exception> ErrorList { get; set; }

        public IEnumerable<Exception> Errors
        {
            get { return ErrorList; }
        }

        public bool HasErrors
        {
            get { return ErrorList.Count > 0; }
        }

        public Ensure<A> Is(Func<A, bool> f, String message = null)
        {
            return new Ensure<A>(Value, f(Value) ? ErrorList : ErrorList.Concat(new[] { new ApplicationException(message ?? "") }));
        }

        public Ensure<A> Is(Action<A> f)
        {
            try
            {
                f(Value);
                return new Ensure<A>(Value, ErrorList);
            }
            catch (Exception exc)
            {
                return new Ensure<A>(Value, ErrorList.Concat(new[] { exc }));
            }
        }

        public Unsure<A> ToUnsure()
        {
            return HasErrors ? Unsure.None<A>() : Unsure.Of(Value);
        }
    }
}
