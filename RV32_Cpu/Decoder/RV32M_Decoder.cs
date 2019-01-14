using RV32_Alu;
using RV32_Cpu.Decoder.Constants;
using RV32_Lsu.Constants;
using System;

namespace RV32_Cpu.Decoder {
    public class RV32M_Decoder : RV32_AbstractDecoder {

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
            Funct7 funct7 = (Funct7)(ins[5] + (ins[6] << 6));
            RV32_Mac alu;


            if (opcode == Opcode.miscOp && funct7 == Funct7.mul_div) { // Op系命令(算術論理演算)
                alu = (RV32_Mac)cpu.Alu(typeof(RV32_Mac));
                switch (funct3) {

                    case Funct3.mul: // mul命令
                        result = alu.Mul(rd, rs1, rs2);
                        break;

                    case Funct3.mulh: // mulh命令
                        result = alu.Mulh(rd, rs1, rs2);
                        break;

                    case Funct3.mulhsu: // mulhsu命令
                        result = alu.Mulhsu(rd, rs1, rs2);
                        break;

                    case Funct3.mulhu: // mulhu命令
                        result = alu.Mulhu(rd, rs1, rs2);
                        break;

                    case Funct3.div: // div命令
                        result = alu.Div(rd, rs1, rs2);
                        break;

                    case Funct3.divu: // divu命令
                        result = alu.Divu(rd, rs1, rs2);
                        break;

                    case Funct3.rem: // rem命令
                        result = alu.Rem(rd, rs1, rs2);
                        break;

                    case Funct3.remu: // remu命令
                        result = alu.Remu(rd, rs1, rs2);
                        break;
                }
            }
            return result;
        }
    }
}