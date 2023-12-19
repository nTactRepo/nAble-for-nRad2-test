namespace nAble
{
    partial class FormRecipeAdd
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
			this.textBoxNewRecipeName = new System.Windows.Forms.TextBox();
			this.labelName = new System.Windows.Forms.Label();
			this.radioButtonStartFrom = new System.Windows.Forms.RadioButton();
			this.radioButtonBlank = new System.Windows.Forms.RadioButton();
			this.comboBoxCopyFrom = new System.Windows.Forms.ComboBox();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.checkBoxSegmented = new System.Windows.Forms.CheckBox();
			this.label1 = new System.Windows.Forms.Label();
			this.buttonKeyboard4Name = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// textBoxNewRecipeName
			// 
			this.textBoxNewRecipeName.Font = new System.Drawing.Font("Tahoma", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.textBoxNewRecipeName.Location = new System.Drawing.Point(248, 36);
			this.textBoxNewRecipeName.MaxLength = 128;
			this.textBoxNewRecipeName.Name = "textBoxNewRecipeName";
			this.textBoxNewRecipeName.Size = new System.Drawing.Size(491, 43);
			this.textBoxNewRecipeName.TabIndex = 18;
			// 
			// labelName
			// 
			this.labelName.AutoSize = true;
			this.labelName.Font = new System.Drawing.Font("Tahoma", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelName.ForeColor = System.Drawing.Color.White;
			this.labelName.Location = new System.Drawing.Point(62, 41);
			this.labelName.Name = "labelName";
			this.labelName.Size = new System.Drawing.Size(180, 33);
			this.labelName.TabIndex = 17;
			this.labelName.Text = "Recipe Name:";
			this.labelName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// radioButtonStartFrom
			// 
			this.radioButtonStartFrom.ForeColor = System.Drawing.Color.White;
			this.radioButtonStartFrom.Location = new System.Drawing.Point(55, 223);
			this.radioButtonStartFrom.Name = "radioButtonStartFrom";
			this.radioButtonStartFrom.Size = new System.Drawing.Size(187, 46);
			this.radioButtonStartFrom.TabIndex = 15;
			this.radioButtonStartFrom.Text = "Copy From:";
			// 
			// radioButtonBlank
			// 
			this.radioButtonBlank.Checked = true;
			this.radioButtonBlank.ForeColor = System.Drawing.Color.White;
			this.radioButtonBlank.Location = new System.Drawing.Point(55, 145);
			this.radioButtonBlank.Name = "radioButtonBlank";
			this.radioButtonBlank.Size = new System.Drawing.Size(306, 46);
			this.radioButtonBlank.TabIndex = 14;
			this.radioButtonBlank.TabStop = true;
			this.radioButtonBlank.Text = "Use Recipe Defaults";
			// 
			// comboBoxCopyFrom
			// 
			this.comboBoxCopyFrom.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.comboBoxCopyFrom.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxCopyFrom.Enabled = false;
			this.comboBoxCopyFrom.Font = new System.Drawing.Font("Tahoma", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.comboBoxCopyFrom.Location = new System.Drawing.Point(248, 225);
			this.comboBoxCopyFrom.Name = "comboBoxCopyFrom";
			this.comboBoxCopyFrom.Size = new System.Drawing.Size(715, 43);
			this.comboBoxCopyFrom.Sorted = true;
			this.comboBoxCopyFrom.TabIndex = 16;
			// 
			// buttonOK
			// 
			this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOK.BackColor = System.Drawing.SystemColors.Control;
			this.buttonOK.Enabled = false;
			this.buttonOK.Location = new System.Drawing.Point(841, 407);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(155, 57);
			this.buttonOK.TabIndex = 12;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = false;
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.BackColor = System.Drawing.SystemColors.Control;
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(680, 407);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(155, 57);
			this.buttonCancel.TabIndex = 13;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = false;
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// checkBoxSegmented
			// 
			this.checkBoxSegmented.AutoSize = true;
			this.checkBoxSegmented.ForeColor = System.Drawing.Color.White;
			this.checkBoxSegmented.Location = new System.Drawing.Point(345, 151);
			this.checkBoxSegmented.Name = "checkBoxSegmented";
			this.checkBoxSegmented.Size = new System.Drawing.Size(168, 37);
			this.checkBoxSegmented.TabIndex = 19;
			this.checkBoxSegmented.Text = "Segmented";
			this.checkBoxSegmented.UseVisualStyleBackColor = true;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Tahoma", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.ForeColor = System.Drawing.Color.White;
			this.label1.Location = new System.Drawing.Point(75, 189);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(40, 33);
			this.label1.TabIndex = 20;
			this.label1.Text = "or";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// buttonKeyboard4Name
			// 
			this.buttonKeyboard4Name.Image = Resources._48pxkeyboard;
			this.buttonKeyboard4Name.Location = new System.Drawing.Point(745, 36);
			this.buttonKeyboard4Name.Name = "buttonKeyboard4Name";
			this.buttonKeyboard4Name.Size = new System.Drawing.Size(57, 43);
			this.buttonKeyboard4Name.TabIndex = 21;
			this.buttonKeyboard4Name.Text = "...";
			this.buttonKeyboard4Name.UseVisualStyleBackColor = true;
			this.buttonKeyboard4Name.Click += new System.EventHandler(this.buttonKeyboard4Name_Click);
			// 
			// FormRecipeAdd
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.BackColor = System.Drawing.Color.SteelBlue;
			this.ClientSize = new System.Drawing.Size(1008, 476);
			this.Controls.Add(this.buttonKeyboard4Name);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.checkBoxSegmented);
			this.Controls.Add(this.textBoxNewRecipeName);
			this.Controls.Add(this.labelName);
			this.Controls.Add(this.radioButtonStartFrom);
			this.Controls.Add(this.radioButtonBlank);
			this.Controls.Add(this.comboBoxCopyFrom);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.buttonCancel);
			this.Font = new System.Drawing.Font("Tahoma", 20.25F);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "FormRecipeAdd";
			this.Text = "Add";
			this.Load += new System.EventHandler(this.FormRecipeAdd_Load);
			this.Enter += new System.EventHandler(this.FormRecipeAdd_Enter);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label labelName;
        private System.Windows.Forms.RadioButton radioButtonStartFrom;
        private System.Windows.Forms.RadioButton radioButtonBlank;
        private System.Windows.Forms.ComboBox comboBoxCopyFrom;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.CheckBox checkBoxSegmented;
        private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button buttonKeyboard4Name;
		private System.Windows.Forms.TextBox textBoxNewRecipeName;
	}
}