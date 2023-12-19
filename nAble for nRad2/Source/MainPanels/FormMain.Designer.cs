namespace nAble
{
    partial class FormMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.labelMainStatus = new System.Windows.Forms.Label();
            this.panelMain = new System.Windows.Forms.Panel();
            this.linkLabelExit = new System.Windows.Forms.LinkLabel();
            this.linkLabelMinimize = new System.Windows.Forms.LinkLabel();
            this.panelMenu = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // labelMainStatus
            // 
            this.labelMainStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelMainStatus.BackColor = System.Drawing.Color.Yellow;
            this.labelMainStatus.Font = new System.Drawing.Font("Tahoma", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelMainStatus.Location = new System.Drawing.Point(0, 562);
            this.labelMainStatus.Name = "labelMainStatus";
            this.labelMainStatus.Size = new System.Drawing.Size(1024, 38);
            this.labelMainStatus.TabIndex = 4;
            this.labelMainStatus.Text = "Starting...";
            this.labelMainStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelMainStatus.Click += new System.EventHandler(this.labelMainStatus_Click);
            // 
            // panelMain
            // 
            this.panelMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelMain.AutoScroll = true;
            this.panelMain.Location = new System.Drawing.Point(0, 45);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(1024, 514);
            this.panelMain.TabIndex = 5;
            // 
            // linkLabelExit
            // 
            this.linkLabelExit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.linkLabelExit.Font = new System.Drawing.Font("Tahoma", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkLabelExit.Location = new System.Drawing.Point(946, 6);
            this.linkLabelExit.Name = "linkLabelExit";
            this.linkLabelExit.Size = new System.Drawing.Size(69, 36);
            this.linkLabelExit.TabIndex = 7;
            this.linkLabelExit.TabStop = true;
            this.linkLabelExit.Text = "Exit";
            this.linkLabelExit.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelExit_LinkClicked);
            // 
            // linkLabelMinimize
            // 
            this.linkLabelMinimize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.linkLabelMinimize.Font = new System.Drawing.Font("Tahoma", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkLabelMinimize.Location = new System.Drawing.Point(818, 6);
            this.linkLabelMinimize.Name = "linkLabelMinimize";
            this.linkLabelMinimize.Size = new System.Drawing.Size(122, 36);
            this.linkLabelMinimize.TabIndex = 9;
            this.linkLabelMinimize.TabStop = true;
            this.linkLabelMinimize.Text = "Minimize";
            this.linkLabelMinimize.Visible = false;
            this.linkLabelMinimize.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelMinimize_LinkClicked);
            // 
            // panelMenu
            // 
            this.panelMenu.Location = new System.Drawing.Point(0, 0);
            this.panelMenu.Name = "panelMenu";
            this.panelMenu.Size = new System.Drawing.Size(812, 42);
            this.panelMenu.TabIndex = 0;
            // 
            // FormMain
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1024, 600);
            this.Controls.Add(this.panelMenu);
            this.Controls.Add(this.linkLabelMinimize);
            this.Controls.Add(this.linkLabelExit);
            this.Controls.Add(this.panelMain);
            this.Controls.Add(this.labelMainStatus);
            this.Font = new System.Drawing.Font("Tahoma", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(5);
            this.Name = "FormMain";
            this.Text = "nAble for nRad 2";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormMain_FormClosed);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labelMainStatus;
        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.LinkLabel linkLabelExit;
        private System.Windows.Forms.LinkLabel linkLabelMinimize;
        private System.Windows.Forms.Panel panelMenu;
    }
}

