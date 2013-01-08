using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iCompiler.Helpers
{
    public abstract class Statement
    {
    }

    public class DeclareVariable : Statement
    {
        public string Ident;
        public Expression Expr;
    }

    public class Print : Statement
    {
        public Expression Expr;
    }

    public class Assign : Statement
    {
        public string Ident;
        public Expression Expr;
    }

    public class ForLoop : Statement
    {
        public string Ident;
        public Expression From;
        public Expression To;
        public Statement Body;
    }

    public class ReadInteger : Statement
    {
        public string Ident;
    }

    public class StatementList : Statement
    {
        public Statement First;
        public Statement Second;
    }

    public abstract class Expression
    {
    }

    public class StringLiteral : Expression
    {
        public string Value;
    }

    public class IntegerLiteral : Expression
    {
        public int Value;
    }

    public class Variable : Expression
    {
        public string Ident;
    }

    public class BinaryExpression : Expression
    {
        public Expression Left;
        public BinaryOperator Op;
        public Expression Right;
    }

    public enum BinaryOperator
    {
        Add,
        Sub,
        Mul,
        Div
    }

    public class ParserException : Exception
    {
        public ParserException(string message)
            : base(message)
        {
        }
    }

    public class CodeGeneratorException : Exception
    {
        public CodeGeneratorException(string message)
            : base(message)
        {
        }
    }

    public class LexicalAnalysisException : Exception
    {
        public LexicalAnalysisException(string message)
            : base(message)
        {
        }
    }
}
