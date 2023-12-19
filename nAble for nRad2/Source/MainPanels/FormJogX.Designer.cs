namespace nAble
{
    partial class FormJogX
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
            this.buttonLocations = new System.Windows.Forms.Button();
            this.panelAirKnife = new System.Windows.Forms.Panel();
            this.buttonAirKnife = new System.Windows.Forms.Button();
            this.buttonAbortJog = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.buttonZeroGT2s = new System.Windows.Forms.Button();
            this.buttonMeasure = new System.Windows.Forms.Button();
            this.labelKeyenceRight = new System.Windows.Forms.Label();
            this.labelKeyenceLeftVal = new System.Windows.Forms.Label();
            this.labelKeyenceLeft = new System.Windows.Forms.Label();
            this.labelKeyenceRightVal = new System.Windows.Forms.Label();
            this.panelXJog = new System.Windows.Forms.Panel();
            this.labelJogXTorque = new System.Windows.Forms.Label();
            this.labelJogXTorqueVal = new System.Windows.Forms.Label();
            this.buttonJogXFwd = new System.Windows.Forms.Button();
            this.labelInMotion = new System.Windows.Forms.Label();
            this.buttonJogXRev = new System.Windows.Forms.Button();
            this.labelXPosError = new System.Windows.Forms.Label();
            this.buttonXJogSpeed = new System.Windows.Forms.Button();
            this.buttonXJogDistance = new System.Windows.Forms.Button();
            this.labelXPos = new System.Windows.Forms.Label();
            this.labelXJogSpeed = new System.Windows.Forms.Label();
            this.labelXJogDistance = new System.Windows.Forms.Label();
            this.panelXAxis.SuspendLayout();
            this.panelAirKnife.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panelXJog.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelXAxis
            // 
            this.panelXAxis.BackColor = System.Drawing.Color.LightSkyBlue;
            this.panelXAxis.Controls.Add(this.buttonLocations);
            this.panelXAxis.Controls.Add(this.panelAirKnife);
            this.panelXAxis.Controls.Add(this.buttonAbortJog);
            this.panelXAxis.Controls.Add(this.panel2);
            this.panelXAxis.Controls.Add(this.panelXJog);
            this.panelXAxis.Location = new System.Drawing.Point(12, 8);
            this.panelXAxis.Name = "panelXAxis";
            this.panelXAxis.Size = new System.Drawing.Size(1000, 494);
            this.panelXAxis.TabIndex = 4;
            // 
            // buttonLocations
            // 
            this.buttonLocations.BackColor = System.Drawing.SystemColors.Control;
            this.buttonLocations.Font = new System.Drawing.Font("Tahoma", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonLocations.Location = new System.Drawing.Point(584, 426);
            this.buttonLocations.Name = "buttonLocations";
            this.buttonLocations.Size = new System.Drawing.Size(303, 57);
            this.buttonLocations.TabIndex = 28;
            this.buttonLocations.Tag = "0";
            this.buttonLocations.Text = "Go To Locations";
            this.buttonLocations.UseVisualStyleBackColor = false;
            this.buttonLocations.Click += new System.EventHandler(this.buttonLocations_Click);
            // 
            // panelAirKnife
            // 
            this.panelAirKnife.BackColor = System.Drawing.Color.SteelBlue;
            this.panelAirKnife.Controls.Add(this.buttonAirKnife);
            this.panelAirKnife.Location = new System.Drawing.Point(532, 253);
            this.panelAirKnife.Name = "panelAirKnife";
            this.panelAirKnife.Size = new System.Drawing.Size(406, 98);
            this.panelAirKnife.TabIndex = 27;
            this.panelAirKnife.Visible = false;
            // 
            // buttonAirKnife
            // 
            this.buttonAirKnife.BackColor = System.Drawing.SystemColors.Control;
            this.buttonAirKnife.Font = new System.Drawing.Font("Tahoma", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonAirKnife.Location = new System.Drawing.Point(52, 20);
            this.buttonAirKnife.Name = "buttonAirKnife";
            this.buttonAirKnife.Size = new System.Drawing.Size(303, 57);
            this.buttonAirKnife.TabIndex = 25;
            this.buttonAirKnife.Tag = "0";
            this.buttonAirKnife.Text = "Enable Air Knife";
            this.buttonAirKnife.UseVisualStyleBackColor = false;
            this.buttonAirKnife.Click += new System.EventHandler(this.buttonAirKnife_Click);
            // 
            // buttonAbortJog
            // 
            this.buttonAbortJog.BackColor = System.Drawing.Color.Red;
            this.buttonAbortJog.Font = new System.Drawing.Font("Tahoma", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonAbortJog.Location = new System.Drawing.Point(584, 360);
            this.buttonAbortJog.Name = "buttonAbortJog";
            this.buttonAbortJog.Size = new System.Drawing.Size(303, 57);
            this.buttonAbortJog.TabIndex = 23;
            this.buttonAbortJog.Tag = "0";
            this.buttonAbortJog.Text = "Abort Jog";
            this.buttonAbortJog.UseVisualStyleBackColor = false;
            this.buttonAbortJog.Visible = false;
            this.buttonAbortJog.Click += new System.EventHandler(this.buttonAbortJog_Click);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.SteelBlue;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.buttonZeroGT2s);
            this.panel2.Controls.Add(this.labelKeyenceRight);
            this.panel2.Controls.Add(this.labelKeyenceLeftVal);
            this.panel2.Controls.Add(this.buttonMeasure);
            this.panel2.Controls.Add(this.labelKeyenceLeft);
            this.panel2.Controls.Add(this.labelKeyenceRightVal);
            this.panel2.Location = new System.Drawing.Point(532, 18);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(406, 229);
            this.panel2.TabIndex = 22;
            // 
            // buttonZeroGT2s
            // 
            this.buttonZeroGT2s.BackColor = System.Drawing.SystemColors.Control;
            this.buttonZeroGT2s.Font = new System.Drawing.Font("Tahoma", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonZeroGT2s.Location = new System.Drawing.Point(51, 162);
            this.buttonZeroGT2s.Name = "buttonZeroGT2s";
            this.buttonZeroGT2s.Size = new System.Drawing.Size(303, 57);
            this.buttonZeroGT2s.TabIndex = 15;
            this.buttonZeroGT2s.Tag = "0";
            this.buttonZeroGT2s.Text = "Zero GT2s";
            this.buttonZeroGT2s.UseVisualStyleBackColor = false;
            this.buttonZeroGT2s.Click += new System.EventHandler(this.buttonZeroGT2s_Click);
            // 
            // buttonMeasure
            // 
            this.buttonMeasure.BackColor = System.Drawing.SystemColors.Control;
            this.buttonMeasure.Font = new System.Drawing.Font("Tahoma", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonMeasure.Location = new System.Drawing.Point(51, 98);
            this.buttonMeasure.Name = "buttonMeasure";
            this.buttonMeasure.Size = new System.Drawing.Size(303, 57);
            this.buttonMeasure.TabIndex = 8;
            this.buttonMeasure.Tag = "0";
            this.buttonMeasure.Text = "Measure Height";
            this.buttonMeasure.UseVisualStyleBackColor = false;
            this.buttonMeasure.Click += new System.EventHandler(this.buttonMeasure_Click);
            // 
            // labelKeyenceRight
            // 
            this.labelKeyenceRight.Font = new System.Drawing.Font("Tahoma", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelKeyenceRight.ForeColor = System.Drawing.Color.White;
            this.labelKeyenceRight.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.labelKeyenceRight.Location = new System.Drawing.Point(202, 13);
            this.labelKeyenceRight.Name = "labelKeyenceRight";
            this.labelKeyenceRight.Size = new System.Drawing.Size(160, 33);
            this.labelKeyenceRight.TabIndex = 9;
            this.labelKeyenceRight.Text = "Right";
            this.labelKeyenceRight.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelKeyenceLeftVal
            // 
            this.labelKeyenceLeftVal.BackColor = System.Drawing.Color.Black;
            this.labelKeyenceLeftVal.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.labelKeyenceLeftVal.Font = new System.Drawing.Font("Tahoma", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelKeyenceLeftVal.ForeColor = System.Drawing.Color.Yellow;
            this.labelKeyenceLeftVal.Location = new System.Drawing.Point(51, 46);
            this.labelKeyenceLeftVal.Name = "labelKeyenceLeftVal";
            this.labelKeyenceLeftVal.Size = new System.Drawing.Size(145, 41);
            this.labelKeyenceLeftVal.TabIndex = 14;
            this.labelKeyenceLeftVal.Text = "000.000";
            this.labelKeyenceLeftVal.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelKeyenceLeft
            // 
            this.labelKeyenceLeft.Font = new System.Drawing.Font("Tahoma", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelKeyenceLeft.ForeColor = System.Drawing.Color.White;
            this.labelKeyenceLeft.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.labelKeyenceLeft.Location = new System.Drawing.Point(40, 13);
            this.labelKeyenceLeft.Name = "labelKeyenceLeft";
            this.labelKeyenceLeft.Size = new System.Drawing.Size(163, 33);
            this.labelKeyenceLeft.TabIndex = 10;
            this.labelKeyenceLeft.Text = "Left";
            this.labelKeyenceLeft.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelKeyenceRightVal
            // 
            this.labelKeyenceRightVal.BackColor = System.Drawing.Color.Black;
            this.labelKeyenceRightVal.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.labelKeyenceRightVal.Font = new System.Drawing.Font("Tahoma", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelKeyenceRightVal.ForeColor = System.Drawing.Color.Yellow;
            this.labelKeyenceRightVal.Location = new System.Drawing.Point(209, 46);
            this.labelKeyenceRightVal.Name = "labelKeyenceRightVal";
            this.labelKeyenceRightVal.Size = new System.Drawing.Size(145, 41);
            this.labelKeyenceRightVal.TabIndex = 13;
            this.labelKeyenceRightVal.Text = "000.000";
            this.labelKeyenceRightVal.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panelXJog
            // 
            this.panelXJog.BackColor = System.Drawing.Color.SteelBlue;
            this.panelXJog.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelXJog.Controls.Add(this.labelJogXTorque);
            this.panelXJog.Controls.Add(this.labelJogXTorqueVal);
            this.panelXJog.Controls.Add(this.buttonJogXFwd);
            this.panelXJog.Controls.Add(this.labelInMotion);
            this.panelXJog.Controls.Add(this.buttonJogXRev);
            this.panelXJog.Controls.Add(this.labelXPosError);
            this.panelXJog.Controls.Add(this.buttonXJogSpeed);
            this.panelXJog.Controls.Add(this.buttonXJogDistance);
            this.panelXJog.Controls.Add(this.labelXPos);
            this.panelXJog.Controls.Add(this.labelXJogSpeed);
            this.panelXJog.Controls.Add(this.labelXJogDistance);
            this.panelXJog.Location = new System.Drawing.Point(21, 18);
            this.panelXJog.Name = "panelXJog";
            this.panelXJog.Size = new System.Drawing.Size(424, 460);
            this.panelXJog.TabIndex = 21;
            // 
            // labelJogXTorque
            // 
            this.labelJogXTorque.AutoSize = true;
            this.labelJogXTorque.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelJogXTorque.ForeColor = System.Drawing.Color.Yellow;
            this.labelJogXTorque.Location = new System.Drawing.Point(36, 438);
            this.labelJogXTorque.Name = "labelJogXTorque";
            this.labelJogXTorque.Size = new System.Drawing.Size(60, 18);
            this.labelJogXTorque.TabIndex = 225;
            this.labelJogXTorque.Text = "Torque:";
            this.labelJogXTorque.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelJogXTorqueVal
            // 
            this.labelJogXTorqueVal.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelJogXTorqueVal.ForeColor = System.Drawing.Color.Yellow;
            this.labelJogXTorqueVal.Location = new System.Drawing.Point(82, 438);
            this.labelJogXTorqueVal.Name = "labelJogXTorqueVal";
            this.labelJogXTorqueVal.Size = new System.Drawing.Size(55, 18);
            this.labelJogXTorqueVal.TabIndex = 226;
            this.labelJogXTorqueVal.Text = "0.000";
            this.labelJogXTorqueVal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // buttonJogXFwd
            // 
            this.buttonJogXFwd.BackColor = System.Drawing.SystemColors.Control;
            this.buttonJogXFwd.Font = new System.Drawing.Font("Tahoma", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonJogXFwd.Location = new System.Drawing.Point(39, 17);
            this.buttonJogXFwd.Name = "buttonJogXFwd";
            this.buttonJogXFwd.Size = new System.Drawing.Size(159, 98);
            this.buttonJogXFwd.TabIndex = 2;
            this.buttonJogXFwd.Text = "Jog Fwd";
            this.buttonJogXFwd.UseVisualStyleBackColor = false;
            this.buttonJogXFwd.Click += new System.EventHandler(this.buttonJogXFwd_Click);
            // 
            // labelInMotion
            // 
            this.labelInMotion.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelInMotion.ForeColor = System.Drawing.Color.Yellow;
            this.labelInMotion.Location = new System.Drawing.Point(36, 330);
            this.labelInMotion.Name = "labelInMotion";
            this.labelInMotion.Size = new System.Drawing.Size(86, 18);
            this.labelInMotion.TabIndex = 20;
            this.labelInMotion.Text = "In Motion";
            this.labelInMotion.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelInMotion.Visible = false;
            // 
            // buttonJogXRev
            // 
            this.buttonJogXRev.BackColor = System.Drawing.SystemColors.Control;
            this.buttonJogXRev.Font = new System.Drawing.Font("Tahoma", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonJogXRev.Location = new System.Drawing.Point(229, 17);
            this.buttonJogXRev.Name = "buttonJogXRev";
            this.buttonJogXRev.Size = new System.Drawing.Size(159, 98);
            this.buttonJogXRev.TabIndex = 1;
            this.buttonJogXRev.Text = "Jog Rev";
            this.buttonJogXRev.UseVisualStyleBackColor = false;
            this.buttonJogXRev.Click += new System.EventHandler(this.buttonJogXRev_Click);
            // 
            // labelXPosError
            // 
            this.labelXPosError.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelXPosError.ForeColor = System.Drawing.Color.Yellow;
            this.labelXPosError.Location = new System.Drawing.Point(252, 434);
            this.labelXPosError.Name = "labelXPosError";
            this.labelXPosError.Size = new System.Drawing.Size(137, 24);
            this.labelXPosError.TabIndex = 19;
            this.labelXPosError.Text = "Max Err: 0.000";
            this.labelXPosError.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // buttonXJogSpeed
            // 
            this.buttonXJogSpeed.BackColor = System.Drawing.SystemColors.Control;
            this.buttonXJogSpeed.Font = new System.Drawing.Font("Tahoma", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonXJogSpeed.Location = new System.Drawing.Point(229, 130);
            this.buttonXJogSpeed.Name = "buttonXJogSpeed";
            this.buttonXJogSpeed.Size = new System.Drawing.Size(159, 98);
            this.buttonXJogSpeed.TabIndex = 3;
            this.buttonXJogSpeed.Tag = "0";
            this.buttonXJogSpeed.Text = "50";
            this.buttonXJogSpeed.UseVisualStyleBackColor = false;
            this.buttonXJogSpeed.Click += new System.EventHandler(this.buttonXJogSpeed_Click);
            // 
            // buttonXJogDistance
            // 
            this.buttonXJogDistance.BackColor = System.Drawing.SystemColors.Control;
            this.buttonXJogDistance.Font = new System.Drawing.Font("Tahoma", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonXJogDistance.Location = new System.Drawing.Point(229, 236);
            this.buttonXJogDistance.Name = "buttonXJogDistance";
            this.buttonXJogDistance.Size = new System.Drawing.Size(159, 98);
            this.buttonXJogDistance.TabIndex = 4;
            this.buttonXJogDistance.Tag = "0";
            this.buttonXJogDistance.Text = "000.000";
            this.buttonXJogDistance.UseVisualStyleBackColor = false;
            this.buttonXJogDistance.Click += new System.EventHandler(this.buttonXJogDistance_Click);
            // 
            // labelXPos
            // 
            this.labelXPos.BackColor = System.Drawing.Color.Black;
            this.labelXPos.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.labelXPos.Font = new System.Drawing.Font("Tahoma", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelXPos.ForeColor = System.Drawing.Color.Red;
            this.labelXPos.Location = new System.Drawing.Point(37, 348);
            this.labelXPos.Name = "labelXPos";
            this.labelXPos.Size = new System.Drawing.Size(353, 85);
            this.labelXPos.TabIndex = 5;
            this.labelXPos.Text = "000.000 mm";
            this.labelXPos.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelXJogSpeed
            // 
            this.labelXJogSpeed.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelXJogSpeed.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.labelXJogSpeed.Location = new System.Drawing.Point(48, 139);
            this.labelXJogSpeed.Name = "labelXJogSpeed";
            this.labelXJogSpeed.Size = new System.Drawing.Size(170, 80);
            this.labelXJogSpeed.TabIndex = 6;
            this.labelXJogSpeed.Text = "Jog Speed\r\n(mm/sec)";
            this.labelXJogSpeed.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelXJogDistance
            // 
            this.labelXJogDistance.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelXJogDistance.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.labelXJogDistance.Location = new System.Drawing.Point(48, 245);
            this.labelXJogDistance.Name = "labelXJogDistance";
            this.labelXJogDistance.Size = new System.Drawing.Size(170, 80);
            this.labelXJogDistance.TabIndex = 7;
            this.labelXJogDistance.Text = "Jog Distance\r\n(mm)";
            this.labelXJogDistance.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // FormJogX
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1024, 514);
            this.Controls.Add(this.panelXAxis);
            this.Font = new System.Drawing.Font("Tahoma", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(5);
            this.Name = "FormJogX";
            this.Text = "X Axis";
            this.Activated += new System.EventHandler(this.FormJogX_Activated);
            this.Deactivate += new System.EventHandler(this.FormJogX_Deactivate);
            this.Load += new System.EventHandler(this.FormJogX_Load);
            this.Enter += new System.EventHandler(this.FormJogX_Enter);
            this.Leave += new System.EventHandler(this.FormJogX_Leave);
            this.panelXAxis.ResumeLayout(false);
            this.panelAirKnife.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panelXJog.ResumeLayout(false);
            this.panelXJog.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelXAxis;
        private System.Windows.Forms.Label labelXJogDistance;
        private System.Windows.Forms.Label labelXJogSpeed;
        private System.Windows.Forms.Label labelXPos;
        private System.Windows.Forms.Button buttonXJogDistance;
        private System.Windows.Forms.Button buttonXJogSpeed;
        private System.Windows.Forms.Button buttonJogXFwd;
        private System.Windows.Forms.Button buttonJogXRev;
        private System.Windows.Forms.Button buttonMeasure;
        private System.Windows.Forms.Label labelKeyenceLeftVal;
        private System.Windows.Forms.Label labelKeyenceRightVal;
        private System.Windows.Forms.Label labelKeyenceLeft;
        private System.Windows.Forms.Label labelKeyenceRight;
        private System.Windows.Forms.Label labelXPosError;
        private System.Windows.Forms.Label labelInMotion;
        private System.Windows.Forms.Panel panelXJog;
        private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Button buttonAbortJog;
		private System.Windows.Forms.Panel panelAirKnife;
		private System.Windows.Forms.Button buttonAirKnife;
		private System.Windows.Forms.Label labelJogXTorque;
		private System.Windows.Forms.Label labelJogXTorqueVal;
        private System.Windows.Forms.Button buttonLocations;
        private System.Windows.Forms.Button buttonZeroGT2s;
    }
}