using System;
using System.Collections.Generic;

namespace RiscVCpu.MemoryHandler {
    /// <summary>メインメモリハンドラ</summary>
    public class RV32_MemoryHandler : RV32_AbstractMemoryHandler {

        /// <summary>メインメモリハンドラ</summary>
        /// <param name="mainMemory">基となるメインメモリのバイト配列</param>
        public RV32_MemoryHandler(byte[] mainMemory) : base(mainMemory) {}

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

        public byte this[UInt64 index] {
            set {
                mainMemory[index - PAddr + Offset] = value;
                if (HostAccessAddress.Contains(index)) {
                    throw new HostAccessTrap();
                }
            }
            get {
                if (HostAccessAddress.Contains(index)) {
                    throw new HostAccessTrap();
                }
                return mainMemory[index - PAddr + Offset];
            }
        }

        private protected readonly byte[] mainMemory;
        public readonly HashSet<UInt64> HostAccessAddress;

        public UInt64 Size { get => (UInt64)mainMemory.LongLength; }
        public UInt64 PAddr { get; set; }
        public UInt64 Offset { get; set; }

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

        public bool IsFaultAddress(UInt64 address, uint length) {
            return address < PAddr || address + length - 1 >= Size - PAddr + Offset;
        }

        /// <summary>
        /// 指定したアドレスから命令をフェッチする
        /// </summary>
        /// <param name="addr">命令のアドレス</param>
        /// <returns>32bit長 Risc-V命令</returns>
        public UInt32 FetchInstruction(UInt64 addr) {
            if (HostAccessAddress.Contains(addr)) {
                throw new HostAccessTrap();
            }
            return BitConverter.ToUInt32(mainMemory, (int)(addr - PAddr + Offset));
        }
    }

    public class HostAccessTrap : Exception {

    }
}
