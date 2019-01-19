using RV32_Register.MemoryHandler;
using RV32_Register;
using RV32_Register.Constants;
using RV32_Register.Exceptions;
using System;

namespace RV32_Lsu {

    /// <summary>
    /// Risc-V RV32I 基本命令セット ロードストアユニット
    /// </summary>
    public class RV32_IntegerLsu : RV32_AbstractLoadStoreUnit {

        /// <summary>
        /// Risc-V ロードストアユニット
        /// </summary>
        /// <param name="registerSet">入出力用レジスタ</param>
        /// <param name="mainMemory">メインメモリ</param>
        public RV32_IntegerLsu(RV32_RegisterSet registerSet, RV32_AbstractMemoryHandler mainMemory) : base(registerSet, mainMemory) {
        }

        #region Risc-V CPU命令

        #region Risc-V CPU Load系命令

        /// <summary>
        /// Load Byte命令
        /// レジスタrs1+offsetのアドレスにあるメモリから、1バイトを符号拡張してレジスタrdに書き込む
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">ロードする対象のアドレスのベースが格納されているレジスタ番号</param>
        /// <param name="offset">オフセット</param>
        public bool Lb(Register rd, Register rs1, Int32 offset, UInt32 insLength = 4U) {
            UInt32 addr = (UInt32)(reg.GetValue(rs1) + offset);
            if (reg.Mem.CanOperate(addr, 1)) {
                byte[] bytes = new byte[4];
                try {
                    bytes[0] = reg.Mem[addr];
                    bytes[1] = (byte)((bytes[0] & 0x80U) > 1U ? 0xffU : 0U);
                    bytes[2] = (byte)((bytes[0] & 0x80U) > 1U ? 0xffU : 0U);
                    bytes[3] = (byte)((bytes[0] & 0x80U) > 1U ? 0xffU : 0U);
                    reg.SetValue(rd, BitConverter.ToUInt32(bytes, 0));
                    reg.IncrementPc(insLength);
                } catch (RiscvException e)
           when (((RiscvExceptionCause)e.Data["cause"]) == RiscvExceptionCause.LoadPageFault) {
                }
            }
            return true;
        }
        /// <summary>
        /// Load Byte Unsigned命令
        /// レジスタrs1+offsetのアドレスにあるメモリから、1バイトをゼロ拡張してレジスタrdに書き込む
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">ロードする対象のアドレスのベースが格納されているレジスタ番号</param>
        /// <param name="offset">オフセット</param>
        public bool Lbu(Register rd, Register rs1, Int32 offset, UInt32 insLength = 4U) {
            UInt32 addr = (UInt32)(reg.GetValue(rs1) + offset);
            if (reg.Mem.CanOperate(addr, 1)) {
                byte[] bytes = new byte[4];
                try {
                    bytes[0] = reg.Mem[addr];
                    bytes[1] = 0;
                    bytes[2] = 0;
                    bytes[3] = 0;
                    reg.SetValue(rd, BitConverter.ToUInt32(bytes, 0));
                    reg.IncrementPc(insLength);
                } catch (RiscvException e)
           when (((RiscvExceptionCause)e.Data["cause"]) == RiscvExceptionCause.LoadPageFault) {
                }
            }
            return true;
        }

        /// <summary>
        /// Load Harf word命令
        /// レジスタrs1+offsetのアドレスにあるメモリから、2バイトを符号拡張してレジスタrdに書き込む
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">ロードする対象のアドレスのベースが格納されているレジスタ番号</param>
        /// <param name="offset">オフセット</param>
        public bool Lh(Register rd, Register rs1, Int32 offset, UInt32 insLength = 4U) {
            UInt32 addr = (UInt32)(reg.GetValue(rs1) + offset);
            if (reg.Mem.CanOperate(addr, 2)) {
                byte[] bytes = new byte[4];
                try {
                    bytes[0] = reg.Mem[addr + 0];
                    bytes[1] = reg.Mem[addr + 1];
                    bytes[2] = (byte)((bytes[1] & 128U) > 1U ? 0xffU : 0U);
                    bytes[3] = (byte)((bytes[1] & 128U) > 1U ? 0xffU : 0U);
                    reg.SetValue(rd, BitConverter.ToUInt32(bytes, 0));
                    reg.IncrementPc(insLength);
                } catch (RiscvException e)
           when (((RiscvExceptionCause)e.Data["cause"]) == RiscvExceptionCause.LoadPageFault) {
                }
            }
            return true;
        }

        /// <summary>
        /// Load Harf word Unsigned命令
        /// レジスタrs1+offsetのアドレスにあるメモリから、2バイトをゼロ拡張してレジスタrdに書き込む
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">ロードする対象のアドレスのベースが格納されているレジスタ番号</param>
        /// <param name="offset">オフセット</param>
        public bool Lhu(Register rd, Register rs1, Int32 offset, UInt32 insLength = 4U) {
            UInt32 addr = (UInt32)(reg.GetValue(rs1) + offset);
            if (reg.Mem.CanOperate(addr, 2)) {
                byte[] bytes = new byte[4];
                try {
                    bytes[0] = reg.Mem[addr + 0];
                    bytes[1] = reg.Mem[addr + 1];
                    bytes[2] = 0;
                    bytes[3] = 0;
                    reg.SetValue(rd, BitConverter.ToUInt32(bytes, 0));
                    reg.IncrementPc(insLength);
                } catch (RiscvException e)
               when (((RiscvExceptionCause)e.Data["cause"]) == RiscvExceptionCause.LoadPageFault) {
                }
            }
            return true;
        }

        /// <summary>
        /// Load Word命令
        /// レジスタrs1+offsetのアドレスにあるメモリから、4バイトをレジスタrdに書き込む
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">ロードする対象のベースアドレスが格納されているレジスタ番号</param>
        /// <param name="offset">オフセット</param>
        public bool Lw(Register rd, Register rs1, Int32 offset, UInt32 insLength = 4U) {
            UInt32 addr = (UInt32)(reg.GetValue(rs1) + offset);
            if (reg.Mem.CanOperate(addr, 4)) {
                byte[] bytes = new byte[4];
                try {
                    bytes[0] = reg.Mem[addr + 0];
                    bytes[1] = reg.Mem[addr + 1];
                    bytes[2] = reg.Mem[addr + 2];
                    bytes[3] = reg.Mem[addr + 3];
                    reg.SetValue(rd, BitConverter.ToUInt32(bytes, 0));
                    reg.IncrementPc(insLength);
                } catch (RiscvException e)
                when (((RiscvExceptionCause)e.Data["cause"]) == RiscvExceptionCause.LoadPageFault) {
                }
            }
            return true;
        }

        #endregion

        #region Risc-V CPU Store系命令

        /// <summary>
        /// Store Byte命令
        /// レジスタrs2の下位1バイトをレジスタrs1+offsetのアドレスのメモリに格納する
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">ストアする対象のアドレスのベースが格納されているレジスタ番号</param>
        /// <param name="offset">オフセット</param>
        public bool Sb(Register rs1, Register rs2, Int32 offset, UInt32 insLength = 4U) {
            UInt32 addr = (UInt32)(reg.GetValue(rs1) + offset);
            if (reg.Mem.CanOperate(addr, 1)) {
                byte[] bytes = BitConverter.GetBytes(reg.GetValue(rs2));
                try {
                    reg.Mem[addr + 0] = bytes[0];
                    reg.IncrementPc(insLength);
                } catch (RiscvException e)
                 when (((RiscvExceptionCause)e.Data["cause"]) == RiscvExceptionCause.StoreAMOPageFault) {
                }
            }
            return true;
        }

        /// <summary>
        /// Store Harf word命令
        /// レジスタrs2の下位2バイトをレジスタrs1+offsetのアドレスのメモリに格納する
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">ストアする対象のアドレスのベースが格納されているレジスタ番号</param>
        /// <param name="offset">オフセット</param>
        public bool Sh(Register rs1, Register rs2, Int32 offset, UInt32 insLength = 4U) {
            UInt32 addr = (UInt32)(reg.GetValue(rs1) + offset);
            if (reg.Mem.CanOperate(addr, 2)) {
                byte[] bytes = BitConverter.GetBytes(reg.GetValue(rs2));
                try {
                    reg.Mem[addr + 0] = bytes[0];
                    reg.Mem[addr + 1] = bytes[1];
                    reg.IncrementPc(insLength);
                } catch (RiscvException e)
                 when (((RiscvExceptionCause)e.Data["cause"]) == RiscvExceptionCause.StoreAMOPageFault) {
                }
            }
            return true;
        }

        /// <summary>
        /// Store Word命令
        /// レジスタrs2の4バイトをレジスタrs1+offsetのアドレスのメモリに格納する
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">ストアする対象のアドレスのベースが格納されているレジスタ番号</param>
        /// <param name="offset">オフセット</param>
        public bool Sw(Register rs1, Register rs2, Int32 offset, UInt32 insLength = 4U) {
            UInt32 addr = (UInt32)(reg.GetValue(rs1) + offset);
            if (reg.Mem.CanOperate(addr, 4)) {
                byte[] bytes = BitConverter.GetBytes(reg.GetValue(rs2));
                try {
                    reg.Mem[addr + 0] = bytes[0];
                    reg.Mem[addr + 1] = bytes[1];
                    reg.Mem[addr + 2] = bytes[2];
                    reg.Mem[addr + 3] = bytes[3];
                    reg.IncrementPc(insLength);
                } catch (RiscvException e)
                   when (((RiscvExceptionCause)e.Data["cause"]) == RiscvExceptionCause.StoreAMOPageFault) {
                }
            }
            return true;
        }

        #endregion


        #endregion
    }
}
