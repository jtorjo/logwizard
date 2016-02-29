namespace lw_common.ui {
    partial class edit_log_settings_form {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(edit_log_settings_form));
            this.panel1 = new System.Windows.Forms.Panel();
            this.typeTab = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.browserFile = new System.Windows.Forms.Button();
            this.fileName = new System.Windows.Forms.TextBox();
            this.fileType = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.fileTypeTab = new System.Windows.Forms.TabControl();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.label6 = new System.Windows.Forms.Label();
            this.syntaxLink = new System.Windows.Forms.LinkLabel();
            this.syntax = new System.Windows.Forms.Label();
            this.editSyntax = new System.Windows.Forms.Button();
            this.ifLine = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.label18 = new System.Windows.Forms.Label();
            this.partSeparator = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.label19 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.xmlDelimeter = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.tabPage6 = new System.Windows.Forms.TabPage();
            this.label20 = new System.Windows.Forms.Label();
            this.csvHasHeader = new System.Windows.Forms.CheckBox();
            this.csvSeparator = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.testLogSizes = new System.Windows.Forms.Button();
            this.label27 = new System.Windows.Forms.Label();
            this.eventLogCheckStatus = new System.Windows.Forms.Label();
            this.eventLogs = new System.Windows.Forms.CheckedListBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.eventLogRemoteSstatus = new System.Windows.Forms.Label();
            this.remoteDomain = new System.Windows.Forms.TextBox();
            this.label26 = new System.Windows.Forms.Label();
            this.label25 = new System.Windows.Forms.Label();
            this.remotePassword = new System.Windows.Forms.TextBox();
            this.label24 = new System.Windows.Forms.Label();
            this.remoteUserName = new System.Windows.Forms.TextBox();
            this.label23 = new System.Windows.Forms.Label();
            this.testRemote = new System.Windows.Forms.Button();
            this.label22 = new System.Windows.Forms.Label();
            this.remoteMachineName = new System.Windows.Forms.TextBox();
            this.label21 = new System.Windows.Forms.Label();
            this.selectedEventLogs = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.tabPage7 = new System.Windows.Forms.TabPage();
            this.debugGlobal = new System.Windows.Forms.CheckBox();
            this.label29 = new System.Windows.Forms.Label();
            this.debugProcessName = new System.Windows.Forms.TextBox();
            this.label28 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.tabPage8 = new System.Windows.Forms.TabPage();
            this.label16 = new System.Windows.Forms.Label();
            this.tabPage9 = new System.Windows.Forms.TabPage();
            this.label17 = new System.Windows.Forms.Label();
            this.reversed = new System.Windows.Forms.CheckBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.type = new System.Windows.Forms.ComboBox();
            this.ok = new System.Windows.Forms.Button();
            this.cancel = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.friendlyName = new System.Windows.Forms.TextBox();
            this.tip = new System.Windows.Forms.ToolTip(this.components);
            this.needsRestart = new System.Windows.Forms.Label();
            this.checkRequiresRestart = new System.Windows.Forms.Timer(this.components);
            this.ofd = new System.Windows.Forms.OpenFileDialog();
            this.ifLineStartsWithTab = new System.Windows.Forms.CheckBox();
            this.panel1.SuspendLayout();
            this.typeTab.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.fileTypeTab.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.tabPage5.SuspendLayout();
            this.tabPage6.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabPage7.SuspendLayout();
            this.tabPage8.SuspendLayout();
            this.tabPage9.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.typeTab);
            this.panel1.Location = new System.Drawing.Point(-3, 34);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(628, 406);
            this.panel1.TabIndex = 0;
            // 
            // typeTab
            // 
            this.typeTab.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.typeTab.Controls.Add(this.tabPage1);
            this.typeTab.Controls.Add(this.tabPage2);
            this.typeTab.Controls.Add(this.tabPage7);
            this.typeTab.Controls.Add(this.tabPage8);
            this.typeTab.Controls.Add(this.tabPage9);
            this.typeTab.Location = new System.Drawing.Point(1, 3);
            this.typeTab.Name = "typeTab";
            this.typeTab.SelectedIndex = 0;
            this.typeTab.Size = new System.Drawing.Size(633, 403);
            this.typeTab.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.browserFile);
            this.tabPage1.Controls.Add(this.fileName);
            this.tabPage1.Controls.Add(this.fileType);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Controls.Add(this.panel2);
            this.tabPage1.Location = new System.Drawing.Point(4, 25);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(625, 374);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // browserFile
            // 
            this.browserFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.browserFile.Location = new System.Drawing.Point(442, 8);
            this.browserFile.Name = "browserFile";
            this.browserFile.Size = new System.Drawing.Size(29, 27);
            this.browserFile.TabIndex = 11;
            this.browserFile.Text = "...";
            this.tip.SetToolTip(this.browserFile, "Browse For File");
            this.browserFile.UseVisualStyleBackColor = true;
            this.browserFile.Click += new System.EventHandler(this.browserFile_Click);
            // 
            // fileName
            // 
            this.fileName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fileName.Location = new System.Drawing.Point(6, 11);
            this.fileName.Name = "fileName";
            this.fileName.ReadOnly = true;
            this.fileName.Size = new System.Drawing.Size(431, 23);
            this.fileName.TabIndex = 10;
            // 
            // fileType
            // 
            this.fileType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.fileType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.fileType.FormattingEnabled = true;
            this.fileType.Items.AddRange(new object[] {
            "Best Guess",
            "Line-By-Line",
            "Part-Per-Line",
            "XML",
            "CSV"});
            this.fileType.Location = new System.Drawing.Point(510, 9);
            this.fileType.Name = "fileType";
            this.fileType.Size = new System.Drawing.Size(109, 24);
            this.fileType.TabIndex = 7;
            this.fileType.SelectedIndexChanged += new System.EventHandler(this.fileType_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(471, 13);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(40, 17);
            this.label3.TabIndex = 6;
            this.label3.Text = "Type";
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.Controls.Add(this.fileTypeTab);
            this.panel2.Location = new System.Drawing.Point(2, 41);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(595, 261);
            this.panel2.TabIndex = 0;
            // 
            // fileTypeTab
            // 
            this.fileTypeTab.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fileTypeTab.Controls.Add(this.tabPage3);
            this.fileTypeTab.Controls.Add(this.tabPage4);
            this.fileTypeTab.Controls.Add(this.tabPage5);
            this.fileTypeTab.Controls.Add(this.tabPage6);
            this.fileTypeTab.Location = new System.Drawing.Point(-5, 0);
            this.fileTypeTab.Name = "fileTypeTab";
            this.fileTypeTab.SelectedIndex = 0;
            this.fileTypeTab.Size = new System.Drawing.Size(617, 258);
            this.fileTypeTab.TabIndex = 0;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.ifLineStartsWithTab);
            this.tabPage3.Controls.Add(this.label6);
            this.tabPage3.Controls.Add(this.syntaxLink);
            this.tabPage3.Controls.Add(this.syntax);
            this.tabPage3.Controls.Add(this.editSyntax);
            this.tabPage3.Controls.Add(this.ifLine);
            this.tabPage3.Controls.Add(this.label5);
            this.tabPage3.Location = new System.Drawing.Point(4, 25);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(609, 229);
            this.tabPage3.TabIndex = 0;
            this.tabPage3.Text = "tabPage3";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(0, 27);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(542, 45);
            this.label6.TabIndex = 7;
            this.label6.Text = resources.GetString("label6.Text");
            // 
            // syntaxLink
            // 
            this.syntaxLink.AutoSize = true;
            this.syntaxLink.Cursor = System.Windows.Forms.Cursors.Hand;
            this.syntaxLink.Location = new System.Drawing.Point(2, 81);
            this.syntaxLink.Name = "syntaxLink";
            this.syntaxLink.Size = new System.Drawing.Size(50, 17);
            this.syntaxLink.TabIndex = 6;
            this.syntaxLink.TabStop = true;
            this.syntaxLink.Text = "Syntax";
            this.syntaxLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.syntaxLink_LinkClicked);
            // 
            // syntax
            // 
            this.syntax.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.syntax.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.syntax.Font = new System.Drawing.Font("Courier New", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.syntax.Location = new System.Drawing.Point(60, 81);
            this.syntax.Name = "syntax";
            this.syntax.Size = new System.Drawing.Size(454, 60);
            this.syntax.TabIndex = 5;
            // 
            // editSyntax
            // 
            this.editSyntax.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.editSyntax.Location = new System.Drawing.Point(521, 76);
            this.editSyntax.Name = "editSyntax";
            this.editSyntax.Size = new System.Drawing.Size(63, 27);
            this.editSyntax.TabIndex = 4;
            this.editSyntax.Text = "Edit";
            this.editSyntax.UseVisualStyleBackColor = true;
            this.editSyntax.Click += new System.EventHandler(this.editSyntax_Click);
            // 
            // ifLine
            // 
            this.ifLine.AutoSize = true;
            this.ifLine.Location = new System.Drawing.Point(60, 147);
            this.ifLine.Name = "ifLine";
            this.ifLine.Size = new System.Drawing.Size(400, 21);
            this.ifLine.TabIndex = 1;
            this.ifLine.Text = "If Line doesn\'t match syntax, consider it\'s from previous line";
            this.ifLine.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(0, 6);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(86, 17);
            this.label5.TabIndex = 0;
            this.label5.Text = "Line By Line";
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.label18);
            this.tabPage4.Controls.Add(this.partSeparator);
            this.tabPage4.Controls.Add(this.label8);
            this.tabPage4.Controls.Add(this.label7);
            this.tabPage4.Location = new System.Drawing.Point(4, 25);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(609, 229);
            this.tabPage4.TabIndex = 1;
            this.tabPage4.Text = "tabPage4";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label18.Location = new System.Drawing.Point(0, 31);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(469, 45);
            this.label18.TabIndex = 8;
            this.label18.Text = "On these logs, a log entry spans multiple lines. Each line contains a part of the" +
    " log entry.\r\nEach line contains a \"NAME separator VALUE\" part. \r\nAn empty line s" +
    "ignals the end of the log entry.";
            // 
            // partSeparator
            // 
            this.partSeparator.Location = new System.Drawing.Point(120, 83);
            this.partSeparator.Name = "partSeparator";
            this.partSeparator.Size = new System.Drawing.Size(31, 23);
            this.partSeparator.TabIndex = 2;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(0, 86);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(105, 17);
            this.label8.TabIndex = 1;
            this.label8.Text = "Separator Char";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(0, 10);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(93, 17);
            this.label7.TabIndex = 0;
            this.label7.Text = "Part-Per-Line";
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.label19);
            this.tabPage5.Controls.Add(this.label13);
            this.tabPage5.Controls.Add(this.xmlDelimeter);
            this.tabPage5.Controls.Add(this.label12);
            this.tabPage5.Controls.Add(this.label9);
            this.tabPage5.Location = new System.Drawing.Point(4, 25);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage5.Size = new System.Drawing.Size(609, 229);
            this.tabPage5.TabIndex = 2;
            this.tabPage5.Text = "tabPage5";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label19.Location = new System.Drawing.Point(0, 31);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(162, 30);
            this.label19.TabIndex = 9;
            this.label19.Text = "This is your usual XML log.\r\nEach XML entry is a log entry.";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(0, 105);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(347, 30);
            this.label13.TabIndex = 7;
            this.label13.Text = "This is the node that separates each log entry.\r\nIf not set, LogWizard will consi" +
    "der the first node as the delimeter.";
            // 
            // xmlDelimeter
            // 
            this.xmlDelimeter.Location = new System.Drawing.Point(185, 77);
            this.xmlDelimeter.Name = "xmlDelimeter";
            this.xmlDelimeter.Size = new System.Drawing.Size(224, 23);
            this.xmlDelimeter.TabIndex = 6;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(0, 80);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(172, 17);
            this.label12.TabIndex = 5;
            this.label12.Text = "XML Delimeter Node (opt)";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(0, 6);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(62, 17);
            this.label9.TabIndex = 0;
            this.label9.Text = "XML File";
            // 
            // tabPage6
            // 
            this.tabPage6.Controls.Add(this.label20);
            this.tabPage6.Controls.Add(this.csvHasHeader);
            this.tabPage6.Controls.Add(this.csvSeparator);
            this.tabPage6.Controls.Add(this.label11);
            this.tabPage6.Controls.Add(this.label10);
            this.tabPage6.Location = new System.Drawing.Point(4, 25);
            this.tabPage6.Name = "tabPage6";
            this.tabPage6.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage6.Size = new System.Drawing.Size(609, 229);
            this.tabPage6.TabIndex = 3;
            this.tabPage6.Text = "tabPage6";
            this.tabPage6.UseVisualStyleBackColor = true;
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label20.Location = new System.Drawing.Point(0, 32);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(532, 45);
            this.label20.TabIndex = 10;
            this.label20.Text = "This is a Comma-Separated-Values log.\r\nEach line contains a long entry, each part" +
    " is separated by the separator character (usually a comma).\r\nBy default, the fir" +
    "st line contains the Column Names.";
            // 
            // csvHasHeader
            // 
            this.csvHasHeader.AutoSize = true;
            this.csvHasHeader.Location = new System.Drawing.Point(10, 115);
            this.csvHasHeader.Name = "csvHasHeader";
            this.csvHasHeader.Size = new System.Drawing.Size(267, 21);
            this.csvHasHeader.TabIndex = 5;
            this.csvHasHeader.Text = "First Line Contains the Column Names";
            this.csvHasHeader.UseVisualStyleBackColor = true;
            // 
            // csvSeparator
            // 
            this.csvSeparator.Location = new System.Drawing.Point(115, 86);
            this.csvSeparator.Name = "csvSeparator";
            this.csvSeparator.Size = new System.Drawing.Size(31, 23);
            this.csvSeparator.TabIndex = 4;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(0, 89);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(105, 17);
            this.label11.TabIndex = 3;
            this.label11.Text = "Separator Char";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(0, 7);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(61, 17);
            this.label10.TabIndex = 0;
            this.label10.Text = "CSV File";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.testLogSizes);
            this.tabPage2.Controls.Add(this.label27);
            this.tabPage2.Controls.Add(this.eventLogCheckStatus);
            this.tabPage2.Controls.Add(this.eventLogs);
            this.tabPage2.Controls.Add(this.groupBox1);
            this.tabPage2.Controls.Add(this.selectedEventLogs);
            this.tabPage2.Controls.Add(this.label14);
            this.tabPage2.Location = new System.Drawing.Point(4, 25);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(625, 374);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // testLogSizes
            // 
            this.testLogSizes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.testLogSizes.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.testLogSizes.Location = new System.Drawing.Point(559, 103);
            this.testLogSizes.Name = "testLogSizes";
            this.testLogSizes.Size = new System.Drawing.Size(52, 20);
            this.testLogSizes.TabIndex = 6;
            this.testLogSizes.Text = "Test";
            this.tip.SetToolTip(this.testLogSizes, "Tests to see if the logs you selected actually have any entries\r\n(in case you sel" +
        "ected some logs for which we can\'t easily query that)");
            this.testLogSizes.UseVisualStyleBackColor = true;
            this.testLogSizes.Click += new System.EventHandler(this.testLogSizes_Click);
            // 
            // label27
            // 
            this.label27.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label27.AutoSize = true;
            this.label27.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label27.Location = new System.Drawing.Point(358, 212);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(270, 156);
            this.label27.TabIndex = 5;
            this.label27.Text = resources.GetString("label27.Text");
            // 
            // eventLogCheckStatus
            // 
            this.eventLogCheckStatus.AutoSize = true;
            this.eventLogCheckStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.eventLogCheckStatus.Location = new System.Drawing.Point(123, 107);
            this.eventLogCheckStatus.Name = "eventLogCheckStatus";
            this.eventLogCheckStatus.Size = new System.Drawing.Size(331, 13);
            this.eventLogCheckStatus.TabIndex = 4;
            this.eventLogCheckStatus.Text = "One entry per line. Edit it manually only if you know what you\'re doing";
            // 
            // eventLogs
            // 
            this.eventLogs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.eventLogs.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.eventLogs.FormattingEnabled = true;
            this.eventLogs.Location = new System.Drawing.Point(124, 124);
            this.eventLogs.Name = "eventLogs";
            this.eventLogs.Size = new System.Drawing.Size(488, 68);
            this.eventLogs.TabIndex = 3;
            this.eventLogs.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.eventLogs_ItemCheck);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox1.Controls.Add(this.eventLogRemoteSstatus);
            this.groupBox1.Controls.Add(this.remoteDomain);
            this.groupBox1.Controls.Add(this.label26);
            this.groupBox1.Controls.Add(this.label25);
            this.groupBox1.Controls.Add(this.remotePassword);
            this.groupBox1.Controls.Add(this.label24);
            this.groupBox1.Controls.Add(this.remoteUserName);
            this.groupBox1.Controls.Add(this.label23);
            this.groupBox1.Controls.Add(this.testRemote);
            this.groupBox1.Controls.Add(this.label22);
            this.groupBox1.Controls.Add(this.remoteMachineName);
            this.groupBox1.Controls.Add(this.label21);
            this.groupBox1.Location = new System.Drawing.Point(8, 212);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(346, 152);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Connect to Remote Computer ";
            // 
            // eventLogRemoteSstatus
            // 
            this.eventLogRemoteSstatus.AutoSize = true;
            this.eventLogRemoteSstatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.eventLogRemoteSstatus.ForeColor = System.Drawing.Color.Red;
            this.eventLogRemoteSstatus.Location = new System.Drawing.Point(8, 122);
            this.eventLogRemoteSstatus.Name = "eventLogRemoteSstatus";
            this.eventLogRemoteSstatus.Size = new System.Drawing.Size(206, 17);
            this.eventLogRemoteSstatus.TabIndex = 15;
            this.eventLogRemoteSstatus.Text = "NOT connected at this time";
            // 
            // remoteDomain
            // 
            this.remoteDomain.Location = new System.Drawing.Point(256, 31);
            this.remoteDomain.Name = "remoteDomain";
            this.remoteDomain.Size = new System.Drawing.Size(84, 23);
            this.remoteDomain.TabIndex = 2;
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(177, 34);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(56, 17);
            this.label26.TabIndex = 13;
            this.label26.Text = "Domain";
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label25.Location = new System.Drawing.Point(248, 98);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(96, 13);
            this.label25.TabIndex = 12;
            this.label25.Text = "NOT saved to disk";
            // 
            // remotePassword
            // 
            this.remotePassword.Location = new System.Drawing.Point(256, 72);
            this.remotePassword.Name = "remotePassword";
            this.remotePassword.PasswordChar = '*';
            this.remotePassword.Size = new System.Drawing.Size(84, 23);
            this.remotePassword.TabIndex = 4;
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(179, 75);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(69, 17);
            this.label24.TabIndex = 10;
            this.label24.Text = "Password";
            // 
            // remoteUserName
            // 
            this.remoteUserName.Location = new System.Drawing.Point(70, 72);
            this.remoteUserName.Name = "remoteUserName";
            this.remoteUserName.Size = new System.Drawing.Size(86, 23);
            this.remoteUserName.TabIndex = 3;
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(7, 75);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(38, 17);
            this.label23.TabIndex = 8;
            this.label23.Text = "User";
            // 
            // testRemote
            // 
            this.testRemote.Location = new System.Drawing.Point(282, 118);
            this.testRemote.Name = "testRemote";
            this.testRemote.Size = new System.Drawing.Size(58, 26);
            this.testRemote.TabIndex = 7;
            this.testRemote.Text = "Test";
            this.testRemote.UseVisualStyleBackColor = true;
            this.testRemote.Click += new System.EventHandler(this.testRemote_Click);
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label22.Location = new System.Drawing.Point(67, 54);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(183, 13);
            this.label22.TabIndex = 2;
            this.label22.Text = "Leave Empty if it\'s the Local Machine";
            // 
            // remoteMachineName
            // 
            this.remoteMachineName.Location = new System.Drawing.Point(70, 28);
            this.remoteMachineName.Name = "remoteMachineName";
            this.remoteMachineName.Size = new System.Drawing.Size(86, 23);
            this.remoteMachineName.TabIndex = 1;
            this.remoteMachineName.TextChanged += new System.EventHandler(this.remoteMachineName_TextChanged);
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(7, 31);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(61, 17);
            this.label21.TabIndex = 0;
            this.label21.Text = "Machine";
            // 
            // selectedEventLogs
            // 
            this.selectedEventLogs.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.selectedEventLogs.Font = new System.Drawing.Font("Courier New", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.selectedEventLogs.Location = new System.Drawing.Point(124, 6);
            this.selectedEventLogs.Multiline = true;
            this.selectedEventLogs.Name = "selectedEventLogs";
            this.selectedEventLogs.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.selectedEventLogs.Size = new System.Drawing.Size(487, 96);
            this.selectedEventLogs.TabIndex = 1;
            this.selectedEventLogs.Enter += new System.EventHandler(this.selectedEventLogs_Enter);
            this.selectedEventLogs.Leave += new System.EventHandler(this.selectedEventLogs_Leave);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(10, 14);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(89, 17);
            this.label14.TabIndex = 0;
            this.label14.Text = "Event Log(s)";
            // 
            // tabPage7
            // 
            this.tabPage7.Controls.Add(this.debugGlobal);
            this.tabPage7.Controls.Add(this.label29);
            this.tabPage7.Controls.Add(this.debugProcessName);
            this.tabPage7.Controls.Add(this.label28);
            this.tabPage7.Controls.Add(this.label15);
            this.tabPage7.Location = new System.Drawing.Point(4, 25);
            this.tabPage7.Name = "tabPage7";
            this.tabPage7.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage7.Size = new System.Drawing.Size(625, 374);
            this.tabPage7.TabIndex = 2;
            this.tabPage7.Text = "tabPage7";
            this.tabPage7.UseVisualStyleBackColor = true;
            // 
            // debugGlobal
            // 
            this.debugGlobal.AutoSize = true;
            this.debugGlobal.Location = new System.Drawing.Point(10, 132);
            this.debugGlobal.Name = "debugGlobal";
            this.debugGlobal.Size = new System.Drawing.Size(166, 21);
            this.debugGlobal.TabIndex = 12;
            this.debugGlobal.Text = "Global Events As Well";
            this.debugGlobal.UseVisualStyleBackColor = true;
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label29.Location = new System.Drawing.Point(6, 71);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(275, 39);
            this.label29.TabIndex = 11;
            this.label29.Text = "By default, you will see the logs from all processes.\r\nYou can filter them only b" +
    "y the process that interests you.\r\nType here the process name (without its endin" +
    "g .EXE)";
            // 
            // debugProcessName
            // 
            this.debugProcessName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.debugProcessName.Location = new System.Drawing.Point(289, 39);
            this.debugProcessName.Name = "debugProcessName";
            this.debugProcessName.Size = new System.Drawing.Size(206, 23);
            this.debugProcessName.TabIndex = 10;
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Location = new System.Drawing.Point(6, 42);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(276, 17);
            this.label28.TabIndex = 1;
            this.label28.Text = "Show me Messages only from this Process";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(6, 15);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(357, 17);
            this.label15.TabIndex = 0;
            this.label15.Text = "Debug Viewer: anything written with OutputDebugString";
            // 
            // tabPage8
            // 
            this.tabPage8.Controls.Add(this.label16);
            this.tabPage8.Location = new System.Drawing.Point(4, 25);
            this.tabPage8.Name = "tabPage8";
            this.tabPage8.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage8.Size = new System.Drawing.Size(625, 374);
            this.tabPage8.TabIndex = 3;
            this.tabPage8.Text = "tabPage8";
            this.tabPage8.UseVisualStyleBackColor = true;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(9, 15);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(212, 17);
            this.label16.TabIndex = 0;
            this.label16.Text = "Database (Not implemented yet)";
            // 
            // tabPage9
            // 
            this.tabPage9.Controls.Add(this.label17);
            this.tabPage9.Location = new System.Drawing.Point(4, 25);
            this.tabPage9.Name = "tabPage9";
            this.tabPage9.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage9.Size = new System.Drawing.Size(625, 374);
            this.tabPage9.TabIndex = 4;
            this.tabPage9.Text = "tabPage9";
            this.tabPage9.UseVisualStyleBackColor = true;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(8, 13);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(234, 17);
            this.label17.TabIndex = 1;
            this.label17.Text = "Multiple Logs (Not implemented yet)";
            // 
            // reversed
            // 
            this.reversed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.reversed.AutoSize = true;
            this.reversed.Location = new System.Drawing.Point(7, 446);
            this.reversed.Name = "reversed";
            this.reversed.Size = new System.Drawing.Size(255, 21);
            this.reversed.TabIndex = 6;
            this.reversed.Text = "Reversed (Last Entry is shown First)";
            this.tip.SetToolTip(this.reversed, "Shows the Log entries in Reversed order\r\n\r\nThis can be useful if retrieving the l" +
        "ogs is time-consuming,\r\nand you\'d like to see the last entries ASAP");
            this.reversed.UseVisualStyleBackColor = true;
            // 
            // checkBox1
            // 
            this.checkBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBox1.AutoSize = true;
            this.checkBox1.Enabled = false;
            this.checkBox1.Location = new System.Drawing.Point(7, 469);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(94, 21);
            this.checkBox1.TabIndex = 1;
            this.checkBox1.Text = "Save Last ";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.textBox1.Enabled = false;
            this.textBox1.Location = new System.Drawing.Point(107, 467);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 23);
            this.textBox1.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Enabled = false;
            this.label1.Location = new System.Drawing.Point(213, 470);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 17);
            this.label1.TabIndex = 3;
            this.label1.Text = "sessions";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 7);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 17);
            this.label2.TabIndex = 4;
            this.label2.Text = "Log Type";
            // 
            // type
            // 
            this.type.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.type.Enabled = false;
            this.type.FormattingEnabled = true;
            this.type.Items.AddRange(new object[] {
            "File",
            "Windows Event Log",
            "Debug Viewer",
            "Database",
            "Multiple"});
            this.type.Location = new System.Drawing.Point(72, 4);
            this.type.Name = "type";
            this.type.Size = new System.Drawing.Size(190, 24);
            this.type.TabIndex = 5;
            this.type.SelectedIndexChanged += new System.EventHandler(this.type_SelectedIndexChanged);
            this.type.DropDownClosed += new System.EventHandler(this.type_DropDownClosed);
            // 
            // ok
            // 
            this.ok.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ok.Location = new System.Drawing.Point(564, 465);
            this.ok.Name = "ok";
            this.ok.Size = new System.Drawing.Size(58, 26);
            this.ok.TabIndex = 6;
            this.ok.Text = "OK";
            this.ok.UseVisualStyleBackColor = true;
            this.ok.Click += new System.EventHandler(this.ok_Click);
            // 
            // cancel
            // 
            this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancel.Location = new System.Drawing.Point(310, 468);
            this.cancel.Name = "cancel";
            this.cancel.Size = new System.Drawing.Size(61, 26);
            this.cancel.TabIndex = 7;
            this.cancel.Text = "Cancel";
            this.cancel.UseVisualStyleBackColor = true;
            this.cancel.Click += new System.EventHandler(this.cancel_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(274, 7);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(97, 17);
            this.label4.TabIndex = 8;
            this.label4.Text = "Friendly name";
            this.tip.SetToolTip(this.label4, "Friendly Name (optional)\r\nIf you set this, it will show up in History");
            // 
            // friendlyName
            // 
            this.friendlyName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.friendlyName.Location = new System.Drawing.Point(377, 5);
            this.friendlyName.Name = "friendlyName";
            this.friendlyName.Size = new System.Drawing.Size(241, 23);
            this.friendlyName.TabIndex = 9;
            // 
            // needsRestart
            // 
            this.needsRestart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.needsRestart.AutoSize = true;
            this.needsRestart.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.needsRestart.ForeColor = System.Drawing.Color.Red;
            this.needsRestart.Location = new System.Drawing.Point(431, 470);
            this.needsRestart.Name = "needsRestart";
            this.needsRestart.Size = new System.Drawing.Size(131, 17);
            this.needsRestart.TabIndex = 10;
            this.needsRestart.Text = "Requires Restart";
            this.needsRestart.Visible = false;
            // 
            // checkRequiresRestart
            // 
            this.checkRequiresRestart.Enabled = true;
            this.checkRequiresRestart.Interval = 250;
            this.checkRequiresRestart.Tick += new System.EventHandler(this.checkRequiresRestart_Tick);
            // 
            // ofd
            // 
            this.ofd.Filter = "Text Files|*.txt|Log Files|*.log|All Files|*.*";
            // 
            // ifLineStartsWithTab
            // 
            this.ifLineStartsWithTab.AutoSize = true;
            this.ifLineStartsWithTab.Location = new System.Drawing.Point(60, 170);
            this.ifLineStartsWithTab.Name = "ifLineStartsWithTab";
            this.ifLineStartsWithTab.Size = new System.Drawing.Size(360, 21);
            this.ifLineStartsWithTab.TabIndex = 8;
            this.ifLineStartsWithTab.Text = "If Line starts with Tab, consider it\'s from previous line";
            this.ifLineStartsWithTab.UseVisualStyleBackColor = true;
            // 
            // edit_log_settings_form
            // 
            this.AcceptButton = this.ok;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.White;
            this.CancelButton = this.cancel;
            this.ClientSize = new System.Drawing.Size(626, 493);
            this.Controls.Add(this.reversed);
            this.Controls.Add(this.needsRestart);
            this.Controls.Add(this.cancel);
            this.Controls.Add(this.friendlyName);
            this.Controls.Add(this.ok);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.type);
            this.Controls.Add(this.label2);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "edit_log_settings_form";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Edit Log Settings";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.edit_log_settings_form_FormClosing);
            this.panel1.ResumeLayout(false);
            this.typeTab.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.fileTypeTab.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            this.tabPage5.ResumeLayout(false);
            this.tabPage5.PerformLayout();
            this.tabPage6.ResumeLayout(false);
            this.tabPage6.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabPage7.ResumeLayout(false);
            this.tabPage7.PerformLayout();
            this.tabPage8.ResumeLayout(false);
            this.tabPage8.PerformLayout();
            this.tabPage9.ResumeLayout(false);
            this.tabPage9.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button ok;
        private System.Windows.Forms.ComboBox type;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.TabControl typeTab;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TabControl fileTypeTab;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.ComboBox fileType;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button cancel;
        private System.Windows.Forms.TextBox friendlyName;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ToolTip tip;
        private System.Windows.Forms.Button editSyntax;
        private System.Windows.Forms.CheckBox ifLine;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox partSeparator;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TabPage tabPage6;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox csvSeparator;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox xmlDelimeter;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TabPage tabPage7;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TabPage tabPage8;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TabPage tabPage9;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.CheckBox csvHasHeader;
        private System.Windows.Forms.Label syntax;
        private System.Windows.Forms.Label needsRestart;
        private System.Windows.Forms.Timer checkRequiresRestart;
        private System.Windows.Forms.LinkLabel syntaxLink;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.TextBox selectedEventLogs;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox remotePassword;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.TextBox remoteUserName;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.Button testRemote;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.TextBox remoteMachineName;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.CheckedListBox eventLogs;
        private System.Windows.Forms.TextBox remoteDomain;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.Label eventLogRemoteSstatus;
        private System.Windows.Forms.Label eventLogCheckStatus;
        private System.Windows.Forms.CheckBox debugGlobal;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.TextBox debugProcessName;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.Button browserFile;
        private System.Windows.Forms.TextBox fileName;
        private System.Windows.Forms.OpenFileDialog ofd;
        private System.Windows.Forms.CheckBox reversed;
        private System.Windows.Forms.Button testLogSizes;
        private System.Windows.Forms.CheckBox ifLineStartsWithTab;
    }
}