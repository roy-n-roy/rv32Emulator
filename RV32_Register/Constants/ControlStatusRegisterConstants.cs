using System;

namespace RV32_Register.Constants {
    /************************
     * CSR 定数、構造体定義 *
     ************************/

    /// <summary>特権レベル</summary>
    public enum PrivilegeLevels : byte {
        /// <summary>ユーザモード</summary>
        UserMode = 0x0,
        /// <summary>ハイパーバイザモード</summary>
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

    /// <summary>割り込み要因</summary>
    public enum RiscvInterruptCause : uint {
        /// <summary>ユーザソフトウェア割り込み</summary>
        UserSoftware = 0x8000_0000u,
        /// <summary>スーパーバイザソフトウェア割り込み</summary>
        SupervisorSoftware = 0x8000_0001u,
        /// <summary>マシンソフトウェア割り込み</summary>
        MachineSoftware = 0x8000_0003u,
        /// <summary>ユーザタイマ割り込み</summary>
        UserTimer = 0x8000_0004u,
        /// <summary>スーパーバイザタイマ割り込み</summary>
        SupervisorTimer = 0x8000_0005u,
        /// <summary>マシンタイマ割り込み</summary>
        MachineTimer = 0x8000_0007u,
        /// <summary>ユーザ外部割り込み</summary>
        UserExternal = 0x8000_0008u,
        /// <summary>スーパーバイザ外部割り込み</summary>
        SupervisorExternal = 0x8000_0009u,
        /// <summary>マシン外部割り込み</summary>
        MachineExternal = 0x8000_000bu,
    }

    /// <summary>例外要因</summary>
    public enum RiscvExceptionCause : uint {
        /// <summary>命令アドレス非整列化例外</summary>
        InstructionAddressMisaligned = 0x0000_0000u,
        /// <summary>命令アクセス・フォールト例外</summary>
        InstructionAccessFault = 0x0000_0001u,
        /// <summary>不正命令例外</summary>
        IllegalInstruction = 0x0000_0002u,
        /// <summary>ブレークポイント例外</summary>
        Breakpoint = 0x0000_0003u,
        /// <summary>ロードアドレス非整列化例外</summary>
        LoadAddressMisaligned = 0x0000_0004u,
        /// <summary>ロードアクセス・フォールト例外</summary>
        LoadAccessFault = 0x0000_0005u,
        /// <summary>ストアアドレス非整列化例外</summary>
        AMOAddressMisaligned = 0x0000_0006u,
        /// <summary>ストアアクセス・フォールト例外</summary>
        StoreAMOAccessFault = 0x0000_0007u,
        /// <summary>ユーザモードからの環境呼び出し例外</summary>
        EnvironmentCallFromUMode = 0x0000_0008u,
        /// <summary>スーパーバイザモードからの環境呼び出し例外</summary>
        EnvironmentCallFromSMode = 0x0000_0009u,
        /// <summary>マシンモードからの環境呼び出し例外</summary>
        EnvironmentCallFromMMode = 0x0000_000bu,
        /// <summary>命令ページ・フォールト例外</summary>
        InstructionPageFault = 0x0000_000cu,
        /// <summary>ロードページ・フォールト例外</summary>
        LoadPageFault = 0x0000_000du,
        /// <summary>ストアページ・フォールト例外</summary>
        StoreAMOPageFault = 0x0000_000fu,
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
        // mstatus, sstatusなどに相当するモード
        private PrivilegeLevels mode;
        public PrivilegeLevels Mode {
            get => mode;
            set {
                if (mode <= PrivilegeLevels.SupervisorMode) {
                    // sstatus, ustatusの場合は、マシンモードのみで読み取れるビットを0にする
                    TSR = false;
                    TW = false;
                    TVM = false;
                    MPRV = false;
                    MPP = 0;
                    MPIE = false;
                    MIE = false;
                }
                if (mode <= PrivilegeLevels.UserMode) {
                    // ustatusの場合は、マシンモード、特権モードのみで読み取れるビットを0にする
                    SPP = false;
                    SPIE = false;
                    SIE = false;
                }
                mode = value;
            }
        }

        // 定数
        /// <summary>マシンモードで読み書き可能なビット</summary>
        public static uint MModeMask = 0b1000_0000_0111_1111_1111_1001_1011_1011u;
        /// <summary>スーパーバイザモードで読み書き可能なビット</summary>
        public static uint SModeMask = 0b1000_0000_0000_1101_1110_0001_0011_0011u;
        /// <summary>ユーザモードで読み書き可能なビット</summary>
        public static uint UModeMask = 0b1000_0000_0000_1101_1110_0000_0001_0001u;

        // 変数
        /// <summary></summary>
        public bool SD { get; set; }
        /// <summary></summary>
        public bool TSR { get; set; }
        /// <summary>タイムアウト待機 true:WFIがSmodeで実行され、制限時間内に完了しない場合不正命令例外を発生さえる</summary>
        public bool TW { get; set; }
        /// <summary>トラップ仮想メモリ true:satpCSRへの読み書き、SFENCE.VMA命令の実効で不正命令例外を発生させる</summary>
        public bool TVM { get; set; }
        /// <summary>実行ファイル読み込み許可 true:読み込み可でないファイルでも実行可である場合は読み込みが可能となる</summary>
        public bool MXR { get; set; }
        /// <summary>スーパーバイザユーザメモリアクセス許可</summary>
        public bool SUM { get; set; }
        /// <summary>権限変更</summary>
        public bool MPRV { get; set; }
        /// <summary></summary>
        public byte XS { get; set; }
        /// <summary></summary>
        public byte FS { get; set; }
        /// <summary></summary>
        public byte MPP { get; set; }
        /// <summary></summary>
        public bool SPP { get; set; }
        /// <summary></summary>
        public bool MPIE { get; set; }
        /// <summary></summary>
        public bool SPIE { get; set; }
        /// <summary></summary>
        public bool UPIE { get; set; }
        /// <summary></summary>
        public bool MIE { get; set; }
        /// <summary></summary>
        public bool SIE { get; set; }
        /// <summary></summary>
        public bool UIE { get; set; }

        // コンストラクタ
        public StatusCSR(uint v) : this() {
            Mode = PrivilegeLevels.MachineMode;
            SD = (v & 0x80000000) > 0;
            TSR = (v & 0x00400000) > 0;
            TW = (v & 0x00200000) > 0;
            TVM = (v & 0x00100000) > 0;
            MXR = (v & 0x00080000) > 0;
            SUM = (v & 0x00040000) > 0;
            MPRV = (v & 0x00020000) > 0;
            XS = (byte)((v & 0x00018000) >> 15);
            FS = (byte)((v & 0x00006000) >> 13);
            MPP = (byte)((v & 0x00001800) >> 11);
            SPP = (v & 0x00000100) > 0;
            MPIE = (v & 0x00000080) > 0;
            SPIE = (v & 0x00000020) > 0;
            UPIE = (v & 0x00000010) > 0;
            MIE = (v & 0x00000008) > 0;
            SIE = (v & 0x00000002) > 0;
            UIE = (v & 0x00000001) > 0;
        }

        // キャスト
        public static implicit operator StatusCSR(uint v) {
            return new StatusCSR(v);
        }

        public static implicit operator uint(StatusCSR v) {
            uint value = 0;

            value |= v.SD ? 1u << 31 : 0u;
            value |= v.TSR ? 1u << 22 : 0u;
            value |= v.TW ? 1u << 21 : 0u;
            value |= v.TVM ? 1u << 20 : 0u;
            value |= v.MXR ? 1u << 19 : 0u;
            value |= v.SUM ? 1u << 18 : 0u;
            value |= v.MPRV ? 1u << 17 : 0u;
            value |= (v.XS & 0x3u) << 15;
            value |= (v.FS & 0x3u) << 13;
            value |= (v.MPP & 0x3u) << 11;
            value |= v.SPP ? 1u << 8 : 0u;
            value |= v.MPIE ? 1u << 7 : 0u;
            value |= v.SPIE ? 1u << 5 : 0u;
            value |= v.UPIE ? 1u << 4 : 0u;
            value |= v.MIE ? 1u << 3 : 0u;
            value |= v.SIE ? 1u << 1 : 0u;
            value |= v.UIE ? 1u << 0 : 0u;

            if (v.Mode == PrivilegeLevels.SupervisorMode) {
                value &= SModeMask;
            } else if (v.Mode == PrivilegeLevels.UserMode) {
                value &= UModeMask;
            }

            return value;
        }
    }

    /// <summary>mip, sipなどの割り込み保留CSRを表す構造体</summary>
    public struct InterruptPendingCSR {
        // mip, sipなどに相当するモード
        private PrivilegeLevels mode;
        public PrivilegeLevels Mode {
            get => mode;
            set {
                mode = value;
                if (Mode <= PrivilegeLevels.SupervisorMode) {
                    // ustatusの場合は、マシンモード、スーパーバイザーモードのみで読み取れるビットを0にする
                    SEIP = false;
                    STIP = false;
                    SSIP = false;
                }
            }
        }
        // 定数
        /// <summary>マシンモードで読み込み可能なビット</summary>
        public static uint MModeReadMask = 0b1011_0011_1011u;
        /// <summary>スーパーバイザモードで読み込み可能なビット</summary>
        public static uint SModeReadMask = 0b0011_0011_0011u;
        /// <summary>ユーザモードで読み込み可能なビット</summary>
        public static uint UModeReadMask = 0b0001_0001_0001u;

        /// <summary>マシンモードで書き込み可能なビット</summary>
        public static uint MModeWriteMask = 0b0011_0011_1011u;
        /// <summary>スーパーバイザモードで書き込み可能なビット</summary>
        public static uint SModeWriteMask = 0b0001_0000_0011u;
        /// <summary>ユーザモードで書き込み可能なビット</summary>
        public static uint UModeWriteMask = 0b0000_0000_0001u;

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
        public InterruptPendingCSR(uint v) : this() {
            MEIP = (v & 0x00000800) > 0;
            SEIP = (v & 0x00000200) > 0;
            UEIP = (v & 0x00000100) > 0;
            MTIP = (v & 0x00000080) > 0;
            STIP = (v & 0x00000020) > 0;
            UTIP = (v & 0x00000010) > 0;
            MSIP = (v & 0x00000008) > 0;
            SSIP = (v & 0x00000002) > 0;
            USIP = (v & 0x00000001) > 0;
        }

        // キャスト
        public static implicit operator InterruptPendingCSR(uint v) {
            return new InterruptPendingCSR(v);
        }

        public static implicit operator uint(InterruptPendingCSR v) {
            uint value = 0;
            value |= v.MEIP ? 1u << 11 : 0u;
            value |= v.SEIP ? 1u << 9 : 0u;
            value |= v.UEIP ? 1u << 8 : 0u;
            value |= v.MTIP ? 1u << 7 : 0u;
            value |= v.STIP ? 1u << 5 : 0u;
            value |= v.UTIP ? 1u << 4 : 0u;
            value |= v.MSIP ? 1u << 3 : 0u;
            value |= v.SSIP ? 1u << 1 : 0u;
            value |= v.USIP ? 1u << 0 : 0u;

            if (v.Mode == PrivilegeLevels.SupervisorMode) {
                value &= SModeReadMask;
            } else if (v.Mode == PrivilegeLevels.UserMode) {
                value &= UModeReadMask;
            }
            return value;
        }
    }

    /// <summary>mip, sipなどの割り込み有効CSRを表す構造体</summary>
    public struct InterruptEnableCSR {
        // mip, sipなどに相当するモード
        private PrivilegeLevels mode;
        public PrivilegeLevels Mode {
            get => mode;
            set {
                mode = value;
                if (Mode <= PrivilegeLevels.SupervisorMode) {
                    // ustatusの場合は、マシンモード、スーパーバイザーモードのみで読み取れるビットを0にする
                    SEIE = false;
                    STIE = false;
                    SSIE = false;
                }
            }
        }

        // 定数
        /// <summary>マシンモードで読み書き可能なビット</summary>
        public static uint MModeMask = 0b1011_1011_1011u;
        /// <summary>スーパーバイザモードで読み書き可能なビット</summary>
        public static uint SModeMask = 0b0011_0011_0011u;
        /// <summary>ユーザモードで読み書き可能なビット</summary>
        public static uint UModeMask = 0b0001_0001_0001u;

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
        public InterruptEnableCSR(uint v) : this() {
            MEIE = (v & 0x00000800u) > 0;
            SEIE = (v & 0x00000200u) > 0;
            UEIE = (v & 0x00000100u) > 0;
            MTIE = (v & 0x00000080u) > 0;
            STIE = (v & 0x00000020u) > 0;
            UTIE = (v & 0x00000010u) > 0;
            MSIE = (v & 0x00000008u) > 0;
            SSIE = (v & 0x00000002u) > 0;
            USIE = (v & 0x00000001u) > 0;
        }

        // キャスト
        public static implicit operator InterruptEnableCSR(uint v) {
            return new InterruptEnableCSR(v);
        }

        public static implicit operator uint(InterruptEnableCSR v) {
            uint value = 0;
            value |= v.MEIE ? 1u << 11 : 0u;
            value |= v.SEIE ? 1u << 9 : 0u;
            value |= v.UEIE ? 1u << 8 : 0u;
            value |= v.MTIE ? 1u << 7 : 0u;
            value |= v.STIE ? 1u << 5 : 0u;
            value |= v.UTIE ? 1u << 4 : 0u;
            value |= v.MSIE ? 1u << 3 : 0u;
            value |= v.SSIE ? 1u << 1 : 0u;
            value |= v.USIE ? 1u << 0 : 0u;

            if (v.Mode == PrivilegeLevels.SupervisorMode) {
                value &= SModeMask;
            } else if (v.Mode == PrivilegeLevels.UserMode) {
                value &= UModeMask;
            }
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
        public CounterEnableCSR(uint v) : this() {
            CY = (v & 0x00000001) > 0;
            TM = (v & 0x00000002) > 0;
            IR = (v & 0x00000004) > 0;
            HPM3 = (v & 0x00000008) > 0;
            HPM4 = (v & 0x00000010) > 0;
            HPM5 = (v & 0x00000020) > 0;
            HPM6 = (v & 0x00000040) > 0;
            HPM7 = (v & 0x00000080) > 0;
            HPM8 = (v & 0x00000100) > 0;
            HPM9 = (v & 0x00000200) > 0;
            HPM10 = (v & 0x00000400) > 0;
            HPM11 = (v & 0x00000800) > 0;
            HPM12 = (v & 0x00001000) > 0;
            HPM13 = (v & 0x00002000) > 0;
            HPM14 = (v & 0x00004000) > 0;
            HPM15 = (v & 0x00008000) > 0;
            HPM16 = (v & 0x00010000) > 0;
            HPM17 = (v & 0x00020000) > 0;
            HPM18 = (v & 0x00040000) > 0;
            HPM19 = (v & 0x00080000) > 0;
            HPM20 = (v & 0x00100000) > 0;
            HPM21 = (v & 0x00200000) > 0;
            HPM22 = (v & 0x00400000) > 0;
            HPM23 = (v & 0x00800000) > 0;
            HPM24 = (v & 0x01000000) > 0;
            HPM25 = (v & 0x02000000) > 0;
            HPM26 = (v & 0x04000000) > 0;
            HPM27 = (v & 0x08000000) > 0;
            HPM28 = (v & 0x10000000) > 0;
            HPM29 = (v & 0x20000000) > 0;
            HPM30 = (v & 0x40000000) > 0;
            HPM31 = (v & 0x80000000) > 0;
        }

        // キャスト
        public static implicit operator CounterEnableCSR(uint v) {
            return new CounterEnableCSR(v);
        }

        public static implicit operator uint(CounterEnableCSR v) {
            uint value = 0;
            value += v.CY ? 1u << 0 : 0u;
            value += v.TM ? 1u << 1 : 0u;
            value += v.IR ? 1u << 2 : 0u;
            value += v.HPM3 ? 1u << 3 : 0u;
            value += v.HPM4 ? 1u << 4 : 0u;
            value += v.HPM5 ? 1u << 5 : 0u;
            value += v.HPM6 ? 1u << 6 : 0u;
            value += v.HPM7 ? 1u << 7 : 0u;
            value += v.HPM8 ? 1u << 8 : 0u;
            value += v.HPM9 ? 1u << 9 : 0u;
            value += v.HPM10 ? 1u << 10 : 0u;
            value += v.HPM11 ? 1u << 11 : 0u;
            value += v.HPM12 ? 1u << 12 : 0u;
            value += v.HPM13 ? 1u << 13 : 0u;
            value += v.HPM14 ? 1u << 14 : 0u;
            value += v.HPM15 ? 1u << 15 : 0u;
            value += v.HPM16 ? 1u << 16 : 0u;
            value += v.HPM17 ? 1u << 17 : 0u;
            value += v.HPM18 ? 1u << 18 : 0u;
            value += v.HPM19 ? 1u << 19 : 0u;
            value += v.HPM20 ? 1u << 20 : 0u;
            value += v.HPM21 ? 1u << 21 : 0u;
            value += v.HPM22 ? 1u << 22 : 0u;
            value += v.HPM23 ? 1u << 23 : 0u;
            value += v.HPM24 ? 1u << 24 : 0u;
            value += v.HPM25 ? 1u << 25 : 0u;
            value += v.HPM26 ? 1u << 26 : 0u;
            value += v.HPM27 ? 1u << 27 : 0u;
            value += v.HPM28 ? 1u << 28 : 0u;
            value += v.HPM29 ? 1u << 29 : 0u;
            value += v.HPM30 ? 1u << 30 : 0u;
            value += v.HPM31 ? 1u << 31 : 0u;
            return value;
        }
    }

    /// <summary>浮動小数点CSRを表す構造体</summary>
    public struct FloatCSR {
        // 定数
        /// <summary>浮動小数点例外フラグ</summary>
        public static uint FflagsMask = 0x1fu;
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
        public FloatCSR(uint v) {
            Frm = (FloatRoundingMode)((v & 0xe0u) >> 5);
            NV = (v & 0x10u) > 0;
            DZ = (v & 0x08u) > 0;
            OF = (v & 0x04u) > 0;
            UF = (v & 0x02u) > 0;
            NX = (v & 0x01u) > 0;
        }

        // キャスト
        public static implicit operator FloatCSR(uint v) {
            return new FloatCSR(v);
        }

        public static implicit operator uint(FloatCSR v) {
            uint value = 0;
            value |= (uint)v.Frm << 5;
            value |= v.NV ? 1u << 4 : 0u;
            value |= v.DZ ? 1u << 3 : 0u;
            value |= v.OF ? 1u << 2 : 0u;
            value |= v.UF ? 1u << 1 : 0u;
            value |= v.NX ? 1u << 0 : 0u;
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

        public SatpCSR(uint v) {
            MODE = (v & 0x8000_0000u) > 0;
            ASID = (ushort)((v & 0x7fc0_0000u) >> 22);
            PPN = (v & 0x003f_ffffu);
        }

        public static implicit operator SatpCSR(uint v) {
            return new SatpCSR(v);
        }

        public static implicit operator uint(SatpCSR v) {
            uint value = 0;
            value |= v.MODE ? 1u << 31 : 0u;
            value |= (v.ASID & 0x01ffu) << 22;
            value |= (v.PPN & 0x003f_ffffu) << 0;
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
        public TvecCSR(uint v) {
            BASE = v & 0xffff_fffcu; ;
            MODE = (byte)(v & 0x3u);
        }

        // キャスト
        public static implicit operator TvecCSR(uint v) {
            return new TvecCSR(v);
        }

        public static implicit operator uint(TvecCSR v) {
            uint value = 0;
            value |= v.BASE & 0xffff_fffcu;
            value |= v.MODE & 0x3u;
            return value;
        }
    }

    #endregion
}
