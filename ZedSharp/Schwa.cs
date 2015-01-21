using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace ZedSharp
{
    public static class Schwa
    {
        public static Expression Parse(String source)
        {
            var syntax = Syntax.Read(source);
            return syntax.Parse();
        }

        public static A Eval<A>(String source)
        {
            var expr = Parse(source);
            var lambda = Expression.Lambda<Func<A>>(expr);
            return lambda.Compile().Invoke();
        }
    }

    internal class Syntax
    {
        public static Token Read(String str)
        {
            using (var reader = new StringReader(str))
                return new Syntax(reader).ReadToken();
        }

        public static List<Token> ReadAll(String str)
        {
            using (var reader = new StringReader(str))
                return new Syntax(reader).ReadAllTokens();
        }

        public static List<Token> ReadFile(String path)
        {
            using (var reader = File.OpenText(path))
                return new Syntax(reader, path).ReadAllTokens();
        }

        private readonly String _sourceUnit;
        private int _line = 1;
        private int _column = 1;
        private readonly TextReader _reader;

        public Syntax(TextReader reader, String sourceUnit = "Unknown")
        {
            _reader = reader;
            _sourceUnit = sourceUnit;
        }

        public List<Token> ReadAllTokens()
        {
            var tokens = new List<Token>();

            for (; ; )
            {
                SkipWhiteSpace();

                if (_reader.Peek() == -1)
                    break;

                tokens.Add(ReadToken());
            }

            return tokens;
        }

        public Token ReadToken()
        {
            SkipWhiteSpace();
            var location = CurrentLocation;

            switch (_reader.Peek())
            {
                case -1:
                    Fail("Unexpected end of file");
                    return null;
                case '(':
                {
                    Skip(); // Pull '(' off the reader
                    var tokens = new List<Token>();

                    for (var token = ReadToken(); !(token is ComboEnd); token = ReadToken())
                    {
                        if (token == null)
                            Fail("Unexpected end of combo");

                        tokens.Add(token);
                    }

                    return new Combo(tokens, location);
                }
                case '[':
                {
                    Skip(); // Pull '[' off the reader
                    var tokens = new List<Token>();

                    for (var token = ReadToken(); !(token is ComboEnd); token = ReadToken())
                    {
                        if (token == null)
                            Fail("Unexpected end of combo");

                        tokens.Add(token);
                    }

                    return new SquareCombo(tokens, location);
                }
                case '{':
                {
                    Skip(); // Pull '{' off the reader
                    var tokens = new List<Token>();

                    for (var token = ReadToken(); !(token is ComboEnd); token = ReadToken())
                    {
                        if (token == null)
                            Fail("Unexpected end of combo");

                        tokens.Add(token);
                    }

                    return new CurlyCombo(tokens, location);
                }
                case ')':
                case ']':
                case '}':
                    Skip(); // Pull ')' or ']' off the reader
                    return new ComboEnd(location);
                case '\"':
                    return new Atom(ReadStringLiteral(), location);
                default:
                    return new Atom(ReadLiteral(), location);
            }
        }

        private class ComboEnd : Token
        {
            internal ComboEnd(Location location) : base(location)
            {
            }

            public override Expression Parse()
            {
                throw new Exception("ComboEnd cannot be parsed");
            }
        }

        private void Skip()
        {
            _reader.Read();
            _column++;
        }

        private void SkipWhiteSpace()
        {
            for (; ; )
            {
                var b = _reader.Peek();

                if (b == -1)
                    break;

                var ch = (char)b;

                if (!Char.IsWhiteSpace(ch))
                    break;

                if (ch != '\r')
                    _column++;

                if (ch == '\n')
                {
                    _line++;
                    _column = 1;
                }

                _reader.Read();
            }
        }

        private String ReadLiteral()
        {
            var builder = new StringBuilder();

            for (; ; )
            {
                var b = _reader.Peek();

                if (b == -1)
                    break;

                var ch = (char)b;

                if (ch != '\r')
                    _column++;

                if (ch == '\n')
                {
                    _line++;
                    _column = 1;
                }

                if (Char.IsWhiteSpace(ch) || ch.IsIn(')', '(', ']', '[', '}', '{'))
                    break;

                builder.Append(ch);
                _reader.Read();
            }

            return builder.ToString();
        }

        private String ReadStringLiteral()
        {
            var builder = new StringBuilder();
            builder.Append((char)_reader.Read()); // pull the opening qoute

            for (; ; )
            {
                var b = _reader.Read();

                if (b == -1)
                    Fail("Uncompleted string literal");

                var ch = (char)b;

                if (ch != '\r')
                    _column++;

                if (ch == '\n')
                {
                    _line++;
                    _column = 1;
                }

                builder.Append(ch);

                if (ch == '\"')
                    break;
            }

            return builder.ToString();
        }

        private Location CurrentLocation
        {
            get { return new Location(_sourceUnit, _line, _column); }
        }

        private void Fail(String message)
        {
            throw new LexException(CurrentLocation, message);
        }
    }

    internal class Location
    {
        public Location(String file, int line, int column)
        {
            File = file;
            Line = line;
            Column = column;
        }

        public String File { get; private set; }
        public int Line { get; private set; }
        public int Column { get; private set; }

        public override string ToString()
        {
            return String.Format("Line {0}, Column {1} in {2}", Line, Column, File);
        }
    }

    internal abstract class Token
    {
        protected Token(Location location)
        {
            Location = location;
        }

        public Location Location { get; private set; }

        public abstract Expression Parse();

        internal static readonly ConstantExpression Zero = Expression.Constant(0);
        internal static readonly ConstantExpression Null = Expression.Constant(null);
        internal static readonly ConstantExpression False = Expression.Constant(false);
        internal static readonly ConstantExpression True = Expression.Constant(true);

    }

    internal class Atom : Token
    {
        public Atom(String atom, Location location) : base(location)
        {
            Literal = atom;
        }

        public String Literal { get; private set; }

        public override string ToString()
        {
            return Literal;
        }

        private static readonly Regex IntRegex = new Regex(@"^[-+]?[0-9]+u?l?$");
        private static readonly Regex FloatRegex = new Regex(@"^[-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?[fm]?$");

        private static String ButLast(String str, int len)
        {
            return str.Substring(0, str.Length - len);
        }

        public override Expression Parse()
        {
            if (Literal == "null")  return Null;
            if (Literal == "true")  return True;
            if (Literal == "false") return False;
            if (Literal[0] == '\"') return Expression.Constant(String.Intern(Literal.Substring(1, Literal.Length - 2)));

            if (IntRegex.IsMatch(Literal))
            {
                if (Literal.EndsWith("ul")) return Expression.Constant(UInt64.Parse(ButLast(Literal, 2)));
                if (Literal.EndsWith("u"))  return Expression.Constant(UInt32.Parse(ButLast(Literal, 1)));
                if (Literal.EndsWith("l"))  return Expression.Constant(Int64.Parse(ButLast(Literal, 1)));

                return Expression.Constant(Int32.Parse(Literal));
            }

            if (FloatRegex.IsMatch(Literal))
            {
                if (Literal.EndsWith("f")) return Expression.Constant(Single.Parse(ButLast(Literal, 1)));
                if (Literal.EndsWith("m")) return Expression.Constant(Decimal.Parse(ButLast(Literal, 1)));

                return Expression.Constant(Double.Parse(Literal));
            }

            if (Literal.Contains("."))
            {
                var parts = Literal.Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries);

                // TODO: identify parts[0] as variable or class
                // identify each item in the chain as property, field or method
            }

            return Expression.Variable(typeof(object), Literal);
        }
    }

    internal class CurlyCombo : Token
    {
        public CurlyCombo(List<Token> tokens, Location location) : base(location)
        {
            Tokens = tokens;
        }

        public List<Token> Tokens { get; private set; }

        public override string ToString()
        {
            return "{" + String.Join(" ", Tokens) + "}";
        }

        public override Expression Parse()
        {
            var keyType = Combo.ParseType(((Atom)Tokens[0]).Literal);
            var valType = Combo.ParseType(((Atom)Tokens[1]).Literal);
            var dictType = typeof(Dictionary<object, object>).GetGenericTypeDefinition().MakeGenericType(keyType, valType);
            var ctor = dictType.GetConstructor(new Type[0]);
            if (ctor == null) throw new Exception("Dictionary doesn't have a no-arg constructor somehow");
            var addMethod = dictType.GetMethod("Add");
            if (addMethod == null) throw new Exception("Dictionary doesn't have an Add method somehow");
            var elementExprs = Tokens.Skip(2).Select(x => x.Parse()).ToArray();
            return Expression.ListInit(
                Expression.New(ctor),
                elementExprs.Partition(2)
                    .Select(x => Expression.ElementInit(addMethod, x.ElementAt(0), x.ElementAt(1))));
        }
    }

    internal class SquareCombo : Token
    {
        public SquareCombo(List<Token> tokens, Location location) : base(location)
        {
            Tokens = tokens;
        }

        public List<Token> Tokens { get; private set; }

        public override string ToString()
        {
            return "[" + String.Join(" ", Tokens) + "]";
        }

        public override Expression Parse()
        {
            var elementType = Combo.ParseType(((Atom) Tokens[0]).Literal);
            var listType = typeof(List<>).MakeGenericType(elementType);
            var ctor = listType.GetConstructor(new Type[0]);
            if (ctor == null) throw new Exception("List doesn't have no-arg constructor somehow");
            var elementExprs = Tokens.Skip(1).Select(x => x.Parse()).ToArray();
            return Expression.ListInit(Expression.New(ctor), elementExprs);
        }
    }

    internal class Combo : Token
    {
        public Combo(List<Token> tokens, Location location) : base(location)
        {
            Tokens = tokens;
        }

        public List<Token> Tokens { get; private set; }

        public override string ToString()
        {
            return "(" + String.Join(" ", Tokens) + ")";
        }

        public override Expression Parse()
        {
            if (Tokens.Count == 0) throw new ParseException(Location, "Empty parens don't mean anything");

            if (Tokens[0] is Atom)
            {
                return ParseAtom((Atom) Tokens[0]);
            }

            throw new NotImplementedException();
        }

        private Expression ParseAtom(Atom atom)
        {
            switch (atom.Literal)
            {
                case "!": return UnaryOp(Tokens, Expression.Not);
                case "~": return UnaryOp(Tokens, Expression.OnesComplement);
                case "++": return UnaryOp(Tokens, Expression.Increment);
                case "--": return UnaryOp(Tokens, Expression.Decrement);
                case "<": return BinaryOp(Tokens, Expression.LessThan);
                case "<=": return BinaryOp(Tokens, Expression.LessThanOrEqual);
                case ">": return BinaryOp(Tokens, Expression.GreaterThan);
                case ">=": return BinaryOp(Tokens, Expression.GreaterThanOrEqual);
                case "<<": return BinaryOp(Tokens, Expression.LeftShift);
                case ">>": return BinaryOp(Tokens, Expression.RightShift);
                case "??": return BinaryOp(Tokens, Expression.Coalesce);
                case "==": return BinaryOp(Tokens, Expression.Equal);
                case "!=": return BinaryOp(Tokens, Expression.NotEqual);
                case "if":
                case "?:": return TernaryOp(Tokens, Expression.Condition);
                case "&": return NaryOp(Tokens, Expression.And);
                case "|": return NaryOp(Tokens, Expression.Or);
                case "and":
                case "&&": return NaryOp(Tokens, Expression.AndAlso);
                case "or":
                case "||": return NaryOp(Tokens, Expression.OrElse);
                case "xor":
                case "^": return NaryOp(Tokens, Expression.ExclusiveOr);
                case "+": return NaryOp(Tokens, Expression.Add);
                case "-": return NaryOp(Tokens, Expression.Subtract);
                case "*": return NaryOp(Tokens, Expression.Multiply);
                case "/": return NaryOp(Tokens, Expression.Divide);
                case "%": return NaryOp(Tokens, Expression.Modulo);
                case "#": // use for indexer access (get only, set not supported)
                {
                    var target = Tokens[1].Parse();
                    var args = Tokens.Skip(2).Select(x => x.Parse());
                    var type = target.Type;
                    var indexer = type.GetProperties()
                        .Where(x => x.GetIndexParameters().Any())
                        .FirstMaybe(x => ParamsMatch(x.GetIndexParameters(), args))
                        .OrElseThrow("No indexer found that matches those arguments");
                    return Expression.Call(target, indexer.GetMethod, args);
                }
                case "is":
                {
                    var typeAtom = (Atom)Tokens[1];
                    var type = ParseType(typeAtom.Literal);
                    return Expression.TypeIs(Tokens[2].Parse(), type);
                }
                case "as":
                {
                    var typeAtom = (Atom)Tokens[1];
                    var type = ParseType(typeAtom.Literal);
                    return Expression.TypeAs(Tokens[2].Parse(), type);
                }
                case "cast":
                {
                    var typeAtom = (Atom)Tokens[1];
                    var type = ParseType(typeAtom.Literal);
                    return Expression.Convert(Tokens[2].Parse(), type);
                }
                case "typeof":
                {
                    var typeAtom = (Atom)Tokens[1];
                    var type = ParseType(typeAtom.Literal);
                    return Expression.Constant(type);
                }
                case "default":
                {
                    var typeAtom = (Atom)Tokens[1];
                    var type = ParseType(typeAtom.Literal);
                    return Expression.Default(type);
                }
                case "new":
                {
                    var typeAtom = (Atom)Tokens[1];
                    var type = ParseType(typeAtom.Literal);
                    var args = Tokens.Skip(2).Select(x => x.Parse()).ToArray();
                    var ctor = type.GetConstructor(args.Select(x => x.Type).ToArray());
                    if (ctor == null) throw new Exception("Type \"" + type + "\" does not have that constructor");
                    return Expression.New(ctor, args);
                }
                case "=>":
                {
                    var paramsToken = (Combo)Tokens[1];
                    var paramsExprs = paramsToken.Tokens.Select(x => x.Parse()).ToArray();
                    var bodyExpr = Tokens[2].Parse();
                    //return Expression.Lambda(bodyExpr, paramsExprs);
                    //needs type spec syntax
                    throw new NotImplementedException();
                }
                case ".":
                {
                    throw new NotImplementedException();
                }
                // TODO: select, where, orderby, groupby, distinct
                // TODO: static methods
                // TODO: instance methods
                // TODO: property access chains
                // TODO: void instance methods return target object
            }

            throw new NotImplementedException();
        }

        private static bool ParamsMatch(IEnumerable<ParameterInfo> paramz, IEnumerable<Expression> args)
        {
            var paramArray = paramz.ToArray();
            var argArray = args.ToArray();

            return paramArray.Zip(argArray, Tuple.Create)
                .All(x => x.Item2.Type.IsAssignableTo(x.Item1.ParameterType));
        }

        public static Type ParseType(String str)
        {
            return TypeKeywords.GetMaybe(str).OrElse(null);
        }

        private static readonly Dictionary<string, Type> TypeKeywords = Dictionary.Of(
            "bool", typeof (bool),
            "char", typeof (char),
            "int", typeof (int),
            "long", typeof (long),
            "byte", typeof (byte),
            "double", typeof (double),
            "decimal", typeof (decimal),
            "string", typeof (string),
            "object", typeof (object));

        private static Expression NaryOp(IEnumerable<Token> tokens, Func<Expression, Expression, Expression> f)
        {
            return tokens.Skip(1).Select(x => x.Parse()).Aggregate(f);
        }

        private Expression TernaryOp(IEnumerable<Token> tokens, Func<Expression, Expression, Expression, Expression> f)
        {
            var array = tokens.Skip(1).ToArray();

            if (array.Length != 3)
                throw new ParseException(Location, "Ternary operator requires exactly 3 arguments");

            return f(array[0].Parse(), array[1].Parse(), array[2].Parse());
        }

        private Expression BinaryOp(IEnumerable<Token> tokens, Func<Expression, Expression, Expression> f)
        {
            var array = tokens.Skip(1).ToArray();

            if (array.Length != 2)
                throw new ParseException(Location, "Binary operator requires exactly 2 arguments");

            return f(array[0].Parse(), array[1].Parse());
        }

        private Expression UnaryOp(IEnumerable<Token> tokens, Func<Expression, Expression> f)
        {
            var array = tokens.Skip(1).ToArray();

            if (array.Length != 1)
                throw new ParseException(Location, "Unary operator requires exactly 1 arguments");

            return f(array[0].Parse());
        }
    }

    internal class LexException : ApplicationException
    {
        public LexException(Location location, String message) : base(message)
        {
            Location = location;
        }

        public Location Location { get; private set; }
    }

    internal class ParseException : ApplicationException
    {
        public ParseException(Location location, String message) : base(message)
        {
            Location = location;
        }

        public Location Location { get; private set; }
    }
}
