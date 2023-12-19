namespace nAble
{
    partial class FormFluidMain
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
            this.buttonFluidTemp = new System.Windows.Forms.Button();
            this.buttonFluidFlow = new System.Windows.Forms.Button();
            this.labelFluidInstructions = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // buttonFluidTemp
            // 
            this.buttonFluidTemp.Location = new System.Drawing.Point(527, 146);
            this.buttonFluidTemp.Name = "buttonFluidTemp";
            this.buttonFluidTemp.Size = new System.Drawing.Size(276, 232);
            this.buttonFluidTemp.TabIndex = 10;
            this.buttonFluidTemp.Text = "Temperature\r\nControl";
            this.buttonFluidTemp.UseVisualStyleBackColor = true;
            this.buttonFluidTemp.Click += new System.EventHandler(this.buttonFluidTemp_Click);
            // 
            // buttonFluidFlow
            // 
            this.buttonFluidFlow.Location = new System.Drawing.Point(223, 146);
            this.buttonFluidFlow.Name = "buttonFluidFlow";
            this.buttonFluidFlow.Size = new System.Drawing.Size(276, 232);
            this.buttonFluidFlow.TabIndex = 9;
            this.buttonFluidFlow.Text = "Fluid\r\n Control";
            this.buttonFluidFlow.UseVisualStyleBackColor = true;
            this.buttonFluidFlow.Click += new System.EventHandler(this.buttonFluidFlow_Click);
            // 
            // labelFluidInstructions
            // 
            this.labelFluidInstructions.AutoSize = true;
            this.labelFluidInstructions.ForeColor = System.Drawing.Color.White;
            this.labelFluidInstructions.Location = new System.Drawing.Point(346, 91);
            this.labelFluidInstructions.Name = "labelFluidInstructions";
            this.labelFluidInstructions.Size = new System.Drawing.Size(333, 33);
            this.labelFluidInstructions.TabIndex = 11;
            this.labelFluidInstructions.Text = "Please select an operation:";
            // 
            // FormFluidMain
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.SteelBlue;
            this.ClientSize = new System.Drawing.Size(1024, 514);
            this.Controls.Add(this.labelFluidInstructions);
            this.Controls.Add(this.buttonFluidTemp);
            this.Controls.Add(this.buttonFluidFlow);
            this.Font = new System.Drawing.Font("Tahoma", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(5);
            this.Name = "FormFluidMain";
            this.Text = "Fluid";
            this.Load += new System.EventHandler(this.FormFluidMain_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonFluidTemp;
        private System.Windows.Forms.Button buttonFluidFlow;
        private System.Windows.Forms.Label labelFluidInstructions;
    }
}