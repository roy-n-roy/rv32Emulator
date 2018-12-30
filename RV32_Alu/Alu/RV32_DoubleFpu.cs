using RiscVCpu.LoadStoreUnit;
using RiscVCpu.LoadStoreUnit.Constants;
using RiscVCpu.LoadStoreUnit.Exceptions;
using RiscVCpu.RegisterSet;
using System;

namespace RiscVCpu.ArithmeticLogicUnit {

    /// <summary>
    /// Risc-V RV32I 倍精度浮動小数点命令セット 算術論理演算命令を実行するFPU
    /// </summary>
    public class RV32_DoubleFpu : RV32_AbstractCalculator {

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
        public bool FmaddD(FPRegister rd, FPRegister rs1, FPRegister rs2, FPRegister rs3, UInt32 insLength = 4u) {
            Decimal value1 = (Decimal)reg.GetDoubleValue(rs1);
            Decimal value2 = (Decimal)reg.GetDoubleValue(rs2);
            Decimal value3 = (Decimal)reg.GetDoubleValue(rs3);

            reg.SetValue(rd, (Double)((value1 * value2) + value3));
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
        public bool FmsubD(FPRegister rd, FPRegister rs1, FPRegister rs2, FPRegister rs3, UInt32 insLength = 4u) {
            Decimal value1 = (Decimal)reg.GetDoubleValue(rs1);
            Decimal value2 = (Decimal)reg.GetDoubleValue(rs2);
            Decimal value3 = (Decimal)reg.GetDoubleValue(rs3);

            reg.SetValue(rd, (Double)((value1 * value2) - value3));
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
        public bool FnmaddD(FPRegister rd, FPRegister rs1, FPRegister rs2, FPRegister rs3, UInt32 insLength = 4u) {
            Decimal value1 = (Decimal)reg.GetDoubleValue(rs1);
            Decimal value2 = (Decimal)reg.GetDoubleValue(rs2);
            Decimal value3 = (Decimal)reg.GetDoubleValue(rs3);

            reg.SetValue(rd, (Double)((-value1 * value2) + value3));
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
        public bool FnmsubD(FPRegister rd, FPRegister rs1, FPRegister rs2, FPRegister rs3, UInt32 insLength = 4u) {
            Decimal value1 = (Decimal)reg.GetDoubleValue(rs1);
            Decimal value2 = (Decimal)reg.GetDoubleValue(rs2);
            Decimal value3 = (Decimal)reg.GetDoubleValue(rs3);

            reg.SetValue(rd, (Double)((-value1 * value2) - value3));
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
        public bool FaddD(FPRegister rd, FPRegister rs1, FPRegister rs2, UInt32 insLength = 4u) {
            reg.SetValue(rd, reg.GetDoubleValue(rs1) + reg.GetDoubleValue(rs2));
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
        public bool FsubD(FPRegister rd, FPRegister rs1, FPRegister rs2, UInt32 insLength = 4u) {
            reg.SetValue(rd, reg.GetDoubleValue(rs1) - reg.GetDoubleValue(rs2));
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
        public bool FmulD(FPRegister rd, FPRegister rs1, FPRegister rs2, UInt32 insLength = 4u) {
            reg.SetValue(rd, reg.GetDoubleValue(rs1) * reg.GetDoubleValue(rs2));
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
        public bool FdivD(FPRegister rd, FPRegister rs1, FPRegister rs2, UInt32 insLength = 4u) {
            Double value1 = reg.GetDoubleValue(rs1);
            Double value2 = reg.GetDoubleValue(rs2);

            if (value1 == 0f && value2 == 0f) {
                reg.SetValue(rd, Double.NaN);

            }else if(value2 == 0f) {
                reg.SetValue(rd, Double.PositiveInfinity);

            } else {
                reg.SetValue(rd, value1 / value2);
            }
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
        public bool FsqrtD(FPRegister rd, FPRegister rs1, UInt32 insLength = 4u) {
            reg.SetValue(rd, (Double)Math.Sqrt(reg.GetDoubleValue(rs1)));
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
            byte[] value1 = BitConverter.GetBytes(reg.GetDoubleValue(rs1));
            byte[] value2 = BitConverter.GetBytes(reg.GetDoubleValue(rs2));

            value1[7] = (byte)((value1[7] & 0x7fu) | (value2[7] & 0x80u));

            reg.SetValue(rd, BitConverter.ToDouble(value1, 0));
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
            byte[] value1 = BitConverter.GetBytes(reg.GetDoubleValue(rs1));
            byte[] value2 = BitConverter.GetBytes(reg.GetDoubleValue(rs2));

            value1[7] = (byte)((value1[7] & 0x7fu) | (~value2[7] & 0x80u));

            reg.SetValue(rd, BitConverter.ToDouble(value1, 0));
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
            byte[] value1 = BitConverter.GetBytes(reg.GetDoubleValue(rs1));
            byte[] value2 = BitConverter.GetBytes(reg.GetDoubleValue(rs2));

            value1[7] = (byte)((value1[7] & 0x7fu) | ((byte)(value1[7] ^ value2[7]) & 0x80u));

            reg.SetValue(rd, BitConverter.ToDouble(value1, 0));
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
            Double value1 = reg.GetDoubleValue(rs1);
            Double value2 = reg.GetDoubleValue(rs2);

            reg.SetValue(rd, value1 < value2 ? value1 : value2);
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
            Double value1 = reg.GetDoubleValue(rs1);
            Double value2 = reg.GetDoubleValue(rs2);

            reg.SetValue(rd, value1 > value2 ? value1 : value2);
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
            reg.SetValue(rd, (reg.GetDoubleValue(rs1) == reg.GetDoubleValue(rs2) ? 1U : 0U));
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
            reg.SetValue(rd, (reg.GetDoubleValue(rs1) < reg.GetDoubleValue(rs2) ? 1U : 0U));
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
            reg.SetValue(rd, (reg.GetDoubleValue(rs1) <= reg.GetDoubleValue(rs2) ? 1U : 0U));
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
        public bool FcvtSD(FPRegister rd, FPRegister rs1, UInt32 insLength = 4u) {
            reg.SetValue(rd, (Single)reg.GetDoubleValue(rs1));
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
        public bool FcvtDS(FPRegister rd, FPRegister rs1, UInt32 insLength = 4u) {
            reg.SetValue(rd, (Double)reg.GetSingleValue(rs1));
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
        public bool FcvtWD(Register rd, FPRegister rs1, UInt32 insLength = 4u) {
            reg.SetValue(rd, (UInt32)(Int32)reg.GetDoubleValue(rs1));
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
        public bool FcvtWUD(Register rd, FPRegister rs1, UInt32 insLength = 4u) {
            reg.SetValue(rd, (UInt32)reg.GetDoubleValue(rs1));
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
        public bool FcvtDW(FPRegister rd, Register rs1, UInt32 insLength = 4u) {
            reg.SetValue(rd, (Double)(Int32)reg.GetValue(rs1));
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
        public bool FcvtDWU(FPRegister rd, Register rs1, UInt32 insLength = 4u) {
            reg.SetValue(rd, (Double)reg.GetValue(rs1));
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
            Double value1 = reg.GetDoubleValue(rs1);
            UInt32 mask = 0;

            if (Double.IsNegativeInfinity(value1)) {
                // rs1が-∞の場合
                mask |= 0b00_0000_0001;

            } else if (-Math.Pow(2, 127) <= value1 && value1 <= -Math.Pow(2, -126)) {
                // rs1が負の正規数の場合
                mask |= 0b00_0000_1000;

            } else if ((Double.NegativeInfinity < value1 && value1 < -Math.Pow(2, 127)) || (-Math.Pow(2, -126) < value1 && value1 <= -Epsilon())) {
                // rs1が負の非正規数の場合
                mask |= 0b00_0000_1000;

            } else if (value1 == -0f) {
                // rs1が-0の場合
                mask |= 0b00_0000_1000;

            } else if (value1 == 0f) {
                // rs1が+0の場合
                mask |= 0b00_0001_0000;

            } else if ((Epsilon() <= value1 && value1 < Math.Pow(2, -126)) || (Math.Pow(2, 127) < value1 && value1 < Double.NegativeInfinity)) {
                // rs1が正の非正規数の場合
                mask |= 0b00_0010_0000;

            } else if (Math.Pow(2, -126) <= value1 && value1 <= -Math.Pow(2, 127)) {
                // rs1が正の正規数の場合
                mask |= 0b00_0100_0000;

            } else if (Double.IsPositiveInfinity(value1)) {
                // rs1が+∞の場合
                mask |= 0b00_1000_0000;

            } else if (Double.IsNaN(value1)) {
                // rs1がシグナル型非数の場合
                mask |= 0b01_0000_0000;

            } else if (Double.IsNaN(value1)) {
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
        static Double Epsilon() {
            Double eps = 1.0f;
            while (1.0f + (eps / 2.0f) > 1.0f) {
                eps /= 2.0f;
            }
            return eps;
        }

    }
}
