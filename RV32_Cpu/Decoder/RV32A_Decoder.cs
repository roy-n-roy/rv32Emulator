using RV32_Cpu.Decoder.Constants;
using RV32_Lsu;
using RV32_Lsu.Constants;
using System;

namespace RV32_Cpu.Decoder {
    public class RV32A_Decoder : RV32_AbstractDecoder {

        /// <summary>
        /// 引数で渡された32bit長の命令をデコードし、cpuで実行する
        /// </summary>
        /// <param name="instruction">32bit長の命令</param>
        /// <param name="cpu">命令を実行するRV32CPU</param>
        /// <returns>実行の成否</returns>
        internal protected override bool Exec(UInt32[] ins, RV32_HaedwareThread cpu) {
            bool result = false;

            // 命令の0～1bit目が "11" でない場合は対象なし
            if ((ins[0] & 0b11u) != 0x11u) {
                return result;
            }

            Register rd = (Register)ins[1],
                        rs1 = (Register)ins[3],
                        rs2 = (Register)ins[4];
            Opcode opcode = (Opcode)ins[0];
            Funct3 funct3 = (Funct3)ins[2];
            Funct5 funct5 = (Funct5)((ins[5] >> 2) + (ins[6] << 4));
            RV32_AmoLsu lsu;


            if (opcode == Opcode.amo && funct3 == Funct3.amo) { // 不可分命令
                lsu = (RV32_AmoLsu)cpu.Lsu(typeof(RV32_AmoLsu));
                bool ac = (ins[5] & 0b10) > 0,
                     rl = (ins[5] & 0b01) > 0;
                switch (funct5) {

                    case Funct5.lr: // lr命令
                        result = lsu.LrW(rd, rs1, ac, rl);
                        break;

                    case Funct5.sc: // sc命令
                        result = lsu.ScW(rd, rs1, rs2, ac, rl);
                        break;

                    case Funct5.amo_swap: // swap命令
                        result = lsu.AmoSwapW(rd, rs1, rs2, ac, rl);
                        break;

                    case Funct5.amo_add: // add命令
                        result = lsu.AmoAddW(rd, rs1, rs2, ac, rl);
                        break;

                    case Funct5.amo_xor: // xor命令
                        result = lsu.AmoXorW(rd, rs1, rs2, ac, rl);
                        break;

                    case Funct5.amo_and: // and命令
                        result = lsu.AmoAndW(rd, rs1, rs2, ac, rl);
                        break;

                    case Funct5.amo_or: // or命令
                        result = lsu.AmoOrW(rd, rs1, rs2, ac, rl);
                        break;

                    case Funct5.amo_min: // min命令
                        result = lsu.AmoMinW(rd, rs1, rs2, ac, rl);
                        break;

                    case Funct5.amo_max: // max命令
                        result = lsu.AmoMaxW(rd, rs1, rs2, ac, rl);
                        break;

                    case Funct5.amo_minu: // minu命令
                        result = lsu.AmoMinuW(rd, rs1, rs2, ac, rl);
                        break;

                    case Funct5.amo_maxu: // maxu命令
                        result = lsu.AmoMaxuW(rd, rs1, rs2, ac, rl);
                        break;
                }
            }
            return result;
        }
    }
}