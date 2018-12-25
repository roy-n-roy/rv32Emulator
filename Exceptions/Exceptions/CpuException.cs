using System;

namespace RiscVCpu.Exceptions {
    /// <summary>Risc-V CPUで発生した例外を表します</summary>
    public class RiscvCpuException : Exception {
        /// <summary>Risc-V CPU例外のインスタンスを初期化します</summary>
        public RiscvCpuException() : base() {
        }
        /// <summary>指定したメッセージを使用して、Risc-V CPU例外のインスタンスを初期化します</summary>
        /// <param name="message"></param>
        public RiscvCpuException(string message) : base(message) {
        }

        /// <summary>指定したメッセージおよびこの例外の原因となった内部例外への参照を使用して、Risc-V CPU例外のインスタンスを初期化します</summary>
        public RiscvCpuException(string message, Exception inner) : base(message, inner) {
        }
    }

    /// <summary>Risc-V CPUで発生した環境呼び出し例外を表します</summary>
    public class RiscvEnvironmentCallException : RiscvCpuException {
        /// <summary>Risc-V CPU 環境呼び出し例外のインスタンスを初期化します</summary>
        public RiscvEnvironmentCallException() : base() {
        }

        /// <summary>指定したメッセージを使用して、Risc-V CPU 環境呼び出し例外のインスタンスを初期化します</summary>
        public RiscvEnvironmentCallException(string message) : base(message) {
        }

        /// <summary>指定したメッセージおよびこの例外の原因となった内部例外への参照を使用して、Risc-V CPU 環境呼び出し例外のインスタンスを初期化します</summary>
        public RiscvEnvironmentCallException(string message, Exception inner) : base(message, inner) {
        }

    }

    /// <summary>Risc-V CPUで発生したブレークポイント例外を表します</summary>
    public class RiscvBreakpointException : RiscvCpuException {
        /// <summary>Risc-V CPU ブレークポイント例外のインスタンスを初期化します</summary>
        public RiscvBreakpointException() : base() {
        }

        /// <summary>指定したメッセージを使用して、Risc-V CPU ブレークポイント例外のインスタンスを初期化します</summary>
        public RiscvBreakpointException(string message) : base(message) {
        }

        /// <summary>指定したメッセージおよびこの例外の原因となった内部例外への参照を使用して、Risc-V CPU ブレークポイント例外のインスタンスを初期化します</summary>
        public RiscvBreakpointException(string message, Exception inner) : base(message, inner) {
        }
    }

}

