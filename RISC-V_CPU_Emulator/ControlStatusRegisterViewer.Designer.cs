namespace RISC_V_CPU_Emulator {
    partial class ControlStatusRegisterViewer {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.ControlStatusRegistersControl = new RISC_V_CPU_Emulator.ControlStatusRegistersControl();
            this.SuspendLayout();
            // 
            // ControlStatusRegistersControl
            // 
            this.ControlStatusRegistersControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ControlStatusRegistersControl.Font = new System.Drawing.Font("Yu Gothic UI", 9F);
            this.ControlStatusRegistersControl.Location = new System.Drawing.Point(12, 13);
            this.ControlStatusRegistersControl.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.ControlStatusRegistersControl.Name = "ControlStatusRegistersControl";
            this.ControlStatusRegistersControl.Size = new System.Drawing.Size(510, 631);
            this.ControlStatusRegistersControl.TabIndex = 0;
            // 
            // ControlStatusRegisterViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(534, 661);
            this.Controls.Add(this.ControlStatusRegistersControl);
            this.Name = "ControlStatusRegisterViewer";
            this.Text = "ControlStatusRegisterViewer";
            this.ResumeLayout(false);

        }

        #endregion

        internal ControlStatusRegistersControl ControlStatusRegistersControl;
    }
}