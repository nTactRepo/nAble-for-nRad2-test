namespace nAble
{
    partial class FormRecipe
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.buttonRecipeEdit = new System.Windows.Forms.Button();
            this.buttonRecipeDelete = new System.Windows.Forms.Button();
            this.buttonRecipeAdd = new System.Windows.Forms.Button();
            this.buttonRecipeExport = new System.Windows.Forms.Button();
            this.vScrollBarRecipes = new System.Windows.Forms.VScrollBar();
            this.dataGridViewRecipes = new System.Windows.Forms.DataGridView();
            this.splitContainerRecipeView = new System.Windows.Forms.SplitContainer();
            this.buttonEditTemplate = new System.Windows.Forms.Button();
            this.buttonSegmentedDefaults = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewRecipes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerRecipeView)).BeginInit();
            this.splitContainerRecipeView.Panel1.SuspendLayout();
            this.splitContainerRecipeView.Panel2.SuspendLayout();
            this.splitContainerRecipeView.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonRecipeEdit
            // 
            this.buttonRecipeEdit.Enabled = false;
            this.buttonRecipeEdit.Font = new System.Drawing.Font("Tahoma", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonRecipeEdit.Location = new System.Drawing.Point(842, 444);
            this.buttonRecipeEdit.Name = "buttonRecipeEdit";
            this.buttonRecipeEdit.Size = new System.Drawing.Size(172, 60);
            this.buttonRecipeEdit.TabIndex = 11;
            this.buttonRecipeEdit.Text = "Edit";
            this.buttonRecipeEdit.UseVisualStyleBackColor = true;
            this.buttonRecipeEdit.Click += new System.EventHandler(this.buttonRecipeEdit_Click);
            // 
            // buttonRecipeDelete
            // 
            this.buttonRecipeDelete.Enabled = false;
            this.buttonRecipeDelete.Font = new System.Drawing.Font("Tahoma", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonRecipeDelete.Location = new System.Drawing.Point(482, 444);
            this.buttonRecipeDelete.Name = "buttonRecipeDelete";
            this.buttonRecipeDelete.Size = new System.Drawing.Size(172, 60);
            this.buttonRecipeDelete.TabIndex = 12;
            this.buttonRecipeDelete.Text = "Delete";
            this.buttonRecipeDelete.UseVisualStyleBackColor = true;
            this.buttonRecipeDelete.Click += new System.EventHandler(this.buttonRecipeDelete_Click);
            // 
            // buttonRecipeAdd
            // 
            this.buttonRecipeAdd.Font = new System.Drawing.Font("Tahoma", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonRecipeAdd.Location = new System.Drawing.Point(662, 444);
            this.buttonRecipeAdd.Name = "buttonRecipeAdd";
            this.buttonRecipeAdd.Size = new System.Drawing.Size(172, 60);
            this.buttonRecipeAdd.TabIndex = 13;
            this.buttonRecipeAdd.Text = "Add";
            this.buttonRecipeAdd.UseVisualStyleBackColor = true;
            this.buttonRecipeAdd.Click += new System.EventHandler(this.buttonRecipeAdd_Click);
            // 
            // buttonRecipeExport
            // 
            this.buttonRecipeExport.Font = new System.Drawing.Font("Tahoma", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonRecipeExport.Location = new System.Drawing.Point(302, 444);
            this.buttonRecipeExport.Name = "buttonRecipeExport";
            this.buttonRecipeExport.Size = new System.Drawing.Size(172, 60);
            this.buttonRecipeExport.TabIndex = 14;
            this.buttonRecipeExport.Text = "Export";
            this.buttonRecipeExport.UseVisualStyleBackColor = true;
            this.buttonRecipeExport.Visible = false;
            this.buttonRecipeExport.Click += new System.EventHandler(this.buttonRecipeExport_Click);
            // 
            // vScrollBarRecipes
            // 
            this.vScrollBarRecipes.Dock = System.Windows.Forms.DockStyle.Top;
            this.vScrollBarRecipes.LargeChange = 25;
            this.vScrollBarRecipes.Location = new System.Drawing.Point(0, 0);
            this.vScrollBarRecipes.Minimum = 1;
            this.vScrollBarRecipes.Name = "vScrollBarRecipes";
            this.vScrollBarRecipes.Size = new System.Drawing.Size(50, 437);
            this.vScrollBarRecipes.TabIndex = 16;
            this.vScrollBarRecipes.Value = 1;
            this.vScrollBarRecipes.ValueChanged += new System.EventHandler(this.vScrollBarRecipes_ValueChanged);
            // 
            // dataGridViewRecipes
            // 
            this.dataGridViewRecipes.AllowUserToAddRows = false;
            this.dataGridViewRecipes.AllowUserToDeleteRows = false;
            this.dataGridViewRecipes.AllowUserToResizeColumns = false;
            this.dataGridViewRecipes.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewRecipes.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridViewRecipes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.Padding = new System.Windows.Forms.Padding(0, 0, 0, 1);
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewRecipes.DefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridViewRecipes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewRecipes.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewRecipes.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dataGridViewRecipes.MultiSelect = false;
            this.dataGridViewRecipes.Name = "dataGridViewRecipes";
            this.dataGridViewRecipes.ReadOnly = true;
            this.dataGridViewRecipes.RowHeadersVisible = false;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Tahoma", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dataGridViewRecipes.RowsDefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridViewRecipes.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Tahoma", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dataGridViewRecipes.RowTemplate.Height = 40;
            this.dataGridViewRecipes.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.dataGridViewRecipes.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewRecipes.Size = new System.Drawing.Size(970, 436);
            this.dataGridViewRecipes.TabIndex = 15;
            this.dataGridViewRecipes.CellContentDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewRecipes_CellContentDoubleClick);
            // 
            // splitContainerRecipeView
            // 
            this.splitContainerRecipeView.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitContainerRecipeView.Location = new System.Drawing.Point(0, 0);
            this.splitContainerRecipeView.Name = "splitContainerRecipeView";
            // 
            // splitContainerRecipeView.Panel1
            // 
            this.splitContainerRecipeView.Panel1.Controls.Add(this.dataGridViewRecipes);
            // 
            // splitContainerRecipeView.Panel2
            // 
            this.splitContainerRecipeView.Panel2.Controls.Add(this.vScrollBarRecipes);
            this.splitContainerRecipeView.Panel2MinSize = 50;
            this.splitContainerRecipeView.Size = new System.Drawing.Size(1024, 436);
            this.splitContainerRecipeView.SplitterDistance = 970;
            this.splitContainerRecipeView.TabIndex = 17;
            // 
            // buttonEditTemplate
            // 
            this.buttonEditTemplate.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonEditTemplate.Location = new System.Drawing.Point(9, 444);
            this.buttonEditTemplate.Name = "buttonEditTemplate";
            this.buttonEditTemplate.Size = new System.Drawing.Size(130, 60);
            this.buttonEditTemplate.TabIndex = 18;
            this.buttonEditTemplate.Text = "Change Defaults";
            this.buttonEditTemplate.UseVisualStyleBackColor = true;
            this.buttonEditTemplate.Click += new System.EventHandler(this.buttonEditTemplate_Click);
            // 
            // buttonSegmentedDefaults
            // 
            this.buttonSegmentedDefaults.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonSegmentedDefaults.Location = new System.Drawing.Point(143, 444);
            this.buttonSegmentedDefaults.Name = "buttonSegmentedDefaults";
            this.buttonSegmentedDefaults.Size = new System.Drawing.Size(130, 60);
            this.buttonSegmentedDefaults.TabIndex = 19;
            this.buttonSegmentedDefaults.Text = "Segmented Defaults";
            this.buttonSegmentedDefaults.UseVisualStyleBackColor = true;
            this.buttonSegmentedDefaults.Click += new System.EventHandler(this.buttonSegmentedDefaults_Click);
            // 
            // FormRecipe
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 23F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1024, 514);
            this.Controls.Add(this.buttonSegmentedDefaults);
            this.Controls.Add(this.buttonEditTemplate);
            this.Controls.Add(this.splitContainerRecipeView);
            this.Controls.Add(this.buttonRecipeExport);
            this.Controls.Add(this.buttonRecipeAdd);
            this.Controls.Add(this.buttonRecipeDelete);
            this.Controls.Add(this.buttonRecipeEdit);
            this.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(5);
            this.Name = "FormRecipe";
            this.Text = "Recipe Management";
            this.Load += new System.EventHandler(this.FormRecipe_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewRecipes)).EndInit();
            this.splitContainerRecipeView.Panel1.ResumeLayout(false);
            this.splitContainerRecipeView.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerRecipeView)).EndInit();
            this.splitContainerRecipeView.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button buttonRecipeEdit;
        private System.Windows.Forms.Button buttonRecipeDelete;
        private System.Windows.Forms.Button buttonRecipeAdd;
        private System.Windows.Forms.Button buttonRecipeExport;
        private System.Windows.Forms.VScrollBar vScrollBarRecipes;
        private System.Windows.Forms.DataGridView dataGridViewRecipes;
        private System.Windows.Forms.SplitContainer splitContainerRecipeView;
        private System.Windows.Forms.Button buttonEditTemplate;
        private System.Windows.Forms.Button buttonSegmentedDefaults;
    }
}