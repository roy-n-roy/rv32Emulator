using RiscVCpu.ArithmeticLogicUnit;
using RiscVCpu.Decoder.Constants;
using RiscVCpu.LoadStoreUnit;
using RiscVCpu.LoadStoreUnit.Constants;
using System;
using System.Linq;

namespace RiscVCpu.Decoder {
    public class RV32C_Decoder : RV32_AbstractDecoder {

        /// <summary>
        /// 引数で渡された32bit長のうち、前半の16bitを命令としてデコードし、cpuで実行する
        /// </summary>
        /// <param name="instruction">32bit長の命令</param>
        /// <param name="cpu">命令を実行するRV32CPU</param>
        /// <returns>実行の成否</returns>
        internal protected override bool Exec(byte[] ins, RV32_Cpu cpu) {
            byte[] cins = GetCompressedInstruction(ins);
            bool result = false;
            Register rd_rs1 = (Register)(cins[2] & 0x1f),
                        rs2 = (Register)cins[1],
                    crd_rs1 = (Register)(0x8 + (cins[1] & 0x7)),
                       crs2 = (Register)(0x8 + (cins[2] & 0x7));
            CompressedOpcode opcode = (CompressedOpcode)cins[0];
            Int32 immediate = 0;
            RV32_Alu alu;
            RV32_Lsu lsu;
            RV32_FloatPointLsu fplsu;

            switch (opcode) {
                case CompressedOpcode.addi: // addi命令
                    alu = (RV32_Alu)cpu.Alu(typeof(RV32_Alu));
                    immediate = GetSignedImmediate("CI", cins);
                    result = alu.Addi(rd_rs1, rd_rs1, immediate, 2);
                    break;

                case CompressedOpcode.misc_alu:
                    alu = (RV32_Alu)cpu.Alu(typeof(RV32_Alu));
                    switch (cins[2] & 0b011000) {
                        case 0b000000: // srli命令
                            immediate = GetUnsignedImmediate("CI", cins);
                            immediate = immediate != 0 ? immediate : 64;
                            result = alu.Srli(crs2, crs2, immediate, 2);
                            break;

                        case 0b001000: // srai命令
                            immediate = GetUnsignedImmediate("CI", cins);
                            immediate = immediate != 0 ? immediate : 64;
                            result = alu.Srai(crs2, crs2, immediate, 2);
                            break;

                        case 0b010000: // andi命令
                            immediate = GetSignedImmediate("CI", cins);
                            result = alu.Andi(crs2, crs2, immediate, 2);
                            break;
                        case 0b011000:
                            switch ((cins[2] & 0b100000) | (cins[1] & 0b11000)) {
                                case 0b000000: // sub命令
                                    result = alu.Sub(crd_rs1, crd_rs1, crs2, 2);
                                    break;

                                case 0b001000: // xor命令
                                    result = alu.Xor(crd_rs1, crd_rs1, crs2, 2);
                                    break;

                                case 0b010000: // or命令
                                    result = alu.Or(crd_rs1, crd_rs1, crs2, 2);
                                    break;

                                case 0b011000: // and命令
                                    result = alu.And(crd_rs1, crd_rs1, crs2, 2);
                                    break;
                            }
                            break;
                    }
                    break;

                case CompressedOpcode.slli: // slli命令
                    immediate = GetUnsignedImmediate("CI", cins);
                    immediate = immediate != 0 ? immediate : 64;
                    alu = (RV32_Alu)cpu.Alu(typeof(RV32_Alu));
                    result = alu.Slli(rd_rs1, rd_rs1, immediate, 2);
                    break;

                case CompressedOpcode.jr_mv_add:
                    if (cins[1] != 0 && (cins[2] & 0x1f) != 0) {
                        alu = (RV32_Alu)cpu.Alu(typeof(RV32_Alu));
                        if ((cins[2] & 0b100000) == 0) {
                            // mv命令
                            result = alu.Add(rd_rs1, Register.zero, rs2, 2);
                        } else {
                            // add命令
                            result = alu.Add(rd_rs1, rd_rs1, rs2, 2);
                        }
                    } else if (cins[1] == 0 && (cins[2] & 0x1f) != 0) {
                        // jalr命令
                        lsu = (RV32_Lsu)cpu.Lsu(typeof(RV32_Lsu));
                        result = lsu.Jalr((Register)(cins[2] >> 5), rd_rs1, 0, 2);

                    } else if (cins[1] == 0 && cins[2] == 0b100000) {
                        // ebreak命令
                        lsu = (RV32_Lsu)cpu.Lsu(typeof(RV32_Lsu));
                        result = lsu.Ebreak(2);
                    }
                    break;

                case CompressedOpcode.addi4spn: // addi4spn命令
                    immediate = GetUnsignedImmediate("CIW", cins);
                    if (immediate != 0) {
                        alu = (RV32_Alu)cpu.Alu(typeof(RV32_Alu));
                        result = alu.Addi(rd_rs1, Register.sp, immediate, 2);
                    }
                    break;

                case CompressedOpcode.li: // li命令
                    alu = (RV32_Alu)cpu.Alu(typeof(RV32_Alu));
                    immediate = GetSignedImmediate("CI", cins);
                    result = alu.Addi(rd_rs1, Register.zero, immediate, 2);
                    break;

                case CompressedOpcode.lui_addi16sp: // lui_addi16sp命令
                    alu = (RV32_Alu)cpu.Alu(typeof(RV32_Alu));
                    immediate = GetUnsignedImmediate("CI", cins);
                    if (immediate != 0) {
                        if ((cins[2] & 0x1f) == 0b000010) {
                            immediate |= (immediate & 0b100000) << 4;
                            immediate |= (immediate & 0b001000) << 3;
                            immediate |= (immediate & 0b000110) << 6;
                            immediate |= (immediate & 0b000001) << 5;
                            immediate &= 0b1111110000;
                            result = alu.Addi(rd_rs1, Register.zero, immediate, 2);
                        } else if ((cins[2] & 0x1f) != 0) {
                            result = alu.Lui(rd_rs1, immediate << 12, 2);
                        }
                    }
                    break;

                case CompressedOpcode.jal: // jal命令
                    immediate = GetUnsignedImmediate("CJ", cins);
                    lsu = (RV32_Lsu)cpu.Lsu(typeof(RV32_Lsu));
                    result = lsu.Jal(Register.ra, immediate, 2);
                    break;

                case CompressedOpcode.j: // j命令
                    immediate = GetUnsignedImmediate("CJ", cins);
                    lsu = (RV32_Lsu)cpu.Lsu(typeof(RV32_Lsu));
                    result = lsu.Jal(Register.zero, immediate, 2);
                    break;

                case CompressedOpcode.beqz: // beqz命令
                    immediate = GetUnsignedImmediate("CB", cins);
                    lsu = (RV32_Lsu)cpu.Lsu(typeof(RV32_Lsu));
                    result = lsu.Beq(Register.zero, crd_rs1, immediate, 2);
                    break;

                case CompressedOpcode.bnez: // bnez命令
                    immediate = GetUnsignedImmediate("CB", cins);
                    lsu = (RV32_Lsu)cpu.Lsu(typeof(RV32_Lsu));
                    result = lsu.Bne(Register.zero, crd_rs1, immediate, 2);
                    break;

                case CompressedOpcode.lw: // lw命令
                    immediate = GetUnsignedImmediate("CL", cins, 2);
                    lsu = (RV32_Lsu)cpu.Lsu(typeof(RV32_Lsu));
                    result = lsu.Lw(crs2, crd_rs1, immediate, 2);
                    break;

                case CompressedOpcode.flw: // flw命令
                    immediate = GetUnsignedImmediate("CL", cins, 2);
                    fplsu = (RV32_FloatPointLsu)cpu.Lsu(typeof(RV32_FloatPointLsu));
                    result = fplsu.Flw((FPRegister)crd_rs1, crs2, immediate, 2);
                    break;

                case CompressedOpcode.fld: // fld命令
                    immediate = GetUnsignedImmediate("CL", cins, 3);
                    fplsu = (RV32_FloatPointLsu)cpu.Lsu(typeof(RV32_FloatPointLsu));
                    result = fplsu.Fld((FPRegister)crd_rs1, crs2, immediate, 2);
                    break;

                case CompressedOpcode.sw: // sw命令
                    immediate = GetUnsignedImmediate("CL", cins, 2);
                    lsu = (RV32_Lsu)cpu.Lsu(typeof(RV32_Lsu));
                    result = lsu.Sw(crd_rs1, crs2, immediate, 2);
                    break;

                case CompressedOpcode.fsw: // fsw命令
                    immediate = GetUnsignedImmediate("CL", cins, 2);
                    fplsu = (RV32_FloatPointLsu)cpu.Lsu(typeof(RV32_FloatPointLsu));
                    result = fplsu.Fsw(crd_rs1, (FPRegister)crs2, immediate, 2);
                    break;

                case CompressedOpcode.fsd: // fsd命令
                    immediate = GetUnsignedImmediate("CL", cins, 3);
                    fplsu = (RV32_FloatPointLsu)cpu.Lsu(typeof(RV32_FloatPointLsu));
                    result = fplsu.Fsd(crd_rs1, (FPRegister)crs2, immediate, 2);
                    break;

                case CompressedOpcode.lwsp: // lwsp命令
                    immediate = GetUnsignedImmediate("CI", cins, 2);
                    lsu = (RV32_Lsu)cpu.Lsu(typeof(RV32_Lsu));
                    result = lsu.Lw(rd_rs1, Register.sp, immediate, 2);
                    break;

                case CompressedOpcode.flwsp: // flwsp命令
                    immediate = GetUnsignedImmediate("CI", cins, 2);
                    fplsu = (RV32_FloatPointLsu)cpu.Lsu(typeof(RV32_FloatPointLsu));
                    result = fplsu.Flw((FPRegister)crd_rs1, Register.sp, immediate, 2);
                    break;

                case CompressedOpcode.fldsp: // fldsp命令
                    immediate = GetUnsignedImmediate("CI", cins, 3);
                    fplsu = (RV32_FloatPointLsu)cpu.Lsu(typeof(RV32_FloatPointLsu));
                    result = fplsu.Fld((FPRegister)crd_rs1, Register.sp, immediate, 2);
                    break;

                case CompressedOpcode.swsp: // swsp命令
                    immediate = GetUnsignedImmediate("CSS", cins, 2);
                    lsu = (RV32_Lsu)cpu.Lsu(typeof(RV32_Lsu));
                    result = lsu.Sw(rs2, Register.sp, immediate, 2);
                    break;

                case CompressedOpcode.fswsp: // fswsp命令
                    immediate = GetUnsignedImmediate("CSS", cins, 2);
                    fplsu = (RV32_FloatPointLsu)cpu.Lsu(typeof(RV32_FloatPointLsu));
                    result = fplsu.Fsw(Register.sp, (FPRegister)crs2, immediate, 2);
                    break;

                case CompressedOpcode.fsdsp: // fsdsp命令
                    immediate = GetUnsignedImmediate("CSS", cins, 3);
                    fplsu = (RV32_FloatPointLsu)cpu.Lsu(typeof(RV32_FloatPointLsu));
                    result = fplsu.Fsd(Register.sp, (FPRegister)crs2, immediate, 2);
                    break;

            }
            return result;
        }



        private byte[] GetCompressedInstruction(byte[] ins) {
            UInt16 instruction = (UInt16)(ins[0] + (ins[1] << 7) + (ins[2] << 12) + (ins[3] << 15));

            bool[] bits = BitByteConverter.ToBits(BitConverter.GetBytes(instruction)).ToArray();
            byte[] cIns = new byte[3];
            cIns[0] = (byte)(BitByteConverter.GetBytes(bits, 0, 2).First() + (BitByteConverter.GetBytes(bits, 13, 3).First() << 2));
            cIns[1] = BitByteConverter.GetBytes(bits, 2, 5).First();
            cIns[2] = BitByteConverter.GetBytes(bits, 7, 6).First();
            return cIns;
        }

        private Int32 GetUnsignedImmediate(string format, byte[] cins, int scaleCount = 0) {
            UInt32 result;
            UInt32[] i = cins.Select(b => (UInt32)b).ToArray();
            int length, j = 0;

            switch (format) {
                case "CI":
                    length = 6;
                    result = i[1] | i[2] & 0b100000u;
                    break;
                case "CL":
                    length = 7;
                    result = ((i[1] & 0b11000u) >> 2) + (i[2] & 0b111000u);
                    j++;
                    scaleCount--;
                    break;
                case "CSS":
                    length = 6;
                    result = i[1] | i[2] & 0b100000u;
                    break;
                case "CB":
                    scaleCount = 0;
                    length = 9;
                    result = (i[1] & 0b00110) + (i[2] & 0b11000) + ((i[1] & 0b00001) >> 4) + (((i[1] & 0b11000) + (i[2] & 0b100000)) << 3);
                    break;
                case "CJ":
                    scaleCount = 0;
                    length = 6;
                    result = (i[1] & 0b01110) + (i[2] & 0b010000) + ((i[1] & 0b00001) << 5) + (((i[2] & 0b101101) + (i[1] & 0b000001)) << 6) + ((i[1] & 0b10000) << 3) + ((i[1] & 0b10000) << 5) + ((i[2] & 0b000010) << 9);
                    break;
                case "CIW":
                    scaleCount = 0;
                    length = 10;
                    result = ((i[1] & 0b10000) >> 2) + (i[1] & 0b01000) + (i[2] & 0b110000) + ((i[2] & 0b001111) << 6);
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
        private Int32 GetSignedImmediate(string format, byte[] cins) {
            UInt32 result;
            UInt32[] i = cins.Select(b => (UInt32)b).ToArray();
            int length;

            switch (format) {
                case "CI":
                    length = 6;
                    UInt32 i2 = i[2] & 0b100000u;
                    result = i[1] | i2;
                    result |= i2 == 0u ? 0u : 0xffffffc0u;
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
