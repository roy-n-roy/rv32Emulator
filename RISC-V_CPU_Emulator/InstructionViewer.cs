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

        private RV32_HaedwareThread cpu;

        private (Label, int, string)[] BinaryInstructionLabels;
        private (Label, int, string)[] ComplexedBinaryInstructionLabels;

        private Label[] ArgLabels;

        private Label[] IntegerRegisterLabels;
        private TextBox[] IntegerRegisterTextBoxes;
        private Label[] FloatPointRegisterLabels;
        private TextBox[] FloatPointRegisterTextBoxes;

        private bool IsWideMode = false;

        /// <summary>
        /// 通常のラベル背景色
        /// </summary>
        private Color DefaultLabelBackColor = Color.FromName(appRes.ResourceManager.GetString("DefaultLabel_BackColor") ?? "Control");
        
        /// <summary>
        /// 通常のテキストボックス背景色
        /// </summary>
        private Color DefaultTextBoxBackColor = Color.FromName(appRes.ResourceManager.GetString("DefaultTextBox_BackColor") ?? "Window");

        /// <summary>
        /// 通常の文字色
        /// </summary>
        private Color DefaultTextColor = Color.FromName(appRes.ResourceManager.GetString("DefaultTextBox_ForeColor") ?? "ControlText");

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

            // 整数レジスタ名ラベル
            IntegerRegisterLabels = new Label[] {
                IntegerRegisterLabel0, IntegerRegisterLabel1, IntegerRegisterLabel2, IntegerRegisterLabel3, IntegerRegisterLabel4,
                IntegerRegisterLabel5, IntegerRegisterLabel6, IntegerRegisterLabel7, IntegerRegisterLabel8, IntegerRegisterLabel9,
                IntegerRegisterLabel10, IntegerRegisterLabel11, IntegerRegisterLabel12, IntegerRegisterLabel13, IntegerRegisterLabel14,
                IntegerRegisterLabel15, IntegerRegisterLabel16, IntegerRegisterLabel17, IntegerRegisterLabel18, IntegerRegisterLabel19,
                IntegerRegisterLabel20, IntegerRegisterLabel21, IntegerRegisterLabel22, IntegerRegisterLabel23, IntegerRegisterLabel24,
                IntegerRegisterLabel25, IntegerRegisterLabel26, IntegerRegisterLabel27, IntegerRegisterLabel28, IntegerRegisterLabel29,
                IntegerRegisterLabel30, IntegerRegisterLabel31
            };

            // 整数レジスタ値テキストボックス
            IntegerRegisterTextBoxes = new TextBox[] {
                IntegerRegisterTextBox0, IntegerRegisterTextBox1, IntegerRegisterTextBox2, IntegerRegisterTextBox3, IntegerRegisterTextBox4,
                IntegerRegisterTextBox5, IntegerRegisterTextBox6, IntegerRegisterTextBox7, IntegerRegisterTextBox8, IntegerRegisterTextBox9,
                IntegerRegisterTextBox10, IntegerRegisterTextBox11, IntegerRegisterTextBox12, IntegerRegisterTextBox13, IntegerRegisterTextBox14,
                IntegerRegisterTextBox15, IntegerRegisterTextBox16, IntegerRegisterTextBox17, IntegerRegisterTextBox18, IntegerRegisterTextBox19,
                IntegerRegisterTextBox20, IntegerRegisterTextBox21, IntegerRegisterTextBox22, IntegerRegisterTextBox23, IntegerRegisterTextBox24,
                IntegerRegisterTextBox25, IntegerRegisterTextBox26, IntegerRegisterTextBox27, IntegerRegisterTextBox28, IntegerRegisterTextBox29,
                IntegerRegisterTextBox30, IntegerRegisterTextBox31
            };

            // 浮動小数点レジスタ名ラベル
            FloatPointRegisterLabels = new Label[] {
                FloatPointRegisterLabel0, FloatPointRegisterLabel1, FloatPointRegisterLabel2, FloatPointRegisterLabel3, FloatPointRegisterLabel4,
                FloatPointRegisterLabel5, FloatPointRegisterLabel6, FloatPointRegisterLabel7, FloatPointRegisterLabel8, FloatPointRegisterLabel9,
                FloatPointRegisterLabel10, FloatPointRegisterLabel11, FloatPointRegisterLabel12, FloatPointRegisterLabel13, FloatPointRegisterLabel14,
                FloatPointRegisterLabel15, FloatPointRegisterLabel16, FloatPointRegisterLabel17, FloatPointRegisterLabel18, FloatPointRegisterLabel19,
                FloatPointRegisterLabel20, FloatPointRegisterLabel21, FloatPointRegisterLabel22, FloatPointRegisterLabel23, FloatPointRegisterLabel24,
                FloatPointRegisterLabel25, FloatPointRegisterLabel26, FloatPointRegisterLabel27, FloatPointRegisterLabel28, FloatPointRegisterLabel29,
                FloatPointRegisterLabel30, FloatPointRegisterLabel31
            };

            // 浮動小数点レジスタ値テキストボックス
            FloatPointRegisterTextBoxes = new TextBox[] {
                FloatPointRegisterTextBox0, FloatPointRegisterTextBox1, FloatPointRegisterTextBox2, FloatPointRegisterTextBox3, FloatPointRegisterTextBox4,
                FloatPointRegisterTextBox5, FloatPointRegisterTextBox6, FloatPointRegisterTextBox7, FloatPointRegisterTextBox8, FloatPointRegisterTextBox9,
                FloatPointRegisterTextBox10, FloatPointRegisterTextBox11, FloatPointRegisterTextBox12, FloatPointRegisterTextBox13, FloatPointRegisterTextBox14,
                FloatPointRegisterTextBox15, FloatPointRegisterTextBox16, FloatPointRegisterTextBox17, FloatPointRegisterTextBox18, FloatPointRegisterTextBox19,
                FloatPointRegisterTextBox20, FloatPointRegisterTextBox21, FloatPointRegisterTextBox22, FloatPointRegisterTextBox23, FloatPointRegisterTextBox24,
                FloatPointRegisterTextBox25, FloatPointRegisterTextBox26, FloatPointRegisterTextBox27, FloatPointRegisterTextBox28, FloatPointRegisterTextBox29,
                FloatPointRegisterTextBox30, FloatPointRegisterTextBox31
            };

            #endregion

            InitializeRegisterNameLabels();

            RiscvInstruction ins = Converter.GetInstruction(cpu.registerSet.IR);

            UpdateInstructionLabels(ins);
            UpdateArgumentLabels(ins);
            UpdateRegisterTextBoxes(ins);
            UpdateStatusLabels();
            ChangeColorDumpTextBox();

            this.Update();
        }

        /// <summary>
        /// 整数・浮動小数点レジスタ名のテキストラベルを初期化する
        /// </summary>
        private void InitializeRegisterNameLabels() {
            for (int i = 0; i < 32; i++) {
                IntegerRegisterLabels[i].Text = RISC_V_Instruction.Converter.IntergerRegisterName[i];
                FloatPointRegisterLabels[i].Text = RISC_V_Instruction.Converter.FloatPointRegisterName[i];
            }
        }

        /// <summary>
        /// ステップ実行前の命令を引数として、
        /// 前回の実行時に書き換えられたレジスタ値テキストボックスの文字色を変更する
        /// </summary>
        /// <param name="ins"></param>
        private void ChangeColorRegisterTextBoxesBeforeExecute(RiscvInstruction ins) {
            // テキストボックスの文字色を黒に戻す
            for (int i = 0; i < 16; i++) {
                IntegerRegisterTextBoxes[i].ForeColor = DefaultTextColor;
                FloatPointRegisterTextBoxes[i].ForeColor = DefaultTextColor;
            }

            // 
            foreach (string key in ins.Arguments.Keys) {
                TextBox target = null;
                if (key.Equals("rd")) {
                    target = IntegerRegisterTextBoxes[(int)ins.Arguments[key]];

                    // 浮動小数点レジスタ名
                } else if (key.Equals("frd")) {
                    target = FloatPointRegisterTextBoxes[(int)ins.Arguments[key]];
                }

                Color color = Color.FromName(appRes.ResourceManager.GetString("RegisterLabel_ForeColor_" + key) ?? "");

                if (!(target is null) && color != Color.Black) {
                    target.ForeColor = color;
                }
            }
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
                    ArgLabels[i].Text = RISC_V_Instruction.Converter.IntergerRegisterName[(int)ins.Arguments[key]];

                } else if (key.StartsWith("frs") || key.Equals("frd")) {
                    // 浮動小数点レジスタ名
                    ArgLabels[i].Text = RISC_V_Instruction.Converter.FloatPointRegisterName[(int)ins.Arguments[key]];

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
        /// レジスタ値テキストボックスを更新する
        /// </summary>
        private void UpdateRegisterTextBoxes(RiscvInstruction ins) {
            Dictionary<string, ulong> reg = cpu.registerSet.GetAllRegisterData();
            // テキストボックスの値を更新する
            for (int i = 0; i < 32; i++) {
                IntegerRegisterTextBoxes[i].Text = "0x" + ((int)reg["x"+ i]).ToString("X").PadLeft(8, '0');
                if ((reg["f" + i] & 0xffff_ffff_0000_0000u) == 0xffff_ffff_0000_0000u) {
                    FloatPointRegisterTextBoxes[i].Text = BitConverter.ToSingle(BitConverter.GetBytes(reg["f" + i]), 0).ToString("0.0#");
                } else {
                    FloatPointRegisterTextBoxes[i].Text = BitConverter.ToDouble(BitConverter.GetBytes(reg["f" + i]), 0).ToString("0.0#");
                }
            }

            // テキストボックスの背景を白に戻す
            for (int i = 0; i < 16; i++) {
                IntegerRegisterTextBoxes[i].BackColor = DefaultTextBoxBackColor;
                FloatPointRegisterTextBoxes[i].BackColor = DefaultTextBoxBackColor;
            }

            // レジスタ名がRISC-V命令の引数内にあるか確認する
            foreach (string key in ins.Arguments.Keys) {
                TextBox target = null;

                if (key.Equals("rd") || key.StartsWith("rs")) {
                    target = IntegerRegisterTextBoxes[(int)ins.Arguments[key]];

                    // 浮動小数点レジスタ名
                } else if (key.Equals("frd") || key.StartsWith("frs")) {
                    target = FloatPointRegisterTextBoxes[(int)ins.Arguments[key]];
                }

                Color color = Color.FromName(appRes.ResourceManager.GetString("RegisterLabel_BackColor_" + key) ?? "");

                if (!(target is null) && color != Color.Black && target.BackColor.Equals(DefaultTextBoxBackColor)) {
                    target.BackColor = color;
                }
            }

            string frm = appRes.ResourceManager.GetString("FloatPointStatusRegisterTextBox_RoundMode_" + ((reg["fcsr"] & 0xe0u) >> 5).ToString("X").ToLower());
            string fflag = appRes.ResourceManager.GetString("FloatPointStatusRegisterTextBox_ExceptionFlag_" + (reg["fcsr"] & 0x1fu).ToString("X").ToLower().PadLeft(2, '0'));
            this.FloatPointStatusRegisterTextBox.Text = " " + (frm is null ? "" : frm) + (fflag is null ? "" : " + " + fflag);
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
                this.IntegerRegistersGroupBox.Left -= offsetWidth;

                // 横サイズ変更
                this.DumpViewTextBox.Width -= offsetWidth;

                // 縦移動
                this.ArgumentsGroupBox.Top -= this.BinaryInstructionPanel.Height - 10;
                this.IntegerRegistersGroupBox.Top -= this.BinaryInstructionPanel.Height - 10;
                this.ProgramCounterGroupBox.Top -= offsetHeight;
                this.CurrentModeGroupBox.Top -= this.ArgumentsGroupBox.Height - this.CurrentModeGroupBox.Height;
                this.FloatPointRegistersGroupBox.Top -= offsetHeight * 2 + this.IntegerRegistersGroupBox.Height;

            } else if (((Control)sender).Width <= windowsDefaultWidth + offsetWidth && IsWideMode) {
                IsWideMode = false;

                // 横移動
                this.ArgumentsGroupBox.Left += offsetWidth;
                this.ProgramCounterGroupBox.Left += offsetWidth / 2;
                this.CurrentModeGroupBox.Left += offsetWidth;
                this.IntegerRegistersGroupBox.Left += offsetWidth;

                // 横サイズ変更
                this.DumpViewTextBox.Width += offsetWidth;

                // 縦移動
                this.ArgumentsGroupBox.Top += this.BinaryInstructionPanel.Height - 10;
                this.IntegerRegistersGroupBox.Top += this.BinaryInstructionPanel.Height - 10;
                this.ProgramCounterGroupBox.Top += offsetHeight;
                this.CurrentModeGroupBox.Top += this.ArgumentsGroupBox.Height - this.CurrentModeGroupBox.Height;
                this.FloatPointRegistersGroupBox.Top += offsetHeight * 2 + this.IntegerRegistersGroupBox.Height;
            }


            if (!this.IntegerRegistersGroupBox.Visible && ((Control)sender).Height > (IsWideMode ? 1055 - ((offsetHeight * 2) + this.IntegerRegistersGroupBox.Height) : 730)) {
                this.IntegerRegistersGroupBox.Visible = true;
            } else if (this.IntegerRegistersGroupBox.Visible && ((Control)sender).Height <= (IsWideMode ? 1055 - ((offsetHeight * 2) + this.IntegerRegistersGroupBox.Height) : 730)) {
                this.IntegerRegistersGroupBox.Visible = false;
            }
            if (!this.FloatPointRegistersGroupBox.Visible && ((Control)sender).Height > 1055 - (IsWideMode ? (offsetHeight * 2) + this.IntegerRegistersGroupBox.Height : 0)) {
                this.FloatPointRegistersGroupBox.Visible = true;
            } else if (this.FloatPointRegistersGroupBox.Visible && ((Control)sender).Height <= 1055 - (IsWideMode ? (offsetHeight * 2) + this.IntegerRegistersGroupBox.Height : 0)) {
                this.FloatPointRegistersGroupBox.Visible = false;
            }
        }

        private void StepExecuteButton_Click(object sender, EventArgs e) {

            try {
                RiscvInstruction boforeIns = Converter.GetInstruction(cpu.registerSet.IR);
                ChangeColorRegisterTextBoxesBeforeExecute(boforeIns);

                // プログラムを1ステップ実行
                cpu.StepExecute();

                RiscvInstruction ins = Converter.GetInstruction(cpu.registerSet.IR);

                UpdateInstructionLabels(ins);
                UpdateArgumentLabels(ins);
                UpdateRegisterTextBoxes(ins);
                UpdateStatusLabels();
                ChangeColorDumpTextBox();

            } catch (HostAccessTrap t) {
                MessageBox.Show(this,
                    "処理が完了しました\r\n" +
                    "    値 : " + t.Data["value"],
                    "実行完了",
                    MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                this.StepExecuteButton.Enabled = false;
            }


            this.Update();
        }

        #endregion
    }
}