namespace MyCompiler
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            menuStrip1 = new MenuStrip();
            TS_File = new ToolStripMenuItem();
            TS_Open = new ToolStripMenuItem();
            TS_Create = new ToolStripMenuItem();
            TS_Save = new ToolStripMenuItem();
            TS_SaveAs = new ToolStripMenuItem();
            TS_Exit = new ToolStripMenuItem();
            TS_Edit = new ToolStripMenuItem();
            TS_Undo = new ToolStripMenuItem();
            TS_Redo = new ToolStripMenuItem();
            TS_Cut = new ToolStripMenuItem();
            TS_Copy = new ToolStripMenuItem();
            TS_Paste = new ToolStripMenuItem();
            TS_Delete = new ToolStripMenuItem();
            TS_SelectAll = new ToolStripMenuItem();
            TS_Reference = new ToolStripMenuItem();
            TS_Directory = new ToolStripMenuItem();
            TS_About = new ToolStripMenuItem();
            toolStrip1 = new ToolStrip();
            CB_Open = new ToolStripButton();
            CB_Create = new ToolStripButton();
            CB_Save = new ToolStripButton();
            CB_SaveAs = new ToolStripButton();
            CB_Exit = new ToolStripButton();
            toolStripSeparator1 = new ToolStripSeparator();
            CB_Undo = new ToolStripButton();
            CB_Redo = new ToolStripButton();
            CB_Cut = new ToolStripButton();
            CB_Copy = new ToolStripButton();
            CB_Paste = new ToolStripButton();
            CB_Delete = new ToolStripButton();
            CB_SelectAll = new ToolStripButton();
            toolStripSeparator2 = new ToolStripSeparator();
            CB_Directory = new ToolStripButton();
            CB_About = new ToolStripButton();
            toolStripSeparator3 = new ToolStripSeparator();
            CB_FontUp = new ToolStripButton();
            CB_FontDown = new ToolStripButton();
            CBox_FontSize = new ToolStripComboBox();
            splitContainer1 = new SplitContainer();
            TB_Edit = new RichTextBox();
            statusStrip1 = new StatusStrip();
            statusLineColumn = new ToolStripStatusLabel();
            toolStripStatusLabel1 = new ToolStripStatusLabel();
            statusFileInfo = new ToolStripStatusLabel();
            toolStripStatusLabel2 = new ToolStripStatusLabel();
            statusCapsLock = new ToolStripStatusLabel();
            TB_Console = new RichTextBox();
            menuStrip1.SuspendLayout();
            toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            statusStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.BackColor = SystemColors.Menu;
            menuStrip1.Items.AddRange(new ToolStripItem[] { TS_File, TS_Edit, TS_Reference });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(884, 24);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // TS_File
            // 
            TS_File.DropDownItems.AddRange(new ToolStripItem[] { TS_Open, TS_Create, TS_Save, TS_SaveAs, TS_Exit });
            TS_File.Name = "TS_File";
            TS_File.Size = new Size(48, 20);
            TS_File.Text = "Файл";
            // 
            // TS_Open
            // 
            TS_Open.Name = "TS_Open";
            TS_Open.Size = new Size(154, 22);
            TS_Open.Text = "Открыть Ctrl+O";
            TS_Open.Click += TS_Open_Click;
            // 
            // TS_Create
            // 
            TS_Create.Name = "TS_Create";
            TS_Create.Size = new Size(154, 22);
            TS_Create.Text = "Создать Ctrl+N";
            TS_Create.Click += TS_Create_Click;
            // 
            // TS_Save
            // 
            TS_Save.Name = "TS_Save";
            TS_Save.Size = new Size(154, 22);
            TS_Save.Text = "Сохранить Ctrl+S";
            TS_Save.Click += TS_Save_Click;
            // 
            // TS_SaveAs
            // 
            TS_SaveAs.Name = "TS_SaveAs";
            TS_SaveAs.Size = new Size(154, 22);
            TS_SaveAs.Text = "Сохранить как Ctrl+Shift+S";
            TS_SaveAs.Click += TS_SaveAs_Click;
            // 
            // TS_Exit
            // 
            TS_Exit.Name = "TS_Exit";
            TS_Exit.Size = new Size(154, 22);
            TS_Exit.Text = "Выход";
            TS_Exit.Click += TS_Exit_Click;
            // 
            // TS_Edit
            // 
            TS_Edit.DropDownItems.AddRange(new ToolStripItem[] { TS_Undo, TS_Redo, TS_Cut, TS_Copy, TS_Paste, TS_Delete, TS_SelectAll });
            TS_Edit.Name = "TS_Edit";
            TS_Edit.Size = new Size(59, 20);
            TS_Edit.Text = "Правка";
            // 
            // TS_Undo
            // 
            TS_Undo.Name = "TS_Undo";
            TS_Undo.Size = new Size(148, 22);
            TS_Undo.Text = "Отменить Ctrl+Z";
            TS_Undo.Click += TS_Undo_Click;
            // 
            // TS_Redo
            // 
            TS_Redo.Name = "TS_Redo";
            TS_Redo.Size = new Size(148, 22);
            TS_Redo.Text = "Повторить Ctrl+Y";
            TS_Redo.Click += TS_Redo_Click;
            // 
            // TS_Cut
            // 
            TS_Cut.Name = "TS_Cut";
            TS_Cut.Size = new Size(148, 22);
            TS_Cut.Text = "Вырезать Ctrl+X";
            TS_Cut.Click += TS_Cut_Click;
            // 
            // TS_Copy
            // 
            TS_Copy.Name = "TS_Copy";
            TS_Copy.Size = new Size(148, 22);
            TS_Copy.Text = "Копировать Ctrl+C";
            TS_Copy.Click += TS_Copy_Click;
            // 
            // TS_Paste
            // 
            TS_Paste.Name = "TS_Paste";
            TS_Paste.Size = new Size(148, 22);
            TS_Paste.Text = "Вставить Ctrl+V";
            TS_Paste.Click += TS_Paste_Click;
            // 
            // TS_Delete
            // 
            TS_Delete.Name = "TS_Delete";
            TS_Delete.Size = new Size(148, 22);
            TS_Delete.Text = "Удалить Delete";
            TS_Delete.Click += TS_Delete_Click;
            // 
            // TS_SelectAll
            // 
            TS_SelectAll.Name = "TS_SelectAll";
            TS_SelectAll.Size = new Size(148, 22);
            TS_SelectAll.Text = "Выделить всё Ctrl+A";
            TS_SelectAll.Click += TS_SelectAll_Click;
            // 
            // TS_Reference
            // 
            TS_Reference.DropDownItems.AddRange(new ToolStripItem[] { TS_Directory, TS_About });
            TS_Reference.Name = "TS_Reference";
            TS_Reference.Size = new Size(65, 20);
            TS_Reference.Text = "Справка";
            // 
            // TS_Directory
            // 
            TS_Directory.Name = "TS_Directory";
            TS_Directory.Size = new Size(149, 22);
            TS_Directory.Text = "Справочник F1";
            TS_Directory.Click += TS_Directory_Click;
            // 
            // TS_About
            // 
            TS_About.Name = "TS_About";
            TS_About.Size = new Size(149, 22);
            TS_About.Text = "О программе";
            TS_About.Click += TS_About_Click;
            // 
            // toolStrip1
            // 
            toolStrip1.BackColor = SystemColors.MenuBar;
            toolStrip1.Items.AddRange(new ToolStripItem[] { CB_Open, CB_Create, CB_Save, CB_SaveAs, CB_Exit, toolStripSeparator1, CB_Undo, CB_Redo, CB_Cut, CB_Copy, CB_Paste, CB_Delete, CB_SelectAll, toolStripSeparator2, CB_Directory, CB_About, toolStripSeparator3, CB_FontUp, CB_FontDown, CBox_FontSize });
            toolStrip1.Location = new Point(0, 24);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new Size(884, 25);
            toolStrip1.TabIndex = 1;
            toolStrip1.Text = "toolStrip1";
            // 
            // CB_Open
            // 
            CB_Open.DisplayStyle = ToolStripItemDisplayStyle.Image;
            CB_Open.Image = (Image)resources.GetObject("CB_Open.Image");
            CB_Open.ImageTransparentColor = Color.Magenta;
            CB_Open.Name = "CB_Open";
            CB_Open.Size = new Size(23, 22);
            CB_Open.Text = "Открыть";
            CB_Open.Click += CB_Open_Click;
            // 
            // CB_Create
            // 
            CB_Create.DisplayStyle = ToolStripItemDisplayStyle.Image;
            CB_Create.Image = (Image)resources.GetObject("CB_Create.Image");
            CB_Create.ImageTransparentColor = Color.Magenta;
            CB_Create.Name = "CB_Create";
            CB_Create.Size = new Size(23, 22);
            CB_Create.Text = "Создать";
            CB_Create.Click += CB_Create_Click;
            // 
            // CB_Save
            // 
            CB_Save.DisplayStyle = ToolStripItemDisplayStyle.Image;
            CB_Save.Image = (Image)resources.GetObject("CB_Save.Image");
            CB_Save.ImageTransparentColor = Color.Magenta;
            CB_Save.Name = "CB_Save";
            CB_Save.Size = new Size(23, 22);
            CB_Save.Text = "Сохранить";
            CB_Save.Click += CB_Save_Click;
            // 
            // CB_SaveAs
            // 
            CB_SaveAs.DisplayStyle = ToolStripItemDisplayStyle.Image;
            CB_SaveAs.Image = (Image)resources.GetObject("CB_SaveAs.Image");
            CB_SaveAs.ImageTransparentColor = Color.Magenta;
            CB_SaveAs.Name = "CB_SaveAs";
            CB_SaveAs.Size = new Size(23, 22);
            CB_SaveAs.Text = "Сохранить как";
            CB_SaveAs.Click += CB_SaveAs_Click;
            // 
            // CB_Exit
            // 
            CB_Exit.DisplayStyle = ToolStripItemDisplayStyle.Image;
            CB_Exit.Image = (Image)resources.GetObject("CB_Exit.Image");
            CB_Exit.ImageTransparentColor = Color.Magenta;
            CB_Exit.Name = "CB_Exit";
            CB_Exit.Size = new Size(23, 22);
            CB_Exit.Text = "Выход";
            CB_Exit.Click += CB_Exit_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(6, 25);
            // 
            // CB_Undo
            // 
            CB_Undo.DisplayStyle = ToolStripItemDisplayStyle.Image;
            CB_Undo.Image = (Image)resources.GetObject("CB_Undo.Image");
            CB_Undo.ImageTransparentColor = Color.Magenta;
            CB_Undo.Name = "CB_Undo";
            CB_Undo.Size = new Size(23, 22);
            CB_Undo.Text = "Отменить";
            CB_Undo.Click += CB_Undo_Click;
            // 
            // CB_Redo
            // 
            CB_Redo.DisplayStyle = ToolStripItemDisplayStyle.Image;
            CB_Redo.Image = (Image)resources.GetObject("CB_Redo.Image");
            CB_Redo.ImageTransparentColor = Color.Magenta;
            CB_Redo.Name = "CB_Redo";
            CB_Redo.Size = new Size(23, 22);
            CB_Redo.Text = "Повторить";
            CB_Redo.Click += CB_Redo_Click;
            // 
            // CB_Cut
            // 
            CB_Cut.DisplayStyle = ToolStripItemDisplayStyle.Image;
            CB_Cut.Image = (Image)resources.GetObject("CB_Cut.Image");
            CB_Cut.ImageTransparentColor = Color.Magenta;
            CB_Cut.Name = "CB_Cut";
            CB_Cut.Size = new Size(23, 22);
            CB_Cut.Text = "Вырезать";
            CB_Cut.Click += CB_Cut_Click;
            // 
            // CB_Copy
            // 
            CB_Copy.DisplayStyle = ToolStripItemDisplayStyle.Image;
            CB_Copy.Image = (Image)resources.GetObject("CB_Copy.Image");
            CB_Copy.ImageTransparentColor = Color.Magenta;
            CB_Copy.Name = "CB_Copy";
            CB_Copy.Size = new Size(23, 22);
            CB_Copy.Text = "Копировать";
            CB_Copy.Click += CB_Copy_Click;
            // 
            // CB_Paste
            // 
            CB_Paste.DisplayStyle = ToolStripItemDisplayStyle.Image;
            CB_Paste.Image = (Image)resources.GetObject("CB_Paste.Image");
            CB_Paste.ImageTransparentColor = Color.Magenta;
            CB_Paste.Name = "CB_Paste";
            CB_Paste.Size = new Size(23, 22);
            CB_Paste.Text = "Вставить";
            CB_Paste.Click += CB_Paste_Click;
            // 
            // CB_Delete
            // 
            CB_Delete.DisplayStyle = ToolStripItemDisplayStyle.Image;
            CB_Delete.Image = (Image)resources.GetObject("CB_Delete.Image");
            CB_Delete.ImageTransparentColor = Color.Magenta;
            CB_Delete.Name = "CB_Delete";
            CB_Delete.Size = new Size(23, 22);
            CB_Delete.Text = "Удалить";
            CB_Delete.Click += CB_Delete_Click;
            // 
            // CB_SelectAll
            // 
            CB_SelectAll.DisplayStyle = ToolStripItemDisplayStyle.Image;
            CB_SelectAll.Image = (Image)resources.GetObject("CB_SelectAll.Image");
            CB_SelectAll.ImageTransparentColor = Color.Magenta;
            CB_SelectAll.Name = "CB_SelectAll";
            CB_SelectAll.Size = new Size(23, 22);
            CB_SelectAll.Text = "Выделить все";
            CB_SelectAll.Click += CB_SelectAll_Click;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(6, 25);
            // 
            // CB_Directory
            // 
            CB_Directory.DisplayStyle = ToolStripItemDisplayStyle.Image;
            CB_Directory.Image = (Image)resources.GetObject("CB_Directory.Image");
            CB_Directory.ImageTransparentColor = Color.Magenta;
            CB_Directory.Name = "CB_Directory";
            CB_Directory.Size = new Size(23, 22);
            CB_Directory.Text = "Справочник";
            CB_Directory.Click += CB_Directory_Click;
            // 
            // CB_About
            // 
            CB_About.DisplayStyle = ToolStripItemDisplayStyle.Image;
            CB_About.Image = (Image)resources.GetObject("CB_About.Image");
            CB_About.ImageTransparentColor = Color.Magenta;
            CB_About.Name = "CB_About";
            CB_About.Size = new Size(23, 22);
            CB_About.Text = "О программе";
            CB_About.Click += CB_About_Click;
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new Size(6, 25);
            // 
            // CB_FontUp
            // 
            CB_FontUp.DisplayStyle = ToolStripItemDisplayStyle.Image;
            CB_FontUp.Image = (Image)resources.GetObject("CB_FontUp.Image");
            CB_FontUp.ImageTransparentColor = Color.Magenta;
            CB_FontUp.Name = "CB_FontUp";
            CB_FontUp.Size = new Size(23, 22);
            CB_FontUp.Text = "Увеличить шрифт\nCtrl+Plus";
            CB_FontUp.Click += CB_FontUp_Click;
            // 
            // CB_FontDown
            // 
            CB_FontDown.DisplayStyle = ToolStripItemDisplayStyle.Image;
            CB_FontDown.Image = (Image)resources.GetObject("CB_FontDown.Image");
            CB_FontDown.ImageTransparentColor = Color.Magenta;
            CB_FontDown.Name = "CB_FontDown";
            CB_FontDown.Size = new Size(23, 22);
            CB_FontDown.Text = "Уменьшить шрифт\nCtrl+Minus";
            CB_FontDown.Click += CB_FontDown_Click;
            // 
            // CBox_FontSize
            // 
            CBox_FontSize.Items.AddRange(new object[] { "6", "7", "8", "9", "10", "11", "12", "14", "16", "18", "20", "22", "24", "26", "28", "36", "48", "72" });
            CBox_FontSize.Name = "CBox_FontSize";
            CBox_FontSize.Size = new Size(75, 25);
            CBox_FontSize.TextChanged += CBox_FontSize_SelectedIndexChanged;
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 49);
            splitContainer1.Name = "splitContainer1";
            splitContainer1.Orientation = Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(TB_Edit);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(statusStrip1);
            splitContainer1.Panel2.Controls.Add(TB_Console);
            splitContainer1.Size = new Size(884, 512);
            splitContainer1.SplitterDistance = 294;
            splitContainer1.TabIndex = 2;
            // 
            // TB_Edit
            // 
            TB_Edit.AcceptsTab = true;
            TB_Edit.Dock = DockStyle.Fill;
            TB_Edit.Location = new Point(0, 0);
            TB_Edit.Name = "TB_Edit";
            TB_Edit.Size = new Size(884, 294);
            TB_Edit.TabIndex = 1;
            TB_Edit.Text = "";
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { statusLineColumn, toolStripStatusLabel1, statusFileInfo, toolStripStatusLabel2, statusCapsLock });
            statusStrip1.Location = new Point(0, 192);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(884, 22);
            statusStrip1.TabIndex = 1;
            statusStrip1.Text = "statusStrip1";
            // 
            // statusLineColumn
            // 
            statusLineColumn.Name = "statusLineColumn";
            statusLineColumn.Size = new Size(123, 17);
            statusLineColumn.Text = "Строка: 1, Столбец: 1";
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Size = new Size(112, 17);
            toolStripStatusLabel1.Text = "                                   ";
            // 
            // statusFileInfo
            // 
            statusFileInfo.Name = "statusFileInfo";
            statusFileInfo.Size = new Size(129, 17);
            statusFileInfo.Text = "Информация о файле";
            // 
            // toolStripStatusLabel2
            // 
            toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            toolStripStatusLabel2.Size = new Size(112, 17);
            toolStripStatusLabel2.Text = "                                   ";
            // 
            // statusCapsLock
            // 
            statusCapsLock.Name = "statusCapsLock";
            statusCapsLock.Size = new Size(36, 17);
            statusCapsLock.Text = "CAPS";
            statusCapsLock.TextAlign = ContentAlignment.MiddleRight;
            // 
            // TB_Console
            // 
            TB_Console.AcceptsTab = true;
            TB_Console.Dock = DockStyle.Fill;
            TB_Console.Location = new Point(0, 0);
            TB_Console.Name = "TB_Console";
            TB_Console.ReadOnly = true;
            TB_Console.Size = new Size(884, 214);
            TB_Console.TabIndex = 0;
            TB_Console.Text = "";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(884, 561);
            Controls.Add(splitContainer1);
            Controls.Add(toolStrip1);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Name = "Form1";
            Text = "MyCompiler";
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuStrip1;
        private ToolStripMenuItem TS_File;
        private ToolStripMenuItem TS_Open;
        private ToolStripMenuItem TS_Create;
        private ToolStripMenuItem TS_Save;
        private ToolStripMenuItem TS_SaveAs;
        private ToolStripMenuItem TS_Exit;
        private ToolStripMenuItem TS_Edit;
        private ToolStripMenuItem TS_Undo;
        private ToolStripMenuItem TS_Redo;
        private ToolStripMenuItem TS_Cut;
        private ToolStripMenuItem TS_Copy;
        private ToolStripMenuItem TS_Paste;
        private ToolStripMenuItem TS_Delete;
        private ToolStripMenuItem TS_SelectAll;
        private ToolStripMenuItem TS_Reference;
        private ToolStripMenuItem TS_Directory;
        private ToolStripMenuItem TS_About;
        private ToolStrip toolStrip1;
        private ToolStripButton CB_Open;
        private ToolStripButton CB_Create;
        private ToolStripButton CB_Save;
        private ToolStripButton CB_SaveAs;
        private ToolStripButton CB_Exit;
        private ToolStripButton CB_Undo;
        private ToolStripButton CB_Redo;
        private ToolStripButton CB_Cut;
        private ToolStripButton CB_Copy;
        private ToolStripButton CB_Paste;
        private ToolStripButton CB_Delete;
        private ToolStripButton CB_SelectAll;
        private ToolStripButton CB_Directory;
        private ToolStripButton CB_About;
        private SplitContainer splitContainer1;
        private RichTextBox TB_Console;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripButton CB_FontUp;
        private ToolStripButton CB_FontDown;
        private ToolStripComboBox CBox_FontSize;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel statusLineColumn;
        private ToolStripStatusLabel statusFileInfo;
        private ToolStripStatusLabel statusCapsLock;
        private ToolStripStatusLabel toolStripStatusLabel1;
        private ToolStripStatusLabel toolStripStatusLabel2;
        private RichTextBox TB_Edit;
    }
}
