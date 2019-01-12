namespace RV32_Cpu.Decoder.Constants {

    #region RV32命令定義

    /// <summary>RV32命令の0～6bit部分</summary>
    public enum Opcode : byte {
        #region RV32I 命令定義
        /// <summary>Load Upper Immediate命令</summary>
        lui = 0b0110111,
        /// <summary>Add Upper Immediate to PC命令</summary>
        auipc = 0b0010111,
        /// <summary>Jump And Link命令</summary>
        jal = 0b1101111,
        /// <summary>Jump And Link Register命令</summary>
        jalr = 0b1100111,
        /// <summary>Branch系命令</summary>
        branch = 0b1100011,
        /// <summary>Load系命令</summary>
        load = 0b0000011,
        /// <summary>Store系命令</summary>
        store = 0b0100011,
        /// <summary>即値付き算術論理演算命令</summary>
        miscOpImm = 0b0010011,
        /// <summary>算術論理演算命令</summary>
        miscOp = 0b0110011,
        /// <summary>同期命令</summary>
        miscMem = 0b0001111,
        /// <summary>特権命令</summary>
        privilege = 0b1110011,

        #endregion

        #region RV32A拡張 命令定義
        /// <summary>不可分メモリ操作命令</summary>
        amo = 0b0101111,

        #endregion

        #region RV32FD拡張 命令定義
        /// <summary>Floating-Point Load命令</summary>
        fl = 0b0000111,
        /// <summary>Floating-Point Store命令</summary>
        fs = 0b0100111,
        /// <summary>Floating-Point Fused Multiply-Add命令</summary>
        fmadd = 0b1000011,
        /// <summary>Floating-Point Fused Multiply-Subtract命令</summary>
        fmsub = 0b1000111,
        /// <summary>Floating-Point Fused Negative Multiply-Add命令</summary>
        fnmadd = 0b1001111,
        /// <summary>Floating-Point Fused Negative Multiply-Subtract命令</summary>
        fnmsub = 0b1001011,
        /// <summary>Floating-Point 算術論理演算命令</summary>
        fmiscOp = 0b1010011,

        #endregion
    }

    /// <summary>RV32命令の12～14bit部分</summary>
    public enum Funct3 : byte {
        #region RV32I 命令定義
        /// <summary>Jump and Link Register命令</summary>
        jalr = 0b000,
        /// <summary>Branch if Equal命令</summary>
        beq = 0b000,
        /// <summary>Branch if Not Equal命令</summary>
        bne = 0b001,
        /// <summary>Branch if Less Then命令</summary>
        blt = 0b100,
        /// <summary>Branch if Greater Then or Equal命令</summary>
        bge = 0b101,
        /// <summary>Branch if Less Then, Unsigned命令</summary>
        bltu = 0b110,
        /// <summary>Branch if Graeter Then or Equal, Unsigned命令</summary>
        bgeu = 0b111,

        /// <summary>Load Byte命令</summary>
        lb = 0b000,
        /// <summary>Load Harf word命令</summary>
        lh = 0b001,
        /// <summary>Load Word命令</summary>
        lw = 0b010,
        /// <summary>Load Byte Unsigned命令</summary>
        lbu = 0b100,
        /// <summary>Load Harf word Unsigned命令</summary>
        lhu = 0b101,

        /// <summary>Store Byte命令</summary>
        sb = 0b000,
        /// <summary>Store Harf word命令</summary>
        sh = 0b001,
        /// <summary>Store Word命令</summary>
        sw = 0b010,

        /// <summary>Add Immmidiate命令</summary>
        addi = 0b000,
        /// <summary>Exclusive-OR Immediate命令</summary>
        xori = 0b100,
        /// <summary>OR Immediate命令</summary>
        ori = 0b110,
        /// <summary>AND Immmidiate命令</summary>
        andi = 0b111,
        /// <summary>Set if Less Then Immediate命令</summary>
        slti = 0b010,
        /// <summary>Set if Less Than Immediate, Unsigned命令</summary>
        sltiu = 0b011,
        /// <summary>Shift Left Logical命令</summary>
        slli = 0b001,
        /// <summary>Shift Right Logical/Arithmetic命令</summary>
        srli_srai = 0b101,

        /// <summary>Add/Subtract命令</summary>
        add_sub = 0b000,
        /// <summary>Exclusive-OR命令</summary>
        xor = 0b100,
        /// <summary>OR命令</summary>
        or = 0b110,
        /// <summary>AND命令</summary>
        and = 0b111,
        /// <summary>Set Less Then命令</summary>
        slt = 0b010,
        /// <summary>Set Less Then, Unsigned命令</summary>
        sltu = 0b011,
        /// <summary>Shift Left Logical命令</summary>
        sll = 0b001,
        /// <summary>Shift Right Logical/Arithmetic命令</summary>
        srl_sra = 0b101,

        /// <summary>Fence Memory and I/O命令</summary>
        fence = 0b000,
        /// <summary>Fence Instruction Stream命令</summary>
        fenceI = 0b001,

        /// <summary>特権命令</summary>
        privilege = 0b000,
        /// <summary>Control and Status Register Read and Write命令</summary>
        csrrw = 0b001,
        /// <summary>Control and Status Register Read and Set命令</summary>
        csrrs = 0b010,
        /// <summary>Control and Status Register Read and Clear命令</summary>
        csrrc = 0b011,
        /// <summary>Control and Status Register Read and Write Immediate命令</summary>
        csrrwi = 0b101,
        /// <summary>Control and Status Register Read and Set Immediate命令</summary>
        csrrsi = 0b110,
        /// <summary>Control and Status Register Read and Clear Immediate命令</summary>
        csrrci = 0b111,

        #endregion

        #region RV32M拡張 命令定義
        /// <summary>Multiply命令</summary>
        mul = 0b000,
        /// <summary>Multiply High命令</summary>
        mulh = 0b001,
        /// <summary>Multiply High, Signed-Unsigned命令</summary>
        mulhsu = 0b010,
        /// <summary>Multiply High, Unsigned命令</summary>
        mulhu = 0b011,
        /// <summary>Divide命令</summary>
        div = 0b100,
        /// <summary>Divide, Unsigned命令</summary>
        divu = 0b101,
        /// <summary>Reminder命令</summary>
        rem = 0b110,
        /// <summary>Reminder, Unsigned命令</summary>
        remu = 0b111,

        #endregion

        #region RV32A拡張 命令定義
        /// <summary>不可分命令</summary>
        amo = 0b010,

        #endregion

        #region RV32FD拡張 命令定義
        /// <summary>Floating-Point Load/Store, Single-Precision命令</summary>
        flw_fsw = 0b010,
        /// <summary>Floating-Point Load/Store, Double-Precision命令</summary>
        fld_fsd = 0b011,

        /// <summary>Floating-Point Sign Inject命令</summary>
        fsgnj = 0b000,
        /// <summary>Floating-Point Sign Inject-Negative命令</summary>
        fsgnjn = 0b001,
        /// <summary>Floating-Point Sign Inject-XOR命令</summary>
        fsgnjx = 0b010,

        /// <summary>Floating-Point Minimum</summary>
        fmin = 0b000,
        /// <summary>Floating-Point Maximum</summary>
        fmax = 0b001,

        /// <summary>Floating-Point Equal</summary>
        fcompare_eq = 0b010,
        /// <summary>Floating-Point Less Then</summary>
        fcompare_lt = 0b001,
        /// <summary>Floating-Point Less Then or Equal</summary>
        fcompare_le = 0b000,

        #endregion
    }

    /// <summary>RV32命令の25～26bit部分</summary>
    public enum Funct2 : byte {
        #region RV32FD拡張 命令定義
        /// <summary>Floating-Point Fused Multiply-Operation, Singre-Precision命令</summary>
        fmiscOpS = 0b00,
        /// <summary>Floating-Point Fused Multiply-Operation, Double-Precision命令</summary>
        fmiscOpD = 0b01,

        #endregion
    }

    /// <summary>RV32命令の25～31bit部分</summary>
    public enum Funct7 : byte {
        #region RV32I 命令定義
        /// <summary>命令</summary>
        slli = 0b0000000,
        /// <summary>命令</summary>
        srli = 0b0000000,
        /// <summary>命令</summary>
        srai = 0b0100000,

        /// <summary>Add命令</summary>
        add = 0b0000000,
        /// <summary>Subtract命令</summary>
        sub = 0b0100000,
        /// <summary>Exclusive-OR命令</summary>
        xor = 0b0000000,
        /// <summary>OR命令</summary>
        or = 0b0000000,
        /// <summary>AND命令</summary>
        and = 0b0000000,
        /// <summary>Set Less Then命令</summary>
        slt = 0b0000000,
        /// <summary>Set Less Then, Unsigned命令</summary>
        sltu = 0b0000000,
        /// <summary>Shift Left Logical命令</summary>
        sll = 0b0000000,
        /// <summary>Shift Right Logical命令</summary>
        srl = 0b0000000,
        /// <summary>Shift Right Arithmetic命令</summary>
        sra = 0b0100000,

        /// <summary>Fence Virtual Memory命令</summary>
        sfenceVma = 0b0001001,

        #endregion

        #region RV32M拡張 命令定義
        /// <summary>命令</summary>
        mul_div = 0b0000001,

        #endregion

    }

    /// <summary>RV32命令の20～31bit部分</summary>
    public enum Funct12 : ushort {
        #region RV32I 命令定義
        /// <summary>Supervisor-Mode Exception Return</summary>
        sret = 0b000100000010,
        /// <summary>Machine-Mode Exeception Return</summary>
        mret = 0b001100000010,
        /// <summary>Wait for Interrupt</summary>
        wfi = 0b000100000101,
        /// <summary>Environment Call命令</summary>
        ecall = 0b00000000000,
        /// <summary>Environment Break命令</summary>
        ebreak = 0b00000000001,

        #endregion
    }

    /// <summary>RV32命令の27～31bit部分</summary>
    public enum Funct5 : ushort {
        #region RV32A拡張 命令定義
        /// <summary>Load Reserved命令</summary>
        lr = 0b00010,
        /// <summary>Store Conditional命令</summary>
        sc = 0b00011,
        /// <summary>Atomic Memory Operation Swap Word命令</summary>
        amo_swap = 0b00001,
        /// <summary>Atomic Memory Operation Add Word命令</summary>
        amo_add = 0b00000,
        /// <summary>Atomic Memory Operation XOR Word命令</summary>
        amo_xor = 0b00100,
        /// <summary>Atomic Memory Operation And Word命令</summary>
        amo_and = 0b01100,
        /// <summary>Atomic Memory Operation OR Word命令</summary>
        amo_or = 0b1000,
        /// <summary>Atomic Memory Operation Minimum Word命令</summary>
        amo_min = 0b10000,
        /// <summary>Atomic Memory Operation Maximum Word命令</summary>
        amo_max = 0b10100,
        /// <summary>Atomic Memory Operation Minimum, Unsigned Word命令</summary>
        amo_minu = 0b11000,
        /// <summary>Atomic Memory Operation Maximum, Unsigned Word命令</summary>
        amo_maxu = 0b11100,

        #endregion

        #region RV32FD拡張 命令定義
        /// <summary>Float-Point Add命令</summary>
        fadd = 0b00000,
        /// <summary>Float-Point Subtract命令</summary>
        fsub = 0b00001,
        /// <summary>Float-Point Multiply命令</summary>
        fmul = 0b00010,
        /// <summary>Float-Point Divide命令</summary>
        fdiv = 0b00011,
        /// <summary>Float-Point Square Root命令</summary>
        fsqrt = 0b01011,
        /// <summary>Float-Point Sign Inject命令</summary>
        fsgnj = 0b00100,
        /// <summary>Float-Point Min/Max命令</summary>
        fmin_fmax = 0b00101,
        /// <summary>Float-Point Compare命令</summary>
        fcompare = 0b10100,
        /// <summary>Float-Point Convert to Single from Double/to Double from Single命令</summary>
        fcvtSD = 0b01000,
        /// <summary>Float-Point Convert to Word from Single/Double命令</summary>
        fcvttoW = 0b11000,
        /// <summary>Float-Point Convert to Single/Double from Word命令</summary>
        fcvtfromW = 0b11010,
        /// <summary>Float-Point Move to Word from Integer命令/Classify命令</summary>
        fmvXW_fclass = 0b11100,
        /// <summary>Float-Point Move to Integer from Word命令</summary>
        fmvWX = 0b11110,

        #endregion

    }

    /// <summary>RV32C拡張命令の15～13bit部分+0～1bit部分</summary>
    public enum CompressedOpcode : byte {
        #region RV32C拡張 命令定義
        /// <summary>Add Immediate命令</summary>
        addi = 0b00001,
        /// <summary>算術論理演算命令</summary>
        misc_alu = 0b10001,
        /// <summary>Add Immediate, Scaled by 4, to Stack Pointer, Nondestructive命令</summary>
        addi4spn = 0b00000,
        /// <summary>Shift Left Logical命令</summary>
        slli = 0b00010,
        /// <summary>Jump Register, Move, Add命令</summary>
        jr_mv_add = 0b10010,

        /// <summary>Jump And Link命令</summary>
        jal = 0b00101,
        /// <summary>Load Immediate命令</summary>
        li = 0b01001,
        /// <summary>Load Upper Immediate命令</summary>
        lui_addi16sp = 0b01101,
        /// <summary>Jump命令</summary>
        j = 0b10101,
        /// <summary>Branch if Equal to Zero命令</summary>
        beqz = 0b11001,
        /// <summary>Branch if Not Equal to Zero命令</summary>
        bnez = 0b11101,

        /// <summary>Floating-point Load Doubleword命令</summary>
        fld = 0b00100,
        /// <summary>Load Word命令</summary>
        lw = 0b01000,
        /// <summary>Floating-Point Load Word命令</summary>
        flw = 0b01100,
        /// <summary>Floating-Point Store Word命令</summary>
        fsd = 0b10100,
        /// <summary>Store Word命令</summary>
        sw = 0b11000,
        /// <summary>Floating-Point Store Word命令</summary>
        fsw = 0b11100,

        /// <summary>Floating-point Load Doubleword, Stack-Pointer Relative命令</summary>
        fldsp = 0b00110,
        /// <summary>Load Word命令, Stack-Pointer Relative</summary>
        lwsp = 0b01010,
        /// <summary>Floating-Point Load Word, Stack-Pointer Relative命令</summary>
        flwsp = 0b01110,
        /// <summary>Floating-Point Store Word, Stack-Pointer Relative命令</summary>
        fsdsp = 0b10110,
        /// <summary>Store Word, Stack-Pointer Relative命令</summary>
        swsp = 0b11010,
        /// <summary>Floating-Point Store Word, Stack-Pointer Relative命令</summary>
        fswsp = 0b11110,

        #endregion
    }

    #endregion
}