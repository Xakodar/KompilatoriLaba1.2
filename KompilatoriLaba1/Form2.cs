// Form2.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace KompilatoriLaba1
{
    public partial class Form2 : Form
    {
        private bool isTextChanged = false;
        private string currentFilePath = string.Empty;

        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            // здесь может быть инициализация
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            isTextChanged = true;
        }

        private bool CheckForUnsavedChanges()
        {
            if (!isTextChanged) return true;
            var res = MessageBox.Show(
                "Сохранить изменения перед продолжением?",
                "Несохранённые изменения",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Warning);

            if (res == DialogResult.Yes) return SaveFile();
            if (res == DialogResult.No) return true;
            return false;
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
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                Title = "Открыть файл"
            })
            {
                if (dlg.ShowDialog() != DialogResult.OK) return;
                currentFilePath = dlg.FileName;
                richTextBox1.Text = File.ReadAllText(currentFilePath);
                isTextChanged = false;
            }
        }

        private void создатьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!CheckForUnsavedChanges()) return;
            using (var dlg = new SaveFileDialog
            {
                Filter = "Text Files|*.txt",
                Title = "Создать файл",
                FileName = "Новый файл.txt"
            })
            {
                if (dlg.ShowDialog() != DialogResult.OK) return;
                File.Create(dlg.FileName).Close();
                currentFilePath = dlg.FileName;
                richTextBox1.Clear();
                isTextChanged = false;
            }
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CheckForUnsavedChanges())
                Close();
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!CheckForUnsavedChanges())
                e.Cancel = true;
        }

        // Меню Правка
        private void отменитьToolStripMenuItem_Click(object sender, EventArgs e) => richTextBox1.Undo();
        private void повторитьToolStripMenuItem_Click(object sender, EventArgs e) => richTextBox1.Redo();
        private void вырезатьToolStripMenuItem_Click(object sender, EventArgs e) => richTextBox1.Cut();
        private void копироватьToolStripMenuItem_Click(object sender, EventArgs e) => richTextBox1.Copy();
        private void вставитьToolStripMenuItem_Click(object sender, EventArgs e) => richTextBox1.Paste();
        private void удалитьToolStripMenuItem_Click(object sender, EventArgs e) => richTextBox1.SelectedText = string.Empty;
        private void выделитьВсеToolStripMenuItem_Click(object sender, EventArgs e) => richTextBox1.SelectAll();

        // Переход к форме компилятора
        private void buttonOpenForm1_Click(object sender, EventArgs e)
        {
            var form1 = new Compiler();
            form1.Show();
            Hide();
        }

        private void buttonLex_Click(object sender, EventArgs e)
        {
            richTextBox2.Clear();
            Lexer.Tokenize(
                richTextBox1.Text,
                out List<Lexer.Token> tokens,
                out List<Lexer.LexError> errors
            );

            richTextBox2.AppendText("=== Токены ===\r\n");
            foreach (var tok in tokens)
                richTextBox2.AppendText(tok + "\r\n");

            if (errors.Count > 0)
            {
                richTextBox2.AppendText("\r\n=== Лексические ошибки ===\r\n");
                foreach (var err in errors)
                    richTextBox2.AppendText(err + "\r\n");
            }
        }

        // Синтаксический анализ (кнопка «Парсер»)
        private void buttonParse_Click(object sender, EventArgs e)
        {
            richTextBox2.Clear();
            // ВСЕГДА лексим, но parse идёт по любым токенам, даже ошибочным
            Lexer.Tokenize(
                richTextBox1.Text,
                out List<Lexer.Token> tokens,
                out List<Lexer.LexError> errors
            );

            var parser = new Parser(tokens, s => richTextBox2.AppendText(s + "\r\n"));
            parser.Parse();
        }
    }
}


//// Form2.cs
//using System;
//using System.IO;
//using System.Windows.Forms;

//namespace KompilatoriLaba1
//{
//    public partial class Form2 : Form
//    {
//        private bool isTextChanged = false;
//        private string currentFilePath = string.Empty;

//        public Form2()
//        {
//            InitializeComponent();
//        }

//        private void Form2_Load(object sender, EventArgs e)
//        {
//            // Инициализация меню и базовых компонентов
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

//            return false; // Cancel
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

//        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
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

//        private void buttonOpenForm1_Click(object sender, EventArgs e)
//        {
//            var form1 = new Compiler();
//            form1.Show();
//            this.Hide();
//        }

//        private void buttonParse_Click(object sender, EventArgs e)
//        {
//            // Очищаем область вывода
//            richTextBox2.Clear();
//            // Получаем входной текст из richTextBox1
//            var text = richTextBox1.Text;
//            // Создаём и запускаем парсер, логируем шаги в richTextBox2
//            var parser = new Parser(text, s => richTextBox2.AppendText(s + Environment.NewLine));
//            parser.Parse();
//        }

//    }
//}
//using System;
//using System.Collections.Generic;
//using System;
//using System.IO;
//using System.Windows.Forms;

//namespace KompilatoriLaba1
//{
//    public partial class Form2 : Form
//    {
//        private bool isTextChanged = false;
//        private string currentFilePath = string.Empty;

//        public Form2()
//        {
//            InitializeComponent();
//        }

//        private void Form2_Load(object sender, EventArgs e)
//        {
//            // Инициализация меню и базовых компонентов
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

//            return false; // Cancel
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

//        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
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

//        private void buttonOpenForm1_Click(object sender, EventArgs e)
//        {
//            var form1 = new Compiler();
//            form1.Show();
//            this.Hide();
//        }
//    }
//}
