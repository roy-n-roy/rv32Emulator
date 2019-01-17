namespace RISC_V_CPU_Emulator {
    partial class ControlStatusRegistersControl {
        /// <summary> 
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region コンポーネント デザイナーで生成されたコード

        /// <summary> 
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を 
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ControlStatusRegistersControl));
            this.CSRDataGrid = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.CSRDataGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // CSRDataGrid
            // 
            this.CSRDataGrid.AllowUserToAddRows = false;
            this.CSRDataGrid.AllowUserToDeleteRows = false;
            resources.ApplyResources(this.CSRDataGrid, "CSRDataGrid");
            this.CSRDataGrid.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            this.CSRDataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.CSRDataGrid.Name = "CSRDataGrid";
            this.CSRDataGrid.RowTemplate.Height = 21;
            // 
            // ControlStatusRegistersControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.CSRDataGrid);
            this.Name = "ControlStatusRegistersControl";
            ((System.ComponentModel.ISupportInitialize)(this.CSRDataGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.DataGridView CSRDataGrid;
    }
}
