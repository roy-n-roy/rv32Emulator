using RV32_Alu;
using RV32_Cpu.Decoder.Constants;
using RV32_Lsu;
using RV32_Lsu.Constants;
using System;

namespace RV32_Cpu.Decoder {
    public class RV32D_Decoder : RV32_AbstractDecoder {

        /// <summary>
        /// 引数で渡された32bit長の命令をデコードし、cpuで実行する
        /// </summary>
        /// <param name="instruction">32bit長の命令</param>
        /// <param name="cpu">命令を実行するRV32CPU</param>
        /// <returns>実行の成否</returns>
        internal protected override bool Exec(UInt32[] ins, RV32_HaedwareThread cpu) {
            bool result = false;

            // 命令の0～1bit目が "11" でない場合は対象なし
            if ((ins[0] & 0x11u) != 0b11u) {
                return result;
            }

            FPRegister rd = (FPRegister)ins[1],
                        rs1 = (FPRegister)ins[3],
                        rs2 = (FPRegister)ins[4],
                        rs3 = (FPRegister)((ins[5] >> 2) | (ins[6] << 4));
            Opcode opcode = (Opcode)ins[0];
            Funct3 funct3 = (Funct3)ins[2];
            Funct5 funct5 = (Funct5)(ins[5] >> 2 | (ins[6] << 4));
            FloatRoundingMode frm = (FloatRoundingMode)ins[2];
            Int32 immediate = 0;
            RV32_DoubleFpu fpu;
            RV32_FloatPointLsu lsu;

            switch (opcode) {
                case Opcode.fl when funct3 == Funct3.fld_fsd: // fld命令
                    immediate = GetImmediate('I', ins);
                    lsu = (RV32_FloatPointLsu)cpu.Lsu(typeof(RV32_FloatPointLsu));
                    result = lsu.Fld(rd, (Register)rs1, immediate);
                    break;

                case Opcode.fs when funct3 == Funct3.fld_fsd: // fsd命令
                    immediate = GetImmediate('S', ins);
                    lsu = (RV32_FloatPointLsu)cpu.Lsu(typeof(RV32_FloatPointLsu));
                    result = lsu.Fsd((Register)rs1, rs2, immediate);
                    break;

                case Opcode.fmadd when (ins[5] & 0x3u) == 0x1u: // fmaddd命令
                    fpu = (RV32_DoubleFpu)cpu.Alu(typeof(RV32_DoubleFpu));
                    result = fpu.FmaddD(rd, rs1, rs2, rs3, frm);
                    break;

                case Opcode.fmsub when (ins[5] & 0x3u) == 0x1u: // fmsubd命令
                    fpu = (RV32_DoubleFpu)cpu.Alu(typeof(RV32_DoubleFpu));
                    result = fpu.FmsubD(rd, rs1, rs2, rs3, frm);
                    break;

                case Opcode.fnmadd when (ins[5] & 0x3u) == 0x1u: // fnmaddd命令
                    fpu = (RV32_DoubleFpu)cpu.Alu(typeof(RV32_DoubleFpu));
                    result = fpu.FnmaddD(rd, rs1, rs2, rs3, frm);
                    break;

                case Opcode.fnmsub when (ins[5] & 0x3u) == 0x1u: // fnmsubd命令
                    fpu = (RV32_DoubleFpu)cpu.Alu(typeof(RV32_DoubleFpu));
                    result = fpu.FnmsubD(rd, rs1, rs2, rs3, frm);
                    break;

                case Opcode.fmiscOp when (ins[5] & 0x3u) == 0x1u: // Double Float-Point Op系命令(算術論理演算)
                    fpu = (RV32_DoubleFpu)cpu.Alu(typeof(RV32_DoubleFpu));
                    switch (funct5) {
                        case Funct5.fadd: // faddd命令
                            result = fpu.FaddD(rd, rs1, rs2, frm);
                            break;

                        case Funct5.fsub: // fsubd命令
                            result = fpu.FsubD(rd, rs1, rs2, frm);
                            break;

                        case Funct5.fmul: // fmuld命令
                            result = fpu.FmulD(rd, rs1, rs2, frm);
                            break;

                        case Funct5.fdiv: // fdivd命令
                            result = fpu.FdivD(rd, rs1, rs2, frm);
                            break;

                        case Funct5.fsqrt: // fsqrtd命令
                            result = fpu.FsqrtD(rd, rs1, frm);
                            break;

                        case Funct5.fsgnj:
                            switch (funct3) {
                                case Funct3.fsgnj: // fsgnjd命令
                                    result = fpu.FsgnjD(rd, rs1, rs2);
                                    break;

                                case Funct3.fsgnjn: // fsgnjnd命令
                                    result = fpu.FsgnjnD(rd, rs1, rs2);
                                    break;

                                case Funct3.fsgnjx: // fsgnjxd命令
                                    result = fpu.FsgnjxD(rd, rs1, rs2);
                                    break;
                            }
                            break;

                        case Funct5.fmin_fmax:
                            switch (funct3) {
                                case Funct3.fmin: // fmind命令
                                    result = fpu.FminD(rd, rs1, rs2);
                                    break;

                                case Funct3.fmax: // fmaxd命令
                                    result = fpu.FmaxD(rd, rs1, rs2);
                                    break;
                            }
                            break;

                        case Funct5.fcompare:
                            switch (funct3) {
                                case Funct3.fcompare_eq: // feqd命令
                                    result = fpu.FeqD((Register)rd, rs1, rs2);
                                    break;

                                case Funct3.fcompare_lt: // fltd命令
                                    result = fpu.FltD((Register)rd, rs1, rs2);
                                    break;

                                case Funct3.fcompare_le: // fled命令
                                    result = fpu.FleD((Register)rd, rs1, rs2);
                                    break;
                            }
                            break;

                        case Funct5.fcvttoW:
                            switch (ins[4]) {
                                case 0x0: // fcvtwd命令
                                    result = fpu.FcvtWD((Register)rd, rs1, frm);
                                    break;

                                case 0x1: // fcvtwud命令
                                    result = fpu.FcvtWUD((Register)rd, rs1, frm);
                                    break;
                            }
                            break;

                        case Funct5.fcvtfromW:
                            switch (ins[4]) {
                                case 0x0: // fcvtdw命令
                                    result = fpu.FcvtDW(rd, (Register)rs1, frm);
                                    break;

                                case 0x1: // fcvtdwu命令
                                    result = fpu.FcvtDWU(rd, (Register)rs1, frm);
                                    break;
                            }
                            break;

                        case Funct5.fmvXW_fclass: // fclassd命令
                            result = fpu.FclassD((Register)rd, rs1);
                            break;

                        case Funct5.fcvtSD: // fcvtds命令
                            result = fpu.FcvtDS(rd, rs1, frm);
                            break;
                    }
                    break;

                case Opcode.fmiscOp when (ins[5] & 0x3u) == 0x0u && funct5 == Funct5.fcvtSD:  // fcvtsd命令
                    fpu = (RV32_DoubleFpu)cpu.Alu(typeof(RV32_DoubleFpu));
                    result = fpu.FcvtSD(rd, rs1, frm);
                    break;
            }
            return result;
        }
    }
}