using RV32_Lsu.MemoryHandler;
using RV32_Lsu.RegisterSet;
using System;
using System.Collections.Generic;

namespace RV32_Lsu {

    /// <summary>
    /// Risc-V 32bitCPU LSU
    /// </summary>
    public class RV32_LoadStoreUnit {
        private readonly Dictionary<Type, RV32_AbstractLoadStoreUnit> lsus = new Dictionary<Type, RV32_AbstractLoadStoreUnit>();
        private protected readonly RV32_RegisterSet reg;

        /// <summary>
        /// Risc-V 32bitCPU LSU
        /// </summary>
        /// <param name="registerSet">入出力用レジスタ</param>
        /// <param name="registerSet">メインメモリ</param>
        public RV32_LoadStoreUnit(RV32_RegisterSet registerSet) {
            reg = registerSet;
        }

        /// <summary>
        /// LSUのインスタンスを取得する
        /// </summary>
        /// <param name="type">取得するインスタンスの型</param>
        /// <returns>Aluインスタンス</returns>
        public RV32_AbstractLoadStoreUnit GetInstance(Type type) {
            if (!lsus.ContainsKey(type)) {
                lsus.Add(type, (RV32_AbstractLoadStoreUnit)Activator.CreateInstance(type, new object[] { reg, reg.Mem }));
            }
            return lsus[type];
        }

        /// <summary>
        /// レジスタを全てクリアし、エントリポイントをPCに設定する
        /// </summary>
        /// <param name="entryOffset"></param>
        public void ClearAndSetPC(UInt32 PhysicalAddress, UInt32 VirtualAddress, UInt32 entryOffset) {
            reg.ClearAll();
            reg.Mem.PAddr = PhysicalAddress;
            reg.Mem.Offset = entryOffset;
            reg.PC = VirtualAddress;
        }

        /// <summary>
        /// サポートするオプション拡張命令セット(Machine ISA)を表すCSRに設定を追加する
        /// </summary>
        /// <param name="option">命令セットを表す文字</param>
        public void AddMisa(char option) {
            reg.AddMisa(option);
        }
    }

    /// <summary>
    /// Risc-V 32bitCPU LSU抽象クラス
    /// </summary>
    public abstract class RV32_AbstractLoadStoreUnit {
        private protected readonly RV32_RegisterSet reg;

        protected internal RV32_AbstractLoadStoreUnit(RV32_RegisterSet registerSet, RV32_AbstractMemoryHandler mainMemory) {
            reg = registerSet;
        }
    }
}