using RV32_Register.Constants;
using RV32_Register.Exceptions;
using RV32_Register.MemoryHandler;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RV32_Register {
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

            // nullチェック
            if (mem is null) {
                throw new ArgumentNullException("mem");
            }

            // レジスタ初期化
            Registers = Enumerable.Range(0, 32).ToDictionary(key => (Register)key, value => 0U);
            FPRegisters = Enumerable.Range(0, 32).ToDictionary(key => (FPRegister)key, value => BitConverter.ToUInt64(BitConverter.GetBytes(0d), 0));

            // コントロール・ステータスレジスタ初期化
            CSRegisters = new RV32_ControlStatusRegisters(Enum.GetValues(typeof(CSR)).Cast<CSR>().ToDictionary(key => key, value => 0U));

            mainMemory = mem;
            mainMemory.SetRegisterSet(this);

            //マシンモードに設定
            CurrentMode = PrivilegeLevel.MachineMode;

            CSRegisters.Misa |= 0x4000_0000U;

        }

        #endregion

        #region プロパティ

        /// <summary>プログラムカウンタ</summary>
        private UInt32 programCounter;
        public UInt32 PC {
            get => programCounter;
            internal set {
                if ((CSRegisters[CSR.misa] & (1U << ('C' - 'A'))) > 0) {
                    // RV32Cをサポートしている場合、命令アドレスが2の倍数でない場合は不正
                    if ((value & 0x1U) > 0) {
                        throw new RiscvException(RiscvExceptionCause.InstructionAddressMisaligned, value, this);
                    }
                } else {
                    // RV32Cをサポートしていない場合、命令アドレスが4の倍数でない場合は不正
                    if ((value & 0x11U) > 0) {
                        throw new RiscvException(RiscvExceptionCause.InstructionAddressMisaligned, value, this);
                    }
                }
                programCounter = value;
                // 命令レジスタのフェッチ
                IR = Mem.FetchInstruction(value);
            }
        }

        /// <summary>命令レジスタ</summary>
        public UInt32 IR { get; private set; }

        /// <summary>現在の実行モード</summary>
        public PrivilegeLevel CurrentMode { get; set; }

        /// <summary>割り込み待ちモード</summary>
        public bool IsWaitMode { get; set; }

        /// <summary>割り込み待ちタイムアウトタイマー</summary>
        private uint WaitTimer { get; set; }

        /// <summary>割り込み待ちタイムアウト値</summary>
        private const uint WaitTimerMax = 0U;

        /// <summary>メインメモリ</summary>
        private RV32_AbstractMemoryHandler mainMemory;
        public RV32_AbstractMemoryHandler Mem {
            get => mainMemory;
            set {
                // nullチェック
                if (value is null) {
                    throw new ArgumentNullException("value");
                }
                mainMemory = value;
                mainMemory.SetRegisterSet(this);
            }
        }

        #endregion

        #region Risc-V CPU命令

        #region RISC-V CPU Jump系命令

        /// <summary>
        /// Jump And Link命令
        /// 次の命令アドレス(pc+4)をレジスタrdに書き込み、現在のpcにoffsetを加えてpcに設定する
        /// </summary>
        /// <param name="rd">次の命令アドレスを格納するレジスタ番号</param>
        /// <param name="offset">ジャンプする現在のpcからの相対アドレス位置</param>
        public bool Jal(Register rd, Int32 offset, UInt32 insLength = 4U) {
            SetValue(rd, PC + insLength);
            PC += (UInt32)offset;
            return true;
        }

        /// <summary>
        /// Jump And Link Register命令
        /// レジスタrs1+offsetをpcに書き込み、
        /// 次の命令アドレスだった値(pc+4)をレジスタrdに書き込む
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">ベースのアドレス</param>
        /// <param name="offset">ジャンプするオフセット値</param>
        public bool Jalr(Register rd, Register rs1, Int32 offset, UInt32 insLength = 4U) {
            UInt32 t = PC + insLength;
            PC = (GetValue(rs1) + (UInt32)offset) & ~1u;
            SetValue(rd, t);
            return true;
        }

        #endregion

        #region Risv-V CPU Branch系命令

        /// <summary>
        /// Branch if Equal命令
        /// レジスタrs1の値とrs2の値が同じ場合、pcにoffsetを加える
        /// </summary>
        /// <param name="rs1">被比較数</param>
        /// <param name="rs2">比較数</param>
        /// <param name="offset">ジャンプするオフセット値</param>
        public bool Beq(Register rs1, Register rs2, Int32 offset, UInt32 insLength = 4U) {
            if (GetValue(rs1) == GetValue(rs2)) {
                PC += (UInt32)offset;
            } else {
                IncrementPc(insLength);
            }
            return true;
        }

        /// <summary>
        /// Branch if Not Equal命令
        /// レジスタrs1の値とrs2の値が異なる場合、pcにoffsetを加える
        /// </summary>
        /// <param name="rs1">被比較数</param>
        /// <param name="rs2">比較数</param>
        /// <param name="offset">ジャンプするオフセット値</param>
        public bool Bne(Register rs1, Register rs2, Int32 offset, UInt32 insLength = 4U) {
            if (GetValue(rs1) != GetValue(rs2)) {
                PC += (UInt32)offset;
            } else {
                IncrementPc(insLength);
            }
            return true;
        }

        /// <summary>
        /// Branch if Less Then命令
        /// レジスタrs1の値がrs2の値より小さい場合、pcにoffsetを加える
        /// </summary>
        /// <param name="rs1">被比較数</param>
        /// <param name="rs2">比較数</param>
        /// <param name="offset">ジャンプするオフセット値</param>
        public bool Blt(Register rs1, Register rs2, Int32 offset, UInt32 insLength = 4U) {
            if ((Int32)GetValue(rs1) < (Int32)GetValue(rs2)) {
                PC += (UInt32)offset;
            } else {
                IncrementPc(insLength);
            }
            return true;
        }

        /// <summary>
        /// Branch if Greater Then or Equal命令
        /// レジスタrs1の値がrs2の値以上の場合、pcにoffsetを加える
        /// </summary>
        /// <param name="rs1">被比較数</param>
        /// <param name="rs2">比較数</param>
        /// <param name="offset">ジャンプするオフセット値</param>
        public bool Bge(Register rs1, Register rs2, Int32 offset, UInt32 insLength = 4U) {
            if ((Int32)GetValue(rs1) >= (Int32)GetValue(rs2)) {
                PC += (UInt32)offset;
            } else {
                IncrementPc(insLength);
            }
            return true;
        }

        /// <summary>
        /// Branch if Less Then, Unsigned命令
        /// レジスタrs1の値がrs2の値より小さい場合、pcにoffsetを加える
        /// </summary>
        /// <param name="rs1">被比較数</param>
        /// <param name="rs2">比較数</param>
        /// <param name="offset">ジャンプするオフセット値</param>
        public bool Bltu(Register rs1, Register rs2, Int32 offset, UInt32 insLength = 4U) {
            if (GetValue(rs1) < GetValue(rs2)) {
                PC += (UInt32)offset;
            } else {
                IncrementPc(insLength);
            }
            return true;
        }

        /// <summary>
        /// Branch if Greater Then or Equal命令
        /// レジスタrs1の値がrs2の値以上の場合、pcにoffsetを加える
        /// </summary>
        /// <param name="rs1">被比較数</param>
        /// <param name="rs2">比較数</param>
        /// <param name="offset">ジャンプするオフセット値</param>
        public bool Bgeu(Register rs1, Register rs2, Int32 offset, UInt32 insLength = 4U) {
            if (GetValue(rs1) >= GetValue(rs2)) {
                PC += (UInt32)offset;
            } else {
                IncrementPc(insLength);
            }
            return true;
        }

        #endregion

        #region Risc-V CPU 特権命令

        /// <summary>
        /// Fence Memory and I/O命令
        /// </summary>
        /// <param name="predsucc"></param>
        /// <returns>処理の成否</returns>
        public bool Fence(byte predsucc, UInt32 insLength = 4U) {
            IncrementPc(insLength);
            return true;
        }

        /// <summary>
        /// Fence Instruction Stream命令
        /// </summary>
        /// <returns>処理の成否</returns>
        public bool FenceI(UInt32 insLength = 4U) {
            IncrementPc(insLength);
            return true;
        }

        /// <summary>
        /// Environment Call命令
        /// 環境呼び出し例外を起こして、実行環境を呼び出す
        /// </summary>
        /// <returns>処理の成否</returns>
        public bool Ecall(UInt32 insLength = 4U) {
            PrivilegeLevel prev_level = CurrentMode;
            CurrentMode = PrivilegeLevel.MachineMode;
            CSRegisters[CSR.mepc] = PC;
            throw new RiscvEnvironmentCallException(prev_level, this);
        }

        /// <summary>
        /// Environment Break命令
        /// ブレークポイント例外を起こして、実行環境を呼び出す
        /// </summary>
        /// <returns>処理の成否</returns>
        public bool Ebreak(UInt32 insLength = 4U) {
            throw new RiscvException(RiscvExceptionCause.Breakpoint, PC, this);
        }

        /// <summary>
        /// Wait For Interrupt命令
        /// </summary>
        /// <returns>処理の成否</returns>
        public bool Wfi(UInt32 insLength = 4U) {

            // 割り込み待ちモードにセット
            IsWaitMode = true;
            
            // 割り込み待ちタイマーリセット
            WaitTimer = 0;

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
        public bool SfenceVma(Register rs1, Register rs2, UInt32 insLength = 4U) {
            if (((StatusCSR)CSRegisters[CSR.mstatus]).TVM && CurrentMode <= PrivilegeLevel.SupervisorMode) {
                throw new RiscvException(RiscvExceptionCause.IllegalInstruction, IR, this);
            }

            Mem.TlbFlush(GetValue(rs1), GetValue(rs2));

            IncrementPc(insLength);
            return true;
        }

        /// <summary>
        /// Machine-Mode Exception Return
        /// マシンモードの例外ハンドラから戻る
        /// </summary>
        /// <returns>処理の成否</returns>
        public bool Mret(UInt32 insLength = 4U) {
            StatusCSR mstatus = CSRegisters[CSR.mstatus];

            PrivilegeLevel priv_level = (PrivilegeLevel)mstatus.MPP;

            mstatus.MIE = mstatus.MPIE;
            mstatus.MPIE = true;
            mstatus.MPP = 0;
            CSRegisters[CSR.mstatus]= mstatus;

            CurrentMode = priv_level;

            PC = CSRegisters[CSR.mepc];

            return true;
        }

        /// <summary>
        /// Supervisor-Mode Exception Return
        /// スーパーバイザモードの例外ハンドラから戻る
        /// </summary>
        /// <returns>処理の成否</returns>
        public bool Sret(UInt32 insLength = 4U) {
            if (((StatusCSR)CSRegisters[CSR.mstatus]).TSR && CurrentMode <= PrivilegeLevel.SupervisorMode) {
                throw new RiscvException(RiscvExceptionCause.IllegalInstruction, IR, this);
            }
            StatusCSR sstatus = CSRegisters[CSR.sstatus];

            PrivilegeLevel priv_level = sstatus.SPP ? PrivilegeLevel.SupervisorMode : PrivilegeLevel.UserMode;

            sstatus.SIE = sstatus.SPIE;
            sstatus.SPIE = true;
            sstatus.SPP = false;
            CSRegisters[CSR.sstatus] = sstatus;

            CurrentMode = priv_level;

            PC = CSRegisters[CSR.sepc];

            return true;
        }

        /// <summary>
        /// User-Mode Exception Return
        /// ユーザモードの例外ハンドラから戻る
        /// </summary>
        /// <returns>処理の成否</returns>
        public bool Uret(UInt32 insLength = 4U) {
            if (((StatusCSR)CSRegisters[CSR.mstatus]).TSR) {
                throw new RiscvException(RiscvExceptionCause.IllegalInstruction, IR, this);
            }
            StatusCSR ustatus = CSRegisters[CSR.ustatus];

            PrivilegeLevel priv_level = PrivilegeLevel.UserMode;

            ustatus.UIE = ustatus.UPIE;
            ustatus.UPIE = true;
            CSRegisters[CSR.ustatus] =ustatus;

            CurrentMode = priv_level;

            PC = CSRegisters[CSR.uepc];

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
        public bool Csrrw(Register rd, Register rs1, CSR csr, UInt32 insLength = 4U) {
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
        public bool Csrrs(Register rd, Register rs1, CSR csr, UInt32 insLength = 4U) {
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
        public bool Csrrc(Register rd, Register rs1, CSR csr, UInt32 insLength = 4U) {
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
        public bool Csrrwi(Register rd, byte zImmediate, CSR csr, UInt32 insLength = 4U) {
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
        public bool Csrrsi(Register rd, byte zImmediate, CSR csr, UInt32 insLength = 4U) {
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
        public bool Csrrci(Register rd, byte zImmediate, CSR csr, UInt32 insLength = 4U) {
            UInt32 t = CSRegisters[csr];
            SetCSR(csr, 'c', zImmediate);
            SetValue(rd, t);
            IncrementPc(insLength);
            return true;
        }

        #endregion

        #endregion

        #region RISC-V命令メソッド用インターフェース

        #region コントロール・ステータスレジスタ操作 for CSR命令

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
            PrivilegeLevel instructionLevel = (PrivilegeLevel)(((UInt32)name >> 8) & 0x3U);
            if (((UInt16)name & 0xC00u) != 0xC00u && instructionLevel <= CurrentMode) {

                // TVM=1かつスーパーバイザモード以下の場合、satpCSRの読み書きは不正命令例外を発生させる
                if (((StatusCSR)CSRegisters[CSR.mstatus]).TVM && CurrentMode <= PrivilegeLevel.SupervisorMode && name == CSR.satp) {
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
            PrivilegeLevel instructionLevel = (PrivilegeLevel)(((UInt32)name >> 8) & 0x3U);
            if (instructionLevel <= CurrentMode) {

                // TVM=1かつスーパーバイザモード以下の場合、satpCSRの読み書きは不正命令例外を発生させる
                if (((StatusCSR)CSRegisters[CSR.mstatus]).TVM && CurrentMode <= PrivilegeLevel.SupervisorMode && name == CSR.satp) {
                    throw new RiscvException(RiscvExceptionCause.IllegalInstruction, IR, this);
                }

                // ユーザモードカウンタ・タイマを読み込む場合
                // mcounteren,scounterenにビットが設定されていない場合は、
                // 参照不可のため、不正命令例外を発行する
                if ((CSR.cycle <= name && name <= CSR.hpmcounter31) ||
                    (CSR.cycleh <= name && name <= CSR.hpmcounter31h)) {

                    if (CurrentMode == PrivilegeLevel.UserMode) {
                        if (((CSRegisters[CSR.mcounteren] & (1u << ((int)name & 0x01f))) == 0) ||
                            ((CSRegisters[CSR.scounteren] & (1u << ((int)name & 0x01f))) == 0)) {
                            throw new RiscvException(RiscvExceptionCause.IllegalInstruction, IR, this);
                        }
                    } else if (CurrentMode == PrivilegeLevel.SupervisorMode) {
                        if (((CSRegisters[CSR.mcounteren] >> ((int)name & 0x01f)) & 0x1U) == 0) {
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

        #region public 整数レジスタ操作 for ALU/LSUクラス

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

        #region public 浮動小数点レジスタ for ALU/LSUクラス

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
            CSRegisters[CSR.fflags] = fcsr & 0x1fU;
        }

        /// <summary>
        /// 浮動小数点レジスタ・fcsrの状態を返す
        /// </summary>
        /// <returns></returns>
        public bool IsFPAvailable() {
            return ((StatusCSR)CSRegisters[CSR.mstatus]).FS != 0;
        }
        #endregion

        #endregion

        #region 外部(ハードウェアスレッド/ユーザ)用インターフェース

        /// <summary>
        /// プログラムカウントを 引数で指定した分 増加させ、次の命令アドレスを指すように更新する
        /// 引数で指定しない場合は 4 増加させる
        /// </summary>
        /// <param name="instructionLength">PCの増数 32bit長命令の場合は4、16bit長命令の場合は2を指定する</param>
        public void IncrementPc(UInt32 instructionLength = 4U) {
            PC += instructionLength;
        }

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
            CSRegisters.Misa |= 1U << (Char.ToUpper(isa) - 'A');

            // 追加標準機能をサポートする場合、Gもセットする
            if (Char.ToUpper(isa) != 'E' && Char.ToUpper(isa) != 'I') {
                CSRegisters.Misa |= 1U << ('G' - 'A');
            }
        }

        /// <summary>
        /// レジスタを全てクリアし、エントリポイントをPCに設定する
        /// </summary>
        public void ClearAndSetPC(UInt32 PhysicalAddress, UInt32 VirtualAddress, UInt32 entryOffset) {
            Registers.Select(r => Registers[r.Key] = 0);
            CSRegisters.Select(c => CSRegisters[c.Key] = 0);

            CurrentMode = PrivilegeLevel.MachineMode;

            Mem.VAddr = VirtualAddress ;
            Mem.Offset = entryOffset;
            PC = PhysicalAddress - VirtualAddress;
        }

        /// <summary>
        /// <para>整数レジスタ、浮動小数点レジスタ、コントロール・ステータスレジスタを結合したDictionary型データを取得します。</para>
        /// <para>Key: 整数レジスタの場合は"x0"～"x31"、浮動小数点レジスタの場合は"f0"～"f31"、コントロール・ステータスレジスタは <see cref="CSR"/>列挙型の名前を指定します</para>
        /// <para>Value: 整数レジスタ、CSRは32bit長intの値を64bit長long型にキャストした値、浮動小数点レジスタの場合はIEEE 754フォーマットに準拠した単精度・倍精度浮動小数点数バイト配列をlong型にキャストした値</para>
        /// <para>浮動小数点数の値を取得するには <see cref="BitConverter.GetBytes(long)"/>、 および、<see cref="BitConverter.ToSingle(byte[], int)"/> <see cref="BitConverter.ToDouble(byte[], int)"/> を使用してください</para>
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, ulong> GetAllRegisterData() {
            Dictionary<string, ulong> intReg = Registers.ToDictionary(i => "x" + ((int)i.Key), i => (ulong)i.Value);
            Dictionary<string, ulong> fltReg = FPRegisters.ToDictionary(f => "f" + ((int)f.Key), f => (ulong)f.Value);
            Dictionary<string, ulong> csrReg = CSRegisters.ToDictionary(c => Enum.GetName(typeof(CSR), c.Key), c => (ulong)c.Value);
            return intReg.Concat(fltReg).Concat(csrReg).ToDictionary(r => r.Key, r => r.Value);
        }

        /// <summary>
        /// 例外発生時のCSR処理を行う
        /// </summary>
        /// <param name="cause">例外の原因</param>
        /// <param name="tval">例外の原因となったアドレス(ない場合は0)</param>
        public void HandleException(RiscvExceptionCause cause, UInt32 tval) {

            if (CurrentMode >= PrivilegeLevel.MachineMode || (CSRegisters[CSR.medeleg] & (1u << (int)cause)) == 0U || (CSRegisters[CSR.misa] & ((1u << ('S' - 'A')) | (1u << ('U' - 'A')))) == 0U) {
                // マシンモードより下位モードに例外トラップ委譲されていない または、 ユーザモード・スーパーバイザモードの実装がない場合

                // マシンモードでトラップ
                TrapAndSetCSR(PrivilegeLevel.MachineMode, (UInt32)cause, tval);

            } else if (CurrentMode >= PrivilegeLevel.SupervisorMode || (CSRegisters[CSR.sedeleg] & (1u << (int)cause)) == 0U || (CSRegisters[CSR.misa] & (1u << ('U' - 'A'))) == 0U) {
                // スーパーバイザモード下位モードに例外トラップ委譲されていない または、 ユーザモードの実装がない場合

                // スーパーバイザモードでトラップ
                TrapAndSetCSR(PrivilegeLevel.SupervisorMode, (UInt32)cause, tval);

            } else {
                // ユーザモードでトラップ
                TrapAndSetCSR(PrivilegeLevel.UserMode, (UInt32)cause, tval);
            }
        }

        /// <summary>
        /// 割り込み有無の確認と、割り込みがある場合はCSR処理を行う
        /// </summary>
        /// <returns>割り込み有無(true: 割り込みあり, false:割り込みなし)</returns>
        public (bool, RiscvExceptionCause) CheckAndHandleInterrupt() {

            // 割り込み有効判定用変数
            UInt32 enableInterrupt = 0U;

            // 割り込みトラップモード
            PrivilegeLevel trapLevel = 0U;


            StatusCSR mstatus = CSRegisters[CSR.mstatus];

            UInt32 mideleg = CSRegisters[CSR.mideleg];
            UInt32 sideleg = CSRegisters[CSR.sideleg];

            // mstatusのタイムアウト待機フラグがセットされていて、タイムアウト値を越えた場合、不正命令例外を発生させる
            if (IsWaitMode && CurrentMode == PrivilegeLevel.SupervisorMode && mstatus.TW && WaitTimer++ >= WaitTimerMax) {
                throw new RiscvException(RiscvExceptionCause.IllegalInstruction, 0, this);
            }

            // 割り込み待ち変数(>0なら割り込み待ち中)
            UInt32 pendingInterrupt = CSRegisters[CSR.mie] & CSRegisters[CSR.mip];



            // 割り込み有効の判定
            if ((pendingInterrupt & ~mideleg) > 0U) {
                // 割り込みがマシンモードより下位モードに割り込みトラップ委譲されていない場合

                if (CurrentMode == PrivilegeLevel.MachineMode && mstatus.MIE) {
                    // 現在がマシンモードかつ、mstatusのマシンモード割り込みが許可されている場合
                    enableInterrupt = pendingInterrupt & ~mideleg; ;
                    trapLevel = PrivilegeLevel.MachineMode;

                } else if (CurrentMode < PrivilegeLevel.MachineMode) {
                    // 現在がマシンモードより下位モードの場合
                    enableInterrupt = pendingInterrupt & ~mideleg; ;
                    trapLevel = PrivilegeLevel.MachineMode;
                }
            } else if ((pendingInterrupt & mideleg) > 0U && (pendingInterrupt & ~sideleg) > 0U &&
                ((CSRegisters[CSR.misa] & (1u << ('U' - 'A'))) == 0U || (CSRegisters[CSR.misa] & (1u << ('N' - 'A'))) == 0U)) {
                // 割り込みがスーパーバイザモードより下位モードに割り込みトラップ委譲されていないかつ、
                // ユーザモード割り込みがサポートされていない場合

                if (CurrentMode == PrivilegeLevel.SupervisorMode && mstatus.SIE) {
                    // 現在がスーパーバイザモードかつ、mstatusのスーパーバイザモード割り込みが許可されている場合
                    enableInterrupt = pendingInterrupt & ~sideleg;
                    trapLevel = PrivilegeLevel.SupervisorMode;

                } else if (CurrentMode < PrivilegeLevel.SupervisorMode) {
                    // 現在がスーパーバイザモードより下位モードの場合
                    enableInterrupt = pendingInterrupt & ~sideleg;
                    trapLevel = PrivilegeLevel.SupervisorMode;

                }
            } else if ((pendingInterrupt & sideleg) > 0U) {
                // 割り込み待ち状態で、割り込み委譲されている場合

                if (CurrentMode == PrivilegeLevel.UserMode && mstatus.UIE) {
                    // 現在がスーパーバイザモードかつ、mstatusのユーザモード割り込みが許可されている場合
                    enableInterrupt = pendingInterrupt & sideleg;
                    trapLevel = PrivilegeLevel.UserMode;
                }
            }

            // 割り込み要因
            RiscvExceptionCause cause = (RiscvExceptionCause)0;

            if (enableInterrupt == 0U) {
                // 割り込みなしと判定

                if (IsWaitMode) {
                    IsWaitMode = false;
                    WaitTimer = 0;
                    PC += 4U;
                }

                return (false, cause);
            }

            // 割り込み要因の特定
            //   *複数の要因での割り込みの可能性があるため、順位の高いものから特定する
            //   *外部割り込み => ソフトウェア割り込み => タイマ割り込み の順
            if ((enableInterrupt & (1u << (int)((uint)RiscvExceptionCause.MachineExternal - 0x8000_0000U))) > 0) {
                cause = RiscvExceptionCause.MachineExternal;

            } else if ((enableInterrupt & (1u << (int)((uint)RiscvExceptionCause.SupervisorExternal - 0x8000_0000U))) > 0) {
                cause = RiscvExceptionCause.SupervisorExternal;

            } else if ((enableInterrupt & (1u << (int)((uint)RiscvExceptionCause.UserExternal - 0x8000_0000U))) > 0) {
                cause = RiscvExceptionCause.UserExternal;

            } else if ((enableInterrupt & (1u << (int)((uint)RiscvExceptionCause.MachineSoftware - 0x8000_0000U))) > 0){
                cause = RiscvExceptionCause.MachineSoftware;

            } else if ((enableInterrupt & (1u << (int)((uint)RiscvExceptionCause.SupervisorSoftware - 0x8000_0000U))) > 0) {
                cause = RiscvExceptionCause.SupervisorSoftware;

            } else if ((enableInterrupt & (1u << (int)((uint)RiscvExceptionCause.UserSoftware - 0x8000_0000U))) > 0) {
                cause = RiscvExceptionCause.UserSoftware;

            } else if ((enableInterrupt & (1u << (int)((uint)RiscvExceptionCause.MachineTimer - 0x8000_0000U))) > 0){
                cause = RiscvExceptionCause.MachineTimer;

            } else if ((enableInterrupt & (1u << (int)((uint)RiscvExceptionCause.SupervisorTimer - 0x8000_0000U))) > 0) {
                cause = RiscvExceptionCause.SupervisorTimer;

            } else if ((enableInterrupt & (1u << (int)((uint)RiscvExceptionCause.UserTimer - 0x8000_0000U))) > 0) {
                cause = RiscvExceptionCause.UserTimer;
            } else {
                return (false, cause);
            }

            // 特定したモード、要因でトラップ
            TrapAndSetCSR(trapLevel, (UInt32)cause, 0U);
            IsWaitMode = false;
            WaitTimer = 0;
            return (true, cause);
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
        private void TrapAndSetCSR(PrivilegeLevel level, UInt32 cause, UInt32 tval) {
            StatusCSR status;

            UInt32 epc = (cause & 0x8000_0000U) == 0U ? PC : PC + 4;

            switch (level) {
                case PrivilegeLevel.MachineMode:
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
                    CurrentMode = PrivilegeLevel.MachineMode;
                    break;

                case PrivilegeLevel.SupervisorMode:
                    // 割り込み・例外の発生したPCを設定
                    CSRegisters[CSR.sepc] = epc;

                    // 割り込み・例外の原因を設定
                    CSRegisters[CSR.scause] = cause;

                    // トラップ値を設定
                    CSRegisters[CSR.stval] = tval;

                    // 割り込み・例外前のステータスを設定
                    status = CSRegisters[CSR.sstatus];
                    status.SPIE = status.SIE;
                    status.SPP = ((byte)CurrentMode & 1U) > 0U;
                    status.SIE = false;
                    CSRegisters[CSR.sstatus] = status;

                    // プログラムカウンタの設定
                    if (PC != CSRegisters[CSR.stvec]) {
                        PC = CSRegisters[CSR.stvec];
                    }

                    // 実行モードの変更
                    CurrentMode = PrivilegeLevel.SupervisorMode;
                    break;

                case PrivilegeLevel.UserMode:
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
                    CurrentMode = PrivilegeLevel.UserMode;
                    break;
            }
        }


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