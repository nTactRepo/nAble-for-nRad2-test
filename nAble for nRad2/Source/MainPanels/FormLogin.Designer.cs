namespace nAble
{
    partial class FormLogin
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormLogin));
            this.labelLogin = new System.Windows.Forms.Label();
            this.textBoxLogin = new System.Windows.Forms.TextBox();
            this.buttonLoginEnter = new System.Windows.Forms.Button();
            this.buttonLogin0 = new System.Windows.Forms.Button();
            this.buttonLogin9 = new System.Windows.Forms.Button();
            this.buttonLogin8 = new System.Windows.Forms.Button();
            this.buttonLogin7 = new System.Windows.Forms.Button();
            this.buttonLogin6 = new System.Windows.Forms.Button();
            this.buttonLogin5 = new System.Windows.Forms.Button();
            this.buttonLogin4 = new System.Windows.Forms.Button();
            this.buttonLogin3 = new System.Windows.Forms.Button();
            this.buttonLogin2 = new System.Windows.Forms.Button();
            this.buttonLogin1 = new System.Windows.Forms.Button();
            this.buttonLoginClear = new System.Windows.Forms.Button();
            this.labelLoginError = new System.Windows.Forms.Label();
            this.buttonLicense = new System.Windows.Forms.Button();
            this.ucTrialStatus = new Support2.Forms.UCTrialStatus();
            this.SuspendLayout();
            // 
            // labelLogin
            // 
            this.labelLogin.ForeColor = System.Drawing.SystemColors.WindowText;
            this.labelLogin.Location = new System.Drawing.Point(163, 55);
            this.labelLogin.Name = "labelLogin";
            this.labelLogin.Size = new System.Drawing.Size(295, 82);
            this.labelLogin.TabIndex = 13;
            this.labelLogin.Text = "Enter User Access Code";
            this.labelLogin.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // textBoxLogin
            // 
            this.textBoxLogin.BackColor = System.Drawing.Color.Black;
            this.textBoxLogin.Font = new System.Drawing.Font("Tahoma", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxLogin.ForeColor = System.Drawing.Color.Yellow;
            this.textBoxLogin.Location = new System.Drawing.Point(196, 137);
            this.textBoxLogin.MaxLength = 16;
            this.textBoxLogin.Name = "textBoxLogin";
            this.textBoxLogin.PasswordChar = '*';
            this.textBoxLogin.Size = new System.Drawing.Size(223, 43);
            this.textBoxLogin.TabIndex = 0;
            this.textBoxLogin.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBoxLogin.Enter += new System.EventHandler(this.textBoxLogin_Enter);
            // 
            // buttonLoginEnter
            // 
            this.buttonLoginEnter.Location = new System.Drawing.Point(851, 358);
            this.buttonLoginEnter.Name = "buttonLoginEnter";
            this.buttonLoginEnter.Size = new System.Drawing.Size(116, 96);
            this.buttonLoginEnter.TabIndex = 14;
            this.buttonLoginEnter.Text = "Enter";
            this.buttonLoginEnter.UseVisualStyleBackColor = true;
            this.buttonLoginEnter.Click += new System.EventHandler(this.buttonLoginEnter_Click);
            // 
            // buttonLogin0
            // 
            this.buttonLogin0.Location = new System.Drawing.Point(729, 358);
            this.buttonLogin0.Name = "buttonLogin0";
            this.buttonLogin0.Size = new System.Drawing.Size(116, 96);
            this.buttonLogin0.TabIndex = 12;
            this.buttonLogin0.Tag = "0";
            this.buttonLogin0.Text = "0";
            this.buttonLogin0.UseVisualStyleBackColor = true;
            this.buttonLogin0.Click += new System.EventHandler(this.buttonLoginNum_Click);
            // 
            // buttonLogin9
            // 
            this.buttonLogin9.Location = new System.Drawing.Point(851, 256);
            this.buttonLogin9.Name = "buttonLogin9";
            this.buttonLogin9.Size = new System.Drawing.Size(116, 96);
            this.buttonLogin9.TabIndex = 11;
            this.buttonLogin9.Tag = "9";
            this.buttonLogin9.Text = "9";
            this.buttonLogin9.UseVisualStyleBackColor = true;
            this.buttonLogin9.Click += new System.EventHandler(this.buttonLoginNum_Click);
            // 
            // buttonLogin8
            // 
            this.buttonLogin8.Location = new System.Drawing.Point(729, 256);
            this.buttonLogin8.Name = "buttonLogin8";
            this.buttonLogin8.Size = new System.Drawing.Size(116, 96);
            this.buttonLogin8.TabIndex = 10;
            this.buttonLogin8.Tag = "8";
            this.buttonLogin8.Text = "8";
            this.buttonLogin8.UseVisualStyleBackColor = true;
            this.buttonLogin8.Click += new System.EventHandler(this.buttonLoginNum_Click);
            // 
            // buttonLogin7
            // 
            this.buttonLogin7.Location = new System.Drawing.Point(607, 256);
            this.buttonLogin7.Name = "buttonLogin7";
            this.buttonLogin7.Size = new System.Drawing.Size(116, 96);
            this.buttonLogin7.TabIndex = 9;
            this.buttonLogin7.Tag = "7";
            this.buttonLogin7.Text = "7";
            this.buttonLogin7.UseVisualStyleBackColor = true;
            this.buttonLogin7.Click += new System.EventHandler(this.buttonLoginNum_Click);
            // 
            // buttonLogin6
            // 
            this.buttonLogin6.Location = new System.Drawing.Point(851, 154);
            this.buttonLogin6.Name = "buttonLogin6";
            this.buttonLogin6.Size = new System.Drawing.Size(116, 96);
            this.buttonLogin6.TabIndex = 8;
            this.buttonLogin6.Tag = "6";
            this.buttonLogin6.Text = "6";
            this.buttonLogin6.UseVisualStyleBackColor = true;
            this.buttonLogin6.Click += new System.EventHandler(this.buttonLoginNum_Click);
            // 
            // buttonLogin5
            // 
            this.buttonLogin5.Location = new System.Drawing.Point(729, 154);
            this.buttonLogin5.Name = "buttonLogin5";
            this.buttonLogin5.Size = new System.Drawing.Size(116, 96);
            this.buttonLogin5.TabIndex = 7;
            this.buttonLogin5.Tag = "5";
            this.buttonLogin5.Text = "5";
            this.buttonLogin5.UseVisualStyleBackColor = true;
            this.buttonLogin5.Click += new System.EventHandler(this.buttonLoginNum_Click);
            // 
            // buttonLogin4
            // 
            this.buttonLogin4.Location = new System.Drawing.Point(607, 154);
            this.buttonLogin4.Name = "buttonLogin4";
            this.buttonLogin4.Size = new System.Drawing.Size(116, 96);
            this.buttonLogin4.TabIndex = 6;
            this.buttonLogin4.Tag = "4";
            this.buttonLogin4.Text = "4";
            this.buttonLogin4.UseVisualStyleBackColor = true;
            this.buttonLogin4.Click += new System.EventHandler(this.buttonLoginNum_Click);
            // 
            // buttonLogin3
            // 
            this.buttonLogin3.Location = new System.Drawing.Point(851, 52);
            this.buttonLogin3.Name = "buttonLogin3";
            this.buttonLogin3.Size = new System.Drawing.Size(116, 96);
            this.buttonLogin3.TabIndex = 5;
            this.buttonLogin3.Tag = "3";
            this.buttonLogin3.Text = "3";
            this.buttonLogin3.UseVisualStyleBackColor = true;
            this.buttonLogin3.Click += new System.EventHandler(this.buttonLoginNum_Click);
            // 
            // buttonLogin2
            // 
            this.buttonLogin2.Location = new System.Drawing.Point(729, 52);
            this.buttonLogin2.Name = "buttonLogin2";
            this.buttonLogin2.Size = new System.Drawing.Size(116, 96);
            this.buttonLogin2.TabIndex = 4;
            this.buttonLogin2.Tag = "2";
            this.buttonLogin2.Text = "2";
            this.buttonLogin2.UseVisualStyleBackColor = true;
            this.buttonLogin2.Click += new System.EventHandler(this.buttonLoginNum_Click);
            // 
            // buttonLogin1
            // 
            this.buttonLogin1.Location = new System.Drawing.Point(607, 52);
            this.buttonLogin1.Name = "buttonLogin1";
            this.buttonLogin1.Size = new System.Drawing.Size(116, 96);
            this.buttonLogin1.TabIndex = 3;
            this.buttonLogin1.Tag = "1";
            this.buttonLogin1.Text = "1";
            this.buttonLogin1.UseVisualStyleBackColor = true;
            this.buttonLogin1.Click += new System.EventHandler(this.buttonLoginNum_Click);
            // 
            // buttonLoginClear
            // 
            this.buttonLoginClear.Location = new System.Drawing.Point(607, 358);
            this.buttonLoginClear.Name = "buttonLoginClear";
            this.buttonLoginClear.Size = new System.Drawing.Size(116, 96);
            this.buttonLoginClear.TabIndex = 13;
            this.buttonLoginClear.Text = "Clear";
            this.buttonLoginClear.UseVisualStyleBackColor = true;
            this.buttonLoginClear.Click += new System.EventHandler(this.buttonLoginClear_Click);
            // 
            // labelLoginError
            // 
            this.labelLoginError.ForeColor = System.Drawing.Color.Red;
            this.labelLoginError.Location = new System.Drawing.Point(163, 358);
            this.labelLoginError.Name = "labelLoginError";
            this.labelLoginError.Size = new System.Drawing.Size(295, 108);
            this.labelLoginError.TabIndex = 14;
            this.labelLoginError.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // buttonLicense
            // 
            this.buttonLicense.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonLicense.BackgroundImage")));
            this.buttonLicense.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.buttonLicense.Location = new System.Drawing.Point(12, 429);
            this.buttonLicense.Name = "buttonLicense";
            this.buttonLicense.Size = new System.Drawing.Size(87, 73);
            this.buttonLicense.TabIndex = 2;
            this.buttonLicense.UseVisualStyleBackColor = true;
            this.buttonLicense.Click += new System.EventHandler(this.buttonLicense_Click);
            // 
            // ucTrialStatus
            // 
            this.ucTrialStatus.AutoSize = true;
            this.ucTrialStatus.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ucTrialStatus.BackColor = System.Drawing.Color.Transparent;
            this.ucTrialStatus.FirstWarningDays = 30;
            this.ucTrialStatus.LicMgr = null;
            this.ucTrialStatus.Location = new System.Drawing.Point(207, 219);
            this.ucTrialStatus.Name = "ucTrialStatus";
            this.ucTrialStatus.SeriousWarningDays = 7;
            this.ucTrialStatus.Size = new System.Drawing.Size(203, 114);
            this.ucTrialStatus.TabIndex = 15;
            // 
            // FormLogin
            // 
            this.AcceptButton = this.buttonLoginEnter;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.SteelBlue;
            this.ClientSize = new System.Drawing.Size(1024, 514);
            this.Controls.Add(this.ucTrialStatus);
            this.Controls.Add(this.buttonLicense);
            this.Controls.Add(this.labelLoginError);
            this.Controls.Add(this.buttonLoginClear);
            this.Controls.Add(this.buttonLoginEnter);
            this.Controls.Add(this.buttonLogin0);
            this.Controls.Add(this.buttonLogin9);
            this.Controls.Add(this.buttonLogin8);
            this.Controls.Add(this.buttonLogin7);
            this.Controls.Add(this.buttonLogin6);
            this.Controls.Add(this.buttonLogin5);
            this.Controls.Add(this.buttonLogin4);
            this.Controls.Add(this.buttonLogin3);
            this.Controls.Add(this.buttonLogin2);
            this.Controls.Add(this.buttonLogin1);
            this.Controls.Add(this.labelLogin);
            this.Controls.Add(this.textBoxLogin);
            this.Font = new System.Drawing.Font("Tahoma", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(5);
            this.Name = "FormLogin";
            this.Text = "FormLogin";
            this.Load += new System.EventHandler(this.FormLogin_Load);
            this.Enter += new System.EventHandler(this.FormLogin_Enter);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelLogin;
        private System.Windows.Forms.TextBox textBoxLogin;
        private System.Windows.Forms.Button buttonLoginEnter;
        private System.Windows.Forms.Button buttonLogin0;
        private System.Windows.Forms.Button buttonLogin9;
        private System.Windows.Forms.Button buttonLogin8;
        private System.Windows.Forms.Button buttonLogin7;
        private System.Windows.Forms.Button buttonLogin6;
        private System.Windows.Forms.Button buttonLogin5;
        private System.Windows.Forms.Button buttonLogin4;
        private System.Windows.Forms.Button buttonLogin3;
        private System.Windows.Forms.Button buttonLogin2;
        private System.Windows.Forms.Button buttonLogin1;
        private System.Windows.Forms.Button buttonLoginClear;
        private System.Windows.Forms.Label labelLoginError;
        private System.Windows.Forms.Button buttonLicense;
        private Support2.Forms.UCTrialStatus ucTrialStatus;
    }
}