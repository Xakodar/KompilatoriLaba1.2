## 6 лабораторная работа.

Решения задач:

1. Построить РВ, описывающее пароль (набор строчных и
 прописных русских букв, цифр и символов).

private static readonly Regex rxWords = new Regex(@"[А-Яа-я0-9\p{P}\p{S}]+", RegexOptions.Compiled);

2.  Построить РВ для поиска ИНН физических и юридических лиц.

private static readonly Regex rxInn = new Regex(@"\b(?:\d{10}|\d{12})\b", RegexOptions.Compiled);

3.  Построить РВ, описывающее время. Формат: ЧЧ:ММ:СС в
 24-часовом формате с необязательным ведущим 0.

private static readonly Regex rxTime = new Regex(@"\b(?:[01]?\d|2[0-3]):(?:[0-5]?\d):(?:[0-5]?\d)\b", RegexOptions.Compiled);

Тестовые примеры поиска подстрок:

Пример 1:

![image](https://github.com/user-attachments/assets/ee8a256e-7a41-4262-8920-08bbcdbc8673)


Пример 2:

![image](https://github.com/user-attachments/assets/f5b3c4bb-308f-498f-99ab-1c0515cb49bb)

Пример 3:

![image](https://github.com/user-attachments/assets/1ca1fad9-86ab-4f03-b711-404c441cf9e1)



## 8 лабораторная работа.
Грамматика:

G[Lexp-seq]: 
1. Lexp-seq ::= Lexp-seq Lexp | Lexp  
2. Lexp ::= Atom | List  
3. Atom ::= „number‟ | „identifier‟  
4.List ::= „(‟ Lexp-seq „)‟

Примечание: данная грамматика порождает цепочки упрощенного 
языка Lisp.
Язык:
C#, Windows Forms (.NET Framework).

Классификация грамматики:

Контекстно-свободная грамматика (КС).

Подходит для разбора методом LL(1) (рекурсивный спуск без левой рекурсии).

Схема вызова функций:

![image](https://github.com/user-attachments/assets/25bdc946-fcea-47c4-9202-d7f5fa893393)


Тестовые примеры:
Пример 1:

![image](https://github.com/user-attachments/assets/a7aefa71-3815-4c66-9d76-e3d43da5a33c)


Пример 2:

![image](https://github.com/user-attachments/assets/5c5372a0-f5b1-4ebf-af43-80aa7ea6ba0f)


Пример 3:
![image](https://github.com/user-attachments/assets/db5464a3-8cd8-4410-ad8e-69026778d825)


## 8 лабораторная.

### Грамматика:

```
Для грамматики G[Lexp-seq] разработать и реализовать алгоритм 
анализа на основе метода рекурсивного спуска. 
1. Lexp-seq ::= Lexp-seq Lexp | Lexp  
2. Lexp ::= Atom | List  
3. Atom ::= „number‟ | „identifier‟  
4. List ::= „(‟ Lexp-seq „)‟ 
Примечание: данная грамматика порождает цепочки упрощенного языка Lisp.
```

### Язык:

C#, Windows Forms (.NET Framework).


### Классификация грамматики:

Контекстно-свободная грамматика (КС).

Подходит для разбора методом LL(1) (рекурсивный спуск без левой рекурсии).


### Схема вызова функций:

```
Lexer
├── TokenType (enum)
│   ├── Number
│   ├── Identifier
│   ├── LParen
│   ├── RParen
│   ├── Unknown
│   └── EOF
│
├── Token (class)
│   ├── Constructor(TokenType, string, int)
│   └── ToString()
│
├── LexError (class)
│   ├── Constructor(string, int)
│   └── ToString()
│
└── Static Methods
    ├── Tokenize(string, out List<Token>, out List<LexError>)
    │   ├── Handles whitespace
    │   ├── Processes numbers ([0-9]+)
    │   ├── Processes identifiers ([A-Za-z_][A-Za-z0-9_]*)
    │   ├── Processes parentheses (with stack validation)
    │   │   ├── '(' → LParen (pushes to stack)
    │   │   └── ')' → RParen (pops from stack)
    │   ├── Processes Cyrillic text (IsCyrillicLetter)
    │   ├── Processes unknown characters
    │   └── Adds EOF token
    │
    └── IsCyrillicLetter(char) (helper)
        └── Checks Unicode ranges:
            ├── U+0400–U+04FF
            └── U+0500–U+052F
```

### Тестовые примеры:

Пример 1:

![image](https://github.com/user-attachments/assets/c0511faf-c94a-4c31-a08c-e5180f1eb0e3)


Пример 2:

![image](https://github.com/user-attachments/assets/cdbe9fa6-afda-478e-a5e8-bcec45016238)


Пример 3:

![image](https://github.com/user-attachments/assets/37ef6ee4-fffe-483d-ab3d-5ab8a0f4af1c)
