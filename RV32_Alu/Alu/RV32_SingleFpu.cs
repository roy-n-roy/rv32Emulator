using RiscVCpu.LoadStoreUnit;
using RiscVCpu.LoadStoreUnit.Constants;
using RiscVCpu.LoadStoreUnit.Exceptions;
using RiscVCpu.RegisterSet;
using System;

namespace RiscVCpu.ArithmeticLogicUnit {

    /// <summary>
    /// Risc-V RV32I 単精度浮動小数点命令セット 算術論理演算命令を実行するFPU
    /// </summary>
    public class RV32_SingleFpu : RV32_AbstractCalculator {

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
        public bool FmaddS(FPRegister rd, FPRegister rs1, FPRegister rs2, FPRegister rs3, UInt32 insLength = 4u) {
            Decimal value1 = (Decimal)reg.GetSingleValue(rs1);
            Decimal value2 = (Decimal)reg.GetSingleValue(rs2);
            Decimal value3 = (Decimal)reg.GetSingleValue(rs3);

            reg.SetValue(rd, (Single)((value1 * value2) + value3));
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
        public bool FmsubS(FPRegister rd, FPRegister rs1, FPRegister rs2, FPRegister rs3, UInt32 insLength = 4u) {
            Decimal value1 = (Decimal)reg.GetSingleValue(rs1);
            Decimal value2 = (Decimal)reg.GetSingleValue(rs2);
            Decimal value3 = (Decimal)reg.GetSingleValue(rs3);

            reg.SetValue(rd, (Single)((value1 * value2) - value3));
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
        public bool FnmaddS(FPRegister rd, FPRegister rs1, FPRegister rs2, FPRegister rs3, UInt32 insLength = 4u) {
            Decimal value1 = (Decimal)reg.GetSingleValue(rs1);
            Decimal value2 = (Decimal)reg.GetSingleValue(rs2);
            Decimal value3 = (Decimal)reg.GetSingleValue(rs3);

            reg.SetValue(rd, (Single)((-value1 * value2) + value3));
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
        public bool FnmsubS(FPRegister rd, FPRegister rs1, FPRegister rs2, FPRegister rs3, UInt32 insLength = 4u) {
            Decimal value1 = (Decimal)reg.GetSingleValue(rs1);
            Decimal value2 = (Decimal)reg.GetSingleValue(rs2);
            Decimal value3 = (Decimal)reg.GetSingleValue(rs3);

            reg.SetValue(rd, (Single)((-value1 * value2) - value3));
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
        public bool FaddS(FPRegister rd, FPRegister rs1, FPRegister rs2, UInt32 insLength = 4u) {
            reg.SetValue(rd, reg.GetSingleValue(rs1) + reg.GetSingleValue(rs2));
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
        public bool FsubS(FPRegister rd, FPRegister rs1, FPRegister rs2, UInt32 insLength = 4u) {
            reg.SetValue(rd, reg.GetSingleValue(rs1) - reg.GetSingleValue(rs2));
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
        public bool FmulS(FPRegister rd, FPRegister rs1, FPRegister rs2, UInt32 insLength = 4u) {
            reg.SetValue(rd, reg.GetSingleValue(rs1) * reg.GetSingleValue(rs2));
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
        public bool FdivS(FPRegister rd, FPRegister rs1, FPRegister rs2, UInt32 insLength = 4u) {
            Single value1 = reg.GetSingleValue(rs1);
            Single value2 = reg.GetSingleValue(rs2);

            if (value1 == 0f && value2 == 0f) {
                reg.SetValue(rd, Single.NaN);

            }else if(value2 == 0f) {
                reg.SetValue(rd, Single.PositiveInfinity);

            } else {
                reg.SetValue(rd, value1 / value2);
            }
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
        public bool FsqrtS(FPRegister rd, FPRegister rs1, UInt32 insLength = 4u) {
            reg.SetValue(rd, (Single)Math.Sqrt(reg.GetSingleValue(rs1)));
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
            byte[] value1 = BitConverter.GetBytes(reg.GetSingleValue(rs1));
            byte[] value2 = BitConverter.GetBytes(reg.GetSingleValue(rs2));

            value1[3] = (byte)((value1[3] & 0x7fu) | (value2[3] & 0x80u));

            reg.SetValue(rd, BitConverter.ToSingle(value1, 0));
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
            byte[] value1 = BitConverter.GetBytes(reg.GetSingleValue(rs1));
            byte[] value2 = BitConverter.GetBytes(reg.GetSingleValue(rs2));

            value1[3] = (byte)((value1[3] & 0x7fu) | (~value2[3] & 0x80u));

            reg.SetValue(rd, BitConverter.ToSingle(value1, 0));
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
            byte[] value1 = BitConverter.GetBytes(reg.GetSingleValue(rs1));
            byte[] value2 = BitConverter.GetBytes(reg.GetSingleValue(rs2));

            value1[3] = (byte)((value1[3] & 0x7fu) | ((byte)(value1[3] ^ value2[3]) & 0x80u));

            reg.SetValue(rd, BitConverter.ToSingle(value1, 0));
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
            Single value1 = reg.GetSingleValue(rs1);
            Single value2 = reg.GetSingleValue(rs2);

            reg.SetValue(rd, value1 < value2 ? value1 : value2);
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
            Single value1 = reg.GetSingleValue(rs1);
            Single value2 = reg.GetSingleValue(rs2);

            reg.SetValue(rd, value1 > value2 ? value1 : value2);
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
            reg.SetValue(rd, (reg.GetSingleValue(rs1) == reg.GetSingleValue(rs2) ? 1U : 0U));
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
            reg.SetValue(rd, (reg.GetSingleValue(rs1) < reg.GetSingleValue(rs2) ? 1U : 0U));
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
            reg.SetValue(rd, (reg.GetSingleValue(rs1) <= reg.GetSingleValue(rs2) ? 1U : 0U));
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
        public bool FcvtWS(Register rd, FPRegister rs1, UInt32 insLength = 4u) {
            reg.SetValue(rd, (UInt32)(Int32)reg.GetSingleValue(rs1));
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
        public bool FcvtWUS(Register rd, FPRegister rs1, UInt32 insLength = 4u) {
            reg.SetValue(rd, (UInt32)reg.GetSingleValue(rs1));
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
        public bool FcvtSW(FPRegister rd, Register rs1, UInt32 insLength = 4u) {
            reg.SetValue(rd, (Single)(Int32)reg.GetValue(rs1));
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
        public bool FcvtSWU(FPRegister rd, Register rs1, UInt32 insLength = 4u) {
            reg.SetValue(rd, (Single)reg.GetValue(rs1));
            reg.IncrementPc(insLength);
            return true;
        }

        /// <summary>
        /// Floating-Point Move to Integer from Word命令
        /// 整数レジスタrs1を浮動小数点レジスタrd(符号付き)に書き込む
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">レジスタ番号</param>
        /// <returns>処理の成否</returns>
        public bool FmvXW(Register rd, FPRegister rs1, UInt32 insLength = 4u) {
            reg.SetValue(rd, BitConverter.ToUInt32(BitConverter.GetBytes(reg.GetSingleValue(rs1)), 0));
            reg.IncrementPc(insLength);
            return true;
        }

        /// <summary>
        /// Floating-Point Move to Word from Integer命令
        /// 浮動小数点レジスタrs1(符号付き)を整数レジスタrdに書き込む
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">レジスタ番号</param>
        /// <returns>処理の成否</returns>
        public bool FmvWX(FPRegister rd, Register rs1, UInt32 insLength = 4u) {
            reg.SetValue(rd, BitConverter.ToSingle(BitConverter.GetBytes(reg.GetValue(rs1)), 0));
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
            Single value1 = reg.GetSingleValue(rs1);
            UInt32 mask = 0;

            if (Single.IsNegativeInfinity(value1)) {
                // rs1が-∞の場合
                mask |= 0b00_0000_0001;

            } else if (-Math.Pow(2, 127) <= value1 && value1 <= -Math.Pow(2, -126)) {
                // rs1が負の正規数の場合
                mask |= 0b00_0000_1000;

            } else if ((Single.NegativeInfinity < value1 && value1 < -Math.Pow(2, 127)) || (-Math.Pow(2, -126) < value1 && value1 <= -Epsilon())) {
                // rs1が負の非正規数の場合
                mask |= 0b00_0000_1000;

            } else if (value1 == -0f) {
                // rs1が-0の場合
                mask |= 0b00_0000_1000;

            } else if (value1 == 0f) {
                // rs1が+0の場合
                mask |= 0b00_0001_0000;

            } else if ((Epsilon() <= value1 && value1 < Math.Pow(2, -126)) || (Math.Pow(2, 127) < value1 && value1 < Single.NegativeInfinity)) {
                // rs1が正の非正規数の場合
                mask |= 0b00_0010_0000;

            } else if (Math.Pow(2, -126) <= value1 && value1 <= -Math.Pow(2, 127)) {
                // rs1が正の正規数の場合
                mask |= 0b00_0100_0000;

            } else if (Single.IsPositiveInfinity(value1)) {
                // rs1が+∞の場合
                mask |= 0b00_1000_0000;

            } else if (Single.IsNaN(value1)) {
                // rs1がシグナル型非数の場合
                mask |= 0b01_0000_0000;

            } else if (Single.IsNaN(value1)) {
                // rs1がクワイエット型非数の場合
                mask |= 0b10_0000_0000;
            }

            reg.SetValue(rd, mask);
            reg.IncrementPc(insLength);
            return true;
        }

        #endregion

        #endregion

        /// <summary>
        /// 正の最小値を求める
        /// </summary>
        /// <returns></returns>
        static Single Epsilon() {
            Single eps = 1.0f;
            while (1.0f + (eps / 2.0f) > 1.0f) {
                eps /= 2.0f;
            }
            return eps;
        }

    }
}
