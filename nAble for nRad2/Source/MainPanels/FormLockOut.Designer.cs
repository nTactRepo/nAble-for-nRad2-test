namespace nAble
{
    partial class FormLockOut
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormLockOut));
            this.labelWarning = new System.Windows.Forms.Label();
            this.labelInfo = new System.Windows.Forms.Label();
            this.linkLabelWebsite = new System.Windows.Forms.LinkLabel();
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBoxLogo = new System.Windows.Forms.PictureBox();
            this.labelLockoutCode = new System.Windows.Forms.Label();
            this.buttonLicense = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // labelWarning
            // 
            this.labelWarning.Font = new System.Drawing.Font("Tahoma", 27.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelWarning.ForeColor = System.Drawing.Color.Red;
            this.labelWarning.Location = new System.Drawing.Point(250, 158);
            this.labelWarning.Name = "labelWarning";
            this.labelWarning.Size = new System.Drawing.Size(524, 117);
            this.labelWarning.TabIndex = 0;
            this.labelWarning.Text = "Software Disabled Due to Licensing Restrictions";
            this.labelWarning.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelInfo
            // 
            this.labelInfo.Location = new System.Drawing.Point(330, 262);
            this.labelInfo.Name = "labelInfo";
            this.labelInfo.Size = new System.Drawing.Size(365, 117);
            this.labelInfo.TabIndex = 2;
            this.labelInfo.Text = "Please contact nTact Support for further assistance.";
            this.labelInfo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // linkLabelWebsite
            // 
            this.linkLabelWebsite.AutoSize = true;
            this.linkLabelWebsite.Location = new System.Drawing.Point(409, 377);
            this.linkLabelWebsite.Name = "linkLabelWebsite";
            this.linkLabelWebsite.Size = new System.Drawing.Size(207, 33);
            this.linkLabelWebsite.TabIndex = 3;
            this.linkLabelWebsite.TabStop = true;
            this.linkLabelWebsite.Text = "www.nTact.com";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(388, 434);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(246, 33);
            this.label1.TabIndex = 4;
            this.label1.Text = "+1 (214) 343-5300";
            // 
            // pictureBoxLogo
            // 
            this.pictureBoxLogo.Image = Resources.ntact;
            this.pictureBoxLogo.Location = new System.Drawing.Point(365, 12);
            this.pictureBoxLogo.Name = "pictureBoxLogo";
            this.pictureBoxLogo.Size = new System.Drawing.Size(294, 146);
            this.pictureBoxLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxLogo.TabIndex = 19;
            this.pictureBoxLogo.TabStop = false;
            // 
            // labelLockoutCode
            // 
            this.labelLockoutCode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.labelLockoutCode.AutoSize = true;
            this.labelLockoutCode.Font = new System.Drawing.Font("Tahoma", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelLockoutCode.Location = new System.Drawing.Point(859, 469);
            this.labelLockoutCode.Name = "labelLockoutCode";
            this.labelLockoutCode.Size = new System.Drawing.Size(127, 33);
            this.labelLockoutCode.TabIndex = 20;
            this.labelLockoutCode.Text = "{{Code}}";
            this.labelLockoutCode.Visible = false;
            // 
            // buttonLicense
            // 
            this.buttonLicense.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonLicense.BackgroundImage")));
            this.buttonLicense.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.buttonLicense.Location = new System.Drawing.Point(12, 429);
            this.buttonLicense.Name = "buttonLicense";
            this.buttonLicense.Size = new System.Drawing.Size(87, 73);
            this.buttonLicense.TabIndex = 21;
            this.buttonLicense.UseVisualStyleBackColor = true;
            this.buttonLicense.Click += new System.EventHandler(this.buttonLicense_Click);
            // 
            // FormLockOut
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(15F, 33F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(1024, 514);
            this.Controls.Add(this.buttonLicense);
            this.Controls.Add(this.labelLockoutCode);
            this.Controls.Add(this.pictureBoxLogo);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.linkLabelWebsite);
            this.Controls.Add(this.labelInfo);
            this.Controls.Add(this.labelWarning);
            this.Font = new System.Drawing.Font("Tahoma", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(8);
            this.Name = "FormLockOut";
            this.Text = "FormLockOut";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelWarning;
        private System.Windows.Forms.Label labelInfo;
        private System.Windows.Forms.LinkLabel linkLabelWebsite;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictureBoxLogo;
        private System.Windows.Forms.Label labelLockoutCode;
        private System.Windows.Forms.Button buttonLicense;
    }
}