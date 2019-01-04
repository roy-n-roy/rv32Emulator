using RiscVCpu.LoadStoreUnit;
using RiscVCpu.LoadStoreUnit.Constants;
using RiscVCpu.LoadStoreUnit.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RiscVCpu.RegisterSet {
    /// <summary>
    /// RV32アーキテクチャのレジスタ
    /// 実データを扱う唯一のクラス
    /// </summary>
    public class RV32_RegisterSet {

        /// <summary>
        /// レジスタを表す32bit符号なし整数の連想配列
        /// RV32Iアーキテクチャでは32個の要素から構成される
        /// </summary>
        private readonly Dictionary<Register, UInt32> Registers;

        /// <summary>
        /// 浮動小数点レジスタを表す64bit符号なし整数の連想配列
        /// RV32FDアーキテクチャでは32個の要素から構成される
        /// </summary>
        private readonly Dictionary<FPRegister, UInt64> FPRegisters;

        /// <summary>
        /// コントロール・ステータスレジスタを表す32bit符号なし整数の連想配列
        /// </summary>
        private readonly Dictionary<CSR, UInt32> CSRArray;
        
        private UInt32 programCounter;
        private UInt32 instructionRegister;


        /// <summary>現在の実行モード</summary>
        private PrivilegeLevels currentMode;

        /// <summary>
        /// RV32IアーキテクチャCPUのレジスタ群を表すクラス
        /// </summary>
        public RV32_RegisterSet() {

            // レジスタ初期化
            Registers = new Dictionary<Register, UInt32>(Enumerable.Range(0, 32).ToDictionary(key => (Register)key, value => 0u));
            FPRegisters = new Dictionary<FPRegister, UInt64>(Enumerable.Range(0, 32).ToDictionary(key => (FPRegister)key, value => BitConverter.ToUInt64(BitConverter.GetBytes(0d), 0)));

            // コントロール・ステータスレジスタ初期化
            CSRArray = Enum.GetValues(typeof(CSR)).Cast<CSR>().ToDictionary(key => key, value => 0u);
            
            //マシンモードに設定
            currentMode = PrivilegeLevels.MachineMode;

            CSRArray[CSR.misa] |= 0x40000000;
        }

        /// <summary>プログラムカウンタ</summary>
        public UInt32 PC { get => programCounter; }

        /// <summary>命令レジスタ</summary>
        public UInt32 IR { get => instructionRegister; }

        /// <summary>
        /// 現在の実行モード
        /// </summary>
        public PrivilegeLevels CurrentMode { get => currentMode; set => currentMode = value; }

        /// <summary>
        /// レジスタを全て初期化する
        /// </summary>
        public void ClearAll() {
            Registers.Select(r => Registers[r.Key] = 0);
            CSRArray.Select(c => CSRArray[c.Key] = 0);

            currentMode = PrivilegeLevels.MachineMode;
            CSRArray[CSR.misa] |= 0x40000000;
        }


        /// <summary>
        /// サイクルカウントを 1 増加させる
        /// </summary>
        public void IncrementCycle() => CSRArray[CSR.cycle]++;

        /// <summary>
        /// プログラムカウントを 引数で指定した分 増加させ、次の命令アドレスを指すように更新する
        /// 引数で指定しない場合は 4 増加させる
        /// </summary>
        /// <param name="instructionLength">PCの増数 32bit長命令の場合は4、16bit長命令の場合は2を指定する</param>
        public void IncrementPc(UInt32 instructionLength = 4u) {
            programCounter += instructionLength;
        }

        /// <summary>
        /// プログラムカウンタに値を設定する
        /// これはLSU内からしかアクセスできない
        /// </summary>
        /// <param name="value"></param>
        internal void SetPc(UInt32 value) {
            programCounter = value;
            instructionRegister = 0;
        }

        /// <summary>
        /// 整数レジスタに値を設定する
        /// </summary>
        /// <param name="name">設定する対象レジスタ名</param>
        /// <param name="value">設定する値</param>
        public void SetValue(Register name, UInt32 value) {
            if (name == Register.zero) {
                //zeroレジスタは常に0から変更されないため、なにもしない

            } else {
                //値をレジスタに設定
                Registers[name] = value;
            }
        }

        /// <summary>
        /// 整数レジスタから値を取得する
        /// </summary>
        /// <param name="name">取得する対象レジスタ名</param>
        /// <returns>レジスタの値</returns>
        public UInt32 GetValue(Register name) {
            //レジスタの値を返す
            return Registers[name];
        }

        /// <summary>
        /// 浮動小数点レジスタに値を設定する
        /// </summary>
        /// <param name="name">設定する対象レジスタ名</param>
        /// <param name="value">設定する値</param>
        public void SetValue(FPRegister name, Single value) {
            //値をレジスタに設定
            FPRegisters[name] = BitConverter.ToUInt64(BitConverter.GetBytes(value).Concat(new byte[] { 0, 0, 0, 0 }).ToArray(), 0);
        }

        /// <summary>
        /// 浮動小数点レジスタに値を設定する
        /// </summary>
        /// <param name="name">設定する対象レジスタ名</param>
        /// <param name="value">設定する値</param>
        public void SetValue(FPRegister name, Double value) {
            //値をレジスタに設定
            FPRegisters[name] = BitConverter.ToUInt64(BitConverter.GetBytes(value), 0);
        }

        /// <summary>
        /// 浮動小数点レジスタから値を取得する
        /// </summary>
        /// <param name="name">取得する対象レジスタ名</param>
        /// <returns>レジスタの値</returns>
        public Single GetSingleValue(FPRegister name) {
            //レジスタの値を返す
            return BitConverter.ToSingle(BitConverter.GetBytes(FPRegisters[name]), 0);
        }

        /// <summary>
        /// 浮動小数点レジスタから値を取得する
        /// </summary>
        /// <param name="name">取得する対象レジスタ名</param>
        /// <returns>レジスタの値</returns>
        public Double GetDoubleValue(FPRegister name) {
            //レジスタの値を返す
            return BitConverter.ToDouble(BitConverter.GetBytes(FPRegisters[name]), 0);
        }


        /// <summary>
        /// CSRに値を設定する
        /// </summary>
        /// <param name="name">設定する対象レジスタ名</param>
        /// <param name="value">設定する値</param>
        internal void SetCSR(CSR name, char operation, Register reg) {

            //レジスタがzeroの場合は何もしない
            if (reg != Register.zero) {
                SetCSR(name, operation, (UInt32)Registers[reg]);
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
                        throw new RiscvException(RiscvExceptionCause.IllegalInstruction, this);
                }
            } else {
                //実行モードがアドレスの権限より小さい場合
                //アドレス上位2bitが 0b11(= 読み取り専用)の場合は
                //不正命令例外が発生する
                throw new RiscvException(RiscvExceptionCause.IllegalInstruction, this);
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
                    InterruptPendingCSR interrupt = (InterruptPendingCSR)CSRArray[name | (CSR)0x200u];
                    interrupt.Mode = currentMode;
                    return (UInt32)interrupt;
                } else {
                    return (UInt32)CSRArray[name];
                }

            } else {
                //実行モードがアドレスの権限より小さい場合は不正命令例外が発生する
                throw new RiscvException(RiscvExceptionCause.IllegalInstruction, this);
            }
        }

        /// <summary>
        /// マシンの拡張命令セットを表すCSRに設定を追加する
        /// </summary>
        /// <param name="isa">拡張命令セットを表すA～Zの文字</param>
        public void AddMisa(char isa) {
            CSRArray[CSR.misa] |= (UInt32)(1 << (Char.ToUpper(isa) - 'A' - 1));

            // IAMAFDをサポートする場合、Gもセットする
            if((CSRArray[CSR.misa] & 0x1129) == 0x1129) {
                CSRArray[CSR.misa] |= 5u;
            }
        }

        public void SetCause(RiscvExceptionCause cause) {
            switch (currentMode) {
                case PrivilegeLevels.MachineMode:
                    CSRArray[CSR.mcause] = (UInt32)cause;
                    break;
                case PrivilegeLevels.SupervisorMode:
                    CSRArray[CSR.scause] = (UInt32)cause;
                    break;
                case PrivilegeLevels.UserMode:
                    CSRArray[CSR.ucause] = (UInt32)cause;
                    break;
            }
        }


        /// <summary>
        /// レジスタ(zero～x31 + pc)の内容をレジスタ名と16進形式の文字列に変換して返す
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