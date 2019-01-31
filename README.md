# rv32Emulator
[![License](https://img.shields.io/badge/license-BSD--3--Clause-blue.svg)](https://github.com/roy-n-roy/rv32Emulator/blob/master/LICENSE)

RISC-V 32bit ISAを備える、学習を目的としたCPUエミュレーターです。  
RV32標準汎用ISA (=RV32IMAFD) の実装を目標としています。

## 使用ソフトウェア
  C# 7.3 および .NET Framework 4.7

## 動作イメージ
  ![main window](https://github.com/roy-n-roy/rv32Emulator/blob/master/images/main_windows.png?raw=true)

## ビルド
cd rv32Emulator  
X:\Path\To\MSBuild.exe RV32Emulator.sln /t:RISC-V_CPU_Emulator /p:Configuration=Release

## ライセンス
  BSD 3-Clause License

## 履歴
\(2019/3/31 v0.1\)  
  ![riscv-tests](https://github.com/riscv/riscv-tests) のうち、rv32\[msu\]\[imafdc\]-p-\* (シングルコア、仮想メモリ無効)でのテストに成功

## 参考文献
[1]  "RISC-V原典(リスクファイブ原典) オープン・アーキテクチャのススメ 第1版", 著者 デイビッド･パターソン, アンドリュー･ウォーターマン,  訳者 成田 光彰, 発行 日経BP社, 2018/10/22  
[2] ["The RISC-V Instruction Set Manual, Volume I: User-Level ISA, Document Version2.2"](https://riscv.org/specifications/), Editors Andrew Waterman and Krste Asanovic, RISC-V Foundation, May 2017.  
[3] ["RISC-V命令セットマニュアル 第一巻：ユーザーレベルISA 文書 2.2 版"](https://github.com/shibatchii/RISC-V/blob/master/RISC-V_spec_manual_v2.2_jp.pdf) 日本語訳 @shibatchii  
[4] [”The RISC-V Instruction Set Manual, Volume II: Privileged Architecture, Version1.10"](https://riscv.org/specifications/)), Editors Andrew Waterman and Krste Asanovi#c, RISC-V Foundation, May 2017.  
[5] ["RISC-V命令セットマニュアル 第二巻：特権付きアーキテクチャ、文書 1.10 版"](https://github.com/shibatchii/RISC-V/blob/master/riscv-privileged-v1.10_jp.pdf) 日本語訳 @shibatchii
