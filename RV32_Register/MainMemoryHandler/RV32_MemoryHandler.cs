﻿using RV32_Register.Constants;
using RV32_Register.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RV32_Register.MemoryHandler {
    /// <summary>メインメモリハンドラ</summary>
    public class RV32_MemoryHandler : RV32_AbstractMemoryHandler {

        /// <summary>メインメモリハンドラ</summary>
        /// <param name="mainMemory">基となるメインメモリのバイト配列</param>
        public RV32_MemoryHandler(byte[] mainMemory) : base(mainMemory) { }

        /// <summary>
        /// メモリハンドラの基となったバイト配列を返す
        /// </summary>
        /// <returns>メモリハンドラの基となったバイト配列</returns>
        public override byte[] GetBytes() {
            return mainMemory;
        }

        /// <summary>
        /// 対象アドレスのメモリが操作可能かどうかを返す
        /// </summary>
        /// <param name="address">メモリアドレス</param>
        /// <param name="length">操作するメモリのバイト長</param>
        /// <returns>メモリ操作の可否</returns>
        public override bool CanOperate(UInt64 address, Int32 length) {
            return true;
        }

        /// <summary>
        /// メモリアドレスを予約する
        /// </summary>
        /// <param name="address">メモリアドレス</param>
        /// <param name="length">予約するアドレスの範囲(バイト)</param>
        public override void Acquire(UInt64 address, Int32 length) {
        }

        /// <summary>
        /// メモリアドレスの予約を解放する
        /// </summary>
        /// <param name="address">メモリアドレス</param>
        /// <param name="length">解放するアドレスの範囲(バイト)</param>
        public override void Release(UInt64 address, Int32 length) {
        }

        /// <summary>
        /// メモリアドレスの予約を全て解放する
        /// </summary>
        public override void ResetReservation() {
        }
    }

    /// <summary>メインメモリハンドラ 抽象クラス</summary>
    public abstract class RV32_AbstractMemoryHandler {

        private RV32_RegisterSet reg;

        /// <summary>メインメモリハンドラ</summary>
        /// <param name="mainMemory">基となるメインメモリのバイト配列</param>
        protected RV32_AbstractMemoryHandler(byte[] mainMemory) {
            this.mainMemory = mainMemory;
            ToHostTrapAddress = new HashSet<UInt64>();
            FromHostTrapAddress = new HashSet<UInt64>();
            TLB = new Dictionary<UInt32, TlbEntry32>();
        }

        /// <summary>
        /// メインメモリにアクセスします
        /// </summary>
        /// <param name="address">メモリアドレス</param>
        /// <returns>指定したメモリアドレスに格納されているbyte</returns>
        public byte this[UInt32 address] {
            // メモリへのストア
            set {
                ulong phy_addr = GetPhysicalAddr(address, MemoryAccessMode.Write);
                CheckPmp(phy_addr, MemoryAccessMode.Write);
                mainMemory[phy_addr] = value;
                if (ToHostTrapAddress.Contains(phy_addr)) {
                    throw new HostAccessTrap(value);
                } else if (FromHostTrapAddress.Contains(phy_addr)) {
                    // 外部割り込みを有効/無効にする
                    reg.CSRegisters.Eip ^= 0xb00U;
                }
            }
            // メモリからのロード
            get {
                ulong phy_addr = GetPhysicalAddr(address, MemoryAccessMode.Read);
                CheckPmp(phy_addr, MemoryAccessMode.Read);
                if (ToHostTrapAddress.Contains(phy_addr)) {
                    throw new HostAccessTrap(mainMemory[phy_addr]);
                }
                return mainMemory[phy_addr];
            }
        }

        /// <summary>
        /// 指定したアドレスから命令をフェッチする
        /// </summary>
        /// <param name="addr">命令のアドレス</param>
        /// <returns>32bit長 Risc-V命令</returns>
        public UInt32 FetchInstruction(UInt32 address) {
            ulong phy_addr = GetPhysicalAddr(address, MemoryAccessMode.Execute);
            CheckPmp(phy_addr, MemoryAccessMode.Execute);
            if (ToHostTrapAddress.Contains(phy_addr)) {
                throw new HostAccessTrap(BitConverter.ToUInt32(mainMemory, (int)phy_addr));
            }

            return BitConverter.ToUInt32(mainMemory, (int)phy_addr);
        }

        /// <summary>メインメモリ本体バイト配列</summary>
        private protected readonly byte[] mainMemory;

        /// <summary>仮想アドレス変換索引バッファ(Translation Lookaside Buffer)</summary>
        private readonly Dictionary<UInt32, TlbEntry32> TLB;

        /// <summary>メインメモリサイズ</summary>
        public UInt64 Size { get => (UInt64)mainMemory.LongLength; }

        /// <summary>仮想メモリ開始アドレス</summary>
        public UInt32 EntryVirtAddr { get; set; }
        /// <summary>物理メモリアクセス時のオフセット</summary>
        internal UInt32 Offset { get; set; }

        /// <summary>メモリアクセス時にホストへ通知するアドレス</summary>
        internal HashSet<UInt64> ToHostTrapAddress { get; }
        /// <summary>ホストからの外部割り込み時にアクセスするアドレス</summary>
        internal HashSet<UInt64> FromHostTrapAddress { get; }

        internal void SetRegisterSet(RV32_RegisterSet registerSet) {
            reg = registerSet;
        }

        /// <summary>
        /// メモリハンドラの基となったバイト配列を返す
        /// </summary>
        /// <returns>メモリハンドラの基となったバイト配列</returns>
        public abstract byte[] GetBytes();

        /// <summary>
        /// 対象アドレスのメモリが操作可能かどうかを返す
        /// </summary>
        /// <param name="address">メモリアドレス</param>
        /// <param name="length">操作するメモリのバイト長</param>
        /// <returns>メモリ操作の可否</returns>
        public abstract bool CanOperate(UInt64 address, Int32 length);

        /// <summary>
        /// メモリアドレスを予約する
        /// </summary>
        /// <param name="address">メモリアドレス</param>
        /// <param name="length">予約するアドレスの範囲(バイト)</param>
        public abstract void Acquire(UInt64 address, Int32 length);

        /// <summary>
        /// メモリアドレスの予約を解放する
        /// </summary>
        /// <param name="address">メモリアドレス</param>
        /// <param name="length">解放するアドレスの範囲(バイト)</param>
        public abstract void Release(UInt64 address, Int32 length);

        /// <summary>
        /// メモリアドレスの予約を全て解放する
        /// </summary>
        public abstract void ResetReservation();

        /// <summary>
        /// TLBをメモリに書き戻す
        /// </summary>
        /// <param name="vadd">仮想アドレス</param>
        /// <param name="asid">アドレス空間識別子(Address Space Identify)</param>
        public void TlbFlush(UInt32 vadd, UInt32 asid) {
            foreach (TlbEntry32 tlbEnt in TLB.Values) {
                // Todo: TLB Flush処理(メモリ書き戻し)の実装

            }
        }

        private protected UInt64 GetPhysicalAddr(UInt32 virt_addr, MemoryAccessMode accMode) {

            UInt64 phy_addr = 0U;

            RiscvExceptionCause pageFaultCouse;
            switch (accMode) {
                case MemoryAccessMode.Execute:
                    pageFaultCouse = RiscvExceptionCause.InstructionPageFault;
                    break;
                case MemoryAccessMode.Write:
                    pageFaultCouse = RiscvExceptionCause.StoreAMOPageFault;
                    break;
                case MemoryAccessMode.Read:
                    pageFaultCouse = RiscvExceptionCause.LoadPageFault;
                    break;
                default:
                    return 0;
            }

            SatpCSR satp = reg.CSRegisters[CSR.satp];

            if (!satp.MODE || reg.CurrentMode > PrivilegeLevel.SupervisorMode) {
                phy_addr = virt_addr + Offset;
                if (phy_addr >= Size) {
                    throw new RiscvException(pageFaultCouse, virt_addr, reg);
                }
                return phy_addr;
            }

            VirtAddr32 vaddr = virt_addr - EntryVirtAddr;

            // TLB検索処理
            uint vaddr_vpn = 0;
            vaddr_vpn = (uint)vaddr.VPN[0] << 10 | (uint)vaddr.VPN[1];
            if (TLB.Keys.Contains(vaddr_vpn)) {
                // ヒットした場合、キャッシュしていたアドレスにオフセットを足して返す
                phy_addr = (TLB[vaddr_vpn] & ~0xfffUL) | vaddr.PageOffset;
                return phy_addr;
            }

            /* 1.pte_addrをsatp:ppn×PAGESIZEとし、i = LEVELS - 1とする
             * （Sv32の場合、PAGESIZE = 2^12、LEVELS = 2）
             */
            const uint PageSize = 0x1000U;
            const int Levels = 2;

            uint pte_addr = satp.PPN * PageSize;

            int i = Levels - 1;

            PageTableEntry32 pte = 0;

            /* 2.pte をアドレス pte_addr + va.ppn[i]×PTESIZEにあるPTEの値とする（Sv32の場合、PTESIZE = 4）
             */
            const uint PteSize = 4U;

            while (i >= 0) {

                pte_addr += vaddr.VPN[i] * PteSize;

                // ToDo: PMA,PMPチェックの実装
                CheckPmp(pte_addr + Offset, accMode);

                pte = BitConverter.ToUInt32(mainMemory, (int)(pte_addr + Offset));

                /* 3.pte.v = 0の場合、またはpte.r = 0かつpte.w = 1の場合、処理を停止してページフォルト例外を発生させる。
                 */
                if (!pte.IsValid || (((int)pte.Permission & 0b011) == 0b010)) {
                    throw new RiscvException(pageFaultCouse, virt_addr, reg);
                }

                /* 4.それ以外の場合、PTEは有効である。
                 * pte：r = 1またはpte：x = 1の場合、手順5に進む。
                 */
                if (pte.Permission != PtPermission.Pointer) {
                    // ループを抜けて次の手順へ
                    break;
                } else {
                    /* それ以外の場合、このPTEはページテーブルの次のレベルへのポインタである。
                     * i = i + 1としする。i < 0の場合、処理を停止してページフォルト例外を発生させる。
                     * それ以外の場合は、pte_addr = pte.ppn×PAGESIZEとして、手順2に戻る。
                     */
                    i--;
                    if (i < 0) {
                        throw new RiscvException(pageFaultCouse, virt_addr, reg);
                    }
                    pte_addr = pte.PPN[i] * PageSize;
                    continue;
                }
            }

            /* 5.リーフPTEが見つかりました。
             * 現在の特権モードと、mstatusレジスタのSUMおよびMXRフィールドの値を考慮して、
             * 要求されたメモリアクセスが許可されているかを pte.r、pte.w、pte.x、およびpte.uビットに基づいて判定する。
             * 許可されない場合は、処理を停止してページフォルト例外を発生させる。
             */
            StatusCSR status = reg.CSRegisters[CSR.mstatus];
            bool allowUser = pte.IsUserMode ^ (reg.CurrentMode != PrivilegeLevel.UserMode);
            bool allowUserWithSUM = reg.CurrentMode == PrivilegeLevel.SupervisorMode && status.SUM;

            bool allowPermission = ((byte)accMode & (byte)pte.Permission) != 0;
            bool allowPermissionWithMXR = accMode == MemoryAccessMode.Read && pte.Permission == PtPermission.ExecuteOnly && status.MXR;

            if (!((allowUser || allowUserWithSUM) && (allowPermission || allowPermissionWithMXR))) {
                throw new RiscvException(pageFaultCouse, virt_addr, reg);
            }

            /* 6. i > 0かつpa.ppn[i - 1: 0]≠0の場合、整列されていないスーパーページである。
             * 処理を停止してページフォルト例外を発生させる。
             */
            if (i > 0 && pte.PPN[i - 1] != 0) {
                throw new RiscvException(pageFaultCouse, virt_addr, reg);
            }

            /* 7. pte.a = 0の場合、もしくはメモリアクセスがストアでpte.d = 0の場合は下記のどちらかを行う。
             * ・処理を停止してページフォルト例外を発生させる。
             * ・pte.aを1に設定し、メモリアクセスがストアの場合はpte.dも1に設定する。
             *   なお、このアクセスがPMAまたはPMPチェックに違反している場合は、アクセス例外を発生させる。
             *   この更新と手順2のpteのロードはアトミックでなければならない。
             */
            if (!pte.IsAccessed || (!pte.IsDarty && accMode == MemoryAccessMode.Write)) {
                throw new RiscvException(pageFaultCouse, virt_addr, reg);
                //pte.IsAccessed = true;
                //if (accMode == MemoryAccessMode.Write) {
                //    pte.IsDarty = true;
                //}
                // ToDo: PMA,PMPチェックの実装
                //CheckPmp(pte_addr + Offset, accMode);
            }


            /* 8.変換成功である。変換された物理アドレスは以下のように与えられます。
             * ・pa.pgoff = va.pgoff
             * ・i> 0の場合、これはスーパーページ変換であり、pa:ppn[i - 1: 0] = va:vpn[i - 1: 0]である。
             * ・あとはpa.ppn[LEVELS - 1: i] = pte.ppn[LEVELS - 1: i]である。
             */

            // 物理アドレスを生成
            phy_addr |= (UInt64)pte.PPN[Levels - 1] << 22;
            phy_addr |= (UInt64)(i > 0 ? vaddr.VPN[i - 1] : pte.PPN[i - 1]) << 12;
            phy_addr |= vaddr.PageOffset;

            // TLBに物理アドレスの上位と PTEの下位8ビットの情報をキャッシュする
            TlbEntry32 tlb = new TlbEntry32(0) {
                IsDarty = pte.IsDarty,
                IsAccessed = pte.IsAccessed,
                IsGlobal = pte.IsGlobal,
                IsUserMode = pte.IsUserMode,
                Permission = pte.Permission,
                IsValid = pte.IsValid,
                PteAddress = pte_addr
            };

            tlb.PPN[1] = pte.PPN[Levels - 1];
            tlb.PPN[0] = i > 0 ? vaddr.VPN[i - 1] : pte.PPN[i];
            TLB.Add(vaddr_vpn, tlb);

            return phy_addr + Offset - EntryVirtAddr;


        }

        private void CheckPmp(UInt64 addr, MemoryAccessMode accessMode) {
            PhysicalMemoryProtectionConfig[] pmpcfg;
            pmpcfg = PhysicalMemoryProtectionConfig.GetPmpCfgs(
                reg.CSRegisters[CSR.pmpcfg0],
                reg.CSRegisters[CSR.pmpcfg1],
                reg.CSRegisters[CSR.pmpcfg2],
                reg.CSRegisters[CSR.pmpcfg3]);

            RiscvExceptionCause accessFaultCause;
            switch (accessMode) {
                case MemoryAccessMode.Execute:
                    accessFaultCause = RiscvExceptionCause.InstructionAccessFault;
                    break;
                case MemoryAccessMode.Write:
                    accessFaultCause = RiscvExceptionCause.StoreAMOAccessFault;
                    break;
                case MemoryAccessMode.Read:
                    accessFaultCause = RiscvExceptionCause.LoadAccessFault;
                    break;
                default:
                    return;
            }

            int i = 0;
            bool match = false;
            for (; i < pmpcfg.Length; i++) {
                if (pmpcfg[i].AddressMatchingMode == AddressMatchingMode.Off) {
                    continue;

                } else if (pmpcfg[i].AddressMatchingMode == AddressMatchingMode.Tor) {
                    if ((i > 0 || reg.CSRegisters[(CSR)((int)CSR.pmpaddr0 + i - 1)] <= (UInt32)(addr >> 2)) &&
                        reg.CSRegisters[(CSR)((int)CSR.pmpaddr0 + i)] > (UInt32)(addr >> 2)) {
                        match = true;
                        break;
                    }
                    continue;

                } else if (pmpcfg[i].AddressMatchingMode == AddressMatchingMode.Na4) {
                    if (reg.CSRegisters[(CSR)((int)CSR.pmpaddr0 + i)] == (UInt32)(addr >> 2)) {
                        match = true;
                        break;
                    }
                    continue;

                } else if (pmpcfg[i].AddressMatchingMode == AddressMatchingMode.NaPot) {
                    int j = 0;
                    UInt32 mask = 0U;
                    while (i < 32) {
                        mask |= 1U << j++;
                        if ((reg.CSRegisters[(CSR)((int)CSR.pmpaddr0 + i)] & mask) != mask) {
                            break;
                        }
                    }
                    mask = ~mask;
                    if ((reg.CSRegisters[(CSR)((int)CSR.pmpaddr0 + i)] & mask) == ((UInt32)(addr >> 2) & mask)) {
                        match = true;
                        break;
                    }
                    continue;

                }
            }

            if (match) {
                if (!pmpcfg[i].IsLockingMode && reg.CurrentMode == PrivilegeLevel.MachineMode) {
                    return;
                }
                else if ((pmpcfg[i].Permission & accessMode) > 0) {
                    return;
                }
                throw new RiscvException(accessFaultCause, (UInt32)addr, reg);
            } else {
                if (reg.CurrentMode == PrivilegeLevel.MachineMode) {
                    return;
                }
                throw new RiscvException(accessFaultCause, (UInt32)addr, reg);
            }
        }
    }

    [Serializable]
    /// <summary>
    /// ホストへトラップを返す
    /// </summary>
    public class HostAccessTrap : ArgumentException {
        public HostAccessTrap(UInt32 value) {
            Data.Add("value", value);
        }
    }
}
