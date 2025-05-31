// Lexer.cs
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

            public override string ToString()
            {
                string name;
                switch (Type)
                {
                    case TokenType.Number:
                        name = "Число";
                        break;
                    case TokenType.Identifier:
                        name = "Идентификатор";
                        break;
                    case TokenType.LParen:
                        name = "Открывающая скобка";
                        break;
                    case TokenType.RParen:
                        name = "Закрывающая скобка";
                        break;
                    case TokenType.Unknown:
                        name = "Неизвестный фрагмент";
                        break;
                    case TokenType.EOF:
                        name = "Конец ввода";
                        break;
                    default:
                        name = Type.ToString();
                        break;
                }
                return string.Format("{0}('{1}') на позиции {2}", name, Lexeme, Position);
            }
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

            public override string ToString()
            {
                return string.Format("{0} на позиции {1}", Message, Position);
            }
        }

        public static void Tokenize(string input, out List<Token> tokens, out List<LexError> errors)
        {
            tokens = new List<Token>();
            errors = new List<LexError>();
            // Используем стек для проверки парности скобок
            var lparenStack = new Stack<int>();

            int i = 0;
            int length = input.Length;

            while (i < length)
            {
                char c = input[i];

                // 1) Пробельные символы — пропускаем
                if (char.IsWhiteSpace(c))
                {
                    i++;
                    continue;
                }

                // 2) Число: [0-9]+
                if (char.IsDigit(c))
                {
                    int start = i;
                    while (i < length && char.IsDigit(input[i]))
                        i++;
                    string lex = input.Substring(start, i - start);
                    tokens.Add(new Token(TokenType.Number, lex, start));
                    continue;
                }

                // 3) Идентификатор: [A-Za-z_][A-Za-z0-9_]*
                if ((c >= 'A' && c <= 'Z') ||
                    (c >= 'a' && c <= 'z') ||
                     c == '_')
                {
                    int start = i;
                    i++;
                    while (i < length &&
                           (
                               (input[i] >= 'A' && input[i] <= 'Z') ||
                               (input[i] >= 'a' && input[i] <= 'z') ||
                                input[i] == '_' ||
                                char.IsDigit(input[i])
                           ))
                    {
                        i++;
                    }
                    string lex = input.Substring(start, i - start);
                    tokens.Add(new Token(TokenType.Identifier, lex, start));
                    continue;
                }

                // 4) Открывающая скобка '('
                if (c == '(')
                {
                    tokens.Add(new Token(TokenType.LParen, "(", i));
                    // запоминаем позицию, чтобы позже проверить, найдётся ли парная ')'
                    lparenStack.Push(i);
                    i++;
                    continue;
                }

                // 5) Закрывающая скобка ')'
                if (c == ')')
                {
                    // если стек пуст, значит нет соответствующей открывающей
                    if (lparenStack.Count == 0)
                    {
                        errors.Add(new LexError("Нет открывающейся скобки", i));
                    }
                    else
                    {
                        // нашли соответствующую '(', выбрасываем её из стека
                        lparenStack.Pop();
                    }
                    tokens.Add(new Token(TokenType.RParen, ")", i));
                    i++;
                    continue;
                }

                // 6) Последовательность русских букв (Cyrillic) — склеиваем в один Unknown-токен
                if (IsCyrillicLetter(c))
                {
                    int start = i;
                    while (i < length && IsCyrillicLetter(input[i]))
                        i++;
                    string lex = input.Substring(start, i - start);
                    errors.Add(new LexError($"Нераспознанный фрагмент '{lex}'", start));
                    tokens.Add(new Token(TokenType.Unknown, lex, start));
                    continue;
                }

                // 7) Любой другой одиночный символ — тоже Unknown
                errors.Add(new LexError($"Нераспознанный символ '{c}'", i));
                tokens.Add(new Token(TokenType.Unknown, c.ToString(), i));
                i++;
            }

            // 8) Проверяем, остались ли в стеке незакрытые '('
            while (lparenStack.Count > 0)
            {
                int pos = lparenStack.Pop();
                errors.Add(new LexError("Нет закрывающейся скобки", pos));
            }

            // 9) Добавляем EOF
            tokens.Add(new Token(TokenType.EOF, string.Empty, length));
        }

        // Вспомогательный метод для проверки кириллических букв
        private static bool IsCyrillicLetter(char ch)
        {
            // Основные диапазоны Unicode для кириллицы: U+0400–U+04FF и U+0500–U+052F (расширенная)
            return (ch >= '\u0400' && ch <= '\u04FF') ||
                   (ch >= '\u0500' && ch <= '\u052F');
        }
    }
}


//using System;
//using System.Collections.Generic;

//namespace KompilatoriLaba1
//{
//    public class Lexer
//    {
//        public enum TokenType { Number, Identifier, LParen, RParen, Unknown, EOF }

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

//            public override string ToString()
//            {
//                string name;
//                switch (Type)
//                {
//                    case TokenType.Number:
//                        name = "Число";
//                        break;
//                    case TokenType.Identifier:
//                        name = "Идентификатор";
//                        break;
//                    case TokenType.LParen:
//                        name = "Открывающая скобка";
//                        break;
//                    case TokenType.RParen:
//                        name = "Закрывающая скобка";
//                        break;
//                    case TokenType.Unknown:
//                        name = "Неизвестный символ";
//                        break;
//                    case TokenType.EOF:
//                        name = "Конец ввода";
//                        break;
//                    default:
//                        name = Type.ToString();
//                        break;
//                }
//                return string.Format("{0}('{1}') на позиции {2}", name, Lexeme, Position);
//            }
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

//            public override string ToString()
//            {
//                return string.Format("{0} на позиции {1}", Message, Position);
//            }
//        }

//        public static void Tokenize(string input, out List<Token> tokens, out List<LexError> errors)
//        {
//            tokens = new List<Token>();
//            errors = new List<LexError>();
//            int i = 0, length = input.Length;

//            while (i < length)
//            {
//                char c = input[i];

//                // Пробельные символы — пропускаем
//                if (char.IsWhiteSpace(c))
//                {
//                    i++;
//                    continue;
//                }

//                // Число: [0-9]+
//                if (char.IsDigit(c))
//                {
//                    int start = i;
//                    while (i < length && char.IsDigit(input[i]))
//                        i++;
//                    string lex = input.Substring(start, i - start);
//                    tokens.Add(new Token(TokenType.Number, lex, start));
//                    continue;
//                }

//                // Идентификатор: [A-Za-z_][A-Za-z0-9_]* 
//                if ((c >= 'A' && c <= 'Z') ||
//                    (c >= 'a' && c <= 'z') ||
//                     c == '_')
//                {
//                    int start = i;
//                    i++;
//                    while (i < length &&
//                           (
//                               (input[i] >= 'A' && input[i] <= 'Z') ||
//                               (input[i] >= 'a' && input[i] <= 'z') ||
//                                input[i] == '_' ||
//                                char.IsDigit(input[i])
//                           ))
//                    {
//                        i++;
//                    }
//                    string lex = input.Substring(start, i - start);
//                    tokens.Add(new Token(TokenType.Identifier, lex, start));
//                    continue;
//                }

//                // Открывающая скобка '('
//                if (c == '(')
//                {
//                    tokens.Add(new Token(TokenType.LParen, "(", i));
//                    i++;
//                    continue;
//                }

//                // Закрывающая скобка ')'
//                if (c == ')')
//                {
//                    tokens.Add(new Token(TokenType.RParen, ")", i));
//                    i++;
//                    continue;
//                }

//                // Любая другая буква (включая русские), символ или пунктуация — лексическая ошибка
//                errors.Add(new LexError($"Нераспознанный символ '{c}'", i));
//                tokens.Add(new Token(TokenType.Unknown, c.ToString(), i));
//                i++;
//            }

//            // EOF
//            tokens.Add(new Token(TokenType.EOF, string.Empty, length));
//        }
//    }
//}

//// Lexer.cs
//using System;
//using System.Collections.Generic;

//namespace KompilatoriLaba1
//{
//    public class Lexer
//    {
//        public enum TokenType { Number, Identifier, LParen, RParen, Unknown, EOF }

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

//            public override string ToString()
//            {
//                string name;
//                switch (Type)
//                {
//                    case TokenType.Number:
//                        name = "Число";
//                        break;
//                    case TokenType.Identifier:
//                        name = "Идентификатор";
//                        break;
//                    case TokenType.LParen:
//                        name = "Открывающая скобка";
//                        break;
//                    case TokenType.RParen:
//                        name = "Закрывающая скобка";
//                        break;
//                    case TokenType.Unknown:
//                        name = "Неизвестный символ";
//                        break;
//                    case TokenType.EOF:
//                        name = "Конец ввода";
//                        break;
//                    default:
//                        name = Type.ToString();
//                        break;
//                }
//                return string.Format("{0}('{1}') на позиции {2}", name, Lexeme, Position);
//            }
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

//            public override string ToString()
//            {
//                return string.Format("{0} на позиции {1}", Message, Position);
//            }
//        }

//        public static void Tokenize(string input, out List<Token> tokens, out List<LexError> errors)
//        {
//            tokens = new List<Token>();
//            errors = new List<LexError>();
//            int i = 0, length = input.Length;

//            while (i < length)
//            {
//                char c = input[i];

//                // Пробельные символы — пропускаем
//                if (char.IsWhiteSpace(c))
//                {
//                    i++;
//                    continue;
//                }

//                // Число
//                if (char.IsDigit(c))
//                {
//                    int start = i;
//                    while (i < length && char.IsDigit(input[i]))
//                        i++;
//                    string lex = input.Substring(start, i - start);
//                    tokens.Add(new Token(TokenType.Number, lex, start));
//                    continue;
//                }

//                // Идентификатор
//                if (char.IsLetter(c) || c == '_')
//                {
//                    int start = i;
//                    while (i < length && (char.IsLetterOrDigit(input[i]) || input[i] == '_'))
//                        i++;
//                    string lex = input.Substring(start, i - start);
//                    tokens.Add(new Token(TokenType.Identifier, lex, start));
//                    continue;
//                }

//                // Открывающая скобка
//                if (c == '(')
//                {
//                    tokens.Add(new Token(TokenType.LParen, "(", i));
//                    i++;
//                    continue;
//                }

//                // Закрывающая скобка
//                if (c == ')')
//                {
//                    tokens.Add(new Token(TokenType.RParen, ")", i));
//                    i++;
//                    continue;
//                }

//                // Неизвестный символ
//                errors.Add(new LexError($"Нераспознанный символ '{c}'", i));
//                tokens.Add(new Token(TokenType.Unknown, c.ToString(), i));
//                i++;
//            }

//            // Конец ввода
//            tokens.Add(new Token(TokenType.EOF, string.Empty, length));
//        }
//    }
//}
//using System;
//using System.Collections.Generic;

//namespace KompilatoriLaba1
//{
//    public class Lexer
//    {
//        public enum TokenType { Number, Identifier, LParen, RParen, Unknown, EOF }

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
//                    while (i < length && char.IsDigit(input[i])) i++;
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

//                // НЕРАСПОЗНАННЫЙ символ → лексическая ошибка и токен Unknown
//                errors.Add(new LexError($"Нераспознанный символ '{c}'", i));
//                tokens.Add(new Token(TokenType.Unknown, c.ToString(), i));
//                i++;
//            }

//            // EOF
//            tokens.Add(new Token(TokenType.EOF, string.Empty, length));
//        }
//    }
//}
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
