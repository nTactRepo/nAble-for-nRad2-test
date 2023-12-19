namespace nAble
{
    partial class nRadMessageBox
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(nRadMessageBox));
            this.labelTitle = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.labelMessage = new System.Windows.Forms.Label();
            this.buttonOpenLKNav = new DevComponents.DotNetBar.ButtonX();
            this.pictureBoxIcon = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // labelTitle
            // 
            this.labelTitle.BackColor = System.Drawing.Color.DarkOrange;
            this.labelTitle.Location = new System.Drawing.Point(0, 0);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(643, 42);
            this.labelTitle.TabIndex = 0;
            this.labelTitle.Text = "Title";
            this.labelTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.SystemColors.Control;
            this.button1.Location = new System.Drawing.Point(489, 296);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(125, 65);
            this.button1.TabIndex = 1;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = false;
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.SystemColors.Control;
            this.button2.Location = new System.Drawing.Point(346, 296);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(125, 65);
            this.button2.TabIndex = 2;
            this.button2.Text = "button2";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Visible = false;
            // 
            // button3
            // 
            this.button3.BackColor = System.Drawing.SystemColors.Control;
            this.button3.Location = new System.Drawing.Point(203, 296);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(125, 65);
            this.button3.TabIndex = 3;
            this.button3.Text = "button3";
            this.button3.UseVisualStyleBackColor = false;
            this.button3.Visible = false;
            // 
            // labelMessage
            // 
            this.labelMessage.ForeColor = System.Drawing.Color.White;
            this.labelMessage.Location = new System.Drawing.Point(203, 59);
            this.labelMessage.Name = "labelMessage";
            this.labelMessage.Size = new System.Drawing.Size(410, 227);
            this.labelMessage.TabIndex = 4;
            this.labelMessage.Text = "Message\r\nLine 2";
            this.labelMessage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // buttonOpenLKNav
            // 
            this.buttonOpenLKNav.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.buttonOpenLKNav.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.buttonOpenLKNav.Image = Resources.LK_Navigator_2_Icon;
            this.buttonOpenLKNav.Location = new System.Drawing.Point(12, 284);
            this.buttonOpenLKNav.Name = "buttonOpenLKNav";
            this.buttonOpenLKNav.Size = new System.Drawing.Size(90, 77);
            this.buttonOpenLKNav.Style = DevComponents.DotNetBar.eDotNetBarStyle.Office2003;
            this.buttonOpenLKNav.TabIndex = 322;
            this.buttonOpenLKNav.Visible = false;
            // 
            // pictureBoxIcon
            // 
            this.pictureBoxIcon.Location = new System.Drawing.Point(43, 89);
            this.pictureBoxIcon.Name = "pictureBoxIcon";
            this.pictureBoxIcon.Size = new System.Drawing.Size(90, 90);
            this.pictureBoxIcon.TabIndex = 5;
            this.pictureBoxIcon.TabStop = false;
            // 
            // nRadMessageBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(15F, 33F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.ClientSize = new System.Drawing.Size(640, 373);
            this.ControlBox = false;
            this.Controls.Add(this.buttonOpenLKNav);
            this.Controls.Add(this.pictureBoxIcon);
            this.Controls.Add(this.labelMessage);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.labelTitle);
            this.Font = new System.Drawing.Font("Tahoma", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(8);
            this.Name = "nRadMessageBox";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxIcon)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Label labelMessage;
        private System.Windows.Forms.PictureBox pictureBoxIcon;
        private DevComponents.DotNetBar.ButtonX buttonOpenLKNav;
    }
}