using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace iCompiler.Helpers
{
    using TokenClass = Tuple<TokenType, LexemeClass>;

    public class LexicalAnalysis
    {
        private List<TokenClass> result;

        public List<TokenClass> Tokens
        {
            get { return result; }
        }

        public LexicalAnalysis(TextReader input)
        {
            result = new List<TokenClass>();
            Analyse(input);
        }

        private void Analyse(TextReader input)
        {
            while (input.Peek() != -1)
            {
                var ch = (char)input.Peek();

                if (char.IsWhiteSpace(ch))
                {
                    // ignore whitespaces
                    input.Read();
                }
                else if (char.IsLetter(ch) || ch == '_')
                {
                    // keyword or identifier

                    var name = new StringBuilder();

                    while (char.IsLetter(ch) || char.IsDigit(ch) || ch == '_')
                    {
                        name.Append(ch);
                        input.Read();

                        if (input.Peek() == -1)
                        {
                            break;
                        }

                        ch = (char)input.Peek();
                    }

                    bool keywordFlag = false;
                    foreach (var keyword in Enum.GetValues(typeof(Keywords)))
                    {
                        if (name.ToString() == keyword.ToString())
                        {
                            this.result.Add(new TokenClass(TokenType.Keyword, new LexemeClass { Lexeme = keyword }));
                            keywordFlag = true;
                        }
                    }

                    if (!keywordFlag)
                    {
                        this.result.Add(new TokenClass(TokenType.Identifier, new LexemeClass { Lexeme = name.ToString() }));
                    }

                }
                else if (ch == '"')
                {
                    // string literal
                    var literal = new StringBuilder();

                    input.Read(); // skip the '"'

                    if (input.Peek() == -1)
                    {
                        throw new LexicalAnalysisException("Unterminated string literal");
                    }

                    while ((ch = (char)input.Peek()) != '"')
                    {
                        literal.Append(ch);
                        input.Read();

                        if (input.Peek() == -1)
                        {
                            throw new LexicalAnalysisException("Unterminated string literal");
                        }
                    }

                    // skip the terminating '"'
                    input.Read();

                    this.result.Add(new TokenClass(TokenType.StringLiteral, new LexemeClass { Lexeme = literal.ToString() }));
                }
                else if (char.IsDigit(ch))
                {
                    // a number literal

                    var number = new StringBuilder();

                    while (char.IsDigit(ch))
                    {
                        number.Append(ch);
                        input.Read();

                        if (input.Peek() == -1)
                        {
                            break;
                        }

                        ch = (char)input.Peek();
                    }

                    this.result.Add(new TokenClass(TokenType.IntegerLiteral, new LexemeClass { Lexeme = int.Parse(number.ToString()) }));
                }
                else
                    switch (ch)
                    {
                        case '+':
                            input.Read();
                            this.result.Add(new TokenClass(TokenType.AddOp, new LexemeClass { Lexeme = ch }));
                            break;

                        case '-':
                            input.Read();
                            this.result.Add(new TokenClass(TokenType.SubOp, new LexemeClass { Lexeme = ch }));
                            break;

                        case '*':
                            input.Read();
                            this.result.Add(new TokenClass(TokenType.MulOp, new LexemeClass { Lexeme = ch }));
                            break;

                        case '/':
                            input.Read();
                            this.result.Add(new TokenClass(TokenType.DivOp, new LexemeClass { Lexeme = ch }));
                            break;

                        case '=':
                            input.Read();
                            this.result.Add(new TokenClass(TokenType.EqualOp, new LexemeClass { Lexeme = ch }));
                            break;

                        case ';':
                            input.Read();
                            this.result.Add(new TokenClass(TokenType.Semicolon, new LexemeClass { Lexeme = ch }));
                            break;

                        default:
                            throw new LexicalAnalysisException("Encountered unrecognized character '" + ch + "'");
                    }

            }
        }
    }


    #region Token types

    public enum TokenType
    {
        AddOp,
        SubOp,
        MulOp,
        DivOp,
        Semicolon,
        EqualOp,
        Keyword,
        Identifier,
        StringLiteral,
        IntegerLiteral,
    }

    public class LexemeClass
    {
        public object Lexeme { get; set; }
    }

    public enum Keywords
    {
        print,
        var,
        read_integer,
        @for,
        to,
        @do,
        end
    }

    #endregion
}
