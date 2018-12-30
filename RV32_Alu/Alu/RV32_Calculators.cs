using RiscVCpu.LoadStoreUnit;
using RiscVCpu.RegisterSet;
using System;
using System.Collections.Generic;

namespace RiscVCpu.ArithmeticLogicUnit {

    /// <summary>
    /// Risc-V 32bitCPU ALU
    /// </summary>
    public class RV32_Calculators {
        private readonly Dictionary<Type, RV32_AbstractCalculator> culcs = new Dictionary<Type, RV32_AbstractCalculator>();
        private protected RV32_RegisterSet reg;

        /// <summary>
        /// Risc-V 32bitCPU ALU
        /// </summary>
        /// <param name="registerSet">演算入出力用レジスタ</param>
        public RV32_Calculators(RV32_RegisterSet registerSet) {
            reg = registerSet;
        }

        /// <summary>
        /// ALUのインスタンスを取得する
        /// </summary>
        /// <param name="type">取得するインスタンスの型</param>
        /// <returns>Aluインスタンス</returns>
        public RV32_AbstractCalculator GetInstance(Type type) {
            if (!culcs.ContainsKey(type)) {
                culcs.Add(type, (RV32_AbstractCalculator)Activator.CreateInstance(type, new object[] { reg }));
            }
            return culcs[type];
        }
    }

    /// <summary>
    /// Risc-V 32bitCPU ALU抽象クラス
    /// </summary>
    public abstract class RV32_AbstractCalculator {
        private protected RV32_RegisterSet reg;

        protected internal RV32_AbstractCalculator(RV32_RegisterSet registerSet) {
            reg = registerSet;
        }
    }
}