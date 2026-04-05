using MyCompiler.LexicalAnalyzer;
using MyCompiler.SyntaxAnalyzer;
using MyCompiler.SearchModule;
using System.Drawing.Text;
using System.Text;
using System.Windows.Forms;

namespace MyCompiler
{

public partial class Form1 : Form
    {

        private string currentFile = string.Empty;
        private bool isTextChanged = false;
        private bool isUpdatingFontSize = false;
        private Lexer _lexer;
        public Form1()
        {
               
            InitializeComponent();
            InitializeLexicalAnalyzer();

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

        private void InitializeLexicalAnalyzer()
        {
            _lexer = new Lexer();

            SetupTokensGrid();
            SetupSyntaxErrorsGrid();
            SetupSearchResultsGrid();

            dgvTokens.CellClick += DgvTokens_CellClick;
            dgvSyntaxErrors.CellClick += DgvSyntaxErrors_CellClick;
            dgvSearchResults.CellClick += DgvSearchResults_CellClick;
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
            else
            {
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
                    "Сохранить изменения в файле?", "Несохраненные изменения",
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

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

        public void Form1_KeyDown(object sender, KeyEventArgs e)
        {
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

        private void CB_Start_Click(object sender, EventArgs e)
        {
            try
            {
                TB_Edit.Text = Preparetext(TB_Edit.Text);
                string inputText = TB_Edit.Text;

                if (string.IsNullOrWhiteSpace(inputText))
                {
                    dgvTokens.Rows.Clear();
                    dgvSyntaxErrors.Rows.Clear();
                    rtbOutput.Clear();
                    statusLineColumn.Text = "Введите код для анализа";
                    return;
                }

                List<Token> tokens = _lexer.Analyze(inputText);
                DisplayTokens(tokens);

                if (_lexer.HasErrors)
                {
                    HighlightErrors(_lexer.Errors);
                    statusLineColumn.Text = $"Лексических ошибок: {_lexer.Errors.Count}";
                }
                else
                {
                    statusLineColumn.Text = "Лексических ошибок не обнаружено";
                }
                var parser = new Parser(tokens);
                List<SyntaxError> syntaxErrors = parser.Parse();

                DisplaySyntaxErrors(syntaxErrors);
                DisplayTextOutput(tokens, syntaxErrors);

                if (syntaxErrors.Count > 0)
                {
                    HighlightSyntaxErrors(syntaxErrors);
                }
                if (syntaxErrors.Count == 0 && !_lexer.HasErrors)
                {
                    statusLineColumn.Text = "✓ Ошибок не обнаружено!";
                }
                else
                {
                    statusLineColumn.Text = $"Лексических: {_lexer.Errors?.Count ?? 0}, Синтаксических: {syntaxErrors.Count}";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при анализе: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                statusLineColumn.Text = "Ошибка при выполнении анализа";
            }
        }

        private void DisplaySyntaxErrors(List<SyntaxError> errors)
        {
            dgvSyntaxErrors.Rows.Clear();

            if (errors == null || errors.Count == 0)
            {
                dgvSyntaxErrors.Rows.Add("", "✓ Синтаксических ошибок не обнаружено", "", "", "");
                return;
            }

            foreach (var error in errors)
            {
                int rowIndex = dgvSyntaxErrors.Rows.Add();
                DataGridViewRow row = dgvSyntaxErrors.Rows[rowIndex];

                row.Cells["ErrorNumber"].Value = error.ErrorNumber;
                row.Cells["ErrorMessage"].Value = error.Message;
                row.Cells["Expected"].Value = error.Expected;
                row.Cells["Found"].Value = error.Found;
                row.Cells["ErrorPosition"].Value = $"{error.Line}:{error.Column}";

                row.DefaultCellStyle.BackColor = Color.LightYellow;
            }
        }

        private void HighlightSyntaxErrors(List<SyntaxError> errors)
        {
            foreach (var error in errors)
            {
                try
                {
                    int lineStartIndex = TB_Edit.GetFirstCharIndexFromLine(error.Line - 1);
                    if (lineStartIndex >= 0)
                    {
                        int errorIndex = lineStartIndex + error.Column - 1;

                        if (errorIndex >= 0 && errorIndex < TB_Edit.TextLength)
                        {
                            TB_Edit.Select(errorIndex, 1);
                            if (TB_Edit.SelectionBackColor != Color.LightCoral)
                            {
                                TB_Edit.SelectionBackColor = Color.LightGoldenrodYellow;
                                TB_Edit.SelectionColor = Color.DarkOrange;
                            }
                        }
                    }
                }
                catch (Exception) { }
            }

            TB_Edit.DeselectAll();
        }

        private void DisplayTextOutput(List<Token> tokens, List<SyntaxError> syntaxErrors)
        {
            rtbOutput.Clear();

            int lexicalErrors = 0;
            foreach (var token in tokens)
            {
                if (token.IsError)
                {
                    lexicalErrors++;
                }
            }

            int syntaxErrorsCount = syntaxErrors?.Count ?? 0;

            rtbOutput.AppendText($"Лексических ошибок: {lexicalErrors}\n");
            rtbOutput.AppendText($"Синтаксических ошибок: {syntaxErrorsCount}\n");
        }

        private void DgvSyntaxErrors_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow row = dgvSyntaxErrors.Rows[e.RowIndex];

            if (row.Cells["ErrorPosition"].Value == null) return;

            string position = row.Cells["ErrorPosition"].Value.ToString();
            if (string.IsNullOrEmpty(position) || position == "0:0") return;

            var parts = position.Split(':');
            if (parts.Length == 2)
            {
                if (int.TryParse(parts[0], out int line) &&
                    int.TryParse(parts[1], out int column))
                {
                    SetSelection(line, column, column);
                }
            }
        }

        private string Preparetext(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            while (input.Contains("    "))
            {
                input = input.Replace("    ", "\t");
            }
            while (input.Contains("  "))
            {
                input = input.Replace("  ", " ");
            }

            return input;
        }

        private string GetTokenTypeDescription(TokenType type)
        {
            return type switch
            {
                TokenType.KeywordWhile => "Ключевое слово while",
                TokenType.Identifier => "Идентификатор",
                TokenType.DelimiterLParen => "Разделитель (",
                TokenType.DelimiterRParen => "Разделитель )",
                TokenType.DelimiterSemicolon => "Разделитель ;",
                TokenType.DelimiterLBrace => "Разделитель {",
                TokenType.DelimiterRBrace => "Разделитель }",
                TokenType.IntegerNumber => "Целое число",
                TokenType.OperatorIncrement => "Оператор инкремента ++",
                TokenType.OperatorPlus => "Оператор сложения +",
                TokenType.OperatorDecrement => "Оператор декремента --",
                TokenType.OperatorMinus => "Оператор вычитания -",
                TokenType.OperatorLessEqual => "Оператор <=",
                TokenType.OperatorLess => "Оператор <",
                TokenType.OperatorGreaterEqual => "Оператор >=",
                TokenType.OperatorGreater => "Оператор >",
                TokenType.OperatorEqual => "Оператор ==",
                TokenType.OperatorNotEqual => "Оператор !=",
                TokenType.OperatorLogicalOr => "Оператор ||",
                TokenType.OperatorLogicalAnd => "Оператор &&",
                TokenType.Error => "Ошибка",
                TokenType.OperatorSpace => "Пробел",
                TokenType.OperatorTab => "Табуляция",
                TokenType.OperatorNewLine => "Перевод строки",
                _ => type.ToString()
            };
        }

        private void HighlightErrors(List<LexicalError> errors)
        {
            TB_Edit.SelectAll();
            TB_Edit.SelectionBackColor = Color.White;
            TB_Edit.SelectionColor = Color.Black;
            TB_Edit.DeselectAll();

            foreach (var error in errors)
            {
                try
                {
                    int lineStartIndex = TB_Edit.GetFirstCharIndexFromLine(error.Line - 1);
                    if (lineStartIndex >= 0)
                    {
                        int errorIndex = lineStartIndex + error.Column - 1;

                        if (errorIndex >= 0 && errorIndex < TB_Edit.TextLength)
                        {
                            TB_Edit.Select(errorIndex, 1);
                            TB_Edit.SelectionBackColor = Color.LightCoral;
                            TB_Edit.SelectionColor = Color.DarkRed;
                        }
                    }
                }
                catch (Exception ex){   }
            }

            TB_Edit.DeselectAll();
        }

        private void DgvTokens_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow row = dgvTokens.Rows[e.RowIndex];

            string location = row.Cells["Location"].Value?.ToString();
            if (string.IsNullOrEmpty(location)) return;

            var parts = location.Split(':', '-');
            if (parts.Length >= 3)
            {
                if (int.TryParse(parts[0], out int line) &&
                    int.TryParse(parts[1], out int startCol) &&
                    int.TryParse(parts[2], out int endCol))
                {
                    SetSelection(line, startCol, endCol);
                }
            }
        }

        private void SetSelection(int line, int startCol, int endCol)
        {
            try
            {
                int lineStartIndex = TB_Edit.GetFirstCharIndexFromLine(line - 1);
                if (lineStartIndex >= 0)
                {
                    int startIndex = lineStartIndex + startCol - 1;
                    int length = endCol - startCol + 1;

                    if (startIndex >= 0 && startIndex + length <= TB_Edit.TextLength)
                    {
                        TB_Edit.Focus();
                        TB_Edit.Select(startIndex, length);
                        TB_Edit.ScrollToCaret();

                        TB_Edit.SelectionBackColor = Color.LightBlue;
                    }
                }
            }
            catch (Exception ex) { }
            
        }

        private void DisplayTokens(List<Token> tokens)
        {
            dgvTokens.Rows.Clear();
            StringBuilder textOutput = new StringBuilder();
            textOutput.AppendLine("Код\tТип\tЛексема\tПозиция");
            textOutput.AppendLine("---\t---\t-------\t--------");

            foreach (Token token in tokens)
            {
                int rowIndex = dgvTokens.Rows.Add();
                DataGridViewRow row = dgvTokens.Rows[rowIndex];

                row.Cells["Code"].Value = (int)token.Type;
                row.Cells["Type"].Value = GetTokenTypeDescription(token.Type);

                string displayValue = token.Value;
                if (token.Type == TokenType.OperatorSpace)
                    displayValue = "␣";
                else if (token.Type == TokenType.OperatorTab)
                    displayValue = "→";
                else if (token.Type == TokenType.OperatorNewLine)
                    displayValue = "¶";

                row.Cells["Lexeme"].Value = displayValue;

                string location = $"{token.Line}:{token.StartColumn}-{token.EndColumn}";
                row.Cells["Location"].Value = location;

                if (token.IsError)
                {
                    row.DefaultCellStyle.BackColor = Color.LightCoral;
                    row.DefaultCellStyle.ForeColor = Color.DarkRed;
                    row.Cells["Type"].Value = "Ошибка";
                }

                textOutput.AppendLine($"{(int)token.Type}\t{GetTokenTypeDescription(token.Type)}\t{displayValue}\t{location}");
            }
        }
        private void SetupTokensGrid()
        {
            dgvTokens.AutoGenerateColumns = false;
            dgvTokens.AllowUserToAddRows = false;
            dgvTokens.ReadOnly = true;
            dgvTokens.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvTokens.MultiSelect = false;

            dgvTokens.Columns.Clear();

            DataGridViewTextBoxColumn colCode = new DataGridViewTextBoxColumn();
            colCode.Name = "Code";
            colCode.HeaderText = "Код";
            colCode.Width = 60;
            dgvTokens.Columns.Add(colCode);

            DataGridViewTextBoxColumn colType = new DataGridViewTextBoxColumn();
            colType.Name = "Type";
            colType.HeaderText = "Тип";
            colType.Width = 120;
            dgvTokens.Columns.Add(colType);

            DataGridViewTextBoxColumn colLexeme = new DataGridViewTextBoxColumn();
            colLexeme.Name = "Lexeme";
            colLexeme.HeaderText = "Лексема";
            colLexeme.Width = 150;
            dgvTokens.Columns.Add(colLexeme);

            DataGridViewTextBoxColumn colLocation = new DataGridViewTextBoxColumn();
            colLocation.Name = "Location";
            colLocation.HeaderText = "Позиция";
            colLocation.Width = 100;
            dgvTokens.Columns.Add(colLocation);
        }

        private void SetupSyntaxErrorsGrid()
        {
            dgvSyntaxErrors.AutoGenerateColumns = false;
            dgvSyntaxErrors.AllowUserToAddRows = false;
            dgvSyntaxErrors.ReadOnly = true;
            dgvSyntaxErrors.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvSyntaxErrors.MultiSelect = false;

            dgvSyntaxErrors.Columns.Clear();

            DataGridViewTextBoxColumn colNumber = new DataGridViewTextBoxColumn();
            colNumber.Name = "ErrorNumber";
            colNumber.HeaderText = "№";
            colNumber.Width = 40;
            dgvSyntaxErrors.Columns.Add(colNumber);

            DataGridViewTextBoxColumn colMessage = new DataGridViewTextBoxColumn();
            colMessage.Name = "ErrorMessage";
            colMessage.HeaderText = "Сообщение об ошибке";
            colMessage.Width = 300;
            dgvSyntaxErrors.Columns.Add(colMessage);

            DataGridViewTextBoxColumn colExpected = new DataGridViewTextBoxColumn();
            colExpected.Name = "Expected";
            colExpected.HeaderText = "Ожидалось";
            colExpected.Width = 100;
            dgvSyntaxErrors.Columns.Add(colExpected);

            DataGridViewTextBoxColumn colFound = new DataGridViewTextBoxColumn();
            colFound.Name = "Found";
            colFound.HeaderText = "Найдено";
            colFound.Width = 100;
            dgvSyntaxErrors.Columns.Add(colFound);

            DataGridViewTextBoxColumn colPosition = new DataGridViewTextBoxColumn();
            colPosition.Name = "ErrorPosition";
            colPosition.HeaderText = "Позиция";
            colPosition.Width = 80;
            dgvSyntaxErrors.Columns.Add(colPosition);
        }

        private void SetupSearchResultsGrid()
        {
            dgvSearchResults.AutoGenerateColumns = false;
            dgvSearchResults.AllowUserToAddRows = false;
            dgvSearchResults.ReadOnly = true;
            dgvSearchResults.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvSearchResults.MultiSelect = false;

            dgvSearchResults.Columns.Clear();

            DataGridViewTextBoxColumn colMatch = new DataGridViewTextBoxColumn();
            colMatch.Name = "Match";
            colMatch.HeaderText = "Найденная подстрока";
            colMatch.Width = 300;
            dgvSearchResults.Columns.Add(colMatch);

            DataGridViewTextBoxColumn colPosition = new DataGridViewTextBoxColumn();
            colPosition.Name = "Position";
            colPosition.HeaderText = "Позиция (строка:символ)";
            colPosition.Width = 120;
            dgvSearchResults.Columns.Add(colPosition);

            DataGridViewTextBoxColumn colLength = new DataGridViewTextBoxColumn();
            colLength.Name = "Length";
            colLength.HeaderText = "Длина";
            colLength.Width = 60;
            dgvSearchResults.Columns.Add(colLength);
        }

        private void CB_Search_Click(object sender, EventArgs e)
        {
            string text = TB_Edit.Text;

            if (string.IsNullOrWhiteSpace(text))
            {
                MessageBox.Show("Введите текст для поиска", "Предупреждение",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            RegexSearcher.SearchType searchType;
            switch (cbSearchType.SelectedIndex)
            {
                case 0:
                    searchType = RegexSearcher.SearchType.Years2000to2010;
                    break;
                case 1:
                    searchType = RegexSearcher.SearchType.MirCard;
                    break;
                case 2:
                    searchType = RegexSearcher.SearchType.IPv6WithPrefix;
                    break;
                default:
                    searchType = RegexSearcher.SearchType.Years2000to2010;
                    break;
            }

            RegexSearcher searcher = new RegexSearcher();
            List<SearchResult> results = searcher.FindMatches(text, searchType);

            DisplaySearchResults(results);

            lblSearchCount.Text = $"Найдено: {results.Count}";

            ClearSearchHighlight();
        }

        private void DisplaySearchResults(List<SearchResult> results)
        {
            dgvSearchResults.Rows.Clear();

            foreach (var result in results)
            {
                int rowIndex = dgvSearchResults.Rows.Add();
                DataGridViewRow row = dgvSearchResults.Rows[rowIndex];

                row.Cells["Match"].Value = result.Match;
                row.Cells["Position"].Value = $"{result.Line}:{result.Position}";
                row.Cells["Length"].Value = result.Length;

                row.Tag = result.AbsoluteIndex;
            }
        }
     
        private void DgvSearchResults_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow row = dgvSearchResults.Rows[e.RowIndex];
            int absoluteIndex = (int)row.Tag;
            int length = Convert.ToInt32(row.Cells["Length"].Value);

            ClearSearchHighlight();

            if (absoluteIndex >= 0 && absoluteIndex + length <= TB_Edit.TextLength)
            {
                TB_Edit.Focus();
                TB_Edit.Select(absoluteIndex, length);
                TB_Edit.SelectionBackColor = Color.Yellow;
                TB_Edit.ScrollToCaret();
            }
        }

        private void ClearSearchHighlight()
        {
            int selectionStart = TB_Edit.SelectionStart;
            int selectionLength = TB_Edit.SelectionLength;

            TB_Edit.SelectAll();
            TB_Edit.SelectionBackColor = Color.White;
            TB_Edit.SelectionColor = Color.Black;

            TB_Edit.Select(selectionStart, selectionLength);
        }
    }
}
