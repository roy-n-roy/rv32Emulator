namespace RISC_V_CPU_Emulator {
    partial class InstructionViewer {
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
            this.BinaryInstructionLabel = new System.Windows.Forms.Label();
            this.InstructionNameLabel = new System.Windows.Forms.Label();
            this.Arg1Name = new System.Windows.Forms.Label();
            this.Arg2Name = new System.Windows.Forms.Label();
            this.Arg3Name = new System.Windows.Forms.Label();
            this.Arg4Name = new System.Windows.Forms.Label();
            this.Arg5Name = new System.Windows.Forms.Label();
            this.Arg1Value = new System.Windows.Forms.Label();
            this.Arg2Value = new System.Windows.Forms.Label();
            this.Arg3Value = new System.Windows.Forms.Label();
            this.Arg4Value = new System.Windows.Forms.Label();
            this.Arg5Value = new System.Windows.Forms.Label();
            this.StepExecuteButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.PCValueLabel = new System.Windows.Forms.Label();
            this.DumpViewTextBox = new System.Windows.Forms.RichTextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.CurrentModeLabel = new System.Windows.Forms.Label();
            this.BinaryInstructionDigitLabel = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // BinaryInstructionLabel
            // 
            this.BinaryInstructionLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BinaryInstructionLabel.AutoSize = true;
            this.BinaryInstructionLabel.Font = new System.Drawing.Font("Yu Gothic UI", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.BinaryInstructionLabel.Location = new System.Drawing.Point(614, 9);
            this.BinaryInstructionLabel.Name = "BinaryInstructionLabel";
            this.BinaryInstructionLabel.Size = new System.Drawing.Size(609, 45);
            this.BinaryInstructionLabel.TabIndex = 0;
            this.BinaryInstructionLabel.Text = "0000000 00000 00000 000 00000 0000000";
            // 
            // InstructionNameLabel
            // 
            this.InstructionNameLabel.AutoSize = true;
            this.InstructionNameLabel.Font = new System.Drawing.Font("メイリオ", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.InstructionNameLabel.Location = new System.Drawing.Point(16, 23);
            this.InstructionNameLabel.Name = "InstructionNameLabel";
            this.InstructionNameLabel.Size = new System.Drawing.Size(0, 44);
            this.InstructionNameLabel.TabIndex = 1;
            // 
            // Arg1Name
            // 
            this.Arg1Name.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Arg1Name.AutoSize = true;
            this.Arg1Name.Font = new System.Drawing.Font("メイリオ", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Arg1Name.Location = new System.Drawing.Point(3, 29);
            this.Arg1Name.Name = "Arg1Name";
            this.Arg1Name.Size = new System.Drawing.Size(0, 36);
            this.Arg1Name.TabIndex = 2;
            // 
            // Arg2Name
            // 
            this.Arg2Name.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Arg2Name.AutoSize = true;
            this.Arg2Name.Font = new System.Drawing.Font("メイリオ", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Arg2Name.Location = new System.Drawing.Point(3, 79);
            this.Arg2Name.Name = "Arg2Name";
            this.Arg2Name.Size = new System.Drawing.Size(0, 36);
            this.Arg2Name.TabIndex = 4;
            // 
            // Arg3Name
            // 
            this.Arg3Name.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Arg3Name.AutoSize = true;
            this.Arg3Name.Font = new System.Drawing.Font("メイリオ", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Arg3Name.Location = new System.Drawing.Point(3, 129);
            this.Arg3Name.Name = "Arg3Name";
            this.Arg3Name.Size = new System.Drawing.Size(0, 36);
            this.Arg3Name.TabIndex = 6;
            // 
            // Arg4Name
            // 
            this.Arg4Name.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Arg4Name.AutoSize = true;
            this.Arg4Name.Font = new System.Drawing.Font("メイリオ", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Arg4Name.Location = new System.Drawing.Point(3, 179);
            this.Arg4Name.Name = "Arg4Name";
            this.Arg4Name.Size = new System.Drawing.Size(0, 36);
            this.Arg4Name.TabIndex = 8;
            // 
            // Arg5Name
            // 
            this.Arg5Name.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Arg5Name.AutoSize = true;
            this.Arg5Name.Font = new System.Drawing.Font("メイリオ", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Arg5Name.Location = new System.Drawing.Point(3, 229);
            this.Arg5Name.Name = "Arg5Name";
            this.Arg5Name.Size = new System.Drawing.Size(0, 36);
            this.Arg5Name.TabIndex = 10;
            // 
            // Arg1Value
            // 
            this.Arg1Value.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Arg1Value.AutoSize = true;
            this.Arg1Value.Font = new System.Drawing.Font("メイリオ", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Arg1Value.Location = new System.Drawing.Point(147, 29);
            this.Arg1Value.Name = "Arg1Value";
            this.Arg1Value.Size = new System.Drawing.Size(0, 36);
            this.Arg1Value.TabIndex = 3;
            // 
            // Arg2Value
            // 
            this.Arg2Value.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Arg2Value.AutoSize = true;
            this.Arg2Value.Font = new System.Drawing.Font("メイリオ", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Arg2Value.Location = new System.Drawing.Point(147, 79);
            this.Arg2Value.Name = "Arg2Value";
            this.Arg2Value.Size = new System.Drawing.Size(0, 36);
            this.Arg2Value.TabIndex = 5;
            // 
            // Arg3Value
            // 
            this.Arg3Value.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Arg3Value.AutoSize = true;
            this.Arg3Value.Font = new System.Drawing.Font("メイリオ", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Arg3Value.Location = new System.Drawing.Point(147, 129);
            this.Arg3Value.Name = "Arg3Value";
            this.Arg3Value.Size = new System.Drawing.Size(0, 36);
            this.Arg3Value.TabIndex = 7;
            // 
            // Arg4Value
            // 
            this.Arg4Value.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Arg4Value.AutoSize = true;
            this.Arg4Value.Font = new System.Drawing.Font("メイリオ", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Arg4Value.Location = new System.Drawing.Point(147, 179);
            this.Arg4Value.Name = "Arg4Value";
            this.Arg4Value.Size = new System.Drawing.Size(0, 36);
            this.Arg4Value.TabIndex = 9;
            // 
            // Arg5Value
            // 
            this.Arg5Value.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Arg5Value.AutoSize = true;
            this.Arg5Value.Font = new System.Drawing.Font("メイリオ", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Arg5Value.Location = new System.Drawing.Point(147, 229);
            this.Arg5Value.Name = "Arg5Value";
            this.Arg5Value.Size = new System.Drawing.Size(0, 36);
            this.Arg5Value.TabIndex = 11;
            // 
            // StepExecuteButton
            // 
            this.StepExecuteButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.StepExecuteButton.Font = new System.Drawing.Font("Meiryo UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.StepExecuteButton.Location = new System.Drawing.Point(1023, 379);
            this.StepExecuteButton.Name = "StepExecuteButton";
            this.StepExecuteButton.Size = new System.Drawing.Size(185, 51);
            this.StepExecuteButton.TabIndex = 12;
            this.StepExecuteButton.Text = "ス テ ッ プ 実 行";
            this.StepExecuteButton.UseVisualStyleBackColor = true;
            this.StepExecuteButton.Click += new System.EventHandler(this.StepExecuteButton_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.Arg5Value);
            this.groupBox1.Controls.Add(this.Arg5Name);
            this.groupBox1.Controls.Add(this.Arg4Value);
            this.groupBox1.Controls.Add(this.Arg4Name);
            this.groupBox1.Controls.Add(this.Arg3Value);
            this.groupBox1.Controls.Add(this.Arg3Name);
            this.groupBox1.Controls.Add(this.Arg2Value);
            this.groupBox1.Controls.Add(this.Arg2Name);
            this.groupBox1.Controls.Add(this.Arg1Value);
            this.groupBox1.Controls.Add(this.Arg1Name);
            this.groupBox1.Font = new System.Drawing.Font("メイリオ", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.groupBox1.Location = new System.Drawing.Point(650, 81);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(310, 285);
            this.groupBox1.TabIndex = 13;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Arguments";
            // 
            // PCValueLabel
            // 
            this.PCValueLabel.AutoSize = true;
            this.PCValueLabel.Cursor = System.Windows.Forms.Cursors.Default;
            this.PCValueLabel.Font = new System.Drawing.Font("メイリオ", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.PCValueLabel.Location = new System.Drawing.Point(13, 24);
            this.PCValueLabel.Name = "PCValueLabel";
            this.PCValueLabel.Size = new System.Drawing.Size(198, 44);
            this.PCValueLabel.TabIndex = 15;
            this.PCValueLabel.Text = "0x00000000";
            // 
            // DumpViewTextBox
            // 
            this.DumpViewTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DumpViewTextBox.BackColor = System.Drawing.Color.White;
            this.DumpViewTextBox.Font = new System.Drawing.Font("Yu Gothic UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.DumpViewTextBox.Location = new System.Drawing.Point(19, 17);
            this.DumpViewTextBox.Name = "DumpViewTextBox";
            this.DumpViewTextBox.ReadOnly = true;
            this.DumpViewTextBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.DumpViewTextBox.Size = new System.Drawing.Size(577, 413);
            this.DumpViewTextBox.TabIndex = 16;
            this.DumpViewTextBox.Text = "";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.InstructionNameLabel);
            this.groupBox2.Font = new System.Drawing.Font("メイリオ", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.groupBox2.Location = new System.Drawing.Point(977, 81);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(231, 78);
            this.groupBox2.TabIndex = 17;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Instruction Register";
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.PCValueLabel);
            this.groupBox3.Font = new System.Drawing.Font("メイリオ", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.groupBox3.Location = new System.Drawing.Point(980, 181);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(228, 79);
            this.groupBox3.TabIndex = 18;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Program Counter";
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox4.Controls.Add(this.CurrentModeLabel);
            this.groupBox4.Font = new System.Drawing.Font("メイリオ", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.groupBox4.Location = new System.Drawing.Point(980, 287);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(228, 79);
            this.groupBox4.TabIndex = 18;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Current Mode";
            // 
            // CurrentModeLabel
            // 
            this.CurrentModeLabel.AutoSize = true;
            this.CurrentModeLabel.Cursor = System.Windows.Forms.Cursors.Default;
            this.CurrentModeLabel.Font = new System.Drawing.Font("メイリオ", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.CurrentModeLabel.Location = new System.Drawing.Point(16, 27);
            this.CurrentModeLabel.Name = "CurrentModeLabel";
            this.CurrentModeLabel.Size = new System.Drawing.Size(174, 36);
            this.CurrentModeLabel.TabIndex = 15;
            this.CurrentModeLabel.Text = "MachineMode";
            // 
            // BinaryInstructionDigitLabel
            // 
            this.BinaryInstructionDigitLabel.AutoSize = true;
            this.BinaryInstructionDigitLabel.Font = new System.Drawing.Font("Yu Gothic UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.BinaryInstructionDigitLabel.Location = new System.Drawing.Point(620, 47);
            this.BinaryInstructionDigitLabel.Name = "BinaryInstructionDigitLabel";
            this.BinaryInstructionDigitLabel.Size = new System.Drawing.Size(598, 15);
            this.BinaryInstructionDigitLabel.TabIndex = 19;
            this.BinaryInstructionDigitLabel.Text = "----Opcode---- -aq/rl- --------rs2-------  -------rs1-------   -Opcode-   -------" +
    "rd--------   --------Opcode--------";
            // 
            // InstructionViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1235, 451);
            this.Controls.Add(this.BinaryInstructionDigitLabel);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.DumpViewTextBox);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.StepExecuteButton);
            this.Controls.Add(this.BinaryInstructionLabel);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.Name = "InstructionViewer";
            this.Text = "Risc-V Instruction Viewer";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label BinaryInstructionLabel;
        private System.Windows.Forms.Label InstructionNameLabel;
        private System.Windows.Forms.Label Arg1Name;
        private System.Windows.Forms.Label Arg2Name;
        private System.Windows.Forms.Label Arg3Name;
        private System.Windows.Forms.Label Arg4Name;
        private System.Windows.Forms.Label Arg5Name;
        private System.Windows.Forms.Label Arg1Value;
        private System.Windows.Forms.Label Arg2Value;
        private System.Windows.Forms.Label Arg3Value;
        private System.Windows.Forms.Label Arg4Value;
        private System.Windows.Forms.Label Arg5Value;
        private System.Windows.Forms.Button StepExecuteButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label PCValueLabel;
        private System.Windows.Forms.RichTextBox DumpViewTextBox;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label CurrentModeLabel;
        private System.Windows.Forms.Label BinaryInstructionDigitLabel;
    }
}