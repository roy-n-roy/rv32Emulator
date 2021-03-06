﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RV32_Register.Constants {
    /*******************************************
     * メモリ保護機能 定数、構造体定義 *
     *******************************************/

    #region ページテーブルエントリ
    
    public enum PtPermission : byte {
        /// <summary>ポインタ</summary>
        Pointer = 0,

        /// <summary>読み込みのみ可(書き実行不可)</summary>
        ReadOnly = MemoryAccessMode.Read,

        /// <summary>読み + 書き可(実行不可)</summary>
        ReadWrite = MemoryAccessMode.Read | MemoryAccessMode.Write,

        /// <summary>実行のみ可(読み書き不可)</summary>
        ExecuteOnly = MemoryAccessMode.Execute,

        /// <summary>読み込み + 実行可(書き込み不可)</summary>
        ReadExecute = MemoryAccessMode.Read | MemoryAccessMode.Execute,

        /// <summary>読み + 書き + 実行可</summary>
        ReadWriteExecute = MemoryAccessMode.Read | MemoryAccessMode.Write | MemoryAccessMode.Execute,
    }

    [FlagsAttribute]
    public enum MemoryAccessMode : byte {
        /// <summary>メモリからの読み込み, ロード</summary>
        Read = 0b001,
        /// <summary>メモリへの書き込み, ストア</summary>
        Write = 0b010,
        /// <summary>メモリ上の命令実行, フェッチ</summary>
        Execute = 0b100,
    }

    /// <summary>Sv32モード ページテーブルエントリ(PTE)</summary>
    public struct PageTableEntry32 {
        // 変数
        /// <summary>物理ページ番号(Physical Page Number)</summary>
        public ushort[] PPN;
        /// <summary>書き込み済みフラグ</summary>
        public bool IsDarty { get; set; }
        /// <summary>アクセス済みフラグ</summary>
        public bool IsAccessed { get; set; }
        /// <summary>グローバルフラグ</summary>
        public bool IsGlobal { get; }
        /// <summary>ユーザーモードアクセスフラグ</summary>
        public bool IsUserMode { get; }
        /// <summary>パーミッション</summary>
        public PtPermission Permission { get; }
        /// <summary>有効フラグ</summary>
        public bool IsValid { get; }

        // コンストラクタ
        public PageTableEntry32(uint value) {
            PPN = new ushort[2];
            PPN[1] = (ushort)((value & 0xfff0_0000U) >> 20);
            PPN[0] = (ushort)((value & 0x000f_fc00U) >> 10);
            IsDarty = (value & 0x0000_0080U) > 0;
            IsAccessed = (value & 0x0000_0040U) > 0;
            IsGlobal = (value & 0x0000_0020U) > 0;
            IsUserMode = (value & 0x0000_0010U) > 0;
            Permission = (PtPermission)((value & 0x0000_000eU) >> 1);
            IsValid = (value & 0x0000_0001) > 0;
        }

        // キャスト
        public static implicit operator PageTableEntry32(uint value) {
            return new PageTableEntry32(value);
        }

        public static implicit operator uint(PageTableEntry32 pte) {
            uint value = 0;
            value |= (pte.PPN[1] & 0xfffU) << 20;
            value |= (pte.PPN[0] & 0x3ffU) << 10;
            value |= pte.IsDarty ? 1U << 7 : 0U;
            value |= pte.IsAccessed ? 1U << 6 : 0U;
            value |= pte.IsGlobal ? 1U << 5 : 0U;
            value |= pte.IsUserMode ? 1U << 4 : 0U;
            value |= (uint)pte.Permission << 1;
            value |= pte.IsValid ? 1U << 0 : 0U;
            return value;
        }
    }

    /// <summary>Sv32モード 32bit長 仮想アドレス</summary>
    public struct VirtAddr32 {
        /// <summary>仮想ページ番号(Virtual Page Number)</summary>
        public ushort[] VPN;
        /// <summary>ページオフセット</summary>
        public ushort PageOffset { get; set; }

        public VirtAddr32(uint value) {
            VPN = new ushort[2];
            VPN[1] = (ushort)((value & 0xffc0_0000U) >> 22);
            VPN[0] = (ushort)((value & 0x003f_f000U) >> 12);
            PageOffset = (ushort)(value & 0x0000_0fffU);
        }

        // キャスト
        public static implicit operator VirtAddr32(uint value) {
            return new VirtAddr32(value);
        }

        public static implicit operator uint(VirtAddr32 vaddr) {
            uint value = 0;
            value |= (vaddr.VPN[1] & 0x3ffU) << 22;
            value |= (vaddr.VPN[0] & 0x3ffU) << 12;
            value |= (vaddr.PageOffset & 0xfffU) << 0;
            return value;
        }
    }

    /// <summary>Sv32モード TLBエントリ(Translation Lookaside Buffer)</summary>
    public struct TlbEntry32 {
        /// <summary>物理ページ番号(Virtual Page Number)</summary>
        public ushort[] PPN { get; set; }
        /// <summary>書き込み済みフラグ</summary>
        public bool IsDarty { get; set; }
        /// <summary>アクセス済みフラグ</summary>
        public bool IsAccessed { get; set; }
        /// <summary>グローバルフラグ</summary>
        public bool IsGlobal { get; set; }
        /// <summary>ユーザーモードアクセスフラグ</summary>
        public bool IsUserMode { get; set; }
        /// <summary>パーミッション</summary>
        public PtPermission Permission { get; set; }
        /// <summary>有効フラグ</summary>
        public bool IsValid { get; set; }
        /// <summary>ページテーブルエントリアドレス</summary>
        public uint PteAddress { get; set; }

        public TlbEntry32(ulong value) {
            PPN = new ushort[2];
            PPN[1] = (ushort)((value & 0x3_ffc0_0000UL) >> 22);
            PPN[0] = (ushort)((value & 0x0_003f_f000U) >> 12);
            IsDarty = (value & 0x0000_0080U) > 0;
            IsAccessed = (value & 0x0000_0040U) > 0;
            IsGlobal = (value & 0x0000_0020U) > 0;
            IsUserMode = (value & 0x0000_0010U) > 0;
            Permission = (PtPermission)((value & 0x0000_000eU) >> 1);
            IsValid = (value & 0x0000_0001) > 0;
            PteAddress = 0;
        }

        // キャスト
        public static implicit operator TlbEntry32(ulong value) {
            return new TlbEntry32(value);
        }

        public static implicit operator ulong(TlbEntry32 tlb) {
            ulong value = 0;
            value |= (tlb.PPN[1] & 0x3ffU) << 22;
            value |= (tlb.PPN[0] & 0x3ffU) << 12;
            value |= tlb.IsDarty ? 1U << 7 : 0U;
            value |= tlb.IsAccessed ? 1U << 6 : 0U;
            value |= tlb.IsGlobal ? 1U << 5 : 0U;
            value |= tlb.IsUserMode ? 1U << 4 : 0U;
            value |= (uint)tlb.Permission << 1;
            value |= tlb.IsValid ? 1U << 0 : 0U;
            return value;
        }
    }

    #endregion

    public enum AddressMatchingMode : byte {
        Off = 0,
        Tor = 1,
        Na4 = 2,
        NaPot = 3,
    }

    public struct PhysicalMemoryProtectionConfig {
        public bool IsLockingMode { get; }
        public AddressMatchingMode AddressMatchingMode { get; }
        public MemoryAccessMode Permission { get; }

        // コンストラクタ
        public PhysicalMemoryProtectionConfig(byte value) {
            IsLockingMode = (value & 0x80) > 0;
            AddressMatchingMode = (AddressMatchingMode)((value & 0x18) >> 3);
            Permission = (MemoryAccessMode)(value & 0x07);
        }

        // キャスト
        public static implicit operator PhysicalMemoryProtectionConfig(byte value) {
            return new PhysicalMemoryProtectionConfig(value);
        }

        public static implicit operator byte(PhysicalMemoryProtectionConfig pmpcfg) {
            byte value = 0;
            value |= (byte)(pmpcfg.IsLockingMode ? 0x80 : 0);
            value |= (byte)(((byte)pmpcfg.AddressMatchingMode & 0x03) << 3);
            value |= (byte)((byte)pmpcfg.Permission & 0x07);
            return value;
        }

        public static PhysicalMemoryProtectionConfig[] GetPmpCfgs(uint pmpcfg0, uint pmpcfg1, uint pmpcfg2, uint pmpcfg3) {
            PhysicalMemoryProtectionConfig[] pmpcfgArray = new PhysicalMemoryProtectionConfig[16];
            int i = 0;
            foreach (uint pmpcfg in new uint[] { pmpcfg0, pmpcfg1, pmpcfg2, pmpcfg3 }) {
                foreach (byte byt in BitConverter.GetBytes(pmpcfg)) {
                    pmpcfgArray[i++] = byt;
                    if (i >= pmpcfgArray.Length) {
                        return pmpcfgArray;
                    }
                }
            }
            return pmpcfgArray;
        }

        public static PhysicalMemoryProtectionConfig[] GetPmpCfgs(ulong pmpcfg0, ulong pmpcfg2) {
            PhysicalMemoryProtectionConfig[] pmpcfgArray = new PhysicalMemoryProtectionConfig[16];
            int i = 0;
            foreach (ulong pmpcfg in new ulong[] { pmpcfg0, pmpcfg2 }) {
                foreach (byte byt in BitConverter.GetBytes(pmpcfg)) {
                    pmpcfgArray[i++] = byt;
                    if (i >= pmpcfgArray.Length) {
                        return pmpcfgArray;
                    }
                }
            }
            return pmpcfgArray;
        }
    }
}
