using ElfLoader;
using RV32_Alu;
using RV32_Cpu.Decoder;
using RV32_Lsu;
using RV32_Lsu.Exceptions;
using RV32_Lsu.MemoryHandler;
using RV32_Lsu.RegisterSet;
using System;
using System.IO;
using System.Threading.Tasks;

namespace RV32_Cpu {
    public class RV32_CentralProcessingUnit {
        /// <summary>
        /// 命令デコーダ
        /// </summary>
        private readonly RV32_Decoder decoder;
        /// <summary>
        /// 算術演算器
        /// </summary>
        private readonly RV32_Calculators alu;
        /// <summary>
        /// ロードストアユニット
        /// </summary>
        private readonly RV32_LoadStoreUnit lsu;
        /// <summary>
        /// レジスタ
        /// </summary>
        public readonly RV32_RegisterSet registerSet;


        #region コンストラクタ
        /// <summary>
        /// 引数で与えられた命令セットをサポートし、メインメモリを持つ
        /// RISC-Vアーキテクチャの32bitCPUを生成する
        /// </summary>
        /// <param name="OptionsInstructionSet">拡張命令セットを表す文字列</param>
        /// <param name="mem">メインメモリとして使用するUInt32配列</param>
        public RV32_CentralProcessingUnit(string OptionsInstructionSet, byte[] mem) {
            // 基本命令セットRV32I
            registerSet = new RV32_RegisterSet(new RV32_MemoryHandler(mem));

            decoder = new RV32_Decoder();
            decoder.AddDecoder(typeof(RV32I_Decoder));

            alu = new RV32_Calculators(registerSet);
            lsu = new RV32_LoadStoreUnit(registerSet);

            lsu.AddMisa('I');
            lsu.AddMisa('S');

            // 拡張命令セット
            foreach (char option in OptionsInstructionSet.ToCharArray()) {
                lsu.AddMisa(option);
                switch (option) {
                    case 'M': // RV32M 拡張命令セット
                        decoder.AddDecoder(typeof(RV32M_Decoder));
                        break;

                    case 'A': // RV32A 拡張命令セット
                        decoder.AddDecoder(typeof(RV32A_Decoder));
                        registerSet.Mem = new RV32_AtomicMemoryHandler(mem);
                        break;

                    case 'C': // RV32C 拡張命令セット
                        decoder.AddDecoder(typeof(RV32C_Decoder));
                        break;

                    case 'F': // RV32F 拡張命令セット
                        decoder.AddDecoder(typeof(RV32F_Decoder));
                        break;

                    case 'D': // RV32D 拡張命令セット
                        decoder.AddDecoder(typeof(RV32D_Decoder));
                        break;

                    default:
                        break;
                }
            }


        }

        /// <summary>
        /// 引数で与えられた命令セットをサポートし、1KBのメインメモリを持つ
        /// RISC-Vアーキテクチャの32bitCPUを生成する
        /// </summary>
        /// <param name="OptionsInstructionSet">拡張命令セットを表す文字列</param>
        public RV32_CentralProcessingUnit(string instractureSetOptions) : this(instractureSetOptions, new byte[1024]) {
        }

        /// <summary>
        /// RV32I命令セットをサポートし、引数で与えられたメインメモリを持つ
        /// RISC-Vアーキテクチャの32bitCPUを生成する
        /// </summary>
        /// <param name="mainMemory">メインメモリとして使用するUInt32配列</param>
        public RV32_CentralProcessingUnit(byte[] mainMemory) : this("I", mainMemory) {
        }

        /// <summary>
        /// RV32I命令セットをサポートし、1KBのメインメモリを持つ
        /// RISC-Vアーキテクチャの32bitCPUを生成する
        /// </summary>
        public RV32_CentralProcessingUnit() : this(new byte[1024]) {
        }

        #endregion

        #region メソッド

        /// <summary>
        /// 指定した型の算術演算器を返す
        /// </summary>
        /// <param name="type">ALUの型</param>
        /// <returns></returns>
        public RV32_AbstractCalculator Alu(Type type) { return alu.GetInstance(type); }

        /// <summary>
        /// ロード/ストアユニットを返す
        /// </summary>
        /// <param name="type">LSUの型</param>
        public RV32_AbstractLoadStoreUnit Lsu(Type type) { return lsu.GetInstance(type); }

        /// <summary>
        /// 外部からプログラムをメモリにロードする
        /// </summary>
        /// <param name="objectPath">プログラムのファイルパス</param>
        /// <returns>正常時:0, 異常時:0以外</returns>
        public int LoadProgram(string objectPath) {

            long fileSize = 0;
            int readBytes = 0;

            registerSet.Mem.Reset();

            using (BinaryReader br = new BinaryReader(File.Open(objectPath, FileMode.Open))) {
                fileSize = br.BaseStream.Length;
                readBytes = br.Read(registerSet.Mem.GetBytes(), 0, (int)fileSize);
            }

            if (fileSize == 0 || fileSize != readBytes) {
                Console.WriteLine(Path.GetFileName(objectPath) + " 実行ファイルの読み込みの失敗しました");
                return -1;
            }

            Elf32_Header elf = (Elf32_Header)registerSet.Mem.GetBytes();

            if (elf.e_type != ElfType.ET_EXEC) {
                Console.WriteLine(Path.GetFileName(objectPath) + " 実行可能ファイルではありません");
                return -1;
            }

            UInt32 entryOffset = 0,
                      virtAddr = 0,
                      physAddr = 0;

            foreach (Elf32_Phdr ph in elf.e_phdrtab) {
                if ((ph.p_flags & 0x1) > 0) {
                    entryOffset = ph.p_offset;
                    virtAddr = ph.p_vaddr;
                    physAddr = ph.p_paddr;
                    break;
                }
            }
            foreach (Elf32_Shdr sh in elf.e_shdrtab) {
                if (sh.sh_name.Equals(".tohost")) {
                    foreach (Elf32_Sym sh_sym in sh.sh_symtab) {
                        if (sh_sym.st_name.Equals("tohost")) {
                            //tohost_addr = sh_sym.st_value - sh.sh_addr + sh.sh_offset;
                            tohost_addr = sh_sym.st_value;
                            break;
                        }
                    }
                    if (tohost_addr != 0) break;
                }
            }

            lsu.ClearAndSetPC(physAddr, virtAddr, entryOffset);

            return 0;
        }

        ulong tohost_addr = 0;
        public ulong GetToHostAddr() {
            return tohost_addr;
        }

        /// <summary>
        /// ロードしたプログラムを実行する
        /// </summary>
        /// <returns>正常時:0, 異常時:0以外</returns>
        public int Run() {
            while (true) {
                try {
                    decoder.Decode(this);
#if DEBUG
                } catch (RiscvException e)
                  when ((RiscvExceptionCause)e.Data["cause"] != RiscvExceptionCause.Breakpoint &&
                        (RiscvExceptionCause)e.Data["cause"] != RiscvExceptionCause.EnvironmentCallFromMMode &&
                        (RiscvExceptionCause)e.Data["cause"] != RiscvExceptionCause.EnvironmentCallFromSMode &&
                        (RiscvExceptionCause)e.Data["cause"] != RiscvExceptionCause.EnvironmentCallFromUMode) {

                    
                    Console.Error.WriteLine("\r\n例外発生");
                    Console.Error.WriteLine("    PC            = " + ((uint)e.Data["pc"]).ToString("X"));
                    Console.Error.WriteLine("    MemoryAddress = " + ((uint)e.Data["tval"]).ToString("X"));
                    Console.Error.WriteLine(e.ToString());
#endif
                } catch (RiscvException) {
                } finally {
                    registerSet.IncrementCycle();
                }
            }
        }

        /// <summary>
        /// 全レジスタの値を表示する
        /// </summary>
        public void ShowRegister() => Console.WriteLine(registerSet);

        #endregion
    }
}
