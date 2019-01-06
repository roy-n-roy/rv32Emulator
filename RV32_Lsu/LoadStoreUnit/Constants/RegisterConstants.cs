namespace RiscVCpu.LoadStoreUnit.Constants {

    #region RV32レジスタ名定義

    /// <summary>
    /// RV32I基本命令セットで使用する整数レジスタのアドレスを表す
    /// アドレスは5bit整数から成り、x0～x31が定義される
    /// </summary>
    public enum Register : byte {
        #region RV32I 番号表記形式
        /*
        x0 = 0,
        x1 = 1,
        x2 = 2,
        x3 = 3,
        x4 = 4,
        x5 = 5,
        x6 = 6,
        x7 = 7,
        x8 = 8,
        x9 = 9,
        x10 = 10,
        x11 = 11,
        x12 = 12,
        x13 = 13,
        x14 = 14,
        x15 = 15,
        x16 = 16,
        x17 = 17,
        x18 = 18,
        x19 = 19,
        x20 = 20,
        x21 = 21,
        x22 = 22,
        x23 = 23,
        x24 = 24,
        x25 = 25,
        x26 = 26,
        x27 = 27,
        x28 = 28,
        x29 = 29,
        x30 = 30,
        x31 = 31,
        */
        #endregion

        #region RV32I 名称表記形式
        zero = 0,
        ra = 1,
        sp = 2,
        gp = 3,
        tp = 4,
        t0 = 5,
        t1 = 6,
        t2 = 7,
        //s0 = 8,
        fp = 8,
        s1 = 9,
        a0 = 10,
        a1 = 11,
        a2 = 12,
        a3 = 13,
        a4 = 14,
        a5 = 15,
        a6 = 16,
        a7 = 17,
        s2 = 18,
        s3 = 19,
        s4 = 20,
        s5 = 21,
        s6 = 22,
        s7 = 23,
        s8 = 24,
        s9 = 25,
        s10 = 26,
        s11 = 27,
        t3 = 28,
        t4 = 29,
        t5 = 30,
        t6 = 31,

        #endregion
    }

    public enum FPRegister : byte {
        #region RV32I 番号表記形式
        /*
        f0 = 0,
        f1 = 1,
        f2 = 2,
        f3 = 3,
        f4 = 4,
        f5 = 5,
        f6 = 6,
        f7 = 7,
        f8 = 8,
        f9 = 9,
        f10 = 10,
        f11 = 11,
        f12 = 12,
        f13 = 13,
        f14 = 14,
        f15 = 15,
        f16 = 16,
        f17 = 17,
        f18 = 18,
        f19 = 19,
        f20 = 20,
        f21 = 21,
        f22 = 22,
        f23 = 23,
        f24 = 24,
        f25 = 25,
        f26 = 26,
        f27 = 27,
        f28 = 28,
        f29 = 29,
        f30 = 30,
        f31 = 31,
        */
        #endregion

        #region RV32I 名称表記形式
        ft0 = 0,
        ft1 = 1,
        ft2 = 2,
        ft3 = 3,
        ft4 = 4,
        ft5 = 5,
        ft6 = 6,
        ft7 = 7,
        fs0 = 8,
        fs1 = 9,
        fa0 = 10,
        fa1 = 11,
        fa2 = 12,
        fa3 = 13,
        fa4 = 14,
        fa5 = 15,
        fa6 = 16,
        fa7 = 17,
        fs2 = 18,
        fs3 = 19,
        fs4 = 20,
        fs5 = 21,
        fs6 = 22,
        fs7 = 23,
        fs8 = 24,
        fs9 = 25,
        fs10 = 26,
        fs11 = 27,
        ft8 = 28,
        ft9 = 29,
        ft10 = 30,
        ft11 = 31,

        #endregion
    }

    #endregion
}
