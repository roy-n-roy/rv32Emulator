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
        public void Decode(UInt32 instruction, RV32_Cpu cpu) {
            bool successed = false;
            byte[] ins = SplitInstruction(instruction);
            foreach (RV32_AbstractDecoder d in decoders.Values) {
                successed |= d.Exec(ins, cpu);
                if (successed) break;
            }
            if (!successed) {
                throw new RiscvException(RiscvExceptionCause.IllegalInstruction, cpu.registerSet);
            }
        }

        /// <summary>
        /// UInt32型のRisc-V命令をbyte[]分割する
        /// </summary>
        /// <param name="instruction">UInt32型のRisc-V命令</param>
        /// <returns></returns>
        private byte[] SplitInstruction(UInt32 instruction) {
            bool[] ins = BitByteConverter.ToBits(BitConverter.GetBytes(instruction)).ToArray();
            byte[] values = new byte[7];
            values[0] = BitByteConverter.GetBytes(ins, 0, 7).First();
            values[1] = BitByteConverter.GetBytes(ins, 7, 5).First();
            values[2] = BitByteConverter.GetBytes(ins, 12, 3).First();
            values[3] = BitByteConverter.GetBytes(ins, 15, 5).First();
            values[4] = BitByteConverter.GetBytes(ins, 20, 5).First();
            values[5] = BitByteConverter.GetBytes(ins, 25, 6).First();
            values[6] = BitByteConverter.GetBytes(ins, 31, 1).First();

            return values;
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
        internal protected abstract bool Exec(byte[] instruction, RV32_Cpu cpu);

        #region 共用メソッド

        /// <summary>
        /// Risc-V命令形式に応じて、命令に渡す即値を生成する
        /// </summary>
        /// <param name="format">Risc-V命令形式</param>
        /// <param name="ins">SplitInstructionメソッドで分割したbyte[]のRisc-V命令</param>
        /// <returns>Int32型即値</returns>
        private protected Int32 GetImmediate(char format, byte[] ins) {
            UInt32 result;
            UInt32[] i = ins.Select(b => (UInt32)b).ToArray();
            switch (format) {
                case 'U':
                    result = (i[2] << 12) | (i[3] << 15) | (i[4] << 20) | (i[5] << 25);
                    break;
                case 'J':
                    result = (i[4] & 0b11110u) | (i[5] << 5) | ((i[4] & 0b1u) << 11) | (i[2] << 12) | (i[3] << 15);
                    result |= i[6] == 0u ? 0u : 0xfff00000u;
                    break;
                case 'I':
                    result = i[4] | (i[5] << 5);
                    result |= i[6] == 0u ? 0u : 0xfffff800u;
                    break;
                case 'S':
                    result = i[1] | (i[5] << 5);
                    result |= i[6] == 0u ? 0u : 0xfffff800u;
                    break;
                case 'B':
                    result = (i[1] & 0b11110) | (i[5] << 5) | ((i[1] & 0b1) << 11);
                    result |= i[6] == 0u ? 0u : 0xfffff000u;
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
