using RiscVCpu.LoadStoreUnit.Constants;
using RiscVCpu.LoadStoreUnit.Exceptions;
using RiscVCpu.MemoryHandler;
using RiscVCpu.RegisterSet;
using System;

namespace RiscVCpu.LoadStoreUnit {

    /// <summary>
    /// Risc-V RV32I 基本命令セット ロードストアユニット
    /// </summary>
    public class RV32_Lsu : RV32_AbstractLoadStoreUnit {

        /// <summary>
        /// Risc-V ロードストアユニット
        /// </summary>
        /// <param name="registerSet">入出力用レジスタ</param>
        /// <param name="mainMemory">メインメモリ</param>
        public RV32_Lsu(RV32_RegisterSet registerSet, RV32_AbstractMemoryHandler mainMemory) : base(registerSet, mainMemory) {
        }

        #region Risc-V CPU命令

        /// <summary>
        /// Jump And Link命令
        /// 次の命令アドレス(pc+4)をレジスタrdに書き込み、現在のpcにoffsetを加えてpcに設定する
        /// </summary>
        /// <param name="rd">次の命令アドレスを格納するレジスタ番号</param>
        /// <param name="offset">ジャンプする現在のpcからの相対アドレス位置</param>
        public bool Jal(Register rd, Int32 offset, UInt32 insLength = 4u) {
            reg.SetValue(rd, reg.PC + 4u);
            reg.SetPc(reg.PC + (UInt32)offset);
            return true;
        }

        /// <summary>
        /// Jump And Link Register命令
        /// レジスタrs1+offsetをpcに書き込み、
        /// 次の命令アドレスだった値(pc+4)をレジスタrdに書き込む
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">ベースのアドレス</param>
        /// <param name="offset">ジャンプするオフセット値</param>
        public bool Jalr(Register rd, Register rs1, Int32 offset, UInt32 insLength = 4u) {
            UInt32 t = reg.PC + 4u;
            reg.SetPc((reg.GetValue(rs1) + (UInt32)offset) & ~1u);
            reg.SetValue(rd, t);
            return true;
        }

        #region Risv-V CPU Branch系命令

        /// <summary>
        /// Branch if Equal命令
        /// レジスタrs1の値とrs2の値が同じ場合、pcにoffsetを加える
        /// </summary>
        /// <param name="rs1">被比較数</param>
        /// <param name="rs2">比較数</param>
        /// <param name="offset">ジャンプするオフセット値</param>
        public bool Beq(Register rs1, Register rs2, Int32 offset, UInt32 insLength = 4u) {
            if (reg.GetValue(rs1) == reg.GetValue(rs2)) {
                reg.SetPc(reg.PC + (UInt32)offset);
            } else {
                reg.IncrementPc(insLength);
            }
            return true;
        }

        /// <summary>
        /// Branch if Not Equal命令
        /// レジスタrs1の値とrs2の値が異なる場合、pcにoffsetを加える
        /// </summary>
        /// <param name="rs1">被比較数</param>
        /// <param name="rs2">比較数</param>
        /// <param name="offset">ジャンプするオフセット値</param>
        public bool Bne(Register rs1, Register rs2, Int32 offset, UInt32 insLength = 4u) {
            if (reg.GetValue(rs1) != reg.GetValue(rs2)) {
                reg.SetPc(reg.PC + (UInt32)offset);
            } else {
                reg.IncrementPc(insLength);
            }
            return true;
        }

        /// <summary>
        /// Branch if Less Then命令
        /// レジスタrs1の値がrs2の値より小さい場合、pcにoffsetを加える
        /// </summary>
        /// <param name="rs1">被比較数</param>
        /// <param name="rs2">比較数</param>
        /// <param name="offset">ジャンプするオフセット値</param>
        public bool Blt(Register rs1, Register rs2, Int32 offset, UInt32 insLength = 4u) {
            if ((Int32)reg.GetValue(rs1) < (Int32)reg.GetValue(rs2)) {
                reg.SetPc(reg.PC + (UInt32)offset);
            } else {
                reg.IncrementPc(insLength);
            }
            return true;
        }

        /// <summary>
        /// Branch if Greater Then or Equal命令
        /// レジスタrs1の値がrs2の値以上の場合、pcにoffsetを加える
        /// </summary>
        /// <param name="rs1">被比較数</param>
        /// <param name="rs2">比較数</param>
        /// <param name="offset">ジャンプするオフセット値</param>
        public bool Bge(Register rs1, Register rs2, Int32 offset, UInt32 insLength = 4u) {
            if ((Int32)reg.GetValue(rs1) >= (Int32)reg.GetValue(rs2)) {
                reg.SetPc(reg.PC + (UInt32)offset);
            } else {
                reg.IncrementPc(insLength);
            }
            return true;
        }

        /// <summary>
        /// Branch if Less Then, Unsigned命令
        /// レジスタrs1の値がrs2の値より小さい場合、pcにoffsetを加える
        /// </summary>
        /// <param name="rs1">被比較数</param>
        /// <param name="rs2">比較数</param>
        /// <param name="offset">ジャンプするオフセット値</param>
        public bool Bltu(Register rs1, Register rs2, Int32 offset, UInt32 insLength = 4u) {
            if (reg.GetValue(rs1) < reg.GetValue(rs2)) {
                reg.SetPc(reg.PC + (UInt32)offset);
            } else {
                reg.IncrementPc(insLength);
            }
            return true;
        }

        /// <summary>
        /// Branch if Greater Then or Equal命令
        /// レジスタrs1の値がrs2の値以上の場合、pcにoffsetを加える
        /// </summary>
        /// <param name="rs1">被比較数</param>
        /// <param name="rs2">比較数</param>
        /// <param name="offset">ジャンプするオフセット値</param>
        public bool Bgeu(Register rs1, Register rs2, Int32 offset, UInt32 insLength = 4u) {
            if (reg.GetValue(rs1) >= reg.GetValue(rs2)) {
                reg.SetPc(reg.PC + (UInt32)offset);
            } else {
                reg.IncrementPc(insLength);
            }
            return true;
        }

        #endregion

        #region Risc-V CPU Load系命令

        /// <summary>
        /// Load Byte命令
        /// レジスタrs1+offsetのアドレスにあるメモリから、1バイトを符号拡張してレジスタrdに書き込む
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">ロードする対象のアドレスのベースが格納されているレジスタ番号</param>
        /// <param name="offset">オフセット</param>
        public bool Lb(Register rd, Register rs1, Int32 offset, UInt32 insLength = 4u) {
            UInt64 addr = (UInt64)(reg.GetValue(rs1) + offset);
            if (mem.CanOperate(addr, 1)) {
                byte[] bytes = new byte[4];
                bytes[0] = mem[addr];
                bytes[1] = (byte)((bytes[0] & 0x80u) > 1u ? 0xffu : 0u);
                bytes[2] = (byte)((bytes[0] & 0x80u) > 1u ? 0xffu : 0u);
                bytes[3] = (byte)((bytes[0] & 0x80u) > 1u ? 0xffu : 0u);
                reg.SetValue(rd, BitConverter.ToUInt32(bytes, 0));
            }
            reg.IncrementPc(insLength);
            return true;
        }
        /// <summary>
        /// Load Byte Unsigned命令
        /// レジスタrs1+offsetのアドレスにあるメモリから、1バイトをゼロ拡張してレジスタrdに書き込む
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">ロードする対象のアドレスのベースが格納されているレジスタ番号</param>
        /// <param name="offset">オフセット</param>
        public bool Lbu(Register rd, Register rs1, Int32 offset, UInt32 insLength = 4u) {
            UInt64 addr = (UInt64)(reg.GetValue(rs1) + offset);
            if (mem.CanOperate(addr, 1)) {
                byte[] bytes = new byte[4];
                bytes[0] = mem[addr];
                bytes[1] = 0;
                bytes[2] = 0;
                bytes[3] = 0;
                reg.SetValue(rd, BitConverter.ToUInt32(bytes, 0));
            }
            reg.IncrementPc(insLength);
            return true;
        }

        /// <summary>
        /// Load Harf word命令
        /// レジスタrs1+offsetのアドレスにあるメモリから、2バイトを符号拡張してレジスタrdに書き込む
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">ロードする対象のアドレスのベースが格納されているレジスタ番号</param>
        /// <param name="offset">オフセット</param>
        public bool Lh(Register rd, Register rs1, Int32 offset, UInt32 insLength = 4u) {
            UInt64 addr = (UInt64)(reg.GetValue(rs1) + offset);
            if (mem.CanOperate(addr, 2)) {
                byte[] bytes = new byte[4];
                bytes[0] = mem[addr + 0];
                bytes[1] = mem[addr + 1];
                bytes[2] = (byte)((bytes[1] & 128u) > 1u ? 0xffu : 0u);
                bytes[3] = (byte)((bytes[1] & 128u) > 1u ? 0xffu : 0u);
                reg.SetValue(rd, BitConverter.ToUInt32(bytes, 0));
            }
            reg.IncrementPc(insLength);
            return true;
        }

        /// <summary>
        /// Load Harf word Unsigned命令
        /// レジスタrs1+offsetのアドレスにあるメモリから、2バイトをゼロ拡張してレジスタrdに書き込む
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">ロードする対象のアドレスのベースが格納されているレジスタ番号</param>
        /// <param name="offset">オフセット</param>
        public bool Lhu(Register rd, Register rs1, Int32 offset, UInt32 insLength = 4u) {
            UInt64 addr = (UInt64)(reg.GetValue(rs1) + offset);
            if (mem.CanOperate(addr, 2)) {
                byte[] bytes = new byte[4];
                bytes[0] = mem[addr + 0];
                bytes[1] = mem[addr + 1];
                bytes[2] = 0;
                bytes[3] = 0;
                reg.SetValue(rd, BitConverter.ToUInt32(bytes, 0));
            }
            reg.IncrementPc(insLength);
            return true;
        }

        /// <summary>
        /// Load Word命令
        /// レジスタrs1+offsetのアドレスにあるメモリから、4バイトをレジスタrdに書き込む
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">ロードする対象のベースアドレスが格納されているレジスタ番号</param>
        /// <param name="offset">オフセット</param>
        public bool Lw(Register rd, Register rs1, Int32 offset, UInt32 insLength = 4u) {
            UInt64 addr = (UInt64)(reg.GetValue(rs1) + offset);
            if (mem.CanOperate(addr, 4)) {
                byte[] bytes = new byte[4];
                bytes[0] = mem[addr + 0];
                bytes[1] = mem[addr + 1];
                bytes[2] = mem[addr + 2];
                bytes[3] = mem[addr + 3];
                reg.SetValue(rd, BitConverter.ToUInt32(bytes, 0));
            }
            reg.IncrementPc(insLength);
            return true;
        }

        #endregion

        #region Risc-V CPU Store系命令

        /// <summary>
        /// Store Byte命令
        /// レジスタrs2の下位1バイトをレジスタrs1+offsetのアドレスのメモリに格納する
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">ストアする対象のアドレスのベースが格納されているレジスタ番号</param>
        /// <param name="offset">オフセット</param>
        public bool Sb(Register rs1, Register rs2, Int32 offset, UInt32 insLength = 4u) {
            UInt64 addr = (UInt64)(reg.GetValue(rs1) + offset);
            if (mem.CanOperate(addr, 1)) {
                byte[] bytes = BitConverter.GetBytes(reg.GetValue(rs2));
                mem[addr + 0] = bytes[0];
            }
            reg.IncrementPc(insLength);
            return true;
        }

        /// <summary>
        /// Store Harf word命令
        /// レジスタrs2の下位2バイトをレジスタrs1+offsetのアドレスのメモリに格納する
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">ストアする対象のアドレスのベースが格納されているレジスタ番号</param>
        /// <param name="offset">オフセット</param>
        public bool Sh(Register rs1, Register rs2, Int32 offset, UInt32 insLength = 4u) {
            UInt64 addr = (UInt64)(reg.GetValue(rs1) + offset);
            if (mem.CanOperate(addr, 2)) {
                byte[] bytes = BitConverter.GetBytes(reg.GetValue(rs2));
                mem[addr + 0] = bytes[0];
                mem[addr + 1] = bytes[1];
            }
            reg.IncrementPc(insLength);
            return true;
        }

        /// <summary>
        /// Store Word命令
        /// レジスタrs2の4バイトをレジスタrs1+offsetのアドレスのメモリに格納する
        /// </summary>
        /// <param name="rd">結果を格納するレジスタ番号</param>
        /// <param name="rs1">ストアする対象のアドレスのベースが格納されているレジスタ番号</param>
        /// <param name="offset">オフセット</param>
        public bool Sw(Register rs1, Register rs2, Int32 offset, UInt32 insLength = 4u) {
            UInt64 addr = (UInt64)(reg.GetValue(rs1) + offset);
            if (mem.CanOperate(addr, 4)) {
                byte[] bytes = BitConverter.GetBytes(reg.GetValue(rs2));
                mem[addr + 0] = bytes[0];
                mem[addr + 1] = bytes[1];
                mem[addr + 2] = bytes[2];
                mem[addr + 3] = bytes[3];
            }
            reg.IncrementPc(insLength);
            return true;
        }

        #endregion

        #region Risc-V CPU 特権命令
        /// <summary>
        /// Fence Memory and I/O命令
        /// </summary>
        /// <param name="pred_succ"></param>
        /// <returns>処理の成否</returns>
        public bool Fence(byte pred_succ, UInt32 insLength = 4u) {
            reg.IncrementPc(insLength);
            return true;
        }

        /// <summary>
        /// Fence Instruction Stream命令
        /// </summary>
        /// <returns>処理の成否</returns>
        public bool FenceI(UInt32 insLength = 4u) {
            reg.IncrementPc(insLength);
            return true;
        }

        /// <summary>
        /// Environment Call命令
        /// 環境呼び出し例外を起こして、実行環境を呼び出す
        /// </summary>
        /// <returns>処理の成否</returns>
        public bool Ecall(UInt32 insLength = 4u) {
            reg.IncrementPc(insLength);
            PrivilegeLevels prevMode = reg.CurrentMode;
            reg.CurrentMode = PrivilegeLevels.MachineMode;
            throw new RiscvEnvironmentCallException(prevMode, reg);
        }

        /// <summary>
        /// Environment Break命令
        /// ブレークポイント例外を起こして、実行環境を呼び出す
        /// </summary>
        /// <returns>処理の成否</returns>
        public bool Ebreak(UInt32 insLength = 4u) {
            reg.IncrementPc(insLength);
            reg.CurrentMode = PrivilegeLevels.MachineMode;
            throw new RiscvBreakpointException(reg);
        }

        /// <summary>
        /// Control Status Register Read and Write命令
        /// 現在のCSRをレジスタrdに書き込み、レジスタrs1をCSRに書き込む
        /// </summary>
        /// <param name="rd">現在のCSRの値を格納するレジスタ番号</param>
        /// <param name="rs1">書き込む値が格納されているレジスタ</param>
        /// <param name="csr">CSRのアドレス</param>
        /// <returns>処理の成否</returns>
        public bool Csrrw(Register rd, Register rs1, CSR csr, UInt32 insLength = 4u) {
            UInt32 t = reg.GetCSR(csr);
            reg.SetCSR(csr, 'w', rs1);
            reg.SetValue(rd, t);
            reg.IncrementPc(insLength);
            return true;
        }
        /// <summary>
        /// Control Status Register Read and Set命令
        /// 現在のCSRをレジスタrdに書き込み、現在のCSRとレジスタrs1の論理和をCSRに書き込む
        /// </summary>
        /// <param name="rd">現在のCSRの値を格納するレジスタ番号</param>
        /// <param name="rs1">セットする値が格納されているレジスタ</param>
        /// <param name="csr">CSRのアドレス</param>
        /// <returns>処理の成否</returns>
        public bool Csrrs(Register rd, Register rs1, CSR csr, UInt32 insLength = 4u) {
            UInt32 t = reg.GetCSR(csr);
            reg.SetCSR(csr, 's', rs1);
            reg.SetValue(rd, t);
            reg.IncrementPc(insLength);
            return true;
        }
        /// <summary>
        /// Control Status Register Read and Clear命令
        /// 現在のCSRをレジスタrdに書き込み、現在のCSRとレジスタrs1の1の補数との論理積をCSRに書き込む
        /// </summary>
        /// <param name="rd">現在のCSRの値を格納するレジスタ番号</param>
        /// <param name="rs1">クリアする値が格納されているレジスタ</param>
        /// <param name="csr">CSRのアドレス</param>
        /// <returns>処理の成否</returns>
        public bool Csrrc(Register rd, Register rs1, CSR csr, UInt32 insLength = 4u) {
            UInt32 t = reg.GetCSR(csr);
            reg.SetCSR(csr, 'c', rs1);
            reg.SetValue(rd, t);
            reg.IncrementPc(insLength);
            return true;
        }
        /// <summary>
        /// Control Status Register Read and Write Immediate命令
        /// 現在のCSRをレジスタrdに書き込み、即値をCSRに書き込む
        /// </summary>
        /// <param name="rd">現在のCSRの値を格納するレジスタ番号</param>
        /// <param name="zImmediate">即値</param>
        /// <param name="csr">CSRのアドレス</param>
        /// <returns>処理の成否</returns>
        public bool Csrrwi(Register rd, byte zImmediate, CSR csr, UInt32 insLength = 4u) {
            UInt32 t = reg.GetCSR(csr);
            reg.SetCSR(csr, 'w', zImmediate);
            reg.SetValue(rd, t);
            reg.IncrementPc(insLength);
            return true;
        }

        /// <summary>
        /// Control Status Register Read and Set Immediate命令
        /// 現在のCSRをレジスタrdに書き込み、現在のCSRと即値の論理和をCSRに書き込む
        /// </summary>
        /// <param name="rd">現在のCSRの値を格納するレジスタ番号</param>
        /// <param name="zImmediate">即値</param>
        /// <param name="csr">CSRのアドレス</param>
        /// <returns>処理の成否</returns>
        public bool Csrrsi(Register rd, byte zImmediate, CSR csr, UInt32 insLength = 4u) {
            UInt32 t = reg.GetCSR(csr);
            reg.SetCSR(csr, 's', zImmediate);
            reg.SetValue(rd, t);
            reg.IncrementPc(insLength);
            return true;
        }
        /// <summary>
        /// Control Status Register Read and Clear Immediate命令
        /// 現在のCSRをレジスタrdに書き込み、現在のCSRと即値の1の補数との論理積をCSRに書き込む
        /// </summary>
        /// <param name="rd">現在のCSRの値を格納するレジスタ番号</param>
        /// <param name="zImmediate">即値</param>
        /// <param name="csr">CSRのアドレス</param>
        /// <returns>処理の成否</returns>
        public bool Csrrci(Register rd, byte zImmediate, CSR csr, UInt32 insLength = 4u) {
            UInt32 t = reg.GetCSR(csr);
            reg.SetCSR(csr, 'c', zImmediate);
            reg.SetValue(rd, t);
            reg.IncrementPc(insLength);
            return true;
        }

        /// <summary>
        /// Wait For Interrupt命令
        /// </summary>
        /// <returns>処理の成否</returns>
        public bool Wfi(UInt32 insLength = 4u) {
            reg.IncrementPc(insLength);
            return true;
        }

        /// <summary>
        /// Fence Virtual Memory
        /// 先行するページ/テーブルへのストアと引き続いて怒る仮想アドレスの変換とを順序付ける
        /// </summary>
        /// <param name="rs1">
        /// 0: 選択されたアドレス空間の全ての仮想アドレスに関するアドレス変換が順序付けられる
        /// 0以外: 選択されたアドレス空間のrs1を含むアドレス変換だけが順序付けられる
        /// </param>
        /// <param name="rs2">
        /// 0: 全てのアドレス空間におけるアドレス変換が順序付けられる
        /// 0以外: rs2によって識別されるアドレス空間におけるアドレス変換のみが順序付けられる
        /// </param>
        /// <returns>処理の成否</returns>
        public bool SfenceVma(Register rs1, Register rs2, UInt32 insLength = 4u) {
            reg.IncrementPc(insLength);
            return true;
        }

        /// <summary>
        /// Machine-Mode Exception Return
        /// マシンモードの例外ハンドラから戻る
        /// </summary>
        /// <returns>処理の成否</returns>
        public bool Mret(UInt32 insLength = 4u) {
            reg.SetPc(reg.GetCSR(CSR.mepc));
            StatusCSR mstatus = (StatusCSR)reg.GetCSR(CSR.mstatus);

            PrivilegeLevels priv_level = (PrivilegeLevels)mstatus.MPP;

            mstatus.MIE = mstatus.MPIE;
            mstatus.MPIE = true;
            mstatus.MPP = 0;
            reg.SetCSR(CSR.mstatus, 'w', (UInt32)mstatus);

            reg.CurrentMode = priv_level;

            return true;
        }

        /// <summary>
        /// Supervisor-Mode Exception Return
        /// スーパーバイザモードの例外ハンドラから戻る
        /// </summary>
        /// <returns>処理の成否</returns>
        public bool Sret(UInt32 insLength = 4u) {
            reg.SetPc(reg.GetCSR(CSR.sepc));
            StatusCSR sstatus = (StatusCSR)reg.GetCSR(CSR.sstatus);
            sstatus.SPP = true;
            sstatus.SIE = sstatus.SPIE;
            sstatus.SPIE = true;
            sstatus.SPP = false;
            reg.SetCSR(CSR.sstatus, 'w', (UInt32)sstatus);

            //reg.CurrentMode = PrivilegeLevels.UserMode;

            return true;
        }

        #endregion

        #endregion
    }
}
