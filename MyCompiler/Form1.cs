using System.Windows.Forms;

namespace MyCompiler
{
    public partial class Form1 : Form
    {

        private string currentFile = string.Empty;
        private bool isTextChanged = false;
        private bool isUpdatingFontSize = false;

        public Form1()
        {
            InitializeComponent();

            if (TB_Edit.Font != null)
            {
                CBox_FontSize.Text = TB_Edit.Font.Size.ToString();
            }

            TB_Edit.TextChanged += (s, e) => { isTextChanged = true; };

            this.FormClosing += Form1_FormClosing;

            this.Text = "Новый документ";

            TB_Edit.SelectionChanged += TB_Edit_SelectionChanged;
            TB_Edit.KeyUp += TB_Edit_KeyUp;
            UpdateCapsLockStatus();
            UpdateFileStatus();

            this.KeyPreview = true;
            this.KeyDown += Form1_KeyDown;
        }

        public void TB_Edit_SelectionChanged(object sender, EventArgs e)
        {
            UpdateCursorPosition();
        }

        public void TB_Edit_KeyUp(object sender, KeyEventArgs e)
        {
            UpdateCursorPosition();
            UpdateCapsLockStatus();
        }

        public void UpdateCursorPosition()
        {
            int line = TB_Edit.GetLineFromCharIndex(TB_Edit.SelectionStart) + 1;
            int column = TB_Edit.SelectionStart -
                         TB_Edit.GetFirstCharIndexFromLine(line - 1) + 1;

            statusLineColumn.Text = $"Строка: {line}, Столбец: {column}";
        }

        public void UpdateFileInfo()
        {
            if (string.IsNullOrEmpty(currentFile))
            {
                statusFileInfo.Text = "0 B";
            }
            else
            {
                FileInfo info = new FileInfo(currentFile);
                string size = FormatFileSize(info.Length);
                statusFileInfo.Text = $"{Path.GetFileName(currentFile)} | {size}";
            }
        }

        public void UpdateCapsLockStatus()
        {
            if (Control.IsKeyLocked(Keys.CapsLock))
            {
                statusCapsLock.Text = "CAPSLOCK ON";
            }
            else {
                statusCapsLock.Text = "capslock off";
            }
            ;
        }

        public void UpdateFileStatus()
        {
            UpdateFileInfo();
            this.Text = $"{(string.IsNullOrEmpty(currentFile) ? "Новый документ" : Path.GetFileName(currentFile))}";
        }

        public string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }

        public void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (isTextChanged)
            {
                DialogResult result = MessageBox.Show(
                    "Сохранить изменения в файле?","Несохраненные изменения",
                    MessageBoxButtons.YesNoCancel,MessageBoxIcon.Question);

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

        public void CBox_FontSize_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (CBox_FontSize.SelectedItem != null)
            {
                if (float.TryParse(CBox_FontSize.SelectedItem.ToString(), out float newSize))
                {
                    SetFontSize(newSize);
                }
            }
        }

        public void SetFontSize(float newSize)
        {
            try
            {
                if (newSize < 6) newSize = 6;
                if (newSize > 72) newSize = 72;

                isUpdatingFontSize = true;

                if (TB_Edit.Font != null)
                {
                    TB_Edit.Font = new Font(TB_Edit.Font.FontFamily, newSize, TB_Edit.Font.Style);
                }

                if (TB_Console != null && TB_Console.Font != null)
                {
                    TB_Console.Font = new Font(TB_Console.Font.FontFamily, newSize, TB_Console.Font.Style);
                }

                CBox_FontSize.Text = newSize.ToString();

                isUpdatingFontSize = false;
            }
            catch (Exception ex)
            {
                isUpdatingFontSize = false;
                MessageBox.Show($"Ошибка при изменении шрифта: {ex.Message}",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void SaveCurrentFile()
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
                    UpdateFileStatus();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при сохранении: {ex.Message}",
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public void SaveFileAs()
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

        public bool CheckSaveBeforeAction()
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
            return true;
        }

        public void TS_Open_Click(object sender, EventArgs e)
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
                        UpdateFileStatus();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при открытии файла: {ex.Message}",
                            "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        public void TS_Create_Click(object sender, EventArgs e)
        {
            if (CheckSaveBeforeAction())
            {
                TB_Edit.Clear();
                currentFile = string.Empty;
                isTextChanged = false;
                this.Text = "Новый документ";
                UpdateFileStatus();
            }
        }

        public void TS_Save_Click(object sender, EventArgs e)
        {
            SaveCurrentFile();
        }

        public void TS_SaveAs_Click(object sender, EventArgs e)
        {
            SaveFileAs();
            UpdateFileStatus();
        }

        public void TS_Exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public void TS_Undo_Click(object sender, EventArgs e)
        {
            if (TB_Edit.CanUndo)
            {
                TB_Edit.Undo();
            }
        }

        public void TS_Redo_Click(object sender, EventArgs e)
        {
            if (TB_Edit.CanRedo)
            {
                TB_Edit.Redo();
            }
        }

        public void TS_Cut_Click(object sender, EventArgs e)
        {
            if (TB_Edit.SelectedText.Length > 0)
            {
                TB_Edit.Cut();
            }
        }

        public void TS_Copy_Click(object sender, EventArgs e)
        {
            if (TB_Edit.SelectedText.Length > 0)
            {
                TB_Edit.Copy();
            }
        }

        public void TS_Paste_Click(object sender, EventArgs e)
        {
            if (Clipboard.ContainsText())
            {
                TB_Edit.Paste();
            }
        }

        public void TS_Delete_Click(object sender, EventArgs e)
        {
            if (TB_Edit.SelectedText.Length > 0)
            {
                int selectionStart = TB_Edit.SelectionStart;
                int selectionLength = TB_Edit.SelectionLength;

                TB_Edit.Text = TB_Edit.Text.Remove(selectionStart, selectionLength);

                TB_Edit.SelectionStart = selectionStart;
            }
        }

        public void TS_SelectAll_Click(object sender, EventArgs e)
        {
            TB_Edit.SelectAll();
        }

        public void TS_Directory_Click(object sender, EventArgs e)
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

        public void TS_About_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
               "MyCompiler\n" +
               "Версия 0.0\n\n" +
               "Текстовый редактор\n" +
               "2026 г.",
               "О программе",
               MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void ChangeFontSize(int delta)
        {
            try
            {
                if (TB_Edit.Font != null)
                {
                    float newSize = TB_Edit.Font.Size + delta;
                    if (newSize >= 6 && newSize <= 72)
                    {
                        TB_Edit.Font = new Font(TB_Edit.Font.FontFamily, newSize, TB_Edit.Font.Style);
                        TB_Console.Font = new Font(TB_Console.Font.FontFamily, newSize, TB_Console.Font.Style);
                        CBox_FontSize.Text = newSize.ToString();
                    }
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при изменении шрифта: {ex.Message}",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void Form1_KeyDown(object sender, KeyEventArgs e){
            if (e.Control)
            {
                switch (e.KeyCode)
                {
                    case Keys.N:
                        TS_Create_Click(sender, e);
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                        break;

                    case Keys.O:
                        TS_Open_Click(sender, e);
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                        break;

                    case Keys.S:
                        TS_Save_Click(sender, e);
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                        break;

                    case Keys.Oemplus:
                    case Keys.Add:
                        ChangeFontSize(1);
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                        break;

                    case Keys.OemMinus:
                    case Keys.Subtract:
                        ChangeFontSize(-1);
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                        break;
                }
            }

            if (e.Control && e.Shift && e.KeyCode == Keys.S)
            {
                TS_SaveAs_Click(sender, e);
                e.Handled = true;
                e.SuppressKeyPress = true;
            }

            if (e.KeyCode == Keys.F1)
            {
                TS_Directory_Click(sender, e);
                e.Handled = true;
                e.SuppressKeyPress = true;
            }

            if (e.KeyCode == Keys.Delete && TB_Edit.SelectedText.Length > 0)
            {
                TS_Delete_Click(sender, e);
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        public void CB_Open_Click(object sender, EventArgs e)
        {
            TS_Open_Click(sender, e);
        }
        public void CB_Create_Click(object sender, EventArgs e)
        {
            TS_Create_Click(sender, e);
        }

        public void CB_Save_Click(object sender, EventArgs e)
        {
            TS_Save_Click(sender, e);
        }

        public void CB_SaveAs_Click(object sender, EventArgs e)
        {
            TS_SaveAs_Click(sender, e);
        }

        public void CB_Exit_Click(object sender, EventArgs e)
        {
            TS_Exit_Click(sender, e);
        }

        public void CB_Undo_Click(object sender, EventArgs e)
        {
            TS_Undo_Click(sender, e);
        }

        public void CB_Redo_Click(object sender, EventArgs e)
        {
            TS_Redo_Click(sender, e);
        }

        public void CB_Cut_Click(object sender, EventArgs e)
        {
            TS_Cut_Click(sender, e);
        }

        public void CB_Copy_Click(object sender, EventArgs e)
        {
            TS_Copy_Click(sender, e);
        }

        public void CB_Paste_Click(object sender, EventArgs e)
        {
            TS_Paste_Click(sender, e);
        }

        public void CB_Delete_Click(object sender, EventArgs e)
        {
            TS_Delete_Click(sender, e);
        }

        public void CB_SelectAll_Click(object sender, EventArgs e)
        {
            TS_SelectAll_Click(sender, e);
        }

        public void CB_Directory_Click(object sender, EventArgs e)
        {
            TS_Directory_Click(sender, e);
        }

        public void CB_About_Click(object sender, EventArgs e)
        {
            TS_About_Click(sender, e);
        }

        public void CB_FontUp_Click(object sender, EventArgs e)
        {
            ChangeFontSize(1);
        }

        public void CB_FontDown_Click(object sender, EventArgs e)
        {
            ChangeFontSize(-1);
        }
    }
}
