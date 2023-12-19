namespace nAble
{
    partial class FormJogTransfer
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
			this.panelXAxis = new System.Windows.Forms.Panel();
			this.labelInMotion = new System.Windows.Forms.Label();
			this.labelXJogDistance = new System.Windows.Forms.Label();
			this.labelXJogSpeed = new System.Windows.Forms.Label();
			this.labelPos = new System.Windows.Forms.Label();
			this.buttonJogDistance = new System.Windows.Forms.Button();
			this.buttonJogSpeed = new System.Windows.Forms.Button();
			this.buttonJogFwd = new System.Windows.Forms.Button();
			this.buttonJogRev = new System.Windows.Forms.Button();
			this.buttonTeachChuckPos = new System.Windows.Forms.Button();
			this.groupBoxChuckPosition = new System.Windows.Forms.GroupBox();
			this.buttonGotoChuckPos = new System.Windows.Forms.Button();
			this.labelChuckPos = new System.Windows.Forms.Label();
			this.groupBoxLoadPosition = new System.Windows.Forms.GroupBox();
			this.buttonGotoLoadPos = new System.Windows.Forms.Button();
			this.labelStackPos = new System.Windows.Forms.Label();
			this.buttonTeachLoadPos = new System.Windows.Forms.Button();
			this.groupBoxPinsAligners = new System.Windows.Forms.GroupBox();
			this.buttonAligners = new System.Windows.Forms.Button();
			this.buttonLiftPins = new System.Windows.Forms.Button();
			this.panelTransferAxis = new System.Windows.Forms.Panel();
			this.labelSubstrate = new System.Windows.Forms.Label();
			this.panel1 = new System.Windows.Forms.Panel();
			this.pictureBoxPinsUp = new System.Windows.Forms.PictureBox();
			this.pictureBoxPinsDown = new System.Windows.Forms.PictureBox();
			this.pictureBoxAlignersUp = new System.Windows.Forms.PictureBox();
			this.pictureBoxAlignersDown = new System.Windows.Forms.PictureBox();
			this.panelXAxis.SuspendLayout();
			this.groupBoxChuckPosition.SuspendLayout();
			this.groupBoxLoadPosition.SuspendLayout();
			this.groupBoxPinsAligners.SuspendLayout();
			this.panelTransferAxis.SuspendLayout();
			this.panel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxPinsUp)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxPinsDown)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxAlignersUp)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxAlignersDown)).BeginInit();
			this.SuspendLayout();
			// 
			// panelXAxis
			// 
			this.panelXAxis.BackColor = System.Drawing.Color.SteelBlue;
			this.panelXAxis.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panelXAxis.Controls.Add(this.labelInMotion);
			this.panelXAxis.Controls.Add(this.labelXJogDistance);
			this.panelXAxis.Controls.Add(this.labelXJogSpeed);
			this.panelXAxis.Controls.Add(this.labelPos);
			this.panelXAxis.Controls.Add(this.buttonJogDistance);
			this.panelXAxis.Controls.Add(this.buttonJogSpeed);
			this.panelXAxis.Controls.Add(this.buttonJogFwd);
			this.panelXAxis.Controls.Add(this.buttonJogRev);
			this.panelXAxis.Location = new System.Drawing.Point(21, 18);
			this.panelXAxis.Name = "panelXAxis";
			this.panelXAxis.Size = new System.Drawing.Size(424, 460);
			this.panelXAxis.TabIndex = 4;
			// 
			// labelInMotion
			// 
			this.labelInMotion.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelInMotion.ForeColor = System.Drawing.Color.Yellow;
			this.labelInMotion.Location = new System.Drawing.Point(36, 330);
			this.labelInMotion.Name = "labelInMotion";
			this.labelInMotion.Size = new System.Drawing.Size(86, 18);
			this.labelInMotion.TabIndex = 21;
			this.labelInMotion.Text = "In Motion";
			this.labelInMotion.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.labelInMotion.Visible = false;
			// 
			// labelXJogDistance
			// 
			this.labelXJogDistance.Font = new System.Drawing.Font("Tahoma", 15.75F);
			this.labelXJogDistance.ForeColor = System.Drawing.SystemColors.ControlLightLight;
			this.labelXJogDistance.Location = new System.Drawing.Point(48, 245);
			this.labelXJogDistance.Name = "labelXJogDistance";
			this.labelXJogDistance.Size = new System.Drawing.Size(170, 80);
			this.labelXJogDistance.TabIndex = 7;
			this.labelXJogDistance.Text = "Jog Distance\r\n(mm)";
			this.labelXJogDistance.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelXJogSpeed
			// 
			this.labelXJogSpeed.Font = new System.Drawing.Font("Tahoma", 15.75F);
			this.labelXJogSpeed.ForeColor = System.Drawing.SystemColors.ControlLightLight;
			this.labelXJogSpeed.Location = new System.Drawing.Point(48, 139);
			this.labelXJogSpeed.Name = "labelXJogSpeed";
			this.labelXJogSpeed.Size = new System.Drawing.Size(170, 80);
			this.labelXJogSpeed.TabIndex = 6;
			this.labelXJogSpeed.Text = "Jog Speed\r\n(mm/sec)";
			this.labelXJogSpeed.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelPos
			// 
			this.labelPos.BackColor = System.Drawing.Color.Black;
			this.labelPos.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.labelPos.Font = new System.Drawing.Font("Tahoma", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelPos.ForeColor = System.Drawing.Color.Red;
			this.labelPos.Location = new System.Drawing.Point(37, 348);
			this.labelPos.Name = "labelPos";
			this.labelPos.Size = new System.Drawing.Size(353, 85);
			this.labelPos.TabIndex = 5;
			this.labelPos.Text = "000.000 mm";
			this.labelPos.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// buttonJogDistance
			// 
			this.buttonJogDistance.BackColor = System.Drawing.SystemColors.Control;
			this.buttonJogDistance.Font = new System.Drawing.Font("Tahoma", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.buttonJogDistance.Location = new System.Drawing.Point(229, 236);
			this.buttonJogDistance.Name = "buttonJogDistance";
			this.buttonJogDistance.Size = new System.Drawing.Size(159, 98);
			this.buttonJogDistance.TabIndex = 4;
			this.buttonJogDistance.Tag = "0";
			this.buttonJogDistance.Text = "000.000";
			this.buttonJogDistance.UseVisualStyleBackColor = false;
			this.buttonJogDistance.Click += new System.EventHandler(this.buttonJogDistance_Click);
			// 
			// buttonJogSpeed
			// 
			this.buttonJogSpeed.BackColor = System.Drawing.SystemColors.Control;
			this.buttonJogSpeed.Font = new System.Drawing.Font("Tahoma", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.buttonJogSpeed.Location = new System.Drawing.Point(229, 130);
			this.buttonJogSpeed.Name = "buttonJogSpeed";
			this.buttonJogSpeed.Size = new System.Drawing.Size(159, 98);
			this.buttonJogSpeed.TabIndex = 3;
			this.buttonJogSpeed.Tag = "0";
			this.buttonJogSpeed.Text = "50";
			this.buttonJogSpeed.UseVisualStyleBackColor = false;
			this.buttonJogSpeed.Click += new System.EventHandler(this.buttonJogSpeed_Click);
			// 
			// buttonJogFwd
			// 
			this.buttonJogFwd.BackColor = System.Drawing.SystemColors.Control;
			this.buttonJogFwd.Font = new System.Drawing.Font("Tahoma", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.buttonJogFwd.Location = new System.Drawing.Point(229, 17);
			this.buttonJogFwd.Name = "buttonJogFwd";
			this.buttonJogFwd.Size = new System.Drawing.Size(159, 98);
			this.buttonJogFwd.TabIndex = 2;
			this.buttonJogFwd.Text = "Jog Fwd";
			this.buttonJogFwd.UseVisualStyleBackColor = false;
			this.buttonJogFwd.Click += new System.EventHandler(this.buttonJogFwd_Click);
			// 
			// buttonJogRev
			// 
			this.buttonJogRev.BackColor = System.Drawing.SystemColors.Control;
			this.buttonJogRev.Font = new System.Drawing.Font("Tahoma", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.buttonJogRev.Location = new System.Drawing.Point(39, 17);
			this.buttonJogRev.Name = "buttonJogRev";
			this.buttonJogRev.Size = new System.Drawing.Size(159, 98);
			this.buttonJogRev.TabIndex = 1;
			this.buttonJogRev.Text = "Jog Rev";
			this.buttonJogRev.UseVisualStyleBackColor = false;
			this.buttonJogRev.Click += new System.EventHandler(this.buttonJogRev_Click);
			// 
			// buttonTeachChuckPos
			// 
			this.buttonTeachChuckPos.BackColor = System.Drawing.SystemColors.Control;
			this.buttonTeachChuckPos.Font = new System.Drawing.Font("Tahoma", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.buttonTeachChuckPos.ForeColor = System.Drawing.SystemColors.ControlText;
			this.buttonTeachChuckPos.Location = new System.Drawing.Point(10, 39);
			this.buttonTeachChuckPos.Name = "buttonTeachChuckPos";
			this.buttonTeachChuckPos.Size = new System.Drawing.Size(125, 52);
			this.buttonTeachChuckPos.TabIndex = 8;
			this.buttonTeachChuckPos.Text = "Teach";
			this.buttonTeachChuckPos.UseVisualStyleBackColor = false;
			this.buttonTeachChuckPos.Click += new System.EventHandler(this.buttonTeachChuckPos_Click);
			// 
			// groupBoxChuckPosition
			// 
			this.groupBoxChuckPosition.BackColor = System.Drawing.Color.Transparent;
			this.groupBoxChuckPosition.Controls.Add(this.buttonGotoChuckPos);
			this.groupBoxChuckPosition.Controls.Add(this.labelChuckPos);
			this.groupBoxChuckPosition.Controls.Add(this.buttonTeachChuckPos);
			this.groupBoxChuckPosition.ForeColor = System.Drawing.Color.White;
			this.groupBoxChuckPosition.Location = new System.Drawing.Point(12, 3);
			this.groupBoxChuckPosition.Name = "groupBoxChuckPosition";
			this.groupBoxChuckPosition.Size = new System.Drawing.Size(341, 160);
			this.groupBoxChuckPosition.TabIndex = 9;
			this.groupBoxChuckPosition.TabStop = false;
			this.groupBoxChuckPosition.Text = "Chuck Position";
			// 
			// buttonGotoChuckPos
			// 
			this.buttonGotoChuckPos.BackColor = System.Drawing.SystemColors.Control;
			this.buttonGotoChuckPos.Font = new System.Drawing.Font("Tahoma", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.buttonGotoChuckPos.ForeColor = System.Drawing.SystemColors.ControlText;
			this.buttonGotoChuckPos.Location = new System.Drawing.Point(10, 97);
			this.buttonGotoChuckPos.Name = "buttonGotoChuckPos";
			this.buttonGotoChuckPos.Size = new System.Drawing.Size(125, 52);
			this.buttonGotoChuckPos.TabIndex = 10;
			this.buttonGotoChuckPos.Text = "GoTo";
			this.buttonGotoChuckPos.UseVisualStyleBackColor = false;
			this.buttonGotoChuckPos.Click += new System.EventHandler(this.buttonGotoChuckPos_Click);
			// 
			// labelChuckPos
			// 
			this.labelChuckPos.AutoSize = true;
			this.labelChuckPos.BackColor = System.Drawing.Color.Transparent;
			this.labelChuckPos.Font = new System.Drawing.Font("Tahoma", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelChuckPos.ForeColor = System.Drawing.Color.White;
			this.labelChuckPos.Location = new System.Drawing.Point(143, 46);
			this.labelChuckPos.Name = "labelChuckPos";
			this.labelChuckPos.Size = new System.Drawing.Size(193, 39);
			this.labelChuckPos.TabIndex = 9;
			this.labelChuckPos.Text = "000.000 mm";
			this.labelChuckPos.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// groupBoxLoadPosition
			// 
			this.groupBoxLoadPosition.BackColor = System.Drawing.Color.Transparent;
			this.groupBoxLoadPosition.Controls.Add(this.buttonGotoLoadPos);
			this.groupBoxLoadPosition.Controls.Add(this.labelStackPos);
			this.groupBoxLoadPosition.Controls.Add(this.buttonTeachLoadPos);
			this.groupBoxLoadPosition.ForeColor = System.Drawing.Color.White;
			this.groupBoxLoadPosition.Location = new System.Drawing.Point(12, 163);
			this.groupBoxLoadPosition.Name = "groupBoxLoadPosition";
			this.groupBoxLoadPosition.Size = new System.Drawing.Size(341, 160);
			this.groupBoxLoadPosition.TabIndex = 10;
			this.groupBoxLoadPosition.TabStop = false;
			this.groupBoxLoadPosition.Text = "(Un)Load Position";
			// 
			// buttonGotoLoadPos
			// 
			this.buttonGotoLoadPos.BackColor = System.Drawing.SystemColors.Control;
			this.buttonGotoLoadPos.Font = new System.Drawing.Font("Tahoma", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.buttonGotoLoadPos.ForeColor = System.Drawing.SystemColors.ControlText;
			this.buttonGotoLoadPos.Location = new System.Drawing.Point(10, 97);
			this.buttonGotoLoadPos.Name = "buttonGotoLoadPos";
			this.buttonGotoLoadPos.Size = new System.Drawing.Size(125, 52);
			this.buttonGotoLoadPos.TabIndex = 11;
			this.buttonGotoLoadPos.Text = "GoTo";
			this.buttonGotoLoadPos.UseVisualStyleBackColor = false;
			this.buttonGotoLoadPos.Click += new System.EventHandler(this.buttonGotoLoadPos_Click);
			// 
			// labelStackPos
			// 
			this.labelStackPos.AutoSize = true;
			this.labelStackPos.BackColor = System.Drawing.Color.Transparent;
			this.labelStackPos.Font = new System.Drawing.Font("Tahoma", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelStackPos.ForeColor = System.Drawing.Color.White;
			this.labelStackPos.Location = new System.Drawing.Point(143, 46);
			this.labelStackPos.Name = "labelStackPos";
			this.labelStackPos.Size = new System.Drawing.Size(193, 39);
			this.labelStackPos.TabIndex = 9;
			this.labelStackPos.Text = "000.000 mm";
			this.labelStackPos.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// buttonTeachLoadPos
			// 
			this.buttonTeachLoadPos.BackColor = System.Drawing.SystemColors.Control;
			this.buttonTeachLoadPos.Font = new System.Drawing.Font("Tahoma", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.buttonTeachLoadPos.ForeColor = System.Drawing.SystemColors.ControlText;
			this.buttonTeachLoadPos.Location = new System.Drawing.Point(10, 39);
			this.buttonTeachLoadPos.Name = "buttonTeachLoadPos";
			this.buttonTeachLoadPos.Size = new System.Drawing.Size(125, 52);
			this.buttonTeachLoadPos.TabIndex = 8;
			this.buttonTeachLoadPos.Text = "Teach";
			this.buttonTeachLoadPos.UseVisualStyleBackColor = false;
			this.buttonTeachLoadPos.Click += new System.EventHandler(this.buttonTeachLoadPos_Click);
			// 
			// groupBoxPinsAligners
			// 
			this.groupBoxPinsAligners.BackColor = System.Drawing.Color.Transparent;
			this.groupBoxPinsAligners.Controls.Add(this.pictureBoxAlignersDown);
			this.groupBoxPinsAligners.Controls.Add(this.pictureBoxAlignersUp);
			this.groupBoxPinsAligners.Controls.Add(this.pictureBoxPinsDown);
			this.groupBoxPinsAligners.Controls.Add(this.pictureBoxPinsUp);
			this.groupBoxPinsAligners.Controls.Add(this.buttonAligners);
			this.groupBoxPinsAligners.Controls.Add(this.buttonLiftPins);
			this.groupBoxPinsAligners.ForeColor = System.Drawing.Color.White;
			this.groupBoxPinsAligners.Location = new System.Drawing.Point(12, 323);
			this.groupBoxPinsAligners.Name = "groupBoxPinsAligners";
			this.groupBoxPinsAligners.Size = new System.Drawing.Size(341, 160);
			this.groupBoxPinsAligners.TabIndex = 11;
			this.groupBoxPinsAligners.TabStop = false;
			this.groupBoxPinsAligners.Text = "Lift Pins/Aligners";
			// 
			// buttonAligners
			// 
			this.buttonAligners.BackColor = System.Drawing.SystemColors.Control;
			this.buttonAligners.Font = new System.Drawing.Font("Tahoma", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.buttonAligners.ForeColor = System.Drawing.SystemColors.ControlText;
			this.buttonAligners.Location = new System.Drawing.Point(56, 97);
			this.buttonAligners.Name = "buttonAligners";
			this.buttonAligners.Size = new System.Drawing.Size(235, 52);
			this.buttonAligners.TabIndex = 11;
			this.buttonAligners.Text = "Raise Aligners";
			this.buttonAligners.UseVisualStyleBackColor = false;
			this.buttonAligners.Click += new System.EventHandler(this.buttonAligners_Click);
			// 
			// buttonLiftPins
			// 
			this.buttonLiftPins.BackColor = System.Drawing.SystemColors.Control;
			this.buttonLiftPins.Font = new System.Drawing.Font("Tahoma", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.buttonLiftPins.ForeColor = System.Drawing.SystemColors.ControlText;
			this.buttonLiftPins.Location = new System.Drawing.Point(56, 39);
			this.buttonLiftPins.Name = "buttonLiftPins";
			this.buttonLiftPins.Size = new System.Drawing.Size(235, 52);
			this.buttonLiftPins.TabIndex = 8;
			this.buttonLiftPins.Text = "Raise Pins";
			this.buttonLiftPins.UseVisualStyleBackColor = false;
			this.buttonLiftPins.Click += new System.EventHandler(this.buttonLiftPins_Click);
			// 
			// panelTransferAxis
			// 
			this.panelTransferAxis.BackColor = System.Drawing.Color.LightSkyBlue;
			this.panelTransferAxis.Controls.Add(this.labelSubstrate);
			this.panelTransferAxis.Controls.Add(this.panel1);
			this.panelTransferAxis.Controls.Add(this.panelXAxis);
			this.panelTransferAxis.Location = new System.Drawing.Point(12, 8);
			this.panelTransferAxis.Name = "panelTransferAxis";
			this.panelTransferAxis.Size = new System.Drawing.Size(1000, 494);
			this.panelTransferAxis.TabIndex = 12;
			// 
			// labelSubstrate
			// 
			this.labelSubstrate.BackColor = System.Drawing.Color.Lime;
			this.labelSubstrate.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.labelSubstrate.Location = new System.Drawing.Point(467, 37);
			this.labelSubstrate.Name = "labelSubstrate";
			this.labelSubstrate.Size = new System.Drawing.Size(152, 41);
			this.labelSubstrate.TabIndex = 13;
			this.labelSubstrate.Text = "Substrate";
			this.labelSubstrate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.labelSubstrate.Visible = false;
			// 
			// panel1
			// 
			this.panel1.BackColor = System.Drawing.Color.SteelBlue;
			this.panel1.Controls.Add(this.groupBoxChuckPosition);
			this.panel1.Controls.Add(this.groupBoxLoadPosition);
			this.panel1.Controls.Add(this.groupBoxPinsAligners);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
			this.panel1.Location = new System.Drawing.Point(637, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(363, 494);
			this.panel1.TabIndex = 12;
			// 
			// pictureBoxPinsUp
			// 
			this.pictureBoxPinsUp.BackColor = System.Drawing.Color.Red;
			this.pictureBoxPinsUp.Location = new System.Drawing.Point(297, 41);
			this.pictureBoxPinsUp.Name = "pictureBoxPinsUp";
			this.pictureBoxPinsUp.Size = new System.Drawing.Size(17, 18);
			this.pictureBoxPinsUp.TabIndex = 14;
			this.pictureBoxPinsUp.TabStop = false;
			// 
			// pictureBoxPinsDown
			// 
			this.pictureBoxPinsDown.BackColor = System.Drawing.Color.Red;
			this.pictureBoxPinsDown.Location = new System.Drawing.Point(297, 71);
			this.pictureBoxPinsDown.Name = "pictureBoxPinsDown";
			this.pictureBoxPinsDown.Size = new System.Drawing.Size(17, 18);
			this.pictureBoxPinsDown.TabIndex = 15;
			this.pictureBoxPinsDown.TabStop = false;
			// 
			// pictureBoxAlignersUp
			// 
			this.pictureBoxAlignersUp.BackColor = System.Drawing.Color.Red;
			this.pictureBoxAlignersUp.Location = new System.Drawing.Point(297, 99);
			this.pictureBoxAlignersUp.Name = "pictureBoxAlignersUp";
			this.pictureBoxAlignersUp.Size = new System.Drawing.Size(17, 18);
			this.pictureBoxAlignersUp.TabIndex = 16;
			this.pictureBoxAlignersUp.TabStop = false;
			// 
			// pictureBoxAlignersDown
			// 
			this.pictureBoxAlignersDown.BackColor = System.Drawing.Color.Red;
			this.pictureBoxAlignersDown.Location = new System.Drawing.Point(297, 129);
			this.pictureBoxAlignersDown.Name = "pictureBoxAlignersDown";
			this.pictureBoxAlignersDown.Size = new System.Drawing.Size(17, 18);
			this.pictureBoxAlignersDown.TabIndex = 17;
			this.pictureBoxAlignersDown.TabStop = false;
			// 
			// FormJogTransfer
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.BackColor = System.Drawing.SystemColors.Control;
			this.ClientSize = new System.Drawing.Size(1024, 514);
			this.Controls.Add(this.panelTransferAxis);
			this.Font = new System.Drawing.Font("Tahoma", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Margin = new System.Windows.Forms.Padding(5);
			this.Name = "FormJogTransfer";
			this.Text = "Transfer Axis";
			this.panelXAxis.ResumeLayout(false);
			this.groupBoxChuckPosition.ResumeLayout(false);
			this.groupBoxChuckPosition.PerformLayout();
			this.groupBoxLoadPosition.ResumeLayout(false);
			this.groupBoxLoadPosition.PerformLayout();
			this.groupBoxPinsAligners.ResumeLayout(false);
			this.panelTransferAxis.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxPinsUp)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxPinsDown)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxAlignersUp)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxAlignersDown)).EndInit();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelXAxis;
        private System.Windows.Forms.Label labelXJogDistance;
        private System.Windows.Forms.Label labelXJogSpeed;
        private System.Windows.Forms.Label labelPos;
        private System.Windows.Forms.Button buttonJogDistance;
        private System.Windows.Forms.Button buttonJogSpeed;
        private System.Windows.Forms.Button buttonJogFwd;
        private System.Windows.Forms.Button buttonJogRev;
        private System.Windows.Forms.GroupBox groupBoxLoadPosition;
        private System.Windows.Forms.Label labelStackPos;
        private System.Windows.Forms.Button buttonTeachLoadPos;
        private System.Windows.Forms.GroupBox groupBoxChuckPosition;
        private System.Windows.Forms.Label labelChuckPos;
        private System.Windows.Forms.Button buttonTeachChuckPos;
        private System.Windows.Forms.Button buttonGotoChuckPos;
        private System.Windows.Forms.Button buttonGotoLoadPos;
        private System.Windows.Forms.GroupBox groupBoxPinsAligners;
        private System.Windows.Forms.Button buttonAligners;
        private System.Windows.Forms.Button buttonLiftPins;
		private System.Windows.Forms.Panel panelTransferAxis;
		private System.Windows.Forms.Label labelSubstrate;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Label labelInMotion;
		private System.Windows.Forms.PictureBox pictureBoxAlignersDown;
		private System.Windows.Forms.PictureBox pictureBoxAlignersUp;
		private System.Windows.Forms.PictureBox pictureBoxPinsDown;
		private System.Windows.Forms.PictureBox pictureBoxPinsUp;
	}
}