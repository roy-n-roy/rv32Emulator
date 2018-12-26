using ElfLoader;
using RiscVCpu.ArithmeticLogicUnit;
using RiscVCpu.Decoder;
using RiscVCpu.Exceptions;
using RiscVCpu.LoadStoreUnit;
using System;
using System.IO;

namespace RiscVCpu {
    public class RV32_Cpu {
        /// <summary>
        /// メインメモリ
        /// </summary>
        public byte[] mem;
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
        private readonly RV32_Lsu lsu;
        /// <summary>
        /// レジスタ
        /// </summary>
        public readonly RV32_RegisterSet registerSet;

        #region コンストラクタ
        /// <summary>
        /// 引数で与えられた命令セットをサポートし、メインメモリを持つ
        /// RISC-Vアーキテクチャの32bitCPUを生成する
        /// </summary>
        /// <param name="mainMemory">メインメモリとして使用するUInt32配列</param>
        public RV32_Cpu(string instractureSetOptions, byte[] mainMemory) {
            mem = mainMemory;

            // 基本命令セットRV32I
            registerSet = new RV32_RegisterSet();
            registerSet.AddMisa('I');

            decoder = new RV32_Decoder();
            decoder.AddDecoder(typeof(RV32I_Decoder));

            alu = new RV32_Calculators(registerSet);
            lsu = new RV32_Lsu(registerSet, mem);


            // 拡張命令セット
            foreach (char option in instractureSetOptions.ToCharArray()) {
                switch (option) {
                    case 'M': // RV32M 拡張命令セット
                        decoder.AddDecoder(typeof(RV32M_Decoder));
                        lsu.AddMisa(option);
                        break;

                    case 'A': // RV32A 拡張命令セット
                        break;

                    case 'F': // RV32F 拡張命令セット
                        break;

                    case 'D': // RV32D 拡張命令セット
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
        /// <param name="mainMemory">メインメモリとして使用するUInt32配列</param>
        public RV32_Cpu(string instractureSetOptions) : this(instractureSetOptions, new byte[1024]) {
        }

        /// <summary>
        /// RV32I命令セットをサポートし、引数で与えられたメインメモリを持つ
        /// RISC-Vアーキテクチャの32bitCPUを生成する
        /// </summary>
        /// <param name="mainMemory">メインメモリとして使用するUInt32配列</param>
        public RV32_Cpu(byte[] mainMemory) : this("I", mainMemory) {
        }

        /// <summary>
        /// RV32I命令セットをサポートし、1KBのメインメモリを持つ
        /// RISC-Vアーキテクチャの32bitCPUを生成する
        /// </summary>
        public RV32_Cpu() : this(new byte[1024]) {
        }

        #endregion

        #region プロパティ
        /// <summary>
        /// 指定した型の算術演算器を返す
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public RV32_AbstractCalculator Alu(Type type) { return alu.GetInstance(type); }

        /// <summary>
        /// ロードストアユニットを返す
        /// </summary>
        public RV32_Lsu Lsu => lsu;
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
            using (BinaryReader br = new BinaryReader(File.Open(objectPath, FileMode.Open))) {
                fileSize = br.BaseStream.Length;
                readBytes = br.Read(mem, 0, (int)fileSize);
            }

            if (fileSize == 0 || fileSize != readBytes) {
                Console.WriteLine(Path.GetFileName(objectPath) + " 実行ファイルの読み込みの失敗しました");
                return -1;
            }

            Elf32_Header elf = (Elf32_Header)mem;

            if (elf.e_type != ElfType.ET_EXEC) {
                Console.WriteLine(Path.GetFileName(objectPath) + " 実行可能ファイルではありません");
                return -1;
            }

            UInt32 entryAddr = 0;

            foreach (Elf32_Phdr ph in elf.ep_header) {
                if ((ph.p_flags & 0x1) > 0) {
                    entryAddr = ph.p_offset;
                    break;
                }
            }

            lsu.ClearAndSetPC(entryAddr);

            return 0;
        }

        /// <summary>
        /// ロードしたプログラムを実行する
        /// </summary>
        /// <returns>正常時:0, 異常時:0以外</returns>
        public int Run() {
            int ret = 0;
            //try {
                while (true) {
                    UInt32 ins = BitConverter.ToUInt32(mem, (int)registerSet.PC);
                    if (ins == 0) {
                        break;
                    }
                    Decode(ins);
                    registerSet.IncrementCycle();
                }
            /*
            } catch (RiscvException e) {
                switch (e.Cause) {
                    case RiscvExceptionCause.Breakpoint:
                        break;
                    case RiscvExceptionCause.EnvironmentCallFromUMode:
                        break;
                    case RiscvExceptionCause.EnvironmentCallFromSMode:
                        break;
                    case RiscvExceptionCause.EnvironmentCallFromMMode:
                        break;
                    default:
                        Console.Error.WriteLine(e);
                        ret = -1;
                        throw;
                }
            }*/
            return ret;
        }


        /// <summary>
        /// 引数で指定した命令をデコード、実行する
        /// </summary>
        /// <param name="instruction">デコード、実行する命令</param>
        private void Decode(UInt32 instruction) {
            decoder.Decode(instruction, this);
        }

        /// <summary>
        /// 全レジスタの値を表示する
        /// </summary>
        public void ShowRegister() => Console.WriteLine(registerSet);

        #endregion
    }
}
