namespace nAble
{
    partial class FormVacuumTrend
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
			this.buttonShowRealTimeChart = new System.Windows.Forms.Button();
			this.groupBoxSlot1 = new System.Windows.Forms.GroupBox();
			this.labelValveAngle = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.labelCurrentPressure = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBoxProcessStep = new System.Windows.Forms.GroupBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.label2 = new System.Windows.Forms.Label();
			this.labelTotalTimeRemaining = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.labelStepTimeRemaining = new System.Windows.Forms.Label();
			this.labelProcessStep = new System.Windows.Forms.Label();
			this.buttonStartRecipeTest = new System.Windows.Forms.Button();
			this.buttonTrendCalendar = new System.Windows.Forms.Button();
			this.groupBoxSlot1.SuspendLayout();
			this.groupBoxProcessStep.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// buttonShowRealTimeChart
			// 
			this.buttonShowRealTimeChart.BackColor = System.Drawing.SystemColors.Control;
			this.buttonShowRealTimeChart.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.buttonShowRealTimeChart.Location = new System.Drawing.Point(763, 412);
			this.buttonShowRealTimeChart.Name = "buttonShowRealTimeChart";
			this.buttonShowRealTimeChart.Size = new System.Drawing.Size(117, 90);
			this.buttonShowRealTimeChart.TabIndex = 37;
			this.buttonShowRealTimeChart.Text = "View Real Time";
			this.buttonShowRealTimeChart.UseVisualStyleBackColor = false;
			this.buttonShowRealTimeChart.Click += new System.EventHandler(this.buttonShowRealTimeChart_Click);
			// 
			// groupBoxSlot1
			// 
			this.groupBoxSlot1.Controls.Add(this.labelValveAngle);
			this.groupBoxSlot1.Controls.Add(this.label3);
			this.groupBoxSlot1.Controls.Add(this.labelCurrentPressure);
			this.groupBoxSlot1.Controls.Add(this.label1);
			this.groupBoxSlot1.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.groupBoxSlot1.ForeColor = System.Drawing.Color.White;
			this.groupBoxSlot1.Location = new System.Drawing.Point(763, 12);
			this.groupBoxSlot1.Name = "groupBoxSlot1";
			this.groupBoxSlot1.Size = new System.Drawing.Size(251, 121);
			this.groupBoxSlot1.TabIndex = 44;
			this.groupBoxSlot1.TabStop = false;
			this.groupBoxSlot1.Text = "Process Values";
			// 
			// labelValveAngle
			// 
			this.labelValveAngle.BackColor = System.Drawing.Color.White;
			this.labelValveAngle.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.labelValveAngle.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelValveAngle.ForeColor = System.Drawing.Color.Black;
			this.labelValveAngle.Location = new System.Drawing.Point(168, 75);
			this.labelValveAngle.Name = "labelValveAngle";
			this.labelValveAngle.Size = new System.Drawing.Size(75, 30);
			this.labelValveAngle.TabIndex = 45;
			this.labelValveAngle.Text = "90°";
			this.labelValveAngle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label3.Location = new System.Drawing.Point(36, 78);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(129, 25);
			this.label3.TabIndex = 44;
			this.label3.Text = "Valve Angle:";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelCurrentPressure
			// 
			this.labelCurrentPressure.BackColor = System.Drawing.Color.White;
			this.labelCurrentPressure.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.labelCurrentPressure.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelCurrentPressure.ForeColor = System.Drawing.Color.Black;
			this.labelCurrentPressure.Location = new System.Drawing.Point(168, 37);
			this.labelCurrentPressure.Name = "labelCurrentPressure";
			this.labelCurrentPressure.Size = new System.Drawing.Size(75, 30);
			this.labelCurrentPressure.TabIndex = 11;
			this.labelCurrentPressure.Text = "764";
			this.labelCurrentPressure.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(4, 40);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(161, 25);
			this.label1.TabIndex = 10;
			this.label1.Text = "Pressure (Torr):";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupBoxProcessStep
			// 
			this.groupBoxProcessStep.Controls.Add(this.groupBox1);
			this.groupBoxProcessStep.Controls.Add(this.labelProcessStep);
			this.groupBoxProcessStep.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.groupBoxProcessStep.ForeColor = System.Drawing.Color.White;
			this.groupBoxProcessStep.Location = new System.Drawing.Point(763, 139);
			this.groupBoxProcessStep.Name = "groupBoxProcessStep";
			this.groupBoxProcessStep.Size = new System.Drawing.Size(251, 189);
			this.groupBoxProcessStep.TabIndex = 45;
			this.groupBoxProcessStep.TabStop = false;
			this.groupBoxProcessStep.Text = "Current Process Step";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.labelTotalTimeRemaining);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.labelStepTimeRemaining);
			this.groupBox1.ForeColor = System.Drawing.Color.White;
			this.groupBox1.Location = new System.Drawing.Point(12, 66);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(230, 113);
			this.groupBox1.TabIndex = 55;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Time Remaining";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label2.Location = new System.Drawing.Point(49, 35);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(65, 25);
			this.label2.TabIndex = 40;
			this.label2.Text = "Total:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelTotalTimeRemaining
			// 
			this.labelTotalTimeRemaining.BackColor = System.Drawing.Color.White;
			this.labelTotalTimeRemaining.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.labelTotalTimeRemaining.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelTotalTimeRemaining.ForeColor = System.Drawing.Color.Black;
			this.labelTotalTimeRemaining.Location = new System.Drawing.Point(116, 34);
			this.labelTotalTimeRemaining.Name = "labelTotalTimeRemaining";
			this.labelTotalTimeRemaining.Size = new System.Drawing.Size(100, 27);
			this.labelTotalTimeRemaining.TabIndex = 41;
			this.labelTotalTimeRemaining.Text = "00:00:00";
			this.labelTotalTimeRemaining.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label4.Location = new System.Drawing.Point(53, 71);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(61, 25);
			this.label4.TabIndex = 42;
			this.label4.Text = "Step:";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelStepTimeRemaining
			// 
			this.labelStepTimeRemaining.BackColor = System.Drawing.Color.White;
			this.labelStepTimeRemaining.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.labelStepTimeRemaining.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelStepTimeRemaining.ForeColor = System.Drawing.Color.Black;
			this.labelStepTimeRemaining.Location = new System.Drawing.Point(116, 70);
			this.labelStepTimeRemaining.Name = "labelStepTimeRemaining";
			this.labelStepTimeRemaining.Size = new System.Drawing.Size(100, 27);
			this.labelStepTimeRemaining.TabIndex = 43;
			this.labelStepTimeRemaining.Text = "00:00:00";
			this.labelStepTimeRemaining.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// labelProcessStep
			// 
			this.labelProcessStep.BackColor = System.Drawing.Color.White;
			this.labelProcessStep.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.labelProcessStep.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelProcessStep.ForeColor = System.Drawing.Color.Black;
			this.labelProcessStep.Location = new System.Drawing.Point(12, 29);
			this.labelProcessStep.Name = "labelProcessStep";
			this.labelProcessStep.Size = new System.Drawing.Size(230, 30);
			this.labelProcessStep.TabIndex = 11;
			this.labelProcessStep.Text = "Not Active";
			this.labelProcessStep.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// buttonStartRecipeTest
			// 
			this.buttonStartRecipeTest.Font = new System.Drawing.Font("Tahoma", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.buttonStartRecipeTest.Location = new System.Drawing.Point(763, 343);
			this.buttonStartRecipeTest.Name = "buttonStartRecipeTest";
			this.buttonStartRecipeTest.Size = new System.Drawing.Size(251, 57);
			this.buttonStartRecipeTest.TabIndex = 55;
			this.buttonStartRecipeTest.Text = "Start Recipe Test";
			this.buttonStartRecipeTest.UseVisualStyleBackColor = true;
			// 
			// buttonTrendCalendar
			// 
			this.buttonTrendCalendar.BackColor = System.Drawing.SystemColors.Control;
			this.buttonTrendCalendar.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.buttonTrendCalendar.Location = new System.Drawing.Point(897, 412);
			this.buttonTrendCalendar.Name = "buttonTrendCalendar";
			this.buttonTrendCalendar.Size = new System.Drawing.Size(117, 90);
			this.buttonTrendCalendar.TabIndex = 56;
			this.buttonTrendCalendar.Text = "Select History";
			this.buttonTrendCalendar.UseVisualStyleBackColor = false;
			this.buttonTrendCalendar.Click += new System.EventHandler(this.buttonTrendCalendar_Click);
			// 
			// FormVacuumTrend
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.BackColor = System.Drawing.Color.SteelBlue;
			this.ClientSize = new System.Drawing.Size(1024, 514);
			this.Controls.Add(this.buttonTrendCalendar);
			this.Controls.Add(this.buttonStartRecipeTest);
			this.Controls.Add(this.groupBoxProcessStep);
			this.Controls.Add(this.groupBoxSlot1);
			this.Controls.Add(this.buttonShowRealTimeChart);
			this.Font = new System.Drawing.Font("Tahoma", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Margin = new System.Windows.Forms.Padding(5);
			this.Name = "FormVacuumTrend";
			this.Text = "Vacuum Trend";
			this.groupBoxSlot1.ResumeLayout(false);
			this.groupBoxSlot1.PerformLayout();
			this.groupBoxProcessStep.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);

        }

		#endregion

		private System.Windows.Forms.Button buttonShowRealTimeChart;
		private System.Windows.Forms.GroupBox groupBoxSlot1;
		private System.Windows.Forms.Label labelValveAngle;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label labelCurrentPressure;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.GroupBox groupBoxProcessStep;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label labelTotalTimeRemaining;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label labelStepTimeRemaining;
		private System.Windows.Forms.Label labelProcessStep;
		private System.Windows.Forms.Button buttonStartRecipeTest;
		private System.Windows.Forms.Button buttonTrendCalendar;
	}
}