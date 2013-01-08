using System;
using System.Collections.Generic;

namespace iCompiler.Helpers
{
    using TokenClass = Tuple<TokenType, LexemeClass>;

    public sealed class SyntaxAnalysis
    {
        private int index;
        private List<TokenClass> Tokens;
        private Statement result;

        public Statement Result
        {
            get { return result; }
        }

        #region Constructors

        public SyntaxAnalysis(List<TokenClass> inputTokens)
        {
            Tokens = inputTokens;
            index = 0;
            result = ParseStatement();

            if (index != Tokens.Count)
            {
                throw new ParserException("Expected EOF");
            }
        }

        #endregion

        private Statement ParseStatement()
        {
            if (index == Tokens.Count)
            {
                throw new ParserException("Unexpected EOF - Expected Statement");
            }

            Statement result;
            var currentToken = Tokens[index];

            if (currentToken.Item1 == TokenType.Keyword && (Keywords)currentToken.Item2.Lexeme == Keywords.print)
            {
                // print statement 

                index++;
                var endIndex = index;

                while (Tokens[endIndex].Item1 != TokenType.Semicolon)
                {
                    endIndex++;
                }

                var print = new Print { Expr = ParseExpression(endIndex) };

                result = print;
            }
            else if (currentToken.Item1 == TokenType.Keyword && (Keywords)currentToken.Item2.Lexeme == Keywords.var)
            {
                // variable declaration

                index++;
                var declareVar = new DeclareVariable();

                if (index < Tokens.Count && Tokens[index].Item1 == TokenType.Identifier)
                {
                    declareVar.Ident = (string)Tokens[index].Item2.Lexeme;
                }
                else
                {
                    throw new ParserException("Expected variable name after 'var' keyword");
                }

                index++;

                if (index == Tokens.Count || Tokens[index].Item1 != TokenType.EqualOp)
                {
                    throw new ParserException("Expected assigning in variable declaration");
                }

                index++;

                var endIndex = index;

                while (Tokens[endIndex].Item1 != TokenType.Semicolon)
                {
                    endIndex++;
                }

                declareVar.Expr = ParseExpression(endIndex);

                result = declareVar;
            }
            else if (currentToken.Item1 == TokenType.Keyword && (Keywords)currentToken.Item2.Lexeme == Keywords.read_integer)
            {
                // read_integer statement
 
                index++;
                var readInt = new ReadInteger();

                if (index < Tokens.Count && Tokens[index].Item1 == TokenType.Identifier)
                {
                    readInt.Ident = (string)Tokens[index++].Item2.Lexeme;

                    result = readInt;
                }
                else
                {
                    throw new ParserException("Expected variable name after 'read_integer' expression");
                }
            }
            else if (currentToken.Item1 == TokenType.Keyword && (Keywords)currentToken.Item2.Lexeme == Keywords.@for)
            {
                // for-loop statement

                index++;
                var forLoop = new ForLoop();

                if (index < Tokens.Count && Tokens[index].Item1 == TokenType.Identifier)
                {
                    forLoop.Ident = (string)Tokens[index].Item2.Lexeme;
                }
                else
                {
                    throw new ParserException("Expected loop counter-variable after 'for'");
                }

                index++;

                if (index == Tokens.Count || Tokens[index].Item1 != TokenType.EqualOp)
                {
                    throw new ParserException("Missing '=' in for loop");
                }

                index++;

                forLoop.From = ParseExpression();

                if (index == Tokens.Count ||
                    (Tokens[index].Item1 != TokenType.Keyword || (Tokens[index].Item1 == TokenType.Keyword &&
                                                                  (Keywords) Tokens[index].Item2.Lexeme !=
                                                                  Keywords.to)))
                {

                    throw new ParserException("Expected 'to' keyword in for-loop statement");
                }

                index++;

                forLoop.To = ParseExpression();

                if (index == Tokens.Count ||
                    (Tokens[index].Item1 != TokenType.Keyword || (Tokens[index].Item1 == TokenType.Keyword &&
                                                                  (Keywords) Tokens[index].Item2.Lexeme !=
                                                                  Keywords.@do)))
                {
                    throw new ParserException("Expected 'do' keyword in for-loop statement");
                }

                index++;

                forLoop.Body = ParseStatement();
                result = forLoop;

                if (index == Tokens.Count ||
                    (Tokens[index].Item1 != TokenType.Keyword || (Tokens[index].Item1 == TokenType.Keyword &&
                                                                  (Keywords) Tokens[index].Item2.Lexeme !=
                                                                  Keywords.end)))
                {
                    throw new ParserException("Unterminated for-loop body");
                }

                index++;
            }
            else if (Tokens[index].Item1 == TokenType.Identifier)
            {
                // variable assigning statement

                var assignStatement = new Assign { Ident = (string)Tokens[index++].Item2.Lexeme };

                if (index == Tokens.Count || Tokens[index].Item1 != TokenType.EqualOp)
                {
                    throw new ParserException("Expected '=' operator");
                }

                index++;

                var endIndex = index;

                while (Tokens[endIndex].Item1 != TokenType.Semicolon)
                {
                    endIndex++;
                }

                assignStatement.Expr = ParseExpression(endIndex);

                result = assignStatement;
            }
            else
            {
                // unknown statement
                throw new ParserException("Parse error at token " + index + ": " + Tokens[index]);
            }

            if (index < Tokens.Count && Tokens[index].Item1 == TokenType.Semicolon)
            {
                index++;

                if (index < Tokens.Count)
                {
                    if (Tokens[index].Item1 != TokenType.Keyword || (Tokens[index].Item1 == TokenType.Keyword &&
                        (Keywords) Tokens[index].Item2.Lexeme != Keywords.end))
                    {
                        var sequenceStatement = new StatementList {First = result, Second = ParseStatement()};

                        result = sequenceStatement;
                    }
                }
            }

            return result;
        }

        private Expression ParseExpression(int endIndex)
        {
            if (index + 1 == endIndex)
            {
                return ParseExpression();
            }

            // binary expression with 2 operands

            var binExpr = new BinaryExpression { Left = ParseExpression() };

            var tokenType = Tokens[index].Item1;

            if (tokenType == TokenType.AddOp)
            {
                binExpr.Op = BinaryOperator.Add;
            }
            else if (tokenType == TokenType.SubOp)
            {
                binExpr.Op = BinaryOperator.Sub;
            }
            else if (tokenType == TokenType.MulOp)
            {
                binExpr.Op = BinaryOperator.Mul;
            }
            else if (tokenType == TokenType.DivOp)
            {
                binExpr.Op = BinaryOperator.Div;
            }
            else
            {
                throw new ParserException("Expected binary operation");
            }

            index++;

            binExpr.Right = ParseExpression(endIndex);

            return binExpr;
        }

        private Expression ParseExpression()
        {
            if (index == Tokens.Count)
            {
                throw new ParserException("Unexpected EOF - Expected Expression");
            }

            if (Tokens[index].Item1 == TokenType.StringLiteral)
            {
                return new StringLiteral { Value = (string)Tokens[index++].Item2.Lexeme };
            }

            if (Tokens[index].Item1 == TokenType.IntegerLiteral)
            {
                return new IntegerLiteral { Value = (int)Tokens[index++].Item2.Lexeme };
            }

            if (Tokens[index].Item1 == TokenType.Identifier)
            {
                return new Variable { Ident = (string)Tokens[index++].Item2.Lexeme };
            }

            throw new ParserException("Expected string literal, integer literal, or variable");
        }
    }
}
