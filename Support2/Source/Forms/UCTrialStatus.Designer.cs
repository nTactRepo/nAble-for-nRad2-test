
namespace Support2.Forms
{
    partial class UCTrialStatus
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panelTrialMode = new System.Windows.Forms.Panel();
            this.labelTrialMode = new System.Windows.Forms.Label();
            this.labelExpireDate = new System.Windows.Forms.Label();
            this.labelExpireHeading = new System.Windows.Forms.Label();
            this.panelTrialMode.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelTrialMode
            // 
            this.panelTrialMode.BackColor = System.Drawing.Color.Transparent;
            this.panelTrialMode.Controls.Add(this.labelTrialMode);
            this.panelTrialMode.Controls.Add(this.labelExpireDate);
            this.panelTrialMode.Controls.Add(this.labelExpireHeading);
            this.panelTrialMode.Location = new System.Drawing.Point(0, 0);
            this.panelTrialMode.Name = "panelTrialMode";
            this.panelTrialMode.Size = new System.Drawing.Size(200, 111);
            this.panelTrialMode.TabIndex = 25;
            // 
            // labelTrialMode
            // 
            this.labelTrialMode.Font = new System.Drawing.Font("Tahoma", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTrialMode.ForeColor = System.Drawing.SystemColors.ControlText;
            this.labelTrialMode.Location = new System.Drawing.Point(6, 18);
            this.labelTrialMode.Name = "labelTrialMode";
            this.labelTrialMode.Size = new System.Drawing.Size(191, 39);
            this.labelTrialMode.TabIndex = 23;
            this.labelTrialMode.Text = "Trial Mode";
            this.labelTrialMode.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelExpireDate
            // 
            this.labelExpireDate.AutoSize = true;
            this.labelExpireDate.BackColor = System.Drawing.Color.Transparent;
            this.labelExpireDate.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelExpireDate.ForeColor = System.Drawing.SystemColors.ControlText;
            this.labelExpireDate.Location = new System.Drawing.Point(91, 60);
            this.labelExpireDate.Name = "labelExpireDate";
            this.labelExpireDate.Size = new System.Drawing.Size(96, 23);
            this.labelExpireDate.TabIndex = 23;
            this.labelExpireDate.Text = "{{expiry}}";
            this.labelExpireDate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelExpireHeading
            // 
            this.labelExpireHeading.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelExpireHeading.Location = new System.Drawing.Point(17, 57);
            this.labelExpireHeading.Name = "labelExpireHeading";
            this.labelExpireHeading.Size = new System.Drawing.Size(80, 30);
            this.labelExpireHeading.TabIndex = 23;
            this.labelExpireHeading.Text = "Expires:";
            this.labelExpireHeading.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // UCTrialStatus
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.panelTrialMode);
            this.Name = "UCTrialStatus";
            this.Size = new System.Drawing.Size(203, 114);
            this.panelTrialMode.ResumeLayout(false);
            this.panelTrialMode.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelTrialMode;
        private System.Windows.Forms.Label labelTrialMode;
        private System.Windows.Forms.Label labelExpireDate;
        private System.Windows.Forms.Label labelExpireHeading;
    }
}
