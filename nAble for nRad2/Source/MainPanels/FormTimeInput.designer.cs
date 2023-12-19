namespace nAble
{
    partial class FormTimeInput
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
			this.labelTimeTitle = new System.Windows.Forms.Label();
			this.textBoxTimeInput = new System.Windows.Forms.TextBox();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonEnter = new System.Windows.Forms.Button();
			this.buttonClear = new System.Windows.Forms.Button();
			this.button0 = new System.Windows.Forms.Button();
			this.button9 = new System.Windows.Forms.Button();
			this.button8 = new System.Windows.Forms.Button();
			this.button7 = new System.Windows.Forms.Button();
			this.button6 = new System.Windows.Forms.Button();
			this.button5 = new System.Windows.Forms.Button();
			this.button4 = new System.Windows.Forms.Button();
			this.button3 = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			this.button1 = new System.Windows.Forms.Button();
			this.labelErrNumInput = new System.Windows.Forms.Label();
			this.labelPrevious = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// labelTimeTitle
			// 
			this.labelTimeTitle.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.labelTimeTitle.ForeColor = System.Drawing.SystemColors.WindowText;
			this.labelTimeTitle.Location = new System.Drawing.Point(31, 19);
			this.labelTimeTitle.Name = "labelTimeTitle";
			this.labelTimeTitle.Size = new System.Drawing.Size(426, 86);
			this.labelTimeTitle.TabIndex = 34;
			this.labelTimeTitle.Text = "Title";
			this.labelTimeTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textBoxTimeInput
			// 
			this.textBoxTimeInput.BackColor = System.Drawing.Color.Black;
			this.textBoxTimeInput.Font = new System.Drawing.Font("Tahoma", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.textBoxTimeInput.ForeColor = System.Drawing.Color.Red;
			this.textBoxTimeInput.Location = new System.Drawing.Point(106, 173);
			this.textBoxTimeInput.MaxLength = 16;
			this.textBoxTimeInput.Name = "textBoxTimeInput";
			this.textBoxTimeInput.ReadOnly = true;
			this.textBoxTimeInput.Size = new System.Drawing.Size(277, 43);
			this.textBoxTimeInput.TabIndex = 33;
			this.textBoxTimeInput.Text = "00:00:00";
			this.textBoxTimeInput.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.textBoxTimeInput.TextChanged += new System.EventHandler(this.textBoxTimeInput_TextChanged);
			// 
			// buttonCancel
			// 
			this.buttonCancel.Location = new System.Drawing.Point(192, 414);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(149, 73);
			this.buttonCancel.TabIndex = 32;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// buttonEnter
			// 
			this.buttonEnter.Enabled = false;
			this.buttonEnter.Location = new System.Drawing.Point(705, 397);
			this.buttonEnter.Name = "buttonEnter";
			this.buttonEnter.Size = new System.Drawing.Size(174, 106);
			this.buttonEnter.TabIndex = 31;
			this.buttonEnter.Text = "Enter";
			this.buttonEnter.UseVisualStyleBackColor = true;
			this.buttonEnter.Click += new System.EventHandler(this.buttonEnter_Click);
			// 
			// buttonClear
			// 
			this.buttonClear.Location = new System.Drawing.Point(495, 397);
			this.buttonClear.Name = "buttonClear";
			this.buttonClear.Size = new System.Drawing.Size(169, 106);
			this.buttonClear.TabIndex = 30;
			this.buttonClear.Text = "Clear";
			this.buttonClear.UseVisualStyleBackColor = true;
			this.buttonClear.Click += new System.EventHandler(this.buttonClear_Click);
			// 
			// button0
			// 
			this.button0.Location = new System.Drawing.Point(628, 301);
			this.button0.Name = "button0";
			this.button0.Size = new System.Drawing.Size(119, 90);
			this.button0.TabIndex = 27;
			this.button0.Tag = "0";
			this.button0.Text = "0";
			this.button0.UseVisualStyleBackColor = true;
			this.button0.Click += new System.EventHandler(this.buttonNum_Click);
			// 
			// button9
			// 
			this.button9.Location = new System.Drawing.Point(760, 205);
			this.button9.Name = "button9";
			this.button9.Size = new System.Drawing.Size(119, 90);
			this.button9.TabIndex = 26;
			this.button9.Tag = "9";
			this.button9.Text = "9";
			this.button9.UseVisualStyleBackColor = true;
			this.button9.Click += new System.EventHandler(this.buttonNum_Click);
			// 
			// button8
			// 
			this.button8.Location = new System.Drawing.Point(628, 205);
			this.button8.Name = "button8";
			this.button8.Size = new System.Drawing.Size(119, 90);
			this.button8.TabIndex = 25;
			this.button8.Tag = "8";
			this.button8.Text = "8";
			this.button8.UseVisualStyleBackColor = true;
			this.button8.Click += new System.EventHandler(this.buttonNum_Click);
			// 
			// button7
			// 
			this.button7.Location = new System.Drawing.Point(495, 205);
			this.button7.Name = "button7";
			this.button7.Size = new System.Drawing.Size(119, 90);
			this.button7.TabIndex = 24;
			this.button7.Tag = "7";
			this.button7.Text = "7";
			this.button7.UseVisualStyleBackColor = true;
			this.button7.Click += new System.EventHandler(this.buttonNum_Click);
			// 
			// button6
			// 
			this.button6.Location = new System.Drawing.Point(760, 109);
			this.button6.Name = "button6";
			this.button6.Size = new System.Drawing.Size(119, 90);
			this.button6.TabIndex = 23;
			this.button6.Tag = "6";
			this.button6.Text = "6";
			this.button6.UseVisualStyleBackColor = true;
			this.button6.Click += new System.EventHandler(this.buttonNum_Click);
			// 
			// button5
			// 
			this.button5.Location = new System.Drawing.Point(628, 109);
			this.button5.Name = "button5";
			this.button5.Size = new System.Drawing.Size(119, 90);
			this.button5.TabIndex = 22;
			this.button5.Tag = "5";
			this.button5.Text = "5";
			this.button5.UseVisualStyleBackColor = true;
			this.button5.Click += new System.EventHandler(this.buttonNum_Click);
			// 
			// button4
			// 
			this.button4.Location = new System.Drawing.Point(495, 109);
			this.button4.Name = "button4";
			this.button4.Size = new System.Drawing.Size(119, 90);
			this.button4.TabIndex = 21;
			this.button4.Tag = "4";
			this.button4.Text = "4";
			this.button4.UseVisualStyleBackColor = true;
			this.button4.Click += new System.EventHandler(this.buttonNum_Click);
			// 
			// button3
			// 
			this.button3.Location = new System.Drawing.Point(760, 13);
			this.button3.Name = "button3";
			this.button3.Size = new System.Drawing.Size(119, 90);
			this.button3.TabIndex = 20;
			this.button3.Tag = "3";
			this.button3.Text = "3";
			this.button3.UseVisualStyleBackColor = true;
			this.button3.Click += new System.EventHandler(this.buttonNum_Click);
			// 
			// button2
			// 
			this.button2.Location = new System.Drawing.Point(628, 13);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(119, 90);
			this.button2.TabIndex = 19;
			this.button2.Tag = "2";
			this.button2.Text = "2";
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new System.EventHandler(this.buttonNum_Click);
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(495, 13);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(119, 90);
			this.button1.TabIndex = 18;
			this.button1.Tag = "1";
			this.button1.Text = "1";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.buttonNum_Click);
			// 
			// labelErrNumInput
			// 
			this.labelErrNumInput.ForeColor = System.Drawing.Color.Yellow;
			this.labelErrNumInput.Location = new System.Drawing.Point(79, 234);
			this.labelErrNumInput.Name = "labelErrNumInput";
			this.labelErrNumInput.Size = new System.Drawing.Size(330, 139);
			this.labelErrNumInput.TabIndex = 35;
			this.labelErrNumInput.Text = "Error Message";
			this.labelErrNumInput.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// labelPrevious
			// 
			this.labelPrevious.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelPrevious.ForeColor = System.Drawing.Color.White;
			this.labelPrevious.Location = new System.Drawing.Point(115, 140);
			this.labelPrevious.Name = "labelPrevious";
			this.labelPrevious.Size = new System.Drawing.Size(259, 30);
			this.labelPrevious.TabIndex = 36;
			this.labelPrevious.Text = "Previous Value:";
			// 
			// FormTimeInput
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.BackColor = System.Drawing.Color.SteelBlue;
			this.ClientSize = new System.Drawing.Size(1024, 514);
			this.Controls.Add(this.labelPrevious);
			this.Controls.Add(this.labelErrNumInput);
			this.Controls.Add(this.labelTimeTitle);
			this.Controls.Add(this.textBoxTimeInput);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonEnter);
			this.Controls.Add(this.buttonClear);
			this.Controls.Add(this.button0);
			this.Controls.Add(this.button9);
			this.Controls.Add(this.button8);
			this.Controls.Add(this.button7);
			this.Controls.Add(this.button6);
			this.Controls.Add(this.button5);
			this.Controls.Add(this.button4);
			this.Controls.Add(this.button3);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.button1);
			this.Font = new System.Drawing.Font("Tahoma", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
			this.Name = "FormTimeInput";
			this.Text = "FormTimeInput";
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelTimeTitle;
        private System.Windows.Forms.TextBox textBoxTimeInput;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonEnter;
        private System.Windows.Forms.Button buttonClear;
        private System.Windows.Forms.Button button0;
        private System.Windows.Forms.Button button9;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label labelErrNumInput;
		private System.Windows.Forms.Label labelPrevious;
	}
}