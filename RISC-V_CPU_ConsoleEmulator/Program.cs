using RV32_Cpu;
using RV32_Lsu;
using RV32_Lsu.Constants;
using RV32_Lsu.MemoryHandler;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace RISC_V_CPU_ConsoleEmulator {
    class Program {
        static void Main(string[] args) {

            string dirPath = @"..\..\..\..\..\riscv-tests\build\isa\";

            Regex rex;
            rex = new Regex("rv32[msu][a-z]-p-[a-z_]+$", RegexOptions.Compiled);

            string[] files = Directory.GetFiles(dirPath).Where<string>(
                f => (rex.IsMatch(f))).ToArray();

            byte[] mem = new byte[32 * 1024];

            RV32_HaedwareThread cpu = new RV32_HaedwareThread("AMCFD", mem);

            RunRiscvProgram(cpu, files);
        }

        private static void RunRiscvProgram(RV32_HaedwareThread cpu, string[] RiscvBinaryFiles) {

            float all = RiscvBinaryFiles.Length;
            float successed = 0f;

            foreach (string file in RiscvBinaryFiles) {

                Console.Write(Path.GetFileName(file).PadRight(20));

                int rCode;
                rCode = cpu.LoadProgram(file);
                if (rCode != 0) {
                    Console.WriteLine(" " + cpu.registerSet.PC.ToString("X") + " : プログラム読み込み失敗");
                    break;
                }

                cpu.registerSet.Mem.HostAccessAddress.Add(cpu.GetToHostAddr());

                string errstr = null;
                while (true) {
                    try {
                        rCode = cpu.Run();
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
            Console.Read();
        }
    }
}