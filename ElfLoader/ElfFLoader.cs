using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ElfLoader {
    /// <summary>符号無しの32bit長プログラムアドレス</summary>
    using Elf32_Addr = UInt32;
    /// <summary>符号無しの16bit長整数</summary>
    using Elf32_Half = UInt16;
    /// <summary>符号無しの32bit長整数</summary>
    using Elf32_Word = UInt32;
    /// <summary>符号無しの32bit長オフセット</summary>
    using Elf32_Off = UInt32;



    #region 定数定義
    /// <summary>ファイルのタイプ</summary>
    public enum ElfType : Elf32_Half {
        /// <summary>不明</summary>
        ET_NONE = 0x0000,
        /// <summary>再配置可能ファイル</summary>
        ET_REL = 0x0001,
        /// <summary>実行ファイル</summary>
        ET_EXEC = 0x0002,
        /// <summary>共有オブジェクトファイル</summary>
        ET_DYN = 0x0003,
        /// <summary>コアファイル</summary>
        ET_CORE = 0x0004,
        /// <summary>プロセッサ依存</summary>
        ET_LOPROC = 0xFF00,
        /// <summary>プロセッサ依存</summary>
        ET_HIPROC = 0xFFFF,
    }

    /// <summary>マシンアーキテクチャ</summary>
    public enum ElfMachine : Elf32_Half {
        /// <summary>不明</summary>
        EM_NONE = 0,
        /// <summary>AT&T WE 32100</summary>
        EM_M32 = 1,
        /// <summary>SPARC</summary>
        EM_SPARC = 2,
        /// <summary>インテルアーキテクチャ</summary>
        EM_386 = 3,
        /// <summary>モトローラ68000</summary>
        EM_68K = 4,
        /// <summary>モトローラ88000</summary>
        EM_88K = 5,
        /// <summary>インテル80860</summary>
        ET_860 = 7,
        /// <summary>MIPS RS3000ビッグエンディアン</summary>
        EM_MIPS = 8,
        /// <summary>MIPS RS4000ビッグエンディアン</summary>
        EM_MIPS_RS4_BE = 10,
        /// <summary>予約</summary>
        RESERVED11 = 11,
        /// <summary>予約</summary>
        RESERVED12 = 12,
        /// <summary>予約</summary>
        RESERVED13 = 13,
        /// <summary>予約</summary>
        RESERVED14 = 14,
        /// <summary>予約</summary>
        RESERVED15 = 15,
        /// <summary>予約</summary>
        RESERVED16 = 16,

    }

    /// <summary>ELFバージョン</summary>
    public enum ElfVersion : Elf32_Word {
        /// <summary>無効</summary>
        EV_NONE = 0,
        /// <summary>バージョン1</summary>
        EV_CURRENT = 1,
    }

    /// <summary>ファイルクラス</summary>
    public enum ElfIdClass : byte {
        /// <summary>無効</summary>
        ELFCLASSNONE = 0,
        /// <summary>32bitオブジェクト</summary>
        ELFCLASS32 = 1,
        /// <summary>64bitオブジェクト</summary>
        ELFCLASS64 = 2,
    }


    /// <summary>プロセッサー固有のデータエンコーディング</summary>
    public enum ElfIdData : byte {
        /// <summary>無効</summary>
        ELFDATANONE = 0,
        /// <summary>LSB(リトルエンディアンデータ)</summary>
        ELFDATA2LSB = 1,
        /// <summary>MSB(ビッグエンディアンデータ)</summary>
        ELFDATA2MSB = 2,
    }

    #endregion

    /// <summary>ELFヘッダ情報</summary>
    public struct Elf32_Header {

        /// <summary>ELF識別子を格納する16のchar配列</summary>
        /// <detail>
        /// [0] = 0x7F(マジックナンバー)
        /// [1] = 'E'(マジックナンバー)
        /// [2] = 'L'(マジックナンバー)
        /// [3] = 'F'(マジックナンバー)
        /// [4] = ファイルクラス
        /// [5] = データエンコーディング
        /// [6] = バージョン
        /// [7-15] = (未定義)
        /// </detail>
        public char[] e_ident;

        /// <summary>ファイルクラス</summary>
        public ElfIdClass ei_class;
        /// <summary>プロセッサー固有のデータエンコーディング</summary>
        public ElfIdData ei_data;

        /// <summary>ファイルタイプ</summary>
        public ElfType e_type;
        /// <summary>マシンアーキテクチャ</summary>
        public ElfMachine e_machine;
        /// <summary>ELFバージョン</summary>
        public ElfVersion e_version;
        /// <summary>エントリーポイントアドレス</summary>
        public Elf32_Addr e_entry;
        /// <summary>プログラムヘッダテーブルへのオフセット</summary>
        public Elf32_Off e_phoff;
        /// <summary>セッションヘッダテーブルへのオフセット</summary>
        public Elf32_Off e_shoff;
        /// <summary></summary>
        public Elf32_Word e_flags;
        /// <summary>ELFヘッダサイズ</summary>
        public Elf32_Half e_ehsize;
        /// <summary>プログラムヘッダテーブル エントリサイズ</summary>
        public Elf32_Half e_phentsize;
        /// <summary>プログラムヘッダテーブル エントリ数</summary>
        public Elf32_Half e_phnum;
        /// <summary>セクションヘッダテーブル エントリサイズ</summary>
        public Elf32_Half e_shentsize;
        /// <summary>セクションヘッダテーブル エントリ数</summary>
        public Elf32_Half e_shnum;
        /// <summary>セクションヘッダテーブル 文字列テーブルインデックス</summary>
        public Elf32_Half e_shstrndx;

        /// <summary>セクションヘッダテーブル</summary>
        public Elf32_Shdr[] e_shdrtab;
        /// <summary>プログラムヘッダテーブル</summary>
        public Elf32_Phdr[] e_phdrtab;

        /// <summary>シンボルテーブル</summary>
        public Elf32_Sym[] e_symtab;

        /// <summary>ELFヘッダ情報</summary>
        public Elf32_Header(IEnumerable<byte> bytes) : this() {
            int ptr = 0;
            e_ident = new char[16];

            byte[] ehBytes = bytes.Take(52).ToArray();

            foreach (byte b in ehBytes.Take(16)) {
                e_ident[ptr++] = (char)b;
            }

            ei_class = (ElfIdClass)e_ident[4];
            ei_data = (ElfIdData)e_ident[5];

            e_type = (ElfType)BitConverter.ToUInt16(ehBytes, ptr); ptr += 2;
            e_machine = (ElfMachine)BitConverter.ToUInt16(ehBytes, ptr); ptr += 2;
            e_version = (ElfVersion)BitConverter.ToUInt32(ehBytes, ptr); ptr += 4;
            e_entry = BitConverter.ToUInt32(ehBytes, ptr); ptr += 4;
            e_phoff = BitConverter.ToUInt32(ehBytes, ptr); ptr += 4;
            e_shoff = BitConverter.ToUInt32(ehBytes, ptr); ptr += 4;
            e_flags = BitConverter.ToUInt32(ehBytes, ptr); ptr += 4;
            e_ehsize = BitConverter.ToUInt16(ehBytes, ptr); ptr += 2;
            e_phentsize = BitConverter.ToUInt16(ehBytes, ptr); ptr += 2;
            e_phnum = BitConverter.ToUInt16(ehBytes, ptr); ptr += 2;
            e_shentsize = BitConverter.ToUInt16(ehBytes, ptr); ptr += 2;
            e_shnum = BitConverter.ToUInt16(ehBytes, ptr); ptr += 2;
            e_shstrndx = BitConverter.ToUInt16(ehBytes, ptr); ptr += 2;

            // セクションヘッダテーブル
            e_shdrtab = new Elf32_Shdr[e_shnum];
            for (int i = 0; i < e_shnum; i++) {
                e_shdrtab[i] = new Elf32_Shdr(bytes.Skip((int)(e_shoff + (e_shentsize * i))).Take((int)e_shentsize));
            }


            char[] sh_strtab = null;
            // セクションヘッダ名文字列テーブルを検索
            Encoding enc = Encoding.ASCII;
            foreach (Elf32_Shdr shdr in e_shdrtab) {
                if (shdr.sh_type == ShType.SHT_STRTAB) {
                    sh_strtab = enc.GetChars(bytes.Skip((int)shdr.sh_offset).Take((int)shdr.sh_size).ToArray());

                    if (new string(sh_strtab.Skip((int)shdr.sh_nameidx).TakeWhile(c => c != '\0').ToArray()).Equals(".shstrtab")) {
                        break;
                    }
                    sh_strtab = null;
                }
            }

            // 各セクションテーブルにセクション名を付与
            if (sh_strtab != null) { 
                for (int i = 0; i < e_shdrtab.Length; i++) {
                    e_shdrtab[i].sh_name = new string(sh_strtab.Skip((int)e_shdrtab[i].sh_nameidx).TakeWhile(c => c != '\0').ToArray());
                }
            }

            // シンボルテーブル
            foreach (Elf32_Shdr shdr in e_shdrtab) {
                if (shdr.sh_type == ShType.SHT_SYMTAB) {
                    char[] sym_strtab = null;
                    foreach (Elf32_Shdr shdr_strtab in e_shdrtab) {
                    }

                    e_symtab = new Elf32_Sym[shdr.sh_size / shdr.sh_entsize];
                    for (int i = 0; i < shdr.sh_size / shdr.sh_entsize; i++) {
                        e_symtab[i] = new Elf32_Sym(bytes.Skip((int)(shdr.sh_offset + (shdr.sh_entsize * i))).Take((int)shdr.sh_entsize).ToArray(), sym_strtab);
                    }
                }
            }

            // プログラムヘッダテーブル
            e_phdrtab = new Elf32_Phdr[e_phnum];
            for (int i = 0; i < e_phnum; i++) {
                e_phdrtab[i] = new Elf32_Phdr(bytes.Skip((int)(e_phoff + (e_phentsize * i))).Take((int)e_phentsize));
            }
        }

        public static explicit operator Elf32_Header(byte[] bytes) {
            return new Elf32_Header(bytes);
        }


    }

    #region 定数定義
    /// <summary>セグメントタイプ</summary>
    public enum ShType : Elf32_Word {
        /// <summary>エントリーを無視</summary>
        SHT_NULL = 0,
        /// <summary>プログラムによって定義された情報</summary>
        SHT_PROGBITS = 1,
        /// <summary>シンボルテーブル</summary>
        SHT_SYMTAB = 2,
        /// <summary>文字列テーブル</summary>
        SHT_STRTAB = 3,
        /// <summary>再配置エントリー</summary>
        SHT_RELA = 4,
        /// <summary>シンボルハッシュテーブル</summary>
        SHT_HASH = 5,
        /// <summary>動的リンク時に必要な情報</summary>
        SHT_DYNAMIC = 6,
        /// <summary>オブジェクトファイルに記録された情報</summary>
        SHT_NOTE = 7,
        /// <summary>セクションスペース無しのプログラムによって定義された情報</summary>
        SHT_NOBITS = 8,
        /// <summary>再配置エントリー</summary>
        SHT_REL = 9,
        /// <summary>予約</summary>
        SHT_SHLIB = 10,
        /// <summary>動的リンク時に必要なシンボルテーブル</summary>
        SHT_DYNSYM = 11,
        /// <summary>プロセッサー固有情報範囲 開始アドレス</summary>
        SHT_LOPROC = 0x70000000,
        /// <summary>プロセッサー固有情報範囲 終了アドレス</summary>
        SHT_HIPROC = 0x7FFFFFFF,
        /// <summary>アプリケーションプログラム使用範囲 開始アドレス</summary>
        SHT_LOUSER = 0x80000000,
        /// <summary>アプリケーションプログラム使用範囲 終了アドレス</summary>
        SHT_HIUSER = 0x8FFFFFFF,
    }

    /// <summary></summary>
    public enum ShFlag : Elf32_Word {
        /// <summary>プロセス実行中書込可能セクション</summary>
        SHF_WRITE = 0x1,
        /// <summary>プロセス実行中にメモリーにロードされるセクション</summary>
        SHF_ALLOC = 0x2,
        /// <summary>実行コードセクション</summary>
        SHF_EXECINSTR = 0x4,
        /// <summary>プロセッサー固有情報セクション</summary>
        SHF_MASKPROC = 0xF0000000,
    }
    #endregion

    /// <summary>セクションヘッダテーブル</summary>
    public struct Elf32_Shdr {
        /// <summary>セクション名インデックス</summary>
        public Elf32_Word sh_nameidx;
        /// <summary>セクション名</summary>
        public string sh_name;
        /// <summary>セクションタイプ</summary>
        public ShType sh_type;
        /// <summary>セクションフラグ</summary>
        public ShFlag sh_flags;
        /// <summary>セクション開始アドレス</summary>
        public Elf32_Addr sh_addr;
        /// <summary>ファイル先頭からセクションまでのオフセット</summary>
        public Elf32_Off sh_offset;
        /// <summary>セクションサイズ</summary>
        public Elf32_Word sh_size;
        /// <summary>セクション名文字列テーブル インデックス値</summary>
        public Elf32_Word sh_link;
        /// <summary>エキストラ情報</summary>
        public Elf32_Word sh_info;
        /// <summary>アライメント値</summary>
        public Elf32_Word sh_addralign;
        /// <summary>エントリーサイズ</summary>
        public Elf32_Word sh_entsize;

        /// <summary>セクションヘッダテーブル</summary>
        public Elf32_Shdr(IEnumerable<byte> bytes) : this() {
            int ptr = 0;
            byte[] sHeader = bytes.ToArray();

            sh_nameidx = BitConverter.ToUInt32(sHeader, ptr); ptr += 4;
            sh_type = (ShType)BitConverter.ToUInt32(sHeader, ptr); ptr += 4;
            sh_flags = (ShFlag)BitConverter.ToUInt32(sHeader, ptr); ptr += 4;
            sh_addr = BitConverter.ToUInt32(sHeader, ptr); ptr += 4;
            sh_offset = BitConverter.ToUInt32(sHeader, ptr); ptr += 4;
            sh_size = BitConverter.ToUInt32(sHeader, ptr); ptr += 4;
            sh_link = BitConverter.ToUInt32(sHeader, ptr); ptr += 4;
            sh_info = BitConverter.ToUInt32(sHeader, ptr); ptr += 4;
            sh_addralign = BitConverter.ToUInt32(sHeader, ptr); ptr += 4;
            sh_entsize = BitConverter.ToUInt32(sHeader, ptr); ptr += 4;
        }

    }

    /// <summary>セグメントタイプ</summary>
    public enum PhType : Elf32_Word {
        /// <summary>エントリーを無視</summary>
        PT_NULL = 0,
        /// <summary>ロード可能なセグメントに ついての情報</summary>
        PT_LOAD = 1,
        /// <summary> 動的リンクに必要な情報</summary>
        PT_DYNAMIC = 2,
        /// <summary>インタープリターを実行するための情報</summary>
        PT_INTERP = 3,
        /// <summary>補足情報</summary>
        PT_NOTE = 4,
        /// <summary>予約</summary>
        PT_SHLIB = 5,
        /// <summary>プログラムヘッダテーブル自体の情報</summary>
        PT_PHDR = 6,
        /// <summary>予約</summary>
        PT_LOPROC = 0x70000000,
        /// <summary>予約</summary>
        PT_HIPROC = 0x7FFFFFFF,
    }

    /// <summary>プログラムヘッダテーブル</summary>
    public struct Elf32_Phdr {
        /// <summary>セグメントタイプ</summary>
        public PhType p_type;
        /// <summary>ファイル先頭からセグメントまでのオフセット</summary>
        public Elf32_Off p_offset;
        /// <summary>セグメントの最初のバイトがメモリー上に配置される仮想アドレス</summary>
        public Elf32_Addr p_vaddr;
        /// <summary>セグメントの物理アドレス</summary>
        public Elf32_Addr p_paddr;
        /// <summary>セグメントのファイルイメージサイズ</summary>
        public Elf32_Word p_filesz;
        /// <summary>メモリー上に配置されるセグメントのサイズ</summary>
        public Elf32_Word p_memsz;
        /// <summary>セグメントに関係するフラグ</summary>
        public Elf32_Word p_flags;
        /// <summary>アライメント値</summary>
        public Elf32_Word p_align;

        /// <summary>プログラムヘッダ</summary>
        public Elf32_Phdr(IEnumerable<byte> bytes) : this() {
            int ptr = 0;
            byte[] pHeader = bytes.ToArray();

            p_type = (PhType)BitConverter.ToUInt32(pHeader, ptr); ptr += 4;
            p_offset = BitConverter.ToUInt32(pHeader, ptr); ptr += 4;
            p_vaddr = BitConverter.ToUInt32(pHeader, ptr); ptr += 4;
            p_paddr = BitConverter.ToUInt32(pHeader, ptr); ptr += 4;
            p_filesz = BitConverter.ToUInt32(pHeader, ptr); ptr += 4;
            p_memsz = BitConverter.ToUInt32(pHeader, ptr); ptr += 4;
            p_flags = BitConverter.ToUInt32(pHeader, ptr); ptr += 4;
            p_align = BitConverter.ToUInt32(pHeader, ptr); ptr += 4;
        }
    }

    /// <summary>シンボルテーブル</summary>
    public struct Elf32_Sym {
        /// <summary>シンボル名</summary>
        public Elf32_Word st_nameidx;
        /// <summary>シンボル名</summary>
        public string st_name;
        /// <summary>値</summary>
        public Elf32_Addr st_value;
        /// <summary>値のサイズ</summary>
        public Elf32_Word st_size;
        /// <summary>シンボルのタイプと属性情報</summary>
        public byte st_info;
        /// <summary>定義なし</summary>
        public byte st_other;
        /// <summary>関連するセクションヘッダーテーブルインデックス値</summary>
        public Elf32_Half st_shndx;

        /// <summary>シンボルテーブル</summary>
        public Elf32_Sym(IEnumerable<byte> bytes, char[] sym_strtab) : this() {
            int ptr = 0;
            byte[] symTable = bytes.ToArray();

            st_nameidx = BitConverter.ToUInt32(symTable, ptr); ptr += 4;
            st_value = BitConverter.ToUInt32(symTable, ptr); ptr += 4;
            st_size = BitConverter.ToUInt32(symTable, ptr); ptr += 4;
            st_info = symTable[ptr++];
            st_other = symTable[ptr++];
            st_shndx = BitConverter.ToUInt16(symTable, ptr); ptr += 2;

            st_name = new string(sym_strtab?.Skip((int)st_nameidx).TakeWhile(c => c != '\0').ToArray());
        }
    }
}
