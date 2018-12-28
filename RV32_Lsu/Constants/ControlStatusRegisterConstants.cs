namespace RiscVCpu.LoadStoreUnit.Constants {

     /// <summary>特権レベル</summary>
    public enum PrivilegeLevels : ushort {
        /// <summary>ユーザモード</summary>
        UserMode = 0x000,
        /// <summary>ハイパーバイザモード</summary>
        SupervisorMode = 0x100,
        /// <summary>マシンモード</summary>
        MachineMode = 0x300,
    }

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

        // 各モードで読書可能なビット
        public static uint MModeCanRead = 0b1000_0000_0111_1111_1111_1001_1011_1011u;
        public static uint SModeCanRead = 0b1000_0000_0000_1101_1110_0001_0011_0011u;
        public static uint UModeCanRead = 0b1000_0000_0000_1101_1110_0000_0001_0001u;

        // 変数
        public bool SD { get; set; }
        public bool TSR { get; set; }
        public bool TW { get; set; }
        public bool TVM { get; set; }
        public bool MXR { get; set; }
        public bool SUM { get; set; }
        public bool MPRV { get; set; }
        public byte XS { get; set; }
        public byte FS { get; set; }
        public byte MPP { get; set; }
        public bool SPP { get; set; }
        public bool MPIE { get; set; }
        public bool SPIE { get; set; }
        public bool UPIE { get; set; }
        public bool MIE { get; set; }
        public bool SIE { get; set; }
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
        public static explicit operator StatusCSR(uint v) {
            return new StatusCSR(v);
        }

        public static explicit operator uint(StatusCSR v) {
            uint value = 0;

            value += (uint)(v.SD ? 1 << 31 : 0);
            value += (uint)(v.TSR ? 1 << 22 : 0);
            value += (uint)(v.TW ? 1 << 21 : 0);
            value += (uint)(v.TVM ? 1 << 20 : 0);
            value += (uint)(v.MXR ? 1 << 19 : 0);
            value += (uint)(v.SUM ? 1 << 18 : 0);
            value += (uint)(v.MPRV ? 1 << 17 : 0);
            value += (uint)v.XS << 15;
            value += (uint)v.FS << 13;
            value += (uint)v.MPP << 11;
            value += (uint)(v.SPP ? 1 << 8 : 0);
            value += (uint)(v.MPIE ? 1 << 7 : 0);
            value += (uint)(v.SPIE ? 1 << 5 : 0);
            value += (uint)(v.UPIE ? 1 << 4 : 0);
            value += (uint)(v.MIE ? 1 << 3 : 0);
            value += (uint)(v.SIE ? 1 << 1 : 0);
            value += (uint)(v.UIE ? 1 : 0);

            if (v.Mode == PrivilegeLevels.SupervisorMode) {
                value &= SModeCanRead;
            } else if (v.Mode == PrivilegeLevels.UserMode) {
                value &= UModeCanRead;
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
        // 各モードで読取り可能なビット
        public static uint MModeCanRead = 0b1011_1011_1011u;
        public static uint SModeCanRead = 0b0011_0011_0011u;
        public static uint UModeCanRead = 0b0001_0001_0001u;

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
        public static explicit operator InterruptPendingCSR(uint v) {
            return new InterruptPendingCSR(v);
        }

        public static explicit operator uint(InterruptPendingCSR v) {
            uint value = 0;
            value += v.MEIP ? 1u << 11 : 0u;
            value += v.SEIP ? 1u << 9 : 0u;
            value += v.UEIP ? 1u << 8 : 0u;
            value += v.MTIP ? 1u << 7 : 0u;
            value += v.STIP ? 1u << 5 : 0u;
            value += v.UTIP ? 1u << 4 : 0u;
            value += v.MSIP ? 1u << 3 : 0u;
            value += v.SSIP ? 1u << 1 : 0u;
            value += (uint)(v.USIP ? 1u << 0 : 0u);

            if (v.Mode == PrivilegeLevels.SupervisorMode) {
                value &= SModeCanRead;
            } else if (v.Mode == PrivilegeLevels.UserMode) {
                value &= UModeCanRead;
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
        // 各モードで読取り可能なビット
        public static uint MModeCanRead = 0b1011_1011_1011u;
        public static uint SModeCanRead = 0b0011_0011_0011u;
        public static uint UModeCanRead = 0b0001_0001_0001u;

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

        /// <summary>mip, sipなどの割り込み有効CSRを表す構造体</summary>
        public InterruptEnableCSR(uint v) : this() {
            MEIE = (v & 0x00000800) > 0;
            SEIE = (v & 0x00000200) > 0;
            UEIE = (v & 0x00000100) > 0;
            MTIE = (v & 0x00000080) > 0;
            STIE = (v & 0x00000020) > 0;
            UTIE = (v & 0x00000010) > 0;
            MSIE = (v & 0x00000008) > 0;
            SSIE = (v & 0x00000002) > 0;
            USIE = (v & 0x00000001) > 0;
        }

        // キャスト
        public static explicit operator InterruptEnableCSR(uint v) {
            return new InterruptEnableCSR(v);
        }

        public static explicit operator uint(InterruptEnableCSR v) {
            uint value = 0;
            value += (uint)(v.MEIE ? 1 : 0) << 11;
            value += (uint)(v.SEIE ? 1 : 0) << 9;
            value += (uint)(v.UEIE ? 1 : 0) << 8;
            value += (uint)(v.MTIE ? 1 : 0) << 7;
            value += (uint)(v.STIE ? 1 : 0) << 5;
            value += (uint)(v.UTIE ? 1 : 0) << 4;
            value += (uint)(v.MSIE ? 1 : 0) << 3;
            value += (uint)(v.SSIE ? 1 : 0) << 1;
            value += (uint)(v.USIE ? 1 : 0);

            if (v.Mode == PrivilegeLevels.SupervisorMode) {
                value &= SModeCanRead;
            } else if (v.Mode == PrivilegeLevels.UserMode) {
                value &= UModeCanRead;
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
        public bool HPM3 { get; }
        /// <summary>hpmcountern4カウンタ有効ビット</summary>
        public bool HPM4 { get; }
        /// <summary>hpmcountern5カウンタ有効ビット</summary>
        public bool HPM5 { get; }
        /// <summary>hpmcountern6カウンタ有効ビット</summary>
        public bool HPM6 { get; }
        /// <summary>hpmcountern7カウンタ有効ビット</summary>
        public bool HPM7 { get; }
        /// <summary>hpmcountern8カウンタ有効ビット</summary>
        public bool HPM8 { get; }
        /// <summary>hpmcountern9カウンタ有効ビット</summary>
        public bool HPM9 { get; }
        /// <summary>hpmcountern10カウンタ有効ビット</summary>
        public bool HPM10 { get; }
        /// <summary>hpmcountern11カウンタ有効ビット</summary>
        public bool HPM11 { get; }
        /// <summary>hpmcountern12カウンタ有効ビット</summary>
        public bool HPM12 { get; }
        /// <summary>hpmcountern13カウンタ有効ビット</summary>
        public bool HPM13 { get; }
        /// <summary>hpmcountern14カウンタ有効ビット</summary>
        public bool HPM14 { get; }
        /// <summary>hpmcountern15カウンタ有効ビット</summary>
        public bool HPM15 { get; }
        /// <summary>hpmcountern16カウンタ有効ビット</summary>
        public bool HPM16 { get; }
        /// <summary>hpmcountern17カウンタ有効ビット</summary>
        public bool HPM17 { get; }
        /// <summary>hpmcountern18カウンタ有効ビット</summary>
        public bool HPM18 { get; }
        /// <summary>hpmcountern19カウンタ有効ビット</summary>
        public bool HPM19 { get; }
        /// <summary>hpmcountern20カウンタ有効ビット</summary>
        public bool HPM20 { get; }
        /// <summary>hpmcountern21カウンタ有効ビット</summary>
        public bool HPM21 { get; }
        /// <summary>hpmcountern22カウンタ有効ビット</summary>
        public bool HPM22 { get; }
        /// <summary>hpmcountern23カウンタ有効ビット</summary>
        public bool HPM23 { get; }
        /// <summary>hpmcountern24カウンタ有効ビット</summary>
        public bool HPM24 { get; }
        /// <summary>hpmcountern25カウンタ有効ビット</summary>
        public bool HPM25 { get; }
        /// <summary>hpmcountern26カウンタ有効ビット</summary>
        public bool HPM26 { get; }
        /// <summary>hpmcountern27カウンタ有効ビット</summary>
        public bool HPM27 { get; }
        /// <summary>hpmcountern28カウンタ有効ビット</summary>
        public bool HPM28 { get; }
        /// <summary>hpmcountern29カウンタ有効ビット</summary>
        public bool HPM29 { get; }
        /// <summary>hpmcountern30カウンタ有効ビット</summary>
        public bool HPM30 { get; }
        /// <summary>hpmcountern31カウンタ有効ビット</summary>
        public bool HPM31 { get; }

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
        public static explicit operator CounterEnableCSR(uint v) {
            return new CounterEnableCSR(v);
        }

        public static explicit operator uint(CounterEnableCSR v) {
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

}
