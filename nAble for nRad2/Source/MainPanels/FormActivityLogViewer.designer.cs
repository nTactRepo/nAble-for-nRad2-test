namespace nAble
{
    partial class FormActivityLogViewer
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dataGridViewActivityHistory = new System.Windows.Forms.DataGridView();
            this.vScrollBarAdv1 = new DevComponents.DotNetBar.VScrollBarAdv();
            this.panelSearchResults = new System.Windows.Forms.Panel();
            this.expandablePanelSearchControls = new DevComponents.DotNetBar.ExpandablePanel();
            this.panelControls = new System.Windows.Forms.Panel();
            this.buttonXGo = new DevComponents.DotNetBar.ButtonX();
            this.label1 = new System.Windows.Forms.Label();
            this.dateTimeInputEndDate = new DevComponents.Editors.DateTimeAdv.DateTimeInput();
            this.dateTimeInputStartDate = new DevComponents.Editors.DateTimeAdv.DateTimeInput();
            this.textBoxXSearch = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.buttonXFindPrev = new DevComponents.DotNetBar.ButtonX();
            this.buttonXExportXML = new DevComponents.DotNetBar.ButtonX();
            this.buttonXFindFirst = new DevComponents.DotNetBar.ButtonX();
            this.label3 = new System.Windows.Forms.Label();
            this.checkBoxXFindOcc = new DevComponents.DotNetBar.Controls.CheckBoxX();
            this.buttonXFindNext = new DevComponents.DotNetBar.ButtonX();
            this.buttonXFindLast = new DevComponents.DotNetBar.ButtonX();
            this.contextMenuStripResults = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItemSearchForString = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewActivityHistory)).BeginInit();
            this.panelSearchResults.SuspendLayout();
            this.expandablePanelSearchControls.SuspendLayout();
            this.panelControls.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dateTimeInputEndDate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dateTimeInputStartDate)).BeginInit();
            this.contextMenuStripResults.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridViewActivityHistory
            // 
            this.dataGridViewActivityHistory.AllowUserToAddRows = false;
            this.dataGridViewActivityHistory.AllowUserToDeleteRows = false;
            this.dataGridViewActivityHistory.AllowUserToResizeColumns = false;
            this.dataGridViewActivityHistory.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.BottomCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.DarkOrange;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Tahoma", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewActivityHistory.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridViewActivityHistory.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.DarkOrange;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewActivityHistory.DefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridViewActivityHistory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewActivityHistory.Location = new System.Drawing.Point(0, 5);
            this.dataGridViewActivityHistory.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dataGridViewActivityHistory.MultiSelect = false;
            this.dataGridViewActivityHistory.Name = "dataGridViewActivityHistory";
            this.dataGridViewActivityHistory.ReadOnly = true;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewActivityHistory.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridViewActivityHistory.RowHeadersVisible = false;
            this.dataGridViewActivityHistory.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dataGridViewActivityHistory.RowsDefaultCellStyle = dataGridViewCellStyle4;
            this.dataGridViewActivityHistory.RowTemplate.DefaultCellStyle.BackColor = System.Drawing.SystemColors.Window;
            this.dataGridViewActivityHistory.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dataGridViewActivityHistory.RowTemplate.DefaultCellStyle.ForeColor = System.Drawing.SystemColors.ControlText;
            this.dataGridViewActivityHistory.RowTemplate.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.DarkOrange;
            this.dataGridViewActivityHistory.RowTemplate.DefaultCellStyle.SelectionForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.dataGridViewActivityHistory.RowTemplate.DefaultCellStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewActivityHistory.RowTemplate.Height = 25;
            this.dataGridViewActivityHistory.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.dataGridViewActivityHistory.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dataGridViewActivityHistory.Size = new System.Drawing.Size(967, 357);
            this.dataGridViewActivityHistory.TabIndex = 4;
            this.dataGridViewActivityHistory.CellContextMenuStripNeeded += new System.Windows.Forms.DataGridViewCellContextMenuStripNeededEventHandler(this.dataGridViewActivityHistory_CellContextMenuStripNeeded);
            this.dataGridViewActivityHistory.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.DataGridViewActivityHistory_DataBindingComplete);
            this.dataGridViewActivityHistory.Scroll += new System.Windows.Forms.ScrollEventHandler(this.DataGridViewActivityHistory_Scroll);
            // 
            // vScrollBarAdv1
            // 
            this.vScrollBarAdv1.Dock = System.Windows.Forms.DockStyle.Right;
            this.vScrollBarAdv1.Location = new System.Drawing.Point(967, 5);
            this.vScrollBarAdv1.Margin = new System.Windows.Forms.Padding(0);
            this.vScrollBarAdv1.Name = "vScrollBarAdv1";
            this.vScrollBarAdv1.Size = new System.Drawing.Size(57, 357);
            this.vScrollBarAdv1.TabIndex = 3;
            this.vScrollBarAdv1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.VScrollBarAdv1_Scroll);
            // 
            // panelSearchResults
            // 
            this.panelSearchResults.Controls.Add(this.dataGridViewActivityHistory);
            this.panelSearchResults.Controls.Add(this.vScrollBarAdv1);
            this.panelSearchResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelSearchResults.Location = new System.Drawing.Point(0, 152);
            this.panelSearchResults.Margin = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.panelSearchResults.Name = "panelSearchResults";
            this.panelSearchResults.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.panelSearchResults.Size = new System.Drawing.Size(1024, 362);
            this.panelSearchResults.TabIndex = 9;
            // 
            // expandablePanelSearchControls
            // 
            this.expandablePanelSearchControls.CanvasColor = System.Drawing.SystemColors.Control;
            this.expandablePanelSearchControls.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.expandablePanelSearchControls.Controls.Add(this.panelControls);
            this.expandablePanelSearchControls.DisabledBackColor = System.Drawing.Color.Empty;
            this.expandablePanelSearchControls.Dock = System.Windows.Forms.DockStyle.Top;
            this.expandablePanelSearchControls.HideControlsWhenCollapsed = true;
            this.expandablePanelSearchControls.Location = new System.Drawing.Point(0, 0);
            this.expandablePanelSearchControls.Margin = new System.Windows.Forms.Padding(0);
            this.expandablePanelSearchControls.Name = "expandablePanelSearchControls";
            this.expandablePanelSearchControls.Size = new System.Drawing.Size(1024, 152);
            this.expandablePanelSearchControls.Style.Alignment = System.Drawing.StringAlignment.Center;
            this.expandablePanelSearchControls.Style.BackColor1.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground;
            this.expandablePanelSearchControls.Style.BackColor2.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground2;
            this.expandablePanelSearchControls.Style.Border = DevComponents.DotNetBar.eBorderType.SingleLine;
            this.expandablePanelSearchControls.Style.BorderColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.BarDockedBorder;
            this.expandablePanelSearchControls.Style.ForeColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.ItemText;
            this.expandablePanelSearchControls.Style.GradientAngle = 90;
            this.expandablePanelSearchControls.TabIndex = 10;
            this.expandablePanelSearchControls.TitleStyle.Alignment = System.Drawing.StringAlignment.Center;
            this.expandablePanelSearchControls.TitleStyle.BackColor1.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground;
            this.expandablePanelSearchControls.TitleStyle.BackColor2.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground2;
            this.expandablePanelSearchControls.TitleStyle.Border = DevComponents.DotNetBar.eBorderType.RaisedInner;
            this.expandablePanelSearchControls.TitleStyle.BorderColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder;
            this.expandablePanelSearchControls.TitleStyle.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.expandablePanelSearchControls.TitleStyle.ForeColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText;
            this.expandablePanelSearchControls.TitleStyle.GradientAngle = 90;
            this.expandablePanelSearchControls.TitleText = "Log Viewer";
            // 
            // panelControls
            // 
            this.panelControls.BackColor = System.Drawing.Color.Transparent;
            this.panelControls.Controls.Add(this.buttonXGo);
            this.panelControls.Controls.Add(this.label1);
            this.panelControls.Controls.Add(this.dateTimeInputEndDate);
            this.panelControls.Controls.Add(this.dateTimeInputStartDate);
            this.panelControls.Controls.Add(this.textBoxXSearch);
            this.panelControls.Controls.Add(this.buttonXFindPrev);
            this.panelControls.Controls.Add(this.buttonXExportXML);
            this.panelControls.Controls.Add(this.buttonXFindFirst);
            this.panelControls.Controls.Add(this.label3);
            this.panelControls.Controls.Add(this.checkBoxXFindOcc);
            this.panelControls.Controls.Add(this.buttonXFindNext);
            this.panelControls.Controls.Add(this.buttonXFindLast);
            this.panelControls.Location = new System.Drawing.Point(9, 38);
            this.panelControls.Name = "panelControls";
            this.panelControls.Size = new System.Drawing.Size(1000, 102);
            this.panelControls.TabIndex = 22;
            // 
            // buttonXGo
            // 
            this.buttonXGo.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.buttonXGo.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.buttonXGo.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonXGo.Location = new System.Drawing.Point(277, 9);
            this.buttonXGo.Name = "buttonXGo";
            this.buttonXGo.Size = new System.Drawing.Size(61, 76);
            this.buttonXGo.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.buttonXGo.TabIndex = 19;
            this.buttonXGo.Text = "Go";
            this.buttonXGo.Click += new System.EventHandler(this.ButtonXGo_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(5, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(85, 19);
            this.label1.TabIndex = 3;
            this.label1.Text = "Start Date:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // dateTimeInputEndDate
            // 
            // 
            // 
            // 
            this.dateTimeInputEndDate.BackgroundStyle.Class = "DateTimeInputBackground";
            this.dateTimeInputEndDate.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.dateTimeInputEndDate.ButtonDropDown.Shortcut = DevComponents.DotNetBar.eShortcut.AltDown;
            this.dateTimeInputEndDate.ButtonDropDown.Visible = true;
            this.dateTimeInputEndDate.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dateTimeInputEndDate.IsPopupCalendarOpen = false;
            this.dateTimeInputEndDate.Location = new System.Drawing.Point(91, 55);
            // 
            // 
            // 
            // 
            // 
            // 
            this.dateTimeInputEndDate.MonthCalendar.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.dateTimeInputEndDate.MonthCalendar.CalendarDimensions = new System.Drawing.Size(1, 1);
            this.dateTimeInputEndDate.MonthCalendar.ClearButtonVisible = true;
            // 
            // 
            // 
            this.dateTimeInputEndDate.MonthCalendar.CommandsBackgroundStyle.BackColor2SchemePart = DevComponents.DotNetBar.eColorSchemePart.BarBackground2;
            this.dateTimeInputEndDate.MonthCalendar.CommandsBackgroundStyle.BackColorGradientAngle = 90;
            this.dateTimeInputEndDate.MonthCalendar.CommandsBackgroundStyle.BackColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.BarBackground;
            this.dateTimeInputEndDate.MonthCalendar.CommandsBackgroundStyle.BorderTop = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.dateTimeInputEndDate.MonthCalendar.CommandsBackgroundStyle.BorderTopColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.BarDockedBorder;
            this.dateTimeInputEndDate.MonthCalendar.CommandsBackgroundStyle.BorderTopWidth = 1;
            this.dateTimeInputEndDate.MonthCalendar.CommandsBackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.dateTimeInputEndDate.MonthCalendar.DisplayMonth = new System.DateTime(2019, 7, 1, 0, 0, 0, 0);
            // 
            // 
            // 
            this.dateTimeInputEndDate.MonthCalendar.NavigationBackgroundStyle.BackColor2SchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground2;
            this.dateTimeInputEndDate.MonthCalendar.NavigationBackgroundStyle.BackColorGradientAngle = 90;
            this.dateTimeInputEndDate.MonthCalendar.NavigationBackgroundStyle.BackColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground;
            this.dateTimeInputEndDate.MonthCalendar.NavigationBackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.dateTimeInputEndDate.MonthCalendar.TodayButtonVisible = true;
            this.dateTimeInputEndDate.Name = "dateTimeInputEndDate";
            this.dateTimeInputEndDate.Size = new System.Drawing.Size(180, 27);
            this.dateTimeInputEndDate.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.dateTimeInputEndDate.TabIndex = 18;
            // 
            // dateTimeInputStartDate
            // 
            // 
            // 
            // 
            this.dateTimeInputStartDate.BackgroundStyle.Class = "DateTimeInputBackground";
            this.dateTimeInputStartDate.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.dateTimeInputStartDate.ButtonDropDown.Shortcut = DevComponents.DotNetBar.eShortcut.AltDown;
            this.dateTimeInputStartDate.ButtonDropDown.Visible = true;
            this.dateTimeInputStartDate.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dateTimeInputStartDate.IsPopupCalendarOpen = false;
            this.dateTimeInputStartDate.Location = new System.Drawing.Point(91, 13);
            // 
            // 
            // 
            // 
            // 
            // 
            this.dateTimeInputStartDate.MonthCalendar.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.dateTimeInputStartDate.MonthCalendar.CalendarDimensions = new System.Drawing.Size(1, 1);
            this.dateTimeInputStartDate.MonthCalendar.ClearButtonVisible = true;
            // 
            // 
            // 
            this.dateTimeInputStartDate.MonthCalendar.CommandsBackgroundStyle.BackColor2SchemePart = DevComponents.DotNetBar.eColorSchemePart.BarBackground2;
            this.dateTimeInputStartDate.MonthCalendar.CommandsBackgroundStyle.BackColorGradientAngle = 90;
            this.dateTimeInputStartDate.MonthCalendar.CommandsBackgroundStyle.BackColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.BarBackground;
            this.dateTimeInputStartDate.MonthCalendar.CommandsBackgroundStyle.BorderTop = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.dateTimeInputStartDate.MonthCalendar.CommandsBackgroundStyle.BorderTopColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.BarDockedBorder;
            this.dateTimeInputStartDate.MonthCalendar.CommandsBackgroundStyle.BorderTopWidth = 1;
            this.dateTimeInputStartDate.MonthCalendar.CommandsBackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.dateTimeInputStartDate.MonthCalendar.DisplayMonth = new System.DateTime(2019, 7, 1, 0, 0, 0, 0);
            // 
            // 
            // 
            this.dateTimeInputStartDate.MonthCalendar.NavigationBackgroundStyle.BackColor2SchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground2;
            this.dateTimeInputStartDate.MonthCalendar.NavigationBackgroundStyle.BackColorGradientAngle = 90;
            this.dateTimeInputStartDate.MonthCalendar.NavigationBackgroundStyle.BackColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground;
            this.dateTimeInputStartDate.MonthCalendar.NavigationBackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.dateTimeInputStartDate.MonthCalendar.TodayButtonVisible = true;
            this.dateTimeInputStartDate.Name = "dateTimeInputStartDate";
            this.dateTimeInputStartDate.Size = new System.Drawing.Size(180, 27);
            this.dateTimeInputStartDate.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.dateTimeInputStartDate.TabIndex = 17;
            this.dateTimeInputStartDate.ValueChanged += new System.EventHandler(this.DateTimeInputStartDate_ValueChanged);
            // 
            // textBoxXSearch
            // 
            // 
            // 
            // 
            this.textBoxXSearch.Border.Class = "TextBoxBorder";
            this.textBoxXSearch.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.textBoxXSearch.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxXSearch.Location = new System.Drawing.Point(356, 11);
            this.textBoxXSearch.Name = "textBoxXSearch";
            this.textBoxXSearch.PreventEnterBeep = true;
            this.textBoxXSearch.Size = new System.Drawing.Size(257, 27);
            this.textBoxXSearch.TabIndex = 6;
            this.textBoxXSearch.WatermarkFont = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxXSearch.WatermarkText = "Search String...";
            // 
            // buttonXFindPrev
            // 
            this.buttonXFindPrev.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.buttonXFindPrev.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.buttonXFindPrev.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonXFindPrev.Location = new System.Drawing.Point(634, 49);
            this.buttonXFindPrev.Name = "buttonXFindPrev";
            this.buttonXFindPrev.Size = new System.Drawing.Size(118, 34);
            this.buttonXFindPrev.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.buttonXFindPrev.TabIndex = 16;
            this.buttonXFindPrev.Text = "Find Prev";
            this.buttonXFindPrev.Click += new System.EventHandler(this.ButtonXFindPrev_Click);
            // 
            // buttonXExportXML
            // 
            this.buttonXExportXML.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.buttonXExportXML.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.buttonXExportXML.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonXExportXML.Location = new System.Drawing.Point(882, 9);
            this.buttonXExportXML.Name = "buttonXExportXML";
            this.buttonXExportXML.Size = new System.Drawing.Size(118, 34);
            this.buttonXExportXML.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.buttonXExportXML.TabIndex = 7;
            this.buttonXExportXML.Text = "Export Log";
            this.buttonXExportXML.Click += new System.EventHandler(this.ButtonXExportXML_Click);
            // 
            // buttonXFindFirst
            // 
            this.buttonXFindFirst.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.buttonXFindFirst.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.buttonXFindFirst.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonXFindFirst.Location = new System.Drawing.Point(634, 9);
            this.buttonXFindFirst.Name = "buttonXFindFirst";
            this.buttonXFindFirst.Size = new System.Drawing.Size(118, 34);
            this.buttonXFindFirst.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.buttonXFindFirst.TabIndex = 15;
            this.buttonXFindFirst.Text = "Find First";
            this.buttonXFindFirst.Click += new System.EventHandler(this.ButtonXFindFirst_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(11, 59);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(79, 19);
            this.label3.TabIndex = 9;
            this.label3.Text = "End Date:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // checkBoxXFindOcc
            // 
            this.checkBoxXFindOcc.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.checkBoxXFindOcc.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.checkBoxXFindOcc.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBoxXFindOcc.Location = new System.Drawing.Point(356, 49);
            this.checkBoxXFindOcc.Name = "checkBoxXFindOcc";
            this.checkBoxXFindOcc.Size = new System.Drawing.Size(175, 23);
            this.checkBoxXFindOcc.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.checkBoxXFindOcc.TabIndex = 14;
            this.checkBoxXFindOcc.Text = "Find Occurrences Only";
            this.checkBoxXFindOcc.CheckedChanged += new System.EventHandler(this.checkBoxXFindOcc_CheckedChanged);
            // 
            // buttonXFindNext
            // 
            this.buttonXFindNext.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.buttonXFindNext.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.buttonXFindNext.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonXFindNext.Location = new System.Drawing.Point(758, 49);
            this.buttonXFindNext.Name = "buttonXFindNext";
            this.buttonXFindNext.Size = new System.Drawing.Size(118, 34);
            this.buttonXFindNext.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.buttonXFindNext.TabIndex = 12;
            this.buttonXFindNext.Text = "Find Next";
            this.buttonXFindNext.Click += new System.EventHandler(this.ButtonXFindNext_Click);
            // 
            // buttonXFindLast
            // 
            this.buttonXFindLast.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.buttonXFindLast.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.buttonXFindLast.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonXFindLast.Location = new System.Drawing.Point(758, 9);
            this.buttonXFindLast.Name = "buttonXFindLast";
            this.buttonXFindLast.Size = new System.Drawing.Size(118, 34);
            this.buttonXFindLast.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.buttonXFindLast.TabIndex = 13;
            this.buttonXFindLast.Text = "Find Last";
            this.buttonXFindLast.Click += new System.EventHandler(this.ButtonXFindLast_Click);
            // 
            // contextMenuStripResults
            // 
            this.contextMenuStripResults.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemSearchForString});
            this.contextMenuStripResults.Name = "contextMenuStripResults";
            this.contextMenuStripResults.Size = new System.Drawing.Size(192, 26);
            this.contextMenuStripResults.Click += new System.EventHandler(this.contextMenuStripResults_Click);
            // 
            // toolStripMenuItemSearchForString
            // 
            this.toolStripMenuItemSearchForString.Name = "toolStripMenuItemSearchForString";
            this.toolStripMenuItemSearchForString.Size = new System.Drawing.Size(191, 22);
            this.toolStripMenuItemSearchForString.Text = "Search for this string...";
            // 
            // FormActivityLogViewer
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1024, 514);
            this.Controls.Add(this.panelSearchResults);
            this.Controls.Add(this.expandablePanelSearchControls);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "FormActivityLogViewer";
            this.Text = "Log Viewer";
            this.Load += new System.EventHandler(this.FormActivityLogViewer_Load);
            this.Leave += new System.EventHandler(this.FormActivityLogViewer_Leave);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewActivityHistory)).EndInit();
            this.panelSearchResults.ResumeLayout(false);
            this.expandablePanelSearchControls.ResumeLayout(false);
            this.panelControls.ResumeLayout(false);
            this.panelControls.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dateTimeInputEndDate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dateTimeInputStartDate)).EndInit();
            this.contextMenuStripResults.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.DataGridView dataGridViewActivityHistory;
        private DevComponents.DotNetBar.VScrollBarAdv vScrollBarAdv1;
        private System.Windows.Forms.Panel panelSearchResults;
        private DevComponents.DotNetBar.ExpandablePanel expandablePanelSearchControls;
        private System.Windows.Forms.Panel panelControls;
        private DevComponents.DotNetBar.ButtonX buttonXGo;
        private System.Windows.Forms.Label label1;
        private DevComponents.Editors.DateTimeAdv.DateTimeInput dateTimeInputEndDate;
        private DevComponents.Editors.DateTimeAdv.DateTimeInput dateTimeInputStartDate;
        private DevComponents.DotNetBar.Controls.TextBoxX textBoxXSearch;
        private DevComponents.DotNetBar.ButtonX buttonXFindPrev;
        private DevComponents.DotNetBar.ButtonX buttonXExportXML;
        private DevComponents.DotNetBar.ButtonX buttonXFindFirst;
        private System.Windows.Forms.Label label3;
        private DevComponents.DotNetBar.Controls.CheckBoxX checkBoxXFindOcc;
        private DevComponents.DotNetBar.ButtonX buttonXFindNext;
        private DevComponents.DotNetBar.ButtonX buttonXFindLast;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripResults;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemSearchForString;
    }
}
