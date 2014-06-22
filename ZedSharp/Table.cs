using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZedSharp
{
    public class Table
    {
        public static Table<A, B, C> Of<A, B, C>(Col<A> a, Col<B> b, Col<C> c)
        {
            return new Table<A, B, C>(a, b, c);
        }
    }

    public class Table<A>
    {

    }

    /// <summary>
    /// Relational data structure.
    /// </summary>
    /// <typeparam name="A"></typeparam>
    /// <typeparam name="B"></typeparam>
    /// <typeparam name="C"></typeparam>
    public class Table<A, B, C>
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

        public Table<Z> SelectCol<Z>(Col<Z> col)
        {
            return new Table<Z>();
        }

        public Table<A, B, C> Where(Func<A, B, C, bool> f)
        {
            return new Table<A, B, C>(Column1, Column2, Column3, Rows.Where(r => f(r.Item1, r.Item2, r.Item3)).ToList());
        }
    }

    public class Col<A>
    {
        public Col(String name)
        {
            Name = name;
        }

        public String Name { get; private set; }
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
