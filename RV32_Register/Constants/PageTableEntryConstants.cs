using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RV32_Register.Constants {
    /*******************************************
     * ページテーブルエントリ 定数、構造体定義 *
     *******************************************/

    /// <summary>ページテーブルエントリ(PTE)</summary>
    public struct PageTableEntry32 {
        // 変数
        public ushort[] PPN { get; set; }
        public bool IsDarty { get; set; }
        public bool IsAccessed { get; set; }
        public bool IsGlobal { get; set; }
        public bool IsUserMode { get; set; }
        public PtPermission Permission { get; set; }
        public bool IsValid { get; set; }

        // コンストラクタ
        public PageTableEntry32(uint value) {
            PPN = new ushort[2];
            PPN[1] = (ushort)((value & 0xfff0_0000u) >> 20);
            PPN[0] = (ushort)((value & 0x000f_fc00u) >> 10);
            IsDarty = (value & 0x0000_0080u) > 0;
            IsAccessed = (value & 0x0000_0040u) > 0;
            IsGlobal = (value & 0x0000_0020u) > 0;
            IsUserMode = (value & 0x0000_0010u) > 0;
            Permission = (PtPermission)((value & 0x0000_00e0u) >> 1);
            IsValid = (value & 0x0000_0001) > 0;
        }

        // キャスト
        public static implicit operator PageTableEntry32(uint value) {
            return new PageTableEntry32(value);
        }

        public static implicit operator uint(PageTableEntry32 pte) {
            uint value = 0;
            value |= (pte.PPN[1] & 0xfffu) << 20;
            value |= (pte.PPN[0] & 0x3ffu) << 10;
            value |= pte.IsDarty ? 1u << 7 : 0u;
            value |= pte.IsAccessed ? 1u << 6 : 0u;
            value |= pte.IsGlobal ? 1u << 5 : 0u;
            value |= pte.IsUserMode ? 1u << 4 : 0u;
            value |= (uint)pte.Permission << 1;
            value |= pte.IsValid ? 1u << 0 : 0u;
            return value;
        }
    }

    public struct VirtAddr32 {
        public ushort[] VPN { get; set; }
        public ushort PageOffset { get; set; }

        public VirtAddr32(uint value) {
            VPN = new ushort[2];
            VPN[1] = (ushort)((value & 0xffc0_0000u) >> 22);
            VPN[0] = (ushort)((value & 0x003f_f000u) >> 12);
            PageOffset = (ushort)(value & 0x0000_0fffu);
        }

        // キャスト
        public static implicit operator VirtAddr32(uint value) {
            return new VirtAddr32(value);
        }

        public static implicit operator uint(VirtAddr32 vaddr) {
            uint value = 0;
            value |= (vaddr.VPN[1] & 0x3ffu) << 22;
            value |= (vaddr.VPN[0] & 0x3ffu) << 12;
            value |= (vaddr.PageOffset & 0xfffu) << 0;
            return value;
        }
    }


    public struct PhyAddr32 {
        public ushort[] PPN { get; set; }
        public ushort PageOffset { get; set; }

        public PhyAddr32(ulong v) {
            PPN = new ushort[2];
            PPN[1] = (ushort)((v & 0x3_ffc0_0000u) >> 22);
            PPN[0] = (ushort)((v & 0x0_003f_f000u) >> 12);
            PageOffset = (ushort)(v & 0x0_0000_0fffu);
        }

        // キャスト
        public static implicit operator PhyAddr32(ulong value) {
            return new PhyAddr32(value);
        }

        public static implicit operator ulong(PhyAddr32 paddr) {
            ulong value = 0;
            value |= (paddr.PPN[1] & 0xfffu) << 22;
            value |= (paddr.PPN[0] & 0x3ffu) << 12;
            value |= (paddr.PageOffset & 0xfffu) << 0;
            return value;
        }
    }

    public enum PtPermission : byte {
        Pointer = 0b000,
        ReadOnly = 0b001,
        ReadWrite = 0b011,
        ExecuteOnly = 0b100,
        ReadExecute = 0b101,
        ReadWriteExecute = 0b111,
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
}
