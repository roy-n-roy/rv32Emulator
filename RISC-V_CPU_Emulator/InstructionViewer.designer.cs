using appRes = global::RISC_V_CPU_Emulator.Properties.Resources;

namespace RISC_V_CPU_Emulator {
    partial class InstructionViewerForm {

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InstructionViewerForm));
            this.DumpViewTextBox = new System.Windows.Forms.RichTextBox();
            this.BinaryInstructionDigitLabel = new System.Windows.Forms.Label();
            this.ArgumentsGroupBox = new System.Windows.Forms.GroupBox();
            this.Arg4Value = new System.Windows.Forms.Label();
            this.Arg4Name = new System.Windows.Forms.Label();
            this.Arg3Value = new System.Windows.Forms.Label();
            this.Arg3Name = new System.Windows.Forms.Label();
            this.Arg2Value = new System.Windows.Forms.Label();
            this.Arg2Name = new System.Windows.Forms.Label();
            this.Arg1Value = new System.Windows.Forms.Label();
            this.Arg1Name = new System.Windows.Forms.Label();
            this.InstructionRegisterGroupBox = new System.Windows.Forms.GroupBox();
            this.InstructionNameLabel = new System.Windows.Forms.Label();
            this.ProgramCounterGroupBox = new System.Windows.Forms.GroupBox();
            this.PCValueLabel = new System.Windows.Forms.Label();
            this.CurrentModeGroupBox = new System.Windows.Forms.GroupBox();
            this.CurrentModeLabel = new System.Windows.Forms.Label();
            this.statusLabel = new System.Windows.Forms.Label();
            this.StepExecuteButton = new System.Windows.Forms.Button();
            this.BinaryInstructionLabel_26_32 = new System.Windows.Forms.Label();
            this.BinaryInstructionLabel_21_25 = new System.Windows.Forms.Label();
            this.BinaryInstructionLabel_16_20 = new System.Windows.Forms.Label();
            this.BinaryInstructionLabel_13_15 = new System.Windows.Forms.Label();
            this.BinaryInstructionLabel_12_8 = new System.Windows.Forms.Label();
            this.BinaryInstructionLabel_1_7 = new System.Windows.Forms.Label();
            this.BinaryInstructionPanel = new System.Windows.Forms.Panel();
            this.DisplayCSRegisterCheckBox = new System.Windows.Forms.CheckBox();
            this.DisplayFPRegisterCheckBox = new System.Windows.Forms.CheckBox();
            this.DisplayIRegisterCheckBox = new System.Windows.Forms.CheckBox();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.RunPcPanel = new System.Windows.Forms.Panel();
            this.RunPcTextBox = new System.Windows.Forms.MaskedTextBox();
            this.RunPcCheckBox = new System.Windows.Forms.CheckBox();
            this.IntegerRegistersControl = new RISC_V_CPU_Emulator.IntegerRegistersControl();
            this.FloatPointRegistersControl = new RISC_V_CPU_Emulator.FloatPointRegistersControl();
            this.ArgumentsGroupBox.SuspendLayout();
            this.InstructionRegisterGroupBox.SuspendLayout();
            this.ProgramCounterGroupBox.SuspendLayout();
            this.CurrentModeGroupBox.SuspendLayout();
            this.BinaryInstructionPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.RunPcPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // DumpViewTextBox
            // 
            resources.ApplyResources(this.DumpViewTextBox, "DumpViewTextBox");
            this.DumpViewTextBox.BackColor = System.Drawing.Color.White;
            this.DumpViewTextBox.Name = "DumpViewTextBox";
            this.DumpViewTextBox.ReadOnly = true;
            this.DumpViewTextBox.TabStop = false;
            // 
            // BinaryInstructionDigitLabel
            // 
            resources.ApplyResources(this.BinaryInstructionDigitLabel, "BinaryInstructionDigitLabel");
            this.BinaryInstructionDigitLabel.Name = "BinaryInstructionDigitLabel";
            // 
            // ArgumentsGroupBox
            // 
            resources.ApplyResources(this.ArgumentsGroupBox, "ArgumentsGroupBox");
            this.ArgumentsGroupBox.Controls.Add(this.Arg4Value);
            this.ArgumentsGroupBox.Controls.Add(this.Arg4Name);
            this.ArgumentsGroupBox.Controls.Add(this.Arg3Value);
            this.ArgumentsGroupBox.Controls.Add(this.Arg3Name);
            this.ArgumentsGroupBox.Controls.Add(this.Arg2Value);
            this.ArgumentsGroupBox.Controls.Add(this.Arg2Name);
            this.ArgumentsGroupBox.Controls.Add(this.Arg1Value);
            this.ArgumentsGroupBox.Controls.Add(this.Arg1Name);
            this.ArgumentsGroupBox.Name = "ArgumentsGroupBox";
            this.ArgumentsGroupBox.TabStop = false;
            // 
            // Arg4Value
            // 
            resources.ApplyResources(this.Arg4Value, "Arg4Value");
            this.Arg4Value.Name = "Arg4Value";
            // 
            // Arg4Name
            // 
            resources.ApplyResources(this.Arg4Name, "Arg4Name");
            this.Arg4Name.Name = "Arg4Name";
            // 
            // Arg3Value
            // 
            resources.ApplyResources(this.Arg3Value, "Arg3Value");
            this.Arg3Value.Name = "Arg3Value";
            // 
            // Arg3Name
            // 
            resources.ApplyResources(this.Arg3Name, "Arg3Name");
            this.Arg3Name.Name = "Arg3Name";
            // 
            // Arg2Value
            // 
            resources.ApplyResources(this.Arg2Value, "Arg2Value");
            this.Arg2Value.Name = "Arg2Value";
            // 
            // Arg2Name
            // 
            resources.ApplyResources(this.Arg2Name, "Arg2Name");
            this.Arg2Name.Name = "Arg2Name";
            // 
            // Arg1Value
            // 
            resources.ApplyResources(this.Arg1Value, "Arg1Value");
            this.Arg1Value.Name = "Arg1Value";
            // 
            // Arg1Name
            // 
            resources.ApplyResources(this.Arg1Name, "Arg1Name");
            this.Arg1Name.Name = "Arg1Name";
            // 
            // InstructionRegisterGroupBox
            // 
            resources.ApplyResources(this.InstructionRegisterGroupBox, "InstructionRegisterGroupBox");
            this.InstructionRegisterGroupBox.Controls.Add(this.InstructionNameLabel);
            this.InstructionRegisterGroupBox.Name = "InstructionRegisterGroupBox";
            this.InstructionRegisterGroupBox.TabStop = false;
            // 
            // InstructionNameLabel
            // 
            resources.ApplyResources(this.InstructionNameLabel, "InstructionNameLabel");
            this.InstructionNameLabel.Name = "InstructionNameLabel";
            // 
            // ProgramCounterGroupBox
            // 
            resources.ApplyResources(this.ProgramCounterGroupBox, "ProgramCounterGroupBox");
            this.ProgramCounterGroupBox.Controls.Add(this.PCValueLabel);
            this.ProgramCounterGroupBox.Name = "ProgramCounterGroupBox";
            this.ProgramCounterGroupBox.TabStop = false;
            // 
            // PCValueLabel
            // 
            resources.ApplyResources(this.PCValueLabel, "PCValueLabel");
            this.PCValueLabel.Cursor = System.Windows.Forms.Cursors.Default;
            this.PCValueLabel.Name = "PCValueLabel";
            // 
            // CurrentModeGroupBox
            // 
            resources.ApplyResources(this.CurrentModeGroupBox, "CurrentModeGroupBox");
            this.CurrentModeGroupBox.Controls.Add(this.CurrentModeLabel);
            this.CurrentModeGroupBox.Controls.Add(this.statusLabel);
            this.CurrentModeGroupBox.Name = "CurrentModeGroupBox";
            this.CurrentModeGroupBox.TabStop = false;
            // 
            // CurrentModeLabel
            // 
            resources.ApplyResources(this.CurrentModeLabel, "CurrentModeLabel");
            this.CurrentModeLabel.Cursor = System.Windows.Forms.Cursors.Default;
            this.CurrentModeLabel.Name = "CurrentModeLabel";
            // 
            // statusLabel
            // 
            resources.ApplyResources(this.statusLabel, "statusLabel");
            this.statusLabel.Name = "statusLabel";
            // 
            // StepExecuteButton
            // 
            resources.ApplyResources(this.StepExecuteButton, "StepExecuteButton");
            this.StepExecuteButton.Name = "StepExecuteButton";
            this.StepExecuteButton.UseVisualStyleBackColor = true;
            this.StepExecuteButton.Click += new System.EventHandler(this.StepExecuteButton_Click);
            // 
            // BinaryInstructionLabel_26_32
            // 
            resources.ApplyResources(this.BinaryInstructionLabel_26_32, "BinaryInstructionLabel_26_32");
            this.BinaryInstructionLabel_26_32.Name = "BinaryInstructionLabel_26_32";
            // 
            // BinaryInstructionLabel_21_25
            // 
            resources.ApplyResources(this.BinaryInstructionLabel_21_25, "BinaryInstructionLabel_21_25");
            this.BinaryInstructionLabel_21_25.Name = "BinaryInstructionLabel_21_25";
            // 
            // BinaryInstructionLabel_16_20
            // 
            resources.ApplyResources(this.BinaryInstructionLabel_16_20, "BinaryInstructionLabel_16_20");
            this.BinaryInstructionLabel_16_20.Name = "BinaryInstructionLabel_16_20";
            // 
            // BinaryInstructionLabel_13_15
            // 
            resources.ApplyResources(this.BinaryInstructionLabel_13_15, "BinaryInstructionLabel_13_15");
            this.BinaryInstructionLabel_13_15.Name = "BinaryInstructionLabel_13_15";
            // 
            // BinaryInstructionLabel_12_8
            // 
            resources.ApplyResources(this.BinaryInstructionLabel_12_8, "BinaryInstructionLabel_12_8");
            this.BinaryInstructionLabel_12_8.Name = "BinaryInstructionLabel_12_8";
            // 
            // BinaryInstructionLabel_1_7
            // 
            resources.ApplyResources(this.BinaryInstructionLabel_1_7, "BinaryInstructionLabel_1_7");
            this.BinaryInstructionLabel_1_7.Name = "BinaryInstructionLabel_1_7";
            // 
            // BinaryInstructionPanel
            // 
            resources.ApplyResources(this.BinaryInstructionPanel, "BinaryInstructionPanel");
            this.BinaryInstructionPanel.Controls.Add(this.BinaryInstructionLabel_26_32);
            this.BinaryInstructionPanel.Controls.Add(this.BinaryInstructionLabel_21_25);
            this.BinaryInstructionPanel.Controls.Add(this.BinaryInstructionLabel_16_20);
            this.BinaryInstructionPanel.Controls.Add(this.BinaryInstructionLabel_13_15);
            this.BinaryInstructionPanel.Controls.Add(this.BinaryInstructionLabel_12_8);
            this.BinaryInstructionPanel.Controls.Add(this.BinaryInstructionLabel_1_7);
            this.BinaryInstructionPanel.Controls.Add(this.BinaryInstructionDigitLabel);
            this.BinaryInstructionPanel.Name = "BinaryInstructionPanel";
            // 
            // DisplayCSRegisterCheckBox
            // 
            resources.ApplyResources(this.DisplayCSRegisterCheckBox, "DisplayCSRegisterCheckBox");
            this.DisplayCSRegisterCheckBox.Name = "DisplayCSRegisterCheckBox";
            this.DisplayCSRegisterCheckBox.UseVisualStyleBackColor = true;
            this.DisplayCSRegisterCheckBox.CheckedChanged += new System.EventHandler(this.DisplayCSRegisterCheckBox_CheckedChanged);
            // 
            // DisplayFPRegisterCheckBox
            // 
            resources.ApplyResources(this.DisplayFPRegisterCheckBox, "DisplayFPRegisterCheckBox");
            this.DisplayFPRegisterCheckBox.Name = "DisplayFPRegisterCheckBox";
            this.DisplayFPRegisterCheckBox.UseVisualStyleBackColor = true;
            this.DisplayFPRegisterCheckBox.CheckedChanged += new System.EventHandler(this.DisplayFPRegisterCheckBox_CheckedChanged);
            // 
            // DisplayIRegisterCheckBox
            // 
            resources.ApplyResources(this.DisplayIRegisterCheckBox, "DisplayIRegisterCheckBox");
            this.DisplayIRegisterCheckBox.Name = "DisplayIRegisterCheckBox";
            this.DisplayIRegisterCheckBox.UseVisualStyleBackColor = true;
            this.DisplayIRegisterCheckBox.CheckedChanged += new System.EventHandler(this.DisplayIRegisterCheckBox_CheckedChanged);
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.DisplayCSRegisterCheckBox);
            this.groupBox1.Controls.Add(this.DisplayIRegisterCheckBox);
            this.groupBox1.Controls.Add(this.DisplayFPRegisterCheckBox);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // RunPcPanel
            // 
            resources.ApplyResources(this.RunPcPanel, "RunPcPanel");
            this.RunPcPanel.Controls.Add(this.RunPcTextBox);
            this.RunPcPanel.Controls.Add(this.RunPcCheckBox);
            this.RunPcPanel.Name = "RunPcPanel";
            // 
            // RunPcTextBox
            // 
            resources.ApplyResources(this.RunPcTextBox, "RunPcTextBox");
            this.RunPcTextBox.InsertKeyMode = System.Windows.Forms.InsertKeyMode.Overwrite;
            this.RunPcTextBox.Name = "RunPcTextBox";
            this.RunPcTextBox.KeyDown += RunPcTextBox_KeyDown;
            // 
            // RunPcCheckBox
            // 
            resources.ApplyResources(this.RunPcCheckBox, "RunPcCheckBox");
            this.RunPcCheckBox.Name = "RunPcCheckBox";
            this.RunPcCheckBox.UseVisualStyleBackColor = true;
            // 
            // IntegerRegistersControl
            // 
            resources.ApplyResources(this.IntegerRegistersControl, "IntegerRegistersControl");
            this.IntegerRegistersControl.Name = "IntegerRegistersControl";
            this.IntegerRegistersControl.TabStop = false;
            // 
            // FloatPointRegistersControl
            // 
            resources.ApplyResources(this.FloatPointRegistersControl, "FloatPointRegistersControl");
            this.FloatPointRegistersControl.Name = "FloatPointRegistersControl";
            this.FloatPointRegistersControl.TabStop = false;
            // 
            // InstructionViewerForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.RunPcPanel);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.DumpViewTextBox);
            this.Controls.Add(this.StepExecuteButton);
            this.Controls.Add(this.CurrentModeGroupBox);
            this.Controls.Add(this.ProgramCounterGroupBox);
            this.Controls.Add(this.InstructionRegisterGroupBox);
            this.Controls.Add(this.ArgumentsGroupBox);
            this.Controls.Add(this.BinaryInstructionPanel);
            this.Controls.Add(this.IntegerRegistersControl);
            this.Controls.Add(this.FloatPointRegistersControl);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.Name = "InstructionViewerForm";
            this.ShowIcon = false;
            this.Resize += new System.EventHandler(this.InstructionViewerForm_Resize);
            this.ArgumentsGroupBox.ResumeLayout(false);
            this.ArgumentsGroupBox.PerformLayout();
            this.InstructionRegisterGroupBox.ResumeLayout(false);
            this.InstructionRegisterGroupBox.PerformLayout();
            this.ProgramCounterGroupBox.ResumeLayout(false);
            this.ProgramCounterGroupBox.PerformLayout();
            this.CurrentModeGroupBox.ResumeLayout(false);
            this.CurrentModeGroupBox.PerformLayout();
            this.BinaryInstructionPanel.ResumeLayout(false);
            this.BinaryInstructionPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.RunPcPanel.ResumeLayout(false);
            this.RunPcPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox DumpViewTextBox;
        private System.Windows.Forms.Label BinaryInstructionDigitLabel;
        private System.Windows.Forms.GroupBox InstructionRegisterGroupBox;
        private System.Windows.Forms.Label InstructionNameLabel;
        private System.Windows.Forms.GroupBox ArgumentsGroupBox;
        private System.Windows.Forms.Label Arg1Name;
        private System.Windows.Forms.Label Arg2Name;
        private System.Windows.Forms.Label Arg3Name;
        private System.Windows.Forms.Label Arg4Name;
        private System.Windows.Forms.Label Arg1Value;
        private System.Windows.Forms.Label Arg2Value;
        private System.Windows.Forms.Label Arg3Value;
        private System.Windows.Forms.Label Arg4Value;
        private System.Windows.Forms.GroupBox ProgramCounterGroupBox;
        private System.Windows.Forms.Label PCValueLabel;
        private System.Windows.Forms.GroupBox CurrentModeGroupBox;
        private System.Windows.Forms.Label CurrentModeLabel;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.Button StepExecuteButton;
        private System.Windows.Forms.Label BinaryInstructionLabel_26_32;
        private System.Windows.Forms.Label BinaryInstructionLabel_21_25;
        private System.Windows.Forms.Label BinaryInstructionLabel_16_20;
        private System.Windows.Forms.Label BinaryInstructionLabel_13_15;
        private System.Windows.Forms.Label BinaryInstructionLabel_12_8;
        private System.Windows.Forms.Label BinaryInstructionLabel_1_7;
        private System.Windows.Forms.Panel BinaryInstructionPanel;
        private IntegerRegistersControl IntegerRegistersControl;
        private FloatPointRegistersControl FloatPointRegistersControl;
        private System.Windows.Forms.CheckBox DisplayCSRegisterCheckBox;
        private System.Windows.Forms.CheckBox DisplayFPRegisterCheckBox;
        private System.Windows.Forms.CheckBox DisplayIRegisterCheckBox;
        private System.Windows.Forms.BindingSource bindingSource1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Panel RunPcPanel;
        private System.Windows.Forms.CheckBox RunPcCheckBox;
        private System.Windows.Forms.MaskedTextBox RunPcTextBox;
    }
}