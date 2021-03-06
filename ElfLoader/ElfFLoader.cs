﻿using System;
using System.Collections.Generic;
using System.IO;
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
        ET_LOPROC = 0xff00,
        /// <summary>プロセッサ依存</summary>
        ET_HIPROC = 0xffff,
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
        /// [0] = 0x7f(マジックナンバー)
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

        /// <summary>シンボルテーブル</summary>
        public string ProgramFilePath;

        /// <summary>ELFヘッダ情報</summary>
        public Elf32_Header(string ProgramFilePath) : this() {

            this.ProgramFilePath = ProgramFilePath;

            long fileSize = 0;
            using (BinaryReader br = new BinaryReader(File.Open(this.ProgramFilePath, FileMode.Open))) {
                int ptr = 0;
                e_ident = new char[16];

                byte[] ehBytes = new byte[52];

                fileSize = br.BaseStream.Length;
                br.Read(ehBytes, 0, ehBytes.Length);

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

                ehBytes = null;

                // プログラムヘッダテーブル
                if (e_phoff != br.BaseStream.Position) {
                    // 現在のファイル内の位置が e_phoff と違っている場合は、e_phoff までシークする
                    br.BaseStream.Seek(e_phoff, SeekOrigin.Begin);
                }
                e_phdrtab = new Elf32_Phdr[e_phnum];
                for (int i = 0; i < e_phnum; i++) {
                    byte[] e_ph = new byte[e_phentsize];
                    br.Read(e_ph, 0, e_ph.Length);
                    e_phdrtab[i] = new Elf32_Phdr(e_ph);
                }

                // セクションヘッダテーブル
                if (e_shoff != br.BaseStream.Position) {
                    // 現在のファイル内の位置が e_shoff と違っている場合は、e_shoff までシークする
                    br.BaseStream.Seek(e_shoff, SeekOrigin.Begin);
                }
                e_shdrtab = new Elf32_Shdr[e_shnum];
                for (int i = 0; i < e_shnum; i++) {
                    byte[] e_sh = new byte[e_shentsize];
                    br.Read(e_sh, 0, e_sh.Length);
                    e_shdrtab[i] = new Elf32_Shdr(e_sh);
                    e_sh = null;
                }

                char[] sh_strtab = null;
                // セクションヘッダ名文字列テーブルを検索
                Encoding enc = Encoding.ASCII;
                foreach (Elf32_Shdr shdr in e_shdrtab) {
                    if (shdr.sh_type == ShType.SHT_STRTAB) {
                        sh_strtab = new char[shdr.sh_size];
                        if (shdr.sh_offset != br.BaseStream.Position) {
                            // 現在のファイル内の位置が sh_offset と違っている場合は、sh_offset までシークする
                            br.BaseStream.Seek(shdr.sh_offset, SeekOrigin.Begin);
                        }
                        br.Read(sh_strtab, 0, sh_strtab.Length);

                        // 読み出したセクションヘッダ名文字列テーブルで自信のセクションヘッダ名をチェック
                        if (new string(sh_strtab.Skip((int)shdr.sh_nameidx).TakeWhile(c => c != '\0').ToArray()).Equals(".shstrtab")) {
                            break;
                        }
                        sh_strtab = null;
                    }
                }

                // 各セクションテーブルにセクション名(string)を付与
                if (sh_strtab != null) {
                    for (int i = 0; i < e_shdrtab.Length; i++) {
                        e_shdrtab[i].sh_name = new string(sh_strtab.Skip((int)e_shdrtab[i].sh_nameidx).TakeWhile(c => c != '\0').ToArray());
                    }
                }

                // シンボルテーブル
                foreach (Elf32_Shdr shdr in e_shdrtab) {
                    if (shdr.sh_type == ShType.SHT_SYMTAB) {

                        // シンボル名文字列テーブル
                        char[] sym_strtab = null;
                        foreach (Elf32_Shdr shdr_strtab in e_shdrtab) {
                            if (shdr_strtab.sh_name.Equals(".strtab")) {
                                sym_strtab = new char[shdr_strtab.sh_size];
                                br.BaseStream.Seek(shdr_strtab.sh_offset, SeekOrigin.Begin);
                                br.Read(sym_strtab, 0, sym_strtab.Length);
                            }

                        }

                        // シンボルを作成
                        List<Elf32_Sym> symtab = new List<Elf32_Sym>();
                        br.BaseStream.Seek(shdr.sh_offset, SeekOrigin.Begin);
                        for (int i = 0; i < shdr.sh_size / shdr.sh_entsize; i++) {
                            byte[] e_sym = new byte[shdr.sh_entsize];
                            br.Read(e_sym, 0, e_sym.Length);
                            symtab.Add(new Elf32_Sym(e_sym, sym_strtab));
                        }
                        sym_strtab = null;

                        // 作成したシンボルのうち、セクションに紐付くものは紐付け
                        List<Elf32_Sym> l = new List<Elf32_Sym>();
                        for (int i = 0; i < e_shdrtab.Length; i++) {
                            for (int j = 0; j < symtab.Count(); j++) {
                                if (symtab[j].st_shndx == i) {
                                    l.Add(symtab[j]);
                                    symtab.RemoveAt(j--);
                                }
                            }
                            e_shdrtab[i].sh_symtab = l.ToArray();
                            l.Clear();
                        }
                        e_symtab = symtab.ToArray();
                        symtab = null;
                    }
                }
            }
        }

        public (byte[], int) GetRelocatedMemory() {

            long maxAddr = -1L, minAddr = -1L;
            
            foreach (Elf32_Shdr sh in e_shdrtab) {
                if((sh.sh_flags & (ShFlag.SHF_WRITE | ShFlag.SHF_ALLOC | ShFlag.SHF_EXECINSTR)) > 0) {
                    if (maxAddr == -1 || maxAddr < (sh.sh_addr + sh.sh_size)) {
                        maxAddr = sh.sh_addr - e_entry + sh.sh_size;
                    }
                    if (minAddr == -1 || minAddr > sh.sh_addr) {
                        minAddr = sh.sh_addr - e_entry;
                    }
                }
            }

            byte[] memory = new byte[maxAddr - minAddr];

            using (BinaryReader br = new BinaryReader(File.Open(this.ProgramFilePath, FileMode.Open))) {
                foreach (Elf32_Shdr sh in e_shdrtab) {
                    if ((sh.sh_flags & (ShFlag.SHF_WRITE | ShFlag.SHF_ALLOC | ShFlag.SHF_EXECINSTR)) > 0 && sh.sh_type == ShType.SHT_PROGBITS) {
                        br.BaseStream.Seek(sh.sh_offset, SeekOrigin.Begin);
                        br.Read(memory, (int)(sh.sh_addr - e_entry), (int)sh.sh_size);
                    }
                }
            }

            return (memory, (int)minAddr);
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
        SHT_HIPROC = 0x7fffffff,
        /// <summary>アプリケーションプログラム使用範囲 開始アドレス</summary>
        SHT_LOUSER = 0x80000000,
        /// <summary>アプリケーションプログラム使用範囲 終了アドレス</summary>
        SHT_HIUSER = 0x8fffffff,
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
        SHF_MASKPROC = 0xf0000000,
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

        /// <summary>シンボルテーブル</summary>
        public Elf32_Sym[] sh_symtab;

        /// <summary>セクションヘッダテーブル</summary>
        public Elf32_Shdr(byte[] bytes) : this() {
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

            sh_symtab = new Elf32_Sym[1];
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
        PT_HIPROC = 0x7fffffff,
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
        public Elf32_Phdr(byte[] bytes) : this() {
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

    public enum Elf32_StBind : byte {
        /// <summary>ローカルシンボル</summary>
        STB_LOCAL = 0,
        /// <summary>グローバルシンボル</summary>
        STB_GLOBAL = 1,
        /// <summary>ウィークシンボル</summary>
        STB_WEAK = 2,
        STB_LOPROC = 13,
        STB_HIPROC = 15,
    }
    public enum ELF32_StType : byte {
        /// <summary>未定義</summary>
        STT_NOTYPE = 0,
        /// <summary>データオブジェクト</summary>
        STT_OBJECT = 1,
        /// <summary>関数または実行コード</summary>
        STT_FUNC = 2,
        /// <summary>セクション</summary>
        STT_SECTION = 3,
        /// <summary>ファイル</summary>
        STT_FILE = 4,
        STT_LOPROC = 13,
        STT_HIPROC = 15,
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
        /// <summary>シンボル属性情報</summary>
        public Elf32_StBind st_info_bind;
        /// <summary>シンボルタイプ</summary>
        public ELF32_StType st_info_type;
        /// <summary>定義なし</summary>
        public byte st_other;
        /// <summary>関連するセクションヘッダーテーブルインデックス値</summary>
        public Elf32_Half st_shndx;

        /// <summary>シンボルテーブル</summary>
        public Elf32_Sym(byte[] bytes, char[] sym_strtab) : this() {
            int ptr = 0;
            byte[] symTable = bytes.ToArray();

            st_nameidx = BitConverter.ToUInt32(symTable, ptr); ptr += 4;
            st_value = BitConverter.ToUInt32(symTable, ptr); ptr += 4;
            st_size = BitConverter.ToUInt32(symTable, ptr); ptr += 4;
            byte st_info = symTable[ptr++];
            st_info_bind = (Elf32_StBind)(st_info >> 4);
            st_info_type = (ELF32_StType)(st_info & 0x0f);
            st_other = symTable[ptr++];
            st_shndx = BitConverter.ToUInt16(symTable, ptr); ptr += 2;

            st_name = new string(sym_strtab?.Skip((int)st_nameidx).TakeWhile(c => c != '\0').ToArray());
        }
    }
}