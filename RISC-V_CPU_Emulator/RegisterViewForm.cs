using RISC_V_Instruction;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RISC_V_CPU_Emulator {
    public partial class RegisterViewerForm : Form {
        public RegisterViewerForm() {
            InitializeComponent();
        }

        internal UserControl RegisgerControl;
    }
}
