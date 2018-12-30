using RiscVCpu.LoadStoreUnit;
using RiscVCpu.LoadStoreUnit.Constants;
using RiscVCpu.RegisterSet;
using System;

namespace RiscVCpu.ArithmeticLogicUnit {
    /// <summary>
    /// Risc-V RV32M 拡張命令セット 乗除算命令を実行するALU
    /// </summary>
    public class RV32_Mac : RV32_AbstractCalculator {

        /// <summary>
        /// Risc-V 乗除算ALU
        /// </summary>
        /// <param name="registerSet">入出力用レジスタ</param>
        public RV32_Mac(RV32_RegisterSet reg) : base(reg) {
        }

        #region Risc-V CPU命令

        #region Risv-V CPU 算術論理演算命令

        /// <summary>
        /// Multiply命令
        /// レジスタrs1とrs2の積をレジスタrdに書き込む
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">レジスタ番号</param>
        /// <param name="rs2">レジスタ番号</param>
        public bool Mul(Register rd, Register rs1, Register rs2, UInt32 insLength = 4u) {
            reg.SetValue(rd, reg.GetValue(rs1) * reg.GetValue(rs2));
            reg.IncrementPc(insLength);
            return true;
        }

        /// <summary>
        /// Multiply High命令
        /// レジスタrs1(符号付き)とrs2(符号付き)の積の上位半分のbitをレジスタrdに書き込む
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">レジスタ番号</param>
        /// <param name="rs2">レジスタ番号</param>
        public bool Mulh(Register rd, Register rs1, Register rs2, UInt32 insLength = 4u) {
            reg.SetValue(rd, (UInt32)(((Int64)(Int32)reg.GetValue(rs1) * (Int64)(Int32)reg.GetValue(rs2)) >> 32));
            reg.IncrementPc(insLength);
            return true;
        }

        /// <summary>
        /// Multiply High, Signed-Unsigned命令
        /// レジスタrs1(符号付き)とrs2(符号なし)の積の上位半分のbitをレジスタrdに書き込む
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">レジスタ番号</param>
        /// <param name="rs2">レジスタ番号</param>
        public bool Mulhsu(Register rd, Register rs1, Register rs2, UInt32 insLength = 4u) {
            bool minus = (Int32)reg.GetValue(rs2) < 0;
            reg.SetValue(rd, (UInt32)(((Int64)(Int32)reg.GetValue(rs1) * (Int64)(UInt64)reg.GetValue(rs2)) >> 32));
            reg.IncrementPc(insLength);
            return true;
        }

        /// <summary>
        /// Multiply High, Unsigned命令
        /// レジスタrs1(符号なし)とrs2(符号なし)の積の上位半分のbitを結果として、レジスタrdに書き込む
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">レジスタ番号</param>
        /// <param name="rs2">レジスタ番号</param>
        public bool Mulhu(Register rd, Register rs1, Register rs2, UInt32 insLength = 4u) {
            reg.SetValue(rd, (UInt32)(((UInt64)reg.GetValue(rs1) * (UInt64)reg.GetValue(rs2)) >> 32));
            reg.IncrementPc(insLength);
            return true;
        }

        /// <summary>
        /// Divide命令
        /// レジスタrs1(符号付き)とrs2(符号付き)の商をレジスタrdに書き込む
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">レジスタ番号</param>
        /// <param name="rs2">レジスタ番号</param>
        public bool Div(Register rd, Register rs1, Register rs2, UInt32 insLength = 4u) {
            if (reg.GetValue(rs2) == 0U) {
                reg.SetValue(rd, 0xffffffffu);
            } else {
                bool minus = (Int32)reg.GetValue(rs2) < 0;
                reg.SetValue(rd, (UInt32)(((Int32)reg.GetValue(rs1) * (minus ? -1L : 1L)) / ((Int32)reg.GetValue(rs2)) * (minus ? -1L : 1L)));
            }
            reg.IncrementPc(insLength);
            return true;
        }

        /// <summary>
        /// Divide, Unsigned命令
        /// レジスタrs1(符号なし)とrs2(符号なし)の商をレジスタrdに書き込む
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">レジスタ番号</param>
        /// <param name="rs2">レジスタ番号</param>
        public bool Divu(Register rd, Register rs1, Register rs2, UInt32 insLength = 4u) {
            if (reg.GetValue(rs2) == 0U) {
                reg.SetValue(rd, 0xffffffffu);
            } else {
                reg.SetValue(rd, reg.GetValue(rs1) / reg.GetValue(rs2));
            }
            reg.IncrementPc(insLength);
            return true;
        }

        /// <summary>
        /// Reminder命令
        /// レジスタrs1(符号付き)とrs2(符号付き)の剰余をレジスタrdに書き込む
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">レジスタ番号</param>
        /// <param name="rs2">レジスタ番号</param>
        public bool Rem(Register rd, Register rs1, Register rs2, UInt32 insLength = 4u) {
            if (reg.GetValue(rs2) == 0U) {
                reg.SetValue(rd, reg.GetValue(rs1));
            } else {
                bool minus = (Int32)reg.GetValue(rs2) < 0;
                reg.SetValue(rd, (UInt32)(((Int32)reg.GetValue(rs1) * (minus ? -1L : 1L)) % ((Int32)reg.GetValue(rs2)) * (minus ? -1L : 1L)));
            }
            reg.IncrementPc(insLength);
            return true;
        }

        /// <summary>
        /// Reminder, Unsigned命令
        /// レジスタrs1(符号なし)とrs2(符号なし)の剰余をレジスタrdに書き込む
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">レジスタ番号</param>
        /// <param name="rs2">レジスタ番号</param>
        public bool Remu(Register rd, Register rs1, Register rs2, UInt32 insLength = 4u) {
            if (reg.GetValue(rs2) == 0U) {
                reg.SetValue(rd, reg.GetValue(rs1));
            } else {
                reg.SetValue(rd, reg.GetValue(rs1) % (UInt32)reg.GetValue(rs2));
            }
            reg.IncrementPc(insLength);
            return true;
        }
        #endregion

        #endregion
    }
}
