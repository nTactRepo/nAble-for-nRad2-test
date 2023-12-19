namespace nAble
{
    partial class FormVacTrendCalendar
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
			this.buttonGotoPrevScreen = new System.Windows.Forms.Button();
			this.calendarViewPicker = new DevComponents.DotNetBar.Schedule.CalendarView();
			this.buttonXNextMonth = new DevComponents.DotNetBar.ButtonX();
			this.buttonXPrevMonth = new DevComponents.DotNetBar.ButtonX();
			this.panelExDateNav = new DevComponents.DotNetBar.PanelEx();
			this.listBoxAdvCharts = new DevComponents.DotNetBar.ListBoxAdv();
			this.panelExSelectedDay = new DevComponents.DotNetBar.PanelEx();
			this.buttonViewSelectedChart = new System.Windows.Forms.Button();
			this.panelExDateNav.SuspendLayout();
			this.SuspendLayout();
			// 
			// buttonGotoPrevScreen
			// 
			this.buttonGotoPrevScreen.BackColor = System.Drawing.SystemColors.Control;
			this.buttonGotoPrevScreen.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.buttonGotoPrevScreen.Location = new System.Drawing.Point(895, 414);
			this.buttonGotoPrevScreen.Name = "buttonGotoPrevScreen";
			this.buttonGotoPrevScreen.Size = new System.Drawing.Size(117, 90);
			this.buttonGotoPrevScreen.TabIndex = 37;
			this.buttonGotoPrevScreen.Text = "Go To Previous Screen";
			this.buttonGotoPrevScreen.UseVisualStyleBackColor = false;
			this.buttonGotoPrevScreen.Click += new System.EventHandler(this.buttonGotoPrevScreen_Click);
			// 
			// calendarViewPicker
			// 
			this.calendarViewPicker.AutoScrollMinSize = new System.Drawing.Size(0, 0);
			// 
			// 
			// 
			this.calendarViewPicker.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
			this.calendarViewPicker.ContainerControlProcessDialogKey = true;
			this.calendarViewPicker.EnableDragCopy = false;
			this.calendarViewPicker.EnableDragDrop = false;
			this.calendarViewPicker.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.calendarViewPicker.HighlightCurrentDay = true;
			this.calendarViewPicker.LicenseKey = "F962CEC7-CD8F-4911-A9E9-CAB39962FC1F";
			this.calendarViewPicker.Location = new System.Drawing.Point(12, 62);
			this.calendarViewPicker.MultiUserTabHeight = 26;
			this.calendarViewPicker.Name = "calendarViewPicker";
			this.calendarViewPicker.SelectedView = DevComponents.DotNetBar.Schedule.eCalendarView.Month;
			this.calendarViewPicker.Size = new System.Drawing.Size(678, 446);
			this.calendarViewPicker.TabIndex = 51;
			this.calendarViewPicker.Text = "calendarViewPicker";
			this.calendarViewPicker.TimeIndicator.BorderColor = System.Drawing.Color.Empty;
			this.calendarViewPicker.TimeIndicator.Tag = null;
			this.calendarViewPicker.TimeSlotDuration = 30;
			this.calendarViewPicker.MonthViewStartDateChanged += new System.EventHandler<DevComponents.DotNetBar.Schedule.DateChangeEventArgs>(this.calendarViewPicker_MonthViewStartDateChanged);
			this.calendarViewPicker.ItemClick += new System.EventHandler(this.calendarViewPicker_ItemClick);
			// 
			// buttonXNextMonth
			// 
			this.buttonXNextMonth.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
			this.buttonXNextMonth.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonXNextMonth.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
			this.buttonXNextMonth.Location = new System.Drawing.Point(600, 2);
			this.buttonXNextMonth.Name = "buttonXNextMonth";
			this.buttonXNextMonth.Size = new System.Drawing.Size(74, 40);
			this.buttonXNextMonth.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
			this.buttonXNextMonth.TabIndex = 3;
			this.buttonXNextMonth.Text = "Next";
			this.buttonXNextMonth.Click += new System.EventHandler(this.buttonXNextMonth_Click);
			// 
			// buttonXPrevMonth
			// 
			this.buttonXPrevMonth.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
			this.buttonXPrevMonth.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
			this.buttonXPrevMonth.Location = new System.Drawing.Point(3, 2);
			this.buttonXPrevMonth.Name = "buttonXPrevMonth";
			this.buttonXPrevMonth.Size = new System.Drawing.Size(74, 40);
			this.buttonXPrevMonth.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
			this.buttonXPrevMonth.TabIndex = 2;
			this.buttonXPrevMonth.Text = "Prev";
			this.buttonXPrevMonth.Click += new System.EventHandler(this.buttonXPrevMonth_Click);
			// 
			// panelExDateNav
			// 
			this.panelExDateNav.CanvasColor = System.Drawing.SystemColors.Control;
			this.panelExDateNav.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
			this.panelExDateNav.Controls.Add(this.buttonXPrevMonth);
			this.panelExDateNav.Controls.Add(this.buttonXNextMonth);
			this.panelExDateNav.DisabledBackColor = System.Drawing.Color.Empty;
			this.panelExDateNav.Location = new System.Drawing.Point(13, 13);
			this.panelExDateNav.Name = "panelExDateNav";
			this.panelExDateNav.Size = new System.Drawing.Size(677, 44);
			this.panelExDateNav.Style.Alignment = System.Drawing.StringAlignment.Center;
			this.panelExDateNav.Style.BackColor1.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground;
			this.panelExDateNav.Style.BackColor2.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground2;
			this.panelExDateNav.Style.Border = DevComponents.DotNetBar.eBorderType.SingleLine;
			this.panelExDateNav.Style.BorderColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder;
			this.panelExDateNav.Style.ForeColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText;
			this.panelExDateNav.Style.GradientAngle = 90;
			this.panelExDateNav.TabIndex = 55;
			this.panelExDateNav.Text = "January 2019";
			// 
			// listBoxAdvCharts
			// 
			this.listBoxAdvCharts.AutoScroll = true;
			// 
			// 
			// 
			this.listBoxAdvCharts.BackgroundStyle.Class = "ListBoxAdv";
			this.listBoxAdvCharts.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
			this.listBoxAdvCharts.ContainerControlProcessDialogKey = true;
			this.listBoxAdvCharts.DragDropSupport = true;
			this.listBoxAdvCharts.Font = new System.Drawing.Font("Tahoma", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.listBoxAdvCharts.ItemHeight = 25;
			this.listBoxAdvCharts.ItemSpacing = 5;
			this.listBoxAdvCharts.LicenseKey = "F962CEC7-CD8F-4911-A9E9-CAB39962FC1F";
			this.listBoxAdvCharts.Location = new System.Drawing.Point(772, 65);
			this.listBoxAdvCharts.Name = "listBoxAdvCharts";
			this.listBoxAdvCharts.Size = new System.Drawing.Size(240, 343);
			this.listBoxAdvCharts.Style = DevComponents.DotNetBar.eDotNetBarStyle.Office2013;
			this.listBoxAdvCharts.TabIndex = 59;
			this.listBoxAdvCharts.Text = "Select Date from Calendar";
			this.listBoxAdvCharts.ValueMember = "Value";
			this.listBoxAdvCharts.ItemDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listBoxAdvCharts_ItemDoubleClick);
			// 
			// panelExSelectedDay
			// 
			this.panelExSelectedDay.CanvasColor = System.Drawing.SystemColors.Control;
			this.panelExSelectedDay.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
			this.panelExSelectedDay.DisabledBackColor = System.Drawing.Color.Empty;
			this.panelExSelectedDay.Font = new System.Drawing.Font("Tahoma", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.panelExSelectedDay.Location = new System.Drawing.Point(772, 13);
			this.panelExSelectedDay.Name = "panelExSelectedDay";
			this.panelExSelectedDay.Size = new System.Drawing.Size(240, 44);
			this.panelExSelectedDay.Style.Alignment = System.Drawing.StringAlignment.Center;
			this.panelExSelectedDay.Style.BackColor1.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground;
			this.panelExSelectedDay.Style.BackColor2.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground2;
			this.panelExSelectedDay.Style.Border = DevComponents.DotNetBar.eBorderType.SingleLine;
			this.panelExSelectedDay.Style.BorderColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder;
			this.panelExSelectedDay.Style.ForeColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText;
			this.panelExSelectedDay.Style.GradientAngle = 90;
			this.panelExSelectedDay.TabIndex = 66;
			this.panelExSelectedDay.Text = "September 30, 2019";
			// 
			// buttonViewSelectedChart
			// 
			this.buttonViewSelectedChart.BackColor = System.Drawing.SystemColors.Control;
			this.buttonViewSelectedChart.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.buttonViewSelectedChart.Location = new System.Drawing.Point(772, 414);
			this.buttonViewSelectedChart.Name = "buttonViewSelectedChart";
			this.buttonViewSelectedChart.Size = new System.Drawing.Size(117, 90);
			this.buttonViewSelectedChart.TabIndex = 73;
			this.buttonViewSelectedChart.Text = "View Selected Chart";
			this.buttonViewSelectedChart.UseVisualStyleBackColor = false;
			this.buttonViewSelectedChart.Click += new System.EventHandler(this.buttonViewSelectedChart_Click);
			// 
			// FormVacTrendCalendar
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.BackColor = System.Drawing.Color.SteelBlue;
			this.ClientSize = new System.Drawing.Size(1024, 514);
			this.Controls.Add(this.buttonViewSelectedChart);
			this.Controls.Add(this.panelExSelectedDay);
			this.Controls.Add(this.listBoxAdvCharts);
			this.Controls.Add(this.panelExDateNav);
			this.Controls.Add(this.buttonGotoPrevScreen);
			this.Controls.Add(this.calendarViewPicker);
			this.Font = new System.Drawing.Font("Tahoma", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Margin = new System.Windows.Forms.Padding(5);
			this.Name = "FormVacTrendCalendar";
			this.Text = "Vac Trend Calendar";
			this.panelExDateNav.ResumeLayout(false);
			this.ResumeLayout(false);

        }

		#endregion

		private System.Windows.Forms.Button buttonGotoPrevScreen;
		private DevComponents.DotNetBar.Schedule.CalendarView calendarViewPicker;
		private DevComponents.DotNetBar.ButtonX buttonXPrevMonth;
		private DevComponents.DotNetBar.ButtonX buttonXNextMonth;
		private DevComponents.DotNetBar.PanelEx panelExDateNav;
		private DevComponents.DotNetBar.ListBoxAdv listBoxAdvCharts;
		private DevComponents.DotNetBar.PanelEx panelExSelectedDay;
		private System.Windows.Forms.Button buttonViewSelectedChart;
	}
}