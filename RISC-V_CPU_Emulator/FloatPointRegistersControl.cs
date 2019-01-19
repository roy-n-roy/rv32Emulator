using RISC_V_Instruction;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using appRes = global::RISC_V_CPU_Emulator.Properties.Resources;

namespace RISC_V_CPU_Emulator {
    public partial class FloatPointRegistersControl : RegisterViewerForm.RegisterControl {

        /// <summary></summary>
        private readonly Label[] FloatPointRegisterLabels;
        /// <summary></summary>
        private TextBox[] FloatPointRegisterTextBoxes;

        public FloatPointRegistersControl() {
            InitializeComponent();

            #region 表示用ラベル配列・テキストボックス配列初期化

            // 浮動小数点レジスタ名ラベル
            FloatPointRegisterLabels = new Label[] {
                FloatPointRegisterLabel0, FloatPointRegisterLabel1, FloatPointRegisterLabel2, FloatPointRegisterLabel3, FloatPointRegisterLabel4,
                FloatPointRegisterLabel5, FloatPointRegisterLabel6, FloatPointRegisterLabel7, FloatPointRegisterLabel8, FloatPointRegisterLabel9,
                FloatPointRegisterLabel10, FloatPointRegisterLabel11, FloatPointRegisterLabel12, FloatPointRegisterLabel13, FloatPointRegisterLabel14,
                FloatPointRegisterLabel15, FloatPointRegisterLabel16, FloatPointRegisterLabel17, FloatPointRegisterLabel18, FloatPointRegisterLabel19,
                FloatPointRegisterLabel20, FloatPointRegisterLabel21, FloatPointRegisterLabel22, FloatPointRegisterLabel23, FloatPointRegisterLabel24,
                FloatPointRegisterLabel25, FloatPointRegisterLabel26, FloatPointRegisterLabel27, FloatPointRegisterLabel28, FloatPointRegisterLabel29,
                FloatPointRegisterLabel30, FloatPointRegisterLabel31
            };

            // 浮動小数点レジスタ値テキストボックス
            FloatPointRegisterTextBoxes = new TextBox[] {
                FloatPointRegisterTextBox0, FloatPointRegisterTextBox1, FloatPointRegisterTextBox2, FloatPointRegisterTextBox3, FloatPointRegisterTextBox4,
                FloatPointRegisterTextBox5, FloatPointRegisterTextBox6, FloatPointRegisterTextBox7, FloatPointRegisterTextBox8, FloatPointRegisterTextBox9,
                FloatPointRegisterTextBox10, FloatPointRegisterTextBox11, FloatPointRegisterTextBox12, FloatPointRegisterTextBox13, FloatPointRegisterTextBox14,
                FloatPointRegisterTextBox15, FloatPointRegisterTextBox16, FloatPointRegisterTextBox17, FloatPointRegisterTextBox18, FloatPointRegisterTextBox19,
                FloatPointRegisterTextBox20, FloatPointRegisterTextBox21, FloatPointRegisterTextBox22, FloatPointRegisterTextBox23, FloatPointRegisterTextBox24,
                FloatPointRegisterTextBox25, FloatPointRegisterTextBox26, FloatPointRegisterTextBox27, FloatPointRegisterTextBox28, FloatPointRegisterTextBox29,
                FloatPointRegisterTextBox30, FloatPointRegisterTextBox31
            };

            #endregion

            InitializeRegisterNameLabels();
        }

        /// <summary>
        /// 浮動小数点レジスタ名のテキストラベルを初期化する
        /// </summary>
        private void InitializeRegisterNameLabels() {
            for (int i = 0; i < 32; i++) {
                FloatPointRegisterLabels[i].Text = RISC_V_Instruction.InstConverter.FloatPointRegisterName[i];
            }
        }

        /// <summary>
        /// ステップ実行前の命令を引数として、
        /// 前回の実行時に書き換えられたレジスタ値テキストボックスの文字色を変更する
        /// </summary>
        /// <param name="ins"></param>
        internal override void UpdateRegisterBeforeExecute(RiscvInstruction ins) {

            InstructionViewerForm parent = ((InstructionViewerForm)this.Parent);

            // テキストボックスの文字色を黒に戻す
            for (int i = 0; i < 16; i++) {
                FloatPointRegisterTextBoxes[i].ForeColor = global::RISC_V_CPU_Emulator.InstructionViewerForm.DefaultTextColor;
            }

            // 
            foreach (string key in ins.Arguments.Keys) {
                TextBox target = null;
                                    // 浮動小数点レジスタ名
                if (key.Equals("frd")) {
                    target = FloatPointRegisterTextBoxes[(int)ins.Arguments[key]];
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
                if ((registers["f" + i] & 0xffff_ffff_0000_0000U) == 0xffff_ffff_0000_0000U) {
                    FloatPointRegisterTextBoxes[i].Text = BitConverter.ToSingle(BitConverter.GetBytes(registers["f" + i]), 0).ToString("0.0#");
                } else {
                    FloatPointRegisterTextBoxes[i].Text = BitConverter.ToDouble(BitConverter.GetBytes(registers["f" + i]), 0).ToString("0.0#");
                }
            }

            // テキストボックスの背景を白に戻す
            for (int i = 0; i < 16; i++) {
                FloatPointRegisterTextBoxes[i].BackColor = global::RISC_V_CPU_Emulator.InstructionViewerForm.DefaultTextBoxBackColor;
            }

            // レジスタ名がRISC-V命令の引数内にあるか確認する
            foreach (string key in ins.Arguments.Keys) {
                TextBox target = null;


                    // 浮動小数点レジスタ名
                if (key.Equals("frd") || key.StartsWith("frs")) {
                    target = FloatPointRegisterTextBoxes[(int)ins.Arguments[key]];
                }

                Color color = Color.FromName(appRes.ResourceManager.GetString("RegisterLabel_BackColor_" + key) ?? "");

                if (!(target is null) && color != Color.Black && target.BackColor.Equals(global::RISC_V_CPU_Emulator.InstructionViewerForm.DefaultTextBoxBackColor)) {
                    target.BackColor = color;
                }
            }

            string frm = appRes.ResourceManager.GetString("FloatPointStatusRegisterTextBox_RoundMode_" + ((registers["fcsr"] & 0xe0U) >> 5).ToString("X").ToLower());
            string fflag = appRes.ResourceManager.GetString("FloatPointStatusRegisterTextBox_ExceptionFlag_" + (registers["fcsr"] & 0x1fU).ToString("X").ToLower().PadLeft(2, '0'));
            this.FloatPointStatusRegisterTextBox.Text = " " + (frm is null ? "" : frm) + (fflag is null ? "" : " + " + fflag);
        }


    }
}
