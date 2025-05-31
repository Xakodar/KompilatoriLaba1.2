using System;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace KompilatoriLaba1
{
    public partial class Compiler : Form
    {
        private bool isTextChanged = false;
        private string currentFilePath = string.Empty;

        // Предкомпилированные регулярные выражения (оставлены для сравнения/других кнопок)
        private static readonly Regex rxWords = new Regex(@"[А-Яа-я0-9\p{P}\p{S}]+", RegexOptions.Compiled);
        private static readonly Regex rxInn = new Regex(@"\b(?:\d{10}|\d{12})\b", RegexOptions.Compiled);
        private static readonly Regex rxTime = new Regex(@"\b(?:[01]?\d|2[0-3]):(?:[0-5]?\d):(?:[0-5]?\d)\b", RegexOptions.Compiled);

        public Compiler()
        {
            InitializeComponent();
        }

        private void Compiler_Load(object sender, EventArgs e)
        {
            // Настройка колонок dataGridView1
            dataGridView1.Columns.Clear();
            dataGridView1.Columns.Add("Тип", "Тип");
            dataGridView1.Columns.Add("Строка", "Строка");
            dataGridView1.Columns.Add("Позиция", "Позиция");
            dataGridView1.AllowUserToAddRows = false;
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            isTextChanged = true;
        }

        private bool CheckForUnsavedChanges()
        {
            if (!isTextChanged) return true;
            var result = MessageBox.Show(
                "Сохранить изменения перед продолжением?",
                "Несохранённые изменения",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
                return SaveFile();
            if (result == DialogResult.No)
                return true;

            return false; // Пользователь нажал «Отмена»
        }

        private bool SaveFile()
        {
            if (string.IsNullOrEmpty(currentFilePath))
                return SaveFileAs();

            try
            {
                File.WriteAllText(currentFilePath, richTextBox1.Text);
                isTextChanged = false;
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private bool SaveFileAs()
        {
            using (var dlg = new SaveFileDialog
            {
                Filter = "Text Files|*.txt",
                Title = "Сохранить файл"
            })
            {
                if (dlg.ShowDialog() != DialogResult.OK)
                    return false;

                currentFilePath = dlg.FileName;
                return SaveFile();
            }
        }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!CheckForUnsavedChanges()) return;

            using (var dlg = new OpenFileDialog
            {
                Filter = "Text Files|*.txt",
                Title = "Открыть текстовый файл"
            })
            {
                if (dlg.ShowDialog() != DialogResult.OK) return;
                try
                {
                    currentFilePath = dlg.FileName;
                    richTextBox1.Text = File.ReadAllText(currentFilePath);
                    isTextChanged = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при открытии файла: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void создатьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!CheckForUnsavedChanges()) return;

            using (var dlg = new SaveFileDialog
            {
                Filter = "Text Files|*.txt",
                Title = "Создать новый текстовый файл",
                FileName = "Новый файл.txt"
            })
            {
                if (dlg.ShowDialog() != DialogResult.OK) return;
                try
                {
                    File.Create(dlg.FileName).Close();
                    currentFilePath = dlg.FileName;
                    richTextBox1.Clear();
                    isTextChanged = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при создании файла: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CheckForUnsavedChanges())
                Close();
        }

        private void Compiler_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!CheckForUnsavedChanges())
                e.Cancel = true;
        }

        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e) => SaveFile();
        private void сохранитьКакToolStripMenuItem_Click(object sender, EventArgs e) => SaveFileAs();

        private void отменитьToolStripMenuItem_Click(object sender, EventArgs e) => richTextBox1.Undo();
        private void повторитьToolStripMenuItem_Click(object sender, EventArgs e) => richTextBox1.Redo();
        private void вырезатьToolStripMenuItem_Click(object sender, EventArgs e) => richTextBox1.Cut();
        private void копироватьToolStripMenuItem_Click(object sender, EventArgs e) => richTextBox1.Copy();
        private void вставитьToolStripMenuItem_Click(object sender, EventArgs e) => richTextBox1.Paste();
        private void удалитьToolStripMenuItem_Click(object sender, EventArgs e) => richTextBox1.SelectedText = string.Empty;
        private void выделитьВсеToolStripMenuItem_Click(object sender, EventArgs e) => richTextBox1.SelectAll();

        private void button10_Click(object sender, EventArgs e) => new AboutProgramm().Show();
        private void button11_Click(object sender, EventArgs e) => new Spravka().Show();

        // Очистка предыдущей подсветки и таблицы
        private void ClearResults()
        {
            dataGridView1.Rows.Clear();
            richTextBox1.SelectAll();
            richTextBox1.SelectionBackColor = richTextBox1.BackColor;
            richTextBox1.SelectionLength = 0;
        }

        // Сброс каретки (чтобы не оставалось активно выделение)
        private void ResetCaret()
        {
            richTextBox1.SelectionStart = 0;
            richTextBox1.SelectionLength = 0;
        }

        // Универсальный метод для Regex-сканирования (оставлен без изменений)
        private void ScanPattern(Regex pattern, string type)
        {
            var text = richTextBox1.Text;
            foreach (Match m in pattern.Matches(text))
            {
                dataGridView1.Rows.Add(type, m.Value, m.Index);
                richTextBox1.Select(m.Index, m.Length);
                // Подсветка жёлтым
                richTextBox1.SelectionBackColor = Color.Yellow;
            }
        }

        // Специальный метод для ИНН (определяем тип по длине)
        private void ScanINN(Regex pattern)
        {
            var text = richTextBox1.Text;
            foreach (Match m in pattern.Matches(text))
            {
                string innType = (m.Value.Length == 10) ? "ИНН Юр. лица" : "ИНН Физ. лица";
                dataGridView1.Rows.Add(innType, m.Value, m.Index);
                richTextBox1.Select(m.Index, m.Length);
                richTextBox1.SelectionBackColor = Color.Yellow;
            }
        }

        // Обработчики кнопок, использующих Regex:
        private void buttonPassword_Click(object sender, EventArgs e)
        {
            ClearResults();
            ScanPattern(rxWords, "Пароль");
            ResetCaret();
        }

        private void buttonINN_Click(object sender, EventArgs e)
        {
            ClearResults();
            ScanINN(rxInn);
            ResetCaret();
        }

        private void buttonTime_Click(object sender, EventArgs e)
        {
            ClearResults();
            ScanPattern(rxTime, "Время (Regex)");
            ResetCaret();
        }

        // ===== Обновлённый метод поиска времени через DFA-автомат (с явной проверкой) =====
        private List<(string Value, int Index)> FindTimeByAutomaton(string text)
        {
            var results = new List<(string, int)>();
            int n = text.Length;

            for (int i = 0; i < n; i++)
            {
                int start = i;
                int j = i;

                // 1) Парсим часы: 1 или 2 цифры
                int hourDigits = 0;
                while (j < n && hourDigits < 2 && char.IsDigit(text[j]))
                {
                    hourDigits++;
                    j++;
                }
                // Должна быть хотя бы одна цифра, не более двух.
                if (hourDigits == 0 || hourDigits > 2)
                    continue;

                string hourStr = text.Substring(start, hourDigits);
                if (!int.TryParse(hourStr, out int hour) || hour > 23)
                    continue;

                // После часов должен идти символ ':'
                if (j >= n || text[j] != ':')
                    continue;
                j++; // пропускаем ':'

                // 2) Парсим минуты: 1 или 2 цифры
                int minDigits = 0;
                int minStart = j;
                while (j < n && minDigits < 2 && char.IsDigit(text[j]))
                {
                    minDigits++;
                    j++;
                }
                if (minDigits == 0 || minDigits > 2)
                    continue;

                string minStr = text.Substring(minStart, minDigits);
                if (!int.TryParse(minStr, out int minute) || minute > 59)
                    continue;

                // После минут снова должен идти символ ':'
                if (j >= n || text[j] != ':')
                    continue;
                j++; // пропускаем ':'

                // 3) Парсим секунды: 1 или 2 цифры
                int secDigits = 0;
                int secStart = j;
                while (j < n && secDigits < 2 && char.IsDigit(text[j]))
                {
                    secDigits++;
                    j++;
                }
                if (secDigits == 0 || secDigits > 2)
                    continue;

                string secStr = text.Substring(secStart, secDigits);
                if (!int.TryParse(secStr, out int second) || second > 59)
                    continue;

                // 4) Убедимся, что после секунды либо конец строки, либо не буква/цифра
                if (j < n && (char.IsDigit(text[j]) || char.IsLetter(text[j])))
                    continue;

                // Если всё ок — это корректное время от позиции start до j (не включая j)
                string match = text.Substring(start, j - start);
                results.Add((match, start));

                // Перескакиваем i на конец найденного фрагмента, чтобы не искать вложенные в него вхождения
                i = j - 1;
            }

            return results;
        }

        // ===== Обработчик кнопки «Время (автомат)» =====
        private void buttonTimeAuto_Click(object sender, EventArgs e)
        {
            // 1) Очищаем таблицу и убираем предыдущую подсветку
            dataGridView1.Rows.Clear();
            richTextBox1.SelectAll();
            richTextBox1.SelectionBackColor = richTextBox1.BackColor;
            richTextBox1.SelectionLength = 0;

            // 2) Ищем все вхождения ЧЧ:ММ:СС (1–2 цифр для каждой части) через DFA-автомат
            var matches = FindTimeByAutomaton(richTextBox1.Text);

            // 3) Добавляем каждое совпадение в таблицу и подсвечиваем в тексте
            foreach (var (val, idx) in matches)
            {
                dataGridView1.Rows.Add("Время (автомат)", val, idx);

                richTextBox1.Select(idx, val.Length);
                richTextBox1.SelectionBackColor = Color.LightGreen;
            }

            // 4) Сбрасываем каретку (убираем текущее выделение)
            richTextBox1.SelectionStart = 0;
            richTextBox1.SelectionLength = 0;
        }

        private void buttonOpenForm2_Click(object sender, EventArgs e)
        {
            var form2 = new Form2();
            form2.Show();
            this.Hide();
        }
    }
}




//using System;
//using System.IO;
//using System.Windows.Forms;
//using System.Text.RegularExpressions;
//using System.Drawing;
//using System.Collections.Generic;

//namespace KompilatoriLaba1
//{
//    public partial class Compiler : Form
//    {
//        private bool isTextChanged = false;
//        private string currentFilePath = string.Empty;

//        // Предкомпилированные регулярные выражения
//        private static readonly Regex rxWords = new Regex(@"[А-Яа-я0-9\p{P}\p{S}]+", RegexOptions.Compiled);
//        private static readonly Regex rxInn = new Regex(@"\b(?:\d{10}|\d{12})\b", RegexOptions.Compiled);
//        private static readonly Regex rxTime = new Regex(@"\b(?:[01]?\d|2[0-3]):(?:[0-5]?\d):(?:[0-5]?\d)\b", RegexOptions.Compiled);

//        public Compiler()
//        {
//            InitializeComponent();
//        }

//        private void Compiler_Load(object sender, EventArgs e)
//        {
//            // Настройка колонок dataGridView1
//            dataGridView1.Columns.Clear();
//            dataGridView1.Columns.Add("Тип", "Тип");
//            dataGridView1.Columns.Add("Строка", "Строка");
//            dataGridView1.Columns.Add("Позиция", "Позиция");
//            dataGridView1.AllowUserToAddRows = false;
//        }

//        private void richTextBox1_TextChanged(object sender, EventArgs e)
//        {
//            isTextChanged = true;
//        }

//        private bool CheckForUnsavedChanges()
//        {
//            if (!isTextChanged) return true;
//            var result = MessageBox.Show(
//                "Сохранить изменения перед продолжением?",
//                "Несохранённые изменения",
//                MessageBoxButtons.YesNoCancel,
//                MessageBoxIcon.Warning);

//            if (result == DialogResult.Yes)
//                return SaveFile();
//            if (result == DialogResult.No)
//                return true;

//            return false;
//        }

//        private bool SaveFile()
//        {
//            if (string.IsNullOrEmpty(currentFilePath))
//                return SaveFileAs();

//            try
//            {
//                File.WriteAllText(currentFilePath, richTextBox1.Text);
//                isTextChanged = false;
//                return true;
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
//                return false;
//            }
//        }

//        private bool SaveFileAs()
//        {
//            using (var dlg = new SaveFileDialog
//            {
//                Filter = "Text Files|*.txt",
//                Title = "Сохранить файл"
//            })
//            {
//                if (dlg.ShowDialog() != DialogResult.OK)
//                    return false;

//                currentFilePath = dlg.FileName;
//                return SaveFile();
//            }
//        }

//        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
//        {
//            if (!CheckForUnsavedChanges()) return;

//            using (var dlg = new OpenFileDialog
//            {
//                Filter = "Text Files|*.txt",
//                Title = "Открыть текстовый файл"
//            })
//            {
//                if (dlg.ShowDialog() != DialogResult.OK) return;
//                try
//                {
//                    currentFilePath = dlg.FileName;
//                    richTextBox1.Text = File.ReadAllText(currentFilePath);
//                    isTextChanged = false;
//                }
//                catch (Exception ex)
//                {
//                    MessageBox.Show($"Ошибка при открытии файла: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
//                }
//            }
//        }

//        private void создатьToolStripMenuItem_Click(object sender, EventArgs e)
//        {
//            if (!CheckForUnsavedChanges()) return;

//            using (var dlg = new SaveFileDialog
//            {
//                Filter = "Text Files|*.txt",
//                Title = "Создать новый текстовый файл",
//                FileName = "Новый файл.txt"
//            })
//            {
//                if (dlg.ShowDialog() != DialogResult.OK) return;
//                try
//                {
//                    File.Create(dlg.FileName).Close();
//                    currentFilePath = dlg.FileName;
//                    richTextBox1.Clear();
//                    isTextChanged = false;
//                }
//                catch (Exception ex)
//                {
//                    MessageBox.Show($"Ошибка при создании файла: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
//                }
//            }
//        }

//        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
//        {
//            if (CheckForUnsavedChanges())
//                Close();
//        }

//        private void Compiler_FormClosing(object sender, FormClosingEventArgs e)
//        {
//            if (!CheckForUnsavedChanges())
//                e.Cancel = true;
//        }

//        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e) => SaveFile();
//        private void сохранитьКакToolStripMenuItem_Click(object sender, EventArgs e) => SaveFileAs();

//        private void отменитьToolStripMenuItem_Click(object sender, EventArgs e) => richTextBox1.Undo();
//        private void повторитьToolStripMenuItem_Click(object sender, EventArgs e) => richTextBox1.Redo();
//        private void вырезатьToolStripMenuItem_Click(object sender, EventArgs e) => richTextBox1.Cut();
//        private void копироватьToolStripMenuItem_Click(object sender, EventArgs e) => richTextBox1.Copy();
//        private void вставитьToolStripMenuItem_Click(object sender, EventArgs e) => richTextBox1.Paste();
//        private void удалитьToolStripMenuItem_Click(object sender, EventArgs e) => richTextBox1.SelectedText = string.Empty;
//        private void выделитьВсеToolStripMenuItem_Click(object sender, EventArgs e) => richTextBox1.SelectAll();

//        private void button10_Click(object sender, EventArgs e) => new AboutProgramm().Show();
//        private void button11_Click(object sender, EventArgs e) => new Spravka().Show();

//        // Вспомогательные методы для общего сканирования
//        private void ClearResults()
//        {
//            dataGridView1.Rows.Clear();
//            richTextBox1.SelectAll();
//            richTextBox1.SelectionBackColor = richTextBox1.BackColor;
//            richTextBox1.SelectionLength = 0;
//        }

//        private void ResetCaret()
//        {
//            richTextBox1.SelectionStart = 0;
//            richTextBox1.SelectionLength = 0;
//        }

//        // Универсальный метод для Regex-сканирования
//        private void ScanPattern(Regex pattern, string type)
//        {
//            var text = richTextBox1.Text;
//            foreach (Match m in pattern.Matches(text))
//            {
//                dataGridView1.Rows.Add(type, m.Value, m.Index);
//                richTextBox1.Select(m.Index, m.Length);
//                richTextBox1.SelectionBackColor = Color.Yellow;
//            }
//        }

//        // Специальный метод для ИНН (определение типа по длине)
//        private void ScanINN(Regex pattern)
//        {
//            var text = richTextBox1.Text;
//            foreach (Match m in pattern.Matches(text))
//            {
//                string innType = m.Value.Length == 10 ? "ИНН Юр. лица" : "ИНН Физ. лица";
//                dataGridView1.Rows.Add(innType, m.Value, m.Index);
//                richTextBox1.Select(m.Index, m.Length);
//                richTextBox1.SelectionBackColor = Color.Yellow;
//            }
//        }

//        // Методы-обработчики для трёх кнопок
//        private void buttonPassword_Click(object sender, EventArgs e)
//        {
//            ClearResults();
//            ScanPattern(rxWords, "Пароль");
//            ResetCaret();
//        }

//        private void buttonINN_Click(object sender, EventArgs e)
//        {
//            ClearResults();
//            ScanINN(rxInn);
//            ResetCaret();
//        }

//        private void buttonTime_Click(object sender, EventArgs e)
//        {
//            ClearResults();
//            ScanPattern(rxTime, "Время (Regex)");
//            ResetCaret();
//        }

//        // Новый метод поиска через автомат
//        private List<(string Value, int Index)> FindTimeByAutomaton(string text)
//        {
//            var results = new List<(string, int)>();
//            int n = text.Length;

//            for (int i = 0; i < n; i++)
//            {
//                int state = 0;
//                int j = i;

//                while (j < n)
//                {
//                    char c = text[j];
//                    switch (state)
//                    {
//                        case 0: // часы
//                            if (char.IsDigit(c))
//                            {
//                                int d = c - '0';
//                                if (d <= 2 && j + 1 < n && char.IsDigit(text[j + 1]))
//                                {
//                                    int d2 = text[j + 1] - '0';
//                                    int hour = d * 10 + d2;
//                                    if (hour <= 23)
//                                    {
//                                        j += 2; state = 1; continue;
//                                    }
//                                }
//                                j++; state = 1; continue;
//                            }
//                            state = -1; break;
//                        case 1: // ':'
//                            if (c == ':') { j++; state = 2; continue; }
//                            state = -1; break;
//                        case 2: // минуты
//                            if (char.IsDigit(c))
//                            {
//                                int d = c - '0';
//                                if (j + 1 < n && char.IsDigit(text[j + 1]))
//                                {
//                                    int d2 = text[j + 1] - '0';
//                                    int minute = d * 10 + d2;
//                                    if (minute <= 59)
//                                    {
//                                        j += 2; state = 3; continue;
//                                    }
//                                }
//                                j++; state = 3; continue;
//                            }
//                            state = -1; break;
//                        case 3: // ':'
//                            if (c == ':') { j++; state = 4; continue; }
//                            state = -1; break;
//                        case 4: // секунды
//                            if (char.IsDigit(c))
//                            {
//                                int d = c - '0';
//                                if (j + 1 < n && char.IsDigit(text[j + 1]))
//                                {
//                                    int d2 = text[j + 1] - '0';
//                                    int second = d * 10 + d2;
//                                    if (second <= 59)
//                                    {
//                                        j += 2; state = 5; continue;
//                                    }
//                                }
//                                j++; state = 5; continue;
//                            }
//                            state = -1; break;
//                        default:
//                            state = -1; break;
//                    }
//                    break; // выход из while при ошибке или default
//                }

//                if (state == 5)
//                {
//                    string match = text.Substring(i, j - i);
//                    results.Add((match, i));
//                }
//            }

//            return results;
//        }

//        // Обработчик для новой кнопки "Время (автомат)"
//        private void buttonTimeAuto_Click(object sender, EventArgs e)
//        {
//            ClearResults();
//            var matches = FindTimeByAutomaton(richTextBox1.Text);
//            foreach (var (val, idx) in matches)
//            {
//                dataGridView1.Rows.Add("Время (автомат)", val, idx);
//                richTextBox1.Select(idx, val.Length);
//                richTextBox1.SelectionBackColor = Color.LightGreen;
//            }
//            ResetCaret();
//        }

//        private void buttonOpenForm2_Click(object sender, EventArgs e)
//        {
//            // Создаём и показываем Form2
//            var form2 = new Form2();
//            form2.Show();
//            // Скрываем эту форму
//            this.Hide();
//        }
//    }
//}

//using System;
//using System.IO;
//using System.Windows.Forms;
//using System.Text.RegularExpressions;
//using System.Drawing;

//namespace KompilatoriLaba1
//{
//    public partial class Compiler : Form
//    {
//        private bool isTextChanged = false;
//        private string currentFilePath = string.Empty;

//        // Предкомпилированные регулярные выражения
//        private static readonly Regex rxWords = new Regex(@"[А-Яа-я0-9\p{P}\p{S}]+", RegexOptions.Compiled);
//        private static readonly Regex rxInn = new Regex(@"\b(?:\d{10}|\d{12})\b", RegexOptions.Compiled);
//        private static readonly Regex rxTime = new Regex(@"\b(?:[01]?\d|2[0-3]):(?:[0-5]?\d):(?:[0-5]?\d)\b", RegexOptions.Compiled);

//        public Compiler()
//        {
//            InitializeComponent();
//        }

//        private void Compiler_Load(object sender, EventArgs e)
//        {
//            dataGridView1.Columns.Clear();
//            dataGridView1.Columns.Add("Тип", "Тип");
//            dataGridView1.Columns.Add("Строка", "Строка");
//            dataGridView1.Columns.Add("Позиция", "Позиция");
//            dataGridView1.AllowUserToAddRows = false;
//        }

//        private void richTextBox1_TextChanged(object sender, EventArgs e)
//        {
//            isTextChanged = true;
//        }

//        private bool CheckForUnsavedChanges()
//        {
//            if (!isTextChanged) return true;
//            var result = MessageBox.Show(
//                "Сохранить изменения перед продолжением?",
//                "Несохранённые изменения",
//                MessageBoxButtons.YesNoCancel,
//                MessageBoxIcon.Warning);

//            if (result == DialogResult.Yes)
//                return SaveFile();
//            if (result == DialogResult.No)
//                return true;

//            return false;
//        }

//        private bool SaveFile()
//        {
//            if (string.IsNullOrEmpty(currentFilePath))
//                return SaveFileAs();

//            try
//            {
//                File.WriteAllText(currentFilePath, richTextBox1.Text);
//                isTextChanged = false;
//                return true;
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
//                return false;
//            }
//        }

//        private bool SaveFileAs()
//        {
//            using (var dlg = new SaveFileDialog
//            {
//                Filter = "Text Files|*.txt",
//                Title = "Сохранить файл"
//            })
//            {
//                if (dlg.ShowDialog() != DialogResult.OK)
//                    return false;

//                currentFilePath = dlg.FileName;
//                return SaveFile();
//            }
//        }

//        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
//        {
//            if (!CheckForUnsavedChanges()) return;

//            using (var dlg = new OpenFileDialog
//            {
//                Filter = "Text Files|*.txt",
//                Title = "Открыть текстовый файл"
//            })
//            {
//                if (dlg.ShowDialog() != DialogResult.OK) return;
//                try
//                {
//                    currentFilePath = dlg.FileName;
//                    richTextBox1.Text = File.ReadAllText(currentFilePath);
//                    isTextChanged = false;
//                }
//                catch (Exception ex)
//                {
//                    MessageBox.Show($"Ошибка при открытии файла: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
//                }
//            }
//        }

//        private void создатьToolStripMenuItem_Click(object sender, EventArgs e)
//        {
//            if (!CheckForUnsavedChanges()) return;

//            using (var dlg = new SaveFileDialog
//            {
//                Filter = "Text Files|*.txt",
//                Title = "Создать новый текстовый файл",
//                FileName = "Новый файл.txt"
//            })
//            {
//                if (dlg.ShowDialog() != DialogResult.OK) return;
//                try
//                {
//                    File.Create(dlg.FileName).Close();
//                    currentFilePath = dlg.FileName;
//                    richTextBox1.Clear();
//                    isTextChanged = false;
//                }
//                catch (Exception ex)
//                {
//                    MessageBox.Show($"Ошибка при создании файла: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
//                }
//            }
//        }

//        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
//        {
//            if (CheckForUnsavedChanges())
//                Close();
//        }

//        private void Compiler_FormClosing(object sender, FormClosingEventArgs e)
//        {
//            if (!CheckForUnsavedChanges())
//                e.Cancel = true;
//        }

//        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e) => SaveFile();
//        private void сохранитьКакToolStripMenuItem_Click(object sender, EventArgs e) => SaveFileAs();

//        private void отменитьToolStripMenuItem_Click(object sender, EventArgs e) => richTextBox1.Undo();
//        private void повторитьToolStripMenuItem_Click(object sender, EventArgs e) => richTextBox1.Redo();
//        private void вырезатьToolStripMenuItem_Click(object sender, EventArgs e) => richTextBox1.Cut();
//        private void копироватьToolStripMenuItem_Click(object sender, EventArgs e) => richTextBox1.Copy();
//        private void вставитьToolStripMenuItem_Click(object sender, EventArgs e) => richTextBox1.Paste();
//        private void удалитьToolStripMenuItem_Click(object sender, EventArgs e) => richTextBox1.SelectedText = string.Empty;
//        private void выделитьВсеToolStripMenuItem_Click(object sender, EventArgs e) => richTextBox1.SelectAll();

//        private void button10_Click(object sender, EventArgs e) => new AboutProgramm().Show();
//        private void button11_Click(object sender, EventArgs e) => new Spravka().Show();

//        private void ClearResults()
//        {
//            dataGridView1.Rows.Clear();
//            richTextBox1.SelectAll();
//            richTextBox1.SelectionBackColor = richTextBox1.BackColor;
//            richTextBox1.SelectionLength = 0;
//        }

//        private void ResetCaret()
//        {
//            richTextBox1.SelectionStart = 0;
//            richTextBox1.SelectionLength = 0;
//        }

//        private void buttonPassword_Click(object sender, EventArgs e)
//        {
//            ClearResults();
//            var text = richTextBox1.Text;
//            foreach (Match m in rxWords.Matches(text))
//            {
//                dataGridView1.Rows.Add("Пароль", m.Value, m.Index);
//                richTextBox1.Select(m.Index, m.Length);
//                richTextBox1.SelectionBackColor = Color.Yellow;
//            }
//            ResetCaret();
//        }

//        private void buttonINN_Click(object sender, EventArgs e)
//        {
//            ClearResults();
//            var text = richTextBox1.Text;
//            foreach (Match m in rxInn.Matches(text))
//            {
//                // Определяем тип ИНН по длине
//                string innType = m.Value.Length == 10
//                    ? "ИНН Юр. лица"
//                    : "ИНН Физ. лица";
//                dataGridView1.Rows.Add(innType, m.Value, m.Index);
//                richTextBox1.Select(m.Index, m.Length);
//                richTextBox1.SelectionBackColor = Color.Yellow;
//            }
//            ResetCaret();
//        }

//        private void buttonTime_Click(object sender, EventArgs e)
//        {
//            ClearResults();
//            var text = richTextBox1.Text;
//            foreach (Match m in rxTime.Matches(text))
//            {
//                dataGridView1.Rows.Add("Время", m.Value, m.Index);
//                richTextBox1.Select(m.Index, m.Length);
//                richTextBox1.SelectionBackColor = Color.Yellow;
//            }
//            ResetCaret();
//        }
//    }
//}
//using System;
//using System.IO;
//using System.Windows.Forms;
//using System.Text.RegularExpressions;
//using System.Drawing;
//using System.Collections.Generic;
//namespace KompilatoriLaba1
//{
//    public partial class Compiler : Form
//    {
//        private bool isTextChanged = false;
//        private string currentFilePath = string.Empty;

//        public Compiler()
//        {
//            InitializeComponent();
//        }

//        private void Compiler_Load(object sender, EventArgs e)
//        {dataGridView1.Columns.Clear();
//         dataGridView1.Columns.Add("Тип", "Тип");
//         dataGridView1.Columns.Add("Строка", "Строка");
//         dataGridView1.Columns.Add("Позиция", "Позиция");
//         dataGridView1.AllowUserToAddRows = false; }

//            private void richTextBox1_TextChanged(object sender, EventArgs e)
//        {
//            isTextChanged = true;
//        }

//        // 🔹 Функция проверки несохранённых изменений перед важными действиями
//        private bool CheckForUnsavedChanges()
//        {
//            if (!isTextChanged) return true; // Если изменений нет – выходим

//            DialogResult result = MessageBox.Show(
//                "Сохранить изменения перед продолжением?",
//                "Несохранённые изменения",
//                MessageBoxButtons.YesNoCancel,
//                MessageBoxIcon.Warning);

//            if (result == DialogResult.Yes)
//            {
//                return SaveFile();
//            }
//            else if (result == DialogResult.No)
//            {
//                return true;
//            }

//            return false; // "Отмена" – прерываем действие
//        }

//        // 🔹 Функция сохранения файла
//        private bool SaveFile()
//        {
//            if (string.IsNullOrEmpty(currentFilePath))
//            {
//                return SaveFileAs(); // Если нет пути, вызываем "Сохранить как"
//            }

//            try
//            {
//                File.WriteAllText(currentFilePath, richTextBox1.Text);
//                isTextChanged = false;
//                return true;
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
//                return false;
//            }
//        }

//        // 🔹 Функция "Сохранить как"
//        private bool SaveFileAs()
//        {
//            SaveFileDialog saveFileDialog = new SaveFileDialog
//            {
//                Filter = "Text Files|*.txt",
//                Title = "Сохранить файл"
//            };

//            if (saveFileDialog.ShowDialog() == DialogResult.OK)
//            {
//                currentFilePath = saveFileDialog.FileName;
//                return SaveFile();
//            }

//            return false; // Пользователь отменил сохранение
//        }

//        // 🔹 Открытие файла с проверкой изменений
//        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
//        {
//            if (!CheckForUnsavedChanges()) return;

//            OpenFileDialog openFileDialog = new OpenFileDialog
//            {
//                Filter = "Text Files|*.txt",
//                Title = "Открыть текстовый файл"
//            };

//            if (openFileDialog.ShowDialog() == DialogResult.OK)
//            {
//                try
//                {
//                    currentFilePath = openFileDialog.FileName;
//                    richTextBox1.Text = File.ReadAllText(currentFilePath);
//                    isTextChanged = false;
//                }
//                catch (Exception ex)
//                {
//                    MessageBox.Show($"Ошибка при открытии файла: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
//                }
//            }
//        }

//        // 🔹 Создание нового файла с проверкой изменений
//        private void создатьToolStripMenuItem_Click(object sender, EventArgs e)
//        {
//            if (!CheckForUnsavedChanges()) return;

//            SaveFileDialog saveFileDialog = new SaveFileDialog
//            {
//                Filter = "Text Files|*.txt",
//                Title = "Создать новый текстовый файл",
//                FileName = "Новый файл.txt"
//            };

//            if (saveFileDialog.ShowDialog() == DialogResult.OK)
//            {
//                try
//                {
//                    File.Create(saveFileDialog.FileName).Close();
//                    currentFilePath = saveFileDialog.FileName;
//                    richTextBox1.Clear();
//                    isTextChanged = false;
//                }
//                catch (Exception ex)
//                {
//                    MessageBox.Show($"Ошибка при создании файла: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
//                }
//            }
//        }

//        // 🔹 Сохранение изменений перед выходом из программы
//        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
//        {
//            if (CheckForUnsavedChanges())
//            {
//                Close();
//            }
//        }

//        // 🔹 Проверка перед закрытием формы
//        private void Compiler_FormClosing(object sender, FormClosingEventArgs e)
//        {
//            if (!CheckForUnsavedChanges())
//            {
//                e.Cancel = true; // Отменяем выход, если пользователь передумал
//            }
//        }

//        // 🔹 Сохранение файла
//        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)
//        {
//            SaveFile();
//        }

//        // 🔹 Сохранение файла как
//        private void сохранитьКакToolStripMenuItem_Click(object sender, EventArgs e)
//        {
//            SaveFileAs();
//        }

//        // 🔹 Функции редактирования текста
//        private void отменитьToolStripMenuItem_Click(object sender, EventArgs e) => richTextBox1.Undo();
//        private void повторитьToolStripMenuItem_Click(object sender, EventArgs e) => richTextBox1.Redo();
//        private void вырезатьToolStripMenuItem_Click(object sender, EventArgs e) => richTextBox1.Cut();
//        private void копироватьToolStripMenuItem_Click(object sender, EventArgs e) => richTextBox1.Copy();
//        private void вставитьToolStripMenuItem_Click(object sender, EventArgs e) => richTextBox1.Paste();
//        private void удалитьToolStripMenuItem_Click(object sender, EventArgs e) => richTextBox1.SelectedText = string.Empty;
//        private void выделитьВсеToolStripMenuItem_Click(object sender, EventArgs e) => richTextBox1.SelectAll();

//        // 🔹 Открытие справки и информации о программе
//        private void button10_Click(object sender, EventArgs e)
//        {
//            var AboutProgramm = new AboutProgramm();
//            AboutProgramm.Show();
//        }

//        private void button11_Click(object sender, EventArgs e)
//        {
//            var Spravka = new Spravka();
//            Spravka.Show();
//        }
//        private void ClearResults()
//        {
//            dataGridView1.Rows.Clear();
//            // Снять подсветку
//            richTextBox1.SelectAll();
//            richTextBox1.SelectionBackColor = richTextBox1.BackColor;
//            richTextBox1.SelectionLength = 0;
//        }

//        private void ResetCaret()
//        {
//            richTextBox1.SelectionStart = 0;
//            richTextBox1.SelectionLength = 0;
//        }
//        private void buttonPassword_Click(object sender, EventArgs e)
//        {
//            ClearResults();
//            string text = richTextBox1.Text;
//            // РВ: русские буквы, цифры, пунктуация/символы
//            var rx = new Regex(@"[А-Яа-я0-9\p{P}\p{S}]+", RegexOptions.Compiled);

//            foreach (Match m in rx.Matches(text))
//            {
//                dataGridView1.Rows.Add("Пароль", m.Value, m.Index);
//                richTextBox1.Select(m.Index, m.Length);
//                richTextBox1.SelectionBackColor = Color.Yellow;
//            }

//            ResetCaret();
//        }

//        // ==== 2) Поиск ИНН физических и юридических лиц ====
//        private void buttonINN_Click(object sender, EventArgs e)
//        {
//            ClearResults();
//            string text = richTextBox1.Text;

//            // Две РВ: 12 цифр и 10 цифр
//            var patterns = new Dictionary<string, Regex>
//    {
//        //{ "ИНН", new Regex(@"\b\d{12}\b", RegexOptions.Compiled) },
//        { "ИНН",  new Regex(@"\b\d{10}|\d{12}\b", RegexOptions.Compiled) }
//    };

//            foreach (var kv in patterns)
//            {
//                foreach (Match m in kv.Value.Matches(text))
//                {
//                    dataGridView1.Rows.Add(kv.Key, m.Value, m.Index);
//                    richTextBox1.Select(m.Index, m.Length);
//                    richTextBox1.SelectionBackColor = Color.Yellow;
//                }
//            }

//            ResetCaret();
//        }

//        // ==== 3) Поиск Времени ====
//        private void buttonTime_Click(object sender, EventArgs e)
//        {
//            ClearResults();
//            string text = richTextBox1.Text;

//            // РВ: ЧЧ:ММ:СС в 24-часовом формате
//            // часы: 0–23 с необязательным ведущим нулём
//            // минуты/секунды: 0–59 с необязательным ведущим нулём
//            var rx = new Regex(@"\b(?:[01]?\d|2[0-3]):(?:[0-5]?\d):(?:[0-5]?\d)\b", RegexOptions.Compiled);

//            foreach (Match m in rx.Matches(text))
//            {
//                dataGridView1.Rows.Add("Время", m.Value, m.Index);
//                richTextBox1.Select(m.Index, m.Length);
//                richTextBox1.SelectionBackColor = Color.Yellow;
//            }

//            ResetCaret();
//        }

//        //private void buttonScan_Click(object sender, EventArgs e)
//        //{
//        //    string inputText = richTextBox1.Text;
//        //    //richTextBox2.Clear();  // Очищаем richTextBox2 перед выводом новых результатов
//        //    //Analyze(inputText);  // Запускаем анализ текста
//        //}
//    }
//}

//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
//using System.Drawing;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Forms;

//namespace KompilatoriLaba1
//{
//    public partial class Compiler : Form
//    {
//        public Compiler()
//        {
//            InitializeComponent();
//        }
//        private bool isTextChanged = false;

//        private string currentFilePath = string.Empty;
//        private void richTextBox1_TextChanged(object sender, EventArgs e)
//        {
//            isTextChanged = true;
//        }

//        private void отменитьToolStripMenuItem_Click(object sender, EventArgs e)
//        {
//            richTextBox1.Undo(); // Отменить
//        }

//        private void повторитьToolStripMenuItem_Click(object sender, EventArgs e)
//        {
//            richTextBox1.Redo(); // Повторить
//        }

//        private void вырезатьToolStripMenuItem_Click(object sender, EventArgs e)
//        {
//            richTextBox1.Cut(); // Вырезать
//        }

//        private void копироватьToolStripMenuItem_Click(object sender, EventArgs e)
//        {
//            richTextBox1.Copy(); // Копировать
//        }

//        private void вставитьToolStripMenuItem_Click(object sender, EventArgs e)
//        {
//            richTextBox1.Paste(); // Вставить
//        }

//        private void удалитьToolStripMenuItem_Click(object sender, EventArgs e)
//        {
//            richTextBox1.SelectedText = string.Empty; // Удалить
//        }

//        private void выделитьВсеToolStripMenuItem_Click(object sender, EventArgs e)
//        {
//           richTextBox1.SelectAll(); // Выделить все
//        }

//        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
//        {
//            Close();
//        }

//        private void Compiler_Load(object sender, EventArgs e)
//        {

//        }

//        private void сохранитьКакToolStripMenuItem_Click(object sender, EventArgs e)
//        {
//            if (string.IsNullOrWhiteSpace(richTextBox1.Text))
//            {
//                MessageBox.Show("Текстовое поле пусто. Нечего сохранять.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
//                return;
//            }

//            SaveFileDialog saveFileDialog = new SaveFileDialog
//            {
//                Filter = "Text Files|*.txt",
//                Title = "Сохранить текст в файл"
//            };

//            if (saveFileDialog.ShowDialog() == DialogResult.OK)
//            {
//                try
//                {
//                    File.WriteAllText(saveFileDialog.FileName, richTextBox1.Text);
//                    MessageBox.Show("Файл успешно сохранён.", "Готово", MessageBoxButtons.OK, MessageBoxIcon.Information);
//                }
//                catch (Exception ex)
//                {
//                    MessageBox.Show($"Ошибка при сохранении файла: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
//                }
//            }
//        }

//        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
//        {
//            OpenFileDialog openFileDialog = new OpenFileDialog
//            {
//                Filter = "Text Files|*.txt",
//                Title = "Открыть текстовый файл"
//            };

//            if (openFileDialog.ShowDialog() == DialogResult.OK)
//            {
//                try
//                {
//                    currentFilePath = openFileDialog.FileName;
//                    richTextBox1.Text = File.ReadAllText(currentFilePath);
//                    MessageBox.Show("Файл успешно загружен.", "Готово", MessageBoxButtons.OK, MessageBoxIcon.Information);
//                }
//                catch (Exception ex)
//                {
//                    MessageBox.Show($"Ошибка при открытии файла: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
//                }
//            }
//        }

//        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)
//        {
//            if (string.IsNullOrWhiteSpace(richTextBox1.Text))
//            {
//                MessageBox.Show("Текстовое поле пусто. Нечего сохранять.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
//                return;
//            }

//            if (string.IsNullOrEmpty(currentFilePath)) // Если файл еще не был открыт или сохранен ранее
//            {
//                SaveFileDialog saveFileDialog = new SaveFileDialog
//                {
//                    Filter = "Text Files|*.txt",
//                    Title = "Сохранить текст в файл"
//                };

//                if (saveFileDialog.ShowDialog() == DialogResult.OK)
//                {
//                    currentFilePath = saveFileDialog.FileName;
//                }
//                else
//                {
//                    return; // Если пользователь отменил сохранение, выходим из функции
//                }
//            }

//            try
//            {
//                File.WriteAllText(currentFilePath, richTextBox1.Text);
//                MessageBox.Show("Файл успешно сохранён.", "Готово", MessageBoxButtons.OK, MessageBoxIcon.Information);
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show($"Ошибка при сохранении файла: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
//            }
//        }

//        private void создатьToolStripMenuItem_Click(object sender, EventArgs e)
//        {
//            SaveFileDialog saveFileDialog = new SaveFileDialog
//            {
//                Filter = "Text Files|*.txt",
//                Title = "Создать новый текстовый файл",
//                FileName = "Новый файл.txt" // Стандартное имя файла
//            };

//            if (saveFileDialog.ShowDialog() == DialogResult.OK)
//            {
//                try
//                {
//                    File.Create(saveFileDialog.FileName).Close(); // Создаем пустой файл
//                    MessageBox.Show("Файл успешно создан!", "Готово", MessageBoxButtons.OK, MessageBoxIcon.Information);
//                }
//                catch (Exception ex)
//                {
//                    MessageBox.Show($"Ошибка при создании файла: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
//                }
//            }
//        }

//        private void button10_Click(object sender, EventArgs e)
//        {
//            var AboutProgramm = new AboutProgramm();
//            AboutProgramm.Show();
//        }

//        private void button11_Click(object sender, EventArgs e)
//        {
//            var Spravka = new Spravka();
//            Spravka.Show();
//        }

//        //private void splitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
//        //{

//        //}

//        //private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
//        //{

//        //}
//    }
//}
