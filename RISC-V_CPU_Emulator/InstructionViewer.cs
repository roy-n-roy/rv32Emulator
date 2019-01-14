using RV32_Cpu;
using RV32_Lsu;
using RV32_Lsu.Constants;
using RV32_Lsu.MemoryHandler;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static RV32_Cpu.InstructionConverter;

namespace RISC_V_CPU_Emulator {
    public partial class InstructionViewer : Form {
        RV32_HaedwareThread cpu;
        public InstructionViewer(RV32_HaedwareThread cpu) {
            this.cpu = cpu;
            string dirPath = @"..\..\..\..\riscv-tests\build\isa\";
            this.cpu.LoadProgram(dirPath + "rv32mi-p-illegal");
            this.cpu.registerSet.Mem.HostAccessAddress.Add(this.cpu.GetToHostAddr());


            InitializeComponent();

            this.DumpViewTextBox.Text = File.ReadAllText(dirPath + "rv32mi-p-illegal" + ".dump");

            string mode = Enum.GetName(typeof(PrivilegeLevels), cpu.registerSet.CurrentMode);

            UpdateTextLabels(mode, cpu.registerSet.PC, cpu.registerSet.IR);
            ChangeColorCurrentStep(cpu.registerSet.PC);

            this.Update();
        }

        /// <summary>
        /// テキストラベルを更新する
        /// </summary>
        /// <param name="pcValue">PCの値</param>
        /// <param name="insValue">命令の値</param>
        private void UpdateTextLabels(string mode, uint pcValue, uint insValue) {

            RiscvInstruction ins = InstructionConverter.Decode(insValue);

            // プログラムカウンタ
            this.PCValueLabel.Text = "0x" + pcValue.ToString("X").PadLeft(8, '0');

            // 命令レジスタ
            this.InstructionNameLabel.Text = ins.Name;

            // 実行モード
            this.CurrentModeLabel.Text = mode;

            // 2進数の命令
            if (ins.Name.StartsWith("C.")) {
                this.BinaryInstructionLabel.Text = Convert.ToString(insValue, 2).PadLeft(16, '0').PadLeft(32, ' ');
            } else {
                this.BinaryInstructionLabel.Text = Convert.ToString(insValue, 2).PadLeft(32, '0');
            }
            this.BinaryInstructionLabel.Text = this.BinaryInstructionLabel.Text.Insert(25, " ").Insert(20, " ").Insert(17, " ").Insert(12, " ").Insert(7, " ");

            if (immediateType.Keys.Contains(ins.InstructionType)) {
                this.BinaryInstructionDigitLabel.Text = immediateType[ins.InstructionType];
            } else {
                this.BinaryInstructionDigitLabel.Text = "";
            }


            Label[] argLabels = new Label[] {
                this.Arg1Name,
                this.Arg1Value,
                this.Arg2Name,
                this.Arg2Value,
                this.Arg3Name,
                this.Arg3Value,
                this.Arg4Name,
                this.Arg4Value,
                this.Arg5Name,
                this.Arg5Value
            };

            int i = 0;
            foreach (string key in ins.Arguments.Keys) {
                argLabels[i++].Text = key;

                // 整数レジスタ名
                if (key.StartsWith("rs") || key.StartsWith("rd")) {
                    argLabels[i++].Text = IntergerRegisterName[(int)ins.Arguments[key]];

                    // 浮動小数点レジスタ名
                } else if (key.StartsWith("frs") || key.StartsWith("frd")) {
                    argLabels[i++].Text = FloatPointRegisterName[(int)ins.Arguments[key]];

                    //コントロール・ステータスレジスタ名
                } else if (key.Equals("csr")) {
                    argLabels[i++].Text = Enum.GetName(typeof(CSR), (CSR)ins.Arguments[key]);

                } else if (key.Equals("immediate") && ins.InstructionType.Equals("J")) {
                    argLabels[i++].Text = "0x" + ins.Arguments[key].ToString("X").PadLeft(5, '0');

                } else if (key.Equals("immediate") && (ins.InstructionType.Equals("B") || ins.InstructionType.Equals("S"))) {
                    argLabels[i++].Text = "0x" + ins.Arguments[key].ToString("X").PadLeft(3, '0');

                } else {
                    argLabels[i++].Text = ins.Arguments[key].ToString();
                }
            }
            while (i < argLabels.Length) {
                argLabels[i++].Text = "";
            }

        }
        /// <summary>
        /// 指定したPCの行の背景色を変更する
        /// </summary>
        /// <param name="pcValue">PCの値</param>
        private void ChangeColorCurrentStep(uint pcValue) {
            RichTextBox dump = this.DumpViewTextBox;
            string t = dump.Text.ToLower();

            string pc = (pcValue.ToString("X").PadLeft(8, '0') + ":").ToLower();

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

        private void StepExecuteButton_Click(object sender, EventArgs e) {

            try {
                // プログラムを1ステップ実行
                cpu.StepExecute();

                string mode = Enum.GetName(typeof(PrivilegeLevels), cpu.registerSet.CurrentMode);
                UpdateTextLabels(mode, cpu.registerSet.PC, cpu.registerSet.IR);
                ChangeColorCurrentStep(cpu.registerSet.PC);

            } catch (HostAccessTrap) {

            }


            this.Update();
        }

        /// <summary>
        /// 
        /// </summary>
        private static readonly IReadOnlyDictionary<string, string> immediateType = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>() {
            { "R",       "--------Opcode-------  -------rs2--------    -------rs1-------  -Opcide-    ------rd--------    -------Opcode--------" },
            { "I",       "----------------Immediate---------------    -------rs1-------   -Opcide-    ------rd--------    -------Opcode--------" },
            { "S",       "------Immediate------   -------rs2--------   -------rs1-------   -Opcide-   ---Immediate---    -------Opcode--------" },
            { "B",       "------Immediate------   -------rs2--------   -------rs1-------   -Opcide-   ---Immediate---    -------Opcode--------" },
            { "U",       "-------------------------------Immediate-------------------------------    ------rd--------    -------Opcode--------" },
            { "J",       "-------------------------------Immediate-------------------------------    ------rd--------    -------Opcode--------" },
            { "SHIFT",   "------Opcode---------  ------shamt------    -------rs1-------   -Opcide-    ------rd--------    -------Opcode--------" },
            { "CSR",     "--------------------CSR------------------    -------rs1-------   -Opcide-    ------rd--------    -------Opcode--------" },
            { "R4",      "-------rs3------- -Op- --------rs2-------    -------rs1-------   -Opcode-    ------rd--------    -------Opcode--------" },
            { "RF",      "--------Opcode-------  -------rs2--------  -------rs1--------  ---rm---    ------rd--------    -------Opcode--------" },
            { "NONE",    "-----------------------------------------------------OpCode-------------------------------------------------------------" },
            { "FENCE",   "-----------------------------------------------------OpCode-------------------------------------------------------------" },
            { "FMV",     "------------------Opcode-----------------  -------rs1-------   -Opcode-    ------rd--------    -------Opcode--------" },
            { "A",       "----Opcode---- -aq/rl- --------rs2-------  -------rs1-------   -Opcode-    ------rd--------    -------Opcode--------" },
        });
    }
}