using RV32_Lsu.Constants;
using RV32_Lsu.RegisterSet;
using System;

namespace RV32_Alu {

    /// <summary>64bit長バイナリ形式</summary>
    using Binary64 = UInt64;
    /// <summary>32bit長バイナリ形式</summary>
    using Binary32 = UInt32;

    /// <summary>
    /// Risc-V RV32I 倍精度浮動小数点命令セット 算術論理演算命令を実行するFPU
    /// </summary>
    public class RV32_DoubleFpu : RV32_AbstractCalculator {

        internal const Binary64 NaN = 0x7ff8_0000_0000_0000u;
        internal const Binary64 Zero = 0x0000_0000_0000_0000u;
        internal const Binary64 Infinity = 0x7ff0_0000_0000_0000u;

        internal const Binary64 NegativeSign = 0x8000_0000_0000_0000u;

        internal const Binary64 SignMask = 0x8000_0000_0000_0000u;
        internal const Binary64 ExpMask = 0x7ff0_0000_0000_0000u;
        internal const Binary64 MantMask = 0x000f_ffff_ffff_ffffu;

        /// <summary>
        /// Risc-V 倍精度浮動小数算術論理演算 FPU
        /// </summary>
        /// <param name="registerSet">入出力用レジスタ</param>
        public RV32_DoubleFpu(RV32_RegisterSet reg) : base(reg) {
        }

        #region Risc-V CPU, Double-Precision命令

        #region Risv-V CPU 浮動小数点演算命令

        /// <summary>
        /// Floating-Point Fused Multiply-Add, Double-Precision命令
        /// 浮動小数点レジスタrs1とrs2の積にrs3を足した値を浮動小数点レジスタrdに書き込む
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">レジスタ番号</param>
        /// <param name="rs2">レジスタ番号</param>
        /// <param name="rs3">レジスタ番号</param>
        /// <returns>処理の成否</returns>
        public bool FmaddD(FPRegister rd, FPRegister rs1, FPRegister rs2, FPRegister rs3, FloatRoundingMode frm, UInt32 insLength = 4u) {
            Binary64 binary1 = reg.GetValue(rs1);
            Binary64 binary2 = reg.GetValue(rs2);
            Binary64 binary3 = reg.GetValue(rs3);
            Binary64 result;
            FloatCSR fcsr = 0;

            result = ToBinary((ToDouble(binary1) * ToDouble(binary2)) + ToDouble(binary3));
            result = IsNaN(result) ? NaN : result;

            if ((IsInfinity(binary1) || IsInfinity(binary2)) && IsInfinity(binary3) &&
                ((IsNegative(binary1) ^ IsPositive(binary2)) ^ IsNegative(binary3))) {
                // ∞ + -∞ もしくは -∞ + ∞の場合
                result = NaN;
                fcsr.NV = true;

            } else if (IsSigNaN(binary1) || IsSigNaN(binary2) || IsSigNaN(binary3)) {
                // いずれかの数値がシグナリングNaNの場合
                fcsr.NV = true;

            } else if (ToDouble(result) != (ToDouble(binary1) * ToDouble(binary2)) + ToDouble(binary3)) {
                // 結果が一致しない場合
                fcsr.NX = true;

            } else if (IsRounded(result, binary1) || IsRounded(result, binary2) || IsRounded(result, binary3)) {
                // 桁丸めが発生している場合
                fcsr.NX = true;
            }

            reg.SetValue(rd, result);
            reg.SetFflagsCSR(fcsr);
            reg.IncrementPc(insLength);
            return true;
        }

        /// <summary>
        /// Floating-Point Fused Multiply-Subtract, Double-Precision命令
        /// 浮動小数点レジスタrs1とrs2の積からrs3を引いた値を浮動小数点レジスタrdに書き込む
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">レジスタ番号</param>
        /// <param name="rs2">レジスタ番号</param>
        /// <param name="rs3">レジスタ番号</param>
        /// <returns>処理の成否</returns>
        public bool FmsubD(FPRegister rd, FPRegister rs1, FPRegister rs2, FPRegister rs3, FloatRoundingMode frm, UInt32 insLength = 4u) {
            Binary64 binary1 = reg.GetValue(rs1);
            Binary64 binary2 = reg.GetValue(rs2);
            Binary64 binary3 = reg.GetValue(rs3);
            Binary64 result;
            FloatCSR fcsr = 0;

            result = ToBinary((ToDouble(binary1) * ToDouble(binary2)) - ToDouble(binary3));
            result = IsNaN(result) ? NaN : result;

            if ((IsInfinity(binary1) || IsInfinity(binary2)) && IsInfinity(binary3) &&
                ((IsNegative(binary1) ^ IsPositive(binary2)) ^ IsPositive(binary3))) {
                // ∞ + -∞ もしくは -∞ + ∞の場合
                result = NaN;
                fcsr.NV = true;

            } else if (IsSigNaN(binary1) || IsSigNaN(binary2) || IsSigNaN(binary3)) {
                // いずれかの数値がシグナリングNaNの場合
                fcsr.NV = true;

            } else if (ToDouble(result) != (ToDouble(binary1) * ToDouble(binary2)) - ToDouble(binary3)) {
                // 結果が一致しない場合
                fcsr.NX = true;

            } else if (IsRounded(result, binary1) || IsRounded(result, binary2) || IsRounded(result, binary3)) {
                // 桁丸めが発生している場合
                fcsr.NX = true;
            }

            reg.SetValue(rd, result);
            reg.SetFflagsCSR(fcsr);
            reg.IncrementPc(insLength);
            return true;
        }

        /// <summary>
        /// Floating-Point Fused Negative Multiply-Add, Double-Precision命令
        /// 浮動小数点レジスタrs1に-1掛けた値とrs2の積にrs3を足した値を浮動小数点レジスタrdに書き込む
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">レジスタ番号</param>
        /// <param name="rs2">レジスタ番号</param>
        /// <param name="rs3">レジスタ番号</param>
        /// <returns>処理の成否</returns>
        public bool FnmaddD(FPRegister rd, FPRegister rs1, FPRegister rs2, FPRegister rs3, FloatRoundingMode frm, UInt32 insLength = 4u) {
            Binary64 binary1 = reg.GetValue(rs1);
            Binary64 binary2 = reg.GetValue(rs2);
            Binary64 binary3 = reg.GetValue(rs3);
            Binary64 result;
            FloatCSR fcsr = 0;

            result = ToBinary((-ToDouble(binary1) * ToDouble(binary2)) - ToDouble(binary3));
            result = IsNaN(result) ? NaN : result;

            if ((IsInfinity(binary1) || IsInfinity(binary2)) && IsInfinity(binary3) &&
                ((IsPositive(binary1) ^ IsPositive(binary2)) ^ IsPositive(binary3))) {
                // ∞ + -∞ もしくは -∞ + ∞の場合
                result = NaN;
                fcsr.NV = true;

            } else if (IsSigNaN(binary1) || IsSigNaN(binary2) || IsSigNaN(binary3)) {
                // いずれかの数値がシグナリングNaNの場合
                fcsr.NV = true;

            } else if (ToDouble(result) != (-ToDouble(binary1) * ToDouble(binary2)) - ToDouble(binary3)) {
                // 結果が一致しない場合
                fcsr.NX = true;

            } else if (IsRounded(result, binary1) || IsRounded(result, binary2) || IsRounded(result, binary3)) {
                // 桁丸めが発生している場合
                fcsr.NX = true;
            }

            reg.SetValue(rd, result);
            reg.SetFflagsCSR(fcsr);
            reg.IncrementPc(insLength);
            return true;
        }

        /// <summary>
        /// Floating-Point Fused Negative Multiply-Subtract, Double-Precision命令
        /// 浮動小数点レジスタrs1に-1掛けた値とrs2の積からrs3を引いた値を浮動小数点レジスタrdに書き込む
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">レジスタ番号</param>
        /// <param name="rs2">レジスタ番号</param>
        /// <param name="rs3">レジスタ番号</param>
        /// <returns>処理の成否</returns>
        public bool FnmsubD(FPRegister rd, FPRegister rs1, FPRegister rs2, FPRegister rs3, FloatRoundingMode frm, UInt32 insLength = 4u) {
            Binary64 binary1 = reg.GetValue(rs1);
            Binary64 binary2 = reg.GetValue(rs2);
            Binary64 binary3 = reg.GetValue(rs3);
            Binary64 result;
            FloatCSR fcsr = 0;

            result = ToBinary((-ToDouble(binary1) * ToDouble(binary2)) + ToDouble(binary3));
            result = IsNaN(result) ? NaN : result;

            if ((IsInfinity(binary1) || IsInfinity(binary2)) && IsInfinity(binary3) &&
                ((IsPositive(binary1) ^ IsPositive(binary2)) ^ IsNegative(binary3))) {
                // ∞ + -∞ もしくは -∞ + ∞の場合
                result = NaN;
                fcsr.NV = true;

            } else if (IsSigNaN(binary1) || IsSigNaN(binary2) || IsSigNaN(binary3)) {
                // いずれかの数値がシグナリングNaNの場合
                fcsr.NV = true;

            } else if (ToDouble(result) != (-ToDouble(binary1) * ToDouble(binary2)) + ToDouble(binary3)) {
                // 結果が一致しない場合
                fcsr.NX = true;

            } else if (IsRounded(result, binary1) || IsRounded(result, binary2) || IsRounded(result, binary3)) {
                // 桁丸めが発生している場合
                fcsr.NX = true;
            }

            reg.SetValue(rd, result);
            reg.SetFflagsCSR(fcsr);
            reg.IncrementPc(insLength);
            return true;
        }

        /// <summary>
        /// Floating-Point Add, Double-Precision命令
        /// 浮動小数点レジスタrs1とrs2の和を浮動小数点レジスタrdに書き込む
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">レジスタ番号</param>
        /// <param name="rs2">レジスタ番号</param>
        /// <returns>処理の成否</returns>
        public bool FaddD(FPRegister rd, FPRegister rs1, FPRegister rs2, FloatRoundingMode frm, UInt32 insLength = 4u) {
            Binary64 binary1 = reg.GetValue(rs1);
            Binary64 binary2 = reg.GetValue(rs2);
            Binary64 result;
            FloatCSR fcsr = 0;

            result = ToBinary(ToDouble(binary1) + ToDouble(binary2));
            result = IsNaN(result) ? NaN : result;

            if (IsInfinity(binary1) && IsInfinity(binary2) &&
                ((IsPositive(binary1) && IsNegative(binary2)) || (IsNegative(binary1) && IsPositive(binary2)))) {
                // ∞ + -∞ もしくは -∞ + ∞の場合
                result = NaN;
                fcsr.NV = true;

            } else if (IsSigNaN(binary1) || IsSigNaN(binary2)) {
                // いずれかの数値がシグナリングNaNの場合
                fcsr.NV = true;

            } else if (ToDouble(result) != ToDouble(binary1) + ToDouble(binary2)) {
                // 結果が一致しない場合
                fcsr.NX = true;

            } else if (IsRounded(result, binary1) || IsRounded(result, binary2)) {
                // 桁丸めが発生している場合
                fcsr.NX = true;
            }

            reg.SetValue(rd, result);
            reg.SetFflagsCSR(fcsr);
            reg.IncrementPc(insLength);
            return true;
        }

        /// <summary>
        /// Floating-Point Subtract, Double-Precision命令
        /// 浮動小数点レジスタrs1とrs2の差を浮動小数点レジスタrdに書き込む
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">レジスタ番号</param>
        /// <param name="rs2">レジスタ番号</param>
        /// <returns>処理の成否</returns>
        public bool FsubD(FPRegister rd, FPRegister rs1, FPRegister rs2, FloatRoundingMode frm, UInt32 insLength = 4u) {
            Binary64 binary1 = reg.GetValue(rs1);
            Binary64 binary2 = reg.GetValue(rs2);
            Binary64 result;
            FloatCSR fcsr = 0;

            result = ToBinary(ToDouble(binary1) - ToDouble(binary2));
            result = IsNaN(result) ? NaN : result;

            if (IsInfinity(binary1) && IsInfinity(binary2) &&
                ((IsPositive(binary1) && IsPositive(binary2)) || (IsNegative(binary1) && IsNegative(binary2)))) {
                // ∞ - ∞ もしくは -∞ - -∞の場合
                result = NaN;
                fcsr.NV = true;

            } else if (IsSigNaN(binary1) || IsSigNaN(binary2)) {
                // いずれかの数値がシグナリングNaNの場合
                fcsr.NV = true;

            } else if (ToDouble(result) != ToDouble(binary1) - ToDouble(binary2)) {
                // 結果が一致しない場合
                fcsr.NX = true;

            } else if (IsRounded(result, binary1) || IsRounded(result, binary2)) {
                // 桁丸めが発生している場合
                fcsr.NX = true;
            }

            reg.SetValue(rd, result);
            reg.SetFflagsCSR(fcsr);
            reg.IncrementPc(insLength);
            return true;
        }

        /// <summary>
        /// Floating-Point Multiply, Double-Precision命令
        /// 浮動小数点レジスタrs1とrs2の積を浮動小数点レジスタrdに書き込む
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">レジスタ番号</param>
        /// <param name="rs2">レジスタ番号</param>
        /// <returns>処理の成否</returns>
        public bool FmulD(FPRegister rd, FPRegister rs1, FPRegister rs2, FloatRoundingMode frm, UInt32 insLength = 4u) {
            Binary64 binary1 = reg.GetValue(rs1);
            Binary64 binary2 = reg.GetValue(rs2);
            Binary64 result;
            FloatCSR fcsr = 0;

            result = ToBinary(ToDouble(binary1) * ToDouble(binary2));
            result = IsNaN(result) ? NaN : result;

            if (IsSigNaN(binary1) || IsSigNaN(binary2)) {
                // いずれかの数値がシグナリングNaNの場合
                fcsr.NV = true;

            } else if (ToDouble(result) != ToDouble(binary1) * ToDouble(binary2)) {
                // 結果が一致しない場合
                fcsr.NX = true;

            } else if (IsRounded(result, binary1) || IsRounded(result, binary2)) {
                // 桁丸めが発生している場合
                fcsr.NX = true;
            }

            reg.SetValue(rd, result);
            reg.SetFflagsCSR(fcsr);
            reg.IncrementPc(insLength);
            return true;
        }

        /// <summary>
        /// Floating-Point Divide, Double-Precision命令
        /// 浮動小数点レジスタrs1とrs2の商を浮動小数点レジスタrdに書き込む
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">レジスタ番号</param>
        /// <param name="rs2">レジスタ番号</param>
        /// <returns>処理の成否</returns>
        public bool FdivD(FPRegister rd, FPRegister rs1, FPRegister rs2, FloatRoundingMode frm, UInt32 insLength = 4u) {
            Binary64 binary1 = reg.GetValue(rs1);
            Binary64 binary2 = reg.GetValue(rs2);
            Binary64 result;
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
                result = ToBinary(ToDouble(binary1) / ToDouble(binary2));
                result = IsNaN(result) ? NaN : result;

                if (IsSigNaN(binary1) || IsSigNaN(binary2)) {
                    // いずれかの数値がシグナリングNaNの場合
                    fcsr.NV = true;

                } else if (ToDouble(result) != ToDouble(binary1) / ToDouble(binary2)) {
                    // 結果が一致しない場合
                    fcsr.NX = true;

                } else if (IsRounded(result, binary1) || IsRounded(result, binary2)) {
                    // 桁丸めが発生している場合
                    fcsr.NX = true;
                }
            }

            reg.SetValue(rd, result);
            reg.SetFflagsCSR(fcsr);
            reg.IncrementPc(insLength);
            return true;
        }

        /// <summary>
        /// Floating-Point Square Root, Double-Precision命令
        /// 浮動小数点レジスタrs1の平方根を浮動小数点レジスタrdに書き込む
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">レジスタ番号</param>
        /// <param name="rs2">レジスタ番号</param>
        /// <returns>処理の成否</returns>
        public bool FsqrtD(FPRegister rd, FPRegister rs1, FloatRoundingMode frm, UInt32 insLength = 4u) {
            Binary64 binary1 = reg.GetValue(rs1);
            Binary64 result;
            FloatCSR fcsr = 0;

            result = ToBinary((Double)Math.Sqrt(ToDouble(binary1)));
            result = IsNaN(result) ? NaN : result;

            if (IsSigNaN(binary1)) {
                // いずれかの数値がシグナリングNaNの場合
                fcsr.NV = true;

            } else if (IsNegative(binary1)) {
                // 負数の平方根を求めようとした場合
                fcsr.NV = true;

            } else if (ToDouble(result) * ToDouble(result) != ToDouble(binary1)) {
                // 結果が一致しない場合
                fcsr.NX = true;

            } else if (IsRounded(binary1, result)) {
                // 桁丸めが発生している場合
                fcsr.NX = true;
            }

            reg.SetValue(rd, result);
            reg.SetFflagsCSR(fcsr);
            reg.IncrementPc(insLength);
            return true;
        }

        /// <summary>
        /// Floating-Point Sign Inject, Double-Precision命令
        /// 浮動小数点レジスタrs1の指数と仮数と、rs2の符号を組み立てて
        /// 浮動小数点レジスタrdに書き込む
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">レジスタ番号</param>
        /// <param name="rs2">レジスタ番号</param>
        /// <returns>処理の成否</returns>
        public bool FsgnjD(FPRegister rd, FPRegister rs1, FPRegister rs2, UInt32 insLength = 4u) {
            Binary64 binary1 = reg.GetValue(rs1);
            Binary64 binary2 = reg.GetValue(rs2);
            Binary64 result;

            result = binary1 & 0x7fff_ffffu | binary2 & 0x8000_0000u;

            reg.SetValue(rd, result);
            reg.IncrementPc(insLength);
            return true;
        }

        /// <summary>
        /// Floating-Point Sign Inject, Double-Precision命令
        /// 浮動小数点レジスタrs1の指数と仮数と、rs2の符号反転したものを組み立てて
        /// 浮動小数点レジスタrdに書き込む
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">レジスタ番号</param>
        /// <param name="rs2">レジスタ番号</param>
        /// <returns>処理の成否</returns>
        public bool FsgnjnD(FPRegister rd, FPRegister rs1, FPRegister rs2, UInt32 insLength = 4u) {
            Binary64 binary1 = reg.GetValue(rs1);
            Binary64 binary2 = reg.GetValue(rs2);
            Binary64 result;
            FloatCSR fcsr = 0;

            result = binary1 & 0x7fff_ffffu | ~binary2 & 0x8000_0000u;

            reg.SetValue(rd, result);
            reg.SetFflagsCSR(fcsr);
            reg.IncrementPc(insLength);
            return true;
        }

        /// <summary>
        /// Floating-Point Sign Inject, Double-Precision命令
        /// 浮動小数点レジスタrs1の指数と仮数と、rs1の符号とrs2の符号の排他的論理和を組み立てて
        /// 浮動小数点レジスタrdに書き込む
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">レジスタ番号</param>
        /// <param name="rs2">レジスタ番号</param>
        /// <returns>処理の成否</returns>
        public bool FsgnjxD(FPRegister rd, FPRegister rs1, FPRegister rs2, UInt32 insLength = 4u) {
            Binary64 binary1 = reg.GetValue(rs1);
            Binary64 binary2 = reg.GetValue(rs2);
            Binary64 result;
            FloatCSR fcsr = 0;

            result = binary1 & 0x7fff_ffffu | (binary1 ^ binary2) & 0x8000_0000u;

            reg.SetValue(rd, result);
            reg.SetFflagsCSR(fcsr);
            reg.IncrementPc(insLength);
            return true;
        }

        /// <summary>
        /// Floating-Point Minimum, Double-Precision命令
        /// 浮動小数点レジスタrs1とrs2の小さい方を浮動小数点レジスタrdに書き込む
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">レジスタ番号</param>
        /// <param name="rs2">レジスタ番号</param>
        /// <returns>処理の成否</returns>
        public bool FminD(FPRegister rd, FPRegister rs1, FPRegister rs2, UInt32 insLength = 4u) {
            Binary64 binary1 = reg.GetValue(rs1);
            Binary64 binary2 = reg.GetValue(rs2);
            Binary64 result;
            FloatCSR fcsr = 0;

            result = ToDouble(binary1) < ToDouble(binary2) ? binary1 : (IsNegative(binary1) && IsZero(binary1) ? binary1 : binary2);
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
        /// Floating-Point Maximum, Double-Precision命令
        /// 浮動小数点レジスタrs1とrs2の大きい方を浮動小数点レジスタrdに書き込む
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">レジスタ番号</param>
        /// <param name="rs2">レジスタ番号</param>
        /// <returns>処理の成否</returns>
        public bool FmaxD(FPRegister rd, FPRegister rs1, FPRegister rs2, UInt32 insLength = 4u) {
            Binary64 binary1 = reg.GetValue(rs1);
            Binary64 binary2 = reg.GetValue(rs2);
            Binary64 result;
            FloatCSR fcsr = 0;

            result = ToDouble(binary1) > ToDouble(binary2) ? binary1 : (IsPositive(binary1) && IsZero(binary1) ? binary1 : binary2);
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
        /// Floating-Point Equals, Double-Precision命令
        /// 浮動小数点レジスタrs1とrs2が等しければ 1 を、そうでなければ 0 を
        /// 整数レジスタrdに書き込む
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">レジスタ番号</param>
        /// <returns>処理の成否</returns>
        public bool FeqD(Register rd, FPRegister rs1, FPRegister rs2, UInt32 insLength = 4u) {
            Binary64 binary1 = reg.GetValue(rs1);
            Binary64 binary2 = reg.GetValue(rs2);
            UInt32 result;
            FloatCSR fcsr = 0;

            result = ToDouble(binary1) == ToDouble(binary2) ? 1u : 0u;

            if (IsSigNaN(binary1) || IsSigNaN(binary2)) {
                fcsr.NV = true;
            }

            reg.SetValue(rd, result);
            reg.SetFflagsCSR(fcsr);
            reg.IncrementPc(insLength);
            return true;
        }

        /// <summary>
        /// Floating-Point Less Then, Double-Precision命令
        /// 浮動小数点レジスタrs1がrs2より小さければ 1 を、そうでなければ 0 を
        /// 整数レジスタrdに書き込む
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">レジスタ番号</param>
        /// <returns>処理の成否</returns>
        public bool FltD(Register rd, FPRegister rs1, FPRegister rs2, UInt32 insLength = 4u) {
            Binary64 binary1 = reg.GetValue(rs1);
            Binary64 binary2 = reg.GetValue(rs2);
            UInt32 result;
            FloatCSR fcsr = 0;

            result = ToDouble(binary1) < ToDouble(binary2) ? 1u : 0u;

            if (IsNaN(binary1) || IsNaN(binary2)) {
                fcsr.NV = true;
            }

            reg.SetValue(rd, result);
            reg.SetFflagsCSR(fcsr);
            reg.IncrementPc(insLength);
            return true;
        }

        /// <summary>
        /// Floating-Point Less Then or Equal, Double-Precision命令
        /// 浮動小数点レジスタrs1がrs2以下であれば 1 を、そうでなければ 0 を
        /// 整数レジスタrdに書き込む
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">レジスタ番号</param>
        /// <returns>処理の成否</returns>
        public bool FleD(Register rd, FPRegister rs1, FPRegister rs2, UInt32 insLength = 4u) {
            Binary64 binary1 = reg.GetValue(rs1);
            Binary64 binary2 = reg.GetValue(rs2);
            UInt32 result;
            FloatCSR fcsr = 0;

            result = ToDouble(binary1) <= ToDouble(binary2) ? 1u : 0u;

            if (IsNaN(binary1) || IsNaN(binary2)) {
                fcsr.NV = true;
            }

            reg.SetValue(rd, result);
            reg.SetFflagsCSR(fcsr);
            reg.IncrementPc(insLength);
            return true;
        }

        /// <summary>
        /// Floating-Point Convert to Single from Double命令
        /// 浮動小数点レジスタrs1の倍精度浮動小数を単精度浮動小数に変換して、浮動小数点レジスタrdに書き込む
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">レジスタ番号</param>
        /// <returns>処理の成否</returns>
        public bool FcvtSD(FPRegister rd, FPRegister rs1, FloatRoundingMode frm, UInt32 insLength = 4u) {
            Binary64 binary1 = reg.GetValue(rs1);
            Binary32 result;
            FloatCSR fcsr = 0;

            Binary32 sign = (Binary32)((binary1 & SignMask) >> 32);

            if (IsInfinity(binary1)) {
                // +-∞の場合
                result = sign | RV32_SingleFpu.Infinity;

            } else if (IsZero(binary1)) {
                // +-0の場合
                result = sign | RV32_SingleFpu.Zero;

            } else if (IsSigNaN(binary1)) {
                // シグナリングNaNの場合
                fcsr.NV = true;
                result = RV32_SingleFpu.NaN;

            } else if (IsQuietNaN(binary1)) {
                result = RV32_SingleFpu.NaN;

            } else {

                result = RV32_SingleFpu.ToBinary((Single)ToDouble(binary1));


                if (RV32_SingleFpu.ToSingle(result) != ToDouble(binary1)) {
                    // 結果が一致しない場合
                    fcsr.NX = true;
                    if (ToDouble(binary1) > 0) {
                        fcsr.OF = true;
                    } else {
                        fcsr.UF = true;
                    }

                }
            }

            reg.SetValue(rd, result);
            reg.SetFflagsCSR(fcsr);
            reg.IncrementPc(insLength);
            return true;
        }

        /// <summary>
        /// Floating-Point Convert to Double from Single命令
        /// 浮動小数点レジスタrs1の単精度浮動小数を倍精度浮動小数に変換して、浮動小数点レジスタrdに書き込む
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">レジスタ番号</param>
        /// <returns>処理の成否</returns>
        public bool FcvtDS(FPRegister rd, FPRegister rs1, FloatRoundingMode frm, UInt32 insLength = 4u) {
            Binary32 binary1 = (Binary32)reg.GetValue(rs1);
            Binary64 result;
            FloatCSR fcsr = 0;

            result = ToBinary((Double)RV32_SingleFpu.ToSingle(binary1));

            reg.SetValue(rd, result);
            reg.SetFflagsCSR(fcsr);
            reg.IncrementPc(insLength);
            return true;
        }

        /// <summary>
        /// Floating-Point Convert to Word from Double命令
        /// 浮動小数点レジスタrs1の倍精度浮動小数を整数(符号付き)に変換して、整数レジスタrdに書き込む
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">レジスタ番号</param>
        /// <returns>処理の成否</returns>
        public bool FcvtWD(Register rd, FPRegister rs1, FloatRoundingMode frm, UInt32 insLength = 4u) {
            Binary64 binary1 = reg.GetValue(rs1);
            Int32 result;
            FloatCSR fcsr = 0;

            Double value1 = ToDouble(binary1);
            Double rvalue1 = ToDouble(RoundNum(binary1, 0, frm));

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
        /// Floating-Point Convert to Unsigned Word from Double命令
        /// 浮動小数点レジスタrs1の倍精度浮動小数を整数(符号なし)に変換して、整数レジスタrdに書き込む
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">レジスタ番号</param>
        /// <returns>処理の成否</returns>
        public bool FcvtWUD(Register rd, FPRegister rs1, FloatRoundingMode frm, UInt32 insLength = 4u) {
            Binary64 binary1 = reg.GetValue(rs1);
            UInt32 result;
            FloatCSR fcsr = 0;

            Double value1 = ToDouble(binary1);
            Double rvalue1 = ToDouble(RoundNum(binary1, 0, frm));

            result = (UInt32)rvalue1;

            if ((IsPositive(binary1) && IsInfinity(binary1)) || IsNaN(binary1)) {
                fcsr.NV = true;
                result = UInt32.MaxValue;

            } else if (IsNegative(binary1) && IsInfinity(binary1)) {
                fcsr.NV = true;
                result = UInt32.MinValue;

            } else if (UInt32.MaxValue < rvalue1) {
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
        /// Floating-Point Convert to Double from Word命令
        /// 整数レジスタrs1(符号付き)の整数を倍精度浮動小数に変換して、浮動小数点レジスタrdに書き込む
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">レジスタ番号</param>
        /// <returns>処理の成否</returns>
        public bool FcvtDW(FPRegister rd, Register rs1, FloatRoundingMode frm, UInt32 insLength = 4u) {
            Int32 value1 = (Int32)reg.GetValue(rs1);
            Binary64 result;
            FloatCSR fcsr = 0;

            result = ToBinary((Double)value1);

            if ((Int32)ToDouble(result) != value1) {
                fcsr.NX = true;
            }

            reg.SetValue(rd, result);
            reg.SetFflagsCSR(fcsr);
            reg.IncrementPc(insLength);
            return true;
        }

        /// <summary>
        /// Floating-Point Convert to Double from Unsigned Word命令
        /// 整数レジスタrs1(符号なし)の整数を倍精度浮動小数に変換して、浮動小数点レジスタrdに書き込む
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">レジスタ番号</param>
        /// <returns>処理の成否</returns>
        public bool FcvtDWU(FPRegister rd, Register rs1, FloatRoundingMode frm, UInt32 insLength = 4u) {
            UInt32 value1 = reg.GetValue(rs1);
            Binary64 result;
            FloatCSR fcsr = 0;

            result = ToBinary((Double)value1);

            if ((UInt32)ToDouble(result) != value1) {
                fcsr.NX = true;
            }

            reg.SetValue(rd, result);
            reg.SetFflagsCSR(fcsr);
            reg.IncrementPc(insLength);
            return true;
        }

        /// <summary>
        /// Floating-Point Classigy, Double-Precision命令
        /// 浮動小数点レジスタrs1のクラスを示すマスクを整数レジスタrdに書き込む
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">レジスタ番号</param>
        /// <returns>処理の成否</returns>
        public bool FclassD(Register rd, FPRegister rs1, UInt32 insLength = 4u) {
            Binary64 binary1 = reg.GetValue(rs1);
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

        internal static bool IsZero(Binary64 binary) => (binary & (ExpMask | MantMask)) == Zero;

        internal static bool IsDenormalNum(Binary64 binary) => (binary & ExpMask) == 0u && !IsZero(binary);
        internal static bool IsNormalNum(Binary64 binary) => (binary & ExpMask) > 0u && (binary & ExpMask) < 0x7ff0_0000_0000_0000u;

        internal static bool IsNaN(Binary64 binary) => (binary & ExpMask) == 0x7ff0_0000_0000_0000u && (binary & MantMask) > 0u;
        internal static bool IsSigNaN(Binary64 binary) => IsNaN(binary) && (binary & 0x0008_0000_0000_0000u) == 0x0000_0000_0000_0000u;
        internal static bool IsQuietNaN(Binary64 binary) => IsNaN(binary) && (binary & 0x0008_0000_0000_0000u) == 0x0008_0000_0000_0000u;

        internal static bool IsInfinity(Binary64 binary) => (binary & (ExpMask | MantMask)) == Infinity;

        internal static bool IsNegative(Binary64 binary) => (binary & SignMask) == NegativeSign;
        internal static bool IsPositive(Binary64 binary) => !IsNegative(binary);

        internal static bool IsRounded(Binary64 base_binary, Binary64 binary) {
            bool result = true;

            // binary1とbinary2の指数の差を求める
            int exp_sa = (int)((base_binary & ExpMask) >> 52) - (int)((binary & ExpMask) >> 52);

            // 差が+の場合は仮数が右シフト、-の場合は左シフトされるので、シフトして消える部分に数値がある場合は誤差ありとする
            //                                                            -0x10_0000_0000_0000 = 0xfff0_0000_0000_0000
            Binary64 mask = (exp_sa > 0 ? ((1ul << exp_sa) - 1) : (ulong)(-0x10_0000_0000_0000 >> -exp_sa));
            result = ((binary & MantMask) & mask) > 0;
            return result;
        }

        /// <summary>
        /// 64bit長バイナリ形式から倍精度浮動小数点数に変換する
        /// </summary>
        /// <param name="binary">バイナリ形式の値</param>
        /// <returns>変換した倍精度浮動小数点数</returns>
        internal static Double ToDouble(Binary64 binary) => BitConverter.ToDouble(BitConverter.GetBytes(binary), 0);

        /// <summary>
        /// 倍精度浮動小数点数を64bit長バイナリ形式に変換する
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static Binary64 ToBinary(Double value) => BitConverter.ToUInt64(BitConverter.GetBytes(value), 0);

        /// <summary>
        /// 値を指定した桁数、丸めモードで丸める
        /// </summary>
        /// <param name="binary">丸めるバイナリ形式の値</param>
        /// <param name="digits">桁数</param>
        /// <param name="frm">丸めモード</param>
        /// <returns>丸めたバイナリ形式の値</returns>
        private static Binary64 RoundNum(Binary64 binary, int digits, FloatRoundingMode frm) {
            Binary64 result;
            Double coef;
            switch (frm) {
                case FloatRoundingMode.RNE: // 最近接丸め(偶数)
                    result = ToBinary((Double)Math.Round(ToDouble(binary), digits, MidpointRounding.ToEven));
                    break;

                case FloatRoundingMode.RTZ: // 0への丸め
                    coef = (Double)Math.Pow(10, digits);
                    Double sign = IsNegative(binary) ? -1f : 1f;
                    result = ToBinary(sign * (Double)(Math.Floor(sign * ToDouble(binary) * coef) / coef));
                    break;

                case FloatRoundingMode.RDN: // 切り下げ
                    coef = (Double)Math.Pow(10, digits);
                    result = ToBinary((Double)(Math.Ceiling(ToDouble(binary) * coef) / coef));
                    break;

                case FloatRoundingMode.RUP: // 切り上げ
                    coef = (Double)Math.Pow(10, digits);
                    result = ToBinary((Double)(Math.Floor(ToDouble(binary) * coef) / coef));
                    break;

                case FloatRoundingMode.RMM: // 最近接丸め(0から遠くへの丸め)
                    result = ToBinary((Double)Math.Round(ToDouble(binary), digits, MidpointRounding.AwayFromZero));
                    break;

                default:
                    result = 0u;
                    break;
            }

            return result;
        }
    }
}
