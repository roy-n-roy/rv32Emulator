using RiscVCpu.LoadStoreUnit;
using RiscVCpu.LoadStoreUnit.Constants;
using RiscVCpu.LoadStoreUnit.Exceptions;
using RiscVCpu.MemoryHandler;
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
        internal readonly RV32_ControlStatusRegisters CSRegisters;

        private UInt32 programCounter;
        private UInt32 instructionRegister;
        private RV32_AbstractMemoryHandler mainMemory;

        /// <summary>現在の実行モード</summary>
        private PrivilegeLevels currentMode;

        #region コンストラクタ

        /// <summary>
        /// RV32IアーキテクチャCPUのレジスタ群を表すクラス
        /// </summary>
        public RV32_RegisterSet(RV32_AbstractMemoryHandler mem) {

            // レジスタ初期化
            Registers = Enumerable.Range(0, 32).ToDictionary(key => (Register)key, value => 0u);
            FPRegisters = Enumerable.Range(0, 32).ToDictionary(key => (FPRegister)key, value => BitConverter.ToUInt64(BitConverter.GetBytes(0d), 0));

            // コントロール・ステータスレジスタ初期化
            CSRegisters = new RV32_ControlStatusRegisters(Enum.GetValues(typeof(CSR)).Cast<CSR>().ToDictionary(key => key, value => 0u));

            mainMemory = mem;
            mainMemory.SetRegisterSet(this);

            //マシンモードに設定
            currentMode = PrivilegeLevels.MachineMode;

            CSRegisters.Misa |= 0x4000_0000u;

        }

        #endregion

        #region プロパティ

        /// <summary>プログラムカウンタ</summary>
        public UInt32 PC {
            get => programCounter;
            internal set {
                programCounter = value;
                instructionRegister = Mem.FetchInstruction(value);
            }
        }

        /// <summary>命令レジスタ</summary>
        public UInt32 IR {
            get => instructionRegister;
        }

        /// <summary>現在の実行モード</summary>
        public PrivilegeLevels CurrentMode {
            get => currentMode;
            internal set => currentMode = value;
        }

        /// <summary>メインメモリ</summary>
        public RV32_AbstractMemoryHandler Mem {
            get => mainMemory;
            set {
                mainMemory = value;
                mainMemory.SetRegisterSet(this);
            }
        }

        #endregion

        #region メソッド定義

        #region プログラムカウンタ

        /// <summary>
        /// プログラムカウントを 引数で指定した分 増加させ、次の命令アドレスを指すように更新する
        /// 引数で指定しない場合は 4 増加させる
        /// </summary>
        /// <param name="instructionLength">PCの増数 32bit長命令の場合は4、16bit長命令の場合は2を指定する</param>
        public void IncrementPc(UInt32 instructionLength = 4u) {
            PC += instructionLength;
        }

        #endregion

        #region 整数レジスタ

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

        #endregion

        #region 浮動小数点レジスタ

        /// <summary>
        /// 浮動小数点レジスタに値を設定する
        /// </summary>
        /// <param name="name">設定する対象レジスタ名</param>
        /// <param name="value">設定する値</param>
        public void SetValue(FPRegister name, UInt32 value) {
            //値をレジスタに設定
            FPRegisters[name] = value;
            StatusCSR status = CSRegisters[CSR.mstatus];
            status.FS = 0x3;
            CSRegisters[CSR.mstatus] = status;
        }

        /// <summary>
        /// 浮動小数点レジスタに値を設定する
        /// </summary>
        /// <param name="name">設定する対象レジスタ名</param>
        /// <param name="value">設定する値</param>
        public void SetValue(FPRegister name, UInt64 value) {
            //値をレジスタに設定
            FPRegisters[name] = value;
            StatusCSR status = CSRegisters[CSR.mstatus];
            status.FS = 0x3;
            CSRegisters[CSR.mstatus] = status;
        }

        /// <summary>
        /// 浮動小数点レジスタから値を取得する
        /// </summary>
        /// <param name="name">取得する対象レジスタ名</param>
        /// <returns>レジスタの値</returns>
        public UInt64 GetValue(FPRegister name) {
            //レジスタの値を返す
            return FPRegisters[name];
        }

        /// <summary>
        /// 浮動小数点例外フラグをCSRに設定する
        /// </summary>
        /// <param name="fcsr"></param>
        public void SetFflagsCSR(FloatCSR fcsr) {
            CSRegisters[CSR.fflags] = fcsr & 0x1fu;
        }

        /// <summary>
        /// 浮動小数点レジスタ・fcsrの状態を返す
        /// </summary>
        /// <returns></returns>
        public bool IsFPAvailable() {
            return ((StatusCSR)CSRegisters[CSR.mstatus]).FS != 0;
        }
        #endregion

        #region コントロール・ステータスレジスタ

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
            PrivilegeLevels instructionLevel = (PrivilegeLevels)(((UInt32)name >> 8) & 0x3u);
            if (((UInt16)name & 0xC00u) != 0xC00u && instructionLevel <= currentMode) {
                switch (operation) {
                    case 'w':
                        CSRegisters[name] = value;
                        break;
                    case 's':
                        CSRegisters[name] |= value;
                        break;
                    case 'c':
                        CSRegisters[name] &= ~value;
                        break;
                    default:
                        throw new RiscvException(RiscvExceptionCause.IllegalInstruction, 0, this);
                }
            } else {
                //実行モードがアドレスの権限より小さい場合
                //アドレス上位2bitが 0b11(= 読み取り専用)の場合は
                //不正命令例外が発生する
                throw new RiscvException(RiscvExceptionCause.IllegalInstruction, 0, this);
            }
        }

        /// <summary>
        /// CSRから値を取得する
        /// </summary>
        /// <param name="name">取得する対象レジスタ名</param>
        /// <returns>CSRの値</returns>
        internal UInt32 GetCSR(CSR name) {
            //CSRアドレスの9～10bitがアクセス権限を表す
            PrivilegeLevels instructionLevel = (PrivilegeLevels)(((UInt32)name >> 8) & 0x3u);
            if (instructionLevel <= currentMode) {

                // ユーザモードカウンタ・タイマを読み込む場合
                // mcounteren,scounterenにビットが設定されていない場合は、
                // 参照不可のため、不正命令例外を発行する
                if ((CSR.cycle <= name && name <= CSR.hpmcounter31) ||
                    (CSR.cycleh <= name && name <= CSR.hpmcounter31h)) {

                    if (CurrentMode == PrivilegeLevels.UserMode) {
                        if (((CSRegisters[CSR.mcounteren] & (1u << ((int)name & 0x01f))) == 0) ||
                            ((CSRegisters[CSR.scounteren] & (1u << ((int)name & 0x01f))) == 0)) {
                            throw new RiscvException(RiscvExceptionCause.IllegalInstruction, 0, this);
                        }
                    } else if (CurrentMode == PrivilegeLevels.SupervisorMode) {
                        if (((CSRegisters[CSR.mcounteren] >> ((int)name & 0x01f)) & 0x1u) == 0) {
                            throw new RiscvException(RiscvExceptionCause.IllegalInstruction, 0, this);
                        }
                    }
                }
                return (UInt32)CSRegisters[name];


            } else {
                //実行モードがアドレスの権限より小さい場合は不正命令例外が発生する
                throw new RiscvException(RiscvExceptionCause.IllegalInstruction, 0, this);
            }
        }

        #endregion

        /// <summary>
        /// レジスタを全て初期化する
        /// </summary>
        public void ClearAll() {
            Registers.Select(r => Registers[r.Key] = 0);
            CSRegisters.Select(c => CSRegisters[c.Key] = 0);

            currentMode = PrivilegeLevels.MachineMode;
        }

        /// <summary>
        /// サイクルカウントを 1 増加させる
        /// </summary>
        public void IncrementCycle() {
            CSRegisters[CSR.mcycle]++;
            CSRegisters[CSR.minstret]++;
            /*
            CSRegisters[CSR.mhpmcounter3] += CSRegisters[CSR.mhpmevent3];
            CSRegisters[CSR.mhpmcounter4] += CSRegisters[CSR.mhpmevent4];
            CSRegisters[CSR.mhpmcounter5] += CSRegisters[CSR.mhpmevent5];
            CSRegisters[CSR.mhpmcounter6] += CSRegisters[CSR.mhpmevent6];
            CSRegisters[CSR.mhpmcounter7] += CSRegisters[CSR.mhpmevent7];
            CSRegisters[CSR.mhpmcounter8] += CSRegisters[CSR.mhpmevent8];
            CSRegisters[CSR.mhpmcounter9] += CSRegisters[CSR.mhpmevent9];
            CSRegisters[CSR.mhpmcounter10] += CSRegisters[CSR.mhpmevent10];
            CSRegisters[CSR.mhpmcounter11] += CSRegisters[CSR.mhpmevent11];
            CSRegisters[CSR.mhpmcounter12] += CSRegisters[CSR.mhpmevent12];
            CSRegisters[CSR.mhpmcounter13] += CSRegisters[CSR.mhpmevent13];
            CSRegisters[CSR.mhpmcounter14] += CSRegisters[CSR.mhpmevent14];
            CSRegisters[CSR.mhpmcounter15] += CSRegisters[CSR.mhpmevent15];
            CSRegisters[CSR.mhpmcounter16] += CSRegisters[CSR.mhpmevent16];
            CSRegisters[CSR.mhpmcounter17] += CSRegisters[CSR.mhpmevent17];
            CSRegisters[CSR.mhpmcounter18] += CSRegisters[CSR.mhpmevent18];
            CSRegisters[CSR.mhpmcounter19] += CSRegisters[CSR.mhpmevent19];
            CSRegisters[CSR.mhpmcounter20] += CSRegisters[CSR.mhpmevent20];
            CSRegisters[CSR.mhpmcounter21] += CSRegisters[CSR.mhpmevent21];
            CSRegisters[CSR.mhpmcounter22] += CSRegisters[CSR.mhpmevent22];
            CSRegisters[CSR.mhpmcounter23] += CSRegisters[CSR.mhpmevent23];
            CSRegisters[CSR.mhpmcounter24] += CSRegisters[CSR.mhpmevent24];
            CSRegisters[CSR.mhpmcounter25] += CSRegisters[CSR.mhpmevent25];
            CSRegisters[CSR.mhpmcounter26] += CSRegisters[CSR.mhpmevent26];
            CSRegisters[CSR.mhpmcounter27] += CSRegisters[CSR.mhpmevent27];
            CSRegisters[CSR.mhpmcounter28] += CSRegisters[CSR.mhpmevent28];
            CSRegisters[CSR.mhpmcounter29] += CSRegisters[CSR.mhpmevent29];
            CSRegisters[CSR.mhpmcounter30] += CSRegisters[CSR.mhpmevent30];
            CSRegisters[CSR.mhpmcounter31] += CSRegisters[CSR.mhpmevent31];*/
        }

        /// <summary>
        /// マシンの拡張命令セットを表すCSRに設定を追加する
        /// </summary>
        /// <param name="isa">拡張命令セットを表すA～Zの文字</param>
        internal void AddMisa(char isa) {
            CSRegisters.Misa |= 1u << (Char.ToUpper(isa) - 'A');

            // 追加標準機能をサポートする場合、Gもセットする
            if (Char.ToUpper(isa) != 'E' && Char.ToUpper(isa) != 'I') {
                CSRegisters.Misa |= 1u << ('G' - 'A');
            }
        }

        /// <summary>
        /// 例外発生時のCSR処理を行う
        /// </summary>
        /// <param name="cause">例外の原因</param>
        /// <param name="tval">例外の原因となったアドレス(ない場合は0)</param>
        public void HandleExceptionCSR(RiscvExceptionCause cause, UInt32 tval) {

            if ((CSRegisters[CSR.medeleg] & (1u << (int)cause)) == 0u || (CSRegisters[CSR.misa] & ((1u << ('S' - 'A')) | (1u << ('U' - 'A')))) == 0u) {
                // マシンモード以下に例外トラップ委譲されていない または、 ユーザモード・スーパーバイザモードの実装がない場合

                // 例外の発生したPCを設定
                CSRegisters[CSR.mepc] = PC;

                // 例外の原因を設定
                CSRegisters[CSR.mcause] = (UInt32)cause;

                // 例外の発生時にアクセスしていたメモリアドレスを設定
                CSRegisters[CSR.mtval] = tval;

                // 例外前のステータスを設定
                StatusCSR status = CSRegisters[CSR.mstatus];
                status.MPIE = status.MIE;
                status.MPP = (byte)currentMode;
                status.MIE = false;
                CSRegisters[CSR.mstatus] = (UInt32)status;

                // プログラムカウンタの設定
                PC = CSRegisters[CSR.mtvec];

                // 実行モードの変更
                currentMode = PrivilegeLevels.MachineMode;

            } else if ((CSRegisters[CSR.sedeleg] & (1u << (int)cause)) == 0u || (CSRegisters[CSR.misa] & (1u << ('U' - 'A'))) == 0u) {
                // スーパーバイザモード以下に例外トラップ委譲されていない または、 ユーザモードの実装がない場合

                // 例外の発生したPCを設定
                CSRegisters[CSR.sepc] = PC;

                // 例外の原因を設定
                CSRegisters[CSR.scause] = (UInt32)cause;

                // 例外の発生時にアクセスしていたメモリアドレスを設定
                CSRegisters[CSR.stval] = tval;

                // 例外前のステータスを設定
                StatusCSR status = CSRegisters[CSR.sstatus];
                status.SPIE = status.SIE;
                status.SPP = ((byte)currentMode & 1u) > 0u;
                status.SIE = false;
                CSRegisters[CSR.sstatus] = (UInt32)status;

                // プログラムカウンタの設定
                PC = CSRegisters[CSR.stvec];

                // 実行モードの変更
                currentMode = PrivilegeLevels.SupervisorMode;
            } else {
                // 例外の発生したPCを設定
                CSRegisters[CSR.uepc] = PC;

                // 例外の原因を設定
                CSRegisters[CSR.ucause] = (UInt32)cause;

                // 例外の発生時にアクセスしていたメモリアドレスを設定
                CSRegisters[CSR.utval] = tval;

                // 例外前のステータスを設定
                StatusCSR status = CSRegisters[CSR.ustatus];
                status.UPIE = status.UIE;
                status.UIE = false;
                CSRegisters[CSR.ustatus] = (UInt32)status;

                // プログラムカウンタの設定
                PC = CSRegisters[CSR.utvec];

                // 実行モードの変更
                currentMode = PrivilegeLevels.UserMode;
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

        #endregion
    }
}