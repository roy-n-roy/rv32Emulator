using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Runtime.CompilerServices;

namespace RISC_V_CPU_Emulator {
    public partial class ControlStatusRegistersControl : UserControl {

        private BindingList<KeyValuePair<string, ulong>> csr;

        public ControlStatusRegistersControl() {
            InitializeComponent();
        }

        internal void UpdateData(Dictionary<string, ulong> registers) {
            if (this.CSRDataGrid.DataSource is null) {
                Regex regex = new Regex("[fx][0-9]+$", RegexOptions.Compiled);
                csr = new BindingList<KeyValuePair<string, ulong>>(registers.Where(r => !regex.IsMatch(r.Key)).ToDictionary(r => r.Key, r => r.Value).ToList());
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
