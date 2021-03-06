﻿using RV32_Register;
using RV32_Register.Constants;
using RV32_Register.Exceptions;
using RV32_Register.MemoryHandler;
using System;

namespace RV32_Lsu {
    /// <summary>
    /// Risc-V RV32A 拡張命令セット アトミック(不可分)なメモリ操作に対応したロードストアユニット
    /// </summary>
    public class RV32_AmoLsu : RV32_IntegerLsu {


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
        public bool LrW(Register rd, Register rs1, bool acquire, bool release, UInt32 insLength = 4U) {
            UInt32 addr = reg.GetValue(rs1);
            if (addr % 4 != 0) {
                throw new RiscvException(RiscvExceptionCause.AMOAddressMisaligned, addr, reg);
            }
            if (reg.Mem.CanOperate(addr, 4)) {
                reg.Mem.Acquire(addr, 4);
                byte[] bytes = new byte[4];
                try {
                    bytes[0] = reg.Mem[addr + 0];
                    bytes[1] = reg.Mem[addr + 1];
                    bytes[2] = reg.Mem[addr + 2];
                    bytes[3] = reg.Mem[addr + 3];
                    reg.SetValue(rd, BitConverter.ToUInt32(bytes, 0));
                } catch (RiscvException e)
                   when (((RiscvExceptionCause)e.Data["cause"]) == RiscvExceptionCause.LoadPageFault) {
                    reg.Mem.Release(addr, 4);
                }
            }
            reg.IncrementPc(insLength);
            return true;
        }

        /// <summary>
        /// Store Conditional Word命令
        /// 
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">レジスタ番号</param>
        /// <param name="rs2">レジスタ番号</param>
        public bool ScW(Register rd, Register rs1, Register rs2, bool acquire, bool release, UInt32 insLength = 4U) {
            UInt32 addr = reg.GetValue(rs1);
            if (addr % 4 != 0) {
                throw new RiscvException(RiscvExceptionCause.AMOAddressMisaligned, addr, reg);
            }
            if (!reg.Mem.CanOperate(addr, 4)) {
                byte[] bytes = BitConverter.GetBytes(reg.GetValue(rs2));
                try {
                    reg.Mem[addr + 0] = bytes[0];
                    reg.Mem[addr + 1] = bytes[1];
                    reg.Mem[addr + 2] = bytes[2];
                    reg.Mem[addr + 3] = bytes[3];
                } catch (RiscvException e)
                   when (((RiscvExceptionCause)e.Data["cause"]) == RiscvExceptionCause.StoreAMOPageFault) {
                }
                reg.Mem.Release(addr, 4);
                reg.SetValue(rd, 0);
            } else {
                reg.SetValue(rd, 1);
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
        public bool AmoSwapW(Register rd, Register rs1, Register rs2, bool acquire, bool release, UInt32 insLength = 4U) {
            UInt32 addr = reg.GetValue(rs1);
            if (addr % 4 != 0) {
                throw new RiscvException(RiscvExceptionCause.AMOAddressMisaligned, addr, reg);
            }

            if (acquire) {
                reg.Mem.Acquire(addr, 4);
            }

            byte[] bytes = new byte[4];
            try {
                bytes[0] = reg.Mem[addr + 0];
                bytes[1] = reg.Mem[addr + 1];
                bytes[2] = reg.Mem[addr + 2];
                bytes[3] = reg.Mem[addr + 3];
                reg.SetValue(rd, BitConverter.ToUInt32(bytes, 0));
            } catch (RiscvException e)
               when (((RiscvExceptionCause)e.Data["cause"]) == RiscvExceptionCause.LoadPageFault) {
                if (acquire) {
                    reg.Mem.Release(addr, 4);
                }
            }

            UInt32 val = BitConverter.ToUInt32(bytes, 0);
            bytes = BitConverter.GetBytes(reg.GetValue(rs2));

            try {
                reg.Mem[addr + 0] = bytes[0];
                reg.Mem[addr + 1] = bytes[1];
                reg.Mem[addr + 2] = bytes[2];
                reg.Mem[addr + 3] = bytes[3];
            } catch (RiscvException e)
               when (((RiscvExceptionCause)e.Data["cause"]) == RiscvExceptionCause.StoreAMOPageFault) {
                if (acquire) {
                    reg.Mem.Release(addr, 4);
                }
            }

            if (release) {
                reg.Mem.Release(addr, 4);
            }

            reg.SetValue(rd, val);
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
        public bool AmoAddW(Register rd, Register rs1, Register rs2, bool acquire, bool release, UInt32 insLength = 4U) {
            UInt32 addr = reg.GetValue(rs1);
            if (addr % 4 != 0) {
                throw new RiscvException(RiscvExceptionCause.AMOAddressMisaligned, addr, reg);
            }

            if (acquire) {
                reg.Mem.Acquire(addr, 4);
            }

            byte[] bytes = new byte[4];
            try {
                bytes[0] = reg.Mem[addr + 0];
                bytes[1] = reg.Mem[addr + 1];
                bytes[2] = reg.Mem[addr + 2];
                bytes[3] = reg.Mem[addr + 3];
                reg.SetValue(rd, BitConverter.ToUInt32(bytes, 0));
            } catch (RiscvException e)
               when (((RiscvExceptionCause)e.Data["cause"]) == RiscvExceptionCause.LoadPageFault) {
                if (acquire) {
                    reg.Mem.Release(addr, 4);
                }
            }

            Int32 val = BitConverter.ToInt32(bytes, 0);
            Int32 result = val + (Int32)reg.GetValue(rs2);

            bytes = BitConverter.GetBytes(result);

            try {
                reg.Mem[addr + 0] = bytes[0];
                reg.Mem[addr + 1] = bytes[1];
                reg.Mem[addr + 2] = bytes[2];
                reg.Mem[addr + 3] = bytes[3];
            } catch (RiscvException e)
               when (((RiscvExceptionCause)e.Data["cause"]) == RiscvExceptionCause.StoreAMOPageFault) {
                if (acquire) {
                    reg.Mem.Release(addr, 4);
                }
            }

            if (release) {
                reg.Mem.Release(addr, 4);
            }

            reg.SetValue(rd, (UInt32)val);
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
        public bool AmoXorW(Register rd, Register rs1, Register rs2, bool acquire, bool release, UInt32 insLength = 4U) {
            UInt32 addr = reg.GetValue(rs1);
            if (addr % 4 != 0) {
                throw new RiscvException(RiscvExceptionCause.AMOAddressMisaligned, addr, reg);
            }

            if (acquire) {
                reg.Mem.Acquire(addr, 4);
            }

            byte[] bytes = new byte[4];
            try {
                bytes[0] = reg.Mem[addr + 0];
                bytes[1] = reg.Mem[addr + 1];
                bytes[2] = reg.Mem[addr + 2];
                bytes[3] = reg.Mem[addr + 3];
                reg.SetValue(rd, BitConverter.ToUInt32(bytes, 0));
            } catch (RiscvException e)
               when (((RiscvExceptionCause)e.Data["cause"]) == RiscvExceptionCause.LoadPageFault) {
                if (acquire) {
                    reg.Mem.Release(addr, 4);
                }
            }

            UInt32 val = BitConverter.ToUInt32(bytes, 0);
            UInt32 result = val ^ reg.GetValue(rs2);

            bytes = BitConverter.GetBytes(result);

            try {
                reg.Mem[addr + 0] = bytes[0];
                reg.Mem[addr + 1] = bytes[1];
                reg.Mem[addr + 2] = bytes[2];
                reg.Mem[addr + 3] = bytes[3];
            } catch (RiscvException e)
               when (((RiscvExceptionCause)e.Data["cause"]) == RiscvExceptionCause.StoreAMOPageFault) {
                if (acquire) {
                    reg.Mem.Release(addr, 4);
                }
            }

            if (release) {
                reg.Mem.Release(addr, 4);
            }

            reg.SetValue(rd, val);
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
        public bool AmoAndW(Register rd, Register rs1, Register rs2, bool acquire, bool release, UInt32 insLength = 4U) {
            UInt32 addr = reg.GetValue(rs1);
            if (addr % 4 != 0) {
                throw new RiscvException(RiscvExceptionCause.AMOAddressMisaligned, addr, reg);
            }

            if (acquire) {
                reg.Mem.Acquire(addr, 4);
            }

            byte[] bytes = new byte[4];
            try {
                bytes[0] = reg.Mem[addr + 0];
                bytes[1] = reg.Mem[addr + 1];
                bytes[2] = reg.Mem[addr + 2];
                bytes[3] = reg.Mem[addr + 3];
                reg.SetValue(rd, BitConverter.ToUInt32(bytes, 0));
            } catch (RiscvException e)
               when (((RiscvExceptionCause)e.Data["cause"]) == RiscvExceptionCause.LoadPageFault) {
                if (acquire) {
                    reg.Mem.Release(addr, 4);
                }
            }

            UInt32 val = BitConverter.ToUInt32(bytes, 0);
            UInt32 result = val & reg.GetValue(rs2);

            bytes = BitConverter.GetBytes(result);

            try {
                reg.Mem[addr + 0] = bytes[0];
                reg.Mem[addr + 1] = bytes[1];
                reg.Mem[addr + 2] = bytes[2];
                reg.Mem[addr + 3] = bytes[3];
            } catch (RiscvException e)
               when (((RiscvExceptionCause)e.Data["cause"]) == RiscvExceptionCause.StoreAMOPageFault) {
                if (acquire) {
                    reg.Mem.Release(addr, 4);
                }
            }

            if (release) {
                reg.Mem.Release(addr, 4);
            }

            reg.SetValue(rd, val);
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
        public bool AmoOrW(Register rd, Register rs1, Register rs2, bool acquire, bool release, UInt32 insLength = 4U) {
            UInt32 addr = reg.GetValue(rs1);
            if (addr % 4 != 0) {
                throw new RiscvException(RiscvExceptionCause.AMOAddressMisaligned, addr, reg);
            }

            if (acquire) {
                reg.Mem.Acquire(addr, 4);
            }

            byte[] bytes = new byte[4];
            try {
                bytes[0] = reg.Mem[addr + 0];
                bytes[1] = reg.Mem[addr + 1];
                bytes[2] = reg.Mem[addr + 2];
                bytes[3] = reg.Mem[addr + 3];
                reg.SetValue(rd, BitConverter.ToUInt32(bytes, 0));
            } catch (RiscvException e)
               when (((RiscvExceptionCause)e.Data["cause"]) == RiscvExceptionCause.LoadPageFault) {
                if (acquire) {
                    reg.Mem.Release(addr, 4);
                }
            }

            UInt32 val = BitConverter.ToUInt32(bytes, 0);
            UInt32 result = val | reg.GetValue(rs2);

            bytes = BitConverter.GetBytes(result);

            try {
                reg.Mem[addr + 0] = bytes[0];
                reg.Mem[addr + 1] = bytes[1];
                reg.Mem[addr + 2] = bytes[2];
                reg.Mem[addr + 3] = bytes[3];
            } catch (RiscvException e)
               when (((RiscvExceptionCause)e.Data["cause"]) == RiscvExceptionCause.StoreAMOPageFault) {
                if (acquire) {
                    reg.Mem.Release(addr, 4);
                }
            }

            if (release) {
                reg.Mem.Release(addr, 4);
            }

            reg.SetValue(rd, val);
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
        public bool AmoMinW(Register rd, Register rs1, Register rs2, bool acquire, bool release, UInt32 insLength = 4U) {
            UInt32 addr = reg.GetValue(rs1);
            if (addr % 4 != 0) {
                throw new RiscvException(RiscvExceptionCause.AMOAddressMisaligned, addr, reg);
            }

            if (acquire) {
                reg.Mem.Acquire(addr, 4);
            }

            byte[] bytes = new byte[4];
            try {
                bytes[0] = reg.Mem[addr + 0];
                bytes[1] = reg.Mem[addr + 1];
                bytes[2] = reg.Mem[addr + 2];
                bytes[3] = reg.Mem[addr + 3];
                reg.SetValue(rd, BitConverter.ToUInt32(bytes, 0));
            } catch (RiscvException e)
               when (((RiscvExceptionCause)e.Data["cause"]) == RiscvExceptionCause.LoadPageFault) {
                if (acquire) {
                    reg.Mem.Release(addr, 4);
                }
            }

            Int32 val = BitConverter.ToInt32(bytes, 0);
            Int32 val2 = (Int32)reg.GetValue(rs2);
            UInt32 result = (UInt32)(val < val2 ? val : val2);

            bytes = BitConverter.GetBytes(result);

            try {
                reg.Mem[addr + 0] = bytes[0];
                reg.Mem[addr + 1] = bytes[1];
                reg.Mem[addr + 2] = bytes[2];
                reg.Mem[addr + 3] = bytes[3];
            } catch (RiscvException e)
               when (((RiscvExceptionCause)e.Data["cause"]) == RiscvExceptionCause.StoreAMOPageFault) {
                if (acquire) {
                    reg.Mem.Release(addr, 4);
                }
            }

            if (release) {
                reg.Mem.Release(addr, 4);
            }

            reg.SetValue(rd, (UInt32)val);
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
        public bool AmoMaxW(Register rd, Register rs1, Register rs2, bool acquire, bool release, UInt32 insLength = 4U) {
            UInt32 addr = reg.GetValue(rs1);
            if (addr % 4 != 0) {
                throw new RiscvException(RiscvExceptionCause.AMOAddressMisaligned, addr, reg);
            }

            if (acquire) {
                reg.Mem.Acquire(addr, 4);
            }

            byte[] bytes = new byte[4];
            try {
                bytes[0] = reg.Mem[addr + 0];
                bytes[1] = reg.Mem[addr + 1];
                bytes[2] = reg.Mem[addr + 2];
                bytes[3] = reg.Mem[addr + 3];
                reg.SetValue(rd, BitConverter.ToUInt32(bytes, 0));
            } catch (RiscvException e)
               when (((RiscvExceptionCause)e.Data["cause"]) == RiscvExceptionCause.LoadPageFault) {
                if (acquire) {
                    reg.Mem.Release(addr, 4);
                }
            }

            Int32 val = BitConverter.ToInt32(bytes, 0);
            Int32 val2 = (Int32)reg.GetValue(rs2);
            UInt32 result = (UInt32)(val > val2 ? val : val2);

            bytes = BitConverter.GetBytes(result);

            try {
                reg.Mem[addr + 0] = bytes[0];
                reg.Mem[addr + 1] = bytes[1];
                reg.Mem[addr + 2] = bytes[2];
                reg.Mem[addr + 3] = bytes[3];
            } catch (RiscvException e)
               when (((RiscvExceptionCause)e.Data["cause"]) == RiscvExceptionCause.StoreAMOPageFault) {
                if (acquire) {
                    reg.Mem.Release(addr, 4);
                }
            }

            if (release) {
                reg.Mem.Release(addr, 4);
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
        public bool AmoMinuW(Register rd, Register rs1, Register rs2, bool acquire, bool release, UInt32 insLength = 4U) {
            UInt32 addr = reg.GetValue(rs1);
            if (addr % 4 != 0) {
                throw new RiscvException(RiscvExceptionCause.AMOAddressMisaligned, addr, reg);
            }

            if (acquire) {
                reg.Mem.Acquire(addr, 4);
            }

            byte[] bytes = new byte[4];
            try {
                bytes[0] = reg.Mem[addr + 0];
                bytes[1] = reg.Mem[addr + 1];
                bytes[2] = reg.Mem[addr + 2];
                bytes[3] = reg.Mem[addr + 3];
                reg.SetValue(rd, BitConverter.ToUInt32(bytes, 0));
            } catch (RiscvException e)
               when (((RiscvExceptionCause)e.Data["cause"]) == RiscvExceptionCause.LoadPageFault) {
                if (acquire) {
                    reg.Mem.Release(addr, 4);
                }
            }

            UInt32 val = BitConverter.ToUInt32(bytes, 0);
            UInt32 val2 = reg.GetValue(rs2);
            UInt32 result = val < val2 ? val : val2;

            bytes = BitConverter.GetBytes(result);

            try {
                reg.Mem[addr + 0] = bytes[0];
                reg.Mem[addr + 1] = bytes[1];
                reg.Mem[addr + 2] = bytes[2];
                reg.Mem[addr + 3] = bytes[3];
            } catch (RiscvException e)
               when (((RiscvExceptionCause)e.Data["cause"]) == RiscvExceptionCause.StoreAMOPageFault) {
                if (acquire) {
                    reg.Mem.Release(addr, 4);
                }
            }

            if (release) {
                reg.Mem.Release(addr, 4);
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
        public bool AmoMaxuW(Register rd, Register rs1, Register rs2, bool acquire, bool release, UInt32 insLength = 4U) {
            UInt32 addr = reg.GetValue(rs1);
            if (addr % 4 != 0) {
                throw new RiscvException(RiscvExceptionCause.AMOAddressMisaligned, addr, reg);
            }

            if (acquire) {
                reg.Mem.Acquire(addr, 4);
            }

            byte[] bytes = new byte[4];
            try {
                bytes[0] = reg.Mem[addr + 0];
                bytes[1] = reg.Mem[addr + 1];
                bytes[2] = reg.Mem[addr + 2];
                bytes[3] = reg.Mem[addr + 3];
                reg.SetValue(rd, BitConverter.ToUInt32(bytes, 0));
            } catch (RiscvException e)
               when (((RiscvExceptionCause)e.Data["cause"]) == RiscvExceptionCause.LoadPageFault) {
                if (acquire) {
                    reg.Mem.Release(addr, 4);
                }
            }

            UInt32 val = BitConverter.ToUInt32(bytes, 0);
            UInt32 val2 = reg.GetValue(rs2);
            UInt32 result = val > val2 ? val : val2;

            bytes = BitConverter.GetBytes(result);

            try {
                reg.Mem[addr + 0] = bytes[0];
                reg.Mem[addr + 1] = bytes[1];
                reg.Mem[addr + 2] = bytes[2];
                reg.Mem[addr + 3] = bytes[3];
            } catch (RiscvException e)
               when (((RiscvExceptionCause)e.Data["cause"]) == RiscvExceptionCause.StoreAMOPageFault) {
                if (acquire) {
                    reg.Mem.Release(addr, 4);
                }
            }

            if (release) {
                reg.Mem.Release(addr, 4);
            }

            reg.SetValue(rd, val);
            reg.IncrementPc(insLength);
            return true;
        }

        #endregion

        #endregion
    }
}
