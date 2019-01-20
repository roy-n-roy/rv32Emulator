using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RISC_V_Instruction;
using appRes = global::RISC_V_CPU_Emulator.Properties.Resources;

namespace RISC_V_CPU_Emulator {
    public partial class IntegerRegistersControl : RegisterViewerForm.RegisterControl {

        /// <summary></summary>
        private readonly Label[] IntegerRegisterLabels;
        /// <summary></summary>
        private TextBox[] IntegerRegisterTextBoxes;

        public IntegerRegistersControl() {

            InitializeComponent();

            #region 表示用ラベル配列・テキストボックス配列初期化
            
            // 整数レジスタ名ラベル
            IntegerRegisterLabels = new Label[] {
                IntegerRegisterLabel0, IntegerRegisterLabel1, IntegerRegisterLabel2, IntegerRegisterLabel3, IntegerRegisterLabel4,
                IntegerRegisterLabel5, IntegerRegisterLabel6, IntegerRegisterLabel7, IntegerRegisterLabel8, IntegerRegisterLabel9,
                IntegerRegisterLabel10, IntegerRegisterLabel11, IntegerRegisterLabel12, IntegerRegisterLabel13, IntegerRegisterLabel14,
                IntegerRegisterLabel15, IntegerRegisterLabel16, IntegerRegisterLabel17, IntegerRegisterLabel18, IntegerRegisterLabel19,
                IntegerRegisterLabel20, IntegerRegisterLabel21, IntegerRegisterLabel22, IntegerRegisterLabel23, IntegerRegisterLabel24,
                IntegerRegisterLabel25, IntegerRegisterLabel26, IntegerRegisterLabel27, IntegerRegisterLabel28, IntegerRegisterLabel29,
                IntegerRegisterLabel30, IntegerRegisterLabel31
            };

            // 整数レジスタ値テキストボックス
            IntegerRegisterTextBoxes = new TextBox[] {
                IntegerRegisterTextBox0, IntegerRegisterTextBox1, IntegerRegisterTextBox2, IntegerRegisterTextBox3, IntegerRegisterTextBox4,
                IntegerRegisterTextBox5, IntegerRegisterTextBox6, IntegerRegisterTextBox7, IntegerRegisterTextBox8, IntegerRegisterTextBox9,
                IntegerRegisterTextBox10, IntegerRegisterTextBox11, IntegerRegisterTextBox12, IntegerRegisterTextBox13, IntegerRegisterTextBox14,
                IntegerRegisterTextBox15, IntegerRegisterTextBox16, IntegerRegisterTextBox17, IntegerRegisterTextBox18, IntegerRegisterTextBox19,
                IntegerRegisterTextBox20, IntegerRegisterTextBox21, IntegerRegisterTextBox22, IntegerRegisterTextBox23, IntegerRegisterTextBox24,
                IntegerRegisterTextBox25, IntegerRegisterTextBox26, IntegerRegisterTextBox27, IntegerRegisterTextBox28, IntegerRegisterTextBox29,
                IntegerRegisterTextBox30, IntegerRegisterTextBox31
            };

            #endregion

            InitializeRegisterNameLabels();
        }

        /// <summary>
        /// 整数・浮動小数点レジスタ名のテキストラベルを初期化する
        /// </summary>
        private void InitializeRegisterNameLabels() {
            for (int i = 0; i < 32; i++) {
                IntegerRegisterLabels[i].Text = RISC_V_Instruction.InstConverter.IntergerRegisterName[i];
            }
        }

        /// <summary>
        /// ステップ実行前の命令を引数として、
        /// 前回の実行時に書き換えられたレジスタ値テキストボックスの文字色を変更する
        /// </summary>
        /// <param name="ins"></param>
        internal override void UpdateRegisterBeforeExecute(RiscvInstruction ins) {
            // テキストボックスの文字色を黒に戻す
            for (int i = 0; i < 16; i++) {
                IntegerRegisterTextBoxes[i].ForeColor = global::RISC_V_CPU_Emulator.InstructionViewerForm.DefaultTextColor;
            }

            // 
            foreach (string key in ins.Arguments.Keys) {
                TextBox target = null;
                if (key.Equals("rd")) {
                    target = IntegerRegisterTextBoxes[(int)ins.Arguments[key]];
                }

                Color color = Color.FromName(appRes.ResourceManager.GetString("RegisterLabel_ForeColor_" + key) ?? "");

                if (!(target is null) && color != Color.Black) {
                    target.ForeColor = color;
                }
            }
        }
        /// <summary>
        /// レジスタ値テキストボックスを更新する
        /// </summary>
        internal override void UpdateRegisterData(RiscvInstruction ins, Dictionary<string, ulong> registers) {

            // テキストボックスの値を更新する
            for (int i = 0; i < 32; i++) {
                IntegerRegisterTextBoxes[i].Text = "0x" + ((int)registers["x" + i]).ToString("X").PadLeft(8, '0');
            }

            // テキストボックスの背景を白に戻す
            for (int i = 0; i < 16; i++) {
                IntegerRegisterTextBoxes[i].BackColor = global::RISC_V_CPU_Emulator.InstructionViewerForm.DefaultTextBoxBackColor;
            }

            // レジスタ名がRISC-V命令の引数内にあるか確認する
            foreach (string key in ins.Arguments.Keys) {
                TextBox target = null;

                if (key.Equals("rd") || key.StartsWith("rs")) {
                    target = IntegerRegisterTextBoxes[(int)ins.Arguments[key]];
                }

                Color color = Color.FromName(appRes.ResourceManager.GetString("RegisterLabel_BackColor_" + key) ?? "");

                if (!(target is null) && color != Color.Black && target.BackColor.Equals(global::RISC_V_CPU_Emulator.InstructionViewerForm.DefaultTextBoxBackColor)) {
                    target.BackColor = color;
                }
            }

        }
    }
}
