using RiscVCpu.LoadStoreUnit.Exceptions;
using RiscVCpu.RegisterSet;
using System;
using System.Collections.Generic;

namespace RiscVCpu.MemoryHandler {
    /// <summary>メインメモリハンドラ</summary>
    public class RV32_MemoryHandler : RV32_AbstractMemoryHandler {

        /// <summary>メインメモリハンドラ</summary>
        /// <param name="mainMemory">基となるメインメモリのバイト配列</param>
        public RV32_MemoryHandler(byte[] mainMemory) : base(mainMemory) { }

        /// <summary>
        /// メモリハンドラの基となったバイト配列を返す
        /// </summary>
        /// <returns>メモリハンドラの基となったバイト配列</returns>
        public override byte[] GetBytes() {
            return mainMemory;
        }

        /// <summary>
        /// 対象アドレスのメモリが操作可能かどうかを返す
        /// </summary>
        /// <param name="address">メモリアドレス</param>
        /// <param name="length">操作するメモリのバイト長</param>
        /// <returns>メモリ操作の可否</returns>
        public override bool CanOperate(UInt64 address, Int32 length) {
            return true;
        }

        /// <summary>
        /// メモリアドレスを予約する
        /// </summary>
        /// <param name="address">メモリアドレス</param>
        public override void Acquire(UInt64 address, Int32 length) {
        }

        /// <summary>
        /// メモリアドレスの予約を解放する
        /// </summary>
        /// <param name="address">メモリアドレス</param>
        public override void Release(UInt64 address) {
        }

        /// <summary>
        /// メモリアドレスの予約を全て解放する
        /// </summary>
        public override void Reset() {
        }
    }

    /// <summary>メインメモリハンドラ 抽象クラス</summary>
    public abstract class RV32_AbstractMemoryHandler {

        private RV32_RegisterSet reg;

        public byte this[UInt32 address] {
            // メモリへのストア
            set {
                if (address < PAddr || address >= Size - PAddr + Offset) {
                    throw new RiscvException(RiscvExceptionCause.StoreAMOPageFault, address, reg);
                }
                mainMemory[address - PAddr + Offset] = value;
                if (HostAccessAddress.Contains(address)) {
                    throw new HostAccessTrap();
                }
            }
            // メモリからのロード
            get {
                if (HostAccessAddress.Contains(address)) {
                    throw new HostAccessTrap();
                }
                if (address < PAddr || address >= Size - PAddr + Offset) {
                    throw new RiscvException(RiscvExceptionCause.LoadPageFault, address, reg);
                }
                return mainMemory[address - PAddr + Offset];
            }
        }

        /// <summary>
        /// 指定したアドレスから命令をフェッチする
        /// </summary>
        /// <param name="addr">命令のアドレス</param>
        /// <returns>32bit長 Risc-V命令</returns>
        public UInt32 FetchInstruction(UInt32 address) {
            if (HostAccessAddress.Contains(address)) {
                throw new HostAccessTrap();
            }
            if (address < PAddr || address >= Size - PAddr + Offset) {
                throw new RiscvException(RiscvExceptionCause.InstructionPageFault, address, reg);
            }
            return BitConverter.ToUInt32(mainMemory, (int)(address - PAddr + Offset));
        }

        private protected readonly byte[] mainMemory;
        public readonly HashSet<UInt64> HostAccessAddress;

        public UInt64 Size { get => (UInt64)mainMemory.LongLength; }
        public UInt64 PAddr { get; set; }
        public UInt64 Offset { get; set; }

        internal void SetRegisterSet(RV32_RegisterSet registerSet) {
            reg = registerSet;
        }

        /// <summary>メインメモリハンドラ</summary>
        /// <param name="mainMemory">基となるメインメモリのバイト配列</param>
        public RV32_AbstractMemoryHandler(byte[] mainMemory) {
            this.mainMemory = mainMemory;
            HostAccessAddress = new HashSet<UInt64>();
        }
        /// <summary>
        /// メモリハンドラの基となったバイト配列を返す
        /// </summary>
        /// <returns>メモリハンドラの基となったバイト配列</returns>
        public abstract byte[] GetBytes();

        /// <summary>
        /// 対象アドレスのメモリが操作可能かどうかを返す
        /// </summary>
        /// <param name="address">メモリアドレス</param>
        /// <param name="length">操作するメモリのバイト長</param>
        /// <returns>メモリ操作の可否</returns>
        public abstract bool CanOperate(UInt64 address, Int32 length);

        /// <summary>
        /// メモリアドレスを予約する
        /// </summary>
        /// <param name="address">メモリアドレス</param>
        public abstract void Acquire(UInt64 address, Int32 length);

        /// <summary>
        /// メモリアドレスの予約を解放する
        /// </summary>
        /// <param name="address">メモリアドレス</param>
        public abstract void Release(UInt64 address);

        /// <summary>
        /// メモリアドレスの予約を全て解放する
        /// </summary>
        public abstract void Reset();

    }

    /// <summary>
    /// ホストへトラップを返す
    /// </summary>
    public class HostAccessTrap : Exception {
    }
}
