namespace nAble
{
    partial class FormDispenseProfileEditor
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panelFreestyleProfile = new System.Windows.Forms.Panel();
            this.chartDispProfile = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.groupBoxRateChange = new System.Windows.Forms.GroupBox();
            this.buttonSaveRateChange = new System.Windows.Forms.Button();
            this.buttonFreestyleRate = new System.Windows.Forms.Button();
            this.buttonFreestylePos = new System.Windows.Forms.Button();
            this.label12 = new System.Windows.Forms.Label();
            this.buttonDeleteRateChange = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.buttonAddRateChange = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.labelFreestyleRowNo = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.dataGridViewFreestyleProfile = new System.Windows.Forms.DataGridView();
            this.colNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colXPos = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colRate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panelTrapezoidalProfile = new System.Windows.Forms.Panel();
            this.label8 = new System.Windows.Forms.Label();
            this.buttonTrapPeakRate = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.buttonTrapAccDecPct = new System.Windows.Forms.Button();
            this.label11 = new System.Windows.Forms.Label();
            this.panelTriangleProfile = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.buttonTrianglePeakRate = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.panelGradientProfile = new System.Windows.Forms.Panel();
            this.label7 = new System.Windows.Forms.Label();
            this.buttonGradientFinalRate = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.updateTimer = new System.Windows.Forms.Timer(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.labelBaseRate = new System.Windows.Forms.Label();
            this.panelFreestyleProfile.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartDispProfile)).BeginInit();
            this.groupBoxRateChange.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewFreestyleProfile)).BeginInit();
            this.panelTrapezoidalProfile.SuspendLayout();
            this.panelTriangleProfile.SuspendLayout();
            this.panelGradientProfile.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelFreestyleProfile
            // 
            this.panelFreestyleProfile.Controls.Add(this.chartDispProfile);
            this.panelFreestyleProfile.Controls.Add(this.groupBoxRateChange);
            this.panelFreestyleProfile.Controls.Add(this.labelFreestyleRowNo);
            this.panelFreestyleProfile.Controls.Add(this.label14);
            this.panelFreestyleProfile.Controls.Add(this.dataGridViewFreestyleProfile);
            this.panelFreestyleProfile.Location = new System.Drawing.Point(12, 12);
            this.panelFreestyleProfile.Name = "panelFreestyleProfile";
            this.panelFreestyleProfile.Size = new System.Drawing.Size(997, 423);
            this.panelFreestyleProfile.TabIndex = 9;
            this.panelFreestyleProfile.Visible = false;
            // 
            // chartDispProfile
            // 
            chartArea1.Name = "ChartArea1";
            this.chartDispProfile.ChartAreas.Add(chartArea1);
            legend1.Enabled = false;
            legend1.Name = "Legend1";
            this.chartDispProfile.Legends.Add(legend1);
            this.chartDispProfile.Location = new System.Drawing.Point(3, 187);
            this.chartDispProfile.Name = "chartDispProfile";
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series1.CustomProperties = "IsXAxisQuantitative=True";
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            series1.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
            series1.YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
            this.chartDispProfile.Series.Add(series1);
            this.chartDispProfile.Size = new System.Drawing.Size(540, 233);
            this.chartDispProfile.TabIndex = 11;
            this.chartDispProfile.Text = "chart1";
            // 
            // groupBoxRateChange
            // 
            this.groupBoxRateChange.Controls.Add(this.buttonSaveRateChange);
            this.groupBoxRateChange.Controls.Add(this.buttonFreestyleRate);
            this.groupBoxRateChange.Controls.Add(this.buttonFreestylePos);
            this.groupBoxRateChange.Controls.Add(this.label12);
            this.groupBoxRateChange.Controls.Add(this.buttonDeleteRateChange);
            this.groupBoxRateChange.Controls.Add(this.label4);
            this.groupBoxRateChange.Controls.Add(this.buttonAddRateChange);
            this.groupBoxRateChange.Controls.Add(this.label3);
            this.groupBoxRateChange.Controls.Add(this.label13);
            this.groupBoxRateChange.Location = new System.Drawing.Point(3, 3);
            this.groupBoxRateChange.Name = "groupBoxRateChange";
            this.groupBoxRateChange.Size = new System.Drawing.Size(540, 178);
            this.groupBoxRateChange.TabIndex = 10;
            this.groupBoxRateChange.TabStop = false;
            this.groupBoxRateChange.Text = "Rate Change No:";
            // 
            // buttonSaveRateChange
            // 
            this.buttonSaveRateChange.Enabled = false;
            this.buttonSaveRateChange.Location = new System.Drawing.Point(400, 74);
            this.buttonSaveRateChange.Margin = new System.Windows.Forms.Padding(6);
            this.buttonSaveRateChange.Name = "buttonSaveRateChange";
            this.buttonSaveRateChange.Size = new System.Drawing.Size(131, 44);
            this.buttonSaveRateChange.TabIndex = 11;
            this.buttonSaveRateChange.Text = "Save";
            this.buttonSaveRateChange.UseVisualStyleBackColor = true;
            this.buttonSaveRateChange.Click += new System.EventHandler(this.buttonSaveRateChange_Click);
            // 
            // buttonFreestyleRate
            // 
            this.buttonFreestyleRate.Location = new System.Drawing.Point(195, 41);
            this.buttonFreestyleRate.Name = "buttonFreestyleRate";
            this.buttonFreestyleRate.Size = new System.Drawing.Size(138, 44);
            this.buttonFreestyleRate.TabIndex = 4;
            this.buttonFreestyleRate.Text = "0.0";
            this.buttonFreestyleRate.UseVisualStyleBackColor = true;
            this.buttonFreestyleRate.Click += new System.EventHandler(this.buttonFreestyleRate_Click);
            // 
            // buttonFreestylePos
            // 
            this.buttonFreestylePos.Location = new System.Drawing.Point(195, 104);
            this.buttonFreestylePos.Name = "buttonFreestylePos";
            this.buttonFreestylePos.Size = new System.Drawing.Size(138, 44);
            this.buttonFreestylePos.TabIndex = 1;
            this.buttonFreestylePos.Text = "0.000";
            this.buttonFreestylePos.UseVisualStyleBackColor = true;
            this.buttonFreestylePos.Click += new System.EventHandler(this.buttonFreestylePos_Click);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(336, 110);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(61, 33);
            this.label12.TabIndex = 2;
            this.label12.Text = "mm";
            this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // buttonDeleteRateChange
            // 
            this.buttonDeleteRateChange.Enabled = false;
            this.buttonDeleteRateChange.Location = new System.Drawing.Point(400, 123);
            this.buttonDeleteRateChange.Margin = new System.Windows.Forms.Padding(6);
            this.buttonDeleteRateChange.Name = "buttonDeleteRateChange";
            this.buttonDeleteRateChange.Size = new System.Drawing.Size(131, 44);
            this.buttonDeleteRateChange.TabIndex = 7;
            this.buttonDeleteRateChange.Text = "Delete";
            this.buttonDeleteRateChange.UseVisualStyleBackColor = true;
            this.buttonDeleteRateChange.Click += new System.EventHandler(this.buttonDeleteRateChange_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 47);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(193, 33);
            this.label4.TabIndex = 3;
            this.label4.Text = "Dispense Rate:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // buttonAddRateChange
            // 
            this.buttonAddRateChange.Location = new System.Drawing.Point(400, 25);
            this.buttonAddRateChange.Margin = new System.Windows.Forms.Padding(6);
            this.buttonAddRateChange.Name = "buttonAddRateChange";
            this.buttonAddRateChange.Size = new System.Drawing.Size(131, 44);
            this.buttonAddRateChange.TabIndex = 6;
            this.buttonAddRateChange.Text = "Add";
            this.buttonAddRateChange.UseVisualStyleBackColor = true;
            this.buttonAddRateChange.Click += new System.EventHandler(this.buttonAddRateChange_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(336, 47);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(58, 33);
            this.label3.TabIndex = 5;
            this.label3.Text = "µl/s";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(55, 110);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(144, 33);
            this.label13.TabIndex = 0;
            this.label13.Text = "X-Position:";
            this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelFreestyleRowNo
            // 
            this.labelFreestyleRowNo.AutoSize = true;
            this.labelFreestyleRowNo.Location = new System.Drawing.Point(231, 111);
            this.labelFreestyleRowNo.Name = "labelFreestyleRowNo";
            this.labelFreestyleRowNo.Size = new System.Drawing.Size(30, 33);
            this.labelFreestyleRowNo.TabIndex = 9;
            this.labelFreestyleRowNo.Text = "0";
            this.labelFreestyleRowNo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(47, 111);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(217, 33);
            this.label14.TabIndex = 8;
            this.label14.Text = "Rate Change No:";
            this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // dataGridViewFreestyleProfile
            // 
            this.dataGridViewFreestyleProfile.AllowUserToAddRows = false;
            this.dataGridViewFreestyleProfile.AllowUserToDeleteRows = false;
            this.dataGridViewFreestyleProfile.AllowUserToResizeColumns = false;
            this.dataGridViewFreestyleProfile.AllowUserToResizeRows = false;
            this.dataGridViewFreestyleProfile.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Tahoma", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewFreestyleProfile.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridViewFreestyleProfile.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewFreestyleProfile.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colNo,
            this.colXPos,
            this.colRate});
            this.dataGridViewFreestyleProfile.Dock = System.Windows.Forms.DockStyle.Right;
            this.dataGridViewFreestyleProfile.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dataGridViewFreestyleProfile.Location = new System.Drawing.Point(549, 0);
            this.dataGridViewFreestyleProfile.MultiSelect = false;
            this.dataGridViewFreestyleProfile.Name = "dataGridViewFreestyleProfile";
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Tahoma", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewFreestyleProfile.RowHeadersDefaultCellStyle = dataGridViewCellStyle5;
            this.dataGridViewFreestyleProfile.RowHeadersVisible = false;
            this.dataGridViewFreestyleProfile.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dataGridViewFreestyleProfile.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewFreestyleProfile.Size = new System.Drawing.Size(448, 423);
            this.dataGridViewFreestyleProfile.TabIndex = 4;
            this.dataGridViewFreestyleProfile.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewFreestyleProfile_CellContentClick);
            // 
            // colNo
            // 
            this.colNo.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colNo.DefaultCellStyle = dataGridViewCellStyle2;
            this.colNo.HeaderText = "No";
            this.colNo.Name = "colNo";
            this.colNo.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colNo.Width = 50;
            // 
            // colXPos
            // 
            this.colXPos.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colXPos.DefaultCellStyle = dataGridViewCellStyle3;
            this.colXPos.FillWeight = 50F;
            this.colXPos.HeaderText = "X-Pos";
            this.colXPos.Name = "colXPos";
            this.colXPos.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // colRate
            // 
            this.colRate.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colRate.DefaultCellStyle = dataGridViewCellStyle4;
            this.colRate.FillWeight = 50F;
            this.colRate.HeaderText = "Rate";
            this.colRate.Name = "colRate";
            this.colRate.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // panelTrapezoidalProfile
            // 
            this.panelTrapezoidalProfile.Controls.Add(this.label8);
            this.panelTrapezoidalProfile.Controls.Add(this.buttonTrapPeakRate);
            this.panelTrapezoidalProfile.Controls.Add(this.label9);
            this.panelTrapezoidalProfile.Controls.Add(this.label10);
            this.panelTrapezoidalProfile.Controls.Add(this.buttonTrapAccDecPct);
            this.panelTrapezoidalProfile.Controls.Add(this.label11);
            this.panelTrapezoidalProfile.Location = new System.Drawing.Point(12, 12);
            this.panelTrapezoidalProfile.Name = "panelTrapezoidalProfile";
            this.panelTrapezoidalProfile.Size = new System.Drawing.Size(544, 430);
            this.panelTrapezoidalProfile.TabIndex = 10;
            this.panelTrapezoidalProfile.Visible = false;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(429, 18);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(58, 33);
            this.label8.TabIndex = 5;
            this.label8.Text = "µl/s";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // buttonTrapPeakRate
            // 
            this.buttonTrapPeakRate.Location = new System.Drawing.Point(273, 8);
            this.buttonTrapPeakRate.Name = "buttonTrapPeakRate";
            this.buttonTrapPeakRate.Size = new System.Drawing.Size(150, 44);
            this.buttonTrapPeakRate.TabIndex = 4;
            this.buttonTrapPeakRate.Text = "0.0";
            this.buttonTrapPeakRate.UseVisualStyleBackColor = true;
            this.buttonTrapPeakRate.Click += new System.EventHandler(this.buttonTrapPeakRate_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(10, 14);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(257, 33);
            this.label9.TabIndex = 3;
            this.label9.Text = "Peak Dispense Rate:";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(429, 68);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(41, 33);
            this.label10.TabIndex = 2;
            this.label10.Text = "%";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // buttonTrapAccDecPct
            // 
            this.buttonTrapAccDecPct.Location = new System.Drawing.Point(273, 58);
            this.buttonTrapAccDecPct.Name = "buttonTrapAccDecPct";
            this.buttonTrapAccDecPct.Size = new System.Drawing.Size(150, 44);
            this.buttonTrapAccDecPct.TabIndex = 1;
            this.buttonTrapAccDecPct.Text = "0";
            this.buttonTrapAccDecPct.UseVisualStyleBackColor = true;
            this.buttonTrapAccDecPct.Click += new System.EventHandler(this.buttonTrapAccDecPct_Click);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(25, 63);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(242, 33);
            this.label11.TabIndex = 0;
            this.label11.Text = "Accel/Decel Profile:";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // panelTriangleProfile
            // 
            this.panelTriangleProfile.Controls.Add(this.label5);
            this.panelTriangleProfile.Controls.Add(this.buttonTrianglePeakRate);
            this.panelTriangleProfile.Controls.Add(this.label6);
            this.panelTriangleProfile.Location = new System.Drawing.Point(12, 12);
            this.panelTriangleProfile.Name = "panelTriangleProfile";
            this.panelTriangleProfile.Size = new System.Drawing.Size(544, 430);
            this.panelTriangleProfile.TabIndex = 11;
            this.panelTriangleProfile.Visible = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(429, 18);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(58, 33);
            this.label5.TabIndex = 5;
            this.label5.Text = "µl/s";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // buttonTrianglePeakRate
            // 
            this.buttonTrianglePeakRate.Location = new System.Drawing.Point(273, 8);
            this.buttonTrianglePeakRate.Name = "buttonTrianglePeakRate";
            this.buttonTrianglePeakRate.Size = new System.Drawing.Size(150, 44);
            this.buttonTrianglePeakRate.TabIndex = 4;
            this.buttonTrianglePeakRate.Text = "0.0";
            this.buttonTrianglePeakRate.UseVisualStyleBackColor = true;
            this.buttonTrianglePeakRate.Click += new System.EventHandler(this.buttonTrianglePeakRate_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(10, 14);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(257, 33);
            this.label6.TabIndex = 3;
            this.label6.Text = "Peak Dispense Rate:";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // panelGradientProfile
            // 
            this.panelGradientProfile.Controls.Add(this.label7);
            this.panelGradientProfile.Controls.Add(this.buttonGradientFinalRate);
            this.panelGradientProfile.Controls.Add(this.label2);
            this.panelGradientProfile.Location = new System.Drawing.Point(12, 12);
            this.panelGradientProfile.Name = "panelGradientProfile";
            this.panelGradientProfile.Size = new System.Drawing.Size(544, 430);
            this.panelGradientProfile.TabIndex = 12;
            this.panelGradientProfile.Visible = false;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(429, 14);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(58, 33);
            this.label7.TabIndex = 6;
            this.label7.Text = "µl/s";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // buttonGradientFinalRate
            // 
            this.buttonGradientFinalRate.Location = new System.Drawing.Point(273, 8);
            this.buttonGradientFinalRate.Name = "buttonGradientFinalRate";
            this.buttonGradientFinalRate.Size = new System.Drawing.Size(150, 44);
            this.buttonGradientFinalRate.TabIndex = 1;
            this.buttonGradientFinalRate.Text = "0.0";
            this.buttonGradientFinalRate.UseVisualStyleBackColor = true;
            this.buttonGradientFinalRate.Click += new System.EventHandler(this.buttonGradientFinalRate_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 14);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(256, 33);
            this.label2.TabIndex = 0;
            this.label2.Text = "Final Dispense Rate:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(854, 442);
            this.buttonOK.Margin = new System.Windows.Forms.Padding(4);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(155, 57);
            this.buttonOK.TabIndex = 121;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(691, 442);
            this.buttonCancel.Margin = new System.Windows.Forms.Padding(4);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(155, 57);
            this.buttonCancel.TabIndex = 122;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // updateTimer
            // 
            this.updateTimer.Enabled = true;
            this.updateTimer.Tick += new System.EventHandler(this.updateTimer_Tick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 466);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(257, 33);
            this.label1.TabIndex = 123;
            this.label1.Text = "Base Dispense Rate:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(377, 466);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(58, 33);
            this.label15.TabIndex = 124;
            this.label15.Text = "µl/s";
            this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelBaseRate
            // 
            this.labelBaseRate.Location = new System.Drawing.Point(276, 466);
            this.labelBaseRate.Name = "labelBaseRate";
            this.labelBaseRate.Size = new System.Drawing.Size(99, 33);
            this.labelBaseRate.TabIndex = 125;
            this.labelBaseRate.Text = "0.00";
            this.labelBaseRate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // FormDispenseProfileEditor
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1022, 512);
            this.ControlBox = false;
            this.Controls.Add(this.labelBaseRate);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.panelFreestyleProfile);
            this.Controls.Add(this.panelGradientProfile);
            this.Controls.Add(this.panelTrapezoidalProfile);
            this.Controls.Add(this.panelTriangleProfile);
            this.Font = new System.Drawing.Font("Tahoma", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FormDispenseProfileEditor";
            this.panelFreestyleProfile.ResumeLayout(false);
            this.panelFreestyleProfile.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartDispProfile)).EndInit();
            this.groupBoxRateChange.ResumeLayout(false);
            this.groupBoxRateChange.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewFreestyleProfile)).EndInit();
            this.panelTrapezoidalProfile.ResumeLayout(false);
            this.panelTrapezoidalProfile.PerformLayout();
            this.panelTriangleProfile.ResumeLayout(false);
            this.panelTriangleProfile.PerformLayout();
            this.panelGradientProfile.ResumeLayout(false);
            this.panelGradientProfile.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panelFreestyleProfile;
        private System.Windows.Forms.Button buttonSaveRateChange;
        private System.Windows.Forms.GroupBox groupBoxRateChange;
        private System.Windows.Forms.Button buttonFreestyleRate;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Button buttonFreestylePos;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label labelFreestyleRowNo;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Button buttonDeleteRateChange;
        private System.Windows.Forms.Button buttonAddRateChange;
        private System.Windows.Forms.DataGridView dataGridViewFreestyleProfile;
        private System.Windows.Forms.Panel panelTrapezoidalProfile;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button buttonTrapPeakRate;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button buttonTrapAccDecPct;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Panel panelTriangleProfile;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button buttonTrianglePeakRate;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Panel panelGradientProfile;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button buttonGradientFinalRate;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartDispProfile;
        private System.Windows.Forms.Timer updateTimer;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colXPos;
        private System.Windows.Forms.DataGridViewTextBoxColumn colRate;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label labelBaseRate;
    }
}