namespace nAble
{
    partial class FormAlarms
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
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
			this.buttonClose = new System.Windows.Forms.Button();
			this.dataGridViewActivityHistory = new System.Windows.Forms.DataGridView();
			this.buttonResetAlarms = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.dataGridViewActivityHistory)).BeginInit();
			this.SuspendLayout();
			// 
			// buttonClose
			// 
			this.buttonClose.Location = new System.Drawing.Point(432, 450);
			this.buttonClose.Name = "buttonClose";
			this.buttonClose.Size = new System.Drawing.Size(160, 55);
			this.buttonClose.TabIndex = 90;
			this.buttonClose.Text = "Close";
			this.buttonClose.UseVisualStyleBackColor = true;
			this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
			// 
			// dataGridViewActivityHistory
			// 
			this.dataGridViewActivityHistory.AllowUserToAddRows = false;
			this.dataGridViewActivityHistory.AllowUserToDeleteRows = false;
			this.dataGridViewActivityHistory.AllowUserToResizeColumns = false;
			this.dataGridViewActivityHistory.AllowUserToResizeRows = false;
			dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle1.Font = new System.Drawing.Font("Tahoma", 20.25F);
			dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.dataGridViewActivityHistory.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
			this.dataGridViewActivityHistory.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
			dataGridViewCellStyle2.Font = new System.Drawing.Font("Tahoma", 20.25F);
			dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
			dataGridViewCellStyle2.Padding = new System.Windows.Forms.Padding(0, 0, 0, 1);
			dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.dataGridViewActivityHistory.DefaultCellStyle = dataGridViewCellStyle2;
			this.dataGridViewActivityHistory.Dock = System.Windows.Forms.DockStyle.Top;
			this.dataGridViewActivityHistory.Location = new System.Drawing.Point(0, 0);
			this.dataGridViewActivityHistory.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.dataGridViewActivityHistory.MultiSelect = false;
			this.dataGridViewActivityHistory.Name = "dataGridViewActivityHistory";
			this.dataGridViewActivityHistory.ReadOnly = true;
			this.dataGridViewActivityHistory.RowHeadersVisible = false;
			dataGridViewCellStyle3.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.dataGridViewActivityHistory.RowsDefaultCellStyle = dataGridViewCellStyle3;
			this.dataGridViewActivityHistory.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.dataGridViewActivityHistory.RowTemplate.Height = 24;
			this.dataGridViewActivityHistory.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.dataGridViewActivityHistory.Size = new System.Drawing.Size(1024, 441);
			this.dataGridViewActivityHistory.TabIndex = 91;
			// 
			// buttonResetAlarms
			// 
			this.buttonResetAlarms.Location = new System.Drawing.Point(60, 450);
			this.buttonResetAlarms.Name = "buttonResetAlarms";
			this.buttonResetAlarms.Size = new System.Drawing.Size(271, 55);
			this.buttonResetAlarms.TabIndex = 92;
			this.buttonResetAlarms.Text = "Reset Stack Alarms";
			this.buttonResetAlarms.UseVisualStyleBackColor = true;
			this.buttonResetAlarms.Visible = false;
			this.buttonResetAlarms.Click += new System.EventHandler(this.buttonResetAlarms_Click);
			// 
			// FormAlarms
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(15F, 33F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1024, 514);
			this.Controls.Add(this.buttonResetAlarms);
			this.Controls.Add(this.dataGridViewActivityHistory);
			this.Controls.Add(this.buttonClose);
			this.Font = new System.Drawing.Font("Tahoma", 20.25F);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Margin = new System.Windows.Forms.Padding(8);
			this.Name = "FormAlarms";
			this.Text = "Alarms";
			this.Load += new System.EventHandler(this.FormAlarms_Load);
			((System.ComponentModel.ISupportInitialize)(this.dataGridViewActivityHistory)).EndInit();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.DataGridView dataGridViewActivityHistory;
		private System.Windows.Forms.Button buttonResetAlarms;
	}
}