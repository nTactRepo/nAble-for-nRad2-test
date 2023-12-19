namespace nAble
{
    partial class FormRecipeEditAdvanced
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
            this.components = new System.ComponentModel.Container();
            this.buttonSCurve = new System.Windows.Forms.Button();
            this.buttonDecel = new System.Windows.Forms.Button();
            this.buttonAccel = new System.Windows.Forms.Button();
            this.labelSCurveUnits = new System.Windows.Forms.Label();
            this.labelSCurve = new System.Windows.Forms.Label();
            this.labelDecelUnits = new System.Windows.Forms.Label();
            this.labelAccelUnits = new System.Windows.Forms.Label();
            this.labelDecel = new System.Windows.Forms.Label();
            this.labelAccel = new System.Windows.Forms.Label();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.labelTitle = new System.Windows.Forms.Label();
            this.timerUpdate = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // buttonSCurve
            // 
            this.buttonSCurve.Location = new System.Drawing.Point(434, 284);
            this.buttonSCurve.Margin = new System.Windows.Forms.Padding(4);
            this.buttonSCurve.Name = "buttonSCurve";
            this.buttonSCurve.Size = new System.Drawing.Size(155, 57);
            this.buttonSCurve.TabIndex = 129;
            this.buttonSCurve.Text = "100";
            this.buttonSCurve.UseVisualStyleBackColor = true;
            this.buttonSCurve.Click += new System.EventHandler(this.buttonSCurve_Click);
            // 
            // buttonDecel
            // 
            this.buttonDecel.Location = new System.Drawing.Point(434, 200);
            this.buttonDecel.Margin = new System.Windows.Forms.Padding(4);
            this.buttonDecel.Name = "buttonDecel";
            this.buttonDecel.Size = new System.Drawing.Size(155, 57);
            this.buttonDecel.TabIndex = 128;
            this.buttonDecel.Text = "100.0";
            this.buttonDecel.UseVisualStyleBackColor = true;
            this.buttonDecel.Click += new System.EventHandler(this.buttonDecel_Click);
            // 
            // buttonAccel
            // 
            this.buttonAccel.Location = new System.Drawing.Point(434, 116);
            this.buttonAccel.Margin = new System.Windows.Forms.Padding(4);
            this.buttonAccel.Name = "buttonAccel";
            this.buttonAccel.Size = new System.Drawing.Size(155, 57);
            this.buttonAccel.TabIndex = 127;
            this.buttonAccel.Text = "100.0";
            this.buttonAccel.UseVisualStyleBackColor = true;
            this.buttonAccel.Click += new System.EventHandler(this.buttonAccel_Click);
            // 
            // labelSCurveUnits
            // 
            this.labelSCurveUnits.AutoSize = true;
            this.labelSCurveUnits.ForeColor = System.Drawing.Color.Black;
            this.labelSCurveUnits.Location = new System.Drawing.Point(596, 296);
            this.labelSCurveUnits.Name = "labelSCurveUnits";
            this.labelSCurveUnits.Size = new System.Drawing.Size(41, 33);
            this.labelSCurveUnits.TabIndex = 126;
            this.labelSCurveUnits.Text = "%";
            this.labelSCurveUnits.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // labelSCurve
            // 
            this.labelSCurve.AutoSize = true;
            this.labelSCurve.ForeColor = System.Drawing.Color.Black;
            this.labelSCurve.Location = new System.Drawing.Point(308, 296);
            this.labelSCurve.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelSCurve.Name = "labelSCurve";
            this.labelSCurve.Size = new System.Drawing.Size(118, 33);
            this.labelSCurve.TabIndex = 125;
            this.labelSCurve.Text = "S-Curve:";
            this.labelSCurve.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelDecelUnits
            // 
            this.labelDecelUnits.AutoSize = true;
            this.labelDecelUnits.ForeColor = System.Drawing.Color.Black;
            this.labelDecelUnits.Location = new System.Drawing.Point(596, 212);
            this.labelDecelUnits.Name = "labelDecelUnits";
            this.labelDecelUnits.Size = new System.Drawing.Size(83, 33);
            this.labelDecelUnits.TabIndex = 124;
            this.labelDecelUnits.Text = "mm/s";
            this.labelDecelUnits.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // labelAccelUnits
            // 
            this.labelAccelUnits.AutoSize = true;
            this.labelAccelUnits.ForeColor = System.Drawing.Color.Black;
            this.labelAccelUnits.Location = new System.Drawing.Point(596, 128);
            this.labelAccelUnits.Name = "labelAccelUnits";
            this.labelAccelUnits.Size = new System.Drawing.Size(83, 33);
            this.labelAccelUnits.TabIndex = 123;
            this.labelAccelUnits.Text = "mm/s";
            this.labelAccelUnits.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // labelDecel
            // 
            this.labelDecel.AutoSize = true;
            this.labelDecel.ForeColor = System.Drawing.Color.Black;
            this.labelDecel.Location = new System.Drawing.Point(254, 212);
            this.labelDecel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelDecel.Name = "labelDecel";
            this.labelDecel.Size = new System.Drawing.Size(172, 33);
            this.labelDecel.TabIndex = 122;
            this.labelDecel.Text = "Deceleration:";
            this.labelDecel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelAccel
            // 
            this.labelAccel.AutoSize = true;
            this.labelAccel.ForeColor = System.Drawing.Color.Black;
            this.labelAccel.Location = new System.Drawing.Point(258, 128);
            this.labelAccel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelAccel.Name = "labelAccel";
            this.labelAccel.Size = new System.Drawing.Size(168, 33);
            this.labelAccel.TabIndex = 121;
            this.labelAccel.Text = "Acceleration:";
            this.labelAccel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(585, 406);
            this.buttonCancel.Margin = new System.Windows.Forms.Padding(4);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(155, 57);
            this.buttonCancel.TabIndex = 120;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(281, 406);
            this.buttonOK.Margin = new System.Windows.Forms.Padding(4);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(155, 57);
            this.buttonOK.TabIndex = 119;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // labelTitle
            // 
            this.labelTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelTitle.ForeColor = System.Drawing.Color.Black;
            this.labelTitle.Location = new System.Drawing.Point(0, 0);
            this.labelTitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(1022, 71);
            this.labelTitle.TabIndex = 130;
            this.labelTitle.Text = "Title";
            this.labelTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // timerUpdate
            // 
            this.timerUpdate.Enabled = true;
            this.timerUpdate.Tick += new System.EventHandler(this.timerUpdate_Tick);
            // 
            // FormRecipeEditAdvanced
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1022, 512);
            this.ControlBox = false;
            this.Controls.Add(this.buttonAccel);
            this.Controls.Add(this.labelAccel);
            this.Controls.Add(this.labelDecel);
            this.Controls.Add(this.labelTitle);
            this.Controls.Add(this.buttonDecel);
            this.Controls.Add(this.labelSCurveUnits);
            this.Controls.Add(this.labelAccelUnits);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.labelDecelUnits);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonSCurve);
            this.Controls.Add(this.labelSCurve);
            this.Font = new System.Drawing.Font("Tahoma", 20.25F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "FormRecipeEditAdvanced";
            this.Load += new System.EventHandler(this.FormRecipeEditAdvanced_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonSCurve;
        private System.Windows.Forms.Button buttonDecel;
        private System.Windows.Forms.Button buttonAccel;
        private System.Windows.Forms.Label labelSCurveUnits;
        private System.Windows.Forms.Label labelSCurve;
        private System.Windows.Forms.Label labelDecelUnits;
        private System.Windows.Forms.Label labelAccelUnits;
        private System.Windows.Forms.Label labelDecel;
        private System.Windows.Forms.Label labelAccel;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.Timer timerUpdate;
    }
}