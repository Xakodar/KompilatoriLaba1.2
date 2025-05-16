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



