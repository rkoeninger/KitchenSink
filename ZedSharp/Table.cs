using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZedSharp
{
    /// <summary>
    /// Relational data structure.
    /// </summary>
    public class Table
    {
        public static Table<A, B, C> Of<A, B, C>(Col<A> a, Col<B> b, Col<C> c)
        {
            return new Table<A, B, C>(a, b, c);
        }
    }

    public class Table<A> : IEnumerable<Row<A>>
    {
        internal Table(Col<A> a) : this(a, new List<Row<A>>())
        {
        }

        internal Table(Col<A> a, List<Row<A>> rows)
        {
            Column1 = a;
            Rows = rows;
        }
        
        public Col<A> Column1 { get; private set; }
        private List<Row<A>> Rows { get; set; }

        public IEnumerator<Row<A, B, C>> GetEnumerator()
        {
            return Rows.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class Table<A, B> : IEnumerable<Row<A, B>>
    {
        internal Table(Col<A> a, Col<B> b) : this(a, b, new List<Row<A, B>>())
        {
        }

        internal Table(Col<A> a, Col<B> b, List<Row<A, B>> rows)
        {
            Column1 = a;
            Column2 = b;
            Rows = rows;
        }
        
        public Col<A> Column1 { get; private set; }
        public Col<B> Column2 { get; private set; }
        private List<Row<A, B>> Rows { get; set; }

        public IEnumerator<Row<A, B, C>> GetEnumerator()
        {
            return Rows.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class Table<A, B, C> : IEnumerable<Row<A, B, C>>
    {
        internal Table(Col<A> a, Col<B> b, Col<C> c) : this(a, b, c, new List<Row<A, B, C>>())
        {
        }

        internal Table(Col<A> a, Col<B> b, Col<C> c, List<Row<A, B, C>> rows)
        {
            Column1 = a;
            Column2 = b;
            Column3 = c;
            Rows = rows;
        }
        
        public Col<A> Column1 { get; private set; }
        public Col<B> Column2 { get; private set; }
        public Col<C> Column3 { get; private set; }
        private List<Row<A, B, C>> Rows { get; set; }

        public IEnumerator<Row<A, B, C>> GetEnumerator()
        {
            return Rows.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Table<D> Select<D>(Col<D> col)
        {
            if (Equals(Column1, col)) return new Table<D>(col, Rows.Select(r => new Row<D>((D) (Object) r.Item1)).ToList());
            if (Equals(Column2, col)) return new Table<D>(col, Rows.Select(r => new Row<D>((D) (Object) r.Item2)).ToList());
            if (Equals(Column3, col)) return new Table<D>(col, Rows.Select(r => new Row<D>((D) (Object) r.Item3)).ToList());

            return new Table<D>(col, new List<Row<D>>());
        }
        
        public Table<D, E> Select<D, E>(Func<A, B, C, Row<D, E>> f)
        {
            return new Table<D, E>(new Col<D>(), new Col<E>(), Rows.Select(r => f(r.Item1, r.Item2, r.Item3)).ToList());
        }

        public Table<D, E, F> Select<D, E, F>(Func<A, B, C, Row<D, E, F>> f)
        {
            return new Table<D, E, F>(new Col<D>(), new Col<E>(), new Col<F>(), Rows.Select(r => f(r.Item1, r.Item2, r.Item3)).ToList());
        }

        public Table<A, B, C> Where(Func<A, B, C, bool> f)
        {
            return new Table<A, B, C>(Column1, Column2, Column3, Rows.Where(r => f(r.Item1, r.Item2, r.Item3)).ToList());
        }
    }

    public class Col<A>
    {
        public Col()
        {
            Name = "";
        }

        public Col(String name)
        {
            Name = name;
        }

        public String Name { get; private set; }
    }

    public class Row<A>
    {
        public Row(A a)
        {
            Item1 = a;
        }

        public A Item1 { get; private set; }
    }

    public class Row<A, B>
    {
        public Row(A a, B b)
        {
            Item1 = a;
            Item2 = b;
        }

        public A Item1 { get; private set; }
        public B Item2 { get; private set; }
    }

    public class Row<A, B, C>
    {
        public Row(A a, B b, C c)
        {
            Item1 = a;
            Item2 = b;
            Item3 = c;
        }

        public A Item1 { get; private set; }
        public B Item2 { get; private set; }
        public C Item3 { get; private set; }
    }
}
