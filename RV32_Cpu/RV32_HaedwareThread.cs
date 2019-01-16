using ElfLoader;
using RV32_Alu;
using RV32_Decoder;
using RV32_Lsu;
using RV32_Register;
using RV32_Register.Constants;
using RV32_Register.Exceptions;
using RV32_Register.MemoryHandler;
using System;
using System.IO;

namespace RV32_Cpu {
    public class RV32_HaedwareThread {
        /// <summary>
        /// 命令デコーダ
        /// </summary>
        private readonly RV32_InstructionDecoder decoder;
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

        /// <summary>
        /// ステップ実行で発生した例外、割り込み
        /// </summary>
        public RiscvException CpuExceptionInterrupt { get; private set; }

        #region コンストラクタ
        /// <summary>
        /// 引数で与えられた命令セットをサポートし、メインメモリを持つ
        /// RISC-Vアーキテクチャの32bitCPUを生成する
        /// </summary>
        /// <param name="OptionsInstructionSet">拡張命令セットを表す文字列</param>
        /// <param name="mem">メインメモリとして使用するUInt32配列</param>
        public RV32_HaedwareThread(string OptionsInstructionSet, byte[] mem) {
            // 基本命令セットRV32I
            registerSet = new RV32_RegisterSet(new RV32_MemoryHandler(mem));

            alu = new RV32_Calculators(registerSet);
            lsu = new RV32_LoadStoreUnit(registerSet);

            decoder = new RV32_InstructionDecoder(registerSet, alu, lsu);
            decoder.AddDecoder(typeof(RV32I_Decoder));

            registerSet.AddMisa('I');
            registerSet.AddMisa('S');

            // 拡張命令セット
            foreach (char option in OptionsInstructionSet.ToCharArray()) {
                registerSet.AddMisa(option);
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
        public RV32_HaedwareThread(string instractureSetOptions) : this(instractureSetOptions, new byte[1024]) {
        }

        /// <summary>
        /// RV32I命令セットをサポートし、引数で与えられたメインメモリを持つ
        /// RISC-Vアーキテクチャの32bitCPUを生成する
        /// </summary>
        /// <param name="mainMemory">メインメモリとして使用するUInt32配列</param>
        public RV32_HaedwareThread(byte[] mainMemory) : this("I", mainMemory) {
        }

        /// <summary>
        /// RV32I命令セットをサポートし、1KBのメインメモリを持つ
        /// RISC-Vアーキテクチャの32bitCPUを生成する
        /// </summary>
        public RV32_HaedwareThread() : this(new byte[1024]) {
        }

        #endregion

        #region メソッド

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

            registerSet.ClearAndSetPC(physAddr, virtAddr, entryOffset);

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
                StepExecute();
            }
        }

        /// <summary>
        /// 1ステップ(1サイクル)命令を実行する
        /// </summary>
        public void StepExecute() {
            try {
                // 前回ステップの割り込み、例外の記録をクリアする
                CpuExceptionInterrupt = null;

                // 割り込みチェックとハンドリング
                if (registerSet.CheckAndHandleInterrupt()) {
                    // メモリアドレスの予約開放
                    registerSet.Mem.Reset();

                    // ToDo: 標準入力制御の実装
                    // ToDo: GUIから参照用の割り込み変数へのデータ格納

                } else {
                    // 割り込みが無ければ、命令レジスタから命令を取り出して、デコード・実行する
                    RISC_V_Instruction.RiscvInstruction ins = RISC_V_Instruction.Converter.GetInstruction(registerSet.IR);
                    decoder.Decode();
                }

#if DEBUG
                // デバッグモードの際、ブレークポイント/環境呼び出し例外以外の例外・割り込みをコンソールに出力する。
            } catch (RiscvException e)
                  when ((RiscvExceptionCause)e.Data["cause"] != RiscvExceptionCause.Breakpoint &&
                        (RiscvExceptionCause)e.Data["cause"] != RiscvExceptionCause.EnvironmentCallFromMMode &&
                        (RiscvExceptionCause)e.Data["cause"] != RiscvExceptionCause.EnvironmentCallFromSMode &&
                        (RiscvExceptionCause)e.Data["cause"] != RiscvExceptionCause.EnvironmentCallFromUMode) {

                CpuExceptionInterrupt = e;

                Console.Error.WriteLine("\r\n例外発生");
                Console.Error.WriteLine("    PC        = 0x" + ((uint)e.Data["pc"]).ToString("X"));
                Console.Error.WriteLine("    TrapValue = 0x" + ((uint)e.Data["tval"]).ToString("X"));
                Console.Error.WriteLine(e.ToString());
#endif
            } catch (RiscvException e) {
                CpuExceptionInterrupt = e;

            } finally {
                registerSet.IncrementCycle();
            }
        }

        /// <summary>
        /// 全レジスタの値を表示する
        /// </summary>
        public void ShowRegister() => Console.WriteLine(registerSet);

        #endregion
    }
}
