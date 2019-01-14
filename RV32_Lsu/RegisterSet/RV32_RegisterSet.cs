using RV32_Lsu.Constants;
using RV32_Lsu.Exceptions;
using RV32_Lsu.MemoryHandler;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RV32_Lsu.RegisterSet {
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
            CurrentMode = PrivilegeLevels.MachineMode;

            CSRegisters.Misa |= 0x4000_0000u;

        }

        #endregion

        #region プロパティ

        /// <summary>プログラムカウンタ</summary>
        private UInt32 programCounter;
        public UInt32 PC {
            get => programCounter;
            internal set {
                programCounter = value;
                // 命令レジスタのフェッチ
                IR = Mem.FetchInstruction(value);
            }
        }

        /// <summary>命令レジスタ</summary>
        public UInt32 IR { get; private set; }

        /// <summary>現在の実行モード</summary>
        public PrivilegeLevels CurrentMode { get; set; }

         /// <summary>割り込み待ちモード</summary>
        public bool IsWaitMode { get; set; }

       /// <summary>メインメモリ</summary>
        private RV32_AbstractMemoryHandler mainMemory;
        public RV32_AbstractMemoryHandler Mem {
            get => mainMemory;
            set {
                mainMemory = value;
                mainMemory.SetRegisterSet(this);
            }
        }

        #endregion

        /// <summary>
        /// プログラムカウントを 引数で指定した分 増加させ、次の命令アドレスを指すように更新する
        /// 引数で指定しない場合は 4 増加させる
        /// </summary>
        /// <param name="instructionLength">PCの増数 32bit長命令の場合は4、16bit長命令の場合は2を指定する</param>
        public void IncrementPc(UInt32 instructionLength = 4u) {
            PC += instructionLength;
        }

        #region Risc-V CPU命令

        #region Risc-V CPU 特権命令
        /// <summary>
        /// Fence Memory and I/O命令
        /// </summary>
        /// <param name="pred_succ"></param>
        /// <returns>処理の成否</returns>
        public bool Fence(byte pred_succ, UInt32 insLength = 4u) {
            IncrementPc(insLength);
            return true;
        }

        /// <summary>
        /// Fence Instruction Stream命令
        /// </summary>
        /// <returns>処理の成否</returns>
        public bool FenceI(UInt32 insLength = 4u) {
            IncrementPc(insLength);
            return true;
        }

        /// <summary>
        /// Environment Call命令
        /// 環境呼び出し例外を起こして、実行環境を呼び出す
        /// </summary>
        /// <returns>処理の成否</returns>
        public bool Ecall(UInt32 insLength = 4u) {
            PrivilegeLevels prev_level = CurrentMode;
            CurrentMode = PrivilegeLevels.MachineMode;
            CSRegisters[CSR.mepc] = PC;
            throw new RiscvEnvironmentCallException(prev_level, this);
        }

        /// <summary>
        /// Environment Break命令
        /// ブレークポイント例外を起こして、実行環境を呼び出す
        /// </summary>
        /// <returns>処理の成否</returns>
        public bool Ebreak(UInt32 insLength = 4u) {
            throw new RiscvException(RiscvExceptionCause.Breakpoint, PC, this);
        }

        /// <summary>
        /// Wait For Interrupt命令
        /// </summary>
        /// <returns>処理の成否</returns>
        public bool Wfi(UInt32 insLength = 4u) {
            if (((StatusCSR)CSRegisters[CSR.mstatus]).TW && CurrentMode <= PrivilegeLevels.SupervisorMode) {
                throw new RiscvException(RiscvExceptionCause.IllegalInstruction, IR, this);
            }
            IsWaitMode = true;
            
            return true;
        }

        /// <summary>
        /// Fence Virtual Memory
        /// 先行するページ/テーブルへのストアと引き続いて怒る仮想アドレスの変換とを順序付ける
        /// </summary>
        /// <param name="rs1">
        /// 0: 選択されたアドレス空間の全ての仮想アドレスに関するアドレス変換が順序付けられる
        /// 0以外: 選択されたアドレス空間のrs1を含むアドレス変換だけが順序付けられる
        /// </param>
        /// <param name="rs2">
        /// 0: 全てのアドレス空間におけるアドレス変換が順序付けられる
        /// 0以外: rs2によって識別されるアドレス空間におけるアドレス変換のみが順序付けられる
        /// </param>
        /// <returns>処理の成否</returns>
        public bool SfenceVma(Register rs1, Register rs2, UInt32 insLength = 4u) {
            if (((StatusCSR)CSRegisters[CSR.mstatus]).TVM && CurrentMode <= PrivilegeLevels.SupervisorMode) {
                throw new RiscvException(RiscvExceptionCause.IllegalInstruction, IR, this);
            }
            IncrementPc(insLength);
            return true;
        }

        /// <summary>
        /// Machine-Mode Exception Return
        /// マシンモードの例外ハンドラから戻る
        /// </summary>
        /// <returns>処理の成否</returns>
        public bool Mret(UInt32 insLength = 4u) {
            PC = CSRegisters[CSR.mepc];
            StatusCSR mstatus = CSRegisters[CSR.mstatus];

            PrivilegeLevels priv_level = (PrivilegeLevels)mstatus.MPP;

            mstatus.MIE = mstatus.MPIE;
            mstatus.MPIE = true;
            mstatus.MPP = 0;
            CSRegisters[CSR.mstatus]= mstatus;

            CurrentMode = priv_level;

            return true;
        }

        /// <summary>
        /// Supervisor-Mode Exception Return
        /// スーパーバイザモードの例外ハンドラから戻る
        /// </summary>
        /// <returns>処理の成否</returns>
        public bool Sret(UInt32 insLength = 4u) {
            if (((StatusCSR)CSRegisters[CSR.mstatus]).TSR && CurrentMode <= PrivilegeLevels.SupervisorMode) {
                throw new RiscvException(RiscvExceptionCause.IllegalInstruction, IR, this);
            }
            PC = CSRegisters[CSR.sepc];
            StatusCSR sstatus = CSRegisters[CSR.sstatus];

            PrivilegeLevels priv_level = sstatus.SPP ? PrivilegeLevels.SupervisorMode : PrivilegeLevels.UserMode;


            sstatus.SIE = sstatus.SPIE;
            sstatus.SPIE = true;
            sstatus.SPP = false;
            CSRegisters[CSR.sstatus] = sstatus;

            CurrentMode = priv_level;

            return true;
        }

        /// <summary>
        /// User-Mode Exception Return
        /// ユーザモードの例外ハンドラから戻る
        /// </summary>
        /// <returns>処理の成否</returns>
        public bool Uret(UInt32 insLength = 4u) {
            if (((StatusCSR)CSRegisters[CSR.mstatus]).TSR) {
                throw new RiscvException(RiscvExceptionCause.IllegalInstruction, IR, this);
            }
            PC = CSRegisters[CSR.uepc];
            StatusCSR ustatus = CSRegisters[CSR.ustatus];

            PrivilegeLevels priv_level = PrivilegeLevels.UserMode;

            ustatus.UIE = ustatus.UPIE;
            ustatus.UPIE = true;
            CSRegisters[CSR.ustatus] =ustatus;

            CurrentMode = priv_level;

            return true;
        }


        /// <summary>
        /// Control Status Register Read and Write命令
        /// 現在のCSRをレジスタrdに書き込み、レジスタrs1をCSRに書き込む
        /// </summary>
        /// <param name="rd">現在のCSRの値を格納するレジスタ番号</param>
        /// <param name="rs1">書き込む値が格納されているレジスタ</param>
        /// <param name="csr">CSRのアドレス</param>
        /// <returns>処理の成否</returns>
        public bool Csrrw(Register rd, Register rs1, CSR csr, UInt32 insLength = 4u) {
            UInt32 t = GetCSR(csr);
            SetCSR(csr, 'w', rs1);
            SetValue(rd, t);
            IncrementPc(insLength);
            return true;
        }
        /// <summary>
        /// Control Status Register Read and Set命令
        /// 現在のCSRをレジスタrdに書き込み、現在のCSRとレジスタrs1の論理和をCSRに書き込む
        /// </summary>
        /// <param name="rd">現在のCSRの値を格納するレジスタ番号</param>
        /// <param name="rs1">セットする値が格納されているレジスタ</param>
        /// <param name="csr">CSRのアドレス</param>
        /// <returns>処理の成否</returns>
        public bool Csrrs(Register rd, Register rs1, CSR csr, UInt32 insLength = 4u) {
            UInt32 t = GetCSR(csr);
            SetCSR(csr, 's', rs1);
            SetValue(rd, t);
            IncrementPc(insLength);
            return true;
        }
        /// <summary>
        /// Control Status Register Read and Clear命令
        /// 現在のCSRをレジスタrdに書き込み、現在のCSRとレジスタrs1の1の補数との論理積をCSRに書き込む
        /// </summary>
        /// <param name="rd">現在のCSRの値を格納するレジスタ番号</param>
        /// <param name="rs1">クリアする値が格納されているレジスタ</param>
        /// <param name="csr">CSRのアドレス</param>
        /// <returns>処理の成否</returns>
        public bool Csrrc(Register rd, Register rs1, CSR csr, UInt32 insLength = 4u) {
            UInt32 t = GetCSR(csr);
            SetCSR(csr, 'c', rs1);
            SetValue(rd, t);
            IncrementPc(insLength);
            return true;
        }
        /// <summary>
        /// Control Status Register Read and Write Immediate命令
        /// 現在のCSRをレジスタrdに書き込み、即値をCSRに書き込む
        /// </summary>
        /// <param name="rd">現在のCSRの値を格納するレジスタ番号</param>
        /// <param name="zImmediate">即値</param>
        /// <param name="csr">CSRのアドレス</param>
        /// <returns>処理の成否</returns>
        public bool Csrrwi(Register rd, byte zImmediate, CSR csr, UInt32 insLength = 4u) {
            UInt32 t = GetCSR(csr);
            SetCSR(csr, 'w', zImmediate);
            SetValue(rd, t);
            IncrementPc(insLength);
            return true;
        }

        /// <summary>
        /// Control Status Register Read and Set Immediate命令
        /// 現在のCSRをレジスタrdに書き込み、現在のCSRと即値の論理和をCSRに書き込む
        /// </summary>
        /// <param name="rd">現在のCSRの値を格納するレジスタ番号</param>
        /// <param name="zImmediate">即値</param>
        /// <param name="csr">CSRのアドレス</param>
        /// <returns>処理の成否</returns>
        public bool Csrrsi(Register rd, byte zImmediate, CSR csr, UInt32 insLength = 4u) {
            UInt32 t = GetCSR(csr);
            SetCSR(csr, 's', zImmediate);
            SetValue(rd, t);
            IncrementPc(insLength);
            return true;
        }

        /// <summary>
        /// Control Status Register Read and Clear Immediate命令
        /// 現在のCSRをレジスタrdに書き込み、現在のCSRと即値の1の補数との論理積をCSRに書き込む
        /// </summary>
        /// <param name="rd">現在のCSRの値を格納するレジスタ番号</param>
        /// <param name="zImmediate">即値</param>
        /// <param name="csr">CSRのアドレス</param>
        /// <returns>処理の成否</returns>
        public bool Csrrci(Register rd, byte zImmediate, CSR csr, UInt32 insLength = 4u) {
            UInt32 t = CSRegisters[csr];
            SetCSR(csr, 'c', zImmediate);
            SetValue(rd, t);
            IncrementPc(insLength);
            return true;
        }

        #endregion

        #endregion

        #region 内部用メソッド

        #region private コントロール・ステータスレジスタ操作

        /// <summary>
        /// CSRに値を設定する
        /// </summary>
        /// <param name="name">設定する対象レジスタ名</param>
        /// <param name="value">設定する値</param>
        private void SetCSR(CSR name, char operation, Register reg) {

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
        private void SetCSR(CSR name, char operation, UInt32 value) {

            //CSRアドレスの11～12bitで読み書き可能 or 読み取り専用を
            //9～10bitがアクセス権限を表す
            PrivilegeLevels instructionLevel = (PrivilegeLevels)(((UInt32)name >> 8) & 0x3u);
            if (((UInt16)name & 0xC00u) != 0xC00u && instructionLevel <= CurrentMode) {
                if (((StatusCSR)CSRegisters[CSR.mstatus]).TVM && CurrentMode <= PrivilegeLevels.SupervisorMode) {
                    throw new RiscvException(RiscvExceptionCause.IllegalInstruction, IR, this);
                }

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
                        throw new RiscvException(RiscvExceptionCause.IllegalInstruction, IR, this);
                }
            } else {
                //実行モードがアドレスの権限より小さい場合
                //アドレス上位2bitが 0b11(= 読み取り専用)の場合は
                //不正命令例外が発生する
                throw new RiscvException(RiscvExceptionCause.IllegalInstruction, IR, this);
            }
        }

        /// <summary>
        /// CSRから値を取得する
        /// </summary>
        /// <param name="name">取得する対象レジスタ名</param>
        /// <returns>CSRの値</returns>
        private UInt32 GetCSR(CSR name) {
            //CSRアドレスの9～10bitがアクセス権限を表す
            PrivilegeLevels instructionLevel = (PrivilegeLevels)(((UInt32)name >> 8) & 0x3u);
            if (instructionLevel <= CurrentMode) {

                // ユーザモードカウンタ・タイマを読み込む場合
                // mcounteren,scounterenにビットが設定されていない場合は、
                // 参照不可のため、不正命令例外を発行する
                if ((CSR.cycle <= name && name <= CSR.hpmcounter31) ||
                    (CSR.cycleh <= name && name <= CSR.hpmcounter31h)) {

                    if (CurrentMode == PrivilegeLevels.UserMode) {
                        if (((CSRegisters[CSR.mcounteren] & (1u << ((int)name & 0x01f))) == 0) ||
                            ((CSRegisters[CSR.scounteren] & (1u << ((int)name & 0x01f))) == 0)) {
                            throw new RiscvException(RiscvExceptionCause.IllegalInstruction, IR, this);
                        }
                    } else if (CurrentMode == PrivilegeLevels.SupervisorMode) {
                        if (((CSRegisters[CSR.mcounteren] >> ((int)name & 0x01f)) & 0x1u) == 0) {
                            throw new RiscvException(RiscvExceptionCause.IllegalInstruction, IR, this);
                        }
                    }
                }
                return (UInt32)CSRegisters[name];


            } else {
                //実行モードがアドレスの権限より小さい場合は不正命令例外が発生する
                throw new RiscvException(RiscvExceptionCause.IllegalInstruction, IR, this);
            }
        }

        #endregion

        #region public 整数レジスタ操作

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

        #region public 浮動小数点レジスタ

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

        #region public コントロール・ステータスレジスタ

        /// <summary>
        /// サイクルカウントを 1 増加させる
        /// </summary>
        public void IncrementCycle() {
            // オーバーフローして0に戻った場合は、それぞれの上位カウンタを加算する
            if (CSRegisters[CSR.mcycle]++ == UInt32.MinValue) CSRegisters[CSR.mcycleh]++;
            if (CSRegisters[CSR.minstret]++ == UInt32.MinValue) CSRegisters[CSR.minstreth]++;
            /*
            if (CSRegisters[CSR.mhpmcounter3] += CSRegisters[CSR.mhpmevent3];
            if (CSRegisters[CSR.mhpmcounter4] += CSRegisters[CSR.mhpmevent4];
            if (CSRegisters[CSR.mhpmcounter5] += CSRegisters[CSR.mhpmevent5];
            if (CSRegisters[CSR.mhpmcounter6] += CSRegisters[CSR.mhpmevent6];
            if (CSRegisters[CSR.mhpmcounter7] += CSRegisters[CSR.mhpmevent7];
            if (CSRegisters[CSR.mhpmcounter8] += CSRegisters[CSR.mhpmevent8];
            if (CSRegisters[CSR.mhpmcounter9] += CSRegisters[CSR.mhpmevent9];
            if (CSRegisters[CSR.mhpmcounter10] += CSRegisters[CSR.mhpmevent10];
            if (CSRegisters[CSR.mhpmcounter11] += CSRegisters[CSR.mhpmevent11];
            if (CSRegisters[CSR.mhpmcounter12] += CSRegisters[CSR.mhpmevent12];
            if (CSRegisters[CSR.mhpmcounter13] += CSRegisters[CSR.mhpmevent13];
            if (CSRegisters[CSR.mhpmcounter14] += CSRegisters[CSR.mhpmevent14];
            if (CSRegisters[CSR.mhpmcounter15] += CSRegisters[CSR.mhpmevent15];
            if (CSRegisters[CSR.mhpmcounter16] += CSRegisters[CSR.mhpmevent16];
            if (CSRegisters[CSR.mhpmcounter17] += CSRegisters[CSR.mhpmevent17];
            if (CSRegisters[CSR.mhpmcounter18] += CSRegisters[CSR.mhpmevent18];
            if (CSRegisters[CSR.mhpmcounter19] += CSRegisters[CSR.mhpmevent19];
            if (CSRegisters[CSR.mhpmcounter20] += CSRegisters[CSR.mhpmevent20];
            if (CSRegisters[CSR.mhpmcounter21] += CSRegisters[CSR.mhpmevent21];
            if (CSRegisters[CSR.mhpmcounter22] += CSRegisters[CSR.mhpmevent22];
            if (CSRegisters[CSR.mhpmcounter23] += CSRegisters[CSR.mhpmevent23];
            if (CSRegisters[CSR.mhpmcounter24] += CSRegisters[CSR.mhpmevent24];
            if (CSRegisters[CSR.mhpmcounter25] += CSRegisters[CSR.mhpmevent25];
            if (CSRegisters[CSR.mhpmcounter26] += CSRegisters[CSR.mhpmevent26];
            if (CSRegisters[CSR.mhpmcounter27] += CSRegisters[CSR.mhpmevent27];
            if (CSRegisters[CSR.mhpmcounter28] += CSRegisters[CSR.mhpmevent28];
            if (CSRegisters[CSR.mhpmcounter29] += CSRegisters[CSR.mhpmevent29];
            if (CSRegisters[CSR.mhpmcounter30] += CSRegisters[CSR.mhpmevent30];
            if (CSRegisters[CSR.mhpmcounter31] += CSRegisters[CSR.mhpmevent31];*/
        }

        /// <summary>
        /// マシンの拡張命令セットを表すCSRに設定を追加する
        /// </summary>
        /// <param name="isa">拡張命令セットを表すA～Zの文字</param>
        public void AddMisa(char isa) {
            CSRegisters.Misa |= 1u << (Char.ToUpper(isa) - 'A');

            // 追加標準機能をサポートする場合、Gもセットする
            if (Char.ToUpper(isa) != 'E' && Char.ToUpper(isa) != 'I') {
                CSRegisters.Misa |= 1u << ('G' - 'A');
            }
        }

        /// <summary>
        /// レジスタを全てクリアし、エントリポイントをPCに設定する
        /// </summary>
        public void ClearAndSetPC(UInt32 PhysicalAddress, UInt32 VirtualAddress, UInt32 entryOffset) {
            Registers.Select(r => Registers[r.Key] = 0);
            CSRegisters.Select(c => CSRegisters[c.Key] = 0);

            CurrentMode = PrivilegeLevels.MachineMode;

            Mem.PAddr = PhysicalAddress;
            Mem.Offset = entryOffset;
            PC = VirtualAddress;
        }

        /// <summary>
        /// 例外発生時のCSR処理を行う
        /// </summary>
        /// <param name="cause">例外の原因</param>
        /// <param name="tval">例外の原因となったアドレス(ない場合は0)</param>
        public void HandleException(RiscvExceptionCause cause, UInt32 tval) {

            if ((CSRegisters[CSR.medeleg] & (1u << (int)cause)) == 0u || (CSRegisters[CSR.misa] & ((1u << ('S' - 'A')) | (1u << ('U' - 'A')))) == 0u) {
                // マシンモードより下位モードに例外トラップ委譲されていない または、 ユーザモード・スーパーバイザモードの実装がない場合

                // マシンモードでトラップ
                TrapAndSetCSR(PrivilegeLevels.MachineMode, (UInt32)cause, tval);

            } else if ((CSRegisters[CSR.sedeleg] & (1u << (int)cause)) == 0u || (CSRegisters[CSR.misa] & (1u << ('U' - 'A'))) == 0u) {
                // スーパーバイザモード下位モードに例外トラップ委譲されていない または、 ユーザモードの実装がない場合

                // スーパーバイザモードでトラップ
                TrapAndSetCSR(PrivilegeLevels.SupervisorMode, (UInt32)cause, tval);

            } else {
                // ユーザモードでトラップ
                TrapAndSetCSR(PrivilegeLevels.UserMode, (UInt32)cause, tval);
            }
        }

        /// <summary>
        /// 割り込み有無の確認と、割り込みがある場合はCSR処理を行う
        /// </summary>
        /// <returns>割り込み有無(true: 割り込みあり, false:割り込みなし)</returns>
        public bool CheckAndHandleInterrupt() {

            // 割り込み有効判定用変数
            UInt32 enableInterrupt = 0u;

            // 割り込みトラップモード
            PrivilegeLevels trapLevel = 0u;

            UInt32 pendingInterrupt = CSRegisters[CSR.mie] & CSRegisters[CSR.mip];
            StatusCSR mstatus = CSRegisters[CSR.mstatus];

            UInt32 mideleg = CSRegisters[CSR.mideleg];
            UInt32 sideleg = CSRegisters[CSR.sideleg];


            // 割り込み条件の判定とトラップモードの特定
            if ((pendingInterrupt & ~mideleg) > 0u) {
                // 割り込み待ち状態でマシンモードより下位モードに割り込みトラップ委譲されていない場合

                if (IsWaitMode) {
                    // ハードウェアスレッドが割り込み待ち状態の場合
                    enableInterrupt = pendingInterrupt & ~mideleg; ;
                    trapLevel = PrivilegeLevels.MachineMode;

                } else if (CurrentMode == PrivilegeLevels.MachineMode && mstatus.MIE) {
                    // 現在がマシンモードかつ、mstatusのマシンモード割り込みが許可されている場合
                    enableInterrupt = pendingInterrupt & ~mideleg; ;
                    trapLevel = PrivilegeLevels.MachineMode;

                } else if (CurrentMode < PrivilegeLevels.MachineMode) {
                    // 現在がマシンモードより下位モードの場合
                    enableInterrupt = pendingInterrupt & ~mideleg; ;
                    trapLevel = PrivilegeLevels.MachineMode;
                }
            } else if ((pendingInterrupt & mideleg) > 0u && (pendingInterrupt & ~sideleg) > 0u &&
                ((CSRegisters[CSR.misa] & (1u << ('U' - 'A'))) == 0u || (CSRegisters[CSR.misa] & (1u << ('N' - 'A'))) == 0u)) {
                // 割り込み待ち状態で、割り込み委譲されているかつ、スーパーバイザモードより下位モードに割り込みトラップ委譲されていないかつ、
                // ユーザモード割り込みがサポートされていない場合

                if (IsWaitMode) {
                    // ハードウェアスレッドが割り込み待ち状態の場合
                    enableInterrupt = pendingInterrupt & ~sideleg;
                    trapLevel = PrivilegeLevels.SupervisorMode;

                } else if (CurrentMode == PrivilegeLevels.SupervisorMode && mstatus.SIE) {
                    // 現在がスーパーバイザモードかつ、mstatusのスーパーバイザモード割り込みが許可されている場合
                    enableInterrupt = pendingInterrupt & ~sideleg;
                    trapLevel = PrivilegeLevels.SupervisorMode;

                } else if (CurrentMode < PrivilegeLevels.SupervisorMode) {
                    // 現在がスーパーバイザモードより下位モードの場合
                    enableInterrupt = pendingInterrupt & ~sideleg;
                    trapLevel = PrivilegeLevels.SupervisorMode;

                }
            } else if ((pendingInterrupt & sideleg) > 0u) {
                // 割り込み待ち状態で、割り込み委譲されている場合

                if (IsWaitMode) {
                    // ハードウェアスレッドが割り込み待ち状態の場合
                    enableInterrupt = pendingInterrupt & sideleg;
                    trapLevel = PrivilegeLevels.UserMode;

                } else if (CurrentMode == PrivilegeLevels.UserMode && mstatus.UIE) {
                    // 現在がスーパーバイザモードかつ、mstatusのユーザモード割り込みが許可されている場合
                    enableInterrupt = pendingInterrupt & sideleg;
                    trapLevel = PrivilegeLevels.UserMode;
                }
            }

            if (enableInterrupt == 0u) {
                // 割り込みなしと判定
                return false;
            }

            if (IsWaitMode) IsWaitMode = false;

            // 割り込み要因の特定
            //   *複数の要因での割り込みの可能性があるため、順位の高いものから特定する
            //   *外部割り込み => ソフトウェア割り込み => タイマ割り込み の順
            RiscvInterruptCause cause;
            if ((enableInterrupt & (1u << (int)((uint)RiscvInterruptCause.MachineExternal - 0x8000_0000u))) > 0) {
                // 
                cause = RiscvInterruptCause.MachineExternal;

            } else if ((enableInterrupt & (1u << (int)((uint)RiscvInterruptCause.SupervisorExternal - 0x8000_0000u))) > 0) {
                cause = RiscvInterruptCause.SupervisorExternal;

            } else if ((enableInterrupt & (1u << (int)((uint)RiscvInterruptCause.UserExternal - 0x8000_0000u))) > 0) {
                cause = RiscvInterruptCause.UserExternal;

            } else if ((enableInterrupt & (1u << (int)((uint)RiscvInterruptCause.MachineSoftware - 0x8000_0000u))) > 0){
                cause = RiscvInterruptCause.MachineSoftware;

            } else if ((enableInterrupt & (1u << (int)((uint)RiscvInterruptCause.SupervisorSoftware - 0x8000_0000u))) > 0) {
                cause = RiscvInterruptCause.SupervisorSoftware;

            } else if ((enableInterrupt & (1u << (int)((uint)RiscvInterruptCause.UserSoftware - 0x8000_0000u))) > 0) {
                cause = RiscvInterruptCause.UserSoftware;

            } else if ((enableInterrupt & (1u << (int)((uint)RiscvInterruptCause.MachineTimer - 0x8000_0000u))) > 0){
                cause = RiscvInterruptCause.MachineTimer;

            } else if ((enableInterrupt & (1u << (int)((uint)RiscvInterruptCause.SupervisorTimer - 0x8000_0000u))) > 0) {
                cause = RiscvInterruptCause.SupervisorTimer;

            } else if ((enableInterrupt & (1u << (int)((uint)RiscvInterruptCause.UserTimer - 0x8000_0000u))) > 0) {
                cause = RiscvInterruptCause.UserTimer;
            } else {
                return false;
            }

            // 特定したモード、要因でトラップ
            TrapAndSetCSR(trapLevel, (UInt32)cause, 0u);
            return true;
        }

        /// <summary>
        /// 割り込み・例外をトラップしてCSRの設定を行う
        /// </summary>
        /// <param name="level">トラップする特権レベル</param>
        /// <param name="cause">割り込み・例外要因コード</param>
        /// <param name="tval">トラップ値
        /// (ブレークポイント/アドレス不正列/アクセス/ページフォールト例外の場合は実効アドレス、
        /// 不正命令例外の場合は命令を設定する。その他の例外については0を設定するが、
        /// 将来の仕様により、再定義される可能性がある。)</param>
        private void TrapAndSetCSR(PrivilegeLevels level, UInt32 cause, UInt32 tval) {
            StatusCSR status;

            UInt32 epc = (cause & 0x8000_0000u) == 0u ? PC : PC + 4;

            switch (level) {
                case PrivilegeLevels.MachineMode:
                    CSRegisters[CSR.mepc] = epc;

                    // 割り込み・例外の原因を設定
                    CSRegisters[CSR.mcause] = cause;


                    // トラップ値を設定
                    CSRegisters[CSR.mtval] = tval;

                    // 割り込み・例外前のステータスを設定
                    status = CSRegisters[CSR.mstatus];
                    status.MPIE = status.MIE;
                    status.MPP = (byte)CurrentMode;
                    status.MIE = false;
                    CSRegisters[CSR.mstatus] = status;

                    // プログラムカウンタの設定
                    if (PC != CSRegisters[CSR.mtvec]) {
                        PC = CSRegisters[CSR.mtvec];
                    }

                    // 実行モードの変更
                    CurrentMode = PrivilegeLevels.MachineMode;
                    break;

                case PrivilegeLevels.SupervisorMode:
                    // 割り込み・例外の発生したPCを設定
                    CSRegisters[CSR.sepc] = epc;

                    // 割り込み・例外の原因を設定
                    CSRegisters[CSR.scause] = cause;

                    // トラップ値を設定
                    CSRegisters[CSR.stval] = tval;

                    // 割り込み・例外前のステータスを設定
                    status = CSRegisters[CSR.sstatus];
                    status.SPIE = status.SIE;
                    status.SPP = ((byte)CurrentMode & 1u) > 0u;
                    status.SIE = false;
                    CSRegisters[CSR.sstatus] = status;

                    // プログラムカウンタの設定
                    if (PC != CSRegisters[CSR.stvec]) {
                        PC = CSRegisters[CSR.stvec];
                    }

                    // 実行モードの変更
                    CurrentMode = PrivilegeLevels.SupervisorMode;
                    break;

                case PrivilegeLevels.UserMode:
                    // 割り込み・例外の発生したPCを設定
                    CSRegisters[CSR.uepc] = epc;

                    // 割り込み・例外の原因を設定
                    CSRegisters[CSR.ucause] = cause;

                    // トラップ値を設定
                    CSRegisters[CSR.utval] = tval;

                    // 割り込み・例外前のステータスを設定
                    status = CSRegisters[CSR.ustatus];
                    status.UPIE = status.UIE;
                    status.UIE = false;
                    CSRegisters[CSR.ustatus] = status;

                    // プログラムカウンタの設定
                    if (PC != CSRegisters[CSR.utvec]) {
                        PC = CSRegisters[CSR.utvec];
                    }

                    // 実行モードの変更
                    CurrentMode = PrivilegeLevels.UserMode;
                    break;
            }
        }


        #endregion

        #endregion

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