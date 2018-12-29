using RiscVCpu.LoadStoreUnit;
using RiscVCpu.LoadStoreUnit.Constants;
using RiscVCpu.LoadStoreUnit.Exceptions;
using System;
using System.Collections.Generic;

namespace RiscVCpu.LoadStoreUnit {
    /// <summary>
    /// Risc-V RV32A 拡張命令セット アトミック(不可分)なメモリ操作に対応したロードストアユニット
    /// </summary>
    public class RV32_AmoLsu : RV32_Lsu {

        private readonly HashSet<UInt64> ReservedAddress;
        
        /// <summary>
        /// Risc-V Atomic Memory Operator
        /// </summary>
        /// <param name="registerSet">入出力用レジスタ</param>
        public RV32_AmoLsu(RV32_RegisterSet reg, byte[] mainMemory) : base(reg, mainMemory) {
            ReservedAddress = new HashSet<UInt64>();
        }

        #region Risc-V CPU命令

        #region Risv-V CPU アトミックなメモリ操作命令

        /// <summary>
        /// Load Reserved Word命令
        /// 
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">レジスタ番号</param>
        /// <param name="rs2">レジスタ番号</param>
        public bool LrW(Register rd, Register rs1, bool acquire, bool release, UInt32 insLength = 4u) {
            UInt32 addr = reg.GetValue(rs1);
            if (!ReservedAddress.Contains(addr)) {
                ReservedAddress.Add(addr);
                base.Lw(rd, rs1, 0);
            } else {
                reg.IncrementPc(insLength);
            }
            return true;
        }

        /// <summary>
        /// Store Conditional Word命令
        /// 
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">レジスタ番号</param>
        /// <param name="rs2">レジスタ番号</param>
        public bool ScW(Register rd, Register rs1, Register rs2, bool acquire, bool release, UInt32 insLength = 4u) {
            UInt32 addr = reg.GetValue(rs1);
            if (ReservedAddress.Contains(addr)) {
                base.Sw(rs1, rs2, 0);
                ReservedAddress.Remove(addr);
                reg.SetValue(rd, 0);
            } else {
                reg.IncrementPc(insLength);
                reg.SetValue(rd, 1);
            }
            return true;
        }

        /// <summary>
        /// Atomic Memory Operation: Swap Word命令
        /// レジスタrs1のアドレスにあるメモリから4byteをレジスタrdに書き込み
        /// レジスタrs2をレジスタrs1のアドレスにあるメモリに格納する
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">レジスタ番号</param>
        /// <param name="rs2">レジスタ番号</param>
        public bool AmoSwapW(Register rd, Register rs1, Register rs2, bool acquire, bool release, UInt32 insLength = 4u) {
            UInt32 addr = reg.GetValue(rs1);

            if (addr % 4 != 0) {
                throw new RiscvException(RiscvExceptionCause.AMOAddressMisaligned, reg);
            }

            if (acquire) {
                ReservedAddress.Add(addr);
            }
            byte[] bytes = new byte[4];
            bytes[0] = mem[addr + 0];
            bytes[1] = mem[addr + 1];
            bytes[2] = mem[addr + 2];
            bytes[3] = mem[addr + 3];

            UInt32 val = BitConverter.ToUInt32(bytes, 0);
            bytes = BitConverter.GetBytes(reg.GetValue(rs2));
            reg.SetValue(rd, val);

            mem[addr + 0] = bytes[0];
            mem[addr + 1] = bytes[1];
            mem[addr + 2] = bytes[2];
            mem[addr + 3] = bytes[3];

            if (release) {
                ReservedAddress.Remove(addr);
            }
            reg.IncrementPc(insLength);
            return true;
        }

        /// <summary>
        /// Atomic Memory Operation: Add Word命令
        /// 
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">レジスタ番号</param>
        /// <param name="rs2">レジスタ番号</param>
        public bool AmoAddW(Register rd, Register rs1, Register rs2, bool acquire, bool release, UInt32 insLength = 4u) {
            UInt32 addr = reg.GetValue(rs1);

            if (addr % 4 != 0) {
                throw new RiscvException(RiscvExceptionCause.AMOAddressMisaligned, reg);
            }

            if (acquire) {
                ReservedAddress.Add(addr);
            }
            byte[] bytes = new byte[4];
            bytes[0] = mem[addr + 0];
            bytes[1] = mem[addr + 1];
            bytes[2] = mem[addr + 2];
            bytes[3] = mem[addr + 3];

            Int32 val = BitConverter.ToInt32(bytes, 0);
            bytes = BitConverter.GetBytes(val + (Int32)reg.GetValue(rs2));
            reg.SetValue(rd, (UInt32)val);

            mem[addr + 0] = bytes[0];
            mem[addr + 1] = bytes[1];
            mem[addr + 2] = bytes[2];
            mem[addr + 3] = bytes[3];

            if (release) {
                ReservedAddress.Remove(addr);
            }
            reg.IncrementPc(insLength);
            return true;
        }

        /// <summary>
        /// Atomic Memory Operation: Exclusive-OR Word命令
        /// レジスタrs1のアドレスにあるメモリから4byteをレジスタrdに書き込み
        /// レジスタrs2をレジスタrs1のアドレスにあるメモリに格納する
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">レジスタ番号</param>
        /// <param name="rs2">レジスタ番号</param>
        public bool AmoXorW(Register rd, Register rs1, Register rs2, bool acquire, bool release, UInt32 insLength = 4u) {
            UInt32 addr = reg.GetValue(rs1);

            if (addr % 4 != 0) {
                throw new RiscvException(RiscvExceptionCause.AMOAddressMisaligned, reg);
            }

            if (acquire) {
                ReservedAddress.Add(addr);
            }
            byte[] bytes = new byte[4];
            bytes[0] = mem[addr + 0];
            bytes[1] = mem[addr + 1];
            bytes[2] = mem[addr + 2];
            bytes[3] = mem[addr + 3];

            UInt32 val = BitConverter.ToUInt32(bytes, 0);
            bytes = BitConverter.GetBytes(val ^ reg.GetValue(rs2));
            reg.SetValue(rd, val);

            mem[addr + 0] = bytes[0];
            mem[addr + 1] = bytes[1];
            mem[addr + 2] = bytes[2];
            mem[addr + 3] = bytes[3];

            if (release) {
                ReservedAddress.Remove(addr);
            }
            reg.IncrementPc(insLength);
            return true;
        }

        /// <summary>
        /// Atomic Memory Operation: Swap Word命令
        /// レジスタrs1のアドレスにあるメモリから4byteをレジスタrdに書き込み
        /// レジスタrs2をレジスタrs1のアドレスにあるメモリに格納する
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">レジスタ番号</param>
        /// <param name="rs2">レジスタ番号</param>
        public bool AmoAndW(Register rd, Register rs1, Register rs2, bool acquire, bool release, UInt32 insLength = 4u) {
            UInt32 addr = reg.GetValue(rs1);

            if (addr % 4 != 0) {
                throw new RiscvException(RiscvExceptionCause.AMOAddressMisaligned, reg);
            }

            if (acquire) {
                ReservedAddress.Add(addr);
            }
            byte[] bytes = new byte[4];
            bytes[0] = mem[addr + 0];
            bytes[1] = mem[addr + 1];
            bytes[2] = mem[addr + 2];
            bytes[3] = mem[addr + 3];

            UInt32 val = BitConverter.ToUInt32(bytes, 0);
            bytes = BitConverter.GetBytes(val & reg.GetValue(rs2));
            reg.SetValue(rd, val);

            mem[addr + 0] = bytes[0];
            mem[addr + 1] = bytes[1];
            mem[addr + 2] = bytes[2];
            mem[addr + 3] = bytes[3];

            if (release) {
                ReservedAddress.Remove(addr);
            }
            reg.IncrementPc(insLength);
            return true;
        }

        /// <summary>
        /// Atomic Memory Operation: Swap Word命令
        /// レジスタrs1のアドレスにあるメモリから4byteをレジスタrdに書き込み
        /// レジスタrs2をレジスタrs1のアドレスにあるメモリに格納する
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">レジスタ番号</param>
        /// <param name="rs2">レジスタ番号</param>
        public bool AmoOrW(Register rd, Register rs1, Register rs2, bool acquire, bool release, UInt32 insLength = 4u) {
            UInt32 addr = reg.GetValue(rs1);

            if (addr % 4 != 0) {
                throw new RiscvException(RiscvExceptionCause.AMOAddressMisaligned, reg);
            }

            if (acquire) {
                ReservedAddress.Add(addr);
            }
            byte[] bytes = new byte[4];
            bytes[0] = mem[addr + 0];
            bytes[1] = mem[addr + 1];
            bytes[2] = mem[addr + 2];
            bytes[3] = mem[addr + 3];

            UInt32 val = BitConverter.ToUInt32(bytes, 0);
            bytes = BitConverter.GetBytes(val | reg.GetValue(rs2));
            reg.SetValue(rd, val);

            mem[addr + 0] = bytes[0];
            mem[addr + 1] = bytes[1];
            mem[addr + 2] = bytes[2];
            mem[addr + 3] = bytes[3];

            if (release) {
                ReservedAddress.Remove(addr);
            }
            reg.IncrementPc(insLength);
            return true;
        }

        /// <summary>
        /// Atomic Memory Operation: Swap Word命令
        /// レジスタrs1のアドレスにあるメモリから4byteをレジスタrdに書き込み
        /// レジスタrs2をレジスタrs1のアドレスにあるメモリに格納する
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">レジスタ番号</param>
        /// <param name="rs2">レジスタ番号</param>
        public bool AmoMinW(Register rd, Register rs1, Register rs2, bool acquire, bool release, UInt32 insLength = 4u) {
            UInt32 addr = reg.GetValue(rs1);

            if (addr % 4 != 0) {
                throw new RiscvException(RiscvExceptionCause.AMOAddressMisaligned, reg);
            }

            if (acquire) {
                ReservedAddress.Add(addr);
            }
            byte[] bytes = new byte[4];
            bytes[0] = mem[addr + 0];
            bytes[1] = mem[addr + 1];
            bytes[2] = mem[addr + 2];
            bytes[3] = mem[addr + 3];

            UInt32 val = BitConverter.ToUInt32(bytes, 0);
            bytes = BitConverter.GetBytes((Int32)val < (Int32)reg.GetValue(rs2) ? val : reg.GetValue(rs2));
            reg.SetValue(rd, val);

            mem[addr + 0] = bytes[0];
            mem[addr + 1] = bytes[1];
            mem[addr + 2] = bytes[2];
            mem[addr + 3] = bytes[3];

            if (release) {
                ReservedAddress.Remove(addr);
            }
            reg.IncrementPc(insLength);
            return true;
        }

        /// <summary>
        /// Atomic Memory Operation: Swap Word命令
        /// レジスタrs1のアドレスにあるメモリから4byteをレジスタrdに書き込み
        /// レジスタrs2をレジスタrs1のアドレスにあるメモリに格納する
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">レジスタ番号</param>
        /// <param name="rs2">レジスタ番号</param>
        public bool AmoMaxW(Register rd, Register rs1, Register rs2, bool acquire, bool release, UInt32 insLength = 4u) {
            UInt32 addr = reg.GetValue(rs1);

            if (addr % 4 != 0) {
                throw new RiscvException(RiscvExceptionCause.AMOAddressMisaligned, reg);
            }

            if (acquire) {
                ReservedAddress.Add(addr);
            }
            byte[] bytes = new byte[4];
            bytes[0] = mem[addr + 0];
            bytes[1] = mem[addr + 1];
            bytes[2] = mem[addr + 2];
            bytes[3] = mem[addr + 3];

            UInt32 val = BitConverter.ToUInt32(bytes, 0);
            bytes = BitConverter.GetBytes((Int32)val > (Int32)reg.GetValue(rs2) ? val : reg.GetValue(rs2));
            reg.SetValue(rd, val);

            mem[addr + 0] = bytes[0];
            mem[addr + 1] = bytes[1];
            mem[addr + 2] = bytes[2];
            mem[addr + 3] = bytes[3];

            if (release) {
                ReservedAddress.Remove(addr);
            }
            reg.IncrementPc(insLength);
            return true;
        }

        /// <summary>
        /// Atomic Memory Operation: Swap Word命令
        /// レジスタrs1のアドレスにあるメモリから4byteをレジスタrdに書き込み
        /// レジスタrs2をレジスタrs1のアドレスにあるメモリに格納する
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">レジスタ番号</param>
        /// <param name="rs2">レジスタ番号</param>
        public bool AmoMinuW(Register rd, Register rs1, Register rs2, bool acquire, bool release, UInt32 insLength = 4u) {
            UInt32 addr = reg.GetValue(rs1);

            if (addr % 4 != 0) {
                throw new RiscvException(RiscvExceptionCause.AMOAddressMisaligned, reg);
            }

            if (acquire) {
                ReservedAddress.Add(addr);
            }
            byte[] bytes = new byte[4];
            bytes[0] = mem[addr + 0];
            bytes[1] = mem[addr + 1];
            bytes[2] = mem[addr + 2];
            bytes[3] = mem[addr + 3];

            UInt32 val = BitConverter.ToUInt32(bytes, 0);
            bytes = BitConverter.GetBytes(val < reg.GetValue(rs2) ? val : reg.GetValue(rs2));
            reg.SetValue(rd, val);

            mem[addr + 0] = bytes[0];
            mem[addr + 1] = bytes[1];
            mem[addr + 2] = bytes[2];
            mem[addr + 3] = bytes[3];

            if (release) {
                ReservedAddress.Remove(addr);
            }
            reg.IncrementPc(insLength);
            return true;
        }

        /// <summary>
        /// Atomic Memory Operation: Swap Word命令
        /// レジスタrs1のアドレスにあるメモリから4byteをレジスタrdに書き込み
        /// レジスタrs2をレジスタrs1のアドレスにあるメモリに格納する
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">レジスタ番号</param>
        /// <param name="rs2">レジスタ番号</param>
        public bool AmoMaxuW(Register rd, Register rs1, Register rs2, bool acquire, bool release, UInt32 insLength = 4u) {
            UInt32 addr = reg.GetValue(rs1);

            if (addr % 4 != 0) {
                throw new RiscvException(RiscvExceptionCause.AMOAddressMisaligned, reg);
            }

            if (acquire) {
                ReservedAddress.Add(addr);
            }
            byte[] bytes = new byte[4];
            bytes[0] = mem[addr + 0];
            bytes[1] = mem[addr + 1];
            bytes[2] = mem[addr + 2];
            bytes[3] = mem[addr + 3];

            UInt32 val = BitConverter.ToUInt32(bytes, 0);
            bytes = BitConverter.GetBytes(val > reg.GetValue(rs2) ? val : reg.GetValue(rs2));
            reg.SetValue(rd, val);

            mem[addr + 0] = bytes[0];
            mem[addr + 1] = bytes[1];
            mem[addr + 2] = bytes[2];
            mem[addr + 3] = bytes[3];

            if (release) {
                ReservedAddress.Remove(addr);
            }
            reg.IncrementPc(insLength);
            return true;
        }




        #region Risc-V CPU Load系命令

        /// <summary>
        /// Load Byte命令
        /// レジスタrs1+offsetのアドレスにあるメモリから、1バイトを符号拡張してレジスタrdに書き込む
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">ロードする対象のアドレスのベースが格納されているレジスタ番号</param>
        /// <param name="offset">オフセット</param>
        public override bool Lb(Register rd, Register rs1, Int32 offset, UInt32 insLength = 4u) {
            UInt64 addr = (UInt64)(reg.GetValue(rs1) + offset);
            if (!ReservedAddress.Contains(addr)) {
                base.Lb(rd, rs1, offset);
            } else {
                reg.IncrementPc(insLength);
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
        public override bool Lbu(Register rd, Register rs1, Int32 offset, UInt32 insLength = 4u) {
            UInt64 addr = (UInt64)(reg.GetValue(rs1) + offset);
            if (!ReservedAddress.Contains(addr)) {
                base.Lbu(rd, rs1, offset);
            } else {
                reg.IncrementPc(insLength);
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
        public override bool Lh(Register rd, Register rs1, Int32 offset, UInt32 insLength = 4u) {
            UInt64 addr = (UInt64)(reg.GetValue(rs1) + offset);
            if (!ReservedAddress.Contains(addr)) {
                base.Lh(rd, rs1, offset);
            } else {
                reg.IncrementPc(insLength);
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
        public override bool Lhu(Register rd, Register rs1, Int32 offset, UInt32 insLength = 4u) {
            UInt64 addr = (UInt64)(reg.GetValue(rs1) + offset);
            if (!ReservedAddress.Contains(addr)) {
                base.Lhu(rd, rs1, offset);
            } else {
                reg.IncrementPc(insLength);
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
        public override bool Lw(Register rd, Register rs1, Int32 offset, UInt32 insLength = 4u) {
            UInt64 addr = (UInt64)(reg.GetValue(rs1) + offset);
            if (!ReservedAddress.Contains(addr)) {
                base.Lw(rd, rs1, offset);
            } else {
                reg.IncrementPc(insLength);
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
        public override bool Sb(Register rs1, Register rs2, Int32 offset, UInt32 insLength = 4u) {
            UInt64 addr = (UInt64)(reg.GetValue(rs1) + offset);
            if (!ReservedAddress.Contains(addr)) {
                base.Sb(rs1, rs2, offset);
            } else {
                reg.IncrementPc(insLength);
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
        public override bool Sh(Register rs1, Register rs2, Int32 offset, UInt32 insLength = 4u) {
            UInt64 addr = (UInt64)(reg.GetValue(rs1) + offset);
            if (!ReservedAddress.Contains(addr)) {
                base.Sh(rs1, rs2, offset);
            } else {
                reg.IncrementPc(insLength);
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
        public override bool Sw(Register rs1, Register rs2, Int32 offset, UInt32 insLength = 4u) {
            UInt64 addr = (UInt64)(reg.GetValue(rs1) + offset);
            if (!ReservedAddress.Contains(addr)) {
                base.Sw(rs1, rs2, offset);
            } else {
                reg.IncrementPc(insLength);
            }
            return true;
        }

        #endregion

        #endregion

        #endregion
    }
}
