using System;
using System.Collections.Generic;

namespace KompilatoriLaba1
{
    public class Lexer
    {
        public enum TokenType { Number, Identifier, LParen, RParen, Unknown, EOF }

        public class Token
        {
            public TokenType Type { get; }
            public string Lexeme { get; }
            public int Position { get; }
            public Token(TokenType type, string lexeme, int position)
            {
                Type = type;
                Lexeme = lexeme;
                Position = position;
            }
            public override string ToString() => $"{Type}('{Lexeme}') at {Position}";
        }

        public class LexError
        {
            public string Message { get; }
            public int Position { get; }
            public LexError(string message, int position)
            {
                Message = message;
                Position = position;
            }
            public override string ToString() => $"{Message} at {Position}";
        }

        public static void Tokenize(string input, out List<Token> tokens, out List<LexError> errors)
        {
            tokens = new List<Token>();
            errors = new List<LexError>();
            int i = 0, length = input.Length;

            while (i < length)
            {
                char c = input[i];

                // Пробелы
                if (char.IsWhiteSpace(c))
                {
                    i++;
                    continue;
                }

                // Числа
                if (char.IsDigit(c))
                {
                    int start = i;
                    while (i < length && char.IsDigit(input[i])) i++;
                    string lex = input.Substring(start, i - start);
                    tokens.Add(new Token(TokenType.Number, lex, start));
                    continue;
                }

                // Идентификаторы
                if (char.IsLetter(c) || c == '_')
                {
                    int start = i;
                    while (i < length && (char.IsLetterOrDigit(input[i]) || input[i] == '_'))
                        i++;
                    string lex = input.Substring(start, i - start);
                    tokens.Add(new Token(TokenType.Identifier, lex, start));
                    continue;
                }

                // Скобки
                if (c == '(')
                {
                    tokens.Add(new Token(TokenType.LParen, "(", i));
                    i++;
                    continue;
                }
                if (c == ')')
                {
                    tokens.Add(new Token(TokenType.RParen, ")", i));
                    i++;
                    continue;
                }

                // НЕРАСПОЗНАННЫЙ символ → лексическая ошибка и токен Unknown
                errors.Add(new LexError($"Нераспознанный символ '{c}'", i));
                tokens.Add(new Token(TokenType.Unknown, c.ToString(), i));
                i++;
            }

            // EOF
            tokens.Add(new Token(TokenType.EOF, string.Empty, length));
        }
    }
}
//// Lexer.cs
//using System;
//using System.Collections.Generic;

//namespace KompilatoriLaba1
//{
//    public class Lexer
//    {
//        public enum TokenType { Number, Identifier, LParen, RParen, EOF }

//        public class Token
//        {
//            public TokenType Type { get; }
//            public string Lexeme { get; }
//            public int Position { get; }
//            public Token(TokenType type, string lexeme, int position)
//            {
//                Type = type;
//                Lexeme = lexeme;
//                Position = position;
//            }
//            public override string ToString() => $"{Type}('{Lexeme}') at {Position}";
//        }

//        public class LexError
//        {
//            public string Message { get; }
//            public int Position { get; }
//            public LexError(string message, int position)
//            {
//                Message = message;
//                Position = position;
//            }
//            public override string ToString() => $"{Message} at {Position}";
//        }

//        public static void Tokenize(string input, out List<Token> tokens, out List<LexError> errors)
//        {
//            tokens = new List<Token>();
//            errors = new List<LexError>();
//            int i = 0, length = input.Length;

//            while (i < length)
//            {
//                char c = input[i];

//                // Пробелы
//                if (char.IsWhiteSpace(c))
//                {
//                    i++;
//                    continue;
//                }

//                // Числа
//                if (char.IsDigit(c))
//                {
//                    int start = i;
//                    while (i < length && char.IsDigit(input[i]))
//                        i++;
//                    string lex = input.Substring(start, i - start);
//                    tokens.Add(new Token(TokenType.Number, lex, start));
//                    continue;
//                }

//                // Идентификаторы
//                if (char.IsLetter(c) || c == '_')
//                {
//                    int start = i;
//                    while (i < length && (char.IsLetterOrDigit(input[i]) || input[i] == '_'))
//                        i++;
//                    string lex = input.Substring(start, i - start);
//                    tokens.Add(new Token(TokenType.Identifier, lex, start));
//                    continue;
//                }

//                // Скобки
//                if (c == '(')
//                {
//                    tokens.Add(new Token(TokenType.LParen, "(", i));
//                    i++;
//                    continue;
//                }
//                if (c == ')')
//                {
//                    tokens.Add(new Token(TokenType.RParen, ")", i));
//                    i++;
//                    continue;
//                }

//                // Неизвестный символ
//                errors.Add(new LexError($"Нераспознанный символ '{c}'", i));
//                i++;
//            }

//            // EOF
//            tokens.Add(new Token(TokenType.EOF, string.Empty, length));
//        }
//    }
//}
