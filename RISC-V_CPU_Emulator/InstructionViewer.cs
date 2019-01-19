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

        #region 変数・定数定義

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

        /// <summary>整数レジスタ表示画面</summary>
        private RegisterViewerForm irViewer;
        /// <summary>浮動小数点レジスタ表示画面</summary>
        private RegisterViewerForm fprViewer;
        /// <summary>コントロール・ステータスレジスタ表示画面</summary>
        private RegisterViewerForm csrViewer;

        /// <summary>通常のラベル背景色</summary>
        internal static readonly Color DefaultLabelBackColor = Color.FromName(appRes.ResourceManager.GetString("DefaultLabel_BackColor") ?? "Control");

        /// <summary>通常のテキストボックス背景色</summary>
        internal static readonly Color DefaultTextBoxBackColor = Color.FromName(appRes.ResourceManager.GetString("DefaultTextBox_BackColor") ?? "Window");

        /// <summary>通常の文字色</summary>
        internal static readonly Color DefaultTextColor = Color.FromName(appRes.ResourceManager.GetString("DefaultTextBox_ForeColor") ?? "ControlText");

        #endregion

        public InstructionViewerForm(RV32_HaedwareThread cpu) {
            this.cpu = cpu;
            string dirPath = @"..\..\..\..\riscv-tests\build\isa\";
            string exePath = dirPath + "rv32ui-v-simple";
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
                (BinaryInstructionLabel_12_8, 5, "rd"),
                (BinaryInstructionLabel_1_7, 7, "")
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
            };

            #endregion


            RiscvInstruction ins = InstConverter.GetInstruction(cpu.registerSet.IR);
            Dictionary<string, ulong> reg = cpu.registerSet.GetAllRegisterData();

            UpdateInstructionLabels(ins);
            UpdateArgumentLabels(ins);
            this.IntegerRegistersControl.UpdateRegisterData(ins, reg);
            this.FloatPointRegistersControl.UpdateRegisterData(ins, reg);
            UpdateStatusLabels();
            ChangeColorDumpTextBox();

            this.Update();
        }

        #region ラベル・テキストボックス更新メソッド

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

                if (((UInt32)cpu.ExceptionCause & 0x8000_0000U) == 0U) {
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

        #endregion

        /// <summary>
        /// メインウインドウ内のコンポーネントを条件に応じて再配置する
        /// </summary>
        private void ReplaceComponents() {
            const int offsetWidth = 570;
            const int offsetHeight = 60;
            const int windowsDefaultWidth = 1160;
            if (this.Width > windowsDefaultWidth + offsetWidth && !IsWideMode) {
                IsWideMode = true;

                // 横移動
                this.ArgumentsGroupBox.Left -= offsetWidth;
                this.ProgramCounterGroupBox.Left -= offsetWidth / 2;
                this.CurrentModeGroupBox.Left -= offsetWidth;
                this.IntegerRegistersControl.Left -= offsetWidth;

                // 横サイズ変更
                this.DumpViewTextBox.Width -= offsetWidth;

                // 縦移動
                this.ArgumentsGroupBox.Top -= this.BinaryInstructionPanel.Height - 10;
                this.IntegerRegistersControl.Top -= this.BinaryInstructionPanel.Height - 10;
                this.ProgramCounterGroupBox.Top -= offsetHeight;
                this.CurrentModeGroupBox.Top -= this.ArgumentsGroupBox.Height - this.CurrentModeGroupBox.Height;
                this.FloatPointRegistersControl.Top -= 150 + this.IntegerRegistersControl.Height;

            } else if (this.Width <= windowsDefaultWidth + offsetWidth && IsWideMode) {
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
                this.FloatPointRegistersControl.Top += 150 + this.IntegerRegistersControl.Height;
            }


            if (!this.IntegerRegistersControl.Visible && this.Height > (IsWideMode ? 1040 - (150 + this.IntegerRegistersControl.Height) : 680) && irViewer is null) {
                this.IntegerRegistersControl.Visible = true;
            } else if (this.IntegerRegistersControl.Visible && this.Height <= (IsWideMode ? 1040 - (150 + this.IntegerRegistersControl.Height) : 680) && fprViewer is null) {
                this.IntegerRegistersControl.Visible = false;
            }
            if (!this.FloatPointRegistersControl.Visible && this.Height > 1040 - (IsWideMode ? 150 + this.IntegerRegistersControl.Height : 0) && fprViewer is null) {
                this.FloatPointRegistersControl.Visible = true;
            } else if (this.FloatPointRegistersControl.Visible && this.Height <= 1040 - (IsWideMode ? 150 + this.IntegerRegistersControl.Height : 0) && fprViewer is null) {
                this.FloatPointRegistersControl.Visible = false;
            }
        }

        /// <summary>
        /// 1ステップ実行、もしくは指定されたPCまで実行する
        /// </summary>
        private void Execute() {

            try {
                RiscvInstruction boforeIns = InstConverter.GetInstruction(cpu.registerSet.IR);
                this.irViewer?.RegisgerControl.UpdateRegisterBeforeExecute(boforeIns);
                this.fprViewer?.RegisgerControl.UpdateRegisterBeforeExecute(boforeIns);
                this.IntegerRegistersControl.UpdateRegisterBeforeExecute(boforeIns);
                this.FloatPointRegistersControl.UpdateRegisterBeforeExecute(boforeIns);


                if (this.RunPcCheckBox.Checked) {
                    if (uint.TryParse(this.RunPcTextBox.Text.Substring(2, 8),
                        System.Globalization.NumberStyles.HexNumber, null, out uint skipPc)) {
                        while (cpu.registerSet.PC != skipPc) cpu.StepExecute();
                        this.RunPcCheckBox.Checked = false;
                    } else {
                        MessageBox.Show(this,
                            "16進数文字列を入力してください",
                            "エラー",
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        this.RunPcCheckBox.Checked = false;
                        return;
                    }
                } else {
                    // プログラムを1ステップ実行
                    cpu.StepExecute();
                }

                RiscvInstruction ins = InstConverter.GetInstruction(cpu.registerSet.IR);
                Dictionary<string, ulong> reg = cpu.registerSet.GetAllRegisterData();

                UpdateInstructionLabels(ins);
                UpdateArgumentLabels(ins);
                UpdateStatusLabels();
                ChangeColorDumpTextBox();

                this.IntegerRegistersControl.UpdateRegisterData(ins, reg);
                this.FloatPointRegistersControl.UpdateRegisterData(ins, reg);
                this.irViewer?.RegisgerControl.UpdateRegisterData(ins, reg);
                this.fprViewer?.RegisgerControl.UpdateRegisterData(ins, reg);
                this.csrViewer?.RegisgerControl.UpdateRegisterData(ins, reg);

            } catch (HostAccessTrap t) {
                MessageBox.Show(this,
                    "処理が完了しました\r\n" +
                    "    値 : " + t.Data["value"],
                    "実行完了",
                    MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                this.StepExecuteButton.Enabled = false;
            }
        }

        #region イベント時呼び出しメソッド

        // 実行ボタン押下時
        private void StepExecuteButton_Click(object sender, EventArgs e) {
            this.Execute();
        }

        // PCテキストボックス入力時
        private void RunPcTextBox_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e) {

            bool keyPressCheck = true;

            if (('A' <= e.KeyValue && e.KeyValue <= 'F') || ('0' <= e.KeyValue && e.KeyValue <= '9') || 
                ((int)Keys.NumPad0 <= e.KeyValue && e.KeyValue <= (int)Keys.NumPad9) || 
                ((int)Keys.Left == e.KeyValue || (int)Keys.Right == e.KeyValue)) {
                keyPressCheck = false;
            } else if ((int)Keys.Enter == e.KeyValue) {
                keyPressCheck = false;
                this.Execute();
            }

            e.SuppressKeyPress = keyPressCheck;
        }

        // ウインドウリサイズ時
        private void InstructionViewerForm_Resize(object sender, EventArgs e) {
            //Console.WriteLine(((Control)sender).Width + ", " + ((Control)sender).Height);
            ReplaceComponents();
        }

        // 整数レジスタ表示チェックボックス変更時
        private void DisplayIRegisterCheckBox_CheckedChanged(object sender, EventArgs e) {
            if (this.DisplayIRegisterCheckBox.Checked) {
                RiscvInstruction ins = InstConverter.GetInstruction(cpu.registerSet.IR);
                Dictionary<string, ulong> reg = cpu.registerSet.GetAllRegisterData();

                // RegisterViewFormを使用して整数レジスタウィンドウを表示する
                this.irViewer = new RegisterViewerForm();
                this.irViewer.ResumeLayout(true);
                this.irViewer.SuspendLayout();
                this.irViewer.RegisgerControl = new RISC_V_CPU_Emulator.IntegerRegistersControl {
                    Name = "IntegerRegistersControl",
                    Location = new Point(5, 5),
                    Padding = new Padding(5)
                };
                this.irViewer.Controls.Add(this.irViewer.RegisgerControl);
                this.irViewer.Text = "Integer Register Viewer";
                this.irViewer.AutoSize = true;
                this.irViewer.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                this.irViewer.ResumeLayout(false);

                this.irViewer.RegisgerControl.UpdateRegisterData(ins, reg);
                this.irViewer.FormClosed += IrViewer_FormClosed;
                this.irViewer.Show();
                this.IntegerRegistersControl.Visible = false;
            } else {
                this.irViewer?.Close();
                this.ReplaceComponents();
            }
        }

        // 浮動小数点レジスタ表示チェックボックス変更時
        private void DisplayFPRegisterCheckBox_CheckedChanged(object sender, EventArgs e) {
            if (DisplayFPRegisterCheckBox.Checked) {
                RiscvInstruction ins = InstConverter.GetInstruction(cpu.registerSet.IR);
                Dictionary<string, ulong> reg = cpu.registerSet.GetAllRegisterData();

                // RegisterViewFormを使用して浮動小数点レジスタウィンドウを表示する
                this.fprViewer = new RegisterViewerForm();
                this.fprViewer.ResumeLayout(true);
                this.fprViewer.SuspendLayout();
                this.fprViewer.RegisgerControl = new RISC_V_CPU_Emulator.FloatPointRegistersControl {
                    Name = "FloatPointRegistersControl",
                    Location = new Point(5, 5),
                    Padding = new Padding(5)
                };
                this.fprViewer.Controls.Add(this.fprViewer.RegisgerControl);
                this.fprViewer.Text = "Float-Point Register Viewer";
                this.fprViewer.AutoSize = true;
                this.fprViewer.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                this.fprViewer.ResumeLayout(false);

                this.fprViewer.RegisgerControl.UpdateRegisterData(ins, reg);
                this.fprViewer.FormClosed += FprViewer_FormClosed;
                this.fprViewer.Show();
                this.FloatPointRegistersControl.Visible = false;
            } else {
                this.fprViewer?.Close();
                this.ReplaceComponents();
            }
        }

        // CSR表示チェックボックス変更時
        private void DisplayCSRegisterCheckBox_CheckedChanged(object sender, EventArgs e) {
            if (DisplayCSRegisterCheckBox.Checked) {
                Dictionary<string, ulong> reg = cpu.registerSet.GetAllRegisterData();
                RiscvInstruction ins = InstConverter.GetInstruction(cpu.registerSet.IR);

                // RegisterViewFormを使用してCSRウィンドウを表示する
                this.csrViewer = new RegisterViewerForm();
                this.csrViewer.ResumeLayout(true);
                this.csrViewer.SuspendLayout();
                this.csrViewer.RegisgerControl = new RISC_V_CPU_Emulator.ControlStatusRegistersControl {
                    Name = "ControlStatusRegistersControl",
                    Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                    Location = new Point(0, 0),
                    Padding = new Padding(5),
                    Size = new Size(500, 500)
                };
                this.csrViewer.Controls.Add(this.csrViewer.RegisgerControl);
                this.csrViewer.Text = "Control-Status Register Viewer";
                this.csrViewer.Height = this.csrViewer.RegisgerControl.Height + 40;
                this.csrViewer.Width = this.csrViewer.RegisgerControl.Width + 20;
                this.csrViewer.AutoSizeMode = AutoSizeMode.GrowOnly;


                this.csrViewer.ResumeLayout(false);

                this.csrViewer.RegisgerControl.UpdateRegisterData(ins, reg);
                this.csrViewer.FormClosed += CsrViewer_FormClosed;
                this.csrViewer.Show();
            } else {
                this.csrViewer?.Close();
            }
        }

        // 整数レジスタ表示ウィンドウクローズ時
        private void IrViewer_FormClosed(object sender, FormClosedEventArgs e) {
            this.DisplayIRegisterCheckBox.Checked = false;
            this.irViewer = null;
        }

        // 浮動小数点レジスタ表示ウィンドウクローズ時
        private void FprViewer_FormClosed(object sender, FormClosedEventArgs e) {
            this.DisplayFPRegisterCheckBox.Checked = false;
            this.fprViewer = null;
        }

        // CSR表示ウィンドウクローズ時
        private void CsrViewer_FormClosed(object sender, FormClosedEventArgs e) {
            this.DisplayCSRegisterCheckBox.Checked = false;
            this.csrViewer = null;
        }

        #endregion
    }
}