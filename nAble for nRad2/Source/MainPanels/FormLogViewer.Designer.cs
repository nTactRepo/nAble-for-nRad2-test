namespace nAble
{
    partial class FormLogViewer
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dataGridViewActivityHistory = new System.Windows.Forms.DataGridView();
            this.contextMenuStripFind = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.findToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.findNextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.findPrevToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.vScrollBarLog = new System.Windows.Forms.VScrollBar();
            this.labelLoadProgress = new System.Windows.Forms.Label();
            this.gpFilter = new DevComponents.DotNetBar.Controls.GroupPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.rbNone = new System.Windows.Forms.RadioButton();
            this.rbAction = new System.Windows.Forms.RadioButton();
            this.rbErr = new System.Windows.Forms.RadioButton();
            this.rbInfo = new System.Windows.Forms.RadioButton();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewActivityHistory)).BeginInit();
            this.contextMenuStripFind.SuspendLayout();
            this.gpFilter.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridViewActivityHistory
            // 
            this.dataGridViewActivityHistory.AllowUserToAddRows = false;
            this.dataGridViewActivityHistory.AllowUserToDeleteRows = false;
            this.dataGridViewActivityHistory.AllowUserToResizeColumns = false;
            this.dataGridViewActivityHistory.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Tahoma", 20.25F);
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewActivityHistory.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridViewActivityHistory.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewActivityHistory.ContextMenuStrip = this.contextMenuStripFind;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Tahoma", 20.25F);
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.Padding = new System.Windows.Forms.Padding(0, 0, 0, 1);
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewActivityHistory.DefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridViewActivityHistory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewActivityHistory.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewActivityHistory.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dataGridViewActivityHistory.MultiSelect = false;
            this.dataGridViewActivityHistory.Name = "dataGridViewActivityHistory";
            this.dataGridViewActivityHistory.ReadOnly = true;
            this.dataGridViewActivityHistory.RowHeadersVisible = false;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dataGridViewActivityHistory.RowsDefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridViewActivityHistory.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dataGridViewActivityHistory.RowTemplate.Height = 24;
            this.dataGridViewActivityHistory.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dataGridViewActivityHistory.Size = new System.Drawing.Size(1024, 514);
            this.dataGridViewActivityHistory.TabIndex = 2;
            this.dataGridViewActivityHistory.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewActivityHistory_CellContentClick);
            // 
            // contextMenuStripFind
            // 
            this.contextMenuStripFind.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.findToolStripMenuItem,
            this.findNextToolStripMenuItem,
            this.findPrevToolStripMenuItem});
            this.contextMenuStripFind.Name = "contextMenuStripFind";
            this.contextMenuStripFind.Size = new System.Drawing.Size(147, 82);
            // 
            // findToolStripMenuItem
            // 
            this.findToolStripMenuItem.Name = "findToolStripMenuItem";
            this.findToolStripMenuItem.Size = new System.Drawing.Size(146, 26);
            this.findToolStripMenuItem.Text = "&Find";
            this.findToolStripMenuItem.Click += new System.EventHandler(this.findToolStripMenuItem_Click);
            // 
            // findNextToolStripMenuItem
            // 
            this.findNextToolStripMenuItem.Name = "findNextToolStripMenuItem";
            this.findNextToolStripMenuItem.Size = new System.Drawing.Size(146, 26);
            this.findNextToolStripMenuItem.Text = "Find &Next";
            this.findNextToolStripMenuItem.Click += new System.EventHandler(this.findNextToolStripMenuItem_Click);
            // 
            // findPrevToolStripMenuItem
            // 
            this.findPrevToolStripMenuItem.Name = "findPrevToolStripMenuItem";
            this.findPrevToolStripMenuItem.Size = new System.Drawing.Size(146, 26);
            this.findPrevToolStripMenuItem.Text = "Find &Prev";
            this.findPrevToolStripMenuItem.Click += new System.EventHandler(this.findPrevToolStripMenuItem_Click);
            // 
            // vScrollBarLog
            // 
            this.vScrollBarLog.Dock = System.Windows.Forms.DockStyle.Right;
            this.vScrollBarLog.LargeChange = 25;
            this.vScrollBarLog.Location = new System.Drawing.Point(974, 0);
            this.vScrollBarLog.Minimum = 1;
            this.vScrollBarLog.Name = "vScrollBarLog";
            this.vScrollBarLog.Size = new System.Drawing.Size(50, 514);
            this.vScrollBarLog.TabIndex = 9;
            this.vScrollBarLog.Value = 1;
            this.vScrollBarLog.ValueChanged += new System.EventHandler(this.vScrollBarLog_ValueChanged);
            // 
            // labelLoadProgress
            // 
            this.labelLoadProgress.AutoSize = true;
            this.labelLoadProgress.BackColor = System.Drawing.Color.LightSteelBlue;
            this.labelLoadProgress.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelLoadProgress.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.labelLoadProgress.Location = new System.Drawing.Point(374, 226);
            this.labelLoadProgress.Name = "labelLoadProgress";
            this.labelLoadProgress.Size = new System.Drawing.Size(217, 35);
            this.labelLoadProgress.TabIndex = 10;
            this.labelLoadProgress.Text = "File Loading: 0%";
            this.labelLoadProgress.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelLoadProgress.Visible = false;
            // 
            // gpFilter
            // 
            this.gpFilter.BackColor = System.Drawing.Color.Transparent;
            this.gpFilter.CanvasColor = System.Drawing.SystemColors.Control;
            this.gpFilter.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007;
            this.gpFilter.Controls.Add(this.label1);
            this.gpFilter.Controls.Add(this.rbNone);
            this.gpFilter.Controls.Add(this.rbAction);
            this.gpFilter.Controls.Add(this.rbErr);
            this.gpFilter.Controls.Add(this.rbInfo);
            this.gpFilter.DisabledBackColor = System.Drawing.Color.Empty;
            this.gpFilter.Location = new System.Drawing.Point(29, 425);
            this.gpFilter.Name = "gpFilter";
            this.gpFilter.Size = new System.Drawing.Size(916, 64);
            // 
            // 
            // 
            this.gpFilter.Style.BackColor2SchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground2;
            this.gpFilter.Style.BackColorGradientAngle = 90;
            this.gpFilter.Style.BackColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground;
            this.gpFilter.Style.BorderBottom = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.gpFilter.Style.BorderBottomWidth = 1;
            this.gpFilter.Style.BorderColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder;
            this.gpFilter.Style.BorderLeft = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.gpFilter.Style.BorderLeftWidth = 1;
            this.gpFilter.Style.BorderRight = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.gpFilter.Style.BorderRightWidth = 1;
            this.gpFilter.Style.BorderTop = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.gpFilter.Style.BorderTopWidth = 1;
            this.gpFilter.Style.CornerDiameter = 4;
            this.gpFilter.Style.CornerType = DevComponents.DotNetBar.eCornerType.Rounded;
            this.gpFilter.Style.TextAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Center;
            this.gpFilter.Style.TextColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText;
            this.gpFilter.Style.TextLineAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Near;
            // 
            // 
            // 
            this.gpFilter.StyleMouseDown.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            // 
            // 
            // 
            this.gpFilter.StyleMouseOver.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.gpFilter.TabIndex = 11;
            this.gpFilter.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 20.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(24, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(179, 33);
            this.label1.TabIndex = 4;
            this.label1.Text = "Choose Filter:";
            // 
            // rbNone
            // 
            this.rbNone.AutoSize = true;
            this.rbNone.Location = new System.Drawing.Point(778, 11);
            this.rbNone.Name = "rbNone";
            this.rbNone.Size = new System.Drawing.Size(103, 37);
            this.rbNone.TabIndex = 3;
            this.rbNone.TabStop = true;
            this.rbNone.Text = "NONE";
            this.rbNone.UseVisualStyleBackColor = true;
            this.rbNone.CheckedChanged += new System.EventHandler(this.rbNone_CheckedChanged);
            // 
            // rbAction
            // 
            this.rbAction.AutoSize = true;
            this.rbAction.Location = new System.Drawing.Point(588, 11);
            this.rbAction.Name = "rbAction";
            this.rbAction.Size = new System.Drawing.Size(128, 37);
            this.rbAction.TabIndex = 2;
            this.rbAction.TabStop = true;
            this.rbAction.Text = "ACTION";
            this.rbAction.UseVisualStyleBackColor = true;
            this.rbAction.CheckedChanged += new System.EventHandler(this.rbAction_CheckedChanged);
            // 
            // rbErr
            // 
            this.rbErr.AutoSize = true;
            this.rbErr.Location = new System.Drawing.Point(408, 11);
            this.rbErr.Name = "rbErr";
            this.rbErr.Size = new System.Drawing.Size(118, 37);
            this.rbErr.TabIndex = 1;
            this.rbErr.TabStop = true;
            this.rbErr.Text = "ERROR";
            this.rbErr.UseVisualStyleBackColor = true;
            this.rbErr.CheckedChanged += new System.EventHandler(this.rbErr_CheckedChanged);
            // 
            // rbInfo
            // 
            this.rbInfo.AutoSize = true;
            this.rbInfo.Location = new System.Drawing.Point(252, 11);
            this.rbInfo.Name = "rbInfo";
            this.rbInfo.Size = new System.Drawing.Size(94, 37);
            this.rbInfo.TabIndex = 0;
            this.rbInfo.TabStop = true;
            this.rbInfo.Text = "INFO";
            this.rbInfo.UseVisualStyleBackColor = true;
            this.rbInfo.CheckedChanged += new System.EventHandler(this.rbInfo_CheckedChanged);
            // 
            // FormLogViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(15F, 33F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1024, 514);
            this.Controls.Add(this.gpFilter);
            this.Controls.Add(this.labelLoadProgress);
            this.Controls.Add(this.vScrollBarLog);
            this.Controls.Add(this.dataGridViewActivityHistory);
            this.Font = new System.Drawing.Font("Tahoma", 20.25F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(8);
            this.Name = "FormLogViewer";
            this.Text = "Viewer";
            this.Load += new System.EventHandler(this.FormLogViewer_Load);
            this.Enter += new System.EventHandler(this.FormLogViewer_Enter);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewActivityHistory)).EndInit();
            this.contextMenuStripFind.ResumeLayout(false);
            this.gpFilter.ResumeLayout(false);
            this.gpFilter.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridViewActivityHistory;
        private System.Windows.Forms.VScrollBar vScrollBarLog;
        private System.Windows.Forms.Label labelLoadProgress;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripFind;
        private System.Windows.Forms.ToolStripMenuItem findToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem findNextToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem findPrevToolStripMenuItem;
        private DevComponents.DotNetBar.Controls.GroupPanel gpFilter;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton rbNone;
        private System.Windows.Forms.RadioButton rbAction;
        private System.Windows.Forms.RadioButton rbErr;
        private System.Windows.Forms.RadioButton rbInfo;
    }
}