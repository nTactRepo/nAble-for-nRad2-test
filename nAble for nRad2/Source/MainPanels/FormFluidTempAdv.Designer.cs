namespace nAble
{
    partial class FormFluidTempAdv
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
            System.Windows.Forms.Label labelResvHeaterSetPointUnits;
            this.groupBoxResvHeaterTemp = new System.Windows.Forms.GroupBox();
            this.panelResvHeaterTemp = new System.Windows.Forms.Panel();
            this.labelResvHeaterSetPointChanges = new System.Windows.Forms.Label();
            this.buttonResvHeaterSetPointApply = new System.Windows.Forms.Button();
            this.buttonResvHeaterSetPoint = new System.Windows.Forms.Button();
            this.labelResvHeaterSetPoint = new System.Windows.Forms.Label();
            this.labelResvHeaterTempController = new System.Windows.Forms.Label();
            this.labelResvHeaterTempOutput = new System.Windows.Forms.Label();
            this.buttonResvTempAdvDone = new System.Windows.Forms.Button();
            labelResvHeaterSetPointUnits = new System.Windows.Forms.Label();
            this.groupBoxResvHeaterTemp.SuspendLayout();
            this.panelResvHeaterTemp.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelResvHeaterSetPointUnits
            // 
            labelResvHeaterSetPointUnits.AutoSize = true;
            labelResvHeaterSetPointUnits.Location = new System.Drawing.Point(493, 55);
            labelResvHeaterSetPointUnits.Name = "labelResvHeaterSetPointUnits";
            labelResvHeaterSetPointUnits.Size = new System.Drawing.Size(44, 33);
            labelResvHeaterSetPointUnits.TabIndex = 103;
            labelResvHeaterSetPointUnits.Text = "°C";
            // 
            // groupBoxResvHeaterTemp
            // 
            this.groupBoxResvHeaterTemp.Controls.Add(this.panelResvHeaterTemp);
            this.groupBoxResvHeaterTemp.Location = new System.Drawing.Point(12, 12);
            this.groupBoxResvHeaterTemp.Name = "groupBoxResvHeaterTemp";
            this.groupBoxResvHeaterTemp.Size = new System.Drawing.Size(776, 203);
            this.groupBoxResvHeaterTemp.TabIndex = 9;
            this.groupBoxResvHeaterTemp.TabStop = false;
            this.groupBoxResvHeaterTemp.Text = "Reservoir Heater Temperature";
            // 
            // panelResvHeaterTemp
            // 
            this.panelResvHeaterTemp.BackColor = System.Drawing.SystemColors.ControlDark;
            this.panelResvHeaterTemp.Controls.Add(this.labelResvHeaterSetPointChanges);
            this.panelResvHeaterTemp.Controls.Add(this.buttonResvHeaterSetPointApply);
            this.panelResvHeaterTemp.Controls.Add(this.buttonResvHeaterSetPoint);
            this.panelResvHeaterTemp.Controls.Add(labelResvHeaterSetPointUnits);
            this.panelResvHeaterTemp.Controls.Add(this.labelResvHeaterSetPoint);
            this.panelResvHeaterTemp.Controls.Add(this.labelResvHeaterTempController);
            this.panelResvHeaterTemp.Controls.Add(this.labelResvHeaterTempOutput);
            this.panelResvHeaterTemp.Location = new System.Drawing.Point(13, 39);
            this.panelResvHeaterTemp.Name = "panelResvHeaterTemp";
            this.panelResvHeaterTemp.Size = new System.Drawing.Size(746, 143);
            this.panelResvHeaterTemp.TabIndex = 9;
            // 
            // labelResvHeaterSetPointChanges
            // 
            this.labelResvHeaterSetPointChanges.AutoSize = true;
            this.labelResvHeaterSetPointChanges.ForeColor = System.Drawing.Color.Yellow;
            this.labelResvHeaterSetPointChanges.Location = new System.Drawing.Point(218, 106);
            this.labelResvHeaterSetPointChanges.Name = "labelResvHeaterSetPointChanges";
            this.labelResvHeaterSetPointChanges.Size = new System.Drawing.Size(394, 33);
            this.labelResvHeaterSetPointChanges.TabIndex = 109;
            this.labelResvHeaterSetPointChanges.Text = "Changes have not been applied.";
            this.labelResvHeaterSetPointChanges.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.labelResvHeaterSetPointChanges.Visible = false;
            // 
            // buttonResvHeaterSetPointApply
            // 
            this.buttonResvHeaterSetPointApply.Location = new System.Drawing.Point(618, 38);
            this.buttonResvHeaterSetPointApply.Name = "buttonResvHeaterSetPointApply";
            this.buttonResvHeaterSetPointApply.Size = new System.Drawing.Size(114, 66);
            this.buttonResvHeaterSetPointApply.TabIndex = 108;
            this.buttonResvHeaterSetPointApply.Text = "Apply";
            this.buttonResvHeaterSetPointApply.UseVisualStyleBackColor = true;
            this.buttonResvHeaterSetPointApply.Click += new System.EventHandler(this.buttonResvHeaterSetPointApply_Click);
            // 
            // buttonResvHeaterSetPoint
            // 
            this.buttonResvHeaterSetPoint.Location = new System.Drawing.Point(401, 51);
            this.buttonResvHeaterSetPoint.Name = "buttonResvHeaterSetPoint";
            this.buttonResvHeaterSetPoint.Size = new System.Drawing.Size(91, 40);
            this.buttonResvHeaterSetPoint.TabIndex = 104;
            this.buttonResvHeaterSetPoint.Text = "50.0";
            this.buttonResvHeaterSetPoint.UseVisualStyleBackColor = true;
            this.buttonResvHeaterSetPoint.Click += new System.EventHandler(this.buttonResvHeaterSetPoint_Click);
            // 
            // labelResvHeaterSetPoint
            // 
            this.labelResvHeaterSetPoint.Location = new System.Drawing.Point(240, 55);
            this.labelResvHeaterSetPoint.Name = "labelResvHeaterSetPoint";
            this.labelResvHeaterSetPoint.Size = new System.Drawing.Size(155, 33);
            this.labelResvHeaterSetPoint.TabIndex = 2;
            this.labelResvHeaterSetPoint.Text = "Set Point:";
            this.labelResvHeaterSetPoint.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelResvHeaterTempController
            // 
            this.labelResvHeaterTempController.BackColor = System.Drawing.Color.Black;
            this.labelResvHeaterTempController.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelResvHeaterTempController.ForeColor = System.Drawing.Color.Lime;
            this.labelResvHeaterTempController.Location = new System.Drawing.Point(24, 27);
            this.labelResvHeaterTempController.Name = "labelResvHeaterTempController";
            this.labelResvHeaterTempController.Size = new System.Drawing.Size(173, 29);
            this.labelResvHeaterTempController.TabIndex = 1;
            this.labelResvHeaterTempController.Text = "Heater Temp";
            this.labelResvHeaterTempController.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelResvHeaterTempOutput
            // 
            this.labelResvHeaterTempOutput.BackColor = System.Drawing.Color.Black;
            this.labelResvHeaterTempOutput.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelResvHeaterTempOutput.ForeColor = System.Drawing.Color.Lime;
            this.labelResvHeaterTempOutput.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.labelResvHeaterTempOutput.Location = new System.Drawing.Point(24, 56);
            this.labelResvHeaterTempOutput.Name = "labelResvHeaterTempOutput";
            this.labelResvHeaterTempOutput.Size = new System.Drawing.Size(173, 58);
            this.labelResvHeaterTempOutput.TabIndex = 0;
            this.labelResvHeaterTempOutput.Text = "21° C";
            this.labelResvHeaterTempOutput.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // buttonResvTempAdvDone
            // 
            this.buttonResvTempAdvDone.Location = new System.Drawing.Point(661, 432);
            this.buttonResvTempAdvDone.Name = "buttonResvTempAdvDone";
            this.buttonResvTempAdvDone.Size = new System.Drawing.Size(110, 66);
            this.buttonResvTempAdvDone.TabIndex = 10;
            this.buttonResvTempAdvDone.Text = "Done";
            this.buttonResvTempAdvDone.UseVisualStyleBackColor = true;
            this.buttonResvTempAdvDone.Click += new System.EventHandler(this.buttonResvTempAdvDone_Click);
            // 
            // FormFluidTempAdv
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1024, 514);
            this.Controls.Add(this.buttonResvTempAdvDone);
            this.Controls.Add(this.groupBoxResvHeaterTemp);
            this.Font = new System.Drawing.Font("Tahoma", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(5);
            this.Name = "FormFluidTempAdv";
            this.Text = "Advanced";
            this.Load += new System.EventHandler(this.FormFluidTempAdv_Load);
            this.groupBoxResvHeaterTemp.ResumeLayout(false);
            this.panelResvHeaterTemp.ResumeLayout(false);
            this.panelResvHeaterTemp.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxResvHeaterTemp;
        private System.Windows.Forms.Panel panelResvHeaterTemp;
        private System.Windows.Forms.Label labelResvHeaterSetPointChanges;
        private System.Windows.Forms.Button buttonResvHeaterSetPointApply;
        private System.Windows.Forms.Button buttonResvHeaterSetPoint;
        private System.Windows.Forms.Label labelResvHeaterSetPoint;
        private System.Windows.Forms.Label labelResvHeaterTempController;
        private System.Windows.Forms.Label labelResvHeaterTempOutput;
        private System.Windows.Forms.Button buttonResvTempAdvDone;

    }
}