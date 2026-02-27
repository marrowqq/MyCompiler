using System.Windows.Forms;

namespace MyCompiler
{
    public partial class Form1 : Form
    {

        private string currentFile = string.Empty;
        private bool isTextChanged = false;

        public Form1()
        {
            InitializeComponent();
            TB_Edit.TextChanged += (s, e) => { isTextChanged = true; };

            this.FormClosing += Form1_FormClosing;

            this.Text = "Новый документ";
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (isTextChanged)
            {
                DialogResult result = MessageBox.Show(
                    "Сохранить изменения в файле?",
                    "Несохраненные изменения",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    SaveCurrentFile();
                    if (isTextChanged)
                        e.Cancel = true;
                }
                else if (result == DialogResult.Cancel)
                {
                    e.Cancel = true;
                }
            }
        }

        private void SaveCurrentFile()
        {
            if (string.IsNullOrEmpty(currentFile))
            {
                SaveFileAs();
            }
            else
            {
                try
                {
                    TB_Edit.SaveFile(currentFile, RichTextBoxStreamType.PlainText);
                    isTextChanged = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при сохранении: {ex.Message}",
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void SaveFileAs()
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*";
                sfd.Title = "Сохранить файл как";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        TB_Edit.SaveFile(sfd.FileName, RichTextBoxStreamType.PlainText);
                        currentFile = sfd.FileName;
                        isTextChanged = false;
                        this.Text = $"{Path.GetFileName(currentFile)}";
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при сохранении: {ex.Message}",
                            "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private bool CheckSaveBeforeAction()
        {
            if (isTextChanged)
            {
                DialogResult result = MessageBox.Show(
                    "Сохранить изменения в файле?", "Несохраненные изменения",
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    SaveCurrentFile();
                    if (isTextChanged) return false;
                    return true;
                }
                else if (result == DialogResult.No)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return true; // Нет изменений, можно продолжать
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void TS_Open_Click(object sender, EventArgs e)
        {
            if (!CheckSaveBeforeAction()) return;

            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*";
                ofd.Title = "Открыть файл";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        TB_Edit.LoadFile(ofd.FileName, RichTextBoxStreamType.PlainText);
                        currentFile = ofd.FileName;
                        isTextChanged = false;
                        this.Text = $"{Path.GetFileName(currentFile)}";
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при открытии файла: {ex.Message}",
                            "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void TS_File_Click(object sender, EventArgs e)
        {

        }

        private void TS_Create_Click(object sender, EventArgs e)
        {
            if (CheckSaveBeforeAction())
            {
                TB_Edit.Clear();
                currentFile = string.Empty;
                isTextChanged = false;
                this.Text = "Новый документ";
            }
        }

        private void TS_Save_Click(object sender, EventArgs e)
        {
            SaveCurrentFile();
        }

        private void TS_SaveAs_Click(object sender, EventArgs e)
        {
            SaveFileAs();
        }

        private void TS_Exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void TS_Undo_Click(object sender, EventArgs e)
        {
            if (TB_Edit.CanUndo)
            {
                TB_Edit.Undo();
            }
        }

        private void TS_Redo_Click(object sender, EventArgs e)
        {
            if (TB_Edit.CanRedo)
            {
                TB_Edit.Redo();
            }
        }

        private void TS_Cut_Click(object sender, EventArgs e)
        {
            if (TB_Edit.SelectedText.Length > 0)
            {
                TB_Edit.Cut();
            }
        }

        private void TS_Copy_Click(object sender, EventArgs e)
        {
            if (TB_Edit.SelectedText.Length > 0)
            {
                TB_Edit.Copy();
            }
        }

        private void TS_Paste_Click(object sender, EventArgs e)
        {
            if (Clipboard.ContainsText())
            {
                TB_Edit.Paste();
            }
        }

        private void TS_Delete_Click(object sender, EventArgs e)
        {
            if (TB_Edit.SelectedText.Length > 0)
            {
                int selectionStart = TB_Edit.SelectionStart;
                int selectionLength = TB_Edit.SelectionLength;

                TB_Edit.Text = TB_Edit.Text.Remove(selectionStart, selectionLength);

                TB_Edit.SelectionStart = selectionStart;
            }
        }

        private void TS_SelectAll_Click(object sender, EventArgs e)
        {
            TB_Edit.SelectAll();
        }

        private void TS_Reference_Click(object sender, EventArgs e)
        {

        }

        private void TS_Directory_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
               "Справочная система\n\n" +
               "Реализованные функции:\n" +
               "• Создание нового файла\n" +
               "• Открытие существующего файла\n" +
               "• Сохранение и сохранение как\n" +
               "• Отмена/повтор действий\n" +
               "• Вырезать/копировать/вставить/удалить\n" +
               "• Выделение всего текста\n" +
               "• Изменение размеров областей\n\n",
               "Справка",
               MessageBoxButtons.OK,
               MessageBoxIcon.Information);
        }

        private void TS_About_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
               "MyCompiler\n" +
               "Версия 0.0\n\n" +
               "Текстовый редактор\n" +
               "2026 г.",
               "О программе",
               MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void CB_Open_Click(object sender, EventArgs e)
        {
            TS_Open_Click(sender, e);
        }

        private void CB_Create_Click(object sender, EventArgs e)
        {
            TS_Create_Click(sender, e);
        }

        private void CB_Save_Click(object sender, EventArgs e)
        {
            TS_Save_Click(sender, e);
        }

        private void CB_SaveAs_Click(object sender, EventArgs e)
        {
            TS_SaveAs_Click(sender, e);
        }

        private void CB_Exit_Click(object sender, EventArgs e)
        {
            TS_Exit_Click(sender, e);
        }

        private void CB_Undo_Click(object sender, EventArgs e)
        {
            TS_Undo_Click(sender, e);
        }

        private void CB_Redo_Click(object sender, EventArgs e)
        {
            TS_Redo_Click(sender, e);
        }

        private void CB_Cut_Click(object sender, EventArgs e)
        {
            TS_Cut_Click(sender, e);
        }

        private void CB_Copy_Click(object sender, EventArgs e)
        {
            TS_Copy_Click(sender, e);
        }

        private void CB_Paste_Click(object sender, EventArgs e)
        {
            TS_Paste_Click(sender, e);
        }

        private void CB_Delete_Click(object sender, EventArgs e)
        {
            TS_Delete_Click(sender, e);
        }

        private void CB_SelectAll_Click(object sender, EventArgs e)
        {
            TS_SelectAll_Click(sender, e);
        }

        private void CB_Directory_Click(object sender, EventArgs e)
        {
            TS_Directory_Click(sender, e);
        }

        private void CB_About_Click(object sender, EventArgs e)
        {
            TS_About_Click(sender, e);
        }
    }
}
