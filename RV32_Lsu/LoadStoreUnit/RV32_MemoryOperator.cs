using RiscVCpu.MemoryHandler;
using RiscVCpu.RegisterSet;
using System;
using System.Collections.Generic;

namespace RiscVCpu.LoadStoreUnit {

    /// <summary>
    /// Risc-V 32bitCPU LSU
    /// </summary>
    public class RV32_LoadStoreUnit {
        private readonly Dictionary<Type, RV32_AbstractLoadStoreUnit> lsus = new Dictionary<Type, RV32_AbstractLoadStoreUnit>();
        private protected readonly RV32_RegisterSet reg;
        private protected RV32_AbstractMemoryHandler mem;

        /// <summary>
        /// メインメモリ
        /// </summary>
        public RV32_AbstractMemoryHandler Mem {
            get => mem;
            set {
                mem = value;
                reg.SetMemHandler(mem);
                foreach (RV32_AbstractLoadStoreUnit lsu in lsus.Values) {
                    lsu.Mem = value;
                }
            }
        }

        /// <summary>
        /// Risc-V 32bitCPU LSU
        /// </summary>
        /// <param name="registerSet">入出力用レジスタ</param>
        /// <param name="registerSet">メインメモリ</param>
        public RV32_LoadStoreUnit(RV32_RegisterSet registerSet, byte[] mainMemory) {
            reg = registerSet;
            mem = new RV32_MemoryHandler(mainMemory);
            reg.SetMemHandler(mem);
        }

        /// <summary>
        /// LSUのインスタンスを取得する
        /// </summary>
        /// <param name="type">取得するインスタンスの型</param>
        /// <returns>Aluインスタンス</returns>
        public RV32_AbstractLoadStoreUnit GetInstance(Type type) {
            if (!lsus.ContainsKey(type)) {
                lsus.Add(type, (RV32_AbstractLoadStoreUnit)Activator.CreateInstance(type, new object[] { reg, mem }));
            }
            return lsus[type];
        }

        /// <summary>
        /// レジスタを全てクリアし、エントリポイントをPCに設定する
        /// </summary>
        /// <param name="entryPoint"></param>
        public void ClearAndSetPC(UInt32 entryPoint, UInt32 insLength = 4u) {
            reg.ClearAll();
            reg.SetPc(entryPoint);
        }

        /// <summary>
        /// サポートするオプション拡張命令セット(Machine ISA)を表すCSRに設定を追加する
        /// </summary>
        /// <param name="option">命令セットを表す文字</param>
        public void AddMisa(char option, UInt32 insLength = 4u) {
            reg.AddMisa(option);
        }
    }

    /// <summary>
    /// Risc-V 32bitCPU LSU抽象クラス
    /// </summary>
    public abstract class RV32_AbstractLoadStoreUnit {
        private protected readonly RV32_RegisterSet reg;
        private protected RV32_AbstractMemoryHandler mem;

        protected internal RV32_AbstractLoadStoreUnit(RV32_RegisterSet registerSet, RV32_AbstractMemoryHandler mainMemory) {
            reg = registerSet;
            mem = mainMemory;
        }

        internal protected RV32_AbstractMemoryHandler Mem { get => mem; set => mem = value; }
    }
}