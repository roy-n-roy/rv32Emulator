using RiscVCpu.Exceptions;
using RiscVCpu.LoadStoreUnit;
using System;

namespace RiscVCpu.ArithmeticLogicUnit {

    /// <summary>
    /// Risc-V RV32I 基本命令セット 算術論理演算命令を実行するALU
    /// </summary>
    public class RV32_Alu : RV32_AbstractCalculator {
        /// <summary>
        /// Risc-V 算術論理ALU
        /// </summary>
        /// <param name="registerSet">入出力用レジスタ</param>

        public RV32_Alu(RV32_RegisterSet reg) : base(reg) {
        }

        #region Risc-V CPU命令

        #region Risv-V CPU 即値付き算術論理演算命令

        /// <summary>
        /// Add Immediate命令
        /// レジスタrs1(符号付き)と即値の和をレジスタrdに書き込む
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">被加数が格納されているレジスタ番号</param>
        /// <param name="immediate">加数(即値)</param>
        /// <returns>処理の成否</returns>
        public bool Addi(Register rd, Register rs1, Int32 immediate) {
            reg.SetValue(rd, reg.GetValue(rs1) + (UInt32)immediate);
            reg.IncrementPc();
            return true;
        }

        /// <summary>
        /// Exclusive-OR Immediate命令
        /// レジスタrs1と即値の排他的論理和をレジスタrdに書き込む
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">レジスタ番号</param>
        /// <param name="immediate">即値</param>
        /// <returns>処理の成否</returns>
        public bool Xori(Register rd, Register rs1, Int32 immediate) {
            reg.SetValue(rd, reg.GetValue(rs1) ^ (UInt32)immediate);
            reg.IncrementPc();
            return true;
        }

        /// <summary>
        /// OR Immediate命令
        /// レジスタrs1と即値の論理和をレジスタrdに書き込む
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">レジスタ番号</param>
        /// <param name="immediate">即値</param>
        /// <returns>処理の成否</returns>
        public bool Ori(Register rd, Register rs1, Int32 immediate) {
            reg.SetValue(rd, reg.GetValue(rs1) | (UInt32)immediate);
            reg.IncrementPc();
            return true;
        }

        /// <summary>
        /// And, Immediate命令
        /// レジスタrs1と即値の論理積をレジスタrdに書き込む
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">レジスタ番号</param>
        /// <param name="immediate">即値</param>
        /// <returns>処理の成否</returns>
        public bool Andi(Register rd, Register rs1, Int32 immediate) {
            reg.SetValue(rd, reg.GetValue(rs1) & (UInt32)immediate);
            reg.IncrementPc();
            return true;
        }

        /// <summary>
        /// Set if Then Immediate命令
        /// レジスタrs1(符号付き)＜即値(符号付き)が成り立つ場合はレジスタrdに1を、そうでない場合は0をレジスタrdに書き込む
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">レジスタ番号</param>
        /// <param name="immediate">即値</param>
        /// <returns>処理の成否</returns>
        public bool Slti(Register rd, Register rs1, Int32 immediate) {
            reg.SetValue(rd, ((Int32)reg.GetValue(rs1) < immediate) ? 1U : 0U);
            reg.IncrementPc();
            return true;
        }

        /// <summary>
        /// Set if Less Then Immediate, Unsigned命令
        /// レジスタrs1(符号なし)＜即値(符号なし)が成り立つ場合はレジスタrdに1を、そうでない場合は0をレジスタrdに書き込む
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">レジスタ番号</param>
        /// <param name="immediate">即値</param>
        /// <returns>処理の成否</returns>
        public bool Sltiu(Register rd, Register rs1, Int32 immediate) {
            reg.SetValue(rd, (reg.GetValue(rs1) < (UInt32)immediate) ? 1U : 0U);
            reg.IncrementPc();
            return true;
        }

        /// <summary>
        /// Shift Left Logical Immediate命令
        /// レジスタrs1を即値ビット左シフトし、レジスタrdに書き込む
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">レジスタ番号</param>
        /// <param name="immediate">即値</param>
        /// <returns>処理の成否</returns>
        public bool Slli(Register rd, Register rs1, Int32 immediate) {
            if ((immediate >> 5) != 0) throw new RiscvCpuException("不正命令例外");
            reg.SetValue(rd, reg.GetValue(rs1) << immediate);
            reg.IncrementPc();
            return true;
        }

        /// <summary>
        /// Shift Right Logical Immediate命令
        /// レジスタrs1を即値ビット右シフト、ゼロ拡張してレジスタrdに書き込む
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">レジスタ番号</param>
        /// <param name="immediate">即値</param>
        /// <returns>処理の成否</returns>
        public bool Srli(Register rd, Register rs1, Int32 immediate) {
            if ((immediate >> 5) != 0) throw new RiscvCpuException("不正命令例外");
            reg.SetValue(rd, reg.GetValue(rs1) >> immediate);
            reg.IncrementPc();
            return true;
        }
        /// <summary>
        /// Shift Right Arithmetic Immediate命令
        /// レジスタrs1を即値ビット右シフト、符号拡張してレジスタrdに書き込む
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1"></param>
        /// <param name="immediate"></param>
        /// <returns>処理の成否</returns>
        public bool Srai(Register rd, Register rs1, Int32 immediate) {
            if ((immediate >> 5) != 0) throw new RiscvCpuException("不正命令例外");
            reg.SetValue(rd, (UInt32)((Int32)reg.GetValue(rs1) >> immediate));
            reg.IncrementPc();
            return true;
        }

        #endregion

        #region Risv-V CPU 算術論理演算命令

        /// <summary>
        /// Add命令
        /// レジスタrs1(符号付き)とrs2(符号付き)の和をレジスタrdに書き込む
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">レジスタ番号</param>
        /// <param name="rs2">レジスタ番号</param>
        /// <returns>処理の成否</returns>
        public bool Add(Register rd, Register rs1, Register rs2) {
            reg.SetValue(rd, reg.GetValue(rs1) + reg.GetValue(rs2));
            reg.IncrementPc();
            return true;
        }

        /// <summary>
        /// Substract命令
        /// レジスタrs1(符号付き)とrs2(符号付き)の差をレジスタrdに書き込む
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">レジスタ番号</param>
        /// <param name="rs2">レジスタ番号</param>
        /// <returns>処理の成否</returns>
        public bool Sub(Register rd, Register rs1, Register rs2) {
            reg.SetValue(rd, reg.GetValue(rs1) - reg.GetValue(rs2));
            reg.IncrementPc();
            return true;
        }

        /// <summary>
        /// Exclusive-OR命令
        /// レジスタrs1とrs2の排他的論理和をレジスタrdに書き込む
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">レジスタ番号</param>
        /// <param name="rs2">レジスタ番号</param>
        /// <returns>処理の成否</returns>
        public bool Xor(Register rd, Register rs1, Register rs2) {
            reg.SetValue(rd, reg.GetValue(rs1) ^ reg.GetValue(rs2));
            reg.IncrementPc();
            return true;
        }

        /// <summary>
        /// OR命令
        /// レジスタrs1とrs2の論理和をレジスタrdに書き込む
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">レジスタ番号</param>
        /// <param name="rs2">レジスタ番号</param>
        /// <returns>処理の成否</returns>
        public bool Or(Register rd, Register rs1, Register rs2) {
            reg.SetValue(rd, reg.GetValue(rs1) | reg.GetValue(rs2));
            reg.IncrementPc();
            return true;
        }

        /// <summary>
        /// AND命令
        /// レジスタrs1とrs2の論理積をレジスタrdに書き込む
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">レジスタ番号</param>
        /// <param name="rs2">レジスタ番号</param>
        /// <returns>処理の成否</returns>
        public bool And(Register rd, Register rs1, Register rs2) {
            reg.SetValue(rd, reg.GetValue(rs1) & reg.GetValue(rs2));
            reg.IncrementPc();
            return true;
        }

        /// <summary>
        /// Set if Less Then命令
        /// レジスタrs1(符号付き)＜レジスタrs2(符号付き)が成り立つ場合はレジスタrdに1を、そうでない場合は0をレジスタrdに書き込む
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">レジスタ番号</param>
        /// <param name="rs2">レジスタ番号</param>
        /// <returns>処理の成否</returns>
        public bool Slt(Register rd, Register rs1, Register rs2) {
            reg.SetValue(rd, (Int32)reg.GetValue(rs1) < (Int32)reg.GetValue(rs2) ? 1U : 0U);
            reg.IncrementPc();
            return true;
        }

        /// <summary>
        /// Set if Less Then, Unsigned命令
        /// レジスタrs1(符号なし)＜レジスタrs2(符号なし)が成り立つ場合はレジスタrdに1を、そうでない場合は0をレジスタrdに書き込む
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">レジスタ番号</param>
        /// <param name="rs2">レジスタ番号</param>
        /// <returns>処理の成否</returns>
        public bool Sltu(Register rd, Register rs1, Register rs2) {
            reg.SetValue(rd, (reg.GetValue(rs1) < reg.GetValue(rs2)) ? 1U : 0U);
            reg.IncrementPc();
            return true;
        }

        /// <summary>
        /// Shift Left Logical命令
        /// レジスタrs1をレジスタrs2の最下位5bitの桁数左シフトし、レジスタrdに格納する
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">レジスタ番号</param>
        /// <param name="rs2">レジスタ番号</param>
        /// <returns>処理の成否</returns>
        public bool Sll(Register rd, Register rs1, Register rs2) {
            reg.SetValue(rd, reg.GetValue(rs1) << (Int32)reg.GetValue(rs2) & 0x1F);
            reg.IncrementPc();
            return true;
        }

        /// <summary>
        /// Shift Right Logical命令
        /// レジスタrs1をレジスタrs2の最下位5bitの桁数右シフト、ゼロ拡張してレジスタrdに格納する
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">レジスタ番号</param>
        /// <param name="rs2">レジスタ番号</param>
        /// <returns>処理の成否</returns>
        public bool Srl(Register rd, Register rs1, Register rs2) {
            reg.SetValue(rd, reg.GetValue(rs1) >> (Int32)reg.GetValue(rs2) & 0x1F);
            reg.IncrementPc();
            return true;
        }

        /// <summary>
        /// Shift Right Arithmetic命令
        /// レジスタrs1をレジスタrs2の最下位5bitの桁数右シフト、符号拡張してレジスタrdに格納する
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">レジスタ番号</param>
        /// <param name="rs2">レジスタ番号</param>
        /// <returns>処理の成否</returns>
        public bool Sra(Register rd, Register rs1, Register rs2) {
            reg.SetValue(rd, (UInt32)((Int32)reg.GetValue(rs1) >> (Int32)reg.GetValue(rs2) & 0x1F));
            reg.IncrementPc();
            return true;
        }
        #endregion

        #endregion
    }
}
