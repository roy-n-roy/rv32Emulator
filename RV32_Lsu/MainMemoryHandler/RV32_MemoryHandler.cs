using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiscVCpu.MemoryHandler {
    /// <summary>メインメモリハンドラ</summary>
    public class RV32_MemoryHandler : RV32_AbstractMemoryHandler {

        /// <summary>メインメモリハンドラ</summary>
        /// <param name="mainMemory">基となるメインメモリのバイト配列</param>
        public RV32_MemoryHandler(byte[] mainMemory) : base(mainMemory) {
        }

        public override byte this[UInt64 index] {
            set { mainMemory[index] = value; }
            get { return mainMemory[index]; }
        }

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
        public override void Acquire(UInt64 address) {
        }

        /// <summary>
        /// メモリアドレスの予約を解放する
        /// </summary>
        /// <param name="address">メモリアドレス</param>
        public override void Release(UInt64 address) {
        }
    }

    /// <summary>メインメモリハンドラ 抽象クラス</summary>
    public abstract class RV32_AbstractMemoryHandler {

        public abstract byte this[UInt64 address] { get; set; }
        private protected readonly byte[] mainMemory;

        /// <summary>メインメモリハンドラ</summary>
        /// <param name="mainMemory">基となるメインメモリのバイト配列</param>
        public RV32_AbstractMemoryHandler(byte[] mainMemory) {
            this.mainMemory = mainMemory;
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
        public abstract void Acquire(UInt64 address);

        /// <summary>
        /// メモリアドレスの予約を解放する
        /// </summary>
        /// <param name="address">メモリアドレス</param>
        public abstract void Release(UInt64 address);

        public UInt32 GetUInt32(int startIndex) {
            return BitConverter.ToUInt32(mainMemory, startIndex);
        }
    }
}
