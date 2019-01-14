using RV32_Lsu.Constants;
using RV32_Lsu.Exceptions;
using RV32_Lsu.MemoryHandler;
using RV32_Lsu.RegisterSet;
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

        /// <summary>
        /// Jump And Link命令
        /// 次の命令アドレス(pc+4)をレジスタrdに書き込み、現在のpcにoffsetを加えてpcに設定する
        /// </summary>
        /// <param name="rd">次の命令アドレスを格納するレジスタ番号</param>
        /// <param name="offset">ジャンプする現在のpcからの相対アドレス位置</param>
        public bool Jal(Register rd, Int32 offset, UInt32 insLength = 4u) {
            reg.SetValue(rd, reg.PC + insLength);
            reg.PC += (UInt32)offset;
            return true;
        }

        /// <summary>
        /// Jump And Link Register命令
        /// レジスタrs1+offsetをpcに書き込み、
        /// 次の命令アドレスだった値(pc+4)をレジスタrdに書き込む
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">ベースのアドレス</param>
        /// <param name="offset">ジャンプするオフセット値</param>
        public bool Jalr(Register rd, Register rs1, Int32 offset, UInt32 insLength = 4u) {
            UInt32 t = reg.PC + insLength;
            reg.PC = (reg.GetValue(rs1) + (UInt32)offset) & ~1u;
            reg.SetValue(rd, t);
            return true;
        }

        #region Risv-V CPU Branch系命令

        /// <summary>
        /// Branch if Equal命令
        /// レジスタrs1の値とrs2の値が同じ場合、pcにoffsetを加える
        /// </summary>
        /// <param name="rs1">被比較数</param>
        /// <param name="rs2">比較数</param>
        /// <param name="offset">ジャンプするオフセット値</param>
        public bool Beq(Register rs1, Register rs2, Int32 offset, UInt32 insLength = 4u) {
            if (reg.GetValue(rs1) == reg.GetValue(rs2)) {
                reg.PC += (UInt32)offset;
            } else {
                reg.IncrementPc(insLength);
            }
            return true;
        }

        /// <summary>
        /// Branch if Not Equal命令
        /// レジスタrs1の値とrs2の値が異なる場合、pcにoffsetを加える
        /// </summary>
        /// <param name="rs1">被比較数</param>
        /// <param name="rs2">比較数</param>
        /// <param name="offset">ジャンプするオフセット値</param>
        public bool Bne(Register rs1, Register rs2, Int32 offset, UInt32 insLength = 4u) {
            if (reg.GetValue(rs1) != reg.GetValue(rs2)) {
                reg.PC += (UInt32)offset;
            } else {
                reg.IncrementPc(insLength);
            }
            return true;
        }

        /// <summary>
        /// Branch if Less Then命令
        /// レジスタrs1の値がrs2の値より小さい場合、pcにoffsetを加える
        /// </summary>
        /// <param name="rs1">被比較数</param>
        /// <param name="rs2">比較数</param>
        /// <param name="offset">ジャンプするオフセット値</param>
        public bool Blt(Register rs1, Register rs2, Int32 offset, UInt32 insLength = 4u) {
            if ((Int32)reg.GetValue(rs1) < (Int32)reg.GetValue(rs2)) {
                reg.PC += (UInt32)offset;
            } else {
                reg.IncrementPc(insLength);
            }
            return true;
        }

        /// <summary>
        /// Branch if Greater Then or Equal命令
        /// レジスタrs1の値がrs2の値以上の場合、pcにoffsetを加える
        /// </summary>
        /// <param name="rs1">被比較数</param>
        /// <param name="rs2">比較数</param>
        /// <param name="offset">ジャンプするオフセット値</param>
        public bool Bge(Register rs1, Register rs2, Int32 offset, UInt32 insLength = 4u) {
            if ((Int32)reg.GetValue(rs1) >= (Int32)reg.GetValue(rs2)) {
                reg.PC += (UInt32)offset;
            } else {
                reg.IncrementPc(insLength);
            }
            return true;
        }

        /// <summary>
        /// Branch if Less Then, Unsigned命令
        /// レジスタrs1の値がrs2の値より小さい場合、pcにoffsetを加える
        /// </summary>
        /// <param name="rs1">被比較数</param>
        /// <param name="rs2">比較数</param>
        /// <param name="offset">ジャンプするオフセット値</param>
        public bool Bltu(Register rs1, Register rs2, Int32 offset, UInt32 insLength = 4u) {
            if (reg.GetValue(rs1) < reg.GetValue(rs2)) {
                reg.PC += (UInt32)offset;
            } else {
                reg.IncrementPc(insLength);
            }
            return true;
        }

        /// <summary>
        /// Branch if Greater Then or Equal命令
        /// レジスタrs1の値がrs2の値以上の場合、pcにoffsetを加える
        /// </summary>
        /// <param name="rs1">被比較数</param>
        /// <param name="rs2">比較数</param>
        /// <param name="offset">ジャンプするオフセット値</param>
        public bool Bgeu(Register rs1, Register rs2, Int32 offset, UInt32 insLength = 4u) {
            if (reg.GetValue(rs1) >= reg.GetValue(rs2)) {
                reg.PC += (UInt32)offset;
            } else {
                reg.IncrementPc(insLength);
            }
            return true;
        }

        #endregion

        #region Risc-V CPU Load系命令

        /// <summary>
        /// Load Byte命令
        /// レジスタrs1+offsetのアドレスにあるメモリから、1バイトを符号拡張してレジスタrdに書き込む
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">ロードする対象のアドレスのベースが格納されているレジスタ番号</param>
        /// <param name="offset">オフセット</param>
        public bool Lb(Register rd, Register rs1, Int32 offset, UInt32 insLength = 4u) {
            UInt32 addr = (UInt32)(reg.GetValue(rs1) + offset);
            if (reg.Mem.CanOperate(addr, 1)) {
                byte[] bytes = new byte[4];
                try {
                    bytes[0] = reg.Mem[addr];
                    bytes[1] = (byte)((bytes[0] & 0x80u) > 1u ? 0xffu : 0u);
                    bytes[2] = (byte)((bytes[0] & 0x80u) > 1u ? 0xffu : 0u);
                    bytes[3] = (byte)((bytes[0] & 0x80u) > 1u ? 0xffu : 0u);
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
        public bool Lbu(Register rd, Register rs1, Int32 offset, UInt32 insLength = 4u) {
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
        public bool Lh(Register rd, Register rs1, Int32 offset, UInt32 insLength = 4u) {
            UInt32 addr = (UInt32)(reg.GetValue(rs1) + offset);
            if (reg.Mem.CanOperate(addr, 2)) {
                byte[] bytes = new byte[4];
                try {
                    bytes[0] = reg.Mem[addr + 0];
                    bytes[1] = reg.Mem[addr + 1];
                    bytes[2] = (byte)((bytes[1] & 128u) > 1u ? 0xffu : 0u);
                    bytes[3] = (byte)((bytes[1] & 128u) > 1u ? 0xffu : 0u);
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
        public bool Lhu(Register rd, Register rs1, Int32 offset, UInt32 insLength = 4u) {
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
        public bool Lw(Register rd, Register rs1, Int32 offset, UInt32 insLength = 4u) {
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
        public bool Sb(Register rs1, Register rs2, Int32 offset, UInt32 insLength = 4u) {
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
        public bool Sh(Register rs1, Register rs2, Int32 offset, UInt32 insLength = 4u) {
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
        public bool Sw(Register rs1, Register rs2, Int32 offset, UInt32 insLength = 4u) {
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
