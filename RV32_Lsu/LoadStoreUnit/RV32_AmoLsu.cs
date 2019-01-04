﻿using RiscVCpu.LoadStoreUnit.Constants;
using RiscVCpu.LoadStoreUnit.Exceptions;
using RiscVCpu.MemoryHandler;
using RiscVCpu.RegisterSet;
using System;

namespace RiscVCpu.LoadStoreUnit {
    /// <summary>
    /// Risc-V RV32A 拡張命令セット アトミック(不可分)なメモリ操作に対応したロードストアユニット
    /// </summary>
    public class RV32_AmoLsu : RV32_Lsu {


        /// <summary>
        /// Risc-V Atomic Memory Operate LSU
        /// </summary>
        /// <param name="registerSet">入出力用レジスタ</param>
        public RV32_AmoLsu(RV32_RegisterSet reg, RV32_AbstractMemoryHandler mainMemory) : base(reg, mainMemory) {
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
            if (mem.CanOperate(addr, 4)) {
                mem.Acquire(addr);
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
            if (mem.CanOperate(addr, 4)) {
                base.Sw(rs1, rs2, 0);
                mem.Release(addr);
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
                throw new RiscvException(RiscvExceptionCause.AMOAddressMisaligned, addr, reg);
            }

            if (acquire) {
                mem.Acquire(addr);
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
                mem.Release(addr);
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
                throw new RiscvException(RiscvExceptionCause.AMOAddressMisaligned, addr, reg);
            }

            if (acquire) {
                mem.Acquire(addr);
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
                mem.Release(addr);
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
                throw new RiscvException(RiscvExceptionCause.AMOAddressMisaligned, addr, reg);
            }

            if (acquire) {
                mem.Acquire(addr);
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
                mem.Release(addr);
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
                throw new RiscvException(RiscvExceptionCause.AMOAddressMisaligned, addr, reg);
            }

            if (acquire) {
                mem.Acquire(addr);
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
                mem.Release(addr);
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
                throw new RiscvException(RiscvExceptionCause.AMOAddressMisaligned, addr, reg);
            }

            if (acquire) {
                mem.Acquire(addr);
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
                mem.Release(addr);
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
                throw new RiscvException(RiscvExceptionCause.AMOAddressMisaligned, addr, reg);
            }

            if (acquire) {
                mem.Acquire(addr);
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
                mem.Release(addr);
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
                throw new RiscvException(RiscvExceptionCause.AMOAddressMisaligned, addr, reg);
            }

            if (acquire) {
                mem.Acquire(addr);
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
                mem.Release(addr);
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
                throw new RiscvException(RiscvExceptionCause.AMOAddressMisaligned, addr, reg);
            }

            if (acquire) {
                mem.Acquire(addr);
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
                mem.Release(addr);
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
                throw new RiscvException(RiscvExceptionCause.AMOAddressMisaligned, addr, reg);
            }

            if (acquire) {
                mem.Acquire(addr);
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
                mem.Release(addr);
            }
            reg.IncrementPc(insLength);
            return true;
        }

        #endregion

        #endregion
    }
}
