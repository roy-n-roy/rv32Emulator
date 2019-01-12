using RV32_Cpu;
using RV32_Lsu;
using RV32_Lsu.Constants;
using RV32_Lsu.MemoryHandler;
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

            string dirPath = @"..\..\..\..\riscv-tests\build\isa\";

            Regex rex;
            rex = new Regex("rv32[msu][a-z]-p-[a-z_]+$", RegexOptions.Compiled);
            //rex = new Regex("rv32ua-p-lrsc$", RegexOptions.Compiled);

            string[] files = Directory.GetFiles(dirPath).Where<string>(
                f => (rex.IsMatch(f))).ToArray();

            byte[] mem = new byte[32 * 1024];

            RV32_CentralProcessingUnit cpu = new RV32_CentralProcessingUnit("AMCFD", mem);

            Application.Run(new InstructionViewer(cpu));


            Task.Run(() => {
                RunRiscvProgram(cpu, files);
            });
        }

        private static void RunRiscvProgram(RV32_CentralProcessingUnit cpu, string[] RiscvBinaryFiles) {

            float all = RiscvBinaryFiles.Length;
            float successed = 0f;

            foreach (string file in RiscvBinaryFiles) {

                Console.Write(Path.GetFileName(file).PadRight(20));

                int rCode = 0;
                rCode = cpu.LoadProgram(file);

                cpu.registerSet.Mem.HostAccessAddress.Add(cpu.GetToHostAddr());

                string errstr = null;
                while (true) {
                    try {
                        rCode = cpu.Run();
                        if (rCode == 0) {
                            break;
                        }
                    } catch (HostAccessTrap) {
                        break;
                    }
                }
                if (rCode != 0) {
                    Console.WriteLine(" " + cpu.registerSet.PC.ToString("X") + " : プログラム異常終了");
                    break;
                }

                if (cpu.registerSet.GetValue(Register.gp) == 1) {
                    Console.WriteLine(" : テスト成功");
                    successed++;
                } else if (cpu.registerSet.GetValue(Register.gp) != 0) {
                    Console.WriteLine(" : テスト失敗 : test_" + (cpu.registerSet.GetValue(Register.gp) >> 1));
                } else if (errstr != null) {
                    Console.WriteLine(" : エラー     :" + errstr);
                }
            }
            Console.WriteLine("成功  : " + successed.ToString("#,0"));
            Console.WriteLine("失敗  : " + (all - successed).ToString("#,0"));
            Console.WriteLine("成功率: " + ((successed / all) * 100f).ToString("#,0.00") + "%");
        }
    }
}