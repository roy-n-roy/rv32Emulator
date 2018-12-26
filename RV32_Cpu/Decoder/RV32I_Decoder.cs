﻿using RiscVCpu.ArithmeticLogicUnit;
using RiscVCpu.Constants;
using RiscVCpu.LoadStoreUnit;
using System;

namespace RiscVCpu.Decoder {
    public class RV32I_Decoder : RV32_AbstractDecoder {

        /// <summary>
        /// 引数で渡された32bit長の命令をデコードし、cpuで実行する
        /// </summary>
        /// <param name="instruction">32bit長の命令</param>
        /// <param name="cpu">命令を実行するRV32CPU</param>
        /// <returns>実行の成否</returns>
        /// <summary>
        internal protected override bool Exec(UInt32 instruction, RV32_Cpu cpu) {
            bool result = false;
            byte[] ins = SplitInstruction(instruction);
            Register rd = (Register)ins[1],
                        rs1 = (Register)ins[3],
                        rs2 = (Register)ins[4];
            Opcode opcode = (Opcode)ins[0];
            Funct3 funct3 = (Funct3)ins[2];
            Funct7 funct7 = (Funct7)(ins[5] + (ins[6] << 6));
            Int32 immediate = 0;
            RV32_Alu alu;



#if DEBUG
            string debugStr = cpu.registerSet.PC.ToString("X") + "  " + opcode.ToString() + " ";
#endif
            switch (opcode) {
                case Opcode.lui: // lui命令
                    immediate = GetImmediate("U", ins);
                    result = cpu.Lsu.Lui(rd, immediate);
                    break;


                case Opcode.auipc: // auipc命令
                    immediate = GetImmediate("U", ins);
                    result = cpu.Lsu.Auipc(rd, immediate);
                    break;

                case Opcode.jal: // jal命令
                    immediate = GetImmediate("J", ins);
                    result = cpu.Lsu.Jal(rd, immediate);
                    break;

                case Opcode.jalr: // jalr命令
                    if (funct3 == Funct3.jalr) {
                        immediate = GetImmediate("I", ins);
                        result = cpu.Lsu.Jalr(rd, rs1, immediate);
                    }
                    break;

                case Opcode.branch: // Branch系命令
                    immediate = GetImmediate("B", ins);
                    switch (funct3) {
                        case Funct3.beq: // beq命令
                            result = cpu.Lsu.Beq(rs1, rs2, immediate);
                            break;

                        case Funct3.bne: // bne命令
                            result = cpu.Lsu.Bne(rs1, rs2, immediate);
                            break;

                        case Funct3.blt: // blt命令
                            result = cpu.Lsu.Blt(rs1, rs2, immediate);
                            break;

                        case Funct3.bge: // bge命令
                            result = cpu.Lsu.Bge(rs1, rs2, immediate);
                            break;

                        case Funct3.bltu: // bltu命令
                            result = cpu.Lsu.Bltu(rs1, rs2, immediate);
                            break;

                        case Funct3.bgeu: // bgeu命令
                            result = cpu.Lsu.Bgeu(rs1, rs2, immediate);
                            break;
                    }
                    break;

                case Opcode.load: // Load系命令
                    immediate = GetImmediate("I", ins);
                    switch (funct3) {

                        case Funct3.lb: // lb命令
                            result = cpu.Lsu.Lb(rd, rs1, immediate);
                            break;

                        case Funct3.lh: // lh命令
                            result = cpu.Lsu.Lh(rd, rs1, immediate);
                            break;

                        case Funct3.lw: //010 lw命令
                            result = cpu.Lsu.Lw(rd, rs1, immediate);
                            break;

                        case Funct3.lbu: //001 lbu命令
                            result = cpu.Lsu.Lbu(rd, rs1, immediate);
                            break;

                        case Funct3.lhu: //101 lhu命令
                            result = cpu.Lsu.Lhu(rd, rs1, immediate);
                            break;
                    }
                    break;

                case Opcode.store: // Store系命令
                    immediate = GetImmediate("S", ins);
                    switch (funct3) {
                        case Funct3.sb: // sb命令
                            result = cpu.Lsu.Sb(rs1, rs2, immediate);
                            break;

                        case Funct3.sh: // sh命令
                            result = cpu.Lsu.Sh(rs1, rs2, immediate);
                            break;

                        case Funct3.sw: // sw命令
                            result = cpu.Lsu.Sw(rs1, rs2, immediate);
                            break;
                    }
                    break;

                case Opcode.opimm: // Op-imm系命令(即値算術論理演算)
                    immediate = GetImmediate("I", ins);
                    alu = (RV32_Alu)cpu.Alu(typeof(RV32_Alu));
                    switch (funct3) {
                        case Funct3.addi: // addi命令
                            result = alu.Addi(rd, rs1, immediate);
                            break;

                        case Funct3.xori: // xori命令
                            result = alu.Xori(rd, rs1, immediate);
                            break;

                        case Funct3.ori: // ori命令
                            result = alu.Ori(rd, rs1, immediate);
                            break;

                        case Funct3.andi: // andi命令
                            result = alu.Andi(rd, rs1, immediate);
                            break;


                        case Funct3.slti: // slti命令
                            result = alu.Slti(rd, rs1, immediate);
                            break;

                        case Funct3.sltiu: //110 sltiu命令
                            result = alu.Sltiu(rd, rs1, immediate);
                            break;


                        case Funct3.slli: // slli命令
                            result = alu.Slli(rd, rs1, ins[4] + ((ins[5] & 0b1) << 5));
                            break;

                        case Funct3.srli_srai: // srli/srai命令
                            switch (funct7) {
                                case Funct7.srli:
                                    result = alu.Srli(rd, rs1, ins[4] + ((ins[5] & 0b1) << 5));
                                    break;

                                case Funct7.srai:
                                    result = alu.Srai(rd, rs1, ins[4] + ((ins[5] & 0b1) << 5));
                                    break;
                            }
                            break;
                    }
                    break;

                case Opcode.op: // Op系命令(算術論理演算)
                    alu = (RV32_Alu)cpu.Alu(typeof(RV32_Alu));
                    switch (((UInt16)funct3 + ((UInt16)funct7 << 3))) {
                        case (UInt16)Funct3.add_sub + ((UInt16)Funct7.add << 3): // add命令
                            result = alu.Add(rd, rs1, rs2);
                            break;

                        case (UInt16)Funct3.add_sub + ((UInt16)Funct7.sub << 3): // sub命令
                            result = alu.Sub(rd, rs1, rs2);
                            break;

                        case (UInt16)Funct3.xor + ((UInt16)Funct7.xor << 3): // xor命令
                            result = alu.Xor(rd, rs1, rs2);
                            break;

                        case (UInt16)Funct3.or + ((UInt16)Funct7.or << 3): // or命令
                            result = alu.Or(rd, rs1, rs2);
                            break;

                        case (UInt16)Funct3.and + ((UInt16)Funct7.and << 3): // and命令
                            result = alu.And(rd, rs1, rs2);
                            break;

                        case (UInt16)Funct3.slt + ((UInt16)Funct7.slt << 3): // slt命令
                            result = alu.Slt(rd, rs1, rs2);
                            break;

                        case (UInt16)Funct3.sltu + ((UInt16)Funct7.sltu << 3): // sltu命令
                            result = alu.Sltu(rd, rs1, rs2);
                            break;

                        case (UInt16)Funct3.sll + ((UInt16)Funct7.sll << 3): // sll命令
                            result = alu.Sll(rd, rs1, rs2);
                            break;

                        case (UInt16)Funct3.srl_sra + ((UInt16)Funct7.srl << 3): // srl命令
                            result = alu.Srl(rd, rs1, rs2);
                            break;

                        case (UInt16)Funct3.srl_sra + ((UInt16)Funct7.sra << 3): // sra命令
                            result = alu.Sra(rd, rs1, rs2);
                            break;
                    }
                    break;


                case Opcode.miscMem: // 同期命令
                    switch (funct3) {
                        case Funct3.fence: // fence命令
                            result = cpu.Lsu.Fence(BitConverter.GetBytes(instruction << 4)[3]);
                            break;

                        case Funct3.fenceI: // fence.i命令
                            result = cpu.Lsu.FenceI();
                            break;
                    }
                    break;
                case Opcode.privilege: // 特権命令
                    switch (funct3) {
                        case Funct3.privilege: // 特権命令
                            if (rd == 0) {
                                if (funct7 == Funct7.sfenceVma) {
                                    result = cpu.Lsu.SfenceVma(rs1, rs2);
                                } else {
                                    Funct12 funct12 = (Funct12)(ins[4] + (ins[5] << 5) + (ins[6] << 11)); ;
                                    switch (funct12) {
                                        case Funct12.ecall:
                                            result = cpu.Lsu.Ecall();
                                            break;

                                        case Funct12.ebreak:
                                            result = cpu.Lsu.Ebreak();
                                            break;

                                        case Funct12.sret:
                                            result = cpu.Lsu.Sret();
                                            break;

                                        case Funct12.wfi:
                                            result = cpu.Lsu.Wfi();
                                            break;

                                        case Funct12.mret:
                                            result = cpu.Lsu.Mret();
                                            break;
                                    }

                                }
                            }
                            break;

                        case Funct3.csrrw: // csrrw命令
                            immediate = (GetImmediate("I", ins) & 0xFFF);
                            result = cpu.Lsu.Csrrw(rd, rs1, (CSR)immediate);
                            break;

                        case Funct3.csrrs: // csrrs命令
                            immediate = (GetImmediate("I", ins) & 0xFFF);
                            result = cpu.Lsu.Csrrs(rd, rs1, (CSR)immediate);
                            break;

                        case Funct3.csrrc: // csrrc命令
                            immediate = (GetImmediate("I", ins) & 0xFFF);
                            result = cpu.Lsu.Csrrc(rd, rs1, (CSR)immediate);
                            break;

                        case Funct3.csrrwi: // csrrwi命令
                            immediate = (GetImmediate("I", ins) & 0xFFF);
                            result = cpu.Lsu.Csrrwi(rd, ins[3], (CSR)immediate);
                            break;

                        case Funct3.csrrsi: // csrrsi命令
                            immediate = (GetImmediate("I", ins) & 0xFFF);
                            result = cpu.Lsu.Csrrsi(rd, ins[3], (CSR)immediate);
                            break;

                        case Funct3.csrrci: // csrrci命令
                            immediate = (GetImmediate("I", ins) & 0xFFF);
                            result = cpu.Lsu.Csrrci(rd, ins[3], (CSR)immediate);
                            break;
                    }
                    break;
            }

#if DEBUG
            if ((ins[0] & 0b111) == 0b111 && opcode != Opcode.jalr) {
                //U,J形式
                debugStr += rd + ", " + immediate.ToString("X");
            } else {
                debugStr += funct3 + " ";
                if ((ins[0] & 0b100011) == 0b100011) {
                    //B,S形式
                    debugStr += rs1 + ", " + rs2 + " " + immediate.ToString("X");
                } else if (ins[0] == 0b0110011) {
                    //R形式
                    debugStr += funct7 + " " + rd + ", " + rs1 + ", " + rs2;
                } else {
                    //I形式
                    debugStr += rd + ", " + rs1 + ", " + immediate.ToString("X");
                }
            }
            Console.WriteLine(debugStr);
#endif
            return result;
        }
    }
}
