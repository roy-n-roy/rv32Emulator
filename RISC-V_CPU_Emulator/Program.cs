using RV32_Cpu;
using RV32_Lsu;
using RV32_Register.Constants;
using RV32_Register.MemoryHandler;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RISC_V_CPU_Emulator {
    static class Program {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);


            byte[] mem = new byte[32 * 1024];

            RV32_HaedwareThread cpu = new RV32_HaedwareThread("IAMFDC", mem);

            Application.Run(new InstructionViewerForm(cpu));
        }
    }
}