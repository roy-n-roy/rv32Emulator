using RiscVCpu.LoadStoreUnit.Constants;
using System;

namespace RiscVCpu.LoadStoreUnit.Exceptions {
    /// <summary>Risc-V CPUで発生した例外を表します</summary>
    public class RiscvException : Exception {

        /// <summary>割り込み・例外要因</summary>
        public RiscvExceptionCause Cause { get; }

        /// <summary>Risc-V CPU例外のインスタンスを初期化します</summary>
        public RiscvException(RiscvExceptionCause cause, RV32_RegisterSet rs) : base(Enum.GetName(typeof(RiscvExceptionCause), cause)) {
            this.Cause = cause;
            rs.SetCause(cause);
        }
        /// <summary>指定したメッセージを使用して、Risc-V CPU例外のインスタンスを初期化します</summary>
        /// <param name="message">エラーを説明するメッセージ</param>
        public RiscvException(RiscvExceptionCause cause, RV32_RegisterSet rs, string message) : base(Enum.GetName(typeof(RiscvExceptionCause), cause) + "\r\n" + message) {
            this.Cause = cause;
            rs.SetCause(cause);
        }

        /// <summary>指定したメッセージおよびこの例外の原因となった内部例外への参照を使用して、Risc-V CPU例外のインスタンスを初期化します</summary>
        /// <param name="message">エラーを説明するメッセージ</param>
        /// <param name="innerException">現在の例外の原因である例外</param>
        public RiscvException(RiscvExceptionCause cause, RV32_RegisterSet rs, string message, Exception innerException) : base(Enum.GetName(typeof(RiscvExceptionCause), cause) + "\r\n" + message, innerException) {
            this.Cause = cause;
            rs.SetCause(cause);
        }
    }

    /// <summary>Risc-V CPUで発生した環境呼び出し例外を表します</summary>
    public class RiscvEnvironmentCallException : RiscvException {
        /// <summary>Risc-V CPU 環境呼び出し例外のインスタンスを初期化します</summary>
        public RiscvEnvironmentCallException(PrivilegeLevels currentLevel, RV32_RegisterSet rs) : base((RiscvExceptionCause)(((UInt16)currentLevel >> 8) & 0x8), rs) {
        }

        /// <summary>指定したメッセージを使用して、Risc-V CPU 環境呼び出し例外のインスタンスを初期化します</summary>
        /// <param name="message">エラーを説明するメッセージ</param>
        public RiscvEnvironmentCallException(PrivilegeLevels currentLevel, RV32_RegisterSet rs, string message) : base((RiscvExceptionCause)(((UInt16)currentLevel >> 8) & 0x8), rs, message) {
        }

        /// <summary>指定したメッセージおよびこの例外の原因となった内部例外への参照を使用して、Risc-V CPU 環境呼び出し例外のインスタンスを初期化します</summary>
        /// <param name="message">エラーを説明するメッセージ</param>
        /// <param name="innerException">現在の例外の原因である例外</param>
        public RiscvEnvironmentCallException(PrivilegeLevels currentLevel, RV32_RegisterSet rs, string message, Exception innerException) 
            : base((RiscvExceptionCause)(((UInt16)currentLevel >> 8) & 0x8), rs, message, innerException) {
        }
    }

    /// <summary>Risc-V CPUで発生したブレークポイント例外を表します</summary>
    public class RiscvBreakpointException : RiscvException {
        /// <summary>Risc-V CPU ブレークポイント例外のインスタンスを初期化します</summary>
        public RiscvBreakpointException(RV32_RegisterSet rs) : base(RiscvExceptionCause.Breakpoint, rs) {
        }

        /// <summary>指定したメッセージを使用して、Risc-V CPU ブレークポイント例外のインスタンスを初期化します</summary>
        /// <param name="message">エラーを説明するメッセージ</param>
        public RiscvBreakpointException(string message, RV32_RegisterSet rs) : base(RiscvExceptionCause.Breakpoint, rs, message) {
        }

        /// <summary>指定したメッセージおよびこの例外の原因となった内部例外への参照を使用して、Risc-V CPU ブレークポイント例外のインスタンスを初期化します</summary>
        /// <param name="message">エラーを説明するメッセージ</param>
        /// <param name="innerException">現在の例外の原因である例外</param>
        public RiscvBreakpointException(string message, RV32_RegisterSet rs, Exception innerException) : base(RiscvExceptionCause.Breakpoint, rs, message, innerException) {
        }
    }

    /// <summary>割り込み・例外要因</summary>
    public enum RiscvExceptionCause : uint {
        // 割り込み
        /// <summary>ユーザソフトウェア割り込み</summary>
        UserSoftwareInterrupt = 0x80000000u,
        /// <summary>スーパーバイザソフトウェア割り込み</summary>
        SupervisorSoftwareInterrupt = 0x80000001u,
        /// <summary>マシンソフトウェア割り込み</summary>
        MachineSoftwareInterrupt = 0x80000003u,
        /// <summary>ユーザタイマ割り込み</summary>
        UserTimerInterrupt = 0x80000004u,
        /// <summary>スーパーバイザタイマ割り込み</summary>
        SupervisorTimerInterrupt = 0x80000005u,
        /// <summary>マシンタイマ割り込み</summary>
        MachineTimerInterrupt = 0x80000007u,
        /// <summary>ユーザ外部割り込み</summary>
        UserExternalInterrupt = 0x80000008u,
        /// <summary>スーパーバイザ外部割り込み</summary>
        SupervisorExternalInterrupt = 0x80000009u,
        /// <summary>マシン外部割り込み</summary>
        MachineExternalInterrupt = 0x8000000bu,

        // 例外
        /// <summary>命令アドレス非整列化例外</summary>
        InstructionAddressMisaligned = 0x00000000u,
        /// <summary>命令アクセス・フォールト例外</summary>
        InstructionAccessFault = 0x00000001u,
        /// <summary>不正命令例外</summary>
        IllegalInstruction = 0x00000002u,
        /// <summary>ブレークポイント例外</summary>
        Breakpoint = 0x00000003u,
        /// <summary>ロードアドレス非整列化例外</summary>
        LoadAddressMisaligned = 0x00000004u,
        /// <summary>ロードアクセス・フォールト例外</summary>
        LoadAccessFault = 0x00000005u,
        /// <summary>ストアアドレス非整列化例外</summary>
        AMOAddressMisaligned = 0x00000006u,
        /// <summary>ストアアクセス・フォールト例外</summary>
        StoreAMOAccessFault = 0x00000007u,
        /// <summary>ユーザモードからの環境呼び出し例外</summary>
        EnvironmentCallFromUMode = 0x00000008u,
        /// <summary>スーパーバイザモードからの環境呼び出し例外</summary>
        EnvironmentCallFromSMode = 0x00000009u,
        /// <summary>マシンモードからの環境呼び出し例外</summary>
        EnvironmentCallFromMMode = 0x0000000bu,
        /// <summary>命令ページ・フォールト例外</summary>
        InstructionPageFault = 0x0000000cu,
        /// <summary>ロードページ・フォールト例外</summary>
        LoadPageFault = 0x0000000du,
        /// <summary>ストアページ・フォールト例外</summary>
        StoreAMOPageFault = 0x0000000fu,
    }
}

