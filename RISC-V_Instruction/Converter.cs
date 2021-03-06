﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace RISC_V_Instruction {


    /// <summary>
    /// RISC-V命令をユーザに視認できる形式に変換するためのクラス
    /// </summary>
    public static partial class InstConverter {

        /// <summary>
        /// UInt32形式のバイナリRISC-V命令を
        /// RiscvInstruction構造体に変換して取得する
        /// </summary>
        /// <param name="ins">UInt32形式のバイナリRISC-V命令</param>
        /// <returns>変換したRiscvInstruction構造体</returns>
        public static RiscvInstruction GetInstruction(uint ins) {


            string name = "";
            string type = "";
            Dictionary<string, int> dic = new Dictionary<string, int>();

            foreach (RiscvInstructionMask insMask in InstructionList) {
                if ((ins & insMask.InstructionMask) == insMask.Instruction) {
                    name = insMask.Name;
                    type = insMask.InstructionType;
                    foreach (string argName in insMask.ArgumentMask.Keys) {
                        uint mask = insMask.ArgumentMask[argName];
                        int t = 0;
                        int j = 0;
                        foreach (int i in Enumerable.Range(0, 32)) {
                            uint bit = mask & (1u << i);
                            if (bit > 0U) {
                                if ((bit & ins) > 0U) {
                                    t |= 1 << j++;
                                } else {
                                    j++;
                                }
                            }
                        }

                        // 即値の場合の設定
                        if (argName.Equals("immediate")) {
                            j--;
                            // 最上位ビットが 1 の場合は、負数にする
                            if ((t & (1 << j)) > 0) {
                                t |= (1 << (j + 1)) - 1;
                            }

                            if (type.Equals("J")) {
                                int t1 = (t & 0x3fe00) >> 8;
                                int t2 = (t & 0x100) << (11 - 8);
                                int t3 = (t & 0xff) << 12;
                                int t4 = (t & 0x80000) << 1;
                                t = t1 | t2 | t3 | t4;
                            }

                            // Branch系,Store系命令の場合は、最下位ビットを最上位に繰り上げる
                            if (type.Equals("B") || type.Equals("S")) {
                                t = (t & ~(1 << j) & -2) | (t & 0x1) << j;
                            }
                        }
                        dic.Add(argName, t);
                    }
                    break;
                }
            }

            RiscvInstruction result = new RiscvInstruction(name, type, dic);
            return result;
        }

    }

    /// <summary>
    /// RISC-V命令を表すための構造体
    /// </summary>
    public readonly struct RiscvInstruction {
        /// <summary>命令名称</summary>
        public readonly string Name;
        /// <summary>命令形式</summary>
        public readonly string InstructionType;
        /// <summary>命令の引数にあたるレジスタ番号・即値など</summary>
        public readonly IReadOnlyDictionary<string, int> Arguments;

        public RiscvInstruction(string name, string insType, Dictionary<string, int> args) {
            Name = name;
            InstructionType = insType;
            Arguments = new ReadOnlyDictionary<string, int>(args);
        }
    }

    public static partial class InstConverter {
        /// <summary>
        /// RISC-V命令に含まれる整数レジスタ番号の変換リスト
        /// </summary>
        public static readonly IReadOnlyList<string> IntergerRegisterName = new List<string> {
            "x0/zero",
            "x1/ra",
            "x2/sp",
            "x3/gp",
            "x4/tp",
            "x5/t0",
            "x6/t1",
            "x7/t2",
            "x8/s0/fp",
            "x9/s1",
            "x10/a0",
            "x11/a1",
            "x12/a2",
            "x13/a3",
            "x14/a4",
            "x15/a5",
            "x16/a6",
            "x17/a7",
            "x18/s2",
            "x19/s3",
            "x20/s4",
            "x21/s5",
            "x22/s6",
            "x23/s7",
            "x24/s8",
            "x25/s9",
            "x26/s10",
            "x27/s11",
            "x28/t3",
            "x29/t4",
            "x30/t5",
            "x31/t6"
        }.AsReadOnly();

                /// <summary>
        /// RISC-V命令に含まれる浮動層数点レジスタ番号の変換リスト
        /// </summary>
        public static readonly IReadOnlyList<string> FloatPointRegisterName = new List<string> {
            "f0/ft0",
            "f1/ft0",
            "f2/ft0",
            "f3/ft0",
            "f4/tp",
            "f5/ft0",
            "f6/ft0",
            "f7/ft0",
            "f8/fs0",
            "f9/fs1",
            "f10/fa0",
            "f11/fa1",
            "f12/fa2",
            "f13/fa3",
            "f14/fa4",
            "f15/fa5",
            "f16/fa6",
            "f17/fa7",
            "f18/fs2",
            "f19/fs3",
            "f20/fs4",
            "f21/fs5",
            "f22/fs6",
            "f23/fs7",
            "f24/fs8",
            "f25/fs9",
            "f26/fs10",
            "f27/fs11",
            "f28/ft8",
            "f29/ft9",
            "f30/ft10",
            "f31/ft11"
        }.AsReadOnly();

        /// <summary>
        /// バイナリ形式のRisc-V命令から
        /// RiscvInstruction構造体を作成するために
        /// 必要なマスク情報などを格納した構造体
        /// </summary>
        private readonly struct RiscvInstructionMask {
            /// <summary>命令名称</summary>
            public readonly string Name;
            /// <summary>命令</summary>
            public readonly uint Instruction;
            /// <summary>命令部分のマスク</summary>
            public readonly uint InstructionMask;
            /// <summary>命令形式</summary>
            public readonly string InstructionType;
            /// <summary>命令の引数にあたるレジスタ番号・即値などを取り出すためのマスク</summary>
            public readonly IReadOnlyDictionary<string, uint> ArgumentMask;

            //コンストラクタ
            public RiscvInstructionMask(string name, uint ins, uint insMask, string insType, Dictionary<string, uint> argMask) {
                Name = name;
                Instruction = ins;
                InstructionMask = insMask;
                InstructionType = insType;
                ArgumentMask = argMask;
            }
        }

        /// <summary>
        /// RISC-V命令マスクのリスト
        /// </summary>
        private static readonly IReadOnlyList<RiscvInstructionMask> InstructionList = new List<RiscvInstructionMask> {
                new RiscvInstructionMask( "LUI",                                       0b00000000000000000000000000110111U, 0b00000000000000000000000001111111U,  "U",     new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "immediate", 0b11111111111111111111000000000000U } } ),
                new RiscvInstructionMask( "AUIPC",                                     0b00000000000000000000000000010111U, 0b00000000000000000000000001111111U,  "U",     new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "immediate", 0b11111111111111111111000000000000U } } ),
                new RiscvInstructionMask( "JAL",                                       0b00000000000000000000000001101111U, 0b00000000000000000000000001111111U,  "J",     new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "immediate", 0b11111111111111111111000000000000U } } ),
                new RiscvInstructionMask( "JALR",                                      0b00000000000000000000000001100111U, 0b00000000000000000111000001111111U,  "I",     new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "rs1", 0b00000000000011111000000000000000U }, { "immediate", 0b11111111111100000000000000000000U } } ),
                new RiscvInstructionMask( "BEQ",                                       0b00000000000000000000000001100011U, 0b00000000000000000111000001111111U,  "B",     new Dictionary<string, uint> { { "rs1", 0b00000000000011111000000000000000U }, { "rs2", 0b00000001111100000000000000000000U }, { "immediate", 0b11111110000000000000111110000000U } } ),
                new RiscvInstructionMask( "BNE",                                       0b00000000000000000001000001100011U, 0b00000000000000000111000001111111U,  "B",     new Dictionary<string, uint> { { "rs1", 0b00000000000011111000000000000000U }, { "rs2", 0b00000001111100000000000000000000U }, { "immediate", 0b11111110000000000000111110000000U } } ),
                new RiscvInstructionMask( "BLT",                                       0b00000000000000000100000001100011U, 0b00000000000000000111000001111111U,  "B",     new Dictionary<string, uint> { { "rs1", 0b00000000000011111000000000000000U }, { "rs2", 0b00000001111100000000000000000000U }, { "immediate", 0b11111110000000000000111110000000U } } ),
                new RiscvInstructionMask( "BGE",                                       0b00000000000000000101000001100011U, 0b00000000000000000111000001111111U,  "B",     new Dictionary<string, uint> { { "rs1", 0b00000000000011111000000000000000U }, { "rs2", 0b00000001111100000000000000000000U }, { "immediate", 0b11111110000000000000111110000000U } } ),
                new RiscvInstructionMask( "BLTU",                                      0b00000000000000000110000001100011U, 0b00000000000000000111000001111111U,  "B",     new Dictionary<string, uint> { { "rs1", 0b00000000000011111000000000000000U }, { "rs2", 0b00000001111100000000000000000000U }, { "immediate", 0b11111110000000000000111110000000U } } ),
                new RiscvInstructionMask( "BGEU",                                      0b00000000000000000111000001100011U, 0b00000000000000000111000001111111U,  "B",     new Dictionary<string, uint> { { "rs1", 0b00000000000011111000000000000000U }, { "rs2", 0b00000001111100000000000000000000U }, { "immediate", 0b11111110000000000000111110000000U } } ),
                new RiscvInstructionMask( "LB",                                        0b00000000000000000000000000000011U, 0b00000000000000000111000001111111U,  "I",     new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "rs1", 0b00000000000011111000000000000000U }, { "immediate", 0b11111111111100000000000000000000U } } ),
                new RiscvInstructionMask( "LH",                                        0b00000000000000000001000000000011U, 0b00000000000000000111000001111111U,  "I",     new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "rs1", 0b00000000000011111000000000000000U }, { "immediate", 0b11111111111100000000000000000000U } } ),
                new RiscvInstructionMask( "LW",                                        0b00000000000000000010000000000011U, 0b00000000000000000111000001111111U,  "I",     new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "rs1", 0b00000000000011111000000000000000U }, { "immediate", 0b11111111111100000000000000000000U } } ),
                new RiscvInstructionMask( "LBU",                                       0b00000000000000000100000000000011U, 0b00000000000000000111000001111111U,  "I",     new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "rs1", 0b00000000000011111000000000000000U }, { "immediate", 0b11111111111100000000000000000000U } } ),
                new RiscvInstructionMask( "LHU",                                       0b00000000000000000101000000000011U, 0b00000000000000000111000001111111U,  "I",     new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "rs1", 0b00000000000011111000000000000000U }, { "immediate", 0b11111111111100000000000000000000U } } ),
                new RiscvInstructionMask( "SB",                                        0b00000000000000000000000000100011U, 0b00000000000000000111000001111111U,  "S",     new Dictionary<string, uint> { { "rs1", 0b00000000000011111000000000000000U }, { "rs2", 0b00000001111100000000000000000000U }, { "immediate", 0b11111110000000000000111110000000U } } ),
                new RiscvInstructionMask( "SH",                                        0b00000000000000000001000000100011U, 0b00000000000000000111000001111111U,  "S",     new Dictionary<string, uint> { { "rs1", 0b00000000000011111000000000000000U }, { "rs2", 0b00000001111100000000000000000000U }, { "immediate", 0b11111110000000000000111110000000U } } ),
                new RiscvInstructionMask( "SW",                                        0b00000000000000000010000000100011U, 0b00000000000000000111000001111111U,  "S",     new Dictionary<string, uint> { { "rs1", 0b00000000000011111000000000000000U }, { "rs2", 0b00000001111100000000000000000000U }, { "immediate", 0b11111110000000000000111110000000U } } ),
                new RiscvInstructionMask( "ADDI",                                      0b00000000000000000000000000010011U, 0b00000000000000000111000001111111U,  "I",     new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "rs1", 0b00000000000011111000000000000000U }, { "immediate", 0b11111111111100000000000000000000U } } ),
                new RiscvInstructionMask( "SLTI",                                      0b00000000000000000010000000010011U, 0b00000000000000000111000001111111U,  "I",     new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "rs1", 0b00000000000011111000000000000000U }, { "immediate", 0b11111111111100000000000000000000U } } ),
                new RiscvInstructionMask( "SLTIU",                                     0b00000000000000000011000000010011U, 0b00000000000000000111000001111111U,  "I",     new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "rs1", 0b00000000000011111000000000000000U }, { "immediate", 0b11111111111100000000000000000000U } } ),
                new RiscvInstructionMask( "XORI",                                      0b00000000000000000100000000010011U, 0b00000000000000000111000001111111U,  "I",     new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "rs1", 0b00000000000011111000000000000000U }, { "immediate", 0b11111111111100000000000000000000U } } ),
                new RiscvInstructionMask( "ORI",                                       0b00000000000000000110000000010011U, 0b00000000000000000111000001111111U,  "I",     new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "rs1", 0b00000000000011111000000000000000U }, { "immediate", 0b11111111111100000000000000000000U } } ),
                new RiscvInstructionMask( "ANDI",                                      0b00000000000000000111000000010011U, 0b00000000000000000111000001111111U,  "I",     new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "rs1", 0b00000000000011111000000000000000U }, { "immediate", 0b11111111111100000000000000000000U } } ),
                new RiscvInstructionMask( "SLLI",                                      0b00000000000000000001000000010011U, 0b11111100000000000111000001111111U,  "SHIFT", new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "shamt", 0b00000000000111111000000000000000U }, { "immediate", 0b00000001111100000000000000000000U } } ),
                new RiscvInstructionMask( "SRLI",                                      0b00000000000000000101000000010011U, 0b11111100000000000111000001111111U,  "SHIFT", new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "shamt", 0b00000000000111111000000000000000U }, { "immediate", 0b00000001111100000000000000000000U } } ),
                new RiscvInstructionMask( "SRAI",                                      0b01000000000000000101000000010011U, 0b11111100000000000111000001111111U,  "SHIFT", new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "shamt", 0b00000000000111111000000000000000U }, { "immediate", 0b00000001111100000000000000000000U } } ),
                new RiscvInstructionMask( "ADD",                                       0b00000000000000000000000000110011U, 0b11111110000000000111000001111111U,  "R",     new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "rs1", 0b00000000000011111000000000000000U }, { "rs2", 0b00000001111100000000000000000000U } } ),
                new RiscvInstructionMask( "SUB",                                       0b01000000000000000000000000110011U, 0b11111110000000000111000001111111U,  "R",     new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "rs1", 0b00000000000011111000000000000000U }, { "rs2", 0b00000001111100000000000000000000U } } ),
                new RiscvInstructionMask( "SLL",                                       0b00000000000000000001000000110011U, 0b11111110000000000111000001111111U,  "R",     new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "rs1", 0b00000000000011111000000000000000U }, { "rs2", 0b00000001111100000000000000000000U } } ),
                new RiscvInstructionMask( "SLT",                                       0b00000000000000000010000000110011U, 0b11111110000000000111000001111111U,  "R",     new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "rs1", 0b00000000000011111000000000000000U }, { "rs2", 0b00000001111100000000000000000000U } } ),
                new RiscvInstructionMask( "SLTU",                                      0b00000000000000000011000000110011U, 0b11111110000000000111000001111111U,  "R",     new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "rs1", 0b00000000000011111000000000000000U }, { "rs2", 0b00000001111100000000000000000000U } } ),
                new RiscvInstructionMask( "XOR",                                       0b00000000000000000100000000110011U, 0b11111110000000000111000001111111U,  "R",     new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "rs1", 0b00000000000011111000000000000000U }, { "rs2", 0b00000001111100000000000000000000U } } ),
                new RiscvInstructionMask( "SRL",                                       0b00000000000000000101000000110011U, 0b11111110000000000111000001111111U,  "R",     new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "rs1", 0b00000000000011111000000000000000U }, { "rs2", 0b00000001111100000000000000000000U } } ),
                new RiscvInstructionMask( "SRA",                                       0b01000000000000000101000000110011U, 0b11111110000000000111000001111111U,  "R",     new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "rs1", 0b00000000000011111000000000000000U }, { "rs2", 0b00000001111100000000000000000000U } } ),
                new RiscvInstructionMask( "OR",                                        0b00000000000000000110000000110011U, 0b11111110000000000111000001111111U,  "R",     new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "rs1", 0b00000000000011111000000000000000U }, { "rs2", 0b00000001111100000000000000000000U } } ),
                new RiscvInstructionMask( "AND",                                       0b00000000000000000111000000110011U, 0b11111110000000000111000001111111U,  "R",     new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "rs1", 0b00000000000011111000000000000000U }, { "rs2", 0b00000001111100000000000000000000U } } ),
                new RiscvInstructionMask( "FENCE",                                     0b00000000000000000000000000001111U, 0b11110000000011111111111111111111U,  "FENCE", new Dictionary<string, uint> { { "pred/succ", 0b00001111111100000000000000000000U } } ),
                new RiscvInstructionMask( "FENCE.I",                                   0b00000000000000000001000000001111U, 0b11111111111111111111111111111111U,  "NONE",  new Dictionary<string, uint> { { "immediate", 0b00000000000000000000000000000000U } } ),
                new RiscvInstructionMask( "ECALL",                                     0b00000000000000000000000001110011U, 0b11111111111111111111111111111111U,  "NONE",  new Dictionary<string, uint> { { "immediate", 0b00000000000000000000000000000000U } } ),
                new RiscvInstructionMask( "EBREAK",                                    0b00000000000100000000000001110011U, 0b11111111111111111111111111111111U,  "NONE",  new Dictionary<string, uint> { { "immediate", 0b00000000000000000000000000000000U } } ),
                new RiscvInstructionMask( "CSRRW",                                     0b00000000000000000001000001110011U, 0b00000000000000000111000001111111U,  "CSR",   new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "rs1", 0b00000000000011111000000000000000U }, { "csr", 0b11111111111100000000000000000000U } } ),
                new RiscvInstructionMask( "CSRRS",                                     0b00000000000000000010000001110011U, 0b00000000000000000111000001111111U,  "CSR",   new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "rs1", 0b00000000000011111000000000000000U }, { "csr", 0b11111111111100000000000000000000U } } ),
                new RiscvInstructionMask( "CSRRC",                                     0b00000000000000000011000001110011U, 0b00000000000000000111000001111111U,  "CSR",   new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "rs1", 0b00000000000011111000000000000000U }, { "csr", 0b11111111111100000000000000000000U } } ),
                new RiscvInstructionMask( "CSRRWI",                                    0b00000000000000000101000001110011U, 0b00000000000000000111000001111111U,  "CSR",   new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "csr", 0b11111111111100000000000000000000U }, { "immediate", 0b00000000000011111000000000000000U } } ),
                new RiscvInstructionMask( "CSRRSI",                                    0b00000000000000000110000001110011U, 0b00000000000000000111000001111111U,  "CSR",   new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "csr", 0b11111111111100000000000000000000U }, { "immediate", 0b00000000000011111000000000000000U } } ),
                new RiscvInstructionMask( "CSRRCI",                                    0b00000000000000000111000001110011U, 0b00000000000000000111000001111111U,  "CSR",   new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "csr", 0b11111111111100000000000000000000U }, { "immediate", 0b00000000000011111000000000000000U } } ),
                new RiscvInstructionMask( "URET",                                      0b00000000001000000000000001110011U, 0b11111111111111111111111111111111U,  "NONE",  new Dictionary<string, uint> { } ),
                new RiscvInstructionMask( "SRET",                                      0b00010000001000000000000001110011U, 0b11111111111111111111111111111111U,  "NONE",  new Dictionary<string, uint> { } ),
                new RiscvInstructionMask( "MRET",                                      0b00110000001000000000000001110011U, 0b11111111111111111111111111111111U,  "NONE",  new Dictionary<string, uint> { } ),
                new RiscvInstructionMask( "WFI",                                       0b00010000010100000000000001110011U, 0b11111111111111111111111111111111U,  "NONE",  new Dictionary<string, uint> { } ),
                new RiscvInstructionMask( "SFENCE.VMA",                                0b00010010000000000000000001110011U, 0b11111110000000000111111111111111U,  "R",     new Dictionary<string, uint> { { "rs1", 0b00000000000011111000000000000000U }, { "rs2", 0b00000000000000000000000001111100U } } ),
                new RiscvInstructionMask( "LWU",                                       0b00000000000000000110000000000011U, 0b00000000000000000111000001111111U,  "U",     new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "rs1", 0b00000000000011111000000000000000U }, { "immediate", 0b11111111111100000000000000000000U } } ),
                new RiscvInstructionMask( "LD",                                        0b00000000000000000011000000000011U, 0b00000000000000000111000001111111U,  "U",     new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "rs1", 0b00000000000011111000000000000000U }, { "immediate", 0b11111111111100000000000000000000U } } ),
                new RiscvInstructionMask( "SD",                                        0b00000000000000000011000000100011U, 0b00000000000000000111000001111111U,  "S",     new Dictionary<string, uint> { { "rs1", 0b00000000000011111000000000000000U }, { "rs2", 0b00000001111100000000000000000000U }, { "immediate", 0b11111110000000000000111110000000U } } ),
                new RiscvInstructionMask( "ADDIW",                                     0b00000000000000000000000000011011U, 0b00000000000000000111000001111111U,  "I",     new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "rs1", 0b00000000000011111000000000000000U }, { "immediate", 0b11111111111100000000000000000000U } } ),
                new RiscvInstructionMask( "SLLIW",                                     0b00000000000000000001000000011011U, 0b11111110000000000111000001111111U,  "SHIFT", new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "shamt", 0b00000000000011111000000000000000U }, { "immediate", 0b00000001111100000000000000000000U } } ),
                new RiscvInstructionMask( "SRLIW",                                     0b00000000000000000101000000011011U, 0b11111110000000000111000001111111U,  "SHIFT", new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "shamt", 0b00000000000011111000000000000000U }, { "immediate", 0b00000001111100000000000000000000U } } ),
                new RiscvInstructionMask( "SRAIW",                                     0b01000000000000000101000000011011U, 0b11111110000000000111000001111111U,  "SHIFT", new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "shamt", 0b00000000000011111000000000000000U }, { "immediate", 0b00000001111100000000000000000000U } } ),
                new RiscvInstructionMask( "ADDW",                                      0b00000000000000000000000000111011U, 0b11111110000000000111000001111111U,  "R",     new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "rs1", 0b00000000000011111000000000000000U }, { "rs2", 0b00000001111100000000000000000000U } } ),
                new RiscvInstructionMask( "SUBW",                                      0b01000000000000000000000000111011U, 0b11111110000000000111000001111111U,  "R",     new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "rs1", 0b00000000000011111000000000000000U }, { "rs2", 0b00000001111100000000000000000000U } } ),
                new RiscvInstructionMask( "SLLW",                                      0b00000000000000000001000000111011U, 0b11111110000000000111000001111111U,  "R",     new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "rs1", 0b00000000000011111000000000000000U }, { "rs2", 0b00000001111100000000000000000000U } } ),
                new RiscvInstructionMask( "SRLW",                                      0b00000000000000000101000000111011U, 0b11111110000000000111000001111111U,  "R",     new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "rs1", 0b00000000000011111000000000000000U }, { "rs2", 0b00000001111100000000000000000000U } } ),
                new RiscvInstructionMask( "SRAW",                                      0b01000000000000000101000000111011U, 0b11111110000000000111000001111111U,  "R",     new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "rs1", 0b00000000000011111000000000000000U }, { "rs2", 0b00000001111100000000000000000000U } } ),
                new RiscvInstructionMask( "MUL",                                       0b00000010000000000000000000110011U, 0b11111110000000000111000001111111U,  "R",     new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "rs1", 0b00000000000011111000000000000000U }, { "rs2", 0b00000001111100000000000000000000U } } ),
                new RiscvInstructionMask( "MULH",                                      0b00000010000000000001000000110011U, 0b11111110000000000111000001111111U,  "R",     new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "rs1", 0b00000000000011111000000000000000U }, { "rs2", 0b00000001111100000000000000000000U } } ),
                new RiscvInstructionMask( "MULHSU",                                    0b00000010000000000010000000110011U, 0b11111110000000000111000001111111U,  "R",     new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "rs1", 0b00000000000011111000000000000000U }, { "rs2", 0b00000001111100000000000000000000U } } ),
                new RiscvInstructionMask( "MULHU",                                     0b00000010000000000011000000110011U, 0b11111110000000000111000001111111U,  "R",     new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "rs1", 0b00000000000011111000000000000000U }, { "rs2", 0b00000001111100000000000000000000U } } ),
                new RiscvInstructionMask( "DIV",                                       0b00000010000000000100000000110011U, 0b11111110000000000111000001111111U,  "R",     new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "rs1", 0b00000000000011111000000000000000U }, { "rs2", 0b00000001111100000000000000000000U } } ),
                new RiscvInstructionMask( "DIVU",                                      0b00000010000000000101000000110011U, 0b11111110000000000111000001111111U,  "R",     new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "rs1", 0b00000000000011111000000000000000U }, { "rs2", 0b00000001111100000000000000000000U } } ),
                new RiscvInstructionMask( "REM",                                       0b00000010000000000110000000110011U, 0b11111110000000000111000001111111U,  "R",     new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "rs1", 0b00000000000011111000000000000000U }, { "rs2", 0b00000001111100000000000000000000U } } ),
                new RiscvInstructionMask( "REMU",                                      0b00000010000000000111000000110011U, 0b11111110000000000111000001111111U,  "R",     new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "rs1", 0b00000000000011111000000000000000U }, { "rs2", 0b00000001111100000000000000000000U } } ),
                new RiscvInstructionMask( "MULW",                                      0b00000010000000000000000000111011U, 0b11111110000000000111000001111111U,  "R",     new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "rs1", 0b00000000000011111000000000000000U }, { "rs2", 0b00000001111100000000000000000000U } } ),
                new RiscvInstructionMask( "DIVW",                                      0b00000010000000000100000000111011U, 0b11111110000000000111000001111111U,  "R",     new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "rs1", 0b00000000000011111000000000000000U }, { "rs2", 0b00000001111100000000000000000000U } } ),
                new RiscvInstructionMask( "DIVUW",                                     0b00000010000000000101000000111011U, 0b11111110000000000111000001111111U,  "R",     new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "rs1", 0b00000000000011111000000000000000U }, { "rs2", 0b00000001111100000000000000000000U } } ),
                new RiscvInstructionMask( "REMW",                                      0b00000010000000000110000000111011U, 0b11111110000000000111000001111111U,  "R",     new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "rs1", 0b00000000000011111000000000000000U }, { "rs2", 0b00000001111100000000000000000000U } } ),
                new RiscvInstructionMask( "REMUW",                                     0b00000010000000000111000000111011U, 0b11111110000000000111000001111111U,  "R",     new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "rs1", 0b00000000000011111000000000000000U }, { "rs2", 0b00000001111100000000000000000000U } } ),
                new RiscvInstructionMask( "LR.W",                                      0b00010000000000000010000000101111U, 0b11111001111100000111000001111111U,  "A",     new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "rs1", 0b00000000000011111000000000000000U }, { "aquire/release", 0b00000110000000000000000000000000U } } ),
                new RiscvInstructionMask( "SC.W",                                      0b00011000000000000010000000101111U, 0b11111000000000000111000001111111U,  "A",     new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "rs1", 0b00000000000011111000000000000000U }, { "rs2", 0b00000001111100000000000000000000U }, { "aquire/release", 0b00000110000000000000000000000000U } } ),
                new RiscvInstructionMask( "AMOSWAP.W",                                 0b00001000000000000010000000101111U, 0b11111000000000000111000001111111U,  "A",     new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "rs1", 0b00000000000011111000000000000000U }, { "rs2", 0b00000001111100000000000000000000U }, { "aquire/release", 0b00000110000000000000000000000000U } } ),
                new RiscvInstructionMask( "AMOADD.W",                                  0b00000000000000000010000000101111U, 0b11111000000000000111000001111111U,  "A",     new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "rs1", 0b00000000000011111000000000000000U }, { "rs2", 0b00000001111100000000000000000000U }, { "aquire/release", 0b00000110000000000000000000000000U } } ),
                new RiscvInstructionMask( "AMOXOR.W",                                  0b00100000000000000010000000101111U, 0b11111000000000000111000001111111U,  "A",     new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "rs1", 0b00000000000011111000000000000000U }, { "rs2", 0b00000001111100000000000000000000U }, { "aquire/release", 0b00000110000000000000000000000000U } } ),
                new RiscvInstructionMask( "AMOAND.W",                                  0b01100000000000000010000000101111U, 0b11111000000000000111000001111111U,  "A",     new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "rs1", 0b00000000000011111000000000000000U }, { "rs2", 0b00000001111100000000000000000000U }, { "aquire/release", 0b00000110000000000000000000000000U } } ),
                new RiscvInstructionMask( "AMOOR.W",                                   0b01000000000000000010000000101111U, 0b11111000000000000111000001111111U,  "A",     new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "rs1", 0b00000000000011111000000000000000U }, { "rs2", 0b00000001111100000000000000000000U }, { "aquire/release", 0b00000110000000000000000000000000U } } ),
                new RiscvInstructionMask( "AMOMIN.W",                                  0b10000000000000000010000000101111U, 0b11111000000000000111000001111111U,  "A",     new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "rs1", 0b00000000000011111000000000000000U }, { "rs2", 0b00000001111100000000000000000000U }, { "aquire/release", 0b00000110000000000000000000000000U } } ),
                new RiscvInstructionMask( "AMOMAX.W",                                  0b10100000000000000010000000101111U, 0b11111000000000000111000001111111U,  "A",     new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "rs1", 0b00000000000011111000000000000000U }, { "rs2", 0b00000001111100000000000000000000U }, { "aquire/release", 0b00000110000000000000000000000000U } } ),
                new RiscvInstructionMask( "AMOMINU.W",                                 0b11000000000000000010000000101111U, 0b11111000000000000111000001111111U,  "A",     new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "rs1", 0b00000000000011111000000000000000U }, { "rs2", 0b00000001111100000000000000000000U }, { "aquire/release", 0b00000110000000000000000000000000U } } ),
                new RiscvInstructionMask( "AMOMAXU.W",                                 0b11100000000000000010000000101111U, 0b11111000000000000111000001111111U,  "A",     new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "rs1", 0b00000000000011111000000000000000U }, { "rs2", 0b00000001111100000000000000000000U }, { "aquire/release", 0b00000110000000000000000000000000U } } ),
                new RiscvInstructionMask( "LR.D",                                      0b00010000000000000011000000101111U, 0b11111001111100000111000001111111U,  "A",     new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "rs1", 0b00000000000011111000000000000000U }, { "aquire/release", 0b00000110000000000000000000000000U } } ),
                new RiscvInstructionMask( "SC.D",                                      0b00011000000000000011000000101111U, 0b11111000000000000111000001111111U,  "A",     new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "rs1", 0b00000000000011111000000000000000U }, { "rs2", 0b00000001111100000000000000000000U }, { "aquire/release", 0b00000110000000000000000000000000U } } ),
                new RiscvInstructionMask( "AMOSWAP.D",                                 0b00001000000000000011000000101111U, 0b11111000000000000111000001111111U,  "A",     new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "rs1", 0b00000000000011111000000000000000U }, { "rs2", 0b00000001111100000000000000000000U }, { "aquire/release", 0b00000110000000000000000000000000U } } ),
                new RiscvInstructionMask( "AMOADD.D",                                  0b00000000000000000011000000101111U, 0b11111000000000000111000001111111U,  "A",     new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "rs1", 0b00000000000011111000000000000000U }, { "rs2", 0b00000001111100000000000000000000U }, { "aquire/release", 0b00000110000000000000000000000000U } } ),
                new RiscvInstructionMask( "AMOXOR.D",                                  0b00100000000000000011000000101111U, 0b11111000000000000111000001111111U,  "A",     new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "rs1", 0b00000000000011111000000000000000U }, { "rs2", 0b00000001111100000000000000000000U }, { "aquire/release", 0b00000110000000000000000000000000U } } ),
                new RiscvInstructionMask( "AMOAND.D",                                  0b01100000000000000011000000101111U, 0b11111000000000000111000001111111U,  "A",     new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "rs1", 0b00000000000011111000000000000000U }, { "rs2", 0b00000001111100000000000000000000U }, { "aquire/release", 0b00000110000000000000000000000000U } } ),
                new RiscvInstructionMask( "AMOOR.D",                                   0b01000000000000000011000000101111U, 0b11111000000000000111000001111111U,  "A",     new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "rs1", 0b00000000000011111000000000000000U }, { "rs2", 0b00000001111100000000000000000000U }, { "aquire/release", 0b00000110000000000000000000000000U } } ),
                new RiscvInstructionMask( "AMOMIN.D",                                  0b10000000000000000011000000101111U, 0b11111000000000000111000001111111U,  "A",     new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "rs1", 0b00000000000011111000000000000000U }, { "rs2", 0b00000001111100000000000000000000U }, { "aquire/release", 0b00000110000000000000000000000000U } } ),
                new RiscvInstructionMask( "AMOMAX.D",                                  0b10100000000000000011000000101111U, 0b11111000000000000111000001111111U,  "A",     new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "rs1", 0b00000000000011111000000000000000U }, { "rs2", 0b00000001111100000000000000000000U }, { "aquire/release", 0b00000110000000000000000000000000U } } ),
                new RiscvInstructionMask( "AMOMINU.D",                                 0b11000000000000000011000000101111U, 0b11111000000000000111000001111111U,  "A",     new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "rs1", 0b00000000000011111000000000000000U }, { "rs2", 0b00000001111100000000000000000000U }, { "aquire/release", 0b00000110000000000000000000000000U } } ),
                new RiscvInstructionMask( "AMOMAXU.D",                                 0b11100000000000000011000000101111U, 0b11111000000000000111000001111111U,  "A",     new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "rs1", 0b00000000000011111000000000000000U }, { "rs2", 0b00000001111100000000000000000000U }, { "aquire/release", 0b00000110000000000000000000000000U } } ),
                new RiscvInstructionMask( "FLW",                                       0b00000000000000000010000000000111U, 0b00000000000000000111000001111111U,  "I",     new Dictionary<string, uint> { { "frd", 0b00000000000000000000111110000000U }, { "rs1", 0b00000000000011111000000000000000U }, { "immediate", 0b11111111111100000000000000000000U } } ),
                new RiscvInstructionMask( "FSW",                                       0b00000000000000000010000000100111U, 0b00000000000000000111000001111111U,  "S",     new Dictionary<string, uint> { { "rs1", 0b00000000000011111000000000000000U }, { "frs2", 0b00000001111100000000000000000000U }, { "immediate", 0b11111110000000000000111110000000U } } ),
                new RiscvInstructionMask( "FMADD.S",                                   0b00000000000000000000000001000011U, 0b00000110000000000000000001111111U,  "R4",    new Dictionary<string, uint> { { "frd", 0b00000000000000000000111110000000U }, { "frs1", 0b00000000000011111000000000000000U }, { "frs2", 0b00000001111100000000000000000000U }, { "frs3", 0b11111000000000000000000000000000U }, { "rm", 0b00000000000000000111000000000000U } } ),
                new RiscvInstructionMask( "FMSUB.S",                                   0b00000000000000000000000001000111U, 0b00000110000000000000000001111111U,  "R4",    new Dictionary<string, uint> { { "frd", 0b00000000000000000000111110000000U }, { "frs1", 0b00000000000011111000000000000000U }, { "frs2", 0b00000001111100000000000000000000U }, { "frs3", 0b11111000000000000000000000000000U }, { "rm", 0b00000000000000000111000000000000U } } ),
                new RiscvInstructionMask( "FNMSUB.S",                                  0b00000000000000000000000001001011U, 0b00000110000000000000000001111111U,  "R4",    new Dictionary<string, uint> { { "frd", 0b00000000000000000000111110000000U }, { "frs1", 0b00000000000011111000000000000000U }, { "frs2", 0b00000001111100000000000000000000U }, { "frs3", 0b11111000000000000000000000000000U }, { "rm", 0b00000000000000000111000000000000U } } ),
                new RiscvInstructionMask( "FNMADD.S",                                  0b00000000000000000000000001001111U, 0b00000110000000000000000001111111U,  "R4",    new Dictionary<string, uint> { { "frd", 0b00000000000000000000111110000000U }, { "frs1", 0b00000000000011111000000000000000U }, { "frs2", 0b00000001111100000000000000000000U }, { "frs3", 0b11111000000000000000000000000000U }, { "rm", 0b00000000000000000111000000000000U } } ),
                new RiscvInstructionMask( "FADD.S",                                    0b00000000000000000000000001010011U, 0b11111110000000000000000001111111U,  "R",     new Dictionary<string, uint> { { "frd", 0b00000000000000000000111110000000U }, { "frs1", 0b00000000000011111000000000000000U }, { "frs2", 0b00000001111100000000000000000000U }, { "rm", 0b00000000000000000111000000000000U } } ),
                new RiscvInstructionMask( "FSUB.S",                                    0b00001000000000000000000001010011U, 0b11111110000000000000000001111111U,  "R",     new Dictionary<string, uint> { { "frd", 0b00000000000000000000111110000000U }, { "frs1", 0b00000000000011111000000000000000U }, { "frs2", 0b00000001111100000000000000000000U }, { "rm", 0b00000000000000000111000000000000U } } ),
                new RiscvInstructionMask( "FMUL.S",                                    0b00010000000000000000000001010011U, 0b11111110000000000000000001111111U,  "R",     new Dictionary<string, uint> { { "frd", 0b00000000000000000000111110000000U }, { "frs1", 0b00000000000011111000000000000000U }, { "frs2", 0b00000001111100000000000000000000U }, { "rm", 0b00000000000000000111000000000000U } } ),
                new RiscvInstructionMask( "FDIV.S",                                    0b00011000000000000000000001010011U, 0b11111110000000000000000001111111U,  "R",     new Dictionary<string, uint> { { "frd", 0b00000000000000000000111110000000U }, { "frs1", 0b00000000000011111000000000000000U }, { "frs2", 0b00000001111100000000000000000000U }, { "rm", 0b00000000000000000111000000000000U } } ),
                new RiscvInstructionMask( "FSQRT.S",                                   0b01011000000000000000000001010011U, 0b11111111111100000000000001111111U,  "RF",    new Dictionary<string, uint> { { "frd", 0b00000000000000000000111110000000U }, { "frs1", 0b00000000000011111000000000000000U }, { "rm", 0b00000000000000000111000000000000U } } ),
                new RiscvInstructionMask( "FSGNJ.S",                                   0b00100000000000000000000001010011U, 0b11111110000000000111000001111111U,  "R",     new Dictionary<string, uint> { { "frd", 0b00000000000000000000111110000000U }, { "frs1", 0b00000000000011111000000000000000U }, { "frs2", 0b00000001111100000000000000000000U } } ),
                new RiscvInstructionMask( "FSGNJN.S",                                  0b00100000000000000001000001010011U, 0b11111110000000000111000001111111U,  "R",     new Dictionary<string, uint> { { "frd", 0b00000000000000000000111110000000U }, { "frs1", 0b00000000000011111000000000000000U }, { "frs2", 0b00000001111100000000000000000000U } } ),
                new RiscvInstructionMask( "FSGNJX.S",                                  0b00100000000000000010000001010011U, 0b11111110000000000111000001111111U,  "R",     new Dictionary<string, uint> { { "frd", 0b00000000000000000000111110000000U }, { "frs1", 0b00000000000011111000000000000000U }, { "frs2", 0b00000001111100000000000000000000U } } ),
                new RiscvInstructionMask( "FMIN.S",                                    0b00101000000000000000000001010011U, 0b11111110000000000111000001111111U,  "R",     new Dictionary<string, uint> { { "frd", 0b00000000000000000000111110000000U }, { "frs1", 0b00000000000011111000000000000000U }, { "frs2", 0b00000001111100000000000000000000U } } ),
                new RiscvInstructionMask( "FMAX.S",                                    0b00101000000000000001000001010011U, 0b11111110000000000111000001111111U,  "R",     new Dictionary<string, uint> { { "frd", 0b00000000000000000000111110000000U }, { "frs1", 0b00000000000011111000000000000000U }, { "frs2", 0b00000001111100000000000000000000U } } ),
                new RiscvInstructionMask( "FCVT.W.S",                                  0b11000000000000000000000001010011U, 0b11111111111100000000000001111111U,  "RF",    new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "frs1", 0b00000000000011111000000000000000U }, { "rm", 0b00000000000000000111000000000000U } } ),
                new RiscvInstructionMask( "FCVT.WU.S",                                 0b11000000000100000000000001010011U, 0b11111111111100000000000001111111U,  "RF",    new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "frs1", 0b00000000000011111000000000000000U }, { "rm", 0b00000000000000000111000000000000U } } ),
                new RiscvInstructionMask( "FMV.X.W",                                   0b11100000000000000000000001010011U, 0b11111111111100000111000001111111U,  "FMV",   new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "frs1", 0b00000000000011111000000000000000U } } ),
                new RiscvInstructionMask( "FEQ.S",                                     0b10100000000000000010000001010011U, 0b11111110000000000111000001111111U,  "R",     new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "frs1", 0b00000000000011111000000000000000U }, { "frs2", 0b00000001111100000000000000000000U } } ),
                new RiscvInstructionMask( "FLT.S",                                     0b10100000000000000001000001010011U, 0b11111110000000000111000001111111U,  "R",     new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "frs1", 0b00000000000011111000000000000000U }, { "frs2", 0b00000001111100000000000000000000U } } ),
                new RiscvInstructionMask( "FLE.S",                                     0b10100000000000000000000001010011U, 0b11111110000000000111000001111111U,  "R",     new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "frs1", 0b00000000000011111000000000000000U }, { "frs2", 0b00000001111100000000000000000000U } } ),
                new RiscvInstructionMask( "FCLASS.S",                                  0b11100000000000000001000001010011U, 0b11111111111100000111000001111111U,  "FMV",   new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "frs1", 0b00000000000011111000000000000000U } } ),
                new RiscvInstructionMask( "FCVT.S.W",                                  0b11010000000000000000000001010011U, 0b11111111111100000000000001111111U,  "RF",    new Dictionary<string, uint> { { "frd", 0b00000000000000000000111110000000U }, { "rs1", 0b00000000000011111000000000000000U }, { "rm", 0b00000000000000000111000000000000U } } ),
                new RiscvInstructionMask( "FCVT.S.WU",                                 0b11010000000100000000000001010011U, 0b11111111111100000000000001111111U,  "RF",    new Dictionary<string, uint> { { "frd", 0b00000000000000000000111110000000U }, { "rs1", 0b00000000000011111000000000000000U }, { "rm", 0b00000000000000000111000000000000U } } ),
                new RiscvInstructionMask( "FMV.W.X",                                   0b11110000000000000000000001010011U, 0b11111111111100000111000001111111U,  "FMV",   new Dictionary<string, uint> { { "frd", 0b00000000000000000000111110000000U }, { "rs1", 0b00000000000011111000000000000000U } } ),
                new RiscvInstructionMask( "FCVT.L.S",                                  0b11000000001000000000000001010011U, 0b11111111111100000000000001111111U,  "RF",    new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "frs1", 0b00000000000011111000000000000000U }, { "rm", 0b00000000000000000111000000000000U } } ),
                new RiscvInstructionMask( "FCVT.LU.S",                                 0b11000000001100000000000001010011U, 0b11111111111100000000000001111111U,  "RF",    new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "frs1", 0b00000000000011111000000000000000U }, { "rm", 0b00000000000000000111000000000000U } } ),
                new RiscvInstructionMask( "FCVT.S.L",                                  0b11010000001000000000000001010011U, 0b11111111111100000000000001111111U,  "RF",    new Dictionary<string, uint> { { "frd", 0b00000000000000000000111110000000U }, { "rs1", 0b00000000000011111000000000000000U }, { "rm", 0b00000000000000000111000000000000U } } ),
                new RiscvInstructionMask( "FCVT.S.LU",                                 0b11010000001100000000000001010011U, 0b11111111111100000000000001111111U,  "RF",    new Dictionary<string, uint> { { "frd", 0b00000000000000000000111110000000U }, { "rs1", 0b00000000000011111000000000000000U }, { "rm", 0b00000000000000000111000000000000U } } ),
                new RiscvInstructionMask( "FLD",                                       0b00000000000000000011000000000111U, 0b00000000000000000111000001111111U,  "I",     new Dictionary<string, uint> { { "frd", 0b00000000000000000000111110000000U }, { "rs1", 0b00000000000011111000000000000000U }, { "immediate", 0b11111111111100000000000000000000U } } ),
                new RiscvInstructionMask( "FSD",                                       0b00000000000000000011000000100111U, 0b00000000000000000111000001111111U,  "S",     new Dictionary<string, uint> { { "rs1", 0b00000000000011111000000000000000U }, { "frs2", 0b00000001111100000000000000000000U }, { "immediate", 0b11111110000000000000111110000000U } } ),
                new RiscvInstructionMask( "FMADD.D",                                   0b00000010000000000000000001000011U, 0b00000110000000000000000001111111U,  "R4",    new Dictionary<string, uint> { { "frd", 0b00000000000000000000111110000000U }, { "frs1", 0b00000000000011111000000000000000U }, { "frs2", 0b00000001111100000000000000000000U }, { "frs3", 0b11111000000000000000000000000000U }, { "rm", 0b00000000000000000111000000000000U } } ),
                new RiscvInstructionMask( "FMSUB.D",                                   0b00000010000000000000000001000111U, 0b00000110000000000000000001111111U,  "R4",    new Dictionary<string, uint> { { "frd", 0b00000000000000000000111110000000U }, { "frs1", 0b00000000000011111000000000000000U }, { "frs2", 0b00000001111100000000000000000000U }, { "frs3", 0b11111000000000000000000000000000U }, { "rm", 0b00000000000000000111000000000000U } } ),
                new RiscvInstructionMask( "FNMSUB.D",                                  0b00000010000000000000000001001011U, 0b00000110000000000000000001111111U,  "R4",    new Dictionary<string, uint> { { "frd", 0b00000000000000000000111110000000U }, { "frs1", 0b00000000000011111000000000000000U }, { "frs2", 0b00000001111100000000000000000000U }, { "frs3", 0b11111000000000000000000000000000U }, { "rm", 0b00000000000000000111000000000000U } } ),
                new RiscvInstructionMask( "FNMADD.D",                                  0b00000010000000000000000001001111U, 0b00000110000000000000000001111111U,  "R4",    new Dictionary<string, uint> { { "frd", 0b00000000000000000000111110000000U }, { "frs1", 0b00000000000011111000000000000000U }, { "frs2", 0b00000001111100000000000000000000U }, { "frs3", 0b11111000000000000000000000000000U }, { "rm", 0b00000000000000000111000000000000U } } ),
                new RiscvInstructionMask( "FADD.D",                                    0b00000010000000000000000001010011U, 0b11111110000000000000000001111111U,  "R",     new Dictionary<string, uint> { { "frd", 0b00000000000000000000111110000000U }, { "frs1", 0b00000000000011111000000000000000U }, { "frs2", 0b00000001111100000000000000000000U }, { "rm", 0b00000000000000000111000000000000U } } ),
                new RiscvInstructionMask( "FSUB.D",                                    0b00001010000000000000000001010011U, 0b11111110000000000000000001111111U,  "R",     new Dictionary<string, uint> { { "frd", 0b00000000000000000000111110000000U }, { "frs1", 0b00000000000011111000000000000000U }, { "frs2", 0b00000001111100000000000000000000U }, { "rm", 0b00000000000000000111000000000000U } } ),
                new RiscvInstructionMask( "FMUL.D",                                    0b00010010000000000000000001010011U, 0b11111110000000000000000001111111U,  "R",     new Dictionary<string, uint> { { "frd", 0b00000000000000000000111110000000U }, { "frs1", 0b00000000000011111000000000000000U }, { "frs2", 0b00000001111100000000000000000000U }, { "rm", 0b00000000000000000111000000000000U } } ),
                new RiscvInstructionMask( "FDIV.D",                                    0b00011010000000000000000001010011U, 0b11111110000000000000000001111111U,  "R",     new Dictionary<string, uint> { { "frd", 0b00000000000000000000111110000000U }, { "frs1", 0b00000000000011111000000000000000U }, { "frs2", 0b00000001111100000000000000000000U }, { "rm", 0b00000000000000000111000000000000U } } ),
                new RiscvInstructionMask( "FSQRT.D",                                   0b01011010000000000000000001010011U, 0b11111111111100000000000001111111U,  "RF",    new Dictionary<string, uint> { { "frd", 0b00000000000000000000111110000000U }, { "frs1", 0b00000000000011111000000000000000U }, { "rm", 0b00000000000000000111000000000000U } } ),
                new RiscvInstructionMask( "FSGNJ.D",                                   0b00100010000000000000000001010011U, 0b11111110000000000111000001111111U,  "R",     new Dictionary<string, uint> { { "frd", 0b00000000000000000000111110000000U }, { "frs1", 0b00000000000011111000000000000000U }, { "frs2", 0b00000001111100000000000000000000U } } ),
                new RiscvInstructionMask( "FSGNJN.D",                                  0b00100010000000000001000001010011U, 0b11111110000000000111000001111111U,  "R",     new Dictionary<string, uint> { { "frd", 0b00000000000000000000111110000000U }, { "frs1", 0b00000000000011111000000000000000U }, { "frs2", 0b00000001111100000000000000000000U } } ),
                new RiscvInstructionMask( "FSGNJX.D",                                  0b00100010000000000010000001010011U, 0b11111110000000000111000001111111U,  "R",     new Dictionary<string, uint> { { "frd", 0b00000000000000000000111110000000U }, { "frs1", 0b00000000000011111000000000000000U }, { "frs2", 0b00000001111100000000000000000000U } } ),
                new RiscvInstructionMask( "FMIN.D",                                    0b00101010000000000000000001010011U, 0b11111110000000000111000001111111U,  "R",     new Dictionary<string, uint> { { "frd", 0b00000000000000000000111110000000U }, { "frs1", 0b00000000000011111000000000000000U }, { "frs2", 0b00000001111100000000000000000000U } } ),
                new RiscvInstructionMask( "FMAX.D",                                    0b00101010000000000001000001010011U, 0b11111110000000000111000001111111U,  "R",     new Dictionary<string, uint> { { "frd", 0b00000000000000000000111110000000U }, { "frs1", 0b00000000000011111000000000000000U }, { "frs2", 0b00000001111100000000000000000000U } } ),
                new RiscvInstructionMask( "FCVT.S.D",                                  0b01000000000100000000000001010011U, 0b11111111111100000000000001111111U,  "RF",    new Dictionary<string, uint> { { "frd", 0b00000000000000000000111110000000U }, { "frs1", 0b00000000000011111000000000000000U }, { "rm", 0b00000000000000000111000000000000U } } ),
                new RiscvInstructionMask( "FCVT.D.S",                                  0b01000010000000000000000001010011U, 0b11111111111100000000000001111111U,  "RF",    new Dictionary<string, uint> { { "frd", 0b00000000000000000000111110000000U }, { "frs1", 0b00000000000011111000000000000000U }, { "rm", 0b00000000000000000111000000000000U } } ),
                new RiscvInstructionMask( "FEQ.D",                                     0b10100010000000000010000001010011U, 0b11111110000000000111000001111111U,  "R",     new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "frs1", 0b00000000000011111000000000000000U }, { "frs2", 0b00000001111100000000000000000000U } } ),
                new RiscvInstructionMask( "FLT.D",                                     0b10100010000000000001000001010011U, 0b11111110000000000111000001111111U,  "R",     new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "frs1", 0b00000000000011111000000000000000U }, { "frs2", 0b00000001111100000000000000000000U } } ),
                new RiscvInstructionMask( "FLE.D",                                     0b10100010000000000000000001010011U, 0b11111110000000000111000001111111U,  "R",     new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "frs1", 0b00000000000011111000000000000000U }, { "frs2", 0b00000001111100000000000000000000U } } ),
                new RiscvInstructionMask( "FCLASS.D",                                  0b11100010000000000001000001010011U, 0b11111111111100000111000001111111U,  "FMV",   new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "frs1", 0b00000000000011111000000000000000U } } ),
                new RiscvInstructionMask( "FCVT.W.D",                                  0b11000010000000000000000001010011U, 0b11111111111100000000000001111111U,  "RF",    new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "frs1", 0b00000000000011111000000000000000U }, { "rm", 0b00000000000000000111000000000000U } } ),
                new RiscvInstructionMask( "FCVT.WU.D",                                 0b11000010000100000000000001010011U, 0b11111111111100000000000001111111U,  "RF",    new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "frs1", 0b00000000000011111000000000000000U }, { "rm", 0b00000000000000000111000000000000U } } ),
                new RiscvInstructionMask( "FCVT.D.W",                                  0b11010010000000000000000001010011U, 0b11111111111100000000000001111111U,  "RF",    new Dictionary<string, uint> { { "frd", 0b00000000000000000000111110000000U }, { "rs1", 0b00000000000011111000000000000000U }, { "rm", 0b00000000000000000111000000000000U } } ),
                new RiscvInstructionMask( "FCVT.D.WU",                                 0b11010010000100000000000001010011U, 0b11111111111100000000000001111111U,  "RF",    new Dictionary<string, uint> { { "frd", 0b00000000000000000000111110000000U }, { "rs1", 0b00000000000011111000000000000000U }, { "rm", 0b00000000000000000111000000000000U } } ),
                new RiscvInstructionMask( "FCVT.L.D",                                  0b11000010001000000000000001010011U, 0b11111111111100000000000001111111U,  "RF",    new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "frs1", 0b00000000000011111000000000000000U }, { "rm", 0b00000000000000000111000000000000U } } ),
                new RiscvInstructionMask( "FCVT.LU.D",                                 0b11000010001100000000000001010011U, 0b11111111111100000000000001111111U,  "RF",    new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "frs1", 0b00000000000011111000000000000000U }, { "rm", 0b00000000000000000111000000000000U } } ),
                new RiscvInstructionMask( "FMV.X.D",                                   0b11100010000000000000000001010011U, 0b11111111111100000111000001111111U,  "FMV",   new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "frs1", 0b00000000000011111000000000000000U } } ),
                new RiscvInstructionMask( "FCVT.D.L",                                  0b11010010001000000000000001010011U, 0b11111111111100000000000001111111U,  "RF",    new Dictionary<string, uint> { { "frd", 0b00000000000000000000111110000000U }, { "rs1", 0b00000000000011111000000000000000U }, { "rm", 0b00000000000000000111000000000000U } } ),
                new RiscvInstructionMask( "FCVT.D.LU",                                 0b11010010001100000000000001010011U, 0b11111111111100000000000001111111U,  "RF",    new Dictionary<string, uint> { { "frd", 0b00000000000000000000111110000000U }, { "rs1", 0b00000000000011111000000000000000U }, { "rm", 0b00000000000000000111000000000000U } } ),
                new RiscvInstructionMask( "FMV.D.X",                                   0b11110010000000000000000001010011U, 0b11111111111100000111000001111111U,  "FMV",   new Dictionary<string, uint> { { "frd", 0b00000000000000000000111110000000U }, { "rs1", 0b00000000000011111000000000000000U } } ),
                new RiscvInstructionMask( "C.ADDI4SPN (RES, nzuimm=0)",                0b00000000000000000010000000000000U, 0b00000000000000001110000000000011U,  "",      new Dictionary<string, uint> { { "rd", 0b00000000000000000000000000011100U }, { "immediate", 0b00000000000000000001111111100000U } } ),
                new RiscvInstructionMask( "C.FLD (RV32/64)",                           0b00000000000000000010000000000000U, 0b00000000000000001110000000000011U,  "",      new Dictionary<string, uint> { { "frd", 0b00000000000000000000000000011100U }, { "rs1", 0b00000000000000000000001110000000U }, { "immediate", 0b00000000000000000001110001100000U } } ),
                new RiscvInstructionMask( "C.LQ (RV128)",                              0b00000000000000000010000000000000U, 0b00000000000000001110000000000011U,  "",      new Dictionary<string, uint> { { "rd", 0b00000000000000000000000000011100U }, { "rs1", 0b00000000000000000000001110000000U }, { "immediate", 0b00000000000000000001110001100000U } } ),
                new RiscvInstructionMask( "C.LW",                                      0b00000000000000000100000000000000U, 0b00000000000000001110000000000011U,  "",      new Dictionary<string, uint> { { "rd", 0b00000000000000000000000000011100U }, { "rs1", 0b00000000000000000000001110000000U }, { "immediate", 0b00000000000000000001110001100000U } } ),
                new RiscvInstructionMask( "C.FLW (RV32)",                              0b00000000000000000110000000000000U, 0b00000000000000001110000000000011U,  "",      new Dictionary<string, uint> { { "frd", 0b00000000000000000000000000011100U }, { "rs1", 0b00000000000000000000001110000000U }, { "immediate", 0b00000000000000000001110001100000U } } ),
                new RiscvInstructionMask( "C.LD (RV64/128)",                           0b00000000000000000110000000000000U, 0b00000000000000001110000000000011U,  "",      new Dictionary<string, uint> { { "rd", 0b00000000000000000000000000011100U }, { "rs1", 0b00000000000000000000001110000000U }, { "immediate", 0b00000000000000000001110001100000U } } ),
                new RiscvInstructionMask( "Reserved",                                  0b00000000000000001000000000000000U, 0b00000000000000001110000000000011U,  "",      new Dictionary<string, uint> { { "immediate", 0b00000000000000000000000000000000U } } ),
                new RiscvInstructionMask( "C.FSD (RV32/64)",                           0b00000000000000001010000000000000U, 0b00000000000000001110000000000011U,  "",      new Dictionary<string, uint> { { "rs1", 0b00000000000000000000001110000000U }, { "frs2", 0b00000000000000000000000000011100U }, { "immediate", 0b00000000000000000001110001100000U } } ),
                new RiscvInstructionMask( "C.SQ (RV128)",                              0b00000000000000001010000000000000U, 0b00000000000000001110000000000011U,  "",      new Dictionary<string, uint> { { "rs1", 0b00000000000000000000001110000000U }, { "rs2", 0b00000000000000000000000000011100U }, { "immediate", 0b00000000000000000001110001100000U } } ),
                new RiscvInstructionMask( "C.SW",                                      0b00000000000000001100000000000000U, 0b00000000000000001110000000000011U,  "",      new Dictionary<string, uint> { { "rs1", 0b00000000000000000000001110000000U }, { "rs2", 0b00000000000000000000000000011100U }, { "immediate", 0b00000000000000000001110001100000U } } ),
                new RiscvInstructionMask( "C.FSW (RV32)",                              0b00000000000000001110000000000000U, 0b00000000000000001110000000000011U,  "",      new Dictionary<string, uint> { { "rs1", 0b00000000000000000000001110000000U }, { "frs2", 0b00000000000000000000000000011100U }, { "immediate", 0b00000000000000000001110001100000U } } ),
                new RiscvInstructionMask( "C.SD (RV64/128)",                           0b00000000000000001110000000000000U, 0b00000000000000001110000000000011U,  "",      new Dictionary<string, uint> { { "rs1", 0b00000000000000000000001110000000U }, { "rs2", 0b00000000000000000000000000011100U }, { "immediate", 0b00000000000000000001110001100000U } } ),
                new RiscvInstructionMask( "C.NOP",                                     0b00000000000000000000000000000001U, 0b00000000000000001111111111111111U,  "",      new Dictionary<string, uint> { { "immediate", 0b00000000000000000000000000000000U } } ),
                new RiscvInstructionMask( "C.ADDI (HINT, nzimm=0)",                    0b00000000000000000000000000000001U, 0b00000000000000001110000000000011U,  "",      new Dictionary<string, uint> { { "rs1", 0b00000000000000000000111110000000U }, { "immediate", 0b00000000000000000001000001111100U } } ),
                new RiscvInstructionMask( "C.JAL (RV32)",                              0b00000000000000000010000000000001U, 0b00000000000000001110000000000011U,  "",      new Dictionary<string, uint> { { "immediate", 0b00000000000000000001111111111100U } } ),
                new RiscvInstructionMask( "C.ADDIW (RV64/128; RES, rd=0)",             0b00000000000000000010000000000001U, 0b00000000000000001110000000000011U,  "",      new Dictionary<string, uint> { { "rs1", 0b00000000000000000000111110000000U }, { "immediate", 0b00000000000000000001000001111100U } } ),
                new RiscvInstructionMask( "C.LI (HINT, rd=0)",                         0b00000000000000000100000000000001U, 0b00000000000000001110000000000011U,  "",      new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "immediate", 0b00000000000000000001000001111100U } } ),
                new RiscvInstructionMask( "C.ADDI16SP (RES, nzimm=0)",                 0b00000000000000000110000100000001U, 0b00000000000000001110111110000011U,  "",      new Dictionary<string, uint> { { "immediate", 0b00000000000000000001000001111100U } } ),
                new RiscvInstructionMask( "C.LUI (RES, nzimm=0; HINT, rd=0)",          0b00000000000000000110000000000001U, 0b00000000000000001110000000000011U,  "",      new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "immediate", 0b00000000000000000001000001111100U } } ),
                new RiscvInstructionMask( "C.SRLI (RV32 NSE, nzuimm[5]=1)",            0b00000000000000001000000000000001U, 0b00000000000000001110110000000011U,  "",      new Dictionary<string, uint> { { "rs1", 0b00000000000000000000001110000000U }, { "immediate", 0b00000000000000000001000001111100U } } ),
                new RiscvInstructionMask( "C.SRLI64 (RV128; RV32/64 HINT)",            0b00000000000000001000000000000001U, 0b00000000000000001111111111100011U,  "",      new Dictionary<string, uint> { { "rs1", 0b00000000000000000000000000011100U } } ),
                new RiscvInstructionMask( "C.SRAI (RV32 NSE, nzuimm[5]=1)",            0b00000000000000001000010000000001U, 0b00000000000000001110110000000011U,  "",      new Dictionary<string, uint> { { "rs1", 0b00000000000000000000001110000000U }, { "immediate", 0b00000000000000000001000001111100U } } ),
                new RiscvInstructionMask( "C.SRAI64 (RV128; RV32/64 HINT)",            0b00000000000000001000000000100001U, 0b00000000000000001111111111100011U,  "",      new Dictionary<string, uint> { { "rs1", 0b00000000000000000000000000011100U } } ),
                new RiscvInstructionMask( "C.ANDI",                                    0b00000000000000001000100000000001U, 0b00000000000000001110110000000011U,  "",      new Dictionary<string, uint> { { "rs1", 0b00000000000000000000001110000000U }, { "immediate", 0b00000000000000000001000001111100U } } ),
                new RiscvInstructionMask( "C.SUB",                                     0b00000000000000001000110000000001U, 0b00000000000000001111110001100011U,  "",      new Dictionary<string, uint> { { "rs1", 0b00000000000000000000001110000000U }, { "rs2", 0b00000000000000000000000000011100U } } ),
                new RiscvInstructionMask( "C.XOR",                                     0b00000000000000001000110000100001U, 0b00000000000000001111110001100011U,  "",      new Dictionary<string, uint> { { "rs1", 0b00000000000000000000001110000000U }, { "rs2", 0b00000000000000000000000000011100U } } ),
                new RiscvInstructionMask( "C.OR",                                      0b00000000000000001000110001000001U, 0b00000000000000001111110001100011U,  "",      new Dictionary<string, uint> { { "rs1", 0b00000000000000000000001110000000U }, { "rs2", 0b00000000000000000000000000011100U } } ),
                new RiscvInstructionMask( "C.AND",                                     0b00000000000000001000110001100001U, 0b00000000000000001111110001100011U,  "",      new Dictionary<string, uint> { { "rs1", 0b00000000000000000000001110000000U }, { "rs2", 0b00000000000000000000000000011100U } } ),
                new RiscvInstructionMask( "C.SUBW (RV64/128; RV32 RES)",               0b00000000000000001001110000000001U, 0b00000000000000001111110001100011U,  "",      new Dictionary<string, uint> { { "rs1", 0b00000000000000000000001110000000U }, { "rs2", 0b00000000000000000000000000011100U } } ),
                new RiscvInstructionMask( "C.ADDW (RV64/128; RV32 RES)",               0b00000000000000001001110000100001U, 0b00000000000000001111110001100011U,  "",      new Dictionary<string, uint> { { "rs1", 0b00000000000000000000001110000000U }, { "rs2", 0b00000000000000000000000000011100U } } ),
                new RiscvInstructionMask( "Reserved",                                  0b00000000000000001001110001000001U, 0b00000000000000001111110001100011U,  "",      new Dictionary<string, uint> { { "immediate", 0b00000000000000000000000000000000U } } ),
                new RiscvInstructionMask( "Reserved",                                  0b00000000000000001001110001100001U, 0b00000000000000001111110001100011U,  "",      new Dictionary<string, uint> { { "immediate", 0b00000000000000000000000000000000U } } ),
                new RiscvInstructionMask( "C.J",                                       0b00000000000000001010000000000001U, 0b00000000000000001110000000000011U,  "",      new Dictionary<string, uint> { { "immediate", 0b00000000000000000001111111111100U } } ),
                new RiscvInstructionMask( "C.BEQZ",                                    0b00000000000000001100000000000001U, 0b00000000000000001110000000000011U,  "",      new Dictionary<string, uint> { { "rs1", 0b00000000000000000000001110000000U }, { "immediate", 0b00000000000000000001110001111100U } } ),
                new RiscvInstructionMask( "C.BNEZ",                                    0b00000000000000001110000000000001U, 0b00000000000000001110000000000011U,  "",      new Dictionary<string, uint> { { "rs1", 0b00000000000000000000001110000000U }, { "immediate", 0b00000000000000000001110001111100U } } ),
                new RiscvInstructionMask( "C.SLLI (HINT, rd=0; RV32 NSE, nzuimm[5]=1)",0b00000000000000000000000000000010u, 0b00000000000000001110000000000011U,  "",      new Dictionary<string, uint> { { "rs1", 0b00000000000000000000111110000000U }, { "immediate", 0b00000000000000000001000001111100U } } ),
                new RiscvInstructionMask( "C.SLLI64 (RV128; RV32/64 HINT; HINT, rd=0)",0b00000000000000000000000000000010u, 0b00000000000000001111000001111111U,  "",      new Dictionary<string, uint> { { "rs1", 0b00000000000000000000111110000000U } } ),
                new RiscvInstructionMask( "C.FLDSP (RV32/64)",                         0b00000000000000000010000000000010U, 0b00000000000000001110000000000011U,  "",      new Dictionary<string, uint> { { "frd", 0b00000000000000000000111110000000U }, { "immediate", 0b00000000000000000001000001111100U } } ),
                new RiscvInstructionMask( "C.LQSP (RV128; RES, rd=0)",                 0b00000000000000000010000000000010U, 0b00000000000000001110000000000011U,  "",      new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "immediate", 0b00000000000000000001000001111100U } } ),
                new RiscvInstructionMask( "C.LWSP (RES, rd=0)",                        0b00000000000000000100000000000010U, 0b00000000000000001110000000000011U,  "",      new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "immediate", 0b00000000000000000001000001111100U } } ),
                new RiscvInstructionMask( "C.FLWSP (RV32)",                            0b00000000000000000110000000000010U, 0b00000000000000001110000000000011U,  "",      new Dictionary<string, uint> { { "frd", 0b00000000000000000000111110000000U }, { "immediate", 0b00000000000000000001000001111100U } } ),
                new RiscvInstructionMask( "C.LDSP (RV64/128; RES, rd=0)",              0b00000000000000000110000000000010U, 0b00000000000000001110000000000011U,  "",      new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "immediate", 0b00000000000000000001000001111100U } } ),
                new RiscvInstructionMask( "C.JR (RES, rs1=0)",                         0b00000000000000001000000000000010U, 0b00000000000000001111000001111111U,  "",      new Dictionary<string, uint> { { "rs1", 0b00000000000000000000111110000000U } } ),
                new RiscvInstructionMask( "C.MV (HINT, rd=0)",                         0b00000000000000001000000000000010U, 0b00000000000000001111000000000011U,  "",      new Dictionary<string, uint> { { "rd", 0b00000000000000000000111110000000U }, { "rs2", 0b00000000000000000000000001111100U } } ),
                new RiscvInstructionMask( "C.EBREAK",                                  0b00000000000000001001000000000010U, 0b00000000000000001111111111111111U,  "",      new Dictionary<string, uint> { { "immediate", 0b00000000000000000000000000000000U } } ),
                new RiscvInstructionMask( "C.JALR",                                    0b00000000000000001001000000000010U, 0b00000000000000001111000001111111U,  "",      new Dictionary<string, uint> { { "rs1", 0b00000000000000000000111110000000U } } ),
                new RiscvInstructionMask( "C.ADD (HINT, rd=0)",                        0b00000000000000001001000000000010U, 0b00000000000000001111000000000011U,  "",      new Dictionary<string, uint> { { "rs1", 0b00000000000000000000111110000000U }, { "rs2", 0b00000000000000000000000001111100U } } ),
                new RiscvInstructionMask( "C.FSDSP (RV32/64)",                         0b00000000000000001010000000000010U, 0b00000000000000001110000000000011U,  "",      new Dictionary<string, uint> { { "frs2", 0b00000000000000000000000001111100U }, { "immediate", 0b00000000000000000001111110000000U } } ),
                new RiscvInstructionMask( "C.SQSP (RV128)",                            0b00000000000000001010000000000010U, 0b00000000000000001110000000000011U,  "",      new Dictionary<string, uint> { { "rs2", 0b00000000000000000000000001111100U }, { "immediate", 0b00000000000000000001111110000000U } } ),
                new RiscvInstructionMask( "C.SWSP",                                    0b00000000000000001100000000000010U, 0b00000000000000001110000000000011U,  "",      new Dictionary<string, uint> { { "rs2", 0b00000000000000000000000001111100U }, { "immediate", 0b00000000000000000001111110000000U } } ),
                new RiscvInstructionMask( "C.FSWSP (RV32)",                            0b00000000000000001110000000000010U, 0b00000000000000001110000000000011U,  "",      new Dictionary<string, uint> { { "frs2", 0b00000000000000000000000001111100U }, { "immediate", 0b00000000000000000001111110000000U } } ),
                new RiscvInstructionMask( "C.SDSP (RV64/128)",                         0b00000000000000001110000000000010U, 0b00000000000000001110000000000011U,  "",      new Dictionary<string, uint> { { "rs2", 0b00000000000000000000000001111100U }, { "immediate", 0b00000000000000000001111110000000U } } ),

        }.AsReadOnly();
    }
}