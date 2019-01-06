using RiscVCpu.LoadStoreUnit.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RiscVCpu.Decoder {

    /// <summary>
    /// 
    /// </summary>
    public class RV32_Decoder {

        private static readonly Dictionary<Type, RV32_AbstractDecoder> decoders = new Dictionary<Type, RV32_AbstractDecoder>();

        /// <summary>
        /// デコーダに命令セットを追加する
        /// </summary>
        /// <param name="type">任意の命令セットを実装したRV32_AbstractDecoder派生クラスの型</param>
        public void AddDecoder(Type type) {
            if (!decoders.ContainsKey(type)) {
                decoders.Add(type, (RV32_AbstractDecoder)Activator.CreateInstance(type));
            }
        }

        /// <summary>
        /// 引数で渡された32bit長の命令を、各デコーダでデコードする
        /// </summary>
        /// <param name="instruction">32bit長の命令</param>
        /// <param name="cpu">命令を実行するRV32CPU</param>
        public void Decode(RV32_Cpu cpu) {
            bool successed = false;
            UInt32[] ins = SplitInstruction(cpu.registerSet.IR);

            foreach (RV32_AbstractDecoder d in decoders.Values) {
                successed |= d.Exec(ins, cpu);
                if (successed) break;
            }
            if (!successed) {
                throw new RiscvException(RiscvExceptionCause.IllegalInstruction, 0, cpu.registerSet);
            }
        }

        /// <summary>
        /// UInt32型のRisc-V命令をbyte[]分割する
        /// </summary>
        /// <param name="instruction">UInt32型のRisc-V命令</param>
        /// <returns></returns>
        private UInt32[] SplitInstruction(UInt32 instruction) {
            UInt32[] values = new UInt32[7];
            values[0] = Slice(instruction, 0, 7);
            values[1] = Slice(instruction, 7, 5);
            values[2] = Slice(instruction, 12, 3);
            values[3] = Slice(instruction, 15, 5);
            values[4] = Slice(instruction, 20, 5);
            values[5] = Slice(instruction, 25, 6);
            values[6] = Slice(instruction, 31, 1);

            return values;
        }

        /// <summary>数値をbyte配列として返します</summary>
        public static UInt32 Slice(UInt32 data, int startIndex, int count) {
            return ((data >> startIndex) & (UInt32.MaxValue >> (32 - count)));
        }
    }

    /// <summary>
    /// Risc-V 32bitCPU デコーダ抽象クラス
    /// </summary>
    public abstract class RV32_AbstractDecoder {

        internal RV32_AbstractDecoder() {
        }

        /// <summary>
        /// 引数で渡された32bit長の命令をデコードし、cpuで実行する
        /// </summary>
        /// <param name="instruction">32bit長の命令</param>
        /// <param name="cpu">命令を実行するRV32CPU</param>
        /// <returns>実行の成否</returns>
        internal protected abstract bool Exec(UInt32[] instruction, RV32_Cpu cpu);

        #region 共用メソッド

        /// <summary>
        /// Risc-V命令形式に応じて、命令に渡す即値を生成する
        /// </summary>
        /// <param name="format">Risc-V命令形式</param>
        /// <param name="ins">SplitInstructionメソッドで分割したbyte[]のRisc-V命令</param>
        /// <returns>Int32型即値</returns>
        private protected Int32 GetImmediate(char format, UInt32[] ins) {
            UInt32 result;
            switch (format) {
                case 'U':
                    result = (ins[2] << 12) | (ins[3] << 15) | (ins[4] << 20) | (ins[5] << 25);
                    result |= ins[6] == 0u ? 0u : 0x8000_0000u;
                    break;
                case 'J':
                    result = (ins[4] & 0b11110u) | (ins[5] << 5) | ((ins[4] & 0b1u) << 11) | (ins[2] << 12) | (ins[3] << 15);
                    result |= ins[6] == 0u ? 0u : 0xfff0_0000u;
                    break;
                case 'I':
                    result = ins[4] | (ins[5] << 5);
                    result |= ins[6] == 0u ? 0u : 0xffff_f800u;
                    break;
                case 'S':
                    result = ins[1] | (ins[5] << 5);
                    result |= ins[6] == 0u ? 0u : 0xffff_f800u;
                    break;
                case 'B':
                    result = (ins[1] & 0b11110) | (ins[5] << 5) | ((ins[1] & 0b1) << 11);
                    result |= ins[6] == 0u ? 0u : 0xffff_f000u;
                    break;
                default:
                    result = 0;
                    break;
            }
            return (Int32)result;
        }

        #endregion
    }

}
