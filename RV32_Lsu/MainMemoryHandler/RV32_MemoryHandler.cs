using RV32_Lsu.Constants;
using RV32_Lsu.Exceptions;
using RV32_Lsu.RegisterSet;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RV32_Lsu.MemoryHandler {
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
        public override void Acquire(UInt64 address, Int32 length) {
        }

        /// <summary>
        /// メモリアドレスの予約を解放する
        /// </summary>
        /// <param name="address">メモリアドレス</param>
        public override void Release(UInt64 address) {
        }

        /// <summary>
        /// メモリアドレスの予約を全て解放する
        /// </summary>
        public override void Reset() {
        }
    }

    /// <summary>メインメモリハンドラ 抽象クラス</summary>
    public abstract class RV32_AbstractMemoryHandler {

        private RV32_RegisterSet reg;

        public byte this[UInt32 address] {
            // メモリへのストア
            set {
                if (address < PAddr || address >= Size - PAddr + Offset) {
                    throw new RiscvException(RiscvExceptionCause.StoreAMOPageFault, address, reg);
                }
                mainMemory[address - PAddr + Offset] = value;
                if (HostAccessAddress.Contains(address)) {
                    throw new HostAccessTrap();
                }
            }
            // メモリからのロード
            get {
                if (HostAccessAddress.Contains(address)) {
                    throw new HostAccessTrap();
                }
                if (address < PAddr || address >= Size - PAddr + Offset) {
                    throw new RiscvException(RiscvExceptionCause.LoadPageFault, address, reg);
                }
                return mainMemory[address - PAddr + Offset];
            }
        }

        /// <summary>
        /// 指定したアドレスから命令をフェッチする
        /// </summary>
        /// <param name="addr">命令のアドレス</param>
        /// <returns>32bit長 Risc-V命令</returns>
        public UInt32 FetchInstruction(UInt32 address) {
            if (HostAccessAddress.Contains(address)) {
                throw new HostAccessTrap();
            }
            if (address < PAddr || address >= Size - PAddr + Offset) {
                throw new RiscvException(RiscvExceptionCause.InstructionPageFault, address, reg);
            }
            return BitConverter.ToUInt32(mainMemory, (int)(address - PAddr + Offset));
        }

        private protected readonly byte[] mainMemory;
        public readonly HashSet<UInt64> HostAccessAddress;

        public UInt64 Size { get => (UInt64)mainMemory.LongLength; }
        public UInt64 PAddr { get; set; }
        public UInt64 Offset { get; set; }

        internal void SetRegisterSet(RV32_RegisterSet registerSet) {
            reg = registerSet;
        }

        /// <summary>メインメモリハンドラ</summary>
        /// <param name="mainMemory">基となるメインメモリのバイト配列</param>
        public RV32_AbstractMemoryHandler(byte[] mainMemory) {
            this.mainMemory = mainMemory;
            HostAccessAddress = new HashSet<UInt64>();
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
        public abstract void Acquire(UInt64 address, Int32 length);

        /// <summary>
        /// メモリアドレスの予約を解放する
        /// </summary>
        /// <param name="address">メモリアドレス</param>
        public abstract void Release(UInt64 address);

        /// <summary>
        /// メモリアドレスの予約を全て解放する
        /// </summary>
        public abstract void Reset();

        private protected UInt64 GetPhysicalAddr(UInt32 v_add, MemoryAccessMode accMode) {

            SatpCSR satp = reg.CSRegisters[CSR.satp];

            if (!satp.MODE || true) {
                return (ulong)v_add;

            } else {
                VirtAddr virt_addr = v_add;

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

                /* 1.aをsatp：ppn×PAGESIZEとし、i = LEVELS - 1とする（Sv32の場合、PAGESIZE = 212およびLEVELS = 2）
                 */
                const uint PageSize = 0x1000u;
                const int Levels = 2;

                uint pte_addr = satp.PPN * PageSize;
                int i = Levels - 1;

                PageTableEntry pte = 0;

                /* 2.アドレスa + vaにあるPTEの値をpteとします。vpn [i]×PTESIZE。 （Sv32の場合、PTESIZE = 4）
                 * pteにアクセスしてPMAまたはPMPチェックに違反した場合は、アクセス例外を発生させます。
                 */
                const uint PteSize = 4u;
                while (i >= 0) {

                    pte_addr += virt_addr.VPN[i] * PteSize;
                    pte = (PageTableEntry)BitConverter.ToUInt32(mainMemory, (int)(pte_addr - PAddr + Offset));

                    if (false) {
                        // ToDo: PMA,PMPチェックの実装
                    }

                    
                    /* 3.pte.v = 0の場合、またはpte.r = 0かつpte.w = 1の場合、停止してページフォルト例外を発生させます。
                     */
                    if (Enum.GetValues(typeof(PtPermission)).Cast<PtPermission>().Contains(pte.Permission) || !pte.IsValid) {
                        throw new RiscvException(pageFaultCouse, v_add, reg);
                    }

                    /* 4.それ以外の場合、PTEは有効です。
                     * pte：r = 1またはpte：x = 1の場合、手順5に進みます。
                     */
                    if (pte.Permission != PtPermission.Pointer) {
                        // ループを抜けて次の手順へ
                        break;
                    } else {
                        /* それ以外の場合、このPTEはページテーブルの次のレベルへのポインタです。
                         * ｉ ＝ ｉ － １とします。
                         * i <0の場合、停止してページ不在例外を発生させます。
                         * それ以外の場合は、a = pte.ppn×PAGESIZEとし、手順2に進みます。
                         */
                        i--;
                        if (i < 0) {
                            throw new RiscvException(pageFaultCouse, v_add, reg);
                        }
                        pte_addr = pte.PPN[i] * PageSize;
                        continue;
                    }
                }

                /* 5.リーフPTEが見つかりました。
                 * 現在の特権モードと、mstatusレジスタのSUMおよびMXRフィールドの値を考慮して、要求されたメモリアクセスが
                 * pte.r、pte.w、pte.x、およびpte.uビットによって許可されているかどうかを確認します。
                 * そうでない場合は、停止してページ不在例外を発生させます。
                 */
                StatusCSR status = reg.CSRegisters[CSR.mstatus];
                bool allowUser = pte.IsUserMode ^ (reg.CurrentMode != PrivilegeLevels.UserMode);
                bool allowUserWithSUM = reg.CurrentMode == PrivilegeLevels.SupervisorMode && status.SUM;

                bool allowPermission = ((byte)accMode & (byte)pte.Permission) != 0;
                bool allowPermissionWithMXR = pte.Permission == PtPermission.ExecuteOnly && status.MXR;

                if (!((allowUser || allowUserWithSUM) && (allowPermission || allowPermissionWithMXR))) {
                    throw new RiscvException(pageFaultCouse, v_add, reg);
                }

                /* 6. i> 0かつpa.ppn [i － 1：0]≠0の場合、これは位置ずれしたスーパーページです。
                 * ページフォルト例外を停止して発生させます。
                 */
                if (i > 0 && pte.PPN[i - 1] != 0) {
                    throw new RiscvException(pageFaultCouse, v_add, reg);
                }

                /* 7. pte.a = 0の場合、またはメモリアクセスがストアでpte.d = 0の場合、ページフォルト例外が発生するか、または、
                 * ・pte.aを1に設定し、メモリアクセスがストアの場合はpte.dも1に設定します。
                 * ・このアクセスがPMAまたはPMPチェックに違反している場合は、アクセス例外を発生させます。
                 * ・この更新と手順2のpteのロードはアトミックでなければなりません。
                 * 特に、PTEへの介在ストアがその間に発生したと認識されることはありません。
                 */
                if (!pte.IsAccessed || pte.IsDarty && accMode == MemoryAccessMode.Write) {
                    throw new RiscvException(pageFaultCouse, v_add, reg);
                }

                /* 8.変換は成功しました。変換された物理アドレスは以下のように与えられます。
                 * ・pa.pgoff = va.pgoff
                 * ・i> 0の場合、これはスーパーページ変換であり、pa：ppn [i - 1：0] = va：vpn [i - 1：0]です。
                 * ・pa.ppn [LEVELS - 1：i] = pte.ppn [LEVELS - 1：i]
                 */
                UInt64 phy_addr = 0u;
                phy_addr |= virt_addr.PageOffset;
                phy_addr |= (UInt64)(i > 0 ? virt_addr.VPN[i - 1] : pte.PPN[i ]) << 12;
                phy_addr |= (UInt64)pte.PPN[Levels - 1] << 22;
                return phy_addr;

            }

        }

    }

    /// <summary>
    /// ホストへトラップを返す
    /// </summary>
    public class HostAccessTrap : Exception {
    }
}
