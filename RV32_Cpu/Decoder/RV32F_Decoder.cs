using RiscVCpu.ArithmeticLogicUnit;
using RiscVCpu.Decoder.Constants;
using RiscVCpu.LoadStoreUnit;
using RiscVCpu.LoadStoreUnit.Constants;
using System;

namespace RiscVCpu.Decoder {
    public class RV32F_Decoder : RV32_AbstractDecoder {

        /// <summary>
        /// 引数で渡された32bit長の命令をデコードし、cpuで実行する
        /// </summary>
        /// <param name="instruction">32bit長の命令</param>
        /// <param name="cpu">命令を実行するRV32CPU</param>
        /// <returns>実行の成否</returns>
        internal protected override bool Exec(UInt32[] ins, RV32_Cpu cpu) {
            bool result = false;
            FPRegister rd = (FPRegister)ins[1],
                        rs1 = (FPRegister)ins[3],
                        rs2 = (FPRegister)ins[4],
                        rs3 = (FPRegister)((ins[5] >> 2) | (ins[6] << 4));
            Opcode opcode = (Opcode)ins[0];
            Funct3 funct3 = (Funct3)ins[2];
            Funct5 funct5 = (Funct5)(ins[5] >> 2 | (ins[6] << 4));
            FloatRoundingMode frm = (FloatRoundingMode)ins[2];
            Int32 immediate = 0;
            RV32_SingleFpu fpu;
            RV32_FloatPointLsu lsu;


            switch (opcode) {
                case Opcode.fl when funct3 == Funct3.flw_fsw: // flw命令

                    immediate = GetImmediate('I', ins);
                    lsu = (RV32_FloatPointLsu)cpu.Lsu(typeof(RV32_FloatPointLsu));
                    result = lsu.Flw(rd, (Register)rs1, immediate);
                    break;

                case Opcode.fs when funct3 == Funct3.flw_fsw: // fsw命令
                    immediate = GetImmediate('S', ins);
                    lsu = (RV32_FloatPointLsu)cpu.Lsu(typeof(RV32_FloatPointLsu));
                    result = lsu.Fsw((Register)rs1, rs2, immediate);
                    break;


                case Opcode.fmadd when (ins[5] & 0x3u) == 0x0u: // fmadds命令
                    fpu = (RV32_SingleFpu)cpu.Alu(typeof(RV32_SingleFpu));
                    result = fpu.FmaddS(rd, rs1, rs2, rs3, frm);
                    break;

                case Opcode.fmsub when (ins[5] & 0x3u) == 0x0u: // fmsubs命令
                    fpu = (RV32_SingleFpu)cpu.Alu(typeof(RV32_SingleFpu));
                    result = fpu.FmsubS(rd, rs1, rs2, rs3, frm);
                    break;

                case Opcode.fnmadd when (ins[5] & 0x3u) == 0x0u: // fnmadds命令
                    fpu = (RV32_SingleFpu)cpu.Alu(typeof(RV32_SingleFpu));
                    result = fpu.FnmaddS(rd, rs1, rs2, rs3, frm);
                    break;

                case Opcode.fnmsub when (ins[5] & 0x3u) == 0x0u: // fnmsubs命令
                    fpu = (RV32_SingleFpu)cpu.Alu(typeof(RV32_SingleFpu));
                    result = fpu.FnmsubS(rd, rs1, rs2, rs3, frm);
                    break;

                case Opcode.fmiscOp when (ins[5] & 0x3u) == 0x0u: // Single Float-Point Op系命令(算術論理演算)
                    fpu = (RV32_SingleFpu)cpu.Alu(typeof(RV32_SingleFpu));
                    switch (funct5) {
                        case Funct5.fadd: // fadds命令
                            result = fpu.FaddS(rd, rs1, rs2, frm);
                            break;

                        case Funct5.fsub: // fsubs命令
                            result = fpu.FsubS(rd, rs1, rs2, frm);
                            break;

                        case Funct5.fmul: // fmuls命令
                            result = fpu.FmulS(rd, rs1, rs2, frm);
                            break;

                        case Funct5.fdiv: // fmuls命令
                            result = fpu.FdivS(rd, rs1, rs2, frm);
                            break;

                        case Funct5.fsqrt: // fsqrts命令
                            result = fpu.FsqrtS(rd, rs1, frm);
                            break;

                        case Funct5.fsgnj:
                            switch (funct3) {
                                case Funct3.fsgnj: // fsgnjs命令
                                    result = fpu.FsgnjS(rd, rs1, rs2);
                                    break;

                                case Funct3.fsgnjn: // fsgnjns命令
                                    result = fpu.FsgnjnS(rd, rs1, rs2);
                                    break;

                                case Funct3.fsgnjx: // fsgnjxs命令
                                    result = fpu.FsgnjxS(rd, rs1, rs2);
                                    break;
                            }
                            break;

                        case Funct5.fmin_fmax:
                            switch (funct3) {
                                case Funct3.fmin: // fmins命令
                                    result = fpu.FminS(rd, rs1, rs2);
                                    break;

                                case Funct3.fmax: // fmaxs命令
                                    result = fpu.FmaxS(rd, rs1, rs2);
                                    break;
                            }
                            break;

                        case Funct5.fcompare:
                            switch (funct3) {
                                case Funct3.fcompare_eq: // feqs命令
                                    result = fpu.FeqS((Register)rd, rs1, rs2);
                                    break;

                                case Funct3.fcompare_lt: // flts命令
                                    result = fpu.FltS((Register)rd, rs1, rs2);
                                    break;

                                case Funct3.fcompare_le: // fles命令
                                    result = fpu.FleS((Register)rd, rs1, rs2);
                                    break;
                            }
                            break;

                        case Funct5.fcvttoW:
                            switch (ins[4]) {
                                case 0x0: // fcnvws命令
                                    result = fpu.FcvtWS((Register)rd, rs1, frm);
                                    break;

                                case 0x1: // fcnvwus命令
                                    result = fpu.FcvtWUS((Register)rd, rs1, frm);
                                    break;
                            }
                            break;

                        case Funct5.fcvtfromW:
                            switch (ins[4]) {
                                case 0x0: // fcnvsw命令
                                    result = fpu.FcvtSW(rd, (Register)rs1, frm);
                                    break;

                                case 0x1: // fcnvswu命令
                                    result = fpu.FcvtSWU(rd, (Register)rs1, frm);
                                    break;
                            }
                            break;

                        case Funct5.fmvXW_fclass:
                            if (funct3 == (Funct3)0b000u) { // fmvxw命令
                                result = fpu.FmvXW((Register)rd, rs1);

                            } else if (funct3 == (Funct3)0b001u) { // fclasss命令
                                result = fpu.FclassS((Register)rd, rs1);
                            }
                            break;

                        case Funct5.fmvWX: // fmvwx命令
                            result = fpu.FmvWX(rd, (Register)rs1);
                            break;

                    }
                    break;
            }
            return result;
        }
}
}