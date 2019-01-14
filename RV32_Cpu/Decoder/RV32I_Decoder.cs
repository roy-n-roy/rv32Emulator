﻿using RV32_Alu;
using RV32_Cpu.Decoder.Constants;
using RV32_Lsu;
using RV32_Lsu.Constants;
using System;

namespace RV32_Cpu.Decoder {
    public class RV32I_Decoder : RV32_AbstractDecoder {

        /// <summary>
        /// 引数で渡された32bit長の命令をデコードし、cpuで実行する
        /// </summary>
        /// <param name="instruction">32bit長の命令</param>
        /// <param name="cpu">命令を実行するRV32CPU</param>
        /// <returns>実行の成否</returns>
        internal protected override bool Exec(UInt32[] ins, RV32_HaedwareThread cpu) {
            bool result = false;

            // 命令の0～1bit目が "11" でない場合は対象なし
            if ((ins[0] & 0b11u) != 0b11u) {
                return result;
            }

            Register rd = (Register)ins[1],
                        rs1 = (Register)ins[3],
                        rs2 = (Register)ins[4];
            Opcode opcode = (Opcode)ins[0];
            Funct3 funct3 = (Funct3)ins[2];
            Funct7 funct7 = (Funct7)(ins[5] | (ins[6] << 6));
            Int32 immediate = 0;
            RV32_IntegerAlu alu;
            RV32_IntegerLsu lsu;

            switch (opcode) {
                case Opcode.lui: // lui命令
                    immediate = GetImmediate('U', ins);
                    alu = (RV32_IntegerAlu)cpu.Alu(typeof(RV32_IntegerAlu));
                    result = alu.Lui(rd, immediate);
                    break;


                case Opcode.auipc: // auipc命令
                    immediate = GetImmediate('U', ins);
                    alu = (RV32_IntegerAlu)cpu.Alu(typeof(RV32_IntegerAlu));
                    result = alu.Auipc(rd, immediate);
                    break;

                case Opcode.jal: // jal命令
                    immediate = GetImmediate('J', ins);
                    lsu = (RV32_IntegerLsu)cpu.Lsu(typeof(RV32_IntegerLsu));
                    result = lsu.Jal(rd, immediate);
                    break;

                case Opcode.jalr: // jalr命令
                    if (funct3 == Funct3.jalr) {
                        immediate = GetImmediate('I', ins);
                        lsu = (RV32_IntegerLsu)cpu.Lsu(typeof(RV32_IntegerLsu));
                        result = lsu.Jalr(rd, rs1, immediate);
                    }
                    break;

                case Opcode.branch: // Branch系命令
                    immediate = GetImmediate('B', ins);
                    lsu = (RV32_IntegerLsu)cpu.Lsu(typeof(RV32_IntegerLsu));
                    switch (funct3) {
                        case Funct3.beq: // beq命令
                            result = lsu.Beq(rs1, rs2, immediate);
                            break;

                        case Funct3.bne: // bne命令
                            result = lsu.Bne(rs1, rs2, immediate);
                            break;

                        case Funct3.blt: // blt命令
                            result = lsu.Blt(rs1, rs2, immediate);
                            break;

                        case Funct3.bge: // bge命令
                            result = lsu.Bge(rs1, rs2, immediate);
                            break;

                        case Funct3.bltu: // bltu命令
                            result = lsu.Bltu(rs1, rs2, immediate);
                            break;

                        case Funct3.bgeu: // bgeu命令
                            result = lsu.Bgeu(rs1, rs2, immediate);
                            break;
                    }
                    break;

                case Opcode.load: // Load系命令
                    immediate = GetImmediate('I', ins);
                    lsu = (RV32_IntegerLsu)cpu.Lsu(typeof(RV32_IntegerLsu));
                    switch (funct3) {

                        case Funct3.lb: // lb命令
                            result = lsu.Lb(rd, rs1, immediate);
                            break;

                        case Funct3.lh: // lh命令
                            result = lsu.Lh(rd, rs1, immediate);
                            break;

                        case Funct3.lw: //010 lw命令
                            result = lsu.Lw(rd, rs1, immediate);
                            break;

                        case Funct3.lbu: //001 lbu命令
                            result = lsu.Lbu(rd, rs1, immediate);
                            break;

                        case Funct3.lhu: //101 lhu命令
                            result = lsu.Lhu(rd, rs1, immediate);
                            break;
                    }
                    break;

                case Opcode.store: // Store系命令
                    immediate = GetImmediate('S', ins);
                    lsu = (RV32_IntegerLsu)cpu.Lsu(typeof(RV32_IntegerLsu));
                    switch (funct3) {
                        case Funct3.sb: // sb命令
                            result = lsu.Sb(rs1, rs2, immediate);
                            break;

                        case Funct3.sh: // sh命令
                            result = lsu.Sh(rs1, rs2, immediate);
                            break;

                        case Funct3.sw: // sw命令
                            result = lsu.Sw(rs1, rs2, immediate);
                            break;
                    }
                    break;

                case Opcode.miscOpImm: // Op-imm系命令(即値算術論理演算)
                    alu = (RV32_IntegerAlu)cpu.Alu(typeof(RV32_IntegerAlu));
                    switch (funct3) {
                        case Funct3.addi: // addi命令
                            immediate = GetImmediate('I', ins);
                            result = alu.Addi(rd, rs1, immediate);
                            break;

                        case Funct3.xori: // xori命令
                            immediate = GetImmediate('I', ins);
                            result = alu.Xori(rd, rs1, immediate);
                            break;

                        case Funct3.ori: // ori命令
                            immediate = GetImmediate('I', ins);
                            result = alu.Ori(rd, rs1, immediate);
                            break;

                        case Funct3.andi: // andi命令
                            immediate = GetImmediate('I', ins);
                            result = alu.Andi(rd, rs1, immediate);
                            break;

                        case Funct3.slti: // slti命令
                            immediate = GetImmediate('I', ins);
                            result = alu.Slti(rd, rs1, immediate);
                            break;

                        case Funct3.sltiu: //110 sltiu命令
                            immediate = GetImmediate('I', ins);
                            result = alu.Sltiu(rd, rs1, immediate);
                            break;

                        case Funct3.slli: // slli命令
                            result = alu.Slli(rd, rs1, (Int32)(ins[4] | ((ins[5] & 0b1) << 5)));
                            break;

                        case Funct3.srli_srai: // srli/srai命令
                            switch (funct7) {
                                case Funct7.srli:
                                    result = alu.Srli(rd, rs1, (Int32)(ins[4] | ((ins[5] & 0b1) << 5)));
                                    break;

                                case Funct7.srai:
                                    result = alu.Srai(rd, rs1, (Int32)(ins[4] | ((ins[5] & 0b1) << 5)));
                                    break;
                            }
                            break;
                    }
                    break;

                case Opcode.miscOp: // Op系命令(算術論理演算)
                    alu = (RV32_IntegerAlu)cpu.Alu(typeof(RV32_IntegerAlu));
                    switch (((UInt16)funct3 | ((UInt16)funct7 << 3))) {
                        case (UInt16)Funct3.add_sub | ((UInt16)Funct7.add << 3): // add命令
                            result = alu.Add(rd, rs1, rs2);
                            break;

                        case (UInt16)Funct3.add_sub | ((UInt16)Funct7.sub << 3): // sub命令
                            result = alu.Sub(rd, rs1, rs2);
                            break;

                        case (UInt16)Funct3.xor | ((UInt16)Funct7.xor << 3): // xor命令
                            result = alu.Xor(rd, rs1, rs2);
                            break;

                        case (UInt16)Funct3.or | ((UInt16)Funct7.or << 3): // or命令
                            result = alu.Or(rd, rs1, rs2);
                            break;

                        case (UInt16)Funct3.and | ((UInt16)Funct7.and << 3): // and命令
                            result = alu.And(rd, rs1, rs2);
                            break;

                        case (UInt16)Funct3.slt | ((UInt16)Funct7.slt << 3): // slt命令
                            result = alu.Slt(rd, rs1, rs2);
                            break;

                        case (UInt16)Funct3.sltu | ((UInt16)Funct7.sltu << 3): // sltu命令
                            result = alu.Sltu(rd, rs1, rs2);
                            break;

                        case (UInt16)Funct3.sll | ((UInt16)Funct7.sll << 3): // sll命令
                            result = alu.Sll(rd, rs1, rs2);
                            break;

                        case (UInt16)Funct3.srl_sra | ((UInt16)Funct7.srl << 3): // srl命令
                            result = alu.Srl(rd, rs1, rs2);
                            break;

                        case (UInt16)Funct3.srl_sra | ((UInt16)Funct7.sra << 3): // sra命令
                            result = alu.Sra(rd, rs1, rs2);
                            break;
                    }
                    break;


                case Opcode.miscMem: // 同期命令
                    switch (funct3) {
                        case Funct3.fence: // fence命令
                            result = cpu.registerSet.Fence((byte)(((ins[5] & 0x7 ) << 5) | ins[4]));
                            break;

                        case Funct3.fenceI: // fence.i命令
                            result = cpu.registerSet.FenceI();
                            break;
                    }
                    break;
                case Opcode.privilege: // 特権命令
                    switch (funct3) {
                        case Funct3.privilege: // 特権命令
                            if (rd == 0) {
                                if (funct7 == Funct7.sfenceVma) {
                                    result = cpu.registerSet.SfenceVma(rs1, rs2);
                                } else {
                                    Funct12 funct12 = (Funct12)(ins[4] | (ins[5] << 5) | (ins[6] << 11)); ;
                                    switch (funct12) {
                                        case Funct12.ecall:
                                            result = cpu.registerSet.Ecall();
                                            break;

                                        case Funct12.ebreak:
                                            result = cpu.registerSet.Ebreak();
                                            break;

                                        case Funct12.mret:
                                            result = cpu.registerSet.Mret();
                                            break;

                                        case Funct12.sret:
                                            result = cpu.registerSet.Sret();
                                            break;

                                        case Funct12.uret:
                                            result = cpu.registerSet.Uret();
                                            break;

                                        case Funct12.wfi:
                                            result = cpu.registerSet.Wfi();
                                            break;
                                    }

                                }
                            }
                            break;

                        case Funct3.csrrw: // csrrw命令
                            immediate = (GetImmediate('I', ins) & 0xfff);
                            result = cpu.registerSet.Csrrw(rd, rs1, (CSR)immediate);
                            break;

                        case Funct3.csrrs: // csrrs命令
                            immediate = (GetImmediate('I', ins) & 0xfff);
                            result = cpu.registerSet.Csrrs(rd, rs1, (CSR)immediate);
                            break;

                        case Funct3.csrrc: // csrrc命令
                            immediate = (GetImmediate('I', ins) & 0xfff);
                            result = cpu.registerSet.Csrrc(rd, rs1, (CSR)immediate);
                            break;

                        case Funct3.csrrwi: // csrrwi命令
                            immediate = (GetImmediate('I', ins) & 0xfff);
                            result = cpu.registerSet.Csrrwi(rd, (byte)ins[3], (CSR)immediate);
                            break;

                        case Funct3.csrrsi: // csrrsi命令
                            immediate = (GetImmediate('I', ins) & 0xfff);
                            result = cpu.registerSet.Csrrsi(rd, (byte)ins[3], (CSR)immediate);
                            break;

                        case Funct3.csrrci: // csrrci命令
                            immediate = (GetImmediate('I', ins) & 0xfff);
                            result = cpu.registerSet.Csrrci(rd, (byte)ins[3], (CSR)immediate);
                            break;
                    }
                    break;
            }
            return result;
        }
    }
}
