namespace nAble
{
    partial class FormPumpPurge
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
			this.labelFluidSyringePurgeRechargeRateUnits = new System.Windows.Forms.Label();
			this.labelFluidSyringePurgeRechargeRate = new System.Windows.Forms.Label();
			this.labelFluidSyringePurgeRateUnits = new System.Windows.Forms.Label();
			this.labelFluidSyringePurgeRate = new System.Windows.Forms.Label();
			this.buttonFluidSyringePurgeRechargeRate = new System.Windows.Forms.Button();
			this.buttonFluidSyringePurgeRate = new System.Windows.Forms.Button();
			this.buttonSyringPurgeStart = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// labelFluidSyringePurgeRechargeRateUnits
			// 
			this.labelFluidSyringePurgeRechargeRateUnits.AutoSize = true;
			this.labelFluidSyringePurgeRechargeRateUnits.ForeColor = System.Drawing.Color.White;
			this.labelFluidSyringePurgeRechargeRateUnits.Location = new System.Drawing.Point(315, 96);
			this.labelFluidSyringePurgeRechargeRateUnits.Name = "labelFluidSyringePurgeRechargeRateUnits";
			this.labelFluidSyringePurgeRechargeRateUnits.Size = new System.Drawing.Size(52, 29);
			this.labelFluidSyringePurgeRechargeRateUnits.TabIndex = 83;
			this.labelFluidSyringePurgeRechargeRateUnits.Text = "µl/s";
			// 
			// labelFluidSyringePurgeRechargeRate
			// 
			this.labelFluidSyringePurgeRechargeRate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelFluidSyringePurgeRechargeRate.AutoSize = true;
			this.labelFluidSyringePurgeRechargeRate.ForeColor = System.Drawing.Color.White;
			this.labelFluidSyringePurgeRechargeRate.Location = new System.Drawing.Point(16, 96);
			this.labelFluidSyringePurgeRechargeRate.Name = "labelFluidSyringePurgeRechargeRate";
			this.labelFluidSyringePurgeRechargeRate.Size = new System.Drawing.Size(178, 29);
			this.labelFluidSyringePurgeRechargeRate.TabIndex = 81;
			this.labelFluidSyringePurgeRechargeRate.Text = "Recharge Rate:";
			this.labelFluidSyringePurgeRechargeRate.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// labelFluidSyringePurgeRateUnits
			// 
			this.labelFluidSyringePurgeRateUnits.AutoSize = true;
			this.labelFluidSyringePurgeRateUnits.ForeColor = System.Drawing.Color.White;
			this.labelFluidSyringePurgeRateUnits.Location = new System.Drawing.Point(315, 26);
			this.labelFluidSyringePurgeRateUnits.Name = "labelFluidSyringePurgeRateUnits";
			this.labelFluidSyringePurgeRateUnits.Size = new System.Drawing.Size(52, 29);
			this.labelFluidSyringePurgeRateUnits.TabIndex = 77;
			this.labelFluidSyringePurgeRateUnits.Text = "µl/s";
			// 
			// labelFluidSyringePurgeRate
			// 
			this.labelFluidSyringePurgeRate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelFluidSyringePurgeRate.AutoSize = true;
			this.labelFluidSyringePurgeRate.ForeColor = System.Drawing.Color.White;
			this.labelFluidSyringePurgeRate.Location = new System.Drawing.Point(21, 26);
			this.labelFluidSyringePurgeRate.Name = "labelFluidSyringePurgeRate";
			this.labelFluidSyringePurgeRate.Size = new System.Drawing.Size(173, 29);
			this.labelFluidSyringePurgeRate.TabIndex = 73;
			this.labelFluidSyringePurgeRate.Text = "Dispense Rate:";
			this.labelFluidSyringePurgeRate.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// buttonFluidSyringePurgeRechargeRate
			// 
			this.buttonFluidSyringePurgeRechargeRate.Location = new System.Drawing.Point(198, 92);
			this.buttonFluidSyringePurgeRechargeRate.Name = "buttonFluidSyringePurgeRechargeRate";
			this.buttonFluidSyringePurgeRechargeRate.Size = new System.Drawing.Size(108, 37);
			this.buttonFluidSyringePurgeRechargeRate.TabIndex = 84;
			this.buttonFluidSyringePurgeRechargeRate.Text = "...";
			this.buttonFluidSyringePurgeRechargeRate.UseVisualStyleBackColor = true;
			this.buttonFluidSyringePurgeRechargeRate.Click += new System.EventHandler(this.buttonFluidSyringePurgeRechargeRate_Click);
			// 
			// buttonFluidSyringePurgeRate
			// 
			this.buttonFluidSyringePurgeRate.Location = new System.Drawing.Point(198, 22);
			this.buttonFluidSyringePurgeRate.Name = "buttonFluidSyringePurgeRate";
			this.buttonFluidSyringePurgeRate.Size = new System.Drawing.Size(108, 37);
			this.buttonFluidSyringePurgeRate.TabIndex = 79;
			this.buttonFluidSyringePurgeRate.Text = "...";
			this.buttonFluidSyringePurgeRate.UseVisualStyleBackColor = true;
			this.buttonFluidSyringePurgeRate.Click += new System.EventHandler(this.buttonFluidSyringePurgeRate_Click);
			// 
			// buttonSyringPurgeStart
			// 
			this.buttonSyringPurgeStart.Location = new System.Drawing.Point(79, 220);
			this.buttonSyringPurgeStart.Name = "buttonSyringPurgeStart";
			this.buttonSyringPurgeStart.Size = new System.Drawing.Size(262, 78);
			this.buttonSyringPurgeStart.TabIndex = 72;
			this.buttonSyringPurgeStart.Text = "Start Pump Purge";
			this.buttonSyringPurgeStart.UseVisualStyleBackColor = true;
			this.buttonSyringPurgeStart.Click += new System.EventHandler(this.buttonSyringPurgeStart_Click);
			// 
			// FormPumpPurge
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.BackColor = System.Drawing.Color.SteelBlue;
			this.ClientSize = new System.Drawing.Size(420, 310);
			this.Controls.Add(this.buttonFluidSyringePurgeRechargeRate);
			this.Controls.Add(this.labelFluidSyringePurgeRechargeRateUnits);
			this.Controls.Add(this.labelFluidSyringePurgeRechargeRate);
			this.Controls.Add(this.buttonFluidSyringePurgeRate);
			this.Controls.Add(this.labelFluidSyringePurgeRateUnits);
			this.Controls.Add(this.labelFluidSyringePurgeRate);
			this.Controls.Add(this.buttonSyringPurgeStart);
			this.Font = new System.Drawing.Font("Tahoma", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Margin = new System.Windows.Forms.Padding(8);
			this.Name = "FormPumpPurge";
			this.Load += new System.EventHandler(this.FormPumpPurge_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonFluidSyringePurgeRechargeRate;
        private System.Windows.Forms.Button buttonFluidSyringePurgeRate;
        private System.Windows.Forms.Button buttonSyringPurgeStart;
        private System.Windows.Forms.Label labelFluidSyringePurgeRechargeRateUnits;
        private System.Windows.Forms.Label labelFluidSyringePurgeRechargeRate;
        private System.Windows.Forms.Label labelFluidSyringePurgeRateUnits;
        private System.Windows.Forms.Label labelFluidSyringePurgeRate;
    }
}