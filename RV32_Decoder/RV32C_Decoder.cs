using RV32_Alu;
using RV32_Decoder.Constants;
using RV32_Lsu;
using RV32_Lsu.Constants;
using System;

namespace RV32_Decoder {
    public class RV32C_Decoder : RV32_AbstractDecoder {

        public RV32C_Decoder(RV32_InstructionDecoder decoder) : base(decoder) { }

        /// <summary>命令長(Byte)</summary>
        const UInt32 InstructionLength = 2u;

        /// <summary>
        /// 引数で渡された32bit長のうち、前半の16bitを命令としてデコードし、cpuで実行する
        /// </summary>
        /// <param name="instruction">32bit長の命令</param>
        /// <param name="cpu">命令を実行するRV32CPU</param>
        /// <returns>実行の成否</returns>
        internal protected override bool Exec(UInt32[] ins) {
            bool result = false;

            // 命令の0～1bit目が "11" の場合は対象なし
            if ((ins[0] & 0b11u) == 0b11u) {
                return result;
            }

            // 32bit長命令の前半から16bit長命令を取り出し
            UInt32[] cins = SplitCompressedInstruction(ins);
            Register rd_rs1 = (Register)(cins[2] & 0x1f),
                        rs2 = (Register)cins[1],
                    crd_rs1 = (Register)(0x8 | (cins[2] & 0x7)),
                       crs2 = (Register)(0x8 | (cins[1] & 0x7));
            CompressedOpcode opcode = (CompressedOpcode)cins[0];
            Int32 immediate = 0;
            RV32_IntegerAlu alu;
            RV32_IntegerLsu lsu;
            RV32_FloatPointLsu fplsu;

            switch (opcode) {
                case CompressedOpcode.addi: // addi命令
                    alu = (RV32_IntegerAlu)Decoder.Alu(typeof(RV32_IntegerAlu));
                    immediate = GetSignedImmediate("CI", cins);
                    result = alu.Addi(rd_rs1, rd_rs1, immediate, InstructionLength);
                    break;

                case CompressedOpcode.misc_alu:
                    alu = (RV32_IntegerAlu)Decoder.Alu(typeof(RV32_IntegerAlu));
                    switch (cins[2] & 0b011000) {
                        case 0b000000: // srli命令
                            immediate = GetUnsignedImmediate("CI", cins);
                            immediate = immediate != 0 ? immediate : 64;
                            result = alu.Srli(crd_rs1, crd_rs1, immediate, InstructionLength);
                            break;

                        case 0b001000: // srai命令
                            immediate = GetUnsignedImmediate("CI", cins);
                            immediate = immediate != 0 ? immediate : 64;
                            result = alu.Srai(crd_rs1, crd_rs1, immediate, InstructionLength);
                            break;

                        case 0b010000: // andi命令
                            immediate = GetSignedImmediate("CI", cins);
                            result = alu.Andi(crd_rs1, crd_rs1, immediate, InstructionLength);
                            break;
                        case 0b011000:
                            switch ((cins[2] & 0b100000) | (cins[1] & 0b11000)) {
                                case 0b000000: // sub命令
                                    result = alu.Sub(crd_rs1, crd_rs1, crs2, InstructionLength);
                                    break;

                                case 0b001000: // xor命令
                                    result = alu.Xor(crd_rs1, crd_rs1, crs2, InstructionLength);
                                    break;

                                case 0b010000: // or命令
                                    result = alu.Or(crd_rs1, crd_rs1, crs2, InstructionLength);
                                    break;

                                case 0b011000: // and命令
                                    result = alu.And(crd_rs1, crd_rs1, crs2, InstructionLength);
                                    break;
                            }
                            break;
                    }
                    break;

                case CompressedOpcode.slli: // slli命令
                    immediate = GetUnsignedImmediate("CI", cins);
                    immediate = immediate != 0 ? immediate : 64;
                    alu = (RV32_IntegerAlu)Decoder.Alu(typeof(RV32_IntegerAlu));
                    result = alu.Slli(crd_rs1, crd_rs1, immediate, InstructionLength);
                    break;

                 case CompressedOpcode.jr_mv_add:
                    if (cins[1] != 0 && (cins[2] & 0x1f) != 0) {
                        alu = (RV32_IntegerAlu)Decoder.Alu(typeof(RV32_IntegerAlu));
                        if ((cins[2] & 0b100000) == 0) { // mv命令
                            result = alu.Add(rd_rs1, Register.zero, rs2, InstructionLength);
                        } else { // add命令
                            result = alu.Add(rd_rs1, rd_rs1, rs2, InstructionLength);
                        }
                    } else if (cins[1] == 0 && (cins[2] & 0x1f) != 0) { // jalr,jr命令
                        lsu = (RV32_IntegerLsu)Decoder.Lsu(typeof(RV32_IntegerLsu));
                        result = lsu.Jalr((Register)(cins[2] >> 5), rd_rs1, 0, InstructionLength);

                    } else if (cins[1] == 0 && cins[2] == 0b100000) { // ebreak命令
                        result = Decoder.Reg.Ebreak(InstructionLength);
                    }
                    break;

                case CompressedOpcode.addi4spn: // addi4spn命令
                    immediate = GetUnsignedImmediate("CIW", cins);
                    if (immediate != 0) {
                        alu = (RV32_IntegerAlu)Decoder.Alu(typeof(RV32_IntegerAlu));
                        result = alu.Addi(crs2, Register.sp, immediate, InstructionLength);
                    }
                    break;

                case CompressedOpcode.li: // li命令
                    alu = (RV32_IntegerAlu)Decoder.Alu(typeof(RV32_IntegerAlu));
                    immediate = GetSignedImmediate("CI", cins);
                    result = alu.Addi(rd_rs1, Register.zero, immediate, InstructionLength);
                    break;

                case CompressedOpcode.lui_addi16sp: // lui_addi16sp命令
                    alu = (RV32_IntegerAlu)Decoder.Alu(typeof(RV32_IntegerAlu));
                    immediate = GetSignedImmediate("CI", cins);
                    if (immediate != 0) { // addi16sp命令
                        if ((cins[2] & 0x1f) == 0b000010) {
                            Int32 t = immediate;
                            immediate &= ~0x3ef;
                            immediate |= (t & 0b100000) << 4;
                            immediate |= (t & 0b001000) << 3;
                            immediate |= (t & 0b000110) << 6;
                            immediate |= (t & 0b000001) << 5;
                            result = alu.Addi(Register.sp, Register.sp, immediate, InstructionLength);
                        } else if ((cins[2] & 0x1f) != 0) { // lui命令
                            result = alu.Lui(rd_rs1, immediate << 12, InstructionLength);
                        }
                    }
                    break;

                case CompressedOpcode.jal: // jal命令
                    immediate = GetUnsignedImmediate("CJ", cins);
                    lsu = (RV32_IntegerLsu)Decoder.Lsu(typeof(RV32_IntegerLsu));
                    result = lsu.Jal(Register.ra, immediate, InstructionLength);
                    break;

                case CompressedOpcode.j: // j命令
                    immediate = GetUnsignedImmediate("CJ", cins);
                    lsu = (RV32_IntegerLsu)Decoder.Lsu(typeof(RV32_IntegerLsu));
                    result = lsu.Jal(Register.zero, immediate, InstructionLength);
                    break;

                case CompressedOpcode.beqz: // beqz命令
                    immediate = GetUnsignedImmediate("CB", cins);
                    lsu = (RV32_IntegerLsu)Decoder.Lsu(typeof(RV32_IntegerLsu));
                    result = lsu.Beq(crd_rs1, Register.zero, immediate, InstructionLength);
                    break;

                case CompressedOpcode.bnez: // bnez命令
                    immediate = GetUnsignedImmediate("CB", cins);
                    lsu = (RV32_IntegerLsu)Decoder.Lsu(typeof(RV32_IntegerLsu));
                    result = lsu.Bne(crd_rs1, Register.zero, immediate, InstructionLength);
                    break;

                case CompressedOpcode.lw: // lw命令
                    immediate = GetUnsignedImmediate("CL", cins, 2);
                    lsu = (RV32_IntegerLsu)Decoder.Lsu(typeof(RV32_IntegerLsu));
                    result = lsu.Lw(crs2, crd_rs1, immediate, InstructionLength);
                    break;

                case CompressedOpcode.flw: // flw命令
                    immediate = GetUnsignedImmediate("CL", cins, 2);
                    fplsu = (RV32_FloatPointLsu)Decoder.Lsu(typeof(RV32_FloatPointLsu));
                    result = fplsu.Flw((FPRegister)crs2, crd_rs1, immediate, InstructionLength);
                    break;

                case CompressedOpcode.fld: // fld命令
                    immediate = GetUnsignedImmediate("CL", cins, 3);
                    fplsu = (RV32_FloatPointLsu)Decoder.Lsu(typeof(RV32_FloatPointLsu));
                    result = fplsu.Fld((FPRegister)crs2, crd_rs1, immediate, InstructionLength);
                    break;

                case CompressedOpcode.sw: // sw命令
                    immediate = GetUnsignedImmediate("CL", cins, 2);
                    lsu = (RV32_IntegerLsu)Decoder.Lsu(typeof(RV32_IntegerLsu));
                    result = lsu.Sw(crd_rs1, crs2, immediate, InstructionLength);
                    break;

                case CompressedOpcode.fsw: // fsw命令
                    immediate = GetUnsignedImmediate("CL", cins, 2);
                    fplsu = (RV32_FloatPointLsu)Decoder.Lsu(typeof(RV32_FloatPointLsu));
                    result = fplsu.Fsw(crd_rs1, (FPRegister)crs2, immediate, InstructionLength);
                    break;

                case CompressedOpcode.fsd: // fsd命令
                    immediate = GetUnsignedImmediate("CL", cins, 3);
                    fplsu = (RV32_FloatPointLsu)Decoder.Lsu(typeof(RV32_FloatPointLsu));
                    result = fplsu.Fsd(crd_rs1, (FPRegister)crs2, immediate, InstructionLength);
                    break;

                case CompressedOpcode.lwsp: // lwsp命令
                    immediate = GetUnsignedImmediate("CI", cins, 2);
                    lsu = (RV32_IntegerLsu)Decoder.Lsu(typeof(RV32_IntegerLsu));
                    result = lsu.Lw(rd_rs1, Register.sp, immediate, InstructionLength);
                    break;

                case CompressedOpcode.flwsp: // flwsp命令
                    immediate = GetUnsignedImmediate("CI", cins, 2);
                    fplsu = (RV32_FloatPointLsu)Decoder.Lsu(typeof(RV32_FloatPointLsu));
                    result = fplsu.Flw((FPRegister)rd_rs1, Register.sp, immediate, InstructionLength);
                    break;

                case CompressedOpcode.fldsp: // fldsp命令
                    immediate = GetUnsignedImmediate("CI", cins, 3);
                    fplsu = (RV32_FloatPointLsu)Decoder.Lsu(typeof(RV32_FloatPointLsu));
                    result = fplsu.Fld((FPRegister)rd_rs1, Register.sp, immediate, InstructionLength);
                    break;

                case CompressedOpcode.swsp: // swsp命令
                    immediate = GetUnsignedImmediate("CSS", cins, 2);
                    lsu = (RV32_IntegerLsu)Decoder.Lsu(typeof(RV32_IntegerLsu));
                    result = lsu.Sw(Register.sp, rs2, immediate, InstructionLength);
                    break;

                case CompressedOpcode.fswsp: // fswsp命令
                    immediate = GetUnsignedImmediate("CSS", cins, 2);
                    fplsu = (RV32_FloatPointLsu)Decoder.Lsu(typeof(RV32_FloatPointLsu));
                    result = fplsu.Fsw(Register.sp, (FPRegister)rs2, immediate, InstructionLength);
                    break;

                case CompressedOpcode.fsdsp: // fsdsp命令
                    immediate = GetUnsignedImmediate("CSS", cins, 3);
                    fplsu = (RV32_FloatPointLsu)Decoder.Lsu(typeof(RV32_FloatPointLsu));
                    result = fplsu.Fsd(Register.sp, (FPRegister)rs2, immediate, InstructionLength);
                    break;

            }
            return result;
        }

        /// <summary>
        /// 32bit長命令の前半16bitをRV32C命令として分割する
        /// </summary>
        /// <param name="ins">SplitInstructionメソッドで分割したUInt32配列のRisc-V命令</param>
        /// <returns></returns>
        private UInt32[] SplitCompressedInstruction(UInt32[] ins) {
            UInt16 instruction = (UInt16)(ins[0] | (ins[1] << 7) | (ins[2] << 12) | (ins[3] << 15));

            UInt32[] cIns = new UInt32[3];
            cIns[0] = (byte)(RV32_InstructionDecoder.Slice(instruction, 0, 2) | (RV32_InstructionDecoder.Slice(instruction, 13, 3) << 2));
            cIns[1] = (byte)RV32_InstructionDecoder.Slice(instruction, 2, 5);
            cIns[2] = (byte)RV32_InstructionDecoder.Slice(instruction, 7, 6);
            return cIns;
        }

        /// <summary>
        /// Risc-V命令形式に応じて、命令に渡す即値を符号なしとして生成する
        /// </summary>
        /// <param name="format">Risc-V命令形式</param>
        /// <param name="ins">SplitCompressedInstructionメソッドで分割したUInt32配列のRisc-V命令</param>
        /// <returns>Int32型即値</returns>
        /// <param name="scaleCount"></param>
        /// <returns></returns>
        private Int32 GetUnsignedImmediate(string format, UInt32[] cins, int scaleCount = 0) {
            UInt32 result;
            int length, j = 0;

            switch (format) {
                case "CI":
                    length = 6;
                    result = cins[1] | cins[2] & 0b100000u;
                    break;
                case "CL":
                    length = 7;
                    result = ((cins[1] & 0b11000u) >> 2) | (cins[2] & 0b111000u);
                    j++;
                    scaleCount--;
                    break;
                case "CSS":
                    length = 6;
                    result = cins[2];
                    break;
                case "CB":
                    scaleCount = 0;
                    length = 9;
                    result = (cins[1] & 0b00110u) | (cins[2] & 0b11000u) | ((cins[1] & 0b00001u) >> 4) | (((cins[1] & 0b11000u) | (cins[2] & 0b100000u)) << 3);
                    break;
                case "CJ":
                    scaleCount = 0;
                    length = 6;
                    result = (cins[1] & 0b01110u) | (cins[2] & 0b010000u) | ((cins[1] & 0b00001u) << 5) | (((cins[2] & 0b101101u) | (cins[1] & 0b000001u)) << 6) | ((cins[1] & 0b10000u) << 3) | ((cins[1] & 0b10000u) << 5) | ((cins[2] & 0b000010u) << 9);
                    break;
                case "CIW":
                    scaleCount = 0;
                    length = 10;
                    result = ((cins[1] & 0b10000u) >> 2) | (cins[1] & 0b01000u) | (cins[2] & 0b110000u) | ((cins[2] & 0b001111u) << 6);
                    break;

                default:
                    length = 1;
                    result = 0;
                    break;
            }

            for (; j < scaleCount; j++) {
                UInt32 mask = (1u << j);
                result |= (result & mask) << length;
                result &= ~mask;
            }

            return (Int32)result;
        }

        /// <summary>
        /// Risc-V命令形式に応じて、命令に渡す即値を符号付きとして生成する
        /// </summary>
        /// <param name="format">Risc-V命令形式</param>
        /// <param name="ins">SplitCompressedInstructionメソッドで分割したUInt32配列のRisc-V命令</param>
        /// <returns>Int32型即値</returns>
        private Int32 GetSignedImmediate(string format, UInt32[] cins) {
            UInt32 result;
            int length;

            switch (format) {
                case "CI":
                    length = 6;
                    result = (cins[2] & 0b100000u) == 0u ? cins[1] : 0xffffffe0u | cins[1];
                    break;
                default:
                    length = 1;
                    result = 0;
                    break;
            }
            if ((result & (1 << length - 1)) > 0) {
                result |= (0x80000000u >> (32 - length));
            }

            return (Int32)result;
        }
    }
}
