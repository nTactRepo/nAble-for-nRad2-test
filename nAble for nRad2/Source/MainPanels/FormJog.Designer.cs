namespace nAble
{
    partial class FormJog
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
            this.buttonJogXAxis = new System.Windows.Forms.Button();
            this.buttonJogZAxis = new System.Windows.Forms.Button();
            this.labelJogInstructions = new System.Windows.Forms.Label();
            this.buttonGotoMaint = new System.Windows.Forms.Button();
            this.buttonGotoHome = new System.Windows.Forms.Button();
            this.buttonFullZUp = new System.Windows.Forms.Button();
            this.buttonDieLoadUnloadPos = new System.Windows.Forms.Button();
            this.buttonJogTransfer = new System.Windows.Forms.Button();
            this.buttonGoToMeasureLoc = new System.Windows.Forms.Button();
            this.buttonGoToVisionLoc = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonJogXAxis
            // 
            this.buttonJogXAxis.Font = new System.Drawing.Font("Tahoma", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonJogXAxis.Location = new System.Drawing.Point(726, 22);
            this.buttonJogXAxis.Name = "buttonJogXAxis";
            this.buttonJogXAxis.Size = new System.Drawing.Size(276, 227);
            this.buttonJogXAxis.TabIndex = 0;
            this.buttonJogXAxis.Text = "X Axis";
            this.buttonJogXAxis.UseVisualStyleBackColor = true;
            this.buttonJogXAxis.Click += new System.EventHandler(this.buttonJogXAxis_Click);
            // 
            // buttonJogZAxis
            // 
            this.buttonJogZAxis.Font = new System.Drawing.Font("Tahoma", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonJogZAxis.Location = new System.Drawing.Point(726, 267);
            this.buttonJogZAxis.Name = "buttonJogZAxis";
            this.buttonJogZAxis.Size = new System.Drawing.Size(276, 227);
            this.buttonJogZAxis.TabIndex = 1;
            this.buttonJogZAxis.Text = "Z Axis";
            this.buttonJogZAxis.UseVisualStyleBackColor = true;
            this.buttonJogZAxis.Click += new System.EventHandler(this.buttonJogZAxis_Click);
            // 
            // labelJogInstructions
            // 
            this.labelJogInstructions.AutoSize = true;
            this.labelJogInstructions.Location = new System.Drawing.Point(12, 9);
            this.labelJogInstructions.Name = "labelJogInstructions";
            this.labelJogInstructions.Size = new System.Drawing.Size(343, 66);
            this.labelJogInstructions.TabIndex = 2;
            this.labelJogInstructions.Text = "Please select an Axis to jog,\r\nor an operation below.";
            // 
            // buttonGotoMaint
            // 
            this.buttonGotoMaint.Font = new System.Drawing.Font("Tahoma", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonGotoMaint.Location = new System.Drawing.Point(18, 309);
            this.buttonGotoMaint.Name = "buttonGotoMaint";
            this.buttonGotoMaint.Size = new System.Drawing.Size(303, 83);
            this.buttonGotoMaint.TabIndex = 3;
            this.buttonGotoMaint.Text = "Goto Maintenance Position";
            this.buttonGotoMaint.UseVisualStyleBackColor = true;
            this.buttonGotoMaint.Click += new System.EventHandler(this.buttonGotoMaint_Click);
            // 
            // buttonGotoHome
            // 
            this.buttonGotoHome.Font = new System.Drawing.Font("Tahoma", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonGotoHome.Location = new System.Drawing.Point(18, 409);
            this.buttonGotoHome.Name = "buttonGotoHome";
            this.buttonGotoHome.Size = new System.Drawing.Size(303, 83);
            this.buttonGotoHome.TabIndex = 4;
            this.buttonGotoHome.Text = "Goto Home Position";
            this.buttonGotoHome.UseVisualStyleBackColor = true;
            this.buttonGotoHome.Click += new System.EventHandler(this.buttonGotoHome_Click);
            // 
            // buttonFullZUp
            // 
            this.buttonFullZUp.Font = new System.Drawing.Font("Tahoma", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonFullZUp.Location = new System.Drawing.Point(18, 109);
            this.buttonFullZUp.Name = "buttonFullZUp";
            this.buttonFullZUp.Size = new System.Drawing.Size(303, 83);
            this.buttonFullZUp.TabIndex = 5;
            this.buttonFullZUp.Text = "Full Up";
            this.buttonFullZUp.UseVisualStyleBackColor = true;
            this.buttonFullZUp.Click += new System.EventHandler(this.buttonFullZUp_Click);
            // 
            // buttonDieLoadUnloadPos
            // 
            this.buttonDieLoadUnloadPos.Font = new System.Drawing.Font("Tahoma", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonDieLoadUnloadPos.Location = new System.Drawing.Point(18, 209);
            this.buttonDieLoadUnloadPos.Name = "buttonDieLoadUnloadPos";
            this.buttonDieLoadUnloadPos.Size = new System.Drawing.Size(303, 83);
            this.buttonDieLoadUnloadPos.TabIndex = 6;
            this.buttonDieLoadUnloadPos.Text = "Goto Die Load/Unload Position";
            this.buttonDieLoadUnloadPos.UseVisualStyleBackColor = true;
            this.buttonDieLoadUnloadPos.Click += new System.EventHandler(this.buttonDieLoadUnloadPos_Click);
            // 
            // buttonJogTransfer
            // 
            this.buttonJogTransfer.Font = new System.Drawing.Font("Tahoma", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonJogTransfer.Location = new System.Drawing.Point(726, 340);
            this.buttonJogTransfer.Name = "buttonJogTransfer";
            this.buttonJogTransfer.Size = new System.Drawing.Size(276, 152);
            this.buttonJogTransfer.TabIndex = 7;
            this.buttonJogTransfer.Text = "Tranfer";
            this.buttonJogTransfer.UseVisualStyleBackColor = true;
            this.buttonJogTransfer.Visible = false;
            this.buttonJogTransfer.Click += new System.EventHandler(this.buttonJogTransfer_Click);
            // 
            // buttonGoToMeasureLoc
            // 
            this.buttonGoToMeasureLoc.Font = new System.Drawing.Font("Tahoma", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonGoToMeasureLoc.Location = new System.Drawing.Point(346, 209);
            this.buttonGoToMeasureLoc.Name = "buttonGoToMeasureLoc";
            this.buttonGoToMeasureLoc.Size = new System.Drawing.Size(303, 83);
            this.buttonGoToMeasureLoc.TabIndex = 9;
            this.buttonGoToMeasureLoc.Text = "Measure Position";
            this.buttonGoToMeasureLoc.UseVisualStyleBackColor = true;
            this.buttonGoToMeasureLoc.Click += new System.EventHandler(this.buttonGoToMeasureLoc_Click);
            // 
            // buttonGoToVisionLoc
            // 
            this.buttonGoToVisionLoc.Font = new System.Drawing.Font("Tahoma", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonGoToVisionLoc.Location = new System.Drawing.Point(346, 109);
            this.buttonGoToVisionLoc.Name = "buttonGoToVisionLoc";
            this.buttonGoToVisionLoc.Size = new System.Drawing.Size(303, 83);
            this.buttonGoToVisionLoc.TabIndex = 8;
            this.buttonGoToVisionLoc.Text = "Vision Position";
            this.buttonGoToVisionLoc.UseVisualStyleBackColor = true;
            this.buttonGoToVisionLoc.Click += new System.EventHandler(this.buttonGoToVisionLoc_Click);
            // 
            // FormJog
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.SteelBlue;
            this.ClientSize = new System.Drawing.Size(1024, 514);
            this.Controls.Add(this.buttonGoToMeasureLoc);
            this.Controls.Add(this.buttonGoToVisionLoc);
            this.Controls.Add(this.buttonJogTransfer);
            this.Controls.Add(this.buttonDieLoadUnloadPos);
            this.Controls.Add(this.buttonFullZUp);
            this.Controls.Add(this.buttonGotoHome);
            this.Controls.Add(this.buttonGotoMaint);
            this.Controls.Add(this.labelJogInstructions);
            this.Controls.Add(this.buttonJogZAxis);
            this.Controls.Add(this.buttonJogXAxis);
            this.Font = new System.Drawing.Font("Tahoma", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(5);
            this.Name = "FormJog";
            this.Text = "Manual Jogging";
            this.Load += new System.EventHandler(this.FormJog_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonJogXAxis;
        private System.Windows.Forms.Button buttonJogZAxis;
        private System.Windows.Forms.Label labelJogInstructions;
        private System.Windows.Forms.Button buttonGotoMaint;
        private System.Windows.Forms.Button buttonGotoHome;
        private System.Windows.Forms.Button buttonFullZUp;
		private System.Windows.Forms.Button buttonDieLoadUnloadPos;
		private System.Windows.Forms.Button buttonJogTransfer;
        private System.Windows.Forms.Button buttonGoToMeasureLoc;
        private System.Windows.Forms.Button buttonGoToVisionLoc;
    }
}