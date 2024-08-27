using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSLox.Src.Lox
{
    using static CSLox.Src.Lox.TokenType;

    internal class Scanner
    {
        readonly string source;
        readonly List<Token> tokens;
        private int start = 0;
        private int current = 0;
        private int line = 1;
        private static readonly Dictionary<string, TokenType> keywords = new Dictionary<string, TokenType>()
        {
            { "and", TokenType.AND },
            { "class", TokenType.CLASS },
            { "else", TokenType.ELSE },
            { "false", TokenType.FALSE },
            { "for", TokenType.FOR },
            { "fun", TokenType.FUN },
            { "if", TokenType.IF },
            { "nil", TokenType.NIL },
            { "or", TokenType.OR },
            { "print", TokenType.PRINT },
            { "return", TokenType.RETURN },
            { "super", TokenType.SUPER },
            { "this", TokenType.THIS },
            { "true", TokenType.TRUE },
            { "var", TokenType.VAR },
            { "while", TokenType.WHILE }
        };

        public Scanner(string source)
        {
            this.source = source;
            tokens = new List<Token>();
        }

        public List<Token> ScanTokens()
        {
            while (!IsAtEnd())
            {
                start = current;
                ScanToken();
            }

            tokens.Add(new Token(EOF, "", null, line));
            return tokens;
        }

        void ScanToken()
        {
            char c = Advance();
            switch (c)
            {
                case '(': AddToken(LEFT_PAREN); break;
                case ')': AddToken(RIGHT_PAREN); break;
                case '{': AddToken(LEFT_BRACE); break;
                case '}': AddToken(RIGHT_BRACE); break;
                case ',': AddToken(COMMA); break;
                case '.': AddToken(DOT); break;
                case '-': AddToken(MINUS); break;
                case '+': AddToken(PLUS); break;
                case ';': AddToken(SEMICOLON); break;
                case '*': AddToken(STAR); break;
                case '!':
                    AddToken(Match('=') ? BANG_EQUAL : BANG);
                    break;
                case '=':
                    AddToken(Match('=') ? EQUAL_EQUAL : EQUAL);
                    break;
                case '<':
                    AddToken(Match('=') ? LESS_EQUAL : LESS);
                    break;
                case '>':
                    AddToken(Match('=') ? GREATER_EQUAL : GREATER);
                    break;
                case '/':
                    if (Match('/'))
                    {
                        // A comment goes until the end of the line.
                        while (Peek() != '\n' && !IsAtEnd()) Advance();
                    }
                    else
                        AddToken(SLASH);
                    break;
                case '"': String(); break;
                case ' ':
                case '\r':
                case '\t':
                    // Ignore whitespace.
                    break;

                case '\n':
                    line++;
                    break;
                default:
                    if (Char.IsDigit(c))
                        Number();
                    else if (IsAlpha(c))
                        Identifier();
                    else Lox.Error(line, "Unexpected character.");
                    break;
            }
        }

        private void Identifier()
        {
            while (IsAlphaNumeric(Peek())) Advance();
            string text = source[start..current];
            
            // Try to get the type from the keywords dictionary
            if (!keywords.TryGetValue(text, out TokenType type))
                // If the keyword doesn't exist in the dictionary, it's an identifier
                type = IDENTIFIER;

            AddToken(type);
        }

        private bool IsAlpha(char c)
        {
            return (c >= 'a' && c <= 'z') ||
                   (c >= 'A' && c <= 'Z') ||
                    c == '_';
        }

        private bool IsDigit(char c) => c >= '0' && c <= '9';

        private bool IsAlphaNumeric(char c) => IsAlpha(c) || IsDigit(c);

        private void String()
        {
            while (Peek() != '"' && !IsAtEnd())
            {
                if (Peek() == '\n') line++;
                Advance();
            }

            if (IsAtEnd())
            {
                Lox.Error(line, "Unterminated string.");
                return;
            }

            // The closing "
            Advance();

            // Trim the surrounding quotes
            string value = source[(start + 1)..(current - 1)];
            AddToken(STRING, value);
        }

        private char Peek()
        {
            if (IsAtEnd()) return '\0';
            return source[current];
        }

        private char PeekNext()
        {
            if (current + 1 >= source.Length) return '\0';
            return source[current + 1];
        }

        private bool Match(char expected)
        {
            if (IsAtEnd()) return false;
            if (source[current] != expected) return false;

            current++;
            return true;
        }

        private void Number()
        {
            while (Char.IsDigit(Peek())) Advance();

            // Look for a fractional part.
            if (Peek() == '.' && Char.IsDigit(PeekNext()))
            {
                // Consume the "."
                Advance();

                while (Char.IsDigit(Peek())) Advance();
            }

            AddToken(NUMBER, Double.Parse(source[start..current]));
        }

        private char Advance()
        {
            return source[current++];
        }

        private void AddToken(TokenType type)
        {
            AddToken(type, null);
        }

        private void AddToken(TokenType type, object? literal)
        {
            string text = source[start..current];
            tokens.Add(new Token(type, text, literal, line));
        }

        bool IsAtEnd() => current >= source.Length;
    }
}
