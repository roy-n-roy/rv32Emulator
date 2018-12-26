using System;

namespace RiscVCpu.Exceptions {
    /// <summary>Risc-V CPUで発生した例外を表します</summary>
    public class RiscvException : Exception {
        /// <summary>
        /// 割り込み・例外要因
        /// </summary>
        public RiscvExceptionCause Cause { get; }

        /// <summary>Risc-V CPU例外のインスタンスを初期化します</summary>
        public RiscvException(RiscvExceptionCause cause) : base(Enum.GetName(typeof(RiscvExceptionCause), cause)) {
            this.Cause = cause;
        }
        /// <summary>指定したメッセージを使用して、Risc-V CPU例外のインスタンスを初期化します</summary>
        /// <param name="message"></param>
        public RiscvException(RiscvExceptionCause cause, string message) : base(Enum.GetName(typeof(RiscvExceptionCause), cause) + "\r\n" + message) {
            this.Cause = cause;
        }

        /// <summary>指定したメッセージおよびこの例外の原因となった内部例外への参照を使用して、Risc-V CPU例外のインスタンスを初期化します</summary>
        public RiscvException(RiscvExceptionCause cause, string message, Exception inner) : base(Enum.GetName(typeof(RiscvExceptionCause), cause) + "\r\n" + message, inner) {
            this.Cause = cause;
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
        InstructionAddressMisaligned = 0x0u,
        /// <summary>命令アクセス・フォールト例外</summary>
        InstructionAccessFault = 0x1u,
        /// <summary>不正命令例外</summary>
        IllegalInstruction = 0x2u,
        /// <summary>ブレークポイント例外</summary>
        Breakpoint = 0x3u,
        /// <summary>ロードアドレス非整列化例外</summary>
        LoadAddressMisaligned = 0x4u,
        /// <summary>ロードアクセス・フォールト例外</summary>
        LoadAccessFault = 0x5u,
        /// <summary>ストアアドレス非整列化例外</summary>
        AMOAddressMisaligned = 0x6u,
        /// <summary>ストアアクセス・フォールト例外</summary>
        StoreAMOAccessFault = 0x7u,
        /// <summary>ユーザモードからの環境呼び出し例外</summary>
        EnvironmentCallFromUMode = 0x8u,
        /// <summary>スーパーバイザモードからの環境呼び出し例外</summary>
        EnvironmentCallFromSMode = 0x9u,
        /// <summary>マシンモードからの環境呼び出し例外</summary>
        EnvironmentCallFromMMode = 0xbu,
        /// <summary>命令ページ・フォールト例外</summary>
        InstructionPageFault = 0xcu,
        /// <summary>ロードページ・フォールト例外</summary>
        LoadPageFault = 0x1du,
        /// <summary>ストアページ・フォールト例外</summary>
        StoreAMOPageFault = 0xfu,
    }
}

