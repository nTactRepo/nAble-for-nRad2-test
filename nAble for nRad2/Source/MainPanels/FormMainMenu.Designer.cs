using nAble_for_nRad2.Properties;

namespace nAble
{
    partial class FormMainMenu
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
            this.pictureBoxLogo = new System.Windows.Forms.PictureBox();
            this.buttonMaint = new System.Windows.Forms.Button();
            this.buttonRun = new System.Windows.Forms.Button();
            this.buttonFluid = new System.Windows.Forms.Button();
            this.buttonChuck = new System.Windows.Forms.Button();
            this.buttonJogging = new System.Windows.Forms.Button();
            this.buttonRecipe = new System.Windows.Forms.Button();
            this.webBrowserInfo = new System.Windows.Forms.WebBrowser();
            this.buttonProcessStack = new System.Windows.Forms.Button();
            this.buttonTester = new System.Windows.Forms.Button();
            this.buttonManualMode = new System.Windows.Forms.Button();
            this.buttonAutoMode = new System.Windows.Forms.Button();
            this.buttonClearLDULDFailure = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBoxLogo
            // 
            this.pictureBoxLogo.Image = Resources.ntact;
            this.pictureBoxLogo.Location = new System.Drawing.Point(691, 3);
            this.pictureBoxLogo.Name = "pictureBoxLogo";
            this.pictureBoxLogo.Size = new System.Drawing.Size(267, 120);
            this.pictureBoxLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxLogo.TabIndex = 18;
            this.pictureBoxLogo.TabStop = false;
            this.pictureBoxLogo.Click += new System.EventHandler(this.pictureBoxLogo_Click);
            // 
            // buttonMaint
            // 
            this.buttonMaint.Font = new System.Drawing.Font("Tahoma", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonMaint.Location = new System.Drawing.Point(244, 264);
            this.buttonMaint.Name = "buttonMaint";
            this.buttonMaint.Size = new System.Drawing.Size(198, 111);
            this.buttonMaint.TabIndex = 17;
            this.buttonMaint.Text = "Maintenance";
            this.buttonMaint.UseVisualStyleBackColor = true;
            this.buttonMaint.Click += new System.EventHandler(this.buttonMaint_Click);
            // 
            // buttonRun
            // 
            this.buttonRun.BackColor = System.Drawing.Color.White;
            this.buttonRun.Font = new System.Drawing.Font("Tahoma", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonRun.Location = new System.Drawing.Point(23, 264);
            this.buttonRun.Name = "buttonRun";
            this.buttonRun.Size = new System.Drawing.Size(198, 111);
            this.buttonRun.TabIndex = 16;
            this.buttonRun.Text = "Run";
            this.buttonRun.UseVisualStyleBackColor = true;
            this.buttonRun.Click += new System.EventHandler(this.buttonRun_Click);
            // 
            // buttonFluid
            // 
            this.buttonFluid.Font = new System.Drawing.Font("Tahoma", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonFluid.Location = new System.Drawing.Point(244, 138);
            this.buttonFluid.Name = "buttonFluid";
            this.buttonFluid.Size = new System.Drawing.Size(198, 111);
            this.buttonFluid.TabIndex = 15;
            this.buttonFluid.Text = "Fluid\r\nControl";
            this.buttonFluid.UseVisualStyleBackColor = true;
            this.buttonFluid.Click += new System.EventHandler(this.buttonFluid_Click);
            // 
            // buttonChuck
            // 
            this.buttonChuck.Font = new System.Drawing.Font("Tahoma", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonChuck.Location = new System.Drawing.Point(23, 138);
            this.buttonChuck.Name = "buttonChuck";
            this.buttonChuck.Size = new System.Drawing.Size(198, 111);
            this.buttonChuck.TabIndex = 14;
            this.buttonChuck.Text = "Chuck\r\nControl";
            this.buttonChuck.UseVisualStyleBackColor = true;
            this.buttonChuck.Click += new System.EventHandler(this.buttonChuck_Click);
            // 
            // buttonJogging
            // 
            this.buttonJogging.Font = new System.Drawing.Font("Tahoma", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonJogging.Location = new System.Drawing.Point(244, 12);
            this.buttonJogging.Name = "buttonJogging";
            this.buttonJogging.Size = new System.Drawing.Size(198, 111);
            this.buttonJogging.TabIndex = 13;
            this.buttonJogging.Text = "Manual\r\nJogging";
            this.buttonJogging.UseVisualStyleBackColor = true;
            this.buttonJogging.Click += new System.EventHandler(this.buttonJogging_Click);
            // 
            // buttonRecipe
            // 
            this.buttonRecipe.Font = new System.Drawing.Font("Tahoma", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonRecipe.Location = new System.Drawing.Point(23, 12);
            this.buttonRecipe.Name = "buttonRecipe";
            this.buttonRecipe.Size = new System.Drawing.Size(198, 111);
            this.buttonRecipe.TabIndex = 12;
            this.buttonRecipe.Text = "Recipe\r\nManagement";
            this.buttonRecipe.UseVisualStyleBackColor = true;
            this.buttonRecipe.Click += new System.EventHandler(this.buttonRecipe_Click);
            // 
            // webBrowserInfo
            // 
            this.webBrowserInfo.AllowWebBrowserDrop = false;
            this.webBrowserInfo.IsWebBrowserContextMenuEnabled = false;
            this.webBrowserInfo.Location = new System.Drawing.Point(647, 129);
            this.webBrowserInfo.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowserInfo.Name = "webBrowserInfo";
            this.webBrowserInfo.ScrollBarsEnabled = false;
            this.webBrowserInfo.Size = new System.Drawing.Size(355, 376);
            this.webBrowserInfo.TabIndex = 20;
            this.webBrowserInfo.WebBrowserShortcutsEnabled = false;
            // 
            // buttonProcessStack
            // 
            this.buttonProcessStack.Font = new System.Drawing.Font("Tahoma", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonProcessStack.Location = new System.Drawing.Point(244, 394);
            this.buttonProcessStack.Name = "buttonProcessStack";
            this.buttonProcessStack.Size = new System.Drawing.Size(198, 111);
            this.buttonProcessStack.TabIndex = 21;
            this.buttonProcessStack.Text = "Process\r\nStack";
            this.buttonProcessStack.UseVisualStyleBackColor = true;
            this.buttonProcessStack.Visible = false;
            this.buttonProcessStack.Click += new System.EventHandler(this.buttonProcessStack_Click);
            // 
            // buttonTester
            // 
            this.buttonTester.Location = new System.Drawing.Point(479, 394);
            this.buttonTester.Name = "buttonTester";
            this.buttonTester.Size = new System.Drawing.Size(136, 111);
            this.buttonTester.TabIndex = 22;
            this.buttonTester.Text = "-Test Here-";
            this.buttonTester.UseVisualStyleBackColor = true;
            this.buttonTester.Visible = false;
            this.buttonTester.Click += new System.EventHandler(this.buttonTester_Click);
            // 
            // buttonManualMode
            // 
            this.buttonManualMode.BackColor = System.Drawing.Color.White;
            this.buttonManualMode.Font = new System.Drawing.Font("Tahoma", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonManualMode.Location = new System.Drawing.Point(23, 451);
            this.buttonManualMode.Name = "buttonManualMode";
            this.buttonManualMode.Size = new System.Drawing.Size(198, 54);
            this.buttonManualMode.TabIndex = 29;
            this.buttonManualMode.Text = "Manual Mode";
            this.buttonManualMode.UseVisualStyleBackColor = false;
            this.buttonManualMode.Visible = false;
            this.buttonManualMode.Click += new System.EventHandler(this.buttonManualMode_Click);
            // 
            // buttonAutoMode
            // 
            this.buttonAutoMode.BackColor = System.Drawing.Color.White;
            this.buttonAutoMode.Font = new System.Drawing.Font("Tahoma", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonAutoMode.Location = new System.Drawing.Point(23, 394);
            this.buttonAutoMode.Name = "buttonAutoMode";
            this.buttonAutoMode.Size = new System.Drawing.Size(198, 54);
            this.buttonAutoMode.TabIndex = 28;
            this.buttonAutoMode.Text = "Auto Mode";
            this.buttonAutoMode.UseVisualStyleBackColor = false;
            this.buttonAutoMode.Visible = false;
            this.buttonAutoMode.Click += new System.EventHandler(this.buttonAutoMode_Click);
            // 
            // buttonClearLDULDFailure
            // 
            this.buttonClearLDULDFailure.BackColor = System.Drawing.Color.Orange;
            this.buttonClearLDULDFailure.Font = new System.Drawing.Font("Tahoma", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonClearLDULDFailure.Location = new System.Drawing.Point(23, 394);
            this.buttonClearLDULDFailure.Name = "buttonClearLDULDFailure";
            this.buttonClearLDULDFailure.Size = new System.Drawing.Size(198, 111);
            this.buttonClearLDULDFailure.TabIndex = 30;
            this.buttonClearLDULDFailure.Text = "Clear LD/ULD Failure";
            this.buttonClearLDULDFailure.UseVisualStyleBackColor = false;
            this.buttonClearLDULDFailure.Visible = false;
            this.buttonClearLDULDFailure.Click += new System.EventHandler(this.buttonClearLDULDFailure_Click);
            // 
            // FormMainMenu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 23F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.AntiqueWhite;
            this.ClientSize = new System.Drawing.Size(1024, 514);
            this.Controls.Add(this.buttonManualMode);
            this.Controls.Add(this.buttonTester);
            this.Controls.Add(this.buttonProcessStack);
            this.Controls.Add(this.webBrowserInfo);
            this.Controls.Add(this.pictureBoxLogo);
            this.Controls.Add(this.buttonMaint);
            this.Controls.Add(this.buttonRun);
            this.Controls.Add(this.buttonFluid);
            this.Controls.Add(this.buttonChuck);
            this.Controls.Add(this.buttonJogging);
            this.Controls.Add(this.buttonRecipe);
            this.Controls.Add(this.buttonAutoMode);
            this.Controls.Add(this.buttonClearLDULDFailure);
            this.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(5);
            this.Name = "FormMainMenu";
            this.Text = "Main";
            this.Load += new System.EventHandler(this.FormMainMenu_Load);
            this.VisibleChanged += new System.EventHandler(this.FormMainMenu_VisibleChanged);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBoxLogo;
        private System.Windows.Forms.Button buttonMaint;
        private System.Windows.Forms.Button buttonRun;
        private System.Windows.Forms.Button buttonFluid;
        private System.Windows.Forms.Button buttonChuck;
        private System.Windows.Forms.Button buttonJogging;
        private System.Windows.Forms.Button buttonRecipe;
        private System.Windows.Forms.WebBrowser webBrowserInfo;
		private System.Windows.Forms.Button buttonProcessStack;
		private System.Windows.Forms.Button buttonTester;
        private System.Windows.Forms.Button buttonManualMode;
        private System.Windows.Forms.Button buttonAutoMode;
        private System.Windows.Forms.Button buttonClearLDULDFailure;
    }
}