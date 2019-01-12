using RV32_Cpu;
using RV32_Lsu;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static RISC_V_CPU_Emulator.InstructionConverter;

namespace RISC_V_CPU_Emulator {
    public partial class InstructionViewer : Form {
        public InstructionViewer(RV32_CentralProcessingUnit cpu) {
            InitializeComponent();
        }

        public void UpdateView(uint v) {

            RiscvInstruction ins = InstructionConverter.Decode(v);

            this.BinaryInstructionLabel.Text = Convert.ToString(v, 2).PadLeft(32, '0');
            this.InstructionNameLabel.Text = ins.Name;

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
                argLabels[i++].Text = ins.Arguments[key].ToString();
            }
            while (i < 10) {
                argLabels[i++].Text = "";
            }

            this.Update();
        }

        public void Test() {
            this.Arg1Name.Text = "arg1";
            this.Arg1Value.Text = "value1value1";
            this.Arg2Name.Text = "arg2";
            this.Arg2Value.Text = "value2value2";
            this.Arg3Name.Text = "arg3";
            this.Arg3Value.Text = "value3value3";
            this.Arg4Name.Text = "arg4";
            this.Arg4Value.Text = "value4value4";
            this.Arg5Name.Text = "arg5";
            this.Arg5Value.Text = "value5value5";
        }

        private void ExecuteButton_Click(object sender, EventArgs e) {
            Test();
        }
    }
}