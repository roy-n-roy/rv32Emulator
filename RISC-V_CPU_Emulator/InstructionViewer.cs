using RISC_V_Instruction;
using RV32_Cpu;
using RV32_Register.Constants;
using RV32_Register.MemoryHandler;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using appRes = global::RISC_V_CPU_Emulator.Properties.Resources;

namespace RISC_V_CPU_Emulator {
    public partial class InstructionViewerForm : Form {

        /// <summary>RISC-V命令をエミュレートする Hardware Thread(Hart)</summary>
        private RV32_HaedwareThread cpu;

        /// <summary>32bit長バイナリ形式命令ラベルの配列</summary>
        private (Label, int, string)[] BinaryInstructionLabels;
        /// <summary>16bit漁バイナリ形式命令ラベルの配列</summary>
        private (Label, int, string)[] ComplexedBinaryInstructionLabels;

        /// <summary>命令引数(レジスタ名・即値など)用ラベルの配列配列</summary>
        private Label[] ArgLabels;

        /// <summary>幅広の画面を使用し、縦に小さいレイアウトモードかのフラグ</summary>
        private bool IsWideMode = false;

        private ControlStatusRegisterViewer csrViewer;

        /// <summary>
        /// 通常のラベル背景色
        /// </summary>
        internal readonly Color DefaultLabelBackColor = Color.FromName(appRes.ResourceManager.GetString("DefaultLabel_BackColor") ?? "Control");

        /// <summary>
        /// 通常のテキストボックス背景色
        /// </summary>
        internal readonly Color DefaultTextBoxBackColor = Color.FromName(appRes.ResourceManager.GetString("DefaultTextBox_BackColor") ?? "Window");

        /// <summary>
        /// 通常の文字色
        /// </summary>
        internal readonly Color DefaultTextColor = Color.FromName(appRes.ResourceManager.GetString("DefaultTextBox_ForeColor") ?? "ControlText");

        public InstructionViewerForm(RV32_HaedwareThread cpu) {
            this.cpu = cpu;
            string dirPath = @"..\..\..\..\riscv-tests\build\isa\";
            string exePath = dirPath + "rv32si-p-wfi";
            this.cpu.LoadProgram(exePath);
            this.cpu.registerSet.Mem.HostAccessAddress.Add(this.cpu.GetToHostAddr());


            InitializeComponent();

            this.DumpViewTextBox.Text = File.ReadAllText(exePath + ".dump");

            #region 表示用ラベル配列・テキストボックス配列初期化

            // 32bit長バイナリ形式命令ラベル
            BinaryInstructionLabels = new (Label, int, string)[] {
                (BinaryInstructionLabel_26_32, 7, ""),
                (BinaryInstructionLabel_21_25, 5, "rs2"),
                (BinaryInstructionLabel_16_20, 5, "rs1"),
                (BinaryInstructionLabel_13_15, 3, ""),
                (label4, 5, "rd"),
                (label5, 7, "")
            };

            // 16bit長バイナリ形式命令ラベル
            ComplexedBinaryInstructionLabels = new (Label, int, string)[] {
            };

            // 命令引数ラベル
            ArgLabels = new Label[] {
                this.Arg1Name, this.Arg1Value,
                this.Arg2Name, this.Arg2Value,
                this.Arg3Name, this.Arg3Value,
                this.Arg4Name, this.Arg4Value,
                this.Arg5Name, this.Arg5Value
            };

            #endregion


            RiscvInstruction ins = InstConverter.GetInstruction(cpu.registerSet.IR);
            Dictionary<string, ulong> reg = cpu.registerSet.GetAllRegisterData();

            UpdateInstructionLabels(ins);
            UpdateArgumentLabels(ins);
            this.IntegerRegistersControl.UpdateRegisterTextBoxes(ins, reg);
            this.FloatPointRegistersControl.UpdateRegisterTextBoxes(ins, reg);
            UpdateStatusLabels();
            ChangeColorDumpTextBox();

            csrViewer = new ControlStatusRegisterViewer();
            csrViewer.ControlStatusRegistersControl.UpdateData(reg);
            csrViewer.Show();

            this.Update();
        }

        /// <summary>
        /// 命令テキストラベルを更新する
        /// </summary>
        private void UpdateInstructionLabels(RiscvInstruction ins) {


            // プログラムカウンタ
            this.PCValueLabel.Text = "0x" + cpu.registerSet.PC.ToString("X").PadLeft(8, '0');

            // 命令レジスタ
            this.InstructionNameLabel.Text = ins.Name;

            // 実行モード
            this.CurrentModeLabel.Text = Enum.GetName(typeof(PrivilegeLevels), cpu.registerSet.CurrentMode);
            // 2進数の命令
            int insLength = 32;
            string insStr = Convert.ToString(cpu.registerSet.IR, 2).PadLeft(insLength, '0');

            int i = insLength;
            foreach ((Label label, int length, string name) in BinaryInstructionLabels) {
                label.Text = insStr.Substring((insLength - i), length);
                i -= length;
                Color color = Color.FromName(appRes.ResourceManager.GetString("RegisterLabel_BackColor_" + name) ?? "");
                if (color != Color.Black && ins.Arguments.Keys.Contains(name)) {
                    label.BackColor = color;
                } else {
                    label.BackColor = DefaultLabelBackColor;
                }

            }

            this.BinaryInstructionDigitLabel.Text = appRes.ResourceManager.GetString("InstructionType_" + ins.InstructionType) ?? "";

        }

        /// <summary>
        /// 引数テキストラベルを更新する
        /// </summary>
        private void UpdateArgumentLabels(RiscvInstruction ins) {
            int i = 0;

            // 引数1～5
            foreach (string key in ins.Arguments.Keys) {
                ArgLabels[i++].Text = key;

                if (key.StartsWith("rs") || key.Equals("rd")) {
                    // 整数レジスタ名
                    ArgLabels[i].Text = RISC_V_Instruction.InstConverter.IntergerRegisterName[(int)ins.Arguments[key]];

                } else if (key.StartsWith("frs") || key.Equals("frd")) {
                    // 浮動小数点レジスタ名
                    ArgLabels[i].Text = RISC_V_Instruction.InstConverter.FloatPointRegisterName[(int)ins.Arguments[key]];

                } else if (key.Equals("csr")) {
                    //コントロール・ステータスレジスタ名
                    ArgLabels[i].Text = Enum.GetName(typeof(CSR), (CSR)ins.Arguments[key]);

                } else if (key.Equals("immediate") && ins.InstructionType.Equals("J")) {
                    // 即値(J形式)
                    ArgLabels[i].Text = "0x" + ins.Arguments[key].ToString("X").PadLeft(5, '0');

                } else if (key.Equals("immediate") && (ins.InstructionType.Equals("B") || ins.InstructionType.Equals("S"))) {
                    // 即値(B,S形式)
                    ArgLabels[i].Text = "0x" + ins.Arguments[key].ToString("X").PadLeft(3, '0');

                } else {
                    // 即値(その他)
                    ArgLabels[i].Text = ins.Arguments[key].ToString();
                }

                if (key.Equals("rd") || key.Equals("frd") || key.StartsWith("rs") || key.StartsWith("frs")) {
                    Color color = Color.FromName(appRes.ResourceManager.GetString("RegisterLabel_BackColor_" + key) ?? "");
                    if (color != Color.Black) {
                        ArgLabels[i++].BackColor = color;
                    }

                } else {
                    ArgLabels[i++].BackColor = DefaultLabelBackColor;
                }
            }

            while (i < ArgLabels.Length) {
                ArgLabels[i].Text = "";
                ArgLabels[i++].BackColor = DefaultLabelBackColor;
            }

        }


        /// <summary>
        /// ステータスラベルを更新する
        /// </summary>
        private void UpdateStatusLabels() {
            // ステータスラベル
            if (cpu.registerSet.IsWaitMode) {
                this.statusLabel.Text = appRes.StatusLabel_Text_Wait;
            } else if (cpu.HasException) {
                string exCauseCode = ((uint)cpu.ExceptionCause).ToString("X").ToLower().PadLeft(8, '0');
                string exCauseText = appRes.ResourceManager.GetString("ExceptionCause_" + exCauseCode) ?? "";

                if(((UInt32)cpu.ExceptionCause & 0x8000_0000u) == 0u) {
                    this.statusLabel.Text = appRes.StatusLabel_Text_Exception + " " + exCauseText;
                } else {
                    this.statusLabel.Text = appRes.StatusLabel_Text_Interrupt + " " + exCauseText;
                }

            } else {
                this.statusLabel.Text = appRes.StatusLabel_Text_Running;
            }

        }

        /// <summary>
        /// 画面左部のテキストボックス内から
        /// 現在のPCの行の背景色を変更する
        /// </summary>
        private void ChangeColorDumpTextBox() {
            RichTextBox dump = this.DumpViewTextBox;
            string t = dump.Text.ToLower();

            string pc = (cpu.registerSet.PC.ToString("X").PadLeft(8, '0') + ":").ToLower();

            int startIdx = t.IndexOf("\n" + pc) + 1;
            if (startIdx < 1) return;

            int startLineCount = (startIdx - t.Substring(0, startIdx).Replace("\n", "").Length);

            int length = t.IndexOf("\n", startIdx) - startIdx;
            if (length <= 0) return;

            dump.Select(0, t.Length);
            dump.SelectionBackColor = DefaultTextBoxBackColor;

            Color color = Color.FromName(appRes.ResourceManager.GetString("DumpTextBox_CurrentStepLine_BackColor") ?? "");
            if (color != Color.Black) {
                dump.Select(startIdx, length);
                dump.SelectionBackColor = color;
            }
            dump.Select(startIdx - 1, 1);
            dump.ScrollToCaret();
        }

        #region イベント時呼び出しメソッド

        private void InstructionViewerForm_Resize(object sender, EventArgs e) {
            Console.WriteLine(((Control)sender).Width + ", " + ((Control)sender).Height);

            const int offsetWidth = 570;
            const int offsetHeight = 100;
            const int windowsDefaultWidth = 1160;
            if (((Control)sender).Width > windowsDefaultWidth + offsetWidth && !IsWideMode) {
                IsWideMode = true;

                // 横移動
                this.ArgumentsGroupBox.Left -= offsetWidth;
                this.ProgramCounterGroupBox.Left -= offsetWidth/2;
                this.CurrentModeGroupBox.Left -= offsetWidth;
                this.IntegerRegistersControl.Left -= offsetWidth;

                // 横サイズ変更
                this.DumpViewTextBox.Width -= offsetWidth;

                // 縦移動
                this.ArgumentsGroupBox.Top -= this.BinaryInstructionPanel.Height - 10;
                this.IntegerRegistersControl.Top -= this.BinaryInstructionPanel.Height - 10;
                this.ProgramCounterGroupBox.Top -= offsetHeight;
                this.CurrentModeGroupBox.Top -= this.ArgumentsGroupBox.Height - this.CurrentModeGroupBox.Height;
                this.FloatPointRegistersControl.Top -= offsetHeight * 2 + this.IntegerRegistersControl.Height;

            } else if (((Control)sender).Width <= windowsDefaultWidth + offsetWidth && IsWideMode) {
                IsWideMode = false;

                // 横移動
                this.ArgumentsGroupBox.Left += offsetWidth;
                this.ProgramCounterGroupBox.Left += offsetWidth / 2;
                this.CurrentModeGroupBox.Left += offsetWidth;
                this.IntegerRegistersControl.Left += offsetWidth;

                // 横サイズ変更
                this.DumpViewTextBox.Width += offsetWidth;

                // 縦移動
                this.ArgumentsGroupBox.Top += this.BinaryInstructionPanel.Height - 10;
                this.IntegerRegistersControl.Top += this.BinaryInstructionPanel.Height - 10;
                this.ProgramCounterGroupBox.Top += offsetHeight;
                this.CurrentModeGroupBox.Top += this.ArgumentsGroupBox.Height - this.CurrentModeGroupBox.Height;
                this.FloatPointRegistersControl.Top += offsetHeight * 2 + this.IntegerRegistersControl.Height;
            }


            if (!this.IntegerRegistersControl.Visible && ((Control)sender).Height > (IsWideMode ? 1055 - ((offsetHeight * 2) + this.IntegerRegistersControl.Height) : 730)) {
                this.IntegerRegistersControl.Visible = true;
            } else if (this.IntegerRegistersControl.Visible && ((Control)sender).Height <= (IsWideMode ? 1055 - ((offsetHeight * 2) + this.IntegerRegistersControl.Height) : 730)) {
                this.IntegerRegistersControl.Visible = false;
            }
            if (!this.FloatPointRegistersControl.Visible && ((Control)sender).Height > 1055 - (IsWideMode ? (offsetHeight * 2) + this.IntegerRegistersControl.Height : 0)) {
                this.FloatPointRegistersControl.Visible = true;
            } else if (this.FloatPointRegistersControl.Visible && ((Control)sender).Height <= 1055 - (IsWideMode ? (offsetHeight * 2) + this.IntegerRegistersControl.Height : 0)) {
                this.FloatPointRegistersControl.Visible = false;
            }
        }

        private void StepExecuteButton_Click(object sender, EventArgs e) {

            try {
                RiscvInstruction boforeIns = InstConverter.GetInstruction(cpu.registerSet.IR);
                this.IntegerRegistersControl.ChangeColorRegisterTextBoxesBeforeExecute(boforeIns);
                this.FloatPointRegistersControl.ChangeColorRegisterTextBoxesBeforeExecute(boforeIns);

                // プログラムを1ステップ実行
                cpu.StepExecute();

                RiscvInstruction ins = InstConverter.GetInstruction(cpu.registerSet.IR);
                Dictionary<string, ulong> reg = cpu.registerSet.GetAllRegisterData();

                UpdateInstructionLabels(ins);
                UpdateArgumentLabels(ins);
                this.IntegerRegistersControl.UpdateRegisterTextBoxes(ins, reg);
                this.FloatPointRegistersControl.UpdateRegisterTextBoxes(ins, reg);
                UpdateStatusLabels();
                ChangeColorDumpTextBox();

                csrViewer.ControlStatusRegistersControl.UpdateData(reg);

            } catch (HostAccessTrap t) {
                MessageBox.Show(this,
                    "処理が完了しました\r\n" +
                    "    値 : " + t.Data["value"],
                    "実行完了",
                    MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                this.StepExecuteButton.Enabled = false;
            }
        }

        #endregion
    }
}