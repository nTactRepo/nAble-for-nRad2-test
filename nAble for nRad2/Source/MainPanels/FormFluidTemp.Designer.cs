namespace nAble
{
    partial class FormFluidTemp
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
            System.Windows.Forms.Label labelDieTempSetPointUnits;
            System.Windows.Forms.Label labelResvSetPointUnits;
            System.Windows.Forms.Label label2;
            this.groupBoxDieTemp = new System.Windows.Forms.GroupBox();
            this.panelDieTempController = new System.Windows.Forms.Panel();
            this.labelDieSetPointChanges = new System.Windows.Forms.Label();
            this.buttonDieSetPointApply = new System.Windows.Forms.Button();
            this.buttonDieTempSetPoint = new System.Windows.Forms.Button();
            this.labelDieTempSetPoint = new System.Windows.Forms.Label();
            this.labelDieTempController = new System.Windows.Forms.Label();
            this.labelDieTempOutput = new System.Windows.Forms.Label();
            this.groupBoxReservoirTemp = new System.Windows.Forms.GroupBox();
            this.panelResvTempController = new System.Windows.Forms.Panel();
            this.buttonResvAdvance = new System.Windows.Forms.Button();
            this.labelResvSetPointChanges = new System.Windows.Forms.Label();
            this.buttonResvSetPointApply = new System.Windows.Forms.Button();
            this.buttonResvSetPoint = new System.Windows.Forms.Button();
            this.labelResvSetupPoint = new System.Windows.Forms.Label();
            this.labelResvTempController = new System.Windows.Forms.Label();
            this.labelResvTempOutput = new System.Windows.Forms.Label();
            this.groupBoxReservoirBTemp = new System.Windows.Forms.GroupBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.buttonResvBAdvance = new System.Windows.Forms.Button();
            this.labelResvBSetPointChanges = new System.Windows.Forms.Label();
            this.buttonResvBSetPointApply = new System.Windows.Forms.Button();
            this.buttonResvBSetPoint = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.labelResvBTempOutput = new System.Windows.Forms.Label();
            labelDieTempSetPointUnits = new System.Windows.Forms.Label();
            labelResvSetPointUnits = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            this.groupBoxDieTemp.SuspendLayout();
            this.panelDieTempController.SuspendLayout();
            this.groupBoxReservoirTemp.SuspendLayout();
            this.panelResvTempController.SuspendLayout();
            this.groupBoxReservoirBTemp.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelDieTempSetPointUnits
            // 
            labelDieTempSetPointUnits.AutoSize = true;
            labelDieTempSetPointUnits.Location = new System.Drawing.Point(498, 37);
            labelDieTempSetPointUnits.Name = "labelDieTempSetPointUnits";
            labelDieTempSetPointUnits.Size = new System.Drawing.Size(44, 33);
            labelDieTempSetPointUnits.TabIndex = 103;
            labelDieTempSetPointUnits.Text = "°C";
            // 
            // labelResvSetPointUnits
            // 
            labelResvSetPointUnits.AutoSize = true;
            labelResvSetPointUnits.Location = new System.Drawing.Point(498, 36);
            labelResvSetPointUnits.Name = "labelResvSetPointUnits";
            labelResvSetPointUnits.Size = new System.Drawing.Size(44, 33);
            labelResvSetPointUnits.TabIndex = 103;
            labelResvSetPointUnits.Text = "°C";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(498, 36);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(44, 33);
            label2.TabIndex = 103;
            label2.Text = "°C";
            // 
            // groupBoxDieTemp
            // 
            this.groupBoxDieTemp.Controls.Add(this.panelDieTempController);
            this.groupBoxDieTemp.ForeColor = System.Drawing.Color.White;
            this.groupBoxDieTemp.Location = new System.Drawing.Point(12, 12);
            this.groupBoxDieTemp.Name = "groupBoxDieTemp";
            this.groupBoxDieTemp.Size = new System.Drawing.Size(1000, 158);
            this.groupBoxDieTemp.TabIndex = 8;
            this.groupBoxDieTemp.TabStop = false;
            this.groupBoxDieTemp.Text = "Die Temperature";
            this.groupBoxDieTemp.Visible = false;
            // 
            // panelDieTempController
            // 
            this.panelDieTempController.BackColor = System.Drawing.SystemColors.ControlDark;
            this.panelDieTempController.Controls.Add(this.labelDieSetPointChanges);
            this.panelDieTempController.Controls.Add(this.buttonDieSetPointApply);
            this.panelDieTempController.Controls.Add(this.buttonDieTempSetPoint);
            this.panelDieTempController.Controls.Add(labelDieTempSetPointUnits);
            this.panelDieTempController.Controls.Add(this.labelDieTempSetPoint);
            this.panelDieTempController.Controls.Add(this.labelDieTempController);
            this.panelDieTempController.Controls.Add(this.labelDieTempOutput);
            this.panelDieTempController.Location = new System.Drawing.Point(13, 39);
            this.panelDieTempController.Name = "panelDieTempController";
            this.panelDieTempController.Size = new System.Drawing.Size(973, 108);
            this.panelDieTempController.TabIndex = 8;
            // 
            // labelDieSetPointChanges
            // 
            this.labelDieSetPointChanges.AutoSize = true;
            this.labelDieSetPointChanges.ForeColor = System.Drawing.Color.Yellow;
            this.labelDieSetPointChanges.Location = new System.Drawing.Point(219, 74);
            this.labelDieSetPointChanges.Name = "labelDieSetPointChanges";
            this.labelDieSetPointChanges.Size = new System.Drawing.Size(394, 33);
            this.labelDieSetPointChanges.TabIndex = 109;
            this.labelDieSetPointChanges.Text = "Changes have not been applied.";
            this.labelDieSetPointChanges.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.labelDieSetPointChanges.Visible = false;
            // 
            // buttonDieSetPointApply
            // 
            this.buttonDieSetPointApply.ForeColor = System.Drawing.SystemColors.ControlText;
            this.buttonDieSetPointApply.Location = new System.Drawing.Point(618, 20);
            this.buttonDieSetPointApply.Name = "buttonDieSetPointApply";
            this.buttonDieSetPointApply.Size = new System.Drawing.Size(114, 66);
            this.buttonDieSetPointApply.TabIndex = 108;
            this.buttonDieSetPointApply.Text = "Apply";
            this.buttonDieSetPointApply.UseVisualStyleBackColor = true;
            this.buttonDieSetPointApply.Click += new System.EventHandler(this.buttonDieSetPointApply_Click);
            // 
            // buttonDieTempSetPoint
            // 
            this.buttonDieTempSetPoint.ForeColor = System.Drawing.SystemColors.ControlText;
            this.buttonDieTempSetPoint.Location = new System.Drawing.Point(401, 33);
            this.buttonDieTempSetPoint.Name = "buttonDieTempSetPoint";
            this.buttonDieTempSetPoint.Size = new System.Drawing.Size(91, 40);
            this.buttonDieTempSetPoint.TabIndex = 104;
            this.buttonDieTempSetPoint.Text = "50.0";
            this.buttonDieTempSetPoint.UseVisualStyleBackColor = true;
            this.buttonDieTempSetPoint.Click += new System.EventHandler(this.buttonDieTempSetPoint_Click);
            // 
            // labelDieTempSetPoint
            // 
            this.labelDieTempSetPoint.Location = new System.Drawing.Point(224, 37);
            this.labelDieTempSetPoint.Name = "labelDieTempSetPoint";
            this.labelDieTempSetPoint.Size = new System.Drawing.Size(171, 33);
            this.labelDieTempSetPoint.TabIndex = 2;
            this.labelDieTempSetPoint.Text = "Set Point:";
            this.labelDieTempSetPoint.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelDieTempController
            // 
            this.labelDieTempController.BackColor = System.Drawing.Color.Black;
            this.labelDieTempController.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelDieTempController.ForeColor = System.Drawing.Color.Lime;
            this.labelDieTempController.Location = new System.Drawing.Point(24, 10);
            this.labelDieTempController.Name = "labelDieTempController";
            this.labelDieTempController.Size = new System.Drawing.Size(173, 29);
            this.labelDieTempController.TabIndex = 1;
            this.labelDieTempController.Text = "Die Temp";
            this.labelDieTempController.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelDieTempOutput
            // 
            this.labelDieTempOutput.BackColor = System.Drawing.Color.Black;
            this.labelDieTempOutput.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelDieTempOutput.ForeColor = System.Drawing.Color.Lime;
            this.labelDieTempOutput.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.labelDieTempOutput.Location = new System.Drawing.Point(24, 39);
            this.labelDieTempOutput.Name = "labelDieTempOutput";
            this.labelDieTempOutput.Size = new System.Drawing.Size(173, 58);
            this.labelDieTempOutput.TabIndex = 0;
            this.labelDieTempOutput.Text = "21° C";
            this.labelDieTempOutput.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // groupBoxReservoirTemp
            // 
            this.groupBoxReservoirTemp.Controls.Add(this.panelResvTempController);
            this.groupBoxReservoirTemp.ForeColor = System.Drawing.Color.White;
            this.groupBoxReservoirTemp.Location = new System.Drawing.Point(12, 176);
            this.groupBoxReservoirTemp.Name = "groupBoxReservoirTemp";
            this.groupBoxReservoirTemp.Size = new System.Drawing.Size(1000, 158);
            this.groupBoxReservoirTemp.TabIndex = 9;
            this.groupBoxReservoirTemp.TabStop = false;
            this.groupBoxReservoirTemp.Text = "Reservoir-A Temperature";
            this.groupBoxReservoirTemp.Visible = false;
            // 
            // panelResvTempController
            // 
            this.panelResvTempController.BackColor = System.Drawing.SystemColors.ControlDark;
            this.panelResvTempController.Controls.Add(this.buttonResvAdvance);
            this.panelResvTempController.Controls.Add(this.labelResvSetPointChanges);
            this.panelResvTempController.Controls.Add(this.buttonResvSetPointApply);
            this.panelResvTempController.Controls.Add(this.buttonResvSetPoint);
            this.panelResvTempController.Controls.Add(labelResvSetPointUnits);
            this.panelResvTempController.Controls.Add(this.labelResvSetupPoint);
            this.panelResvTempController.Controls.Add(this.labelResvTempController);
            this.panelResvTempController.Controls.Add(this.labelResvTempOutput);
            this.panelResvTempController.Location = new System.Drawing.Point(13, 39);
            this.panelResvTempController.Name = "panelResvTempController";
            this.panelResvTempController.Size = new System.Drawing.Size(973, 108);
            this.panelResvTempController.TabIndex = 9;
            // 
            // buttonResvAdvance
            // 
            this.buttonResvAdvance.ForeColor = System.Drawing.SystemColors.ControlText;
            this.buttonResvAdvance.Location = new System.Drawing.Point(808, 62);
            this.buttonResvAdvance.Name = "buttonResvAdvance";
            this.buttonResvAdvance.Size = new System.Drawing.Size(151, 42);
            this.buttonResvAdvance.TabIndex = 10;
            this.buttonResvAdvance.Text = "Adv.";
            this.buttonResvAdvance.UseVisualStyleBackColor = true;
            this.buttonResvAdvance.Click += new System.EventHandler(this.buttonResvAdvance_Click);
            // 
            // labelResvSetPointChanges
            // 
            this.labelResvSetPointChanges.AutoSize = true;
            this.labelResvSetPointChanges.ForeColor = System.Drawing.Color.Yellow;
            this.labelResvSetPointChanges.Location = new System.Drawing.Point(219, 74);
            this.labelResvSetPointChanges.Name = "labelResvSetPointChanges";
            this.labelResvSetPointChanges.Size = new System.Drawing.Size(394, 33);
            this.labelResvSetPointChanges.TabIndex = 109;
            this.labelResvSetPointChanges.Text = "Changes have not been applied.";
            this.labelResvSetPointChanges.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.labelResvSetPointChanges.Visible = false;
            // 
            // buttonResvSetPointApply
            // 
            this.buttonResvSetPointApply.ForeColor = System.Drawing.SystemColors.ControlText;
            this.buttonResvSetPointApply.Location = new System.Drawing.Point(618, 19);
            this.buttonResvSetPointApply.Name = "buttonResvSetPointApply";
            this.buttonResvSetPointApply.Size = new System.Drawing.Size(114, 66);
            this.buttonResvSetPointApply.TabIndex = 108;
            this.buttonResvSetPointApply.Text = "Apply";
            this.buttonResvSetPointApply.UseVisualStyleBackColor = true;
            this.buttonResvSetPointApply.Click += new System.EventHandler(this.buttonResvSetPointApply_Click);
            // 
            // buttonResvSetPoint
            // 
            this.buttonResvSetPoint.ForeColor = System.Drawing.SystemColors.ControlText;
            this.buttonResvSetPoint.Location = new System.Drawing.Point(401, 32);
            this.buttonResvSetPoint.Name = "buttonResvSetPoint";
            this.buttonResvSetPoint.Size = new System.Drawing.Size(91, 40);
            this.buttonResvSetPoint.TabIndex = 104;
            this.buttonResvSetPoint.Text = "50.0";
            this.buttonResvSetPoint.UseVisualStyleBackColor = true;
            this.buttonResvSetPoint.Click += new System.EventHandler(this.buttonResvSetPoint_Click);
            // 
            // labelResvSetupPoint
            // 
            this.labelResvSetupPoint.Location = new System.Drawing.Point(224, 36);
            this.labelResvSetupPoint.Name = "labelResvSetupPoint";
            this.labelResvSetupPoint.Size = new System.Drawing.Size(171, 33);
            this.labelResvSetupPoint.TabIndex = 2;
            this.labelResvSetupPoint.Text = "Set Point:";
            this.labelResvSetupPoint.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelResvTempController
            // 
            this.labelResvTempController.BackColor = System.Drawing.Color.Black;
            this.labelResvTempController.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelResvTempController.ForeColor = System.Drawing.Color.Lime;
            this.labelResvTempController.Location = new System.Drawing.Point(24, 9);
            this.labelResvTempController.Name = "labelResvTempController";
            this.labelResvTempController.Size = new System.Drawing.Size(173, 29);
            this.labelResvTempController.TabIndex = 1;
            this.labelResvTempController.Text = "Fluid Temp";
            this.labelResvTempController.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelResvTempOutput
            // 
            this.labelResvTempOutput.BackColor = System.Drawing.Color.Black;
            this.labelResvTempOutput.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelResvTempOutput.ForeColor = System.Drawing.Color.Lime;
            this.labelResvTempOutput.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.labelResvTempOutput.Location = new System.Drawing.Point(24, 38);
            this.labelResvTempOutput.Name = "labelResvTempOutput";
            this.labelResvTempOutput.Size = new System.Drawing.Size(173, 58);
            this.labelResvTempOutput.TabIndex = 0;
            this.labelResvTempOutput.Text = "21° C";
            this.labelResvTempOutput.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // groupBoxReservoirBTemp
            // 
            this.groupBoxReservoirBTemp.Controls.Add(this.panel1);
            this.groupBoxReservoirBTemp.ForeColor = System.Drawing.Color.White;
            this.groupBoxReservoirBTemp.Location = new System.Drawing.Point(12, 344);
            this.groupBoxReservoirBTemp.Name = "groupBoxReservoirBTemp";
            this.groupBoxReservoirBTemp.Size = new System.Drawing.Size(1000, 158);
            this.groupBoxReservoirBTemp.TabIndex = 10;
            this.groupBoxReservoirBTemp.TabStop = false;
            this.groupBoxReservoirBTemp.Text = "Reservoir-B Temperature";
            this.groupBoxReservoirBTemp.Visible = false;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ControlDark;
            this.panel1.Controls.Add(this.buttonResvBAdvance);
            this.panel1.Controls.Add(this.labelResvBSetPointChanges);
            this.panel1.Controls.Add(this.buttonResvBSetPointApply);
            this.panel1.Controls.Add(this.buttonResvBSetPoint);
            this.panel1.Controls.Add(label2);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.labelResvBTempOutput);
            this.panel1.Location = new System.Drawing.Point(13, 39);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(973, 108);
            this.panel1.TabIndex = 9;
            // 
            // buttonResvBAdvance
            // 
            this.buttonResvBAdvance.ForeColor = System.Drawing.SystemColors.ControlText;
            this.buttonResvBAdvance.Location = new System.Drawing.Point(808, 62);
            this.buttonResvBAdvance.Name = "buttonResvBAdvance";
            this.buttonResvBAdvance.Size = new System.Drawing.Size(151, 42);
            this.buttonResvBAdvance.TabIndex = 10;
            this.buttonResvBAdvance.Text = "Adv.";
            this.buttonResvBAdvance.UseVisualStyleBackColor = true;
            this.buttonResvBAdvance.Click += new System.EventHandler(this.buttonResvBAdvance_Click);
            // 
            // labelResvBSetPointChanges
            // 
            this.labelResvBSetPointChanges.AutoSize = true;
            this.labelResvBSetPointChanges.ForeColor = System.Drawing.Color.Yellow;
            this.labelResvBSetPointChanges.Location = new System.Drawing.Point(219, 74);
            this.labelResvBSetPointChanges.Name = "labelResvBSetPointChanges";
            this.labelResvBSetPointChanges.Size = new System.Drawing.Size(394, 33);
            this.labelResvBSetPointChanges.TabIndex = 109;
            this.labelResvBSetPointChanges.Text = "Changes have not been applied.";
            this.labelResvBSetPointChanges.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.labelResvBSetPointChanges.Visible = false;
            // 
            // buttonResvBSetPointApply
            // 
            this.buttonResvBSetPointApply.ForeColor = System.Drawing.SystemColors.ControlText;
            this.buttonResvBSetPointApply.Location = new System.Drawing.Point(618, 19);
            this.buttonResvBSetPointApply.Name = "buttonResvBSetPointApply";
            this.buttonResvBSetPointApply.Size = new System.Drawing.Size(114, 66);
            this.buttonResvBSetPointApply.TabIndex = 108;
            this.buttonResvBSetPointApply.Text = "Apply";
            this.buttonResvBSetPointApply.UseVisualStyleBackColor = true;
            this.buttonResvBSetPointApply.Click += new System.EventHandler(this.buttonResvBSetPointApply_Click);
            // 
            // buttonResvBSetPoint
            // 
            this.buttonResvBSetPoint.ForeColor = System.Drawing.SystemColors.ControlText;
            this.buttonResvBSetPoint.Location = new System.Drawing.Point(401, 32);
            this.buttonResvBSetPoint.Name = "buttonResvBSetPoint";
            this.buttonResvBSetPoint.Size = new System.Drawing.Size(91, 40);
            this.buttonResvBSetPoint.TabIndex = 104;
            this.buttonResvBSetPoint.Text = "50.0";
            this.buttonResvBSetPoint.UseVisualStyleBackColor = true;
            this.buttonResvBSetPoint.Click += new System.EventHandler(this.buttonResvBSetPoint_Click);
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(224, 36);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(171, 33);
            this.label3.TabIndex = 2;
            this.label3.Text = "Set Point:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label4
            // 
            this.label4.BackColor = System.Drawing.Color.Black;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.Lime;
            this.label4.Location = new System.Drawing.Point(24, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(173, 29);
            this.label4.TabIndex = 1;
            this.label4.Text = "Fluid Temp";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelResvBTempOutput
            // 
            this.labelResvBTempOutput.BackColor = System.Drawing.Color.Black;
            this.labelResvBTempOutput.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelResvBTempOutput.ForeColor = System.Drawing.Color.Lime;
            this.labelResvBTempOutput.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.labelResvBTempOutput.Location = new System.Drawing.Point(24, 38);
            this.labelResvBTempOutput.Name = "labelResvBTempOutput";
            this.labelResvBTempOutput.Size = new System.Drawing.Size(173, 58);
            this.labelResvBTempOutput.TabIndex = 0;
            this.labelResvBTempOutput.Text = "21° C";
            this.labelResvBTempOutput.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // FormFluidTemp
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.SteelBlue;
            this.ClientSize = new System.Drawing.Size(1024, 514);
            this.Controls.Add(this.groupBoxReservoirBTemp);
            this.Controls.Add(this.groupBoxReservoirTemp);
            this.Controls.Add(this.groupBoxDieTemp);
            this.Font = new System.Drawing.Font("Tahoma", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(5);
            this.Name = "FormFluidTemp";
            this.Text = "Fluid Temperature";
            this.Load += new System.EventHandler(this.FormFluidTemp_Load);
            this.groupBoxDieTemp.ResumeLayout(false);
            this.panelDieTempController.ResumeLayout(false);
            this.panelDieTempController.PerformLayout();
            this.groupBoxReservoirTemp.ResumeLayout(false);
            this.panelResvTempController.ResumeLayout(false);
            this.panelResvTempController.PerformLayout();
            this.groupBoxReservoirBTemp.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxDieTemp;
        private System.Windows.Forms.Panel panelDieTempController;
        private System.Windows.Forms.Label labelDieSetPointChanges;
        private System.Windows.Forms.Button buttonDieSetPointApply;
        private System.Windows.Forms.Button buttonDieTempSetPoint;
        private System.Windows.Forms.Label labelDieTempSetPoint;
        private System.Windows.Forms.Label labelDieTempController;
        private System.Windows.Forms.Label labelDieTempOutput;
        private System.Windows.Forms.GroupBox groupBoxReservoirTemp;
        private System.Windows.Forms.Button buttonResvAdvance;
        private System.Windows.Forms.Panel panelResvTempController;
        private System.Windows.Forms.Label labelResvSetPointChanges;
        private System.Windows.Forms.Button buttonResvSetPointApply;
        private System.Windows.Forms.Button buttonResvSetPoint;
        private System.Windows.Forms.Label labelResvSetupPoint;
        private System.Windows.Forms.Label labelResvTempController;
        private System.Windows.Forms.Label labelResvTempOutput;
        private System.Windows.Forms.GroupBox groupBoxReservoirBTemp;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button buttonResvBAdvance;
        private System.Windows.Forms.Label labelResvBSetPointChanges;
        private System.Windows.Forms.Button buttonResvBSetPointApply;
        private System.Windows.Forms.Button buttonResvBSetPoint;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label labelResvBTempOutput;
    }
}