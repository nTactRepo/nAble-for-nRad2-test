namespace nAble
{
    partial class FormLogListing
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
            this.listBoxLogFiles = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // listBoxLogFiles
            // 
            this.listBoxLogFiles.Font = new System.Drawing.Font("Tahoma", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listBoxLogFiles.FormattingEnabled = true;
            this.listBoxLogFiles.ItemHeight = 39;
            this.listBoxLogFiles.Location = new System.Drawing.Point(13, 13);
            this.listBoxLogFiles.Name = "listBoxLogFiles";
            this.listBoxLogFiles.Size = new System.Drawing.Size(999, 472);
            this.listBoxLogFiles.TabIndex = 0;
            this.listBoxLogFiles.SelectedIndexChanged += new System.EventHandler(this.listBoxLogFiles_SelectedIndexChanged);
            this.listBoxLogFiles.DoubleClick += new System.EventHandler(this.listBoxLogFiles_DoubleClick);
            // 
            // FormLogListing
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(15F, 33F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1024, 524);
            this.Controls.Add(this.listBoxLogFiles);
            this.Font = new System.Drawing.Font("Tahoma", 20.25F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(8);
            this.Name = "FormLogListing";
            this.Text = "Logs";
            this.Load += new System.EventHandler(this.FormLogListing_Load);
            this.Enter += new System.EventHandler(this.FormLogListing_Enter);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox listBoxLogFiles;
    }
}