using System;

namespace RV32_Register.Constants {
    /************************
     * CSR 定数、構造体定義 *
     ************************/

    /// <summary>特権レベル</summary>
    public enum PrivilegeLevel : byte {
        /// <summary>ユーザモード</summary>
        UserMode = 0x0,
        /// <summary>スーパーバイザモード</summary>
        SupervisorMode = 0x1,
        /// <summary>マシンモード</summary>
        MachineMode = 0x3,
    }

    /// <summary>丸めモード</summary>
    public enum FloatRoundingMode : byte {
        /// <summary>最近接丸め(偶数)(Round to Nearest, ties to Even)</summary>
        RNE = 0b000,
        /// <summary>0への丸め(Round towards Zero)</summary>
        RTZ = 0b001,
        /// <summary>-∞への丸め(Round Down (towards -∞))</summary>
        RDN = 0b010,
        /// <summary>+∞への丸め(Round Up (towards +∞))</summary>
        RUP = 0b011,
        /// <summary>最近接丸め(0から遠い方向)(Round to Nearest, ties to Max Magnitude)</summary>
        RMM = 0b100,
        /// <summary>命令のrmフィールドを使用する、動的丸め(In instruction's rm eld, selects dynamic rounding mode;)</summary>
        DYN = 0b111,
    }

    /// <summary>割り込み・例外要因</summary>
    public enum RiscvExceptionCause : uint {
        /// <summary>ユーザソフトウェア割り込み</summary>
        UserSoftware = 0x8000_0000U,
        /// <summary>スーパーバイザソフトウェア割り込み</summary>
        SupervisorSoftware = 0x8000_0001U,
        /// <summary>マシンソフトウェア割り込み</summary>
        MachineSoftware = 0x8000_0003U,
        /// <summary>ユーザタイマ割り込み</summary>
        UserTimer = 0x8000_0004U,
        /// <summary>スーパーバイザタイマ割り込み</summary>
        SupervisorTimer = 0x8000_0005U,
        /// <summary>マシンタイマ割り込み</summary>
        MachineTimer = 0x8000_0007U,
        /// <summary>ユーザ外部割り込み</summary>
        UserExternal = 0x8000_0008U,
        /// <summary>スーパーバイザ外部割り込み</summary>
        SupervisorExternal = 0x8000_0009U,
        /// <summary>マシン外部割り込み</summary>
        MachineExternal = 0x8000_000bU,
        /// <summary>命令アドレス非整列化例外</summary>
        InstructionAddressMisaligned = 0x0000_0000U,
        /// <summary>命令アクセス・フォールト例外</summary>
        InstructionAccessFault = 0x0000_0001U,
        /// <summary>不正命令例外</summary>
        IllegalInstruction = 0x0000_0002U,
        /// <summary>ブレークポイント例外</summary>
        Breakpoint = 0x0000_0003U,
        /// <summary>ロードアドレス非整列化例外</summary>
        LoadAddressMisaligned = 0x0000_0004U,
        /// <summary>ロードアクセス・フォールト例外</summary>
        LoadAccessFault = 0x0000_0005U,
        /// <summary>ストアアドレス非整列化例外</summary>
        AMOAddressMisaligned = 0x0000_0006U,
        /// <summary>ストアアクセス・フォールト例外</summary>
        StoreAMOAccessFault = 0x0000_0007U,
        /// <summary>ユーザモードからの環境呼び出し例外</summary>
        EnvironmentCallFromUMode = 0x0000_0008U,
        /// <summary>スーパーバイザモードからの環境呼び出し例外</summary>
        EnvironmentCallFromSMode = 0x0000_0009U,
        /// <summary>マシンモードからの環境呼び出し例外</summary>
        EnvironmentCallFromMMode = 0x0000_000bU,
        /// <summary>命令ページ・フォールト例外</summary>
        InstructionPageFault = 0x0000_000cU,
        /// <summary>ロードページ・フォールト例外</summary>
        LoadPageFault = 0x0000_000dU,
        /// <summary>ストアページ・フォールト例外</summary>
        StoreAMOPageFault = 0x0000_000fU,
    }

    #region CSRアドレス定義
    /// <summary>
    /// RV32I基本命令セットで使用するコントロール・ステータスレジスタのアドレス
    /// アドレスは12bitから成り、最上位4bitがアクセス権限を表す
    /// 上位2bit 00-10:読み書き可, 11:読み取り専用
    /// 下位2bit => 特権レベル(PrivilegeLevels)
    /// </summary>
    public enum CSR : ushort {

        #region ユーザモードCSR
        /// <summary>ユーザ ステータスレジスタ</summary>
        ustatus = 0x000,
        /// <summary>ユーザ 浮動小数点 例外フラグレジスタ</summary>
        fflags = 0x001,
        /// <summary>ユーザ 浮動小数点 丸めモード</summary>
        frm = 0x002,
        /// <summary>ユーザ 浮動小数点 制御・状態レジスタ</summary>
        fcsr = 0x003,
        /// <summary>ユーザ 割り込み有効レジスタ</summary>
        uie = 0x004,
        /// <summary>ユーザ 例外・割り込みハンドラ ベースアドレス</summary>
        utvec = 0x005,

        /// <summary>ユーザ 例外・割り込みハンドラ スクラッチレジスタ</summary>
        uscratch = 0x040,
        /// <summary>ユーザ 例外発生時 プログラムカウンタプログラム</summary>
        uepc = 0x041,
        /// <summary>ユーザ 例外・割り込み原因レジスタ</summary>
        ucause = 0x042,
        /// <summary>ユーザ 例外発生時 メモリアドレスレジスタ</summary>
        utval = 0x043,
        /// <summary>ユーザ 割り込みペンディングレジスタ</summary>
        uip = 0x044,

        /// <summary>ユーザ サイクルカウンタ</summary>
        cycle = 0xc00,
        /// <summary>ユーザ タイマ</summary>
        time = 0xc01,
        /// <summary>ユーザ 命令リタイヤカウンタ</summary>
        instret = 0xc02,
        /// <summary>ユーザ 性能監視カウンタ</summary>
        hpmcounter3 = 0xc03,
        /// <summary>ユーザ 性能監視カウンタ</summary>
        hpmcounter4 = 0xc04,
        /// <summary>ユーザ 性能監視カウンタ</summary>
        hpmcounter5 = 0xc05,
        /// <summary>ユーザ 性能監視カウンタ</summary>
        hpmcounter6 = 0xc06,
        /// <summary>ユーザ 性能監視カウンタ</summary>
        hpmcounter7 = 0xc07,
        /// <summary>ユーザ 性能監視カウンタ</summary>
        hpmcounter8 = 0xc08,
        /// <summary>ユーザ 性能監視カウンタ</summary>
        hpmcounter9 = 0xc09,
        /// <summary>ユーザ 性能監視カウンタ</summary>
        hpmcounter10 = 0xc0a,
        /// <summary>ユーザ 性能監視カウンタ</summary>
        hpmcounter11 = 0xc0b,
        /// <summary>ユーザ 性能監視カウンタ</summary>
        hpmcounter12 = 0xc0c,
        /// <summary>ユーザ 性能監視カウンタ</summary>
        hpmcounter13 = 0xc0d,
        /// <summary>ユーザ 性能監視カウンタ</summary>
        hpmcounter14 = 0xc0e,
        /// <summary>ユーザ 性能監視カウンタ</summary>
        hpmcounter15 = 0xc0f,
        /// <summary>ユーザ 性能監視カウンタ</summary>
        hpmcounter16 = 0xc10,
        /// <summary>ユーザ 性能監視カウンタ</summary>
        hpmcounter17 = 0xc11,
        /// <summary>ユーザ 性能監視カウンタ</summary>
        hpmcounter18 = 0xc12,
        /// <summary>ユーザ 性能監視カウンタ</summary>
        hpmcounter19 = 0xc13,
        /// <summary>ユーザ 性能監視カウンタ</summary>
        /// <summary>ユーザ 性能監視カウンタ</summary>
        hpmcounter20 = 0xc14,
        /// <summary>ユーザ 性能監視カウンタ</summary>
        hpmcounter21 = 0xc15,
        /// <summary>ユーザ 性能監視カウンタ</summary>
        hpmcounter22 = 0xc16,
        /// <summary>ユーザ 性能監視カウンタ</summary>
        hpmcounter23 = 0xc17,
        /// <summary>ユーザ 性能監視カウンタ</summary>
        hpmcounter24 = 0xc18,
        /// <summary>ユーザ 性能監視カウンタ</summary>
        hpmcounter25 = 0xc19,
        /// <summary>ユーザ 性能監視カウンタ</summary>
        hpmcounter26 = 0xc1a,
        /// <summary>ユーザ 性能監視カウンタ</summary>
        hpmcounter27 = 0xc1b,
        /// <summary>ユーザ 性能監視カウンタ</summary>
        hpmcounter28 = 0xc1c,
        /// <summary>ユーザ 性能監視カウンタ</summary>
        hpmcounter29 = 0xc1d,
        /// <summary>ユーザ 性能監視カウンタ</summary>
        hpmcounter30 = 0xc1e,
        /// <summary>ユーザ 性能監視カウンタ</summary>
        hpmcounter31 = 0xc1f,

        /// <summary>ユーザ サイクルカウンタ(上位ビット)</summary>
        cycleh = 0xc80,
        /// <summary>ユーザ タイマ(上位ビット)</summary>
        timeh = 0xc81,
        /// <summary>ユーザ 命令リタイヤカウンタ(上位ビット)</summary>
        instreth = 0xc82,
        /// <summary>ユーザ 性能監視カウンタ(上位ビット)</summary>
        hpmcounter3h = 0xc83,
        /// <summary>ユーザ 性能監視カウンタ(上位ビット)</summary>
        hpmcounter4h = 0xc84,
        /// <summary>ユーザ 性能監視カウンタ(上位ビット)</summary>
        hpmcounter5h = 0xc85,
        /// <summary>ユーザ 性能監視カウンタ(上位ビット)</summary>
        hpmcounter6h = 0xc86,
        /// <summary>ユーザ 性能監視カウンタ(上位ビット)</summary>
        hpmcounter7h = 0xc87,
        /// <summary>ユーザ 性能監視カウンタ(上位ビット)</summary>
        hpmcounter8h = 0xc88,
        /// <summary>ユーザ 性能監視カウンタ(上位ビット)</summary>
        hpmcounter9h = 0xc89,
        /// <summary>ユーザ 性能監視カウンタ(上位ビット)</summary>
        hpmcounter10h = 0x8a,
        /// <summary>ユーザ 性能監視カウンタ(上位ビット)</summary>
        hpmcounter11h = 0xc8b,
        /// <summary>ユーザ 性能監視カウンタ(上位ビット)</summary>
        hpmcounter12h = 0xc8c,
        /// <summary>ユーザ 性能監視カウンタ(上位ビット)</summary>
        hpmcounter13h = 0xc8d,
        /// <summary>ユーザ 性能監視カウンタ(上位ビット)</summary>
        hpmcounter14h = 0xc8e,
        /// <summary>ユーザ 性能監視カウンタ(上位ビット)</summary>
        hpmcounter15h = 0xc8f,
        /// <summary>ユーザ 性能監視カウンタ(上位ビット)</summary>
        hpmcounter16h = 0xc90,
        /// <summary>ユーザ 性能監視カウンタ(上位ビット)</summary>
        hpmcounter17h = 0xc91,
        /// <summary>ユーザ 性能監視カウンタ(上位ビット)</summary>
        hpmcounter18h = 0xc92,
        /// <summary>ユーザ 性能監視カウンタ(上位ビット)</summary>
        hpmcounter19h = 0xc93,
        /// <summary>ユーザ 性能監視カウンタ(上位ビット)</summary>
        hpmcounter20h = 0xc94,
        /// <summary>ユーザ 性能監視カウンタ(上位ビット)</summary>
        hpmcounter21h = 0xc95,
        /// <summary>ユーザ 性能監視カウンタ(上位ビット)</summary>
        hpmcounter22h = 0xc96,
        /// <summary>ユーザ 性能監視カウンタ(上位ビット)</summary>
        hpmcounter23h = 0xc97,
        /// <summary>ユーザ 性能監視カウンタ(上位ビット)</summary>
        hpmcounter24h = 0xc98,
        /// <summary>ユーザ 性能監視カウンタ(上位ビット)</summary>
        hpmcounter25h = 0xc99,
        /// <summary>ユーザ 性能監視カウンタ(上位ビット)</summary>
        hpmcounter26h = 0xc9a,
        /// <summary>ユーザ 性能監視カウンタ(上位ビット)</summary>
        hpmcounter27h = 0xc9b,
        /// <summary>ユーザ 性能監視カウンタ(上位ビット)</summary>
        hpmcounter28h = 0xc9c,
        /// <summary>ユーザ 性能監視カウンタ(上位ビット)</summary>
        hpmcounter29h = 0xc9d,
        /// <summary>ユーザ 性能監視カウンタ(上位ビット)</summary>
        hpmcounter30h = 0xc9e,
        /// <summary>ユーザ 性能監視カウンタ(上位ビット)</summary>
        hpmcounter31h = 0xc9f,

        #endregion

        #region スーパーバイザモードCSR
        /// <summary>スーパーバイザ ステータスレジスタ</summary>
        sstatus = 0x100,
        /// <summary>スーパーバイザ 例外委譲レジスタ</summary>
        sedeleg = 0x102,
        /// <summary>スーパーバイザ 割り込み委譲レジスタ</summary>
        sideleg = 0x103,
        /// <summary>スーパーバイザ 割り込み有効レジスタ</summary>
        sie = 0x104,
        /// <summary>スーパーバイザ 例外・割り込みハンドラ ベースアドレス</summary>
        stvec = 0x105,
        /// <summary>スーパーバイザ カウンタ有効レジスタ</summary>
        scounteren = 0x106,

        /// <summary>スーパーバイザ 例外・割り込みハンドラ スクラッチレジスタ</summary>
        sscratch = 0x140,
        /// <summary>スーパーバイザ 例外発生時 プログラムカウンタプログラム</summary>
        sepc = 0x141,
        /// <summary>スーパーバイザ 例外・割り込み原因レジスタ</summary>
        scause = 0x142,
        /// <summary>スーパーバイザ 例外発生時 メモリアドレスレジスタ</summary>
        stval = 0x143,
        /// <summary>スーパーバイザ 割り込みペンディングレジスタ</summary>
        sip = 0x144,

        /// <summary>スーパーバイザ アドレス変換・保護レジスタ</summary>
        satp = 0x180,

        #endregion

        #region マシンモードCSR
        /// <summary>マシン ベンダーID</summary>
        mvendorid = 0xf11,
        /// <summary>マシン アーキテクチャID</summary>
        marchid = 0xf12,
        /// <summary>マシン 実装ID</summary>
        mimpid = 0xf13,
        /// <summary>マシン ハードウェアスレッドID</summary>
        mhartid = 0xf14,

        /// <summary>マシン ステータスレジスタ</summary>
        mstatus = 0x300,
        misa = 0x301,
        /// <summary>マシン 例外委譲レジスタ</summary>
        medeleg = 0x302,
        /// <summary>マシン 割り込み委譲レジスタ</summary>
        mideleg = 0x303,
        /// <summary>マシン 割り込み有効レジスタ</summary>
        mie = 0x304,
        /// <summary>マシン 例外・割り込みハンドラ ベースアドレス</summary>
        mtvec = 0x305,
        /// <summary>マシン カウンタ有効レジスタ</summary>
        mcounteren = 0x306,

        /// <summary>マシン 例外・割り込みハンドラ スクラッチレジスタ</summary>
        mscratch = 0x340,
        /// <summary>マシン 例外発生時 プログラムカウンタプログラム</summary>
        mepc = 0x341,
        /// <summary>マシン 例外・割り込み原因レジスタ</summary>
        mcause = 0x342,
        /// <summary>マシン 例外発生時 メモリアドレスレジスタ</summary>
        mtval = 0x343,
        /// <summary>マシン 割り込みペンディングレジスタ</summary>
        mip = 0x344,

        /// <summary>マシン メモリ保護 構成レジスタ</summary>
        pmpcfg0 = 0x3a0,
        /// <summary>マシン メモリ保護 構成レジスタ</summary>
        pmpcfg1 = 0x3a1,
        /// <summary>マシン メモリ保護 構成レジスタ</summary>
        pmpcfg2 = 0x3a2,
        /// <summary>マシン メモリ保護 構成レジスタ</summary>
        pmpcfg3 = 0x3a3,
        /// <summary>マシン メモリ保護 アドレスレジスタ</summary>
        pmpaddr0 = 0x3b0,
        /// <summary>マシン メモリ保護 アドレスレジスタ</summary>
        pmpaddr1 = 0x3b1,
        /// <summary>マシン メモリ保護 アドレスレジスタ</summary>
        pmpaddr2 = 0x3b2,
        /// <summary>マシン メモリ保護 アドレスレジスタ</summary>
        pmpaddr3 = 0x3b3,
        /// <summary>マシン メモリ保護 アドレスレジスタ</summary>
        pmpaddr4 = 0x3b4,
        /// <summary>マシン メモリ保護 アドレスレジスタ</summary>
        pmpaddr5 = 0x3b5,
        /// <summary>マシン メモリ保護 アドレスレジスタ</summary>
        pmpaddr6 = 0x3b6,
        /// <summary>マシン メモリ保護 アドレスレジスタ</summary>
        pmpaddr7 = 0x3b7,
        /// <summary>マシン メモリ保護 アドレスレジスタ</summary>
        pmpaddr8 = 0x3b8,
        /// <summary>マシン メモリ保護 アドレスレジスタ</summary>
        pmpaddr9 = 0x3b9,
        /// <summary>マシン メモリ保護 アドレスレジスタ</summary>
        pmpaddr10 = 0x3ba,
        /// <summary>マシン メモリ保護 アドレスレジスタ</summary>
        pmpaddr11 = 0x3bb,
        /// <summary>マシン メモリ保護 アドレスレジスタ</summary>
        pmpaddr12 = 0x3bc,
        /// <summary>マシン メモリ保護 アドレスレジスタ</summary>
        pmpaddr13 = 0x3bd,
        /// <summary>マシン メモリ保護 アドレスレジスタ</summary>
        pmpaddr14 = 0x3be,
        /// <summary>マシン メモリ保護 アドレスレジスタ</summary>
        pmpaddr15 = 0x3bf,

        /// <summary>マシン サイクルカウンタ</summary>
        mcycle = 0xb00,
        /// <summary>マシン タイマ</summary>
        mtime = 0xb01,
        /// <summary>マシン 命令リタイヤカウンタ</summary>
        minstret = 0xb02,
        /// <summary>マシン 性能監視カウンタ</summary>
        mhpmcounter3 = 0xb03,
        /// <summary>マシン 性能監視カウンタ</summary>
        mhpmcounter4 = 0xb04,
        /// <summary>マシン 性能監視カウンタ</summary>
        mhpmcounter5 = 0xb05,
        /// <summary>マシン 性能監視カウンタ</summary>
        mhpmcounter6 = 0xb06,
        /// <summary>マシン 性能監視カウンタ</summary>
        mhpmcounter7 = 0xb07,
        /// <summary>マシン 性能監視カウンタ</summary>
        mhpmcounter8 = 0xb08,
        /// <summary>マシン 性能監視カウンタ</summary>
        mhpmcounter9 = 0xb09,
        /// <summary>マシン 性能監視カウンタ</summary>
        mhpmcounter10 = 0xb0a,
        /// <summary>マシン 性能監視カウンタ</summary>
        mhpmcounter11 = 0xb0b,
        /// <summary>マシン 性能監視カウンタ</summary>
        mhpmcounter12 = 0xb0c,
        /// <summary>マシン 性能監視カウンタ</summary>
        mhpmcounter13 = 0xb0d,
        /// <summary>マシン 性能監視カウンタ</summary>
        mhpmcounter14 = 0xb0e,
        /// <summary>マシン 性能監視カウンタ</summary>
        mhpmcounter15 = 0xb0f,
        /// <summary>マシン 性能監視カウンタ</summary>
        mhpmcounter16 = 0xb10,
        /// <summary>マシン 性能監視カウンタ</summary>
        mhpmcounter17 = 0xb11,
        /// <summary>マシン 性能監視カウンタ</summary>
        mhpmcounter18 = 0xb12,
        /// <summary>マシン 性能監視カウンタ</summary>
        mhpmcounter19 = 0xb13,
        /// <summary>マシン 性能監視カウンタ</summary>
        mhpmcounter20 = 0xb14,
        /// <summary>マシン 性能監視カウンタ</summary>
        mhpmcounter21 = 0xb15,
        /// <summary>マシン 性能監視カウンタ</summary>
        mhpmcounter22 = 0xb16,
        /// <summary>マシン 性能監視カウンタ</summary>
        mhpmcounter23 = 0xb17,
        /// <summary>マシン 性能監視カウンタ</summary>
        mhpmcounter24 = 0xb18,
        /// <summary>マシン 性能監視カウンタ</summary>
        mhpmcounter25 = 0xb19,
        /// <summary>マシン 性能監視カウンタ</summary>
        mhpmcounter26 = 0xb1a,
        /// <summary>マシン 性能監視カウンタ</summary>
        mhpmcounter27 = 0xb1b,
        /// <summary>マシン 性能監視カウンタ</summary>
        mhpmcounter28 = 0xb1c,
        /// <summary>マシン 性能監視カウンタ</summary>
        mhpmcounter29 = 0xb1d,
        /// <summary>マシン 性能監視カウンタ</summary>
        mhpmcounter30 = 0xb1e,
        /// <summary>マシン 性能監視カウンタ</summary>
        mhpmcounter31 = 0xb1f,

        /// <summary>マシン サイクルカウンタ(上位ビット)</summary>
        mcycleh = 0xb80,
        /// <summary>マシン タイマ(上位ビット)</summary>
        mtimeh = 0xb81,
        /// <summary>マシン 命令リタイヤカウンタ(上位ビット)</summary>
        minstreth = 0xb82,
        /// <summary>マシン 性能監視カウンタ(上位ビット)</summary>
        mhpmcounter3h = 0xb83,
        /// <summary>マシン 性能監視カウンタ(上位ビット)</summary>
        mhpmcounter4h = 0xb84,
        /// <summary>マシン 性能監視カウンタ(上位ビット)</summary>
        mhpmcounter5h = 0xb85,
        /// <summary>マシン 性能監視カウンタ(上位ビット)</summary>
        mhpmcounter6h = 0xb86,
        /// <summary>マシン 性能監視カウンタ(上位ビット)</summary>
        mhpmcounter7h = 0xb87,
        /// <summary>マシン 性能監視カウンタ(上位ビット)</summary>
        mhpmcounter8h = 0xb88,
        /// <summary>マシン 性能監視カウンタ(上位ビット)</summary>
        mhpmcounter9h = 0xb89,
        /// <summary>マシン 性能監視カウンタ(上位ビット)</summary>
        mhpmcounter10h = 0xb8a,
        /// <summary>マシン 性能監視カウンタ(上位ビット)</summary>
        mhpmcounter11h = 0xb8b,
        /// <summary>マシン 性能監視カウンタ(上位ビット)</summary>
        mhpmcounter12h = 0xb8c,
        /// <summary>マシン 性能監視カウンタ(上位ビット)</summary>
        mhpmcounter13h = 0xb8d,
        /// <summary>マシン 性能監視カウンタ(上位ビット)</summary>
        mhpmcounter14h = 0xb8e,
        /// <summary>マシン 性能監視カウンタ(上位ビット)</summary>
        mhpmcounter15h = 0xb8f,
        /// <summary>マシン 性能監視カウンタ(上位ビット)</summary>
        mhpmcounter16h = 0xb90,
        /// <summary>マシン 性能監視カウンタ(上位ビット)</summary>
        mhpmcounter17h = 0xb91,
        /// <summary>マシン 性能監視カウンタ(上位ビット)</summary>
        mhpmcounter18h = 0xb92,
        /// <summary>マシン 性能監視カウンタ(上位ビット)</summary>
        mhpmcounter19h = 0xb93,
        /// <summary>マシン 性能監視カウンタ(上位ビット)</summary>
        mhpmcounter20h = 0xb94,
        /// <summary>マシン 性能監視カウンタ(上位ビット)</summary>
        mhpmcounter21h = 0xb95,
        /// <summary>マシン 性能監視カウンタ(上位ビット)</summary>
        mhpmcounter22h = 0xb96,
        /// <summary>マシン 性能監視カウンタ(上位ビット)</summary>
        mhpmcounter23h = 0xb97,
        /// <summary>マシン 性能監視カウンタ(上位ビット)</summary>
        mhpmcounter24h = 0xb98,
        /// <summary>マシン 性能監視カウンタ(上位ビット)</summary>
        mhpmcounter25h = 0xb99,
        /// <summary>マシン 性能監視カウンタ(上位ビット)</summary>
        mhpmcounter26h = 0xb9a,
        /// <summary>マシン 性能監視カウンタ(上位ビット)</summary>
        mhpmcounter27h = 0xb9b,
        /// <summary>マシン 性能監視カウンタ(上位ビット)</summary>
        mhpmcounter28h = 0xb9c,
        /// <summary>マシン 性能監視カウンタ(上位ビット)</summary>
        mhpmcounter29h = 0xb9d,
        /// <summary>マシン 性能監視カウンタ(上位ビット)</summary>
        mhpmcounter30h = 0xb9e,
        /// <summary>マシン 性能監視カウンタ(上位ビット)</summary>
        mhpmcounter31h = 0xb9f,

        /// <summary>マシン 性能監視イベントセレクタ</summary>
        mhpmevent3 = 0x323,
        /// <summary>マシン 性能監視イベントセレクタ</summary>
        mhpmevent4 = 0x324,
        /// <summary>マシン 性能監視イベントセレクタ</summary>
        mhpmevent5 = 0x325,
        /// <summary>マシン 性能監視イベントセレクタ</summary>
        mhpmevent6 = 0x326,
        /// <summary>マシン 性能監視イベントセレクタ</summary>
        mhpmevent7 = 0x327,
        /// <summary>マシン 性能監視イベントセレクタ</summary>
        mhpmevent8 = 0x328,
        /// <summary>マシン 性能監視イベントセレクタ</summary>
        mhpmevent9 = 0x329,
        /// <summary>マシン 性能監視イベントセレクタ</summary>
        mhpmevent10 = 0x32a,
        /// <summary>マシン 性能監視イベントセレクタ</summary>
        mhpmevent11 = 0x32b,
        /// <summary>マシン 性能監視イベントセレクタ</summary>
        mhpmevent12 = 0x32c,
        /// <summary>マシン 性能監視イベントセレクタ</summary>
        mhpmevent13 = 0x32d,
        /// <summary>マシン 性能監視イベントセレクタ</summary>
        mhpmevent14 = 0x32e,
        /// <summary>マシン 性能監視イベントセレクタ</summary>
        mhpmevent15 = 0x32f,
        /// <summary>マシン 性能監視イベントセレクタ</summary>
        mhpmevent16 = 0x330,
        /// <summary>マシン 性能監視イベントセレクタ</summary>
        mhpmevent17 = 0x331,
        /// <summary>マシン 性能監視イベントセレクタ</summary>
        mhpmevent18 = 0x332,
        /// <summary>マシン 性能監視イベントセレクタ</summary>
        mhpmevent19 = 0x333,
        /// <summary>マシン 性能監視イベントセレクタ</summary>
        mhpmevent20 = 0x334,
        /// <summary>マシン 性能監視イベントセレクタ</summary>
        mhpmevent21 = 0x335,
        /// <summary>マシン 性能監視イベントセレクタ</summary>
        mhpmevent22 = 0x336,
        /// <summary>マシン 性能監視イベントセレクタ</summary>
        mhpmevent23 = 0x337,
        /// <summary>マシン 性能監視イベントセレクタ</summary>
        mhpmevent24 = 0x338,
        /// <summary>マシン 性能監視イベントセレクタ</summary>
        mhpmevent25 = 0x339,
        /// <summary>マシン 性能監視イベントセレクタ</summary>
        mhpmevent26 = 0x33a,
        /// <summary>マシン 性能監視イベントセレクタ</summary>
        mhpmevent27 = 0x33b,
        /// <summary>マシン 性能監視イベントセレクタ</summary>
        mhpmevent28 = 0x33c,
        /// <summary>マシン 性能監視イベントセレクタ</summary>
        mhpmevent29 = 0x33d,
        /// <summary>マシン 性能監視イベントセレクタ</summary>
        mhpmevent30 = 0x33e,
        /// <summary>マシン 性能監視イベントセレクタ</summary>
        mhpmevent31 = 0x33f,

        #endregion

        #region デバッグモードCSR
        /// <summary>デバッグ/トレース トリガセレクタ</summary>
        tselect = 0x7a0,
        /// <summary>デバッグ/トレース トリガデータレジスタ</summary>
        tdata1 = 0x7a1,
        /// <summary>デバッグ/トレース トリガデータレジスタ</summary>
        tdata2 = 0x7a2,
        /// <summary>デバッグ/トレース トリガデータレジスタ</summary>
        tdata3 = 0x7a3,

        /// <summary>デバッグ 制御・状態レジスタ</summary>
        dcsr = 0x7b0,
        /// <summary>デバッグ プログラムカウンタ</summary>
        dpc = 0x7b1,
        /// <summary>デバッグ スクラッチレジスタ</summary>
        dscratch = 0x7b2,

        #endregion
    }

    #endregion

    #region CSR構造体定義
    /// <summary>mstatus, sstatusなどのステータスCSRを表す構造体</summary>
    public struct StatusCSR {
        // 定数
        /// <summary>マシンモードで読み込み可能なビット</summary>
        public const uint MModeReadMask = 0b1000_0000_0111_1111_1111_1001_1011_1011U;
        /// <summary>スーパーバイザモードで読み込み可能なビット</summary>
        public const uint SModeReadMask = 0b1000_0000_0000_1101_1110_0001_0011_0011U;
        /// <summary>ユーザモードで読み込み可能なビット</summary>
        public const uint UModeReadMask = 0b1000_0000_0000_1101_1110_0000_0001_0001U;

        /// <summary>マシンモードで書き込み可能なビット</summary>
        public const uint MModeWriteMask = 0b0000_0000_0111_1110_0111_1001_1011_1011U;
        /// <summary>スーパーバイザモードで書き込み可能なビット</summary>
        public const uint SModeWriteMask = 0b0000_0000_0000_1100_0110_0001_0011_0011U;
        /// <summary>ユーザモードで書き込み可能なビット</summary>
        public const uint UModeWriteMask = 0b0000_0000_0000_1100_0110_0000_0001_0001U;

        // 変数
        /// <summary>FSがダーティ、もしくはXSがダーティの場合にtrueとなる(Summarize Dirty)</summary>
        public bool SD { get; set; }
        /// <summary>SRETトラップ(Trap Supervisor-Mode Return)</summary>
        public bool TSR { get; set; }
        /// <summary>タイムアウト待機 true:WFIがSmodeで実行され、制限時間内に完了しない場合不正命令例外を発生さえる</summary>
        public bool TW { get; set; }
        /// <summary>トラップ仮想メモリ true:satpCSRへの読み書き、SFENCE.VMA命令の実効で不正命令例外を発生させる</summary>
        public bool TVM { get; set; }
        /// <summary>実行ファイル読み込み許可 true:読み込み可でないファイルでも実行可である場合は読み込みが可能となる</summary>
        public bool MXR { get; set; }
        /// <summary>スーパーバイザユーザメモリアクセス許可</summary>
        public bool SUM { get; set; }
        /// <summary>権限変更許可(Modify PRiVilege)</summary>
        public bool MPRV { get; set; }
        /// <summary>拡張ユニットステータス(User-mode Extension unit Status)</summary>
        public byte XS { get; set; }
        /// <summary>浮動小数点ユニットステータス(Float-point unit Status)</summary>
        public byte FS { get; set; }
        /// <summary>マシンモード 割り込み前 特権モード(Machine-mode Previous Privilege mode)</summary>
        public byte MPP { get; set; }
        /// <summary>スーバーバイザモード 割り込み前 特権モード(Supervisor-mode Previous Privilege mode)</summary>
        public bool SPP { get; set; }
        /// <summary>マシンモード 割り込み前 割り込み許可(Machine-mode Previous Interrupt Enable)</summary>
        public bool MPIE { get; set; }
        /// <summary>スーパーバイザモード 割り込み前 割り込み許可(Supervisor-mode Previous Interrupt Enable)</summary>
        public bool SPIE { get; set; }
        /// <summary>ユーザモード 割り込み前 割り込み許可(User-mode Previous Interrupt Enable)</summary>
        public bool UPIE { get; set; }
        /// <summary>マシンモード 割り込み許可(Machine-mode Interrupt Enable)</summary>
        public bool MIE { get; set; }
        /// <summary>スーパーバイザ 割り込み許可(Supervisor-mode Interrupt Enable)</summary>
        public bool SIE { get; set; }
        /// <summary>ユーザモード 割り込み許可(User-mode Interrupt Enable)</summary>
        public bool UIE { get; set; }

        // コンストラクタ
        public StatusCSR(uint value) : this() {
            SD = (value & 0x80000000) > 0;
            TSR = (value & 0x00400000) > 0;
            TW = (value & 0x00200000) > 0;
            TVM = (value & 0x00100000) > 0;
            MXR = (value & 0x00080000) > 0;
            SUM = (value & 0x00040000) > 0;
            MPRV = (value & 0x00020000) > 0;
            XS = (byte)((value & 0x00018000) >> 15);
            FS = (byte)((value & 0x00006000) >> 13);
            MPP = (byte)((value & 0x00001800) >> 11);
            SPP = (value & 0x00000100) > 0;
            MPIE = (value & 0x00000080) > 0;
            SPIE = (value & 0x00000020) > 0;
            UPIE = (value & 0x00000010) > 0;
            MIE = (value & 0x00000008) > 0;
            SIE = (value & 0x00000002) > 0;
            UIE = (value & 0x00000001) > 0;
        }

        // キャスト
        public static implicit operator StatusCSR(uint value) {
            return new StatusCSR(value);
        }

        public static implicit operator uint(StatusCSR status) {
            uint value = 0;

            value |= status.SD ? 1U << 31 : 0U;
            value |= status.TSR ? 1U << 22 : 0U;
            value |= status.TW ? 1U << 21 : 0U;
            value |= status.TVM ? 1U << 20 : 0U;
            value |= status.MXR ? 1U << 19 : 0U;
            value |= status.SUM ? 1U << 18 : 0U;
            value |= status.MPRV ? 1U << 17 : 0U;
            value |= (status.XS & 0x3U) << 15;
            value |= (status.FS & 0x3U) << 13;
            value |= (status.MPP & 0x3U) << 11;
            value |= status.SPP ? 1U << 8 : 0U;
            value |= status.MPIE ? 1U << 7 : 0U;
            value |= status.SPIE ? 1U << 5 : 0U;
            value |= status.UPIE ? 1U << 4 : 0U;
            value |= status.MIE ? 1U << 3 : 0U;
            value |= status.SIE ? 1U << 1 : 0U;
            value |= status.UIE ? 1U << 0 : 0U;

            return value;
        }
    }

    /// <summary>mip, sipなどの割り込み保留CSRを表す構造体</summary>
    public struct InterruptPendingCSR {
        // 定数
        /// <summary>マシンモードで読み込み可能なビット</summary>
        public const uint MModeReadMask = 0b1011_0011_1011U;
        /// <summary>スーパーバイザモードで読み込み可能なビット</summary>
        public const uint SModeReadMask = 0b0011_0011_0011U;
        /// <summary>ユーザモードで読み込み可能なビット</summary>
        public const uint UModeReadMask = 0b0001_0001_0001U;

        /// <summary>マシンモードで書き込み可能なビット</summary>
        public const uint MModeWriteMask = 0b0011_0011_1011U;
        /// <summary>スーパーバイザモードで書き込み可能なビット</summary>
        public const uint SModeWriteMask = 0b0001_0000_0011U;
        /// <summary>ユーザモードで書き込み可能なビット</summary>
        public const uint UModeWriteMask = 0b0000_0000_0001U;

        // 変数
        /// <summary>マシン外部割り込み保留ビット</summary>
        public bool MEIP { get; }
        /// <summary>スーパーバイザ外部割り込み保留ビット</summary>
        public bool SEIP { get; set; }
        /// <summary>ユーザ外部割り込み保留ビット</summary>
        public bool UEIP { get; set; }
        /// <summary>マシンタイマ割り込み保留ビット</summary>
        public bool MTIP { get; }
        /// <summary>スーパーバイザタイマ割り込み保留ビット</summary>
        public bool STIP { get; set; }
        /// <summary>ユーザタイマ割り込み保留ビット</summary>
        public bool UTIP { get; set; }
        /// <summary>マシンソフトウェア割り込み保留ビット</summary>
        public bool MSIP { get; }
        /// <summary>スーパーバイザソフトウェア割り込み保留ビット</summary>
        public bool SSIP { get; set; }
        /// <summary>ユーザソフトウェア割り込み保留ビット</summary>
        public bool USIP { get; set; }

        /// <summary>mip, sipなどの割り込み保留CSRを表す構造体</summary>
        public InterruptPendingCSR(uint value) : this() {
            MEIP = (value & 0x00000800) > 0;
            SEIP = (value & 0x00000200) > 0;
            UEIP = (value & 0x00000100) > 0;
            MTIP = (value & 0x00000080) > 0;
            STIP = (value & 0x00000020) > 0;
            UTIP = (value & 0x00000010) > 0;
            MSIP = (value & 0x00000008) > 0;
            SSIP = (value & 0x00000002) > 0;
            USIP = (value & 0x00000001) > 0;
        }

        // キャスト
        public static implicit operator InterruptPendingCSR(uint value) {
            return new InterruptPendingCSR(value);
        }

        public static implicit operator uint(InterruptPendingCSR ip) {
            uint value = 0;
            value |= ip.MEIP ? 1U << 11 : 0U;
            value |= ip.SEIP ? 1U << 9 : 0U;
            value |= ip.UEIP ? 1U << 8 : 0U;
            value |= ip.MTIP ? 1U << 7 : 0U;
            value |= ip.STIP ? 1U << 5 : 0U;
            value |= ip.UTIP ? 1U << 4 : 0U;
            value |= ip.MSIP ? 1U << 3 : 0U;
            value |= ip.SSIP ? 1U << 1 : 0U;
            value |= ip.USIP ? 1U << 0 : 0U;

            return value;
        }
    }

    /// <summary>mip, sipなどの割り込み有効CSRを表す構造体</summary>
    public struct InterruptEnableCSR {
        // 定数
        /// <summary>マシンモードで読み書き可能なビット</summary>
        public const uint MModeMask = 0b1011_1011_1011U;
        /// <summary>スーパーバイザモードで読み書き可能なビット</summary>
        public const uint SModeMask = 0b0011_0011_0011U;
        /// <summary>ユーザモードで読み書き可能なビット</summary>
        public const uint UModeMask = 0b0001_0001_0001U;

        // 変数
        /// <summary>マシン外部割り込み有効ビット</summary>
        public bool MEIE { get; }
        /// <summary>スーパーバイザ外部割り込み有効ビット</summary>
        public bool SEIE { get; set; }
        /// <summary>ユーザ外部割り込み有効ビット</summary>
        public bool UEIE { get; set; }
        /// <summary>マシンタイマ割り込み有効ビット</summary>
        public bool MTIE { get; }
        /// <summary>スーパーバイザタイマ割り込み有効ビット</summary>
        public bool STIE { get; set; }
        /// <summary>ユーザタイマ割り込み有効ビット</summary>
        public bool UTIE { get; set; }
        /// <summary>マシンソフトウェア割り込み有効ビット</summary>
        public bool MSIE { get; }
        /// <summary>スーパーバイザソフトウェア割り込み有効ビット</summary>
        public bool SSIE { get; set; }
        /// <summary>ユーザソフトウェア割り込み有効ビット</summary>
        public bool USIE { get; set; }

        // コンストラクタ
        /// <summary>mip, sipなどの割り込み有効CSRを表す構造体</summary>
        public InterruptEnableCSR(uint value) : this() {
            MEIE = (value & 0x00000800U) > 0;
            SEIE = (value & 0x00000200U) > 0;
            UEIE = (value & 0x00000100U) > 0;
            MTIE = (value & 0x00000080U) > 0;
            STIE = (value & 0x00000020U) > 0;
            UTIE = (value & 0x00000010U) > 0;
            MSIE = (value & 0x00000008U) > 0;
            SSIE = (value & 0x00000002U) > 0;
            USIE = (value & 0x00000001U) > 0;
        }

        // キャスト
        public static implicit operator InterruptEnableCSR(uint value) {
            return new InterruptEnableCSR(value);
        }

        public static implicit operator uint(InterruptEnableCSR ie) {
            uint value = 0;
            value |= ie.MEIE ? 1U << 11 : 0U;
            value |= ie.SEIE ? 1U << 9 : 0U;
            value |= ie.UEIE ? 1U << 8 : 0U;
            value |= ie.MTIE ? 1U << 7 : 0U;
            value |= ie.STIE ? 1U << 5 : 0U;
            value |= ie.UTIE ? 1U << 4 : 0U;
            value |= ie.MSIE ? 1U << 3 : 0U;
            value |= ie.SSIE ? 1U << 1 : 0U;
            value |= ie.USIE ? 1U << 0 : 0U;

            return value;
        }
    }

    /// <summary>mcounteren, scounterenなどのカウンタ有効CSRを表す構造体</summary>
    public struct CounterEnableCSR {

        // 変数
        /// <summary>cycleカウンタ有効ビット</summary>
        public bool CY { get; set; }
        /// <summary>timeカウンタ有効ビット</summary>
        public bool TM { get; set; }
        /// <summary>instretカウンタ有効ビット</summary>
        public bool IR { get; set; }
        /// <summary>hpmcountern3カウンタ有効ビット</summary>
        public bool HPM3 { get; set; }
        /// <summary>hpmcountern4カウンタ有効ビット</summary>
        public bool HPM4 { get; set; }
        /// <summary>hpmcountern5カウンタ有効ビット</summary>
        public bool HPM5 { get; set; }
        /// <summary>hpmcountern6カウンタ有効ビット</summary>
        public bool HPM6 { get; set; }
        /// <summary>hpmcountern7カウンタ有効ビット</summary>
        public bool HPM7 { get; set; }
        /// <summary>hpmcountern8カウンタ有効ビット</summary>
        public bool HPM8 { get; set; }
        /// <summary>hpmcountern9カウンタ有効ビット</summary>
        public bool HPM9 { get; set; }
        /// <summary>hpmcountern10カウンタ有効ビット</summary>
        public bool HPM10 { get; set; }
        /// <summary>hpmcountern11カウンタ有効ビット</summary>
        public bool HPM11 { get; set; }
        /// <summary>hpmcountern12カウンタ有効ビット</summary>
        public bool HPM12 { get; set; }
        /// <summary>hpmcountern13カウンタ有効ビット</summary>
        public bool HPM13 { get; set; }
        /// <summary>hpmcountern14カウンタ有効ビット</summary>
        public bool HPM14 { get; set; }
        /// <summary>hpmcountern15カウンタ有効ビット</summary>
        public bool HPM15 { get; set; }
        /// <summary>hpmcountern16カウンタ有効ビット</summary>
        public bool HPM16 { get; set; }
        /// <summary>hpmcountern17カウンタ有効ビット</summary>
        public bool HPM17 { get; set; }
        /// <summary>hpmcountern18カウンタ有効ビット</summary>
        public bool HPM18 { get; set; }
        /// <summary>hpmcountern19カウンタ有効ビット</summary>
        public bool HPM19 { get; set; }
        /// <summary>hpmcountern20カウンタ有効ビット</summary>
        public bool HPM20 { get; set; }
        /// <summary>hpmcountern21カウンタ有効ビット</summary>
        public bool HPM21 { get; set; }
        /// <summary>hpmcountern22カウンタ有効ビット</summary>
        public bool HPM22 { get; set; }
        /// <summary>hpmcountern23カウンタ有効ビット</summary>
        public bool HPM23 { get; set; }
        /// <summary>hpmcountern24カウンタ有効ビット</summary>
        public bool HPM24 { get; set; }
        /// <summary>hpmcountern25カウンタ有効ビット</summary>
        public bool HPM25 { get; set; }
        /// <summary>hpmcountern26カウンタ有効ビット</summary>
        public bool HPM26 { get; set; }
        /// <summary>hpmcountern27カウンタ有効ビット</summary>
        public bool HPM27 { get; set; }
        /// <summary>hpmcountern28カウンタ有効ビット</summary>
        public bool HPM28 { get; set; }
        /// <summary>hpmcountern29カウンタ有効ビット</summary>
        public bool HPM29 { get; set; }
        /// <summary>hpmcountern30カウンタ有効ビット</summary>
        public bool HPM30 { get; set; }
        /// <summary>hpmcountern31カウンタ有効ビット</summary>
        public bool HPM31 { get; set; }

        /// <summary>mip, sipなどの割り込み有効CSRを表す構造体</summary>
        public CounterEnableCSR(uint value) : this() {
            CY = (value & 0x00000001) > 0;
            TM = (value & 0x00000002) > 0;
            IR = (value & 0x00000004) > 0;
            HPM3 = (value & 0x00000008) > 0;
            HPM4 = (value & 0x00000010) > 0;
            HPM5 = (value & 0x00000020) > 0;
            HPM6 = (value & 0x00000040) > 0;
            HPM7 = (value & 0x00000080) > 0;
            HPM8 = (value & 0x00000100) > 0;
            HPM9 = (value & 0x00000200) > 0;
            HPM10 = (value & 0x00000400) > 0;
            HPM11 = (value & 0x00000800) > 0;
            HPM12 = (value & 0x00001000) > 0;
            HPM13 = (value & 0x00002000) > 0;
            HPM14 = (value & 0x00004000) > 0;
            HPM15 = (value & 0x00008000) > 0;
            HPM16 = (value & 0x00010000) > 0;
            HPM17 = (value & 0x00020000) > 0;
            HPM18 = (value & 0x00040000) > 0;
            HPM19 = (value & 0x00080000) > 0;
            HPM20 = (value & 0x00100000) > 0;
            HPM21 = (value & 0x00200000) > 0;
            HPM22 = (value & 0x00400000) > 0;
            HPM23 = (value & 0x00800000) > 0;
            HPM24 = (value & 0x01000000) > 0;
            HPM25 = (value & 0x02000000) > 0;
            HPM26 = (value & 0x04000000) > 0;
            HPM27 = (value & 0x08000000) > 0;
            HPM28 = (value & 0x10000000) > 0;
            HPM29 = (value & 0x20000000) > 0;
            HPM30 = (value & 0x40000000) > 0;
            HPM31 = (value & 0x80000000) > 0;
        }

        // キャスト
        public static implicit operator CounterEnableCSR(uint value) {
            return new CounterEnableCSR(value);
        }

        public static implicit operator uint(CounterEnableCSR counteren) {
            uint value = 0;
            value += counteren.CY ? 1U << 0 : 0U;
            value += counteren.TM ? 1U << 1 : 0U;
            value += counteren.IR ? 1U << 2 : 0U;
            value += counteren.HPM3 ? 1U << 3 : 0U;
            value += counteren.HPM4 ? 1U << 4 : 0U;
            value += counteren.HPM5 ? 1U << 5 : 0U;
            value += counteren.HPM6 ? 1U << 6 : 0U;
            value += counteren.HPM7 ? 1U << 7 : 0U;
            value += counteren.HPM8 ? 1U << 8 : 0U;
            value += counteren.HPM9 ? 1U << 9 : 0U;
            value += counteren.HPM10 ? 1U << 10 : 0U;
            value += counteren.HPM11 ? 1U << 11 : 0U;
            value += counteren.HPM12 ? 1U << 12 : 0U;
            value += counteren.HPM13 ? 1U << 13 : 0U;
            value += counteren.HPM14 ? 1U << 14 : 0U;
            value += counteren.HPM15 ? 1U << 15 : 0U;
            value += counteren.HPM16 ? 1U << 16 : 0U;
            value += counteren.HPM17 ? 1U << 17 : 0U;
            value += counteren.HPM18 ? 1U << 18 : 0U;
            value += counteren.HPM19 ? 1U << 19 : 0U;
            value += counteren.HPM20 ? 1U << 20 : 0U;
            value += counteren.HPM21 ? 1U << 21 : 0U;
            value += counteren.HPM22 ? 1U << 22 : 0U;
            value += counteren.HPM23 ? 1U << 23 : 0U;
            value += counteren.HPM24 ? 1U << 24 : 0U;
            value += counteren.HPM25 ? 1U << 25 : 0U;
            value += counteren.HPM26 ? 1U << 26 : 0U;
            value += counteren.HPM27 ? 1U << 27 : 0U;
            value += counteren.HPM28 ? 1U << 28 : 0U;
            value += counteren.HPM29 ? 1U << 29 : 0U;
            value += counteren.HPM30 ? 1U << 30 : 0U;
            value += counteren.HPM31 ? 1U << 31 : 0U;
            return value;
        }
    }

    /// <summary>浮動小数点CSRを表す構造体</summary>
    public struct FloatCSR {
        // 定数
        /// <summary>浮動小数点例外フラグ</summary>
        public static uint FflagsMask = 0x1fU;
        /// <summary>丸めモード</summary>
        public static uint FrmMask = 0xe0;

        // 変数
        /// <summary>丸めモード</summary>
        public FloatRoundingMode Frm { get; set; }
        /// <summary>無効な操作(Invalid Operation)</summary>
        public bool NV { get; set; }
        /// <summary>ゼロ除算(Divide By Zero)</summary>
        public bool DZ { get; set; }
        /// <summary>オーバーフロー(Overflow)</summary>
        public bool OF { get; set; }
        /// <summary>アンダーフロー(Underflow)</summary>
        public bool UF { get; set; }
        /// <summary>不正確(Inexact)</summary>
        public bool NX { get; set; }

        // コンストラクタ
        public FloatCSR(uint value) {
            Frm = (FloatRoundingMode)((value & 0xe0U) >> 5);
            NV = (value & 0x10U) > 0;
            DZ = (value & 0x08U) > 0;
            OF = (value & 0x04U) > 0;
            UF = (value & 0x02U) > 0;
            NX = (value & 0x01U) > 0;
        }

        // キャスト
        public static implicit operator FloatCSR(uint value) {
            return new FloatCSR(value);
        }

        public static implicit operator uint(FloatCSR fcsr) {
            uint value = 0;
            value |= (uint)fcsr.Frm << 5;
            value |= fcsr.NV ? 1U << 4 : 0U;
            value |= fcsr.DZ ? 1U << 3 : 0U;
            value |= fcsr.OF ? 1U << 2 : 0U;
            value |= fcsr.UF ? 1U << 1 : 0U;
            value |= fcsr.NX ? 1U << 0 : 0U;
            return value;
        }
    }

    /// <summary>スーパーバイザアドレス変換・保護CSR (Supervisor Address Translation and Protection Register)</summary>
    public struct SatpCSR {
        // 変数
        /// <summary>アドレッシングモード(false: Bare, true: Sv32モード)</summary>
        public bool MODE { get; set; }
        /// <summary>アドレス空間ID (Address Space Identifier)</summary>
        public ushort ASID { get; set; }
        /// <summary>物理ページ番号 (Physical Page Number)</summary>
        public uint PPN { get; set; }

        public SatpCSR(uint value) {
            MODE = (value & 0x8000_0000U) > 0;
            ASID = (ushort)((value & 0x7fc0_0000U) >> 22);
            PPN = (value & 0x003f_ffffU);
        }

        public static implicit operator SatpCSR(uint value) {
            return new SatpCSR(value);
        }

        public static implicit operator uint(SatpCSR satp) {
            uint value = 0;
            value |= satp.MODE ? 1U << 31 : 0U;
            value |= (satp.ASID & 0x01ffU) << 22;
            value |= (satp.PPN & 0x003f_ffffU) << 0;
            return value;
        }
    }

    /// <summary>トラップベクタベースアドレスレジスタ(Trap-Vector Base-Address Register)</summary>
    public struct TvecCSR {
        /// <summary>ベースアドレス</summary>
        public uint BASE { get; set; }
        /// <summary>モード(0:ダイレクト, 1:ベクタード 2≦予約)</summary>
        public byte MODE { get; set; }

        // コンストラクタ
        public TvecCSR(uint value) {
            BASE = value & 0xffff_fffcU; ;
            MODE = (byte)(value & 0x3U);
        }

        // キャスト
        public static implicit operator TvecCSR(uint value) {
            return new TvecCSR(value);
        }

        public static implicit operator uint(TvecCSR tvec) {
            uint value = 0;
            value |= tvec.BASE & 0xffff_fffcU;
            value |= tvec.MODE & 0x3U;
            return value;
        }
    }

    #endregion
}
