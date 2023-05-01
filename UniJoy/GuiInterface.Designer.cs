namespace UniJoy
{
    partial class GuiInterface
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this._protocolsComboBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this._protocolsFolderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            this._protocolBrowserBtn = new System.Windows.Forms.Button();
            this._btnStart = new System.Windows.Forms.Button();
            this._varyingListBox = new System.Windows.Forms.ListBox();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this._btnMakeTrials = new System.Windows.Forms.Button();
            this._btnStop = new System.Windows.Forms.Button();
            this._trialDetailsListView = new System.Windows.Forms.ListView();
            this._trialParametersGroup = new System.Windows.Forms.GroupBox();
            this._dynamicParametersPanel = new System.Windows.Forms.Panel();
            this._varyingControlGroupBox = new System.Windows.Forms.GroupBox();
            this._btnEnagae = new System.Windows.Forms.Button();
            this._btnPark = new System.Windows.Forms.Button();
            this._numOfRepetitionsTextBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.backgroundWorker2 = new System.ComponentModel.BackgroundWorker();
            this.backgroundWorker3 = new System.ComponentModel.BackgroundWorker();
            this._guiInterfaceToolTip = new System.Windows.Forms.ToolTip(this.components);
            this._trialInfoGroupBox = new System.Windows.Forms.GroupBox();
            this.globalExperimentInfogroupBox = new System.Windows.Forms.GroupBox();
            this._globaExperimentlInfoListView = new System.Windows.Forms.ListView();
            this._onlinePsychGrpahGroupBox = new System.Windows.Forms.GroupBox();
            this._onlinePsychGraphControl = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this._warningsGroupBox = new System.Windows.Forms.GroupBox();
            this._ardionoPrtWarningLabel = new System.Windows.Forms.Label();
            this._checkboxRRDelta = new System.Windows.Forms.CheckBox();
            this._checkBoxRightAndLeftSame = new System.Windows.Forms.CheckBox();
            this._checkBoxFixationOnly = new System.Windows.Forms.CheckBox();
            this._checkBoxEnableGoCue = new System.Windows.Forms.CheckBox();
            this._checkboxErrorSoundOn = new System.Windows.Forms.CheckBox();
            this._btnSaveProtocol = new System.Windows.Forms.Button();
            this._textboxNewProtocolName = new System.Windows.Forms.TextBox();
            this._labelNewProtocolName = new System.Windows.Forms.Label();
            this._groupboxGoCue = new System.Windows.Forms.GroupBox();
            this._radiobuttonGoCueCorrectSide = new System.Windows.Forms.RadioButton();
            this._radiobuttonGoCueBothSide = new System.Windows.Forms.RadioButton();
            this._trialParametersGroup.SuspendLayout();
            this._varyingControlGroupBox.SuspendLayout();
            this._trialInfoGroupBox.SuspendLayout();
            this.globalExperimentInfogroupBox.SuspendLayout();
            this._onlinePsychGrpahGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._onlinePsychGraphControl)).BeginInit();
            this._warningsGroupBox.SuspendLayout();
            this._groupboxGoCue.SuspendLayout();
            this.SuspendLayout();
            // 
            // _protocolsComboBox
            // 
            this._protocolsComboBox.FormattingEnabled = true;
            this._protocolsComboBox.Location = new System.Drawing.Point(12, 54);
            this._protocolsComboBox.Name = "_protocolsComboBox";
            this._protocolsComboBox.Size = new System.Drawing.Size(193, 21);
            this._protocolsComboBox.TabIndex = 0;
            this._protocolsComboBox.SelectedIndexChanged += new System.EventHandler(this._protocolsComboBox_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 35);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(95, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Protocols In Folder";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(208, 35);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(130, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Protocols Directory Path...";
            // 
            // _protocolsFolderBrowser
            // 
            this._protocolsFolderBrowser.SelectedPath = "C:\\Users\\user\\source\\repos\\UniJoy\\Protocols";
            // 
            // _protocolBrowserBtn
            // 
            this._protocolBrowserBtn.Location = new System.Drawing.Point(211, 51);
            this._protocolBrowserBtn.Name = "_protocolBrowserBtn";
            this._protocolBrowserBtn.Size = new System.Drawing.Size(75, 23);
            this._protocolBrowserBtn.TabIndex = 3;
            this._protocolBrowserBtn.Text = "Browse Folder...";
            this._protocolBrowserBtn.UseVisualStyleBackColor = true;
            this._protocolBrowserBtn.Click += new System.EventHandler(this.protocolBrowserBtn_Click);
            // 
            // _btnStart
            // 
            this._btnStart.Location = new System.Drawing.Point(6, 409);
            this._btnStart.Name = "_btnStart";
            this._btnStart.Size = new System.Drawing.Size(75, 23);
            this._btnStart.TabIndex = 4;
            this._btnStart.Text = "Start";
            this._btnStart.UseVisualStyleBackColor = true;
            this._btnStart.Click += new System.EventHandler(this._btnStart_Click);
            // 
            // _varyingListBox
            // 
            this._varyingListBox.FormattingEnabled = true;
            this._varyingListBox.IntegralHeight = false;
            this._varyingListBox.Location = new System.Drawing.Point(87, 19);
            this._varyingListBox.Name = "_varyingListBox";
            this._varyingListBox.Size = new System.Drawing.Size(240, 384);
            this._varyingListBox.TabIndex = 5;
            // 
            // _btnMakeTrials
            // 
            this._btnMakeTrials.Location = new System.Drawing.Point(6, 380);
            this._btnMakeTrials.Name = "_btnMakeTrials";
            this._btnMakeTrials.Size = new System.Drawing.Size(75, 23);
            this._btnMakeTrials.TabIndex = 8;
            this._btnMakeTrials.Text = "MakeTrials";
            this._btnMakeTrials.UseVisualStyleBackColor = true;
            this._btnMakeTrials.Click += new System.EventHandler(this._makeTrials_Click);
            // 
            // _btnStop
            // 
            this._btnStop.Location = new System.Drawing.Point(6, 438);
            this._btnStop.Name = "_btnStop";
            this._btnStop.Size = new System.Drawing.Size(75, 23);
            this._btnStop.TabIndex = 9;
            this._btnStop.Text = "Stop";
            this._btnStop.UseVisualStyleBackColor = true;
            this._btnStop.Click += new System.EventHandler(this._btnStop_Click);
            // 
            // _trialDetailsListView
            // 
            this._trialDetailsListView.HoverSelection = true;
            this._trialDetailsListView.Location = new System.Drawing.Point(6, 16);
            this._trialDetailsListView.Name = "_trialDetailsListView";
            this._trialDetailsListView.Size = new System.Drawing.Size(712, 156);
            this._trialDetailsListView.TabIndex = 10;
            this._trialDetailsListView.UseCompatibleStateImageBehavior = false;
            // 
            // _trialParametersGroup
            // 
            this._trialParametersGroup.Controls.Add(this._dynamicParametersPanel);
            this._trialParametersGroup.Location = new System.Drawing.Point(15, 81);
            this._trialParametersGroup.Name = "_trialParametersGroup";
            this._trialParametersGroup.Size = new System.Drawing.Size(909, 798);
            this._trialParametersGroup.TabIndex = 11;
            this._trialParametersGroup.TabStop = false;
            this._trialParametersGroup.Text = "Parameters";
            // 
            // _dynamicParametersPanel
            // 
            this._dynamicParametersPanel.AutoScroll = true;
            this._dynamicParametersPanel.Location = new System.Drawing.Point(6, 19);
            this._dynamicParametersPanel.Name = "_dynamicParametersPanel";
            this._dynamicParametersPanel.Size = new System.Drawing.Size(897, 779);
            this._dynamicParametersPanel.TabIndex = 12;
            this._dynamicParametersPanel.Paint += new System.Windows.Forms.PaintEventHandler(this._dynamicParametersPanel_Paint);
            // 
            // _varyingControlGroupBox
            // 
            this._varyingControlGroupBox.Controls.Add(this._btnEnagae);
            this._varyingControlGroupBox.Controls.Add(this._btnPark);
            this._varyingControlGroupBox.Controls.Add(this._numOfRepetitionsTextBox);
            this._varyingControlGroupBox.Controls.Add(this.label5);
            this._varyingControlGroupBox.Controls.Add(this._varyingListBox);
            this._varyingControlGroupBox.Controls.Add(this._btnStart);
            this._varyingControlGroupBox.Controls.Add(this._btnMakeTrials);
            this._varyingControlGroupBox.Controls.Add(this._btnStop);
            this._varyingControlGroupBox.Location = new System.Drawing.Point(930, 81);
            this._varyingControlGroupBox.Name = "_varyingControlGroupBox";
            this._varyingControlGroupBox.Size = new System.Drawing.Size(339, 473);
            this._varyingControlGroupBox.TabIndex = 13;
            this._varyingControlGroupBox.TabStop = false;
            this._varyingControlGroupBox.Text = "Varying Control";
            // 
            // _btnEnagae
            // 
            this._btnEnagae.Location = new System.Drawing.Point(7, 322);
            this._btnEnagae.Name = "_btnEnagae";
            this._btnEnagae.Size = new System.Drawing.Size(75, 23);
            this._btnEnagae.TabIndex = 17;
            this._btnEnagae.Text = "Engage";
            this._btnEnagae.UseVisualStyleBackColor = true;
            this._btnEnagae.Click += new System.EventHandler(this._btnEnagae_Click);
            // 
            // _btnPark
            // 
            this._btnPark.Location = new System.Drawing.Point(7, 351);
            this._btnPark.Name = "_btnPark";
            this._btnPark.Size = new System.Drawing.Size(75, 23);
            this._btnPark.TabIndex = 16;
            this._btnPark.Text = "Park";
            this._btnPark.UseVisualStyleBackColor = true;
            this._btnPark.Click += new System.EventHandler(this._btnPark_Click);
            // 
            // _numOfRepetitionsTextBox
            // 
            this._numOfRepetitionsTextBox.Location = new System.Drawing.Point(156, 442);
            this._numOfRepetitionsTextBox.Name = "_numOfRepetitionsTextBox";
            this._numOfRepetitionsTextBox.Size = new System.Drawing.Size(45, 20);
            this._numOfRepetitionsTextBox.TabIndex = 11;
            this._numOfRepetitionsTextBox.Text = "1";
            this._numOfRepetitionsTextBox.TextChanged += new System.EventHandler(this._numOfRepetitionsTextBox_TextChanged);
            this._numOfRepetitionsTextBox.Leave += new System.EventHandler(this._numOfRepetitionsTextBox_Leave);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(87, 445);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(63, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "Repetitions:";
            // 
            // panel3
            // 
            this.panel3.Location = new System.Drawing.Point(165, 61);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(47, 33);
            this.panel3.TabIndex = 5;
            // 
            // panel2
            // 
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(200, 100);
            this.panel2.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Location = new System.Drawing.Point(19, 60);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(42, 34);
            this.panel1.TabIndex = 3;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(64, 326);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(23, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "0ml";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(64, 45);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "60ml";
            // 
            // _guiInterfaceToolTip
            // 
            this._guiInterfaceToolTip.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            // 
            // _trialInfoGroupBox
            // 
            this._trialInfoGroupBox.Controls.Add(this._trialDetailsListView);
            this._trialInfoGroupBox.Location = new System.Drawing.Point(936, 707);
            this._trialInfoGroupBox.Name = "_trialInfoGroupBox";
            this._trialInfoGroupBox.Size = new System.Drawing.Size(724, 178);
            this._trialInfoGroupBox.TabIndex = 17;
            this._trialInfoGroupBox.TabStop = false;
            this._trialInfoGroupBox.Text = "Trial Info";
            // 
            // globalExperimentInfogroupBox
            // 
            this.globalExperimentInfogroupBox.Controls.Add(this._globaExperimentlInfoListView);
            this.globalExperimentInfogroupBox.Location = new System.Drawing.Point(1275, 81);
            this.globalExperimentInfogroupBox.Name = "globalExperimentInfogroupBox";
            this.globalExperimentInfogroupBox.Size = new System.Drawing.Size(391, 310);
            this.globalExperimentInfogroupBox.TabIndex = 18;
            this.globalExperimentInfogroupBox.TabStop = false;
            this.globalExperimentInfogroupBox.Text = "Global Experiment Info";
            // 
            // _globaExperimentlInfoListView
            // 
            this._globaExperimentlInfoListView.Location = new System.Drawing.Point(6, 19);
            this._globaExperimentlInfoListView.Name = "_globaExperimentlInfoListView";
            this._globaExperimentlInfoListView.Size = new System.Drawing.Size(379, 285);
            this._globaExperimentlInfoListView.TabIndex = 0;
            this._globaExperimentlInfoListView.UseCompatibleStateImageBehavior = false;
            // 
            // _onlinePsychGrpahGroupBox
            // 
            this._onlinePsychGrpahGroupBox.Controls.Add(this._onlinePsychGraphControl);
            this._onlinePsychGrpahGroupBox.Location = new System.Drawing.Point(1275, 397);
            this._onlinePsychGrpahGroupBox.Name = "_onlinePsychGrpahGroupBox";
            this._onlinePsychGrpahGroupBox.Size = new System.Drawing.Size(391, 304);
            this._onlinePsychGrpahGroupBox.TabIndex = 19;
            this._onlinePsychGrpahGroupBox.TabStop = false;
            this._onlinePsychGrpahGroupBox.Text = "Online Psycho Graph";
            // 
            // _onlinePsychGraphControl
            // 
            chartArea1.Name = "ChartArea1";
            this._onlinePsychGraphControl.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this._onlinePsychGraphControl.Legends.Add(legend1);
            this._onlinePsychGraphControl.Location = new System.Drawing.Point(6, 19);
            this._onlinePsychGraphControl.Name = "_onlinePsychGraphControl";
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            this._onlinePsychGraphControl.Series.Add(series1);
            this._onlinePsychGraphControl.Size = new System.Drawing.Size(379, 279);
            this._onlinePsychGraphControl.TabIndex = 0;
            this._onlinePsychGraphControl.Text = "chart1";
            this._onlinePsychGraphControl.DoubleClick += new System.EventHandler(this._onlinePsychGraphControl_Click);
            // 
            // _warningsGroupBox
            // 
            this._warningsGroupBox.Controls.Add(this._ardionoPrtWarningLabel);
            this._warningsGroupBox.Location = new System.Drawing.Point(930, 560);
            this._warningsGroupBox.Name = "_warningsGroupBox";
            this._warningsGroupBox.Size = new System.Drawing.Size(339, 135);
            this._warningsGroupBox.TabIndex = 21;
            this._warningsGroupBox.TabStop = false;
            this._warningsGroupBox.Text = "Warnings";
            // 
            // _ardionoPrtWarningLabel
            // 
            this._ardionoPrtWarningLabel.AutoSize = true;
            this._ardionoPrtWarningLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this._ardionoPrtWarningLabel.ForeColor = System.Drawing.Color.Red;
            this._ardionoPrtWarningLabel.Location = new System.Drawing.Point(6, 20);
            this._ardionoPrtWarningLabel.Name = "_ardionoPrtWarningLabel";
            this._ardionoPrtWarningLabel.Size = new System.Drawing.Size(385, 13);
            this._ardionoPrtWarningLabel.TabIndex = 0;
            this._ardionoPrtWarningLabel.Text = "The COM4 (Arduino) port is not connected-LEDS not work properly";
            this._ardionoPrtWarningLabel.Visible = false;
            // 
            // _checkboxRRDelta
            // 
            this._checkboxRRDelta.Location = new System.Drawing.Point(0, 0);
            this._checkboxRRDelta.Name = "_checkboxRRDelta";
            this._checkboxRRDelta.Size = new System.Drawing.Size(104, 24);
            this._checkboxRRDelta.TabIndex = 0;
            // 
            // _checkBoxRightAndLeftSame
            // 
            this._checkBoxRightAndLeftSame.AutoSize = true;
            this._checkBoxRightAndLeftSame.Location = new System.Drawing.Point(2, 82);
            this._checkBoxRightAndLeftSame.Name = "_checkBoxRightAndLeftSame";
            this._checkBoxRightAndLeftSame.Size = new System.Drawing.Size(82, 17);
            this._checkBoxRightAndLeftSame.TabIndex = 17;
            this._checkBoxRightAndLeftSame.Text = "R + L Same";
            this._checkBoxRightAndLeftSame.UseVisualStyleBackColor = true;
            this._checkBoxRightAndLeftSame.CheckedChanged += new System.EventHandler(this._checkBoxRightAndLeftSame_CheckedChanged);
            // 
            // _checkBoxFixationOnly
            // 
            this._checkBoxFixationOnly.Location = new System.Drawing.Point(0, 0);
            this._checkBoxFixationOnly.Name = "_checkBoxFixationOnly";
            this._checkBoxFixationOnly.Size = new System.Drawing.Size(104, 24);
            this._checkBoxFixationOnly.TabIndex = 0;
            // 
            // _checkBoxEnableGoCue
            // 
            this._checkBoxEnableGoCue.Location = new System.Drawing.Point(0, 0);
            this._checkBoxEnableGoCue.Name = "_checkBoxEnableGoCue";
            this._checkBoxEnableGoCue.Size = new System.Drawing.Size(104, 24);
            this._checkBoxEnableGoCue.TabIndex = 0;
            // 
            // _checkboxErrorSoundOn
            // 
            this._checkboxErrorSoundOn.AutoSize = true;
            this._checkboxErrorSoundOn.Location = new System.Drawing.Point(4, 82);
            this._checkboxErrorSoundOn.Name = "_checkboxErrorSoundOn";
            this._checkboxErrorSoundOn.Size = new System.Drawing.Size(48, 17);
            this._checkboxErrorSoundOn.TabIndex = 14;
            this._checkboxErrorSoundOn.Text = "Error";
            this._checkboxErrorSoundOn.UseVisualStyleBackColor = true;
            this._checkboxErrorSoundOn.CheckedChanged += new System.EventHandler(this._checkboxErrorSoundOn_CheckedChanged);
            // 
            // _btnSaveProtocol
            // 
            this._btnSaveProtocol.Location = new System.Drawing.Point(576, 47);
            this._btnSaveProtocol.Name = "_btnSaveProtocol";
            this._btnSaveProtocol.Size = new System.Drawing.Size(75, 23);
            this._btnSaveProtocol.TabIndex = 25;
            this._btnSaveProtocol.Text = "Save Protocol";
            this._btnSaveProtocol.UseVisualStyleBackColor = true;
            this._btnSaveProtocol.Click += new System.EventHandler(this._btnSaveProtocol_Click);
            // 
            // _textboxNewProtocolName
            // 
            this._textboxNewProtocolName.Location = new System.Drawing.Point(470, 49);
            this._textboxNewProtocolName.Name = "_textboxNewProtocolName";
            this._textboxNewProtocolName.Size = new System.Drawing.Size(100, 20);
            this._textboxNewProtocolName.TabIndex = 26;
            // 
            // _labelNewProtocolName
            // 
            this._labelNewProtocolName.AutoSize = true;
            this._labelNewProtocolName.Location = new System.Drawing.Point(362, 54);
            this._labelNewProtocolName.Name = "_labelNewProtocolName";
            this._labelNewProtocolName.Size = new System.Drawing.Size(102, 13);
            this._labelNewProtocolName.TabIndex = 27;
            this._labelNewProtocolName.Text = "New Protocol Name";
            // 
            // _groupboxGoCue
            // 
            this._groupboxGoCue.Controls.Add(this._radiobuttonGoCueCorrectSide);
            this._groupboxGoCue.Controls.Add(this._radiobuttonGoCueBothSide);
            this._groupboxGoCue.Location = new System.Drawing.Point(103, 37);
            this._groupboxGoCue.Name = "_groupboxGoCue";
            this._groupboxGoCue.Size = new System.Drawing.Size(117, 50);
            this._groupboxGoCue.TabIndex = 20;
            this._groupboxGoCue.TabStop = false;
            this._groupboxGoCue.Text = "GoCue";
            // 
            // _radiobuttonGoCueCorrectSide
            // 
            this._radiobuttonGoCueCorrectSide.Location = new System.Drawing.Point(0, 0);
            this._radiobuttonGoCueCorrectSide.Name = "_radiobuttonGoCueCorrectSide";
            this._radiobuttonGoCueCorrectSide.Size = new System.Drawing.Size(104, 24);
            this._radiobuttonGoCueCorrectSide.TabIndex = 0;
            // 
            // _radiobuttonGoCueBothSide
            // 
            this._radiobuttonGoCueBothSide.Location = new System.Drawing.Point(0, 0);
            this._radiobuttonGoCueBothSide.Name = "_radiobuttonGoCueBothSide";
            this._radiobuttonGoCueBothSide.Size = new System.Drawing.Size(104, 24);
            this._radiobuttonGoCueBothSide.TabIndex = 1;
            // 
            // GuiInterface
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1678, 912);
            this.Controls.Add(this._labelNewProtocolName);
            this.Controls.Add(this._textboxNewProtocolName);
            this.Controls.Add(this._btnSaveProtocol);
            this.Controls.Add(this._warningsGroupBox);
            this.Controls.Add(this._onlinePsychGrpahGroupBox);
            this.Controls.Add(this.globalExperimentInfogroupBox);
            this.Controls.Add(this._trialInfoGroupBox);
            this.Controls.Add(this._varyingControlGroupBox);
            this.Controls.Add(this._trialParametersGroup);
            this.Controls.Add(this._protocolBrowserBtn);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this._protocolsComboBox);
            this.Name = "GuiInterface";
            this.Text = "UniJoy";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.GuiInterface_Close);
            this._trialParametersGroup.ResumeLayout(false);
            this._varyingControlGroupBox.ResumeLayout(false);
            this._varyingControlGroupBox.PerformLayout();
            this._trialInfoGroupBox.ResumeLayout(false);
            this.globalExperimentInfogroupBox.ResumeLayout(false);
            this._onlinePsychGrpahGroupBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._onlinePsychGraphControl)).EndInit();
            this._warningsGroupBox.ResumeLayout(false);
            this._warningsGroupBox.PerformLayout();
            this._groupboxGoCue.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.ComboBox _protocolsComboBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.FolderBrowserDialog _protocolsFolderBrowser;
        private System.Windows.Forms.Button _protocolBrowserBtn;
        private System.Windows.Forms.Button _btnStart;
        private System.Windows.Forms.ListBox _varyingListBox;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.Button _btnMakeTrials;
        private System.Windows.Forms.Button _btnStop;
        private System.Windows.Forms.ListView _trialDetailsListView;
        private System.Windows.Forms.GroupBox _trialParametersGroup;
        private System.Windows.Forms.Panel _dynamicParametersPanel;
        private System.Windows.Forms.GroupBox _varyingControlGroupBox;
        //private System.Windows.Forms.GroupBox _interactiveNolduscommuncation;
        //private System.Windows.Forms.RadioButton _rightNoldusCommunicationRadioButton;
        //private System.Windows.Forms.RadioButton _centerNoldusCommunicationRadioButton;
        //private System.Windows.Forms.RadioButton _leftNoldusCommunicationRadioButton;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel1;
        //private System.Windows.Forms.GroupBox _drinkControlGroupBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.ComponentModel.BackgroundWorker backgroundWorker2;
        //private System.Windows.Forms.GroupBox _selectedRatGroupBox;
        //private System.Windows.Forms.ComboBox _comboBoxSelectedRatName;
        private System.ComponentModel.BackgroundWorker backgroundWorker3;
        private System.Windows.Forms.ToolTip _guiInterfaceToolTip;
        private System.Windows.Forms.GroupBox _trialInfoGroupBox;
        private System.Windows.Forms.GroupBox globalExperimentInfogroupBox;
        private System.Windows.Forms.ListView _globaExperimentlInfoListView;
        private System.Windows.Forms.TextBox _numOfRepetitionsTextBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox _onlinePsychGrpahGroupBox;
        private System.Windows.Forms.DataVisualization.Charting.Chart _onlinePsychGraphControl;
        //private System.Windows.Forms.CheckBox _checkBoxAutoChoice;
        //private System.Windows.Forms.GroupBox _autosGroupBox;
        //private System.Windows.Forms.CheckBox _checkBoxAutoStart;
        private System.Windows.Forms.GroupBox _warningsGroupBox;
        private System.Windows.Forms.Label _ardionoPrtWarningLabel;
        //private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox _checkBoxFixationOnly;
        private System.Windows.Forms.Button _btnPark;
        private System.Windows.Forms.Button _btnEnagae;
        //private System.Windows.Forms.ComboBox _comboBoxStudentName;
        private System.Windows.Forms.Button _btnBreakFixationSound;
        private System.Windows.Forms.CheckBox _checkboxErrorSoundOn;
        private System.Windows.Forms.CheckBox _checkBoxEnableGoCue;
        private System.Windows.Forms.Button _btnSaveProtocol;
        private System.Windows.Forms.TextBox _textboxNewProtocolName;
        private System.Windows.Forms.Label _labelNewProtocolName;
        private System.Windows.Forms.CheckBox _checkBoxRightAndLeftSame;
        private System.Windows.Forms.CheckBox _checkboxRRDelta;
        private System.Windows.Forms.GroupBox _groupboxGoCue;
        private System.Windows.Forms.RadioButton _radiobuttonGoCueCorrectSide;
        private System.Windows.Forms.RadioButton _radiobuttonGoCueBothSide;
    }
}