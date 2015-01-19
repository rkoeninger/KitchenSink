using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
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

        private readonly String _source;
        private int _line = 1;
        private int _column = 1;
        private readonly TextReader _reader;

        public Syntax(TextReader reader, String source = "Unknown")
        {
            _reader = reader;
            _source = source;
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
                    Skip(); // Pull '(' off the reader
                    var tokens = new List<Token>();

                    for (var token = ReadToken(); !(token is ComboEnd); token = ReadToken())
                    {
                        if (token == null)
                            Fail("Unexpected end of combo");

                        tokens.Add(token);
                    }

                    return new Combo(tokens, location);
                case ')':
                    Skip(); // Pull ')' off the reader
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

                if (Char.IsWhiteSpace(ch) || ch == ')' || ch == '(')
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
            get { return new Location(_source, _line, _column); }
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

        private static readonly Regex NumberRegex = new Regex(@"^[-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?$");

        public override Expression Parse()
        {
            if (NumberRegex.IsMatch(Literal))
                return Expression.Constant(Decimal.Parse(Literal));

            if (Literal[0] == '\"') return Expression.Constant(Literal.Substring(1, Literal.Length - 2));
            if (Literal == "true") return Expression.Constant(true);
            if (Literal == "false") return Expression.Constant(false);

            return Expression.Constant(Literal);
        }
    }

    internal class Combo : Token
    {
        public Combo(List<Token> tokens, Location location)
            : base(location)
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
                var atom = (Atom) Tokens[0];

                switch (atom.Literal)
                {
                    case "!":  return UnaryOp(Tokens, Expression.Not);
                    case "~":  return UnaryOp(Tokens, Expression.OnesComplement);
                    case "++": return UnaryOp(Tokens, Expression.Increment);
                    case "--": return UnaryOp(Tokens, Expression.Decrement);
                    case "<":  return BinaryOp(Tokens, Expression.LessThan);
                    case "<=": return BinaryOp(Tokens, Expression.LessThanOrEqual);
                    case ">":  return BinaryOp(Tokens, Expression.GreaterThan);
                    case ">=": return BinaryOp(Tokens, Expression.GreaterThanOrEqual);
                    case "<<": return BinaryOp(Tokens, Expression.LeftShift);
                    case ">>": return BinaryOp(Tokens, Expression.RightShift);
                    case "??": return BinaryOp(Tokens, Expression.Coalesce);
                    case "==": return BinaryOp(Tokens, Expression.Equal);
                    case "!=": return BinaryOp(Tokens, Expression.NotEqual);
                    case "&":  return NaryOp(Tokens, Expression.And);
                    case "&&": return NaryOp(Tokens, Expression.AndAlso);
                    case "|":  return NaryOp(Tokens, Expression.Or);
                    case "||": return NaryOp(Tokens, Expression.OrElse);
                    case "+":  return NaryOp(Tokens, Expression.Add);
                    case "-":  return NaryOp(Tokens, Expression.Subtract);
                    case "*":  return NaryOp(Tokens, Expression.Multiply);
                    case "/":  return NaryOp(Tokens, Expression.Divide);
                    case "%":  return NaryOp(Tokens, Expression.Modulo);
                    case "if": return TeraryOp(Tokens, Expression.Condition);
                    // [] operator?
                    case "=>":
                        var paramsToken = (Combo) Tokens[1];
                        var paramsExprs = paramsToken.Tokens.Select(x => x.Parse()).ToArray();
                        var bodyExpr = Tokens[2].Parse();
                        //return Expression.Lambda(bodyExpr, paramsExprs);
                        return null;
                    case ".":
                    case "is":
                    case "as":
                    case "cast":
                    case "typeof":
                    case "default":
                    case "new":
                        throw new NotImplementedException();
                    // select, where, orderby, groupby, distinct

                        // void instance methods return target object
                }
            }

            return null;
        }

        private static readonly ConstantExpression Zero = Expression.Constant(0);
        private static readonly ConstantExpression Null = Expression.Constant(null);
        private static readonly ConstantExpression False = Expression.Constant(false);
        private static readonly ConstantExpression True = Expression.Constant(true);

        private static Expression NaryOp(IEnumerable<Token> tokens, Func<Expression, Expression, Expression> f)
        {
            return tokens.Skip(1).Select(x => x.Parse()).Aggregate(f);
        }

        private Expression TeraryOp(IEnumerable<Token> tokens, Func<Expression, Expression, Expression, Expression> f)
        {
            var array = tokens.Skip(1).ToArray();

            if (array.Length != 3)
                throw new ParseException(Location, "Binary operator requires exactly 3 arguments");

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
                throw new ParseException(Location, "Binary operator requires exactly 1 arguments");

            return f(array[0].Parse());
        }

        private static void CheckArity(Token t, int count, String form)
        {
            var combo = (Combo) t;
            var tokenCount = combo.Tokens.Count;

            if (tokenCount != count)
                throw new ParseException(t.Location, String.Format("{0} expression should have {1} parts but found {2}", form, count, tokenCount));
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
