using System;
using System.Collections.Generic;

namespace RV32_Lsu.MemoryHandler {
    public class RV32_AtomicMemoryHandler : RV32_AbstractMemoryHandler {

        private readonly HashSet<UInt64> reservedAddress;

        /// <summary>アトミック(不可分)なメモリ操作をサポートするメインメモリハンドラ</summary>
        /// <param name="mainMemory">基となるメインメモリのバイト配列</param>
        public RV32_AtomicMemoryHandler(byte[] mainMemory) : base(mainMemory) {
            reservedAddress = new HashSet<UInt64>();
        }

        /// <summary>アトミック(不可分)なメモリ操作をサポートするメインメモリハンドラ</summary>
        /// <param name="mainMemory">基となるメインメモリのバイト配列</param>
        public RV32_AtomicMemoryHandler(RV32_AbstractMemoryHandler mainMemory) : base(mainMemory.GetBytes()) {
            reservedAddress = new HashSet<UInt64>();
        }

        /// <summary>
        /// メモリハンドラの基となったバイト配列を返す
        /// </summary>
        /// <returns>メモリハンドラの基となったバイト配列</returns>
        public override byte[] GetBytes() {
            return mainMemory;
        }

        public override bool CanOperate(UInt64 address, Int32 length) {
            /// <summary>
            /// 対象アドレスのメモリが操作可能かどうかを返す
            /// </summary>
            /// <param name="address">メモリアドレス</param>
            /// <param name="length">操作するメモリのバイト長</param>
            /// <returns>メモリ操作の可否</returns>
            bool result = false;
            for (uint i = 0; i < length; i++) {
                result |= !reservedAddress.Contains(address + i);
            }
            return result;
        }

        /// <summary>
        /// メモリアドレスを予約する
        /// </summary>
        /// <param name="address">メモリアドレス</param>
        public override void Acquire(UInt64 address, Int32 length) {
            for (uint i = 0; i < length; i++) {
                reservedAddress.Add(address + i);
            }
        }

        /// <summary>
        /// メモリアドレスの予約を解放する
        /// </summary>
        /// <param name="address">メモリアドレス</param>
        public override void Release(UInt64 address) {
            reservedAddress.Remove(address);
        }

        /// <summary>
        /// メモリアドレスの予約を全て解放する
        /// </summary>
        public override void Reset() {
            reservedAddress.Clear();
        }
    }
}
