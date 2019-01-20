using RV32_Register.Constants;
using System;

namespace RV32_Register.Exceptions {
    [Serializable]
    /// <summary>Risc-V CPUで発生した例外を表します</summary>
    public class RiscvException : ArgumentException {

        /// <summary>Risc-V CPU例外のインスタンスを初期化します</summary>
        public RiscvException(RiscvExceptionCause cause, UInt32 tval, RV32_RegisterSet reg) : base(Enum.GetName(typeof(RiscvExceptionCause), cause)) {
            // nullチェック
            if (reg is null) {
                throw new ArgumentNullException("reg");
            }

            Data.Add("pc", reg.PC);
            Data.Add("cause", cause);
            Data.Add("tval", tval);
            reg.HandleException(cause, tval);
        }
        /// <summary>指定したメッセージを使用して、Risc-V CPU例外のインスタンスを初期化します</summary>
        /// <param name="message">エラーを説明するメッセージ</param>
        public RiscvException(RiscvExceptionCause cause, UInt32 tval, RV32_RegisterSet reg, string message) : base(Enum.GetName(typeof(RiscvExceptionCause), cause) + "\r\n" + message) {
            // nullチェック
            if (reg is null) {
                throw new ArgumentNullException("reg");
            }
            if (message is null) {
                throw new ArgumentNullException("message");
            }

            Data.Add("pc", reg.PC);
            Data.Add("cause", cause);
            Data.Add("tval", tval);
            reg.HandleException(cause, tval);
        }

        /// <summary>指定したメッセージおよびこの例外の原因となった内部例外への参照を使用して、Risc-V CPU例外のインスタンスを初期化します</summary>
        /// <param name="message">エラーを説明するメッセージ</param>
        /// <param name="innerException">現在の例外の原因である例外</param>
        public RiscvException(RiscvExceptionCause cause, UInt32 tval, RV32_RegisterSet reg, string message, Exception innerException) : base(Enum.GetName(typeof(RiscvExceptionCause), cause) + "\r\n" + message, innerException) {
            // nullチェック
            if (reg is null) {
                throw new ArgumentNullException("reg");
            }
            if (message is null) {
                throw new ArgumentNullException("message");
            }
            if (innerException is null) {
                throw new ArgumentNullException("innerException");
            }

            Data.Add("pc", reg.PC);
            Data.Add("cause", cause);
            Data.Add("tval", tval);
            reg.HandleException(cause, tval);
        }
    }

    [Serializable]
    /// <summary>Risc-V CPUで発生した環境呼び出し例外を表します</summary>
    public class RiscvEnvironmentCallException : RiscvException {
        /// <summary>Risc-V CPU 環境呼び出し例外のインスタンスを初期化します</summary>
        public RiscvEnvironmentCallException(PrivilegeLevels currentLevel, RV32_RegisterSet rs) : base((RiscvExceptionCause)((byte)currentLevel | 0x8U), 0, rs) {
        }

        /// <summary>指定したメッセージを使用して、Risc-V CPU 環境呼び出し例外のインスタンスを初期化します</summary>
        /// <param name="message">エラーを説明するメッセージ</param>
        public RiscvEnvironmentCallException(PrivilegeLevels currentLevel, RV32_RegisterSet rs, string message) : base((RiscvExceptionCause)((byte)currentLevel | 0x8U), 0, rs, message) {
        }

        /// <summary>指定したメッセージおよびこの例外の原因となった内部例外への参照を使用して、Risc-V CPU 環境呼び出し例外のインスタンスを初期化します</summary>
        /// <param name="message">エラーを説明するメッセージ</param>
        /// <param name="innerException">現在の例外の原因である例外</param>
        public RiscvEnvironmentCallException(PrivilegeLevels currentLevel, RV32_RegisterSet rs, string message, Exception innerException) 
            : base((RiscvExceptionCause)((byte)currentLevel | 0x8U), 0, rs, message, innerException) {
        }
    }
 }

