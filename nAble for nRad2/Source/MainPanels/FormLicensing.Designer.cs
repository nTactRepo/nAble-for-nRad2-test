namespace nAble
{
    partial class FormLicensing
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
            this.buttonActivate = new System.Windows.Forms.Button();
            this.labelTitle = new System.Windows.Forms.Label();
            this.buttonBack = new System.Windows.Forms.Button();
            this.dateTimePickerCreation = new System.Windows.Forms.DateTimePicker();
            this.labelCreation = new System.Windows.Forms.Label();
            this.dateTimePickerExpiry = new System.Windows.Forms.DateTimePicker();
            this.labelExpiry = new System.Windows.Forms.Label();
            this.textBoxProductCode = new System.Windows.Forms.TextBox();
            this.labelFeatures = new System.Windows.Forms.Label();
            this.labelValidator = new System.Windows.Forms.Label();
            this.panelEntryControls = new System.Windows.Forms.Panel();
            this.labelNoExpiration = new System.Windows.Forms.Label();
            this.maskedTextBoxProductKey = new System.Windows.Forms.MaskedTextBox();
            this.labelAlreadyExpired = new System.Windows.Forms.Label();
            this.panelEntryControls.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonActivate
            // 
            this.buttonActivate.AutoSize = true;
            this.buttonActivate.Location = new System.Drawing.Point(363, 368);
            this.buttonActivate.Name = "buttonActivate";
            this.buttonActivate.Size = new System.Drawing.Size(394, 70);
            this.buttonActivate.TabIndex = 4;
            this.buttonActivate.Text = "Activate";
            this.buttonActivate.UseVisualStyleBackColor = true;
            this.buttonActivate.Click += new System.EventHandler(this.buttonActivate_Click);
            // 
            // labelTitle
            // 
            this.labelTitle.BackColor = System.Drawing.Color.Transparent;
            this.labelTitle.Font = new System.Drawing.Font("Tahoma", 27.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTitle.ForeColor = System.Drawing.Color.White;
            this.labelTitle.Location = new System.Drawing.Point(21, 9);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(991, 45);
            this.labelTitle.TabIndex = 5;
            this.labelTitle.Text = "Please Enter License Information";
            this.labelTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelTitle.Click += new System.EventHandler(this.labelTitle_Click);
            // 
            // buttonBack
            // 
            this.buttonBack.Location = new System.Drawing.Point(911, 432);
            this.buttonBack.Name = "buttonBack";
            this.buttonBack.Size = new System.Drawing.Size(101, 70);
            this.buttonBack.TabIndex = 5;
            this.buttonBack.Text = "Back";
            this.buttonBack.UseVisualStyleBackColor = true;
            this.buttonBack.Click += new System.EventHandler(this.buttonBack_Click);
            // 
            // dateTimePickerCreation
            // 
            this.dateTimePickerCreation.CustomFormat = "MM/dd/yyyy";
            this.dateTimePickerCreation.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePickerCreation.Location = new System.Drawing.Point(363, 181);
            this.dateTimePickerCreation.Name = "dateTimePickerCreation";
            this.dateTimePickerCreation.Size = new System.Drawing.Size(206, 40);
            this.dateTimePickerCreation.TabIndex = 0;
            // 
            // labelCreation
            // 
            this.labelCreation.AutoSize = true;
            this.labelCreation.ForeColor = System.Drawing.Color.White;
            this.labelCreation.Location = new System.Drawing.Point(76, 187);
            this.labelCreation.Name = "labelCreation";
            this.labelCreation.Size = new System.Drawing.Size(281, 33);
            this.labelCreation.TabIndex = 8;
            this.labelCreation.Text = "License Creation Date:";
            // 
            // dateTimePickerExpiry
            // 
            this.dateTimePickerExpiry.CustomFormat = "MM/dd/yyyy";
            this.dateTimePickerExpiry.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePickerExpiry.Location = new System.Drawing.Point(363, 256);
            this.dateTimePickerExpiry.Name = "dateTimePickerExpiry";
            this.dateTimePickerExpiry.Size = new System.Drawing.Size(206, 40);
            this.dateTimePickerExpiry.TabIndex = 1;
            // 
            // labelExpiry
            // 
            this.labelExpiry.AutoSize = true;
            this.labelExpiry.ForeColor = System.Drawing.Color.White;
            this.labelExpiry.Location = new System.Drawing.Point(57, 262);
            this.labelExpiry.Name = "labelExpiry";
            this.labelExpiry.Size = new System.Drawing.Size(300, 33);
            this.labelExpiry.TabIndex = 8;
            this.labelExpiry.Text = "License Expiration Date:";
            // 
            // textBoxProductCode
            // 
            this.textBoxProductCode.Location = new System.Drawing.Point(363, 35);
            this.textBoxProductCode.Name = "textBoxProductCode";
            this.textBoxProductCode.Size = new System.Drawing.Size(322, 40);
            this.textBoxProductCode.TabIndex = 2;
            // 
            // labelFeatures
            // 
            this.labelFeatures.AutoSize = true;
            this.labelFeatures.ForeColor = System.Drawing.Color.White;
            this.labelFeatures.Location = new System.Drawing.Point(173, 38);
            this.labelFeatures.Name = "labelFeatures";
            this.labelFeatures.Size = new System.Drawing.Size(184, 33);
            this.labelFeatures.TabIndex = 8;
            this.labelFeatures.Text = "Product Code:";
            // 
            // labelValidator
            // 
            this.labelValidator.AutoSize = true;
            this.labelValidator.ForeColor = System.Drawing.Color.White;
            this.labelValidator.Location = new System.Drawing.Point(190, 111);
            this.labelValidator.Name = "labelValidator";
            this.labelValidator.Size = new System.Drawing.Size(167, 33);
            this.labelValidator.TabIndex = 8;
            this.labelValidator.Text = "Product Key:";
            // 
            // panelEntryControls
            // 
            this.panelEntryControls.Controls.Add(this.dateTimePickerExpiry);
            this.panelEntryControls.Controls.Add(this.labelNoExpiration);
            this.panelEntryControls.Controls.Add(this.labelExpiry);
            this.panelEntryControls.Controls.Add(this.maskedTextBoxProductKey);
            this.panelEntryControls.Controls.Add(this.textBoxProductCode);
            this.panelEntryControls.Controls.Add(this.labelAlreadyExpired);
            this.panelEntryControls.Controls.Add(this.labelValidator);
            this.panelEntryControls.Controls.Add(this.labelFeatures);
            this.panelEntryControls.Controls.Add(this.labelCreation);
            this.panelEntryControls.Controls.Add(this.dateTimePickerCreation);
            this.panelEntryControls.Controls.Add(this.buttonActivate);
            this.panelEntryControls.Location = new System.Drawing.Point(21, 64);
            this.panelEntryControls.Name = "panelEntryControls";
            this.panelEntryControls.Size = new System.Drawing.Size(884, 447);
            this.panelEntryControls.TabIndex = 10;
            // 
            // labelNoExpiration
            // 
            this.labelNoExpiration.AutoSize = true;
            this.labelNoExpiration.ForeColor = System.Drawing.Color.White;
            this.labelNoExpiration.Location = new System.Drawing.Point(356, 262);
            this.labelNoExpiration.Name = "labelNoExpiration";
            this.labelNoExpiration.Size = new System.Drawing.Size(174, 33);
            this.labelNoExpiration.TabIndex = 8;
            this.labelNoExpiration.Text = "No Expiration";
            // 
            // maskedTextBoxProductKey
            // 
            this.maskedTextBoxProductKey.Location = new System.Drawing.Point(363, 108);
            this.maskedTextBoxProductKey.Mask = "AAAA-AAAA-AAAA-AAAA";
            this.maskedTextBoxProductKey.Name = "maskedTextBoxProductKey";
            this.maskedTextBoxProductKey.Size = new System.Drawing.Size(322, 40);
            this.maskedTextBoxProductKey.TabIndex = 9;
            this.maskedTextBoxProductKey.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.maskedTextBoxProductKey_KeyPress);
            // 
            // labelAlreadyExpired
            // 
            this.labelAlreadyExpired.AutoSize = true;
            this.labelAlreadyExpired.ForeColor = System.Drawing.Color.Red;
            this.labelAlreadyExpired.Location = new System.Drawing.Point(590, 262);
            this.labelAlreadyExpired.Name = "labelAlreadyExpired";
            this.labelAlreadyExpired.Size = new System.Drawing.Size(208, 33);
            this.labelAlreadyExpired.TabIndex = 8;
            this.labelAlreadyExpired.Text = "Already Expired!";
            this.labelAlreadyExpired.Visible = false;
            // 
            // FormLicensing
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(15F, 33F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.SteelBlue;
            this.ClientSize = new System.Drawing.Size(1024, 514);
            this.Controls.Add(this.buttonBack);
            this.Controls.Add(this.labelTitle);
            this.Controls.Add(this.panelEntryControls);
            this.Font = new System.Drawing.Font("Tahoma", 20.25F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(8);
            this.Name = "FormLicensing";
            this.Text = "Licensing";
            this.Load += new System.EventHandler(this.FormLicensing_Load);
            this.panelEntryControls.ResumeLayout(false);
            this.panelEntryControls.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonActivate;
        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.Button buttonBack;
        private System.Windows.Forms.DateTimePicker dateTimePickerCreation;
        private System.Windows.Forms.Label labelCreation;
        private System.Windows.Forms.DateTimePicker dateTimePickerExpiry;
        private System.Windows.Forms.Label labelExpiry;
        private System.Windows.Forms.TextBox textBoxProductCode;
        private System.Windows.Forms.Label labelFeatures;
        private System.Windows.Forms.Label labelValidator;
        private System.Windows.Forms.Panel panelEntryControls;
        private System.Windows.Forms.MaskedTextBox maskedTextBoxProductKey;
        private System.Windows.Forms.Label labelNoExpiration;
        private System.Windows.Forms.Label labelAlreadyExpired;
    }
}