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
using insTypeRes = global::RISC_V_CPU_Emulator.Properties.InstructionDigitTypeResources;

namespace RISC_V_CPU_Emulator {
    public partial class InstructionViewerForm : Form {

        private RV32_HaedwareThread cpu;

        private Label[] IntegerRegisterLabels;
        private TextBox[] IntegerRegisterTextBoxes;
        private Label[] FloatPointRegisterLabels;
        private TextBox[] FloatPointRegisterTextBoxes;

        public InstructionViewerForm(RV32_HaedwareThread cpu) {
            this.cpu = cpu;
            string dirPath = @"..\..\..\..\riscv-tests\build\isa\";
            string exePath = dirPath + "rv32si-p-wfi";
            this.cpu.LoadProgram(exePath);
            this.cpu.registerSet.Mem.HostAccessAddress.Add(this.cpu.GetToHostAddr());


            InitializeComponent();

            this.DumpViewTextBox.Text = File.ReadAllText(exePath + ".dump");

            #region レジスタ表示用ラベル・テキストボックス配列初期化

            IntegerRegisterLabels = new Label[] {
                IntegerRegisterLabel0, IntegerRegisterLabel1, IntegerRegisterLabel2, IntegerRegisterLabel3, IntegerRegisterLabel4,
                IntegerRegisterLabel5, IntegerRegisterLabel6, IntegerRegisterLabel7, IntegerRegisterLabel8, IntegerRegisterLabel9,
                IntegerRegisterLabel10, IntegerRegisterLabel11, IntegerRegisterLabel12, IntegerRegisterLabel13, IntegerRegisterLabel14,
                IntegerRegisterLabel15, IntegerRegisterLabel16, IntegerRegisterLabel17, IntegerRegisterLabel18, IntegerRegisterLabel19,
                IntegerRegisterLabel20, IntegerRegisterLabel21, IntegerRegisterLabel22, IntegerRegisterLabel23, IntegerRegisterLabel24,
                IntegerRegisterLabel25, IntegerRegisterLabel26, IntegerRegisterLabel27, IntegerRegisterLabel28, IntegerRegisterLabel29,
                IntegerRegisterLabel30, IntegerRegisterLabel31
            };

            IntegerRegisterTextBoxes = new TextBox[] {
                IntegerRegisterTextBox0, IntegerRegisterTextBox1, IntegerRegisterTextBox2, IntegerRegisterTextBox3, IntegerRegisterTextBox4,
                IntegerRegisterTextBox5, IntegerRegisterTextBox6, IntegerRegisterTextBox7, IntegerRegisterTextBox8, IntegerRegisterTextBox9,
                IntegerRegisterTextBox10, IntegerRegisterTextBox11, IntegerRegisterTextBox12, IntegerRegisterTextBox13, IntegerRegisterTextBox14,
                IntegerRegisterTextBox15, IntegerRegisterTextBox16, IntegerRegisterTextBox17, IntegerRegisterTextBox18, IntegerRegisterTextBox19,
                IntegerRegisterTextBox20, IntegerRegisterTextBox21, IntegerRegisterTextBox22, IntegerRegisterTextBox23, IntegerRegisterTextBox24,
                IntegerRegisterTextBox25, IntegerRegisterTextBox26, IntegerRegisterTextBox27, IntegerRegisterTextBox28, IntegerRegisterTextBox29,
                IntegerRegisterTextBox30, IntegerRegisterTextBox31
            };

            FloatPointRegisterLabels = new Label[] {
                FloatPointRegisterLabel0, FloatPointRegisterLabel1, FloatPointRegisterLabel2, FloatPointRegisterLabel3, FloatPointRegisterLabel4,
                FloatPointRegisterLabel5, FloatPointRegisterLabel6, FloatPointRegisterLabel7, FloatPointRegisterLabel8, FloatPointRegisterLabel9,
                FloatPointRegisterLabel10, FloatPointRegisterLabel11, FloatPointRegisterLabel12, FloatPointRegisterLabel13, FloatPointRegisterLabel14,
                FloatPointRegisterLabel15, FloatPointRegisterLabel16, FloatPointRegisterLabel17, FloatPointRegisterLabel18, FloatPointRegisterLabel19,
                FloatPointRegisterLabel20, FloatPointRegisterLabel21, FloatPointRegisterLabel22, FloatPointRegisterLabel23, FloatPointRegisterLabel24,
                FloatPointRegisterLabel25, FloatPointRegisterLabel26, FloatPointRegisterLabel27, FloatPointRegisterLabel28, FloatPointRegisterLabel29,
                FloatPointRegisterLabel30, FloatPointRegisterLabel31
            };

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

            InitializeLabelsText();

            ChangeColorRegisterTextBox(Color.PaleGreen);
            UpdateLabelsText();
            ChangeColorCurrentStep();

            this.Update();
        }

        public void InitializeLabelsText() {
            this.CurrentModeGroupBox.Text = appRes.CurrentModeGroupBox_Text;
            this.FloatPointRegistersGroupBox.Text = appRes.FloatPointRegistersGroupBox_Text;
            this.IntegerRegistersGroupBox.Text = appRes.IntegerRegistersGroupBox_Text;
            this.BinaryInstructionLabel.Text = appRes.BinaryInstructionLabel_Text_Defalut;
            this.Text = appRes.InstructionViewerForm_Text;
            this.StepExecuteButton.Text = global::RISC_V_CPU_Emulator.Properties.Resources.StepExecuteButton_Text;
            this.ArgumentsGroupBox.Text = appRes.ArgumentsGroupBox_Text;
            this.InstructionRegisterGroupBox.Text = appRes.InstructionRegisterGroupBox_Text;
            this.ProgramCounterGroupBox.Text = appRes.ProgramCounterGroupBox_Text;

            for (int i = 0; i < 32; i++) {
                IntegerRegisterLabels[i].Text = RISC_V_Instruction.Converter.IntergerRegisterName[i];
                FloatPointRegisterLabels[i].Text = RISC_V_Instruction.Converter.FloatPointRegisterName[i];
            }
        }

        private void InstructionViewerForm_Resize(object sender, EventArgs e) {

            if (!this.IntegerRegistersGroupBox.Visible && ((Control)sender).Height >= 730) {
                this.IntegerRegistersGroupBox.Visible = true;
            } else if (this.IntegerRegistersGroupBox.Visible && ((Control)sender).Height < 730) {
                this.IntegerRegistersGroupBox.Visible = false;
            }
            if (!this.FloatPointRegistersGroupBox.Visible && ((Control)sender).Height >= 1050) {
                this.FloatPointRegistersGroupBox.Visible = true;
            } else if (this.FloatPointRegistersGroupBox.Visible && ((Control)sender).Height < 1050) {
                this.FloatPointRegistersGroupBox.Visible = false;
            }
        }

        /// <summary>
        /// テキストラベルを更新する
        /// </summary>
        private void UpdateLabelsText() {

            RISC_V_Instruction.RiscvInstruction ins = RISC_V_Instruction.Converter.GetInstruction(cpu.registerSet.IR);

            // プログラムカウンタ
            this.PCValueLabel.Text = "0x" + cpu.registerSet.PC.ToString("X").PadLeft(8, '0');

            // 命令レジスタ
            this.InstructionNameLabel.Text = ins.Name;

            // 実行モード
            this.CurrentModeLabel.Text = Enum.GetName(typeof(PrivilegeLevels), cpu.registerSet.CurrentMode);
            ;

            // 2進数の命令
            if (ins.Name.StartsWith("C.")) {
                this.BinaryInstructionLabel.Text = Convert.ToString(cpu.registerSet.IR, 2).PadLeft(16, '0').PadLeft(32, ' ');
            } else {
                this.BinaryInstructionLabel.Text = Convert.ToString(cpu.registerSet.IR, 2).PadLeft(32, '0');
            }
            this.BinaryInstructionLabel.Text = this.BinaryInstructionLabel.Text.Insert(25, " ").Insert(20, " ").Insert(17, " ").Insert(12, " ").Insert(7, " ");

            this.BinaryInstructionDigitLabel.Text = insTypeRes.ResourceManager.GetString(ins.InstructionType) ?? "";

            // 引数1～5
            Label[] argLabels = new Label[] {
                this.Arg1Name, this.Arg1Value,
                this.Arg2Name, this.Arg2Value,
                this.Arg3Name, this.Arg3Value,
                this.Arg4Name, this.Arg4Value,
                this.Arg5Name, this.Arg5Value
            };

            int i = 0;
            foreach (string key in ins.Arguments.Keys) {
                argLabels[i++].Text = key;

                if (key.StartsWith("rs") || key.Equals("rd")) {
                    // 整数レジスタ名
                    argLabels[i].Text = RISC_V_Instruction.Converter.IntergerRegisterName[(int)ins.Arguments[key]];

                } else if (key.StartsWith("frs") || key.Equals("frd")) {
                    // 浮動小数点レジスタ名
                    argLabels[i].Text = RISC_V_Instruction.Converter.FloatPointRegisterName[(int)ins.Arguments[key]];

                } else if (key.Equals("csr")) {
                    //コントロール・ステータスレジスタ名
                    argLabels[i].Text = Enum.GetName(typeof(CSR), (CSR)ins.Arguments[key]);

                } else if (key.Equals("immediate") && ins.InstructionType.Equals("J")) {
                    // 即値(J形式)
                    argLabels[i].Text = "0x" + ins.Arguments[key].ToString("X").PadLeft(5, '0');

                } else if (key.Equals("immediate") && (ins.InstructionType.Equals("B") || ins.InstructionType.Equals("S"))) {
                    // 即値(B,S形式)
                    argLabels[i].Text = "0x" + ins.Arguments[key].ToString("X").PadLeft(3, '0');

                } else {
                    // 即値(その他)
                    argLabels[i].Text = ins.Arguments[key].ToString();
                }

                if (key.Equals("rd") || key.Equals("frd")) {
                    argLabels[i++].BackColor = Color.PaleGreen;

                } else {
                    argLabels[i++].BackColor = SystemColors.Control;
                }
            }

            while (i < argLabels.Length) {
                argLabels[i].Text = "";
                argLabels[i++].BackColor = SystemColors.Control;
            }

            // レジスタテキストボックス
            for (i = 0; i < 32; i++) {
                IntegerRegisterTextBoxes[i].Text = "0x" + cpu.registerSet.GetValue((Register)i).ToString("X").PadLeft(8, '0');
                ulong f = cpu.registerSet.GetValue((FPRegister)i);
                if ((f & 0xffff_ffff_0000_0000u) == 0xffff_ffff_0000_0000u) {
                    FloatPointRegisterTextBoxes[i].Text = BitConverter.ToSingle(BitConverter.GetBytes(f), 0).ToString("0.0#");
                } else {
                    FloatPointRegisterTextBoxes[i].Text = BitConverter.ToDouble(BitConverter.GetBytes(f), 0).ToString("0.0#");
                }
            }


        }
        /// <summary>
        /// 現在のPCの行の背景色を変更する
        /// </summary>
        private void ChangeColorCurrentStep() {
            RichTextBox dump = this.DumpViewTextBox;
            string t = dump.Text.ToLower();

            string pc = (cpu.registerSet.PC.ToString("X").PadLeft(8, '0') + ":").ToLower();

            int startIdx = t.IndexOf("\n" + pc) + 1;
            if (startIdx < 1) return;

            int startLineCount = (startIdx - t.Substring(0, startIdx).Replace("\n", "").Length);

            int length = t.IndexOf("\n", startIdx) - startIdx;
            if (length <= 0) return;

            dump.Select(0, t.Length);
            dump.SelectionBackColor = Color.White;

            dump.Select(startIdx, length);
            dump.SelectionBackColor = Color.LightPink;
            dump.Select(startIdx - 1, 1);
            dump.ScrollToCaret();
        }


        /// <summary>レジスタ値のテキストボックスの背景を白に戻す</summary>
        private void ClearColorRegisterTextBox() {
            for (int i = 0; i < 16; i++) {
                IntegerRegisterTextBoxes[i].BackColor = Color.White;
                FloatPointRegisterTextBoxes[i].BackColor = Color.White;
            }
        }

        /// <summary>現在の命令レジスタの命令に含まれるレジスタのテキストボックスの背景を指定した色に変更する</summary>
        private void ChangeColorRegisterTextBox(Color color) {

            RISC_V_Instruction.RiscvInstruction ins = RISC_V_Instruction.Converter.GetInstruction(cpu.registerSet.IR);

            foreach (string key in ins.Arguments.Keys) {
                TextBox target = null;
                if (key.Equals("rd")) {
                    target = IntegerRegisterTextBoxes[(int)ins.Arguments[key]];

                    // 浮動小数点レジスタ名
                } else if (key.Equals("frd")) {
                    target = FloatPointRegisterTextBoxes[(int)ins.Arguments[key]];
                }
                if (!(target is null) && target.BackColor.Equals(Color.White)) {
                    target.BackColor = color;
                }
            }

        }

        private void StepExecuteButton_Click(object sender, EventArgs e) {

            try {

                ClearColorRegisterTextBox();
                ChangeColorRegisterTextBox(Color.SandyBrown);

                // プログラムを1ステップ実行
                cpu.StepExecute();

                ChangeColorRegisterTextBox(Color.PaleGreen);
                UpdateLabelsText();
                ChangeColorCurrentStep();

            } catch (HostAccessTrap t) {
                MessageBox.Show(this,
                    "処理が完了しました\r\n" +
                    "値: " + t.Data["value"],
                    "実行完了",
                    MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                this.StepExecuteButton.Enabled = false;
            }


            this.Update();
        }
    }
}