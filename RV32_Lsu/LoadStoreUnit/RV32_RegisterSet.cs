using RiscVCpu.Constants;
using RiscVCpu.Constants.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RiscVCpu.LoadStoreUnit {
    public class RV32_RegisterSet {
        /// <summary>
        /// レジスタを表す32bit符号付き整数の連想配列
        /// RV32Iアーキテクチャでは32+PCの33個の要素から構成される
        /// </summary>
        private readonly Dictionary<Register, UInt32> RegisterArray;

        private readonly Dictionary<CSR, UInt32> CSRArray;

        private UInt32 pc;
        private PrivilegeLevels currentMode;

        /// <summary>
        /// RV32IアーキテクチャCPUのレジスタ群を表すクラス
        /// </summary>
        public RV32_RegisterSet() {

            // レジスタ初期化
            RegisterArray = new Dictionary<Register, UInt32>(Enumerable.Range(0, 32).ToDictionary(key => (Register)key, value => 0U));

            // コントロール・ステータスレジスタ初期化
            CSRArray = Enum.GetValues(typeof(CSR)).Cast<CSR>().ToDictionary(key => key, value => 0U);
            
            //マシンモードに設定
            currentMode = PrivilegeLevels.MachineMode;

            CSRArray[CSR.misa] |= 0x40000000;
        }

        /// <summary>
        /// プログラムカウンタ
        /// </summary>
        public UInt32 PC { get => pc; }
        /// <summary>
        /// 現在の実行モード
        /// </summary>
        public PrivilegeLevels CurrentMode { get => currentMode; set => currentMode = value; }

        /// <summary>
        /// レジスタを全てクリアし、エントリポイントをPCに設定する
        /// </summary>
        /// <param name="entryPoint"></param>
        public void ClearAll() {
            RegisterArray.Select(r => RegisterArray[r.Key] = 0);
            CSRArray.Select(c => CSRArray[c.Key] = 0);

            CSRArray[CSR.misa] |= 0x40000000;
        }

        public void IncrementCycle() {
            CSRArray[CSR.cycle]++;
        }

        /// <summary>
        /// プログラムカウントを 4 増加させ、次の命令アドレスを指すように更新する
        /// </summary>
        public void IncrementPc() {
            pc += 4U;
        }

        /// <summary>
        /// プログラムカウンタに値を設定する
        /// これはLSU内からしかアクセスできない
        /// </summary>
        /// <param name="value"></param>
        internal void SetPc(UInt32 value) {
            pc = value;
        }

        /// <summary>
        /// レジスタに値を設定する
        /// </summary>
        /// <param name="name">設定する対象レジスタ名</param>
        /// <param name="value">設定する値</param>
        public void SetValue(Register name, UInt32 value) {
            if (name == Register.x0) {
                //x0レジスタは常に0から変更されないため、なにもしない

            } else {
                //値をレジスタに設定
                RegisterArray[name] = value;
            }
        }

        /// <summary>
        /// レジスタから値を取得する
        /// </summary>
        /// <param name="name">取得する対象レジスタ名</param>
        /// <returns>レジスタの値</returns>
        public UInt32 GetValue(Register name) {
            //レジスタの値を返す
            return RegisterArray[name];
        }


        /// <summary>
        /// CSRに値を設定する
        /// </summary>
        /// <param name="name">設定する対象レジスタ名</param>
        /// <param name="value">設定する値</param>
        internal void SetCSR(CSR name, char operation, Register reg) {

            //レジスタがx0の場合は何もしない
            if (reg != Register.x0) {
                SetCSR(name, operation, (UInt32)RegisterArray[reg]);
            }
        }

        /// <summary>
        /// CSRに値を設定する
        /// </summary>
        /// <param name="name">設定する対象レジスタ名</param>
        /// <param name="value">設定する値</param>
        internal void SetCSR(CSR name, char operation, UInt32 value) {

            //CSRアドレスの11～12bitで読み書き可能 or 読み取り専用を
            //9～10bitがアクセス権限を表す
            if (((UInt16)name & 0xC00u) != 0xC00u && (PrivilegeLevels)((UInt16)name & 0x300u) <= currentMode) {
                switch (operation) {
                    case 'w':
                        if ((CSR)name == CSR.sstatus) {
                            StatusCSR status = (StatusCSR)value;
                            status.Mode = currentMode;
                            CSRArray[name] = (CSRArray[name] & ~StatusCSR.SModeCanRead) | (UInt32)status;
                        } else {
                            CSRArray[name] = value;
                        }
                        break;
                    case 's':
                        if ((CSR)name == CSR.sstatus) {
                            StatusCSR status = (StatusCSR)value;
                            status.Mode = currentMode;
                            CSRArray[name] |= StatusCSR.SModeCanRead & (UInt32)status;
                        } else {
                            CSRArray[name] |= value;
                        }
                        break;
                    case 'c':
                        CSRArray[name] &= ~value;
                        break;
                    default:
                        throw new RiscvException(RiscvExceptionCause.IllegalInstruction);
                }
            } else {
                //実行モードがアドレスの権限より小さい場合
                //アドレス上位2bitが 0b11(= 読み取り専用)の場合は
                //不正命令例外が発生する
                throw new RiscvException(RiscvExceptionCause.IllegalInstruction);
            }
        }

        /// <summary>
        /// CSRから値を取得する
        /// </summary>
        /// <param name="name">取得する対象レジスタ名</param>
        /// <returns>CSRの値</returns>
        internal UInt32 GetCSR(CSR name) {
            //CSRアドレスの9～10bitがアクセス権限を表す
            if ((PrivilegeLevels)((UInt16)name & 0x300u) <= currentMode) {

                // 一部スーパーバイザーモードCSRにはマシンモードCSRに読み替える, sie, sieはsstatus, sie, sieは
                if ((CSR)name == CSR.sstatus) {
                    // []status CSR
                    StatusCSR status = (StatusCSR)CSRArray[name | (CSR)(PrivilegeLevels.MachineMode - currentMode)];
                    status.Mode = currentMode;
                    return (UInt32)status;
                } else if ((CSR)name == CSR.sie || (CSR)name == CSR.sip) {
                    // マシンモードCSRに読み替える
                    InterruptPendingCSR interrupt = (InterruptPendingCSR)CSRArray[name | (CSR)0x200U];
                    interrupt.Mode = currentMode;
                    return (UInt32)interrupt;
                } else {
                    return (UInt32)CSRArray[name];
                }

            } else {
                //実行モードがアドレスの権限より小さい場合は不正命令例外が発生する
                throw new RiscvException(RiscvExceptionCause.IllegalInstruction);
            }
        }

        /// <summary>
        /// マシンの拡張命令セットを表すCSRに設定を追加する
        /// </summary>
        /// <param name="isa">拡張命令セットを表すA～Zの文字</param>
        public void AddMisa(char isa) {
            CSRArray[CSR.misa] += (UInt32)(1 << (Char.ToUpper(isa) - 'A' - 1));
        }

        /// <summary>
        /// レジスタ(x0～x31 + pc)の内容をレジスタ名と16進形式の文字列に変換して返す
        /// </summary>
        /// <returns>レジスタ状態を表す文字列</returns>
        public override string ToString() {
            string s = "";
            foreach (Register rName in Enumerable.Range(0, 32)) {
                s += ("x" + (byte)rName).PadLeft(3) + ": " + BitConverter.ToString(BitConverter.GetBytes(GetValue(rName))) + "\n";
            }

            s += " pc: " + BitConverter.ToString(BitConverter.GetBytes(PC)) + "\n";

            return s;
        }
    }
}