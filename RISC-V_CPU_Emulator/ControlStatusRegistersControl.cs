using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using RISC_V_Instruction;

namespace RISC_V_CPU_Emulator {
    public partial class ControlStatusRegistersControl : UserControl, InstructionViewerForm.IRegisterControl {

        private BindingList<KeyValuePair<string, ulong>> csr;

        public ControlStatusRegistersControl() {
            InitializeComponent();
        }

        public void UpdateRegisterBeforeExecute(RiscvInstruction ins) {
        }

        public void UpdateRegisterData(RISC_V_Instruction.RiscvInstruction ins, Dictionary<string, ulong> registers) {
            if (this.CSRDataGrid.DataSource is null) {
                Regex ifregex = new Regex("[fx][0-9]+$", RegexOptions.Compiled);
                Regex cntregex = new Regex("^(m|)(cycle|time|insret|hpmcounter[0-9]+|hpmevent[0-9]+)(h|)$", RegexOptions.Compiled);
                List<KeyValuePair<string, ulong>> noCounterCsr = registers.Where(r => !ifregex.IsMatch(r.Key)).Where(r => !cntregex.IsMatch(r.Key)).ToDictionary(r => r.Key, r => r.Value).ToList();
                List<KeyValuePair<string, ulong>> counterCsr = registers.Where(r => !ifregex.IsMatch(r.Key)).Where(r => cntregex.IsMatch(r.Key)).ToDictionary(r => r.Key, r => r.Value).ToList();
                csr = new BindingList<KeyValuePair<string, ulong>>(noCounterCsr.Concat(counterCsr).ToList());

                this.CSRDataGrid.DataSource = csr;
                this.CSRDataGrid.Columns[0].HeaderText = "Name";
                this.CSRDataGrid.Columns[1].HeaderText = "Value";
                this.CSRDataGrid.Columns[0].Width = 120;


            } else {
                for (int i = 0; i < csr.Count; i++) {
                    if (registers[csr[i].Key] != csr[i].Value) {
                        this.CSRDataGrid[1, i].Style.BackColor = Color.Red;
                        csr[i] = new KeyValuePair<string, ulong>(csr[i].Key, registers[csr[i].Key]);
                    } else {
                        this.CSRDataGrid[1, i].Style.BackColor = SystemColors.Window;
                    }
                }
            }
        }
    }
}
