using RiscVCpu.LoadStoreUnit.Constants;
using RiscVCpu.LoadStoreUnit.Exceptions;
using RiscVCpu.MemoryHandler;
using RiscVCpu.RegisterSet;
using System;

namespace RiscVCpu.LoadStoreUnit {

    /// <summary>
    /// Risc-V RV32I 浮動小数点命令セット ロードストアユニット
    /// </summary>
    public class RV32_FloatPointLsu : RV32_AbstractLoadStoreUnit {

        /// <summary>
        /// Risc-V 浮動小数ロードストアユニット
        /// </summary>
        /// <param name="registerSet">入出力用レジスタ</param>
        /// <param name="mainMemory">メインメモリ</param>
        public RV32_FloatPointLsu(RV32_RegisterSet registerSet, RV32_AbstractMemoryHandler mainMemory) : base(registerSet, mainMemory) {
        }

        #region Risc-V CPU命令

        #region Risc-V CPU Load系命令

        /// <summary>
        /// Floating-Point Load Word命令
        /// レジスタrs1+offsetのアドレスにあるメモリから、4バイトを浮動小数点レジスタrdに書き込む
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">ロードする対象のアドレスのベースが格納されているレジスタ番号</param>
        /// <param name="offset">オフセット</param>
        public bool Flw(FPRegister rd, Register rs1, Int32 offset, UInt32 insLength = 4u) {
            UInt64 addr = (UInt64)(reg.GetValue(rs1) + offset);
            if (reg.Mem.CanOperate(addr, 4)) {
                byte[] bytes = new byte[8];
                bytes[0] = reg.Mem[addr + 0];
                bytes[1] = reg.Mem[addr + 1];
                bytes[2] = reg.Mem[addr + 2];
                bytes[3] = reg.Mem[addr + 3];
                bytes[4] = 0xff;
                bytes[5] = 0xff;
                bytes[6] = 0xff;
                bytes[7] = 0xff;
                reg.SetValue(rd, BitConverter.ToUInt64(bytes, 0));
            }
            reg.IncrementPc(insLength);
            return true;
        }

        /// <summary>
        /// Floating-Point Load Double Word命令
        /// レジスタrs1+offsetのアドレスにあるメモリから、8バイトを浮動小数点レジスタrdに書き込む
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">ロードする対象のアドレスのベースが格納されているレジスタ番号</param>
        /// <param name="offset">オフセット</param>
        public bool Fld(FPRegister rd, Register rs1, Int32 offset, UInt32 insLength = 4u) {
            UInt64 addr = (UInt64)(reg.GetValue(rs1) + offset);
            if (reg.Mem.CanOperate(addr, 8)) {
                byte[] bytes = new byte[8];
                bytes[0] = reg.Mem[addr + 0];
                bytes[1] = reg.Mem[addr + 1];
                bytes[2] = reg.Mem[addr + 2];
                bytes[3] = reg.Mem[addr + 3];
                bytes[4] = reg.Mem[addr + 4];
                bytes[5] = reg.Mem[addr + 5];
                bytes[6] = reg.Mem[addr + 6];
                bytes[7] = reg.Mem[addr + 7];
                reg.SetValue(rd, BitConverter.ToUInt64(bytes, 0));
            }
            reg.IncrementPc(insLength);
            return true;
        }

        #endregion

        #region Risc-V CPU Store系命令

        /// <summary>
        /// Floating-Point Store Word命令
        /// 浮動小数点レジスタrs2から4バイトをレジスタrs1+offsetのアドレスのメモリに格納する
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">ストアする対象のアドレスのベースが格納されているレジスタ番号</param>
        /// <param name="offset">オフセット</param>
        public bool Fsw(Register rs1, FPRegister rs2, Int32 offset, UInt32 insLength = 4u) {
            UInt64 addr = (UInt64)(reg.GetValue(rs1) + offset);
            if (reg.Mem.CanOperate(addr, 4)) {
                byte[] bytes = BitConverter.GetBytes(reg.GetValue(rs2));
                reg.Mem[addr + 0] = bytes[0];
                reg.Mem[addr + 1] = bytes[1];
                reg.Mem[addr + 2] = bytes[2];
                reg.Mem[addr + 3] = bytes[3];
            }
            reg.IncrementPc(insLength);
            return true;
        }

        /// <summary>
        /// Floating-Point Store Double Word命令
        /// 浮動小数点レジスタrs2から8バイトをレジスタrs1+offsetのアドレスのメモリに格納する
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">ストアする対象のアドレスのベースが格納されているレジスタ番号</param>
        /// <param name="offset">オフセット</param>
        public bool Fsd(Register rs1, FPRegister rs2, Int32 offset, UInt32 insLength = 4u) {
            UInt64 addr = (UInt64)(reg.GetValue(rs1) + offset);
            if (reg.Mem.CanOperate(addr, 8)) {
                byte[] bytes = BitConverter.GetBytes(reg.GetValue(rs2));
                reg.Mem[addr + 0] = bytes[0];
                reg.Mem[addr + 1] = bytes[1];
                reg.Mem[addr + 2] = bytes[2];
                reg.Mem[addr + 3] = bytes[3];
                reg.Mem[addr + 4] = bytes[4];
                reg.Mem[addr + 5] = bytes[5];
                reg.Mem[addr + 6] = bytes[6];
                reg.Mem[addr + 7] = bytes[7];
            }
            reg.IncrementPc(insLength);
            return true;
        }

        #endregion

        #endregion
    }
}
