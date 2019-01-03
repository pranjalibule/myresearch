namespace SQS.nTier.TTM.UserRoleDataImport
{
    partial class frmUserRoleDataImport
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnClear = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnSelectFile = new System.Windows.Forms.Button();
            this.txtSelectFile = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.openFileDialogTTM = new System.Windows.Forms.OpenFileDialog();
            this.grpReport = new System.Windows.Forms.GroupBox();
            this.lblRecordProcessed = new System.Windows.Forms.Label();
            this.lblNumOfRows = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.grpReport.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(424, 57);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 23);
            this.btnClear.TabIndex = 11;
            this.btnClear.Text = "C&lear";
            this.btnClear.UseVisualStyleBackColor = true;
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(505, 57);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 10;
            this.btnStart.Text = "&Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnSelectFile
            // 
            this.btnSelectFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSelectFile.Location = new System.Drawing.Point(587, 30);
            this.btnSelectFile.Name = "btnSelectFile";
            this.btnSelectFile.Size = new System.Drawing.Size(26, 23);
            this.btnSelectFile.TabIndex = 9;
            this.btnSelectFile.Text = "&..";
            this.btnSelectFile.UseVisualStyleBackColor = true;
            this.btnSelectFile.Click += new System.EventHandler(this.btnSelectFile_Click);
            // 
            // txtSelectFile
            // 
            this.txtSelectFile.Enabled = false;
            this.txtSelectFile.Location = new System.Drawing.Point(87, 31);
            this.txtSelectFile.Name = "txtSelectFile";
            this.txtSelectFile.Size = new System.Drawing.Size(493, 20);
            this.txtSelectFile.TabIndex = 8;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Select File";
            // 
            // openFileDialogTTM
            // 
            this.openFileDialogTTM.Filter = "Excel File | *.xls;*.xlsx;";
            this.openFileDialogTTM.FileOk += new System.ComponentModel.CancelEventHandler(this.openFileDialogTTM_FileOk);
            // 
            // grpReport
            // 
            this.grpReport.Controls.Add(this.lblRecordProcessed);
            this.grpReport.Controls.Add(this.lblNumOfRows);
            this.grpReport.Controls.Add(this.label3);
            this.grpReport.Controls.Add(this.label2);
            this.grpReport.Location = new System.Drawing.Point(55, 89);
            this.grpReport.Name = "grpReport";
            this.grpReport.Size = new System.Drawing.Size(605, 94);
            this.grpReport.TabIndex = 12;
            this.grpReport.TabStop = false;
            this.grpReport.Text = "Report (# of #)";
            // 
            // lblRecordProcessed
            // 
            this.lblRecordProcessed.AutoSize = true;
            this.lblRecordProcessed.ForeColor = System.Drawing.Color.Green;
            this.lblRecordProcessed.Location = new System.Drawing.Point(137, 50);
            this.lblRecordProcessed.Name = "lblRecordProcessed";
            this.lblRecordProcessed.Size = new System.Drawing.Size(14, 13);
            this.lblRecordProcessed.TabIndex = 4;
            this.lblRecordProcessed.Text = "#";
            // 
            // lblNumOfRows
            // 
            this.lblNumOfRows.AutoSize = true;
            this.lblNumOfRows.ForeColor = System.Drawing.Color.Green;
            this.lblNumOfRows.Location = new System.Drawing.Point(162, 22);
            this.lblNumOfRows.Name = "lblNumOfRows";
            this.lblNumOfRows.Size = new System.Drawing.Size(14, 13);
            this.lblNumOfRows.TabIndex = 3;
            this.lblNumOfRows.Text = "#";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 50);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(125, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Total Record Processed:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(150, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Total Number of rows Present:";
            // 
            // frmUserRoleDataImport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(714, 206);
            this.Controls.Add(this.grpReport);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.btnSelectFile);
            this.Controls.Add(this.txtSelectFile);
            this.Controls.Add(this.label1);
            this.Name = "frmUserRoleDataImport";
            this.Text = "frmUserRoleDataImport";
            this.grpReport.ResumeLayout(false);
            this.grpReport.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnSelectFile;
        private System.Windows.Forms.TextBox txtSelectFile;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.OpenFileDialog openFileDialogTTM;
        private System.Windows.Forms.GroupBox grpReport;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblRecordProcessed;
        private System.Windows.Forms.Label lblNumOfRows;
    }
}