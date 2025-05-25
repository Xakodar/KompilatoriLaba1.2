using System;
using System.Collections.Generic;

namespace KompilatoriLaba1
{
    public class Parser
    {
        private readonly List<Lexer.Token> tokens;
        private int position;
        private readonly Action<string> log;

        public Parser(List<Lexer.Token> tokens, Action<string> logCallback)
        {
            this.tokens = new List<Lexer.Token>(tokens);
            this.log = logCallback;
            this.position = 0;

            // Гарантируем EOF в конце
            if (this.tokens.Count == 0 ||
                this.tokens[this.tokens.Count - 1].Type != Lexer.TokenType.EOF)
            {
                int pos = this.tokens.Count > 0
                    ? this.tokens[this.tokens.Count - 1].Position
                    : 0;
                this.tokens.Add(new Lexer.Token(Lexer.TokenType.EOF, "", pos));
            }
        }

        private Lexer.Token Peek() => tokens[position];
        private Lexer.Token Consume() => tokens[position++];

        public void Parse()
        {
            log("=== Начало разбора ===");
            ParseLexpSeq();
            if (Peek().Type != Lexer.TokenType.EOF)
                log($"[Ошибка] неожиданный токен {Peek()}");
            log("=== Разбор завершён ===");
        }

        // Lexp-seq ::= Lexp { Lexp }
        private void ParseLexpSeq()
        {
            log("→ ParseLexpSeq");
            ParseLexp();
            while (IsStartOfLexp(Peek().Type))
                ParseLexp();
        }

        // Теперь Unknown тоже считается «стартом» Lexp
        private bool IsStartOfLexp(Lexer.TokenType t) =>
               t == Lexer.TokenType.Number
            || t == Lexer.TokenType.Identifier
            || t == Lexer.TokenType.LParen
            || t == Lexer.TokenType.Unknown;

        // Lexp ::= Atom | List
        private void ParseLexp()
        {
            log("  → ParseLexp");
            var t = Peek();
            if (t.Type == Lexer.TokenType.Number
             || t.Type == Lexer.TokenType.Identifier)
                ParseAtom();
            else if (t.Type == Lexer.TokenType.LParen)
                ParseList();
            else
            {
                // сюда попадёт Unknown
                log($"  [Ошибка] ожидался Atom или List, но {t}");
                Consume(); // пропускаем ошибочный токен
            }
        }

        private void ParseAtom()
        {
            log("    → ParseAtom");
            var t = Peek();
            if (t.Type == Lexer.TokenType.Number
             || t.Type == Lexer.TokenType.Identifier)
            {
                log($"      matched {t}");
                Consume();
            }
            else
            {
                log($"    [Ошибка] ожидался Atom, но {t}");
                Consume();
            }
        }

        private void ParseList()
        {
            log("    → ParseList");
            if (Peek().Type == Lexer.TokenType.LParen)
            {
                log($"      matched {Peek()}");
                Consume();
            }
            else
            {
                log($"    [Ошибка] ожидалась '(', но {Peek()}");
            }

            // Разбираем всё, что похоже на Lexp, включая Unknown
            ParseLexpSeq();

            if (Peek().Type == Lexer.TokenType.RParen)
            {
                log($"      matched {Peek()}");
                Consume();
            }
            else
            {
                log($"    [Ошибка] ожидалась ')', но {Peek()}");
            }
        }
    }
}

//// Parser.cs
//using System;
//using System.Collections.Generic;

//namespace KompilatoriLaba1
//{
//    public class Parser
//    {
//        private readonly List<Lexer.Token> tokens;
//        private int position;
//        private readonly Action<string> log;

//        public Parser(List<Lexer.Token> tokens, Action<string> logCallback)
//        {
//            // Копируем, чтобы не портить исходный список
//            this.tokens = new List<Lexer.Token>(tokens);
//            this.log = logCallback;
//            this.position = 0;

//            // Убедимся, что в конце есть EOF
//            if (this.tokens.Count == 0 ||
//                this.tokens[this.tokens.Count - 1].Type != Lexer.TokenType.EOF)
//            {
//                int pos = this.tokens.Count > 0
//                    ? this.tokens[this.tokens.Count - 1].Position
//                    : 0;
//                this.tokens.Add(new Lexer.Token(Lexer.TokenType.EOF, string.Empty, pos));
//            }
//        }

//        private Lexer.Token Peek() => tokens[position];
//        private Lexer.Token Consume() => tokens[position++];

//        public void Parse()
//        {
//            log("=== Начало разбора ===");
//            ParseLexpSeq();
//            if (Peek().Type != Lexer.TokenType.EOF)
//                log($"[Ошибка] неожиданный токен {Peek()}");
//            log("=== Разбор завершён ===");
//        }

//        // Lexp-seq ::= Lexp { Lexp }
//        private void ParseLexpSeq()
//        {
//            log("→ ParseLexpSeq");
//            ParseLexp();
//            while (IsStartOfLexp(Peek().Type))
//                ParseLexp();
//        }

//        private bool IsStartOfLexp(Lexer.TokenType type) =>
//            type == Lexer.TokenType.Number ||
//            type == Lexer.TokenType.Identifier ||
//            type == Lexer.TokenType.LParen;

//        // Lexp ::= Atom | List
//        private void ParseLexp()
//        {
//            log("  → ParseLexp");
//            var t = Peek();
//            if (t.Type == Lexer.TokenType.Number || t.Type == Lexer.TokenType.Identifier)
//                ParseAtom();
//            else if (t.Type == Lexer.TokenType.LParen)
//                ParseList();
//            else
//            {
//                log($"  [Ошибка] ожидался Atom или List, но {t}");
//                Consume(); // восстановление
//            }
//        }

//        // Atom ::= number | identifier
//        private void ParseAtom()
//        {
//            log("    → ParseAtom");
//            var t = Peek();
//            if (t.Type == Lexer.TokenType.Number || t.Type == Lexer.TokenType.Identifier)
//            {
//                log($"      matched {t}");
//                Consume();
//            }
//            else
//            {
//                log($"    [Ошибка] ожидался Atom, но {t}");
//                Consume();
//            }
//        }

//        // List ::= '(' Lexp-seq ')'
//        private void ParseList()
//        {
//            log("    → ParseList");
//            // '('
//            if (Peek().Type == Lexer.TokenType.LParen)
//            {
//                log($"      matched {Peek()}");
//                Consume();
//            }
//            else
//                log($"    [Ошибка] ожидалась '(', но {Peek()}");

//            // внутренности
//            ParseLexpSeq();

//            // ')'
//            if (Peek().Type == Lexer.TokenType.RParen)
//            {
//                log($"      matched {Peek()}");
//                Consume();
//            }
//            else
//                log($"    [Ошибка] ожидалась ')', но {Peek()}");
//        }
//    }
//}

//// Parser.cs
//using System;
//using System.Collections.Generic;
//using System.Text.RegularExpressions;

//namespace KompilatoriLaba1
//{
//    public class Parser
//    {
//        public enum TokenType { Number, Identifier, LParen, RParen, EOF }

//        public struct Token
//        {
//            public TokenType Type;
//            public string Text;
//            public Token(TokenType type, string text)
//            {
//                Type = type;
//                Text = text;
//            }
//            public override string ToString() => $"{Type}('{Text}')";
//        }

//        private readonly List<Token> tokens;
//        private int position;
//        private readonly Action<string> log;

//        public Parser(string input, Action<string> logCallback)
//        {
//            tokens = Tokenize(input);
//            tokens.Add(new Token(TokenType.EOF, ""));
//            log = logCallback;
//        }

//        private static List<Token> Tokenize(string input)
//        {
//            var list = new List<Token>();
//            var pattern = @"\s*(\d+|[A-Za-z_]\w*|\(|\))";
//            foreach (Match m in Regex.Matches(input, pattern))
//            {
//                var t = m.Groups[1].Value;
//                if (Regex.IsMatch(t, "^\\d+$")) list.Add(new Token(TokenType.Number, t));
//                else if (Regex.IsMatch(t, "^[A-Za-z_]\\w*$")) list.Add(new Token(TokenType.Identifier, t));
//                else if (t == "(") list.Add(new Token(TokenType.LParen, t));
//                else if (t == ")") list.Add(new Token(TokenType.RParen, t));
//            }
//            return list;
//        }

//        private Token Peek() => tokens[position];
//        private Token Consume()
//        {
//            var t = tokens[position];
//            position++;
//            return t;
//        }

//        public void Parse()
//        {
//            log("=== Начало разбора ===");
//            ParseLexpSeq();
//            if (Peek().Type != TokenType.EOF)
//                log($"Ошибка: неожиданный токен {Peek()}");
//            log("=== Разбор завершён ===");
//        }

//        private void ParseLexpSeq()
//        {
//            log("→ ParseLexpSeq");
//            ParseLexp();
//            while (Peek().Type == TokenType.Number || Peek().Type == TokenType.Identifier || Peek().Type == TokenType.LParen)
//            {
//                ParseLexp();
//            }
//        }

//        private void ParseLexp()
//        {
//            log("  → ParseLexp");
//            var t = Peek().Type;
//            if (t == TokenType.Number || t == TokenType.Identifier)
//                ParseAtom();
//            else if (t == TokenType.LParen)
//                ParseList();
//            else
//            {
//                log($"  [Ошибка] ожидался Atom или List, но {Peek()}");
//                Consume();
//            }
//        }

//        private void ParseAtom()
//        {
//            log("    → ParseAtom");
//            var tok = Peek();
//            if (tok.Type == TokenType.Number || tok.Type == TokenType.Identifier)
//            {
//                log($"      matched {tok}");
//                Consume();
//            }
//            else
//            {
//                log($"    [Ошибка] ожидался Atom, но {tok}");
//                Consume();
//            }
//        }

//        private void ParseList()
//        {
//            log("    → ParseList");
//            if (Peek().Type == TokenType.LParen)
//            {
//                log($"      matched {Peek()}");
//                Consume();
//            }
//            else
//            {
//                log($"    [Ошибка] ожидалась '(', но {Peek()}");
//            }

//            ParseLexpSeq();

//            if (Peek().Type == TokenType.RParen)
//            {
//                log($"      matched {Peek()}");
//                Consume();
//            }
//            else
//            {
//                log($"    [Ошибка] ожидалась ')', но {Peek()}");
//            }
//        }
//    }
//}