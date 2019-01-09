using RiscVCpu.LoadStoreUnit.Constants;
using RiscVCpu.RegisterSet;
using System;

namespace RiscVCpu.ArithmeticLogicUnit {

    /// <summary>32bit長バイナリ形式</summary>
    using Binary32 = UInt32;

    /// <summary>
    /// Risc-V RV32I 単精度浮動小数点命令セット 算術論理演算命令を実行するFPU
    /// </summary>
    public class RV32_SingleFpu : RV32_AbstractCalculator {

        internal const Binary32 NaN = 0x7fc0_0000u;
        internal const Binary32 Zero = 0x0000_0000u;
        internal const Binary32 Infinity = 0x7f80_0000u;

        internal const Binary32 NegativeSign = 0x8000_0000u;

        internal const Binary32 SignMask = 0x8000_0000u;
        internal const Binary32 ExpMask = 0x7f80_0000u;
        internal const Binary32 MantMask = 0x007f_ffffu;

        /// <summary>
        /// Risc-V 単精度浮動小数算術論理演算 FPU
        /// </summary>
        /// <param name="registerSet">入出力用レジスタ</param>
        public RV32_SingleFpu(RV32_RegisterSet reg) : base(reg) {
        }

        #region Risc-V CPU, Single-Precision命令

        #region Risv-V CPU 浮動小数点演算命令

        /// <summary>
        /// Floating-Point Fused Multiply-Add, Single-Precision命令
        /// 浮動小数点レジスタrs1とrs2の積にrs3を足した値を浮動小数点レジスタrdに書き込む
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">レジスタ番号</param>
        /// <param name="rs2">レジスタ番号</param>
        /// <param name="rs3">レジスタ番号</param>
        /// <returns>処理の成否</returns>
        public bool FmaddS(FPRegister rd, FPRegister rs1, FPRegister rs2, FPRegister rs3, FloatRoundingMode frm, UInt32 insLength = 4u) {
            Binary32 binary1 = (Binary32)reg.GetValue(rs1);
            Binary32 binary2 = (Binary32)reg.GetValue(rs2);
            Binary32 binary3 = (Binary32)reg.GetValue(rs3);
            Binary32 result;
            FloatCSR fcsr = 0;

            result = ToBinary((ToSingle(binary1) * ToSingle(binary2)) + ToSingle(binary3));
            result = IsNaN(result) ? NaN : result;

            if ((IsInfinity(binary1) || IsInfinity(binary2)) && IsInfinity(binary3) && 
                ((IsNegative(binary1) ^ IsPositive(binary2)) ^ IsNegative(binary3))) {
                // ∞ + -∞ もしくは -∞ + ∞の場合
                result = NaN;
                fcsr.NV = true;

            } else if (IsSigNaN(binary1) || IsSigNaN(binary2) || IsSigNaN(binary3)) {
                // いずれかの数値がシグナリングNaNの場合
                fcsr.NV = true;

            } else if (ToSingle(result) != (ToSingle(binary1) * ToSingle(binary2)) + ToSingle(binary3)) {
                // 結果が一致しない場合
                fcsr.NX = true;
            }

            reg.SetValue(rd, result);
            reg.SetFflagsCSR(fcsr);
            reg.IncrementPc(insLength);
            return true;
        }

        /// <summary>
        /// Floating-Point Fused Multiply-Subtract, Single-Precision命令
        /// 浮動小数点レジスタrs1とrs2の積からrs3を引いた値を浮動小数点レジスタrdに書き込む
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">レジスタ番号</param>
        /// <param name="rs2">レジスタ番号</param>
        /// <param name="rs3">レジスタ番号</param>
        /// <returns>処理の成否</returns>
        public bool FmsubS(FPRegister rd, FPRegister rs1, FPRegister rs2, FPRegister rs3, FloatRoundingMode frm, UInt32 insLength = 4u) {
            Binary32 binary1 = (Binary32)reg.GetValue(rs1);
            Binary32 binary2 = (Binary32)reg.GetValue(rs2);
            Binary32 binary3 = (Binary32)reg.GetValue(rs3);
            Binary32 result;
            FloatCSR fcsr = 0;

            result = ToBinary((ToSingle(binary1) * ToSingle(binary2)) - ToSingle(binary3));
            result = IsNaN(result) ? NaN : result;

            if ((IsInfinity(binary1) || IsInfinity(binary2)) && IsInfinity(binary3) &&
                ((IsNegative(binary1) ^ IsPositive(binary2)) ^ IsPositive(binary3))) {
                // ∞ + -∞ もしくは -∞ + ∞の場合
                result = NaN;
                fcsr.NV = true;

            } else if (IsSigNaN(binary1) || IsSigNaN(binary2) || IsSigNaN(binary3)) {
                // いずれかの数値がシグナリングNaNの場合
                fcsr.NV = true;

            } else if (ToSingle(result) != (ToSingle(binary1) * ToSingle(binary2)) - ToSingle(binary3)) {
                // 結果が一致しない場合
                fcsr.NX = true;
            }

            reg.SetValue(rd, result);
            reg.SetFflagsCSR(fcsr);
            reg.IncrementPc(insLength);
            return true;
        }

        /// <summary>
        /// Floating-Point Fused Negative Multiply-Add, Single-Precision命令
        /// 浮動小数点レジスタrs1に-1掛けた値とrs2の積にrs3を足した値を浮動小数点レジスタrdに書き込む
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">レジスタ番号</param>
        /// <param name="rs2">レジスタ番号</param>
        /// <param name="rs3">レジスタ番号</param>
        /// <returns>処理の成否</returns>
        public bool FnmaddS(FPRegister rd, FPRegister rs1, FPRegister rs2, FPRegister rs3, FloatRoundingMode frm, UInt32 insLength = 4u) {
            Binary32 binary1 = (Binary32)reg.GetValue(rs1);
            Binary32 binary2 = (Binary32)reg.GetValue(rs2);
            Binary32 binary3 = (Binary32)reg.GetValue(rs3);
            Binary32 result;
            FloatCSR fcsr = 0;

            result = ToBinary((-ToSingle(binary1) * ToSingle(binary2)) + ToSingle(binary3));
            result = IsNaN(result) ? NaN : result;

            if ((IsInfinity(binary1) || IsInfinity(binary2)) && IsInfinity(binary3) &&
                ((IsPositive(binary1) ^ IsPositive(binary2)) ^ IsNegative(binary3))) {
                // ∞ + -∞ もしくは -∞ + ∞の場合
                result = NaN;
                fcsr.NV = true;

            } else if (IsSigNaN(binary1) || IsSigNaN(binary2) || IsSigNaN(binary3)) {
                // いずれかの数値がシグナリングNaNの場合
                fcsr.NV = true;

            } else if (ToSingle(result) != (-ToSingle(binary1) * ToSingle(binary2)) + ToSingle(binary3)) {
                // 結果が一致しない場合
                fcsr.NX = true;
            }

            reg.SetValue(rd, result);
            reg.SetFflagsCSR(fcsr);
            reg.IncrementPc(insLength);
            return true;
        }

        /// <summary>
        /// Floating-Point Fused Negative Multiply-Subtract, Single-Precision命令
        /// 浮動小数点レジスタrs1に-1掛けた値とrs2の積からrs3を引いた値を浮動小数点レジスタrdに書き込む
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">レジスタ番号</param>
        /// <param name="rs2">レジスタ番号</param>
        /// <param name="rs3">レジスタ番号</param>
        /// <returns>処理の成否</returns>
        public bool FnmsubS(FPRegister rd, FPRegister rs1, FPRegister rs2, FPRegister rs3, FloatRoundingMode frm, UInt32 insLength = 4u) {
            Binary32 binary1 = (Binary32)reg.GetValue(rs1);
            Binary32 binary2 = (Binary32)reg.GetValue(rs2);
            Binary32 binary3 = (Binary32)reg.GetValue(rs3);
            Binary32 result;
            FloatCSR fcsr = 0;

            result = ToBinary((-ToSingle(binary1) * ToSingle(binary2)) - ToSingle(binary3));
            result = IsNaN(result) ? NaN : result;

            if ((IsInfinity(binary1) || IsInfinity(binary2)) && IsInfinity(binary3) &&
                ((IsPositive(binary1) ^ IsPositive(binary2)) ^ IsPositive(binary3))) {
                // ∞ + -∞ もしくは -∞ + ∞の場合
                result = NaN;
                fcsr.NV = true;

            } else if (IsSigNaN(binary1) || IsSigNaN(binary2) || IsSigNaN(binary3)) {
                // いずれかの数値がシグナリングNaNの場合
                fcsr.NV = true;

            } else if (ToSingle(result) != (-ToSingle(binary1) * ToSingle(binary2)) - ToSingle(binary3)) {
                // 結果が一致しない場合
                fcsr.NX = true;
            }

            reg.SetValue(rd, result);
            reg.SetFflagsCSR(fcsr);
            reg.IncrementPc(insLength);
            return true;
        }

        /// <summary>
        /// Floating-Point Add, Single-Precision命令
        /// 浮動小数点レジスタrs1とrs2の和を浮動小数点レジスタrdに書き込む
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">レジスタ番号</param>
        /// <param name="rs2">レジスタ番号</param>
        /// <returns>処理の成否</returns>
        public bool FaddS(FPRegister rd, FPRegister rs1, FPRegister rs2, FloatRoundingMode frm, UInt32 insLength = 4u) {
            Binary32 binary1 = (Binary32)reg.GetValue(rs1);
            Binary32 binary2 = (Binary32)reg.GetValue(rs2);
            Binary32 result;
            FloatCSR fcsr = 0;

            result = ToBinary(ToSingle(binary1) + ToSingle(binary2));
            result = IsNaN(result) ? NaN : result;

            if (IsInfinity(binary1) && IsInfinity(binary2) && (IsNegative(binary1) ^ IsNegative(binary2))) {
                // ∞ + -∞ もしくは -∞ + ∞の場合
                result = NaN;
                fcsr.NV = true;

            } else if (IsSigNaN(binary1) || IsSigNaN(binary2)) {
                // いずれかの数値がシグナリングNaNの場合
                fcsr.NV = true;

            } else if (ToSingle(result) != ToSingle(binary1) + ToSingle(binary2)) {
                // 結果が一致しない場合
                fcsr.NX = true;
            }

            reg.SetValue(rd, result);
            reg.SetFflagsCSR(fcsr);
            reg.IncrementPc(insLength);
            return true;
        }

        /// <summary>
        /// Floating-Point Subtract, Single-Precision命令
        /// 浮動小数点レジスタrs1とrs2の差を浮動小数点レジスタrdに書き込む
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">レジスタ番号</param>
        /// <param name="rs2">レジスタ番号</param>
        /// <returns>処理の成否</returns>
        public bool FsubS(FPRegister rd, FPRegister rs1, FPRegister rs2, FloatRoundingMode frm, UInt32 insLength = 4u) {
            Binary32 binary1 = (Binary32)reg.GetValue(rs1);
            Binary32 binary2 = (Binary32)reg.GetValue(rs2);
            Binary32 result;
            FloatCSR fcsr = 0;

            result = ToBinary(ToSingle(binary1) - ToSingle(binary2));
            result = IsNaN(result) ? NaN : result;

            if (IsInfinity(binary1) && IsInfinity(binary2) && (IsNegative(binary1) ^ IsPositive(binary2))) {
                // ∞ - ∞ もしくは -∞ - -∞の場合
                result = NaN;
                fcsr.NV = true;

            } else if (IsSigNaN(binary1) || IsSigNaN(binary2)) {
                // いずれかの数値がシグナリングNaNの場合
                fcsr.NV = true;

            } else if (ToSingle(result) != ToSingle(binary1) - ToSingle(binary2)) {
                // 結果が一致しない場合
                fcsr.NX = true;
            }

            reg.SetValue(rd, result);
            reg.SetFflagsCSR(fcsr);
            reg.IncrementPc(insLength);
            return true;
        }

        /// <summary>
        /// Floating-Point Multiply, Single-Precision命令
        /// 浮動小数点レジスタrs1とrs2の積を浮動小数点レジスタrdに書き込む
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">レジスタ番号</param>
        /// <param name="rs2">レジスタ番号</param>
        /// <returns>処理の成否</returns>
        public bool FmulS(FPRegister rd, FPRegister rs1, FPRegister rs2, FloatRoundingMode frm, UInt32 insLength = 4u) {
            Binary32 binary1 = (Binary32)reg.GetValue(rs1);
            Binary32 binary2 = (Binary32)reg.GetValue(rs2);
            Binary32 result;
            FloatCSR fcsr = 0;

            result = ToBinary(ToSingle(binary1) * ToSingle(binary2));
            result = IsNaN(result) ? NaN : result;

            if (IsSigNaN(binary1) || IsSigNaN(binary2)) {
                // いずれかの数値がシグナリングNaNの場合
                fcsr.NV = true;

            } else if (ToSingle(result) != ToSingle(binary1) * ToSingle(binary2)) {
                // 結果が一致しない場合
                fcsr.NX = true;
            }

            reg.SetValue(rd, result);
            reg.SetFflagsCSR(fcsr);
            reg.IncrementPc(insLength);
            return true;
        }

        /// <summary>
        /// Floating-Point Divide, Single-Precision命令
        /// 浮動小数点レジスタrs1とrs2の商を浮動小数点レジスタrdに書き込む
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">レジスタ番号</param>
        /// <param name="rs2">レジスタ番号</param>
        /// <returns>処理の成否</returns>
        public bool FdivS(FPRegister rd, FPRegister rs1, FPRegister rs2, FloatRoundingMode frm, UInt32 insLength = 4u) {
            Binary32 binary1 = (Binary32)reg.GetValue(rs1);
            Binary32 binary2 = (Binary32)reg.GetValue(rs2);
            Binary32 result;
            FloatCSR fcsr = 0;

            if (binary1 == 0f && binary2 == 0f) {
                // 被除数、除数ともに0の場合
                result = NaN;
                fcsr.DZ = true;

            } else if (IsZero(binary2)) {
                // ゼロ除算の場合
                result = Infinity | ((binary1 & SignMask) ^ (binary2 & SignMask));
                fcsr.DZ = true;

            } else {
                result = ToBinary(ToSingle(binary1) / ToSingle(binary2));
                result = IsNaN(result) ? NaN : result;

                if (IsSigNaN(binary1) || IsSigNaN(binary2)) {
                    // いずれかの数値がシグナリングNaNの場合
                    fcsr.NV = true;

                } else if (ToSingle(result) != ToSingle(binary1) / ToSingle(binary2)) {
                    // 結果が一致しない場合
                    fcsr.NX = true;
                }
            }

            reg.SetValue(rd, result);
            reg.SetFflagsCSR(fcsr);
            reg.IncrementPc(insLength);
            return true;
        }

        /// <summary>
        /// Floating-Point Square Root, Single-Precision命令
        /// 浮動小数点レジスタrs1の平方根を浮動小数点レジスタrdに書き込む
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">レジスタ番号</param>
        /// <param name="rs2">レジスタ番号</param>
        /// <returns>処理の成否</returns>
        public bool FsqrtS(FPRegister rd, FPRegister rs1, FloatRoundingMode frm, UInt32 insLength = 4u) {
            Binary32 binary1 = (Binary32)reg.GetValue(rs1);
            Binary32 result;
            FloatCSR fcsr = 0;

            result = ToBinary((Single)Math.Sqrt(ToSingle(binary1)));
            result = IsNaN(result) ? NaN : result;

            if (IsSigNaN(binary1)) {
                // いずれかの数値がシグナリングNaNの場合
                fcsr.NV = true;

            } else if (IsNegative(binary1)) {
                // 負数の平方根を求めようとした場合
                fcsr.NV = true;

            } else if (ToSingle(result) * ToSingle(result) != ToSingle(binary1)) {
                // 結果が一致しない場合
                fcsr.NX = true;
            }

            reg.SetValue(rd, result);
            reg.SetFflagsCSR(fcsr);
            reg.IncrementPc(insLength);
            return true;
        }

        /// <summary>
        /// Floating-Point Sign Inject, Single-Precision命令
        /// 浮動小数点レジスタrs1の指数と仮数と、rs2の符号を組み立てて
        /// 浮動小数点レジスタrdに書き込む
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">レジスタ番号</param>
        /// <param name="rs2">レジスタ番号</param>
        /// <returns>処理の成否</returns>
        public bool FsgnjS(FPRegister rd, FPRegister rs1, FPRegister rs2, UInt32 insLength = 4u) {
            Binary32 binary1 = (Binary32)reg.GetValue(rs1);
            Binary32 binary2 = (Binary32)reg.GetValue(rs2);
            Binary32 result;

            result = binary1 & 0x7fff_ffffu | binary2 & 0x8000_0000u;

            reg.SetValue(rd, result);
            reg.IncrementPc(insLength);
            return true;
        }

        /// <summary>
        /// Floating-Point Sign Inject, Single-Precision命令
        /// 浮動小数点レジスタrs1の指数と仮数と、rs2の符号反転したものを組み立てて
        /// 浮動小数点レジスタrdに書き込む
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">レジスタ番号</param>
        /// <param name="rs2">レジスタ番号</param>
        /// <returns>処理の成否</returns>
        public bool FsgnjnS(FPRegister rd, FPRegister rs1, FPRegister rs2, UInt32 insLength = 4u) {
            Binary32 binary1 = (Binary32)reg.GetValue(rs1);
            Binary32 binary2 = (Binary32)reg.GetValue(rs2);
            Binary32 result;
            FloatCSR fcsr = 0;

            result = binary1 & 0x7fff_ffffu | ~binary2 & 0x8000_0000u;

            reg.SetValue(rd, result);
            reg.SetFflagsCSR(fcsr);
            reg.IncrementPc(insLength);
            return true;
        }

        /// <summary>
        /// Floating-Point Sign Inject, Single-Precision命令
        /// 浮動小数点レジスタrs1の指数と仮数と、rs1の符号とrs2の符号の排他的論理和を組み立てて
        /// 浮動小数点レジスタrdに書き込む
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">レジスタ番号</param>
        /// <param name="rs2">レジスタ番号</param>
        /// <returns>処理の成否</returns>
        public bool FsgnjxS(FPRegister rd, FPRegister rs1, FPRegister rs2, UInt32 insLength = 4u) {
            Binary32 binary1 = (Binary32)reg.GetValue(rs1);
            Binary32 binary2 = (Binary32)reg.GetValue(rs2);
            Binary32 result;
            FloatCSR fcsr = 0;

            result = binary1 & 0x7fff_ffffu | (binary1 ^ binary2) & 0x8000_0000u;

            reg.SetValue(rd, result);
            reg.SetFflagsCSR(fcsr);
            reg.IncrementPc(insLength);
            return true;
        }

        /// <summary>
        /// Floating-Point Minimum, Single-Precision命令
        /// 浮動小数点レジスタrs1とrs2の小さい方を浮動小数点レジスタrdに書き込む
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">レジスタ番号</param>
        /// <param name="rs2">レジスタ番号</param>
        /// <returns>処理の成否</returns>
        public bool FminS(FPRegister rd, FPRegister rs1, FPRegister rs2, UInt32 insLength = 4u) {
            Binary32 binary1 = (Binary32)reg.GetValue(rs1);
            Binary32 binary2 = (Binary32)reg.GetValue(rs2);
            Binary32 result;
            FloatCSR fcsr = 0;

            result = ToSingle(binary1) < ToSingle(binary2) ? binary1 : (IsNegative(binary1) && IsZero(binary1) ? binary1 : binary2);
            result = IsNaN(result) ? NaN : result;

            if (IsSigNaN(binary1) || IsSigNaN(binary2)) {
                fcsr.NV = true;
            }

            reg.SetValue(rd, result);
            reg.SetFflagsCSR(fcsr);
            reg.IncrementPc(insLength);
            return true;
        }

        /// <summary>
        /// Floating-Point Maximum, Single-Precision命令
        /// 浮動小数点レジスタrs1とrs2の大きい方を浮動小数点レジスタrdに書き込む
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">レジスタ番号</param>
        /// <param name="rs2">レジスタ番号</param>
        /// <returns>処理の成否</returns>
        public bool FmaxS(FPRegister rd, FPRegister rs1, FPRegister rs2, UInt32 insLength = 4u) {
            Binary32 binary1 = (Binary32)reg.GetValue(rs1);
            Binary32 binary2 = (Binary32)reg.GetValue(rs2);
            Binary32 result;
            FloatCSR fcsr = 0;

            result = ToSingle(binary1) > ToSingle(binary2) ? binary1 : (IsPositive(binary1) && IsZero(binary1) ? binary1 : binary2);
            result = IsNaN(result) ? NaN : result;

            if (IsSigNaN(binary1) || IsSigNaN(binary2)) {
                fcsr.NV = true;
            }

            reg.SetValue(rd, result);
            reg.SetFflagsCSR(fcsr);
            reg.IncrementPc(insLength);
            return true;
        }

        /// <summary>
        /// Floating-Point Equals, Single-Precision命令
        /// 浮動小数点レジスタrs1とrs2が等しければ 1 を、そうでなければ 0 を
        /// 整数レジスタrdに書き込む
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">レジスタ番号</param>
        /// <returns>処理の成否</returns>
        public bool FeqS(Register rd, FPRegister rs1, FPRegister rs2, UInt32 insLength = 4u) {
            Binary32 binary1 = (Binary32)reg.GetValue(rs1);
            Binary32 binary2 = (Binary32)reg.GetValue(rs2);
            UInt32 result;
            FloatCSR fcsr = 0;

            result = ToSingle(binary1) == ToSingle(binary2) ? 1u : 0u;

            if (IsSigNaN(binary1) || IsSigNaN(binary2)) {
                fcsr.NV = true;
            }

            reg.SetValue(rd, result);
            reg.SetFflagsCSR(fcsr);
            reg.IncrementPc(insLength);
            return true;
        }

        /// <summary>
        /// Floating-Point Less Then, Single-Precision命令
        /// 浮動小数点レジスタrs1がrs2より小さければ 1 を、そうでなければ 0 を
        /// 整数レジスタrdに書き込む
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">レジスタ番号</param>
        /// <returns>処理の成否</returns>
        public bool FltS(Register rd, FPRegister rs1, FPRegister rs2, UInt32 insLength = 4u) {
            Binary32 binary1 = (Binary32)reg.GetValue(rs1);
            Binary32 binary2 = (Binary32)reg.GetValue(rs2);
            UInt32 result;
            FloatCSR fcsr = 0;

            result = ToSingle(binary1) < ToSingle(binary2) ? 1u : 0u;

            if (IsNaN(binary1) || IsNaN(binary2)) {
                fcsr.NV = true;
            }

            reg.SetValue(rd, result);
            reg.SetFflagsCSR(fcsr);
            reg.IncrementPc(insLength);
            return true;
        }

        /// <summary>
        /// Floating-Point Less Then or Equal, Single-Precision命令
        /// 浮動小数点レジスタrs1がrs2以下であれば 1 を、そうでなければ 0 を
        /// 整数レジスタrdに書き込む
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">レジスタ番号</param>
        /// <returns>処理の成否</returns>
        public bool FleS(Register rd, FPRegister rs1, FPRegister rs2, UInt32 insLength = 4u) {
            Binary32 binary1 = (Binary32)reg.GetValue(rs1);
            Binary32 binary2 = (Binary32)reg.GetValue(rs2);
            UInt32 result;
            FloatCSR fcsr = 0;

            result = ToSingle(binary1) <= ToSingle(binary2) ? 1u : 0u;

            if (IsNaN(binary1) || IsNaN(binary2)) {
                fcsr.NV = true;
            }

            reg.SetValue(rd, result);
            reg.SetFflagsCSR(fcsr);
            reg.IncrementPc(insLength);
            return true;
        }

        /// <summary>
        /// Floating-Point Convert to Word from Single命令
        /// 浮動小数点レジスタrs1の単精度浮動小数を整数(符号付き)に変換して、整数レジスタrdに書き込む
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">レジスタ番号</param>
        /// <returns>処理の成否</returns>
        public bool FcvtWS(Register rd, FPRegister rs1, FloatRoundingMode frm, UInt32 insLength = 4u) {
            Binary32 binary1 = (Binary32)reg.GetValue(rs1);
            Single value1 = ToSingle(binary1);
            Single rvalue1 = ToSingle(RoundNum(binary1, 0, frm));
            Int32 result;
            FloatCSR fcsr = 0;

            result = (Int32)rvalue1;

            if ((IsPositive(binary1) && IsInfinity(binary1)) || IsNaN(binary1)) {
                fcsr.NV = true;
                result = Int32.MaxValue;

            } else if (IsNegative(binary1) && IsInfinity(binary1)) {
                fcsr.NV = true;
                result = Int32.MinValue;

            } else if (Int32.MaxValue < rvalue1) {
                fcsr.NV = true;
                result = Int32.MaxValue;

            } else if (Int32.MinValue > rvalue1) {
                fcsr.NV = true;
                result = Int32.MinValue;

            } else if (result != value1) {
                fcsr.NX = true;
            }

            reg.SetValue(rd, (UInt32)result);
            reg.SetFflagsCSR(fcsr);
            reg.IncrementPc(insLength);
            return true;
        }

        /// <summary>
        /// Floating-Point Convert to Unsigned Word from Single命令
        /// 浮動小数点レジスタrs1の単精度浮動小数を整数(符号なし)に変換して、整数レジスタrdに書き込む
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">レジスタ番号</param>
        /// <returns>処理の成否</returns>
        public bool FcvtWUS(Register rd, FPRegister rs1, FloatRoundingMode frm, UInt32 insLength = 4u) {
            Binary32 binary1 = (Binary32)reg.GetValue(rs1);
            Single value1 = ToSingle(binary1);
            Single rvalue1 = ToSingle(RoundNum(binary1, 0, frm));
            UInt32 result;
            FloatCSR fcsr = 0;

            result = (UInt32)rvalue1;

            if ((IsPositive(binary1) && IsInfinity(binary1)) || IsNaN(binary1)) {
                fcsr.NV = true;
                result = UInt32.MaxValue;

            } else if (IsNegative(binary1) && IsInfinity(binary1)) {
                fcsr.NV = true;
                result = UInt32.MinValue;

            } else if(UInt32.MaxValue < rvalue1) {
                fcsr.NV = true;
                result = UInt32.MaxValue;

            } else if (UInt32.MinValue > rvalue1) {
                fcsr.NV = true;
                result = UInt32.MinValue;

            } else if (result != value1) {
                fcsr.NX = true;
            }

            reg.SetValue(rd, result);
            reg.SetFflagsCSR(fcsr);
            reg.IncrementPc(insLength);
            return true;
        }

        /// <summary>
        /// Floating-Point Convert to Single from Word命令
        /// 整数レジスタrs1(符号付き)の整数を単精度浮動小数に変換して、浮動小数点レジスタrdに書き込む
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">レジスタ番号</param>
        /// <returns>処理の成否</returns>
        public bool FcvtSW(FPRegister rd, Register rs1, FloatRoundingMode frm, UInt32 insLength = 4u) {
            Int32 value1 = (Int32)reg.GetValue(rs1);
            Binary32 result;
            FloatCSR fcsr = 0;

            result = ToBinary((Single)value1);

            if ((Int32)ToSingle(result) != value1) {
                fcsr.NX = true;
            }

            reg.SetValue(rd, result);
            reg.SetFflagsCSR(fcsr);
            reg.IncrementPc(insLength);
            return true;
        }

        /// <summary>
        /// Floating-Point Convert to Single from Unsigned Word命令
        /// 整数レジスタrs1(符号なし)の整数を単精度浮動小数に変換して、浮動小数点レジスタrdに書き込む
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">レジスタ番号</param>
        /// <returns>処理の成否</returns>
        public bool FcvtSWU(FPRegister rd, Register rs1, FloatRoundingMode frm, UInt32 insLength = 4u) {
            UInt32 value1 = reg.GetValue(rs1);
            Binary32 result;
            FloatCSR fcsr = 0;

            result = ToBinary((Single)value1);

            if ((UInt32)ToSingle(result) != value1) {
                fcsr.NX = true;
            }

            reg.SetValue(rd, result);
            reg.SetFflagsCSR(fcsr);
            reg.IncrementPc(insLength);
            return true;
        }

        /// <summary>
        /// Floating-Point Move to Integer from Word命令
        /// 浮動小数点レジスタrs1を整数レジスタrd(符号付き)に書き込む
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">レジスタ番号</param>
        /// <returns>処理の成否</returns>
        public bool FmvXW(Register rd, FPRegister rs1, UInt32 insLength = 4u) {
            Binary32 binary1 = (Binary32)reg.GetValue(rs1);
            UInt32 result;

            result = binary1;

            reg.SetValue(rd, result);
            reg.IncrementPc(insLength);
            return true;
        }

        /// <summary>
        /// Floating-Point Move to Word from Integer命令
        /// 整数レジスタrs1(符号付き)を浮動小数点レジスタrdに書き込む
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">レジスタ番号</param>
        /// <returns>処理の成否</returns>
        public bool FmvWX(FPRegister rd, Register rs1, UInt32 insLength = 4u) {
            UInt32 value1 = reg.GetValue(rs1);
            Binary32 result;

            result = value1;

            reg.SetValue(rd, result);
            reg.IncrementPc(insLength);
            return true;
        }

        /// <summary>
        /// Floating-Point Classigy, Single-Precision命令
        /// 浮動小数点レジスタrs1のクラスを示すマスクを整数レジスタrdに書き込む
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">レジスタ番号</param>
        /// <returns>処理の成否</returns>
        public bool FclassS(Register rd, FPRegister rs1, UInt32 insLength = 4u) {
            Binary32 binary1 = (Binary32)reg.GetValue(rs1);
            UInt32 result = 0u;

            if (IsNegative(binary1) && IsInfinity(binary1)) {
                // rs1が-∞の場合
                result |= 0b00_0000_0001;

            } else if (IsNegative(binary1) && IsNormalNum(binary1)) {
                // rs1が負の正規数の場合
                result |= 0b00_0000_0010;

            } else if (IsNegative(binary1) && IsDenormalNum(binary1)) {
                // rs1が負の非正規数の場合
                result |= 0b00_0000_0100;

            } else if (IsNegative(binary1) && IsZero(binary1)) {
                // rs1が-0の場合
                result |= 0b00_0000_1000;

            } else if (IsPositive(binary1) && IsZero(binary1)) {
                // rs1が+0の場合
                result |= 0b00_0001_0000;

            } else if (IsPositive(binary1) && IsDenormalNum(binary1)) {
                // rs1が正の非正規数の場合
                result |= 0b00_0010_0000;

            } else if (IsPositive(binary1) && IsNormalNum(binary1)) {
                // rs1が正の正規数の場合
                result |= 0b00_0100_0000;

            } else if (IsPositive(binary1) && IsInfinity(binary1)) {
                // rs1が+∞の場合
                result |= 0b00_1000_0000;

            } else if (IsSigNaN(binary1)) {
                // rs1がシグナル型非数の場合
                result |= 0b01_0000_0000;

            } else if (IsQuietNaN(binary1)) {
                // rs1がクワイエット型非数の場合
                result |= 0b10_0000_0000;
            }

            reg.SetValue(rd, result);
            reg.IncrementPc(insLength);
            return true;
        }

        #endregion

        #endregion

        internal static bool IsZero(Binary32 binary) => (binary & (ExpMask | MantMask)) == Zero;

        internal static bool IsDenormalNum(Binary32 binary) => (binary & ExpMask) == 0u && !IsZero(binary);
        internal static bool IsNormalNum(Binary32 binary) => (binary & ExpMask) > 0u && (binary & ExpMask) < 0x7f80_0000u;

        internal static bool IsNaN(Binary32 binary) => (binary & ExpMask) == 0x7f80_0000u && (binary & MantMask) > 0u;
        internal static bool IsSigNaN(Binary32 binary) => IsNaN(binary) && (binary & 0x0040_0000u) == 0x0000_0000u;
        internal static bool IsQuietNaN(Binary32 binary) => IsNaN(binary) && (binary & 0x0040_0000u) == 0x0040_0000u;

        internal static bool IsInfinity(Binary32 binary) => (binary & (ExpMask | MantMask)) == Infinity;

        internal static bool IsNegative(Binary32 binary) => (binary & SignMask) == NegativeSign;
        internal static bool IsPositive(Binary32 binary) => !IsNegative(binary);

        /// <summary>
        /// 32bit長バイナリ形式から単精度浮動小数点数に変換する
        /// </summary>
        /// <param name="binary">バイナリ形式の値</param>
        /// <returns>変換した単精度浮動小数点数</returns>
        internal static Single ToSingle(Binary32 binary) => BitConverter.ToSingle(BitConverter.GetBytes(binary), 0);
        
        /// <summary>
        /// 単精度浮動小数点数を32bit長バイナリ形式に変換する
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static Binary32 ToBinary(Single value) => BitConverter.ToUInt32(BitConverter.GetBytes(value), 0);

        /// <summary>
        /// 値を指定した桁数、丸めモードで丸める
        /// </summary>
        /// <param name="binary">丸めるバイナリ形式の値</param>
        /// <param name="digits">桁数</param>
        /// <param name="frm">丸めモード</param>
        /// <returns>丸めたバイナリ形式の値</returns>
        private static Binary32 RoundNum(Binary32 binary, int digits, FloatRoundingMode frm) {
            Binary32 result;
            Single coef;
            switch (frm) {
                case FloatRoundingMode.RNE: // 最近接丸め(偶数)
                    result = ToBinary((Single)Math.Round(ToSingle(binary), digits, MidpointRounding.ToEven));
                    break;

                case FloatRoundingMode.RTZ: // 0への丸め
                    coef = (Single)Math.Pow(10, digits);
                    Single sign = IsNegative(binary) ? -1f : 1f;
                    result = ToBinary(sign * (Single)(Math.Floor(sign * ToSingle(binary) * coef) / coef));
                    break;

                case FloatRoundingMode.RDN: // 切り下げ
                    coef = (Single)Math.Pow(10, digits);
                    result = ToBinary((Single)(Math.Ceiling(ToSingle(binary) * coef) / coef));
                    break;

                case FloatRoundingMode.RUP: // 切り上げ
                    coef = (Single)Math.Pow(10, digits);
                    result = ToBinary((Single)(Math.Floor(ToSingle(binary) * coef) / coef));
                    break;

                case FloatRoundingMode.RMM: // 最近接丸め(0から遠くへの丸め)
                    result = ToBinary((Single)Math.Round(ToSingle(binary), digits, MidpointRounding.AwayFromZero));
                    break;

                default:
                    result = 0u;
                    break;
            }

            return result;
        }
    }
}
