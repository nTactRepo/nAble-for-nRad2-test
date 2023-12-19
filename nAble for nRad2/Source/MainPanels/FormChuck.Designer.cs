namespace nAble
{
    partial class FormChuck
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
            System.Windows.Forms.Label labelTempChuckSetPointUnits;
            this.panelChuckTempController = new System.Windows.Forms.Panel();
            this.labelChuckSetPointChanges = new System.Windows.Forms.Label();
            this.buttonChuckSetPointApply = new System.Windows.Forms.Button();
            this.buttonChuckTempSetPoint = new System.Windows.Forms.Button();
            this.labelChuckTempSetPoint = new System.Windows.Forms.Label();
            this.labelChuckTempController = new System.Windows.Forms.Label();
            this.labelChuckTempOutput = new System.Windows.Forms.Label();
            this.panelMainVac = new System.Windows.Forms.Panel();
            this.labelMainVacStatus = new System.Windows.Forms.Label();
            this.labelMainVac = new System.Windows.Forms.Label();
            this.panelZone1Status = new System.Windows.Forms.Panel();
            this.labelZone1 = new System.Windows.Forms.Label();
            this.buttonZone1Vacuum = new System.Windows.Forms.Button();
            this.panelZone2Status = new System.Windows.Forms.Panel();
            this.labelZone2 = new System.Windows.Forms.Label();
            this.buttonZone2Vacuum = new System.Windows.Forms.Button();
            this.panelPrimingStatuspanelPrimingStatus = new System.Windows.Forms.Panel();
            this.labelPrimingZone = new System.Windows.Forms.Label();
            this.buttonPrimingVacuum = new System.Windows.Forms.Button();
            this.panelZone3Status = new System.Windows.Forms.Panel();
            this.labelZone3 = new System.Windows.Forms.Label();
            this.buttonZone3Vacuum = new System.Windows.Forms.Button();
            this.buttonPrepareToLoad = new System.Windows.Forms.Button();
            this.buttonLoad = new System.Windows.Forms.Button();
            this.buttonUnload = new System.Windows.Forms.Button();
            this.buttonLiftPins = new System.Windows.Forms.Button();
            this.buttonAligners = new System.Windows.Forms.Button();
            this.groupBoxSelectiveZones = new System.Windows.Forms.GroupBox();
            this.checkBoxSelectiveZone3 = new System.Windows.Forms.CheckBox();
            this.checkBoxSelectiveZone2 = new System.Windows.Forms.CheckBox();
            this.checkBoxSelectiveZone1 = new System.Windows.Forms.CheckBox();
            this.labelLeftLaser = new System.Windows.Forms.Label();
            this.labelRightLaser = new System.Windows.Forms.Label();
            this.panelKeyenceLasers = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.labelCurProgram = new System.Windows.Forms.Label();
            this.tabControlLoadUnload = new System.Windows.Forms.TabControl();
            this.tabPageCoatingChuck = new System.Windows.Forms.TabPage();
            this.tabPagePrimingChuck = new System.Windows.Forms.TabPage();
            this.buttonPrepareToLoadPriming = new System.Windows.Forms.Button();
            this.buttonLoadPriming = new System.Windows.Forms.Button();
            this.buttonUnloadPriming = new System.Windows.Forms.Button();
            this.buttonLiftPinsPriming = new System.Windows.Forms.Button();
            labelTempChuckSetPointUnits = new System.Windows.Forms.Label();
            this.panelChuckTempController.SuspendLayout();
            this.panelMainVac.SuspendLayout();
            this.panelZone1Status.SuspendLayout();
            this.panelZone2Status.SuspendLayout();
            this.panelPrimingStatuspanelPrimingStatus.SuspendLayout();
            this.panelZone3Status.SuspendLayout();
            this.groupBoxSelectiveZones.SuspendLayout();
            this.panelKeyenceLasers.SuspendLayout();
            this.tabControlLoadUnload.SuspendLayout();
            this.tabPageCoatingChuck.SuspendLayout();
            this.tabPagePrimingChuck.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelTempChuckSetPointUnits
            // 
            labelTempChuckSetPointUnits.AutoSize = true;
            labelTempChuckSetPointUnits.Location = new System.Drawing.Point(515, 43);
            labelTempChuckSetPointUnits.Name = "labelTempChuckSetPointUnits";
            labelTempChuckSetPointUnits.Size = new System.Drawing.Size(44, 33);
            labelTempChuckSetPointUnits.TabIndex = 103;
            labelTempChuckSetPointUnits.Text = "°C";
            // 
            // panelChuckTempController
            // 
            this.panelChuckTempController.BackColor = System.Drawing.SystemColors.ControlDark;
            this.panelChuckTempController.Controls.Add(this.labelChuckSetPointChanges);
            this.panelChuckTempController.Controls.Add(this.buttonChuckSetPointApply);
            this.panelChuckTempController.Controls.Add(this.buttonChuckTempSetPoint);
            this.panelChuckTempController.Controls.Add(labelTempChuckSetPointUnits);
            this.panelChuckTempController.Controls.Add(this.labelChuckTempSetPoint);
            this.panelChuckTempController.Controls.Add(this.labelChuckTempController);
            this.panelChuckTempController.Controls.Add(this.labelChuckTempOutput);
            this.panelChuckTempController.Location = new System.Drawing.Point(138, 338);
            this.panelChuckTempController.Name = "panelChuckTempController";
            this.panelChuckTempController.Size = new System.Drawing.Size(778, 91);
            this.panelChuckTempController.TabIndex = 8;
            this.panelChuckTempController.Visible = false;
            // 
            // labelChuckSetPointChanges
            // 
            this.labelChuckSetPointChanges.ForeColor = System.Drawing.Color.Yellow;
            this.labelChuckSetPointChanges.Location = new System.Drawing.Point(215, 4);
            this.labelChuckSetPointChanges.Name = "labelChuckSetPointChanges";
            this.labelChuckSetPointChanges.Size = new System.Drawing.Size(409, 33);
            this.labelChuckSetPointChanges.TabIndex = 109;
            this.labelChuckSetPointChanges.Text = "Changes have not been applied.";
            this.labelChuckSetPointChanges.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.labelChuckSetPointChanges.Visible = false;
            // 
            // buttonChuckSetPointApply
            // 
            this.buttonChuckSetPointApply.Location = new System.Drawing.Point(639, 12);
            this.buttonChuckSetPointApply.Name = "buttonChuckSetPointApply";
            this.buttonChuckSetPointApply.Size = new System.Drawing.Size(114, 66);
            this.buttonChuckSetPointApply.TabIndex = 108;
            this.buttonChuckSetPointApply.Text = "Apply";
            this.buttonChuckSetPointApply.UseVisualStyleBackColor = true;
            this.buttonChuckSetPointApply.Click += new System.EventHandler(this.buttonChuckSetPointApply_Click);
            // 
            // buttonChuckTempSetPoint
            // 
            this.buttonChuckTempSetPoint.Location = new System.Drawing.Point(426, 39);
            this.buttonChuckTempSetPoint.Name = "buttonChuckTempSetPoint";
            this.buttonChuckTempSetPoint.Size = new System.Drawing.Size(88, 40);
            this.buttonChuckTempSetPoint.TabIndex = 104;
            this.buttonChuckTempSetPoint.Text = "50.0";
            this.buttonChuckTempSetPoint.UseVisualStyleBackColor = true;
            this.buttonChuckTempSetPoint.Click += new System.EventHandler(this.buttonChuckTempSetPoint_Click);
            // 
            // labelChuckTempSetPoint
            // 
            this.labelChuckTempSetPoint.Location = new System.Drawing.Point(287, 43);
            this.labelChuckTempSetPoint.Name = "labelChuckTempSetPoint";
            this.labelChuckTempSetPoint.Size = new System.Drawing.Size(134, 33);
            this.labelChuckTempSetPoint.TabIndex = 2;
            this.labelChuckTempSetPoint.Text = "Set Point:";
            this.labelChuckTempSetPoint.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelChuckTempController
            // 
            this.labelChuckTempController.BackColor = System.Drawing.Color.Black;
            this.labelChuckTempController.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelChuckTempController.ForeColor = System.Drawing.Color.Lime;
            this.labelChuckTempController.Location = new System.Drawing.Point(24, 12);
            this.labelChuckTempController.Name = "labelChuckTempController";
            this.labelChuckTempController.Size = new System.Drawing.Size(173, 29);
            this.labelChuckTempController.TabIndex = 1;
            this.labelChuckTempController.Text = "Chuck Temp";
            this.labelChuckTempController.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelChuckTempOutput
            // 
            this.labelChuckTempOutput.BackColor = System.Drawing.Color.Black;
            this.labelChuckTempOutput.Font = new System.Drawing.Font("Microsoft Sans Serif", 27.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelChuckTempOutput.ForeColor = System.Drawing.Color.Lime;
            this.labelChuckTempOutput.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.labelChuckTempOutput.Location = new System.Drawing.Point(24, 37);
            this.labelChuckTempOutput.Name = "labelChuckTempOutput";
            this.labelChuckTempOutput.Size = new System.Drawing.Size(173, 42);
            this.labelChuckTempOutput.TabIndex = 0;
            this.labelChuckTempOutput.Text = "21° C";
            this.labelChuckTempOutput.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panelMainVac
            // 
            this.panelMainVac.BackColor = System.Drawing.SystemColors.ControlDark;
            this.panelMainVac.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelMainVac.Controls.Add(this.labelMainVacStatus);
            this.panelMainVac.Controls.Add(this.labelMainVac);
            this.panelMainVac.Location = new System.Drawing.Point(138, 143);
            this.panelMainVac.Name = "panelMainVac";
            this.panelMainVac.Size = new System.Drawing.Size(150, 190);
            this.panelMainVac.TabIndex = 11;
            // 
            // labelMainVacStatus
            // 
            this.labelMainVacStatus.BackColor = System.Drawing.Color.Red;
            this.labelMainVacStatus.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelMainVacStatus.Location = new System.Drawing.Point(3, 70);
            this.labelMainVacStatus.Name = "labelMainVacStatus";
            this.labelMainVacStatus.Size = new System.Drawing.Size(142, 109);
            this.labelMainVacStatus.TabIndex = 2;
            this.labelMainVacStatus.Text = "Vacuum\r\nNOT\r\nDetected";
            this.labelMainVacStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelMainVac
            // 
            this.labelMainVac.Font = new System.Drawing.Font("Tahoma", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelMainVac.Location = new System.Drawing.Point(12, 3);
            this.labelMainVac.Name = "labelMainVac";
            this.labelMainVac.Size = new System.Drawing.Size(124, 64);
            this.labelMainVac.TabIndex = 1;
            this.labelMainVac.Text = "Main Vac.\r\nStatus";
            this.labelMainVac.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panelZone1Status
            // 
            this.panelZone1Status.BackColor = System.Drawing.SystemColors.ControlDark;
            this.panelZone1Status.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelZone1Status.Controls.Add(this.labelZone1);
            this.panelZone1Status.Controls.Add(this.buttonZone1Vacuum);
            this.panelZone1Status.Location = new System.Drawing.Point(452, 143);
            this.panelZone1Status.Name = "panelZone1Status";
            this.panelZone1Status.Size = new System.Drawing.Size(150, 190);
            this.panelZone1Status.TabIndex = 12;
            // 
            // labelZone1
            // 
            this.labelZone1.Font = new System.Drawing.Font("Tahoma", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelZone1.Location = new System.Drawing.Point(12, 3);
            this.labelZone1.Name = "labelZone1";
            this.labelZone1.Size = new System.Drawing.Size(124, 64);
            this.labelZone1.TabIndex = 2;
            this.labelZone1.Text = "Zone 1\r\nInterlock";
            this.labelZone1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // buttonZone1Vacuum
            // 
            this.buttonZone1Vacuum.BackColor = System.Drawing.SystemColors.Control;
            this.buttonZone1Vacuum.Location = new System.Drawing.Point(3, 70);
            this.buttonZone1Vacuum.Name = "buttonZone1Vacuum";
            this.buttonZone1Vacuum.Size = new System.Drawing.Size(142, 109);
            this.buttonZone1Vacuum.TabIndex = 1;
            this.buttonZone1Vacuum.Text = "Vacuum\r\nOFF";
            this.buttonZone1Vacuum.UseVisualStyleBackColor = true;
            this.buttonZone1Vacuum.Click += new System.EventHandler(this.buttonZone1Vacuum_Click);
            // 
            // panelZone2Status
            // 
            this.panelZone2Status.BackColor = System.Drawing.SystemColors.ControlDark;
            this.panelZone2Status.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelZone2Status.Controls.Add(this.labelZone2);
            this.panelZone2Status.Controls.Add(this.buttonZone2Vacuum);
            this.panelZone2Status.Enabled = false;
            this.panelZone2Status.Location = new System.Drawing.Point(609, 143);
            this.panelZone2Status.Name = "panelZone2Status";
            this.panelZone2Status.Size = new System.Drawing.Size(150, 190);
            this.panelZone2Status.TabIndex = 10;
            // 
            // labelZone2
            // 
            this.labelZone2.Font = new System.Drawing.Font("Tahoma", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelZone2.Location = new System.Drawing.Point(12, 3);
            this.labelZone2.Name = "labelZone2";
            this.labelZone2.Size = new System.Drawing.Size(124, 64);
            this.labelZone2.TabIndex = 2;
            this.labelZone2.Text = "Zone 2\r\nInterlock";
            this.labelZone2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // buttonZone2Vacuum
            // 
            this.buttonZone2Vacuum.BackColor = System.Drawing.SystemColors.Control;
            this.buttonZone2Vacuum.Location = new System.Drawing.Point(3, 70);
            this.buttonZone2Vacuum.Name = "buttonZone2Vacuum";
            this.buttonZone2Vacuum.Size = new System.Drawing.Size(142, 109);
            this.buttonZone2Vacuum.TabIndex = 1;
            this.buttonZone2Vacuum.Text = "Vacuum\r\nOFF";
            this.buttonZone2Vacuum.UseVisualStyleBackColor = true;
            this.buttonZone2Vacuum.Click += new System.EventHandler(this.buttonZone2Vacuum_Click);
            // 
            // panelPrimingStatuspanelPrimingStatus
            // 
            this.panelPrimingStatuspanelPrimingStatus.BackColor = System.Drawing.SystemColors.ControlDark;
            this.panelPrimingStatuspanelPrimingStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelPrimingStatuspanelPrimingStatus.Controls.Add(this.labelPrimingZone);
            this.panelPrimingStatuspanelPrimingStatus.Controls.Add(this.buttonPrimingVacuum);
            this.panelPrimingStatuspanelPrimingStatus.Location = new System.Drawing.Point(295, 143);
            this.panelPrimingStatuspanelPrimingStatus.Name = "panelPrimingStatuspanelPrimingStatus";
            this.panelPrimingStatuspanelPrimingStatus.Size = new System.Drawing.Size(150, 190);
            this.panelPrimingStatuspanelPrimingStatus.TabIndex = 9;
            // 
            // labelPrimingZone
            // 
            this.labelPrimingZone.Font = new System.Drawing.Font("Tahoma", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelPrimingZone.Location = new System.Drawing.Point(12, 3);
            this.labelPrimingZone.Name = "labelPrimingZone";
            this.labelPrimingZone.Size = new System.Drawing.Size(124, 64);
            this.labelPrimingZone.TabIndex = 1;
            this.labelPrimingZone.Text = "Priming\r\nInterlock";
            this.labelPrimingZone.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // buttonPrimingVacuum
            // 
            this.buttonPrimingVacuum.BackColor = System.Drawing.SystemColors.Control;
            this.buttonPrimingVacuum.Location = new System.Drawing.Point(3, 70);
            this.buttonPrimingVacuum.Name = "buttonPrimingVacuum";
            this.buttonPrimingVacuum.Size = new System.Drawing.Size(142, 109);
            this.buttonPrimingVacuum.TabIndex = 0;
            this.buttonPrimingVacuum.Text = "Vacuum\r\nOFF";
            this.buttonPrimingVacuum.UseVisualStyleBackColor = true;
            this.buttonPrimingVacuum.Click += new System.EventHandler(this.buttonPrimingVacuum_Click);
            // 
            // panelZone3Status
            // 
            this.panelZone3Status.BackColor = System.Drawing.SystemColors.ControlDark;
            this.panelZone3Status.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelZone3Status.Controls.Add(this.labelZone3);
            this.panelZone3Status.Controls.Add(this.buttonZone3Vacuum);
            this.panelZone3Status.Enabled = false;
            this.panelZone3Status.Location = new System.Drawing.Point(766, 143);
            this.panelZone3Status.Name = "panelZone3Status";
            this.panelZone3Status.Size = new System.Drawing.Size(150, 190);
            this.panelZone3Status.TabIndex = 13;
            // 
            // labelZone3
            // 
            this.labelZone3.Font = new System.Drawing.Font("Tahoma", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelZone3.Location = new System.Drawing.Point(12, 3);
            this.labelZone3.Name = "labelZone3";
            this.labelZone3.Size = new System.Drawing.Size(124, 64);
            this.labelZone3.TabIndex = 2;
            this.labelZone3.Text = "Zone 3\r\nInterlock";
            this.labelZone3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // buttonZone3Vacuum
            // 
            this.buttonZone3Vacuum.BackColor = System.Drawing.SystemColors.Control;
            this.buttonZone3Vacuum.Location = new System.Drawing.Point(3, 70);
            this.buttonZone3Vacuum.Name = "buttonZone3Vacuum";
            this.buttonZone3Vacuum.Size = new System.Drawing.Size(142, 109);
            this.buttonZone3Vacuum.TabIndex = 1;
            this.buttonZone3Vacuum.Text = "Vacuum\r\nOFF";
            this.buttonZone3Vacuum.UseVisualStyleBackColor = true;
            this.buttonZone3Vacuum.Click += new System.EventHandler(this.buttonZone3Vacuum_Click);
            // 
            // buttonPrepareToLoad
            // 
            this.buttonPrepareToLoad.BackColor = System.Drawing.SystemColors.Control;
            this.buttonPrepareToLoad.Location = new System.Drawing.Point(3, 3);
            this.buttonPrepareToLoad.Name = "buttonPrepareToLoad";
            this.buttonPrepareToLoad.Size = new System.Drawing.Size(150, 74);
            this.buttonPrepareToLoad.TabIndex = 14;
            this.buttonPrepareToLoad.Text = "Prepare To Load";
            this.buttonPrepareToLoad.UseVisualStyleBackColor = true;
            this.buttonPrepareToLoad.Click += new System.EventHandler(this.buttonPrepareToLoad_Click);
            // 
            // buttonLoad
            // 
            this.buttonLoad.BackColor = System.Drawing.SystemColors.Control;
            this.buttonLoad.Location = new System.Drawing.Point(156, 3);
            this.buttonLoad.Name = "buttonLoad";
            this.buttonLoad.Size = new System.Drawing.Size(150, 74);
            this.buttonLoad.TabIndex = 15;
            this.buttonLoad.Text = "Load";
            this.buttonLoad.UseVisualStyleBackColor = true;
            this.buttonLoad.Click += new System.EventHandler(this.buttonLoad_Click);
            // 
            // buttonUnload
            // 
            this.buttonUnload.BackColor = System.Drawing.SystemColors.Control;
            this.buttonUnload.Location = new System.Drawing.Point(309, 3);
            this.buttonUnload.Name = "buttonUnload";
            this.buttonUnload.Size = new System.Drawing.Size(150, 74);
            this.buttonUnload.TabIndex = 16;
            this.buttonUnload.Text = "Unload";
            this.buttonUnload.UseVisualStyleBackColor = true;
            this.buttonUnload.Click += new System.EventHandler(this.buttonUnload_Click);
            // 
            // buttonLiftPins
            // 
            this.buttonLiftPins.BackColor = System.Drawing.SystemColors.Control;
            this.buttonLiftPins.Location = new System.Drawing.Point(462, 3);
            this.buttonLiftPins.Name = "buttonLiftPins";
            this.buttonLiftPins.Size = new System.Drawing.Size(150, 74);
            this.buttonLiftPins.TabIndex = 17;
            this.buttonLiftPins.Text = "Raise\r\nPins";
            this.buttonLiftPins.UseVisualStyleBackColor = true;
            this.buttonLiftPins.Click += new System.EventHandler(this.buttonLiftPins_Click);
            // 
            // buttonAligners
            // 
            this.buttonAligners.BackColor = System.Drawing.SystemColors.Control;
            this.buttonAligners.Location = new System.Drawing.Point(615, 3);
            this.buttonAligners.Name = "buttonAligners";
            this.buttonAligners.Size = new System.Drawing.Size(150, 74);
            this.buttonAligners.TabIndex = 18;
            this.buttonAligners.Text = "Raise Aligners";
            this.buttonAligners.UseVisualStyleBackColor = true;
            this.buttonAligners.Click += new System.EventHandler(this.buttonAligners_Click);
            // 
            // groupBoxSelectiveZones
            // 
            this.groupBoxSelectiveZones.Controls.Add(this.checkBoxSelectiveZone3);
            this.groupBoxSelectiveZones.Controls.Add(this.checkBoxSelectiveZone2);
            this.groupBoxSelectiveZones.Controls.Add(this.checkBoxSelectiveZone1);
            this.groupBoxSelectiveZones.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBoxSelectiveZones.ForeColor = System.Drawing.Color.White;
            this.groupBoxSelectiveZones.Location = new System.Drawing.Point(23, 132);
            this.groupBoxSelectiveZones.Name = "groupBoxSelectiveZones";
            this.groupBoxSelectiveZones.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.groupBoxSelectiveZones.Size = new System.Drawing.Size(109, 216);
            this.groupBoxSelectiveZones.TabIndex = 35;
            this.groupBoxSelectiveZones.TabStop = false;
            this.groupBoxSelectiveZones.Text = "Selective Zones";
            // 
            // checkBoxSelectiveZone3
            // 
            this.checkBoxSelectiveZone3.AutoSize = true;
            this.checkBoxSelectiveZone3.BackColor = System.Drawing.Color.Gray;
            this.checkBoxSelectiveZone3.CheckAlign = System.Drawing.ContentAlignment.TopCenter;
            this.checkBoxSelectiveZone3.Location = new System.Drawing.Point(18, 167);
            this.checkBoxSelectiveZone3.Name = "checkBoxSelectiveZone3";
            this.checkBoxSelectiveZone3.Size = new System.Drawing.Size(72, 41);
            this.checkBoxSelectiveZone3.TabIndex = 2;
            this.checkBoxSelectiveZone3.Text = "Zone 3";
            this.checkBoxSelectiveZone3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBoxSelectiveZone3.UseVisualStyleBackColor = false;
            this.checkBoxSelectiveZone3.CheckedChanged += new System.EventHandler(this.checkBoxSelectiveZone_CheckedChanged);
            // 
            // checkBoxSelectiveZone2
            // 
            this.checkBoxSelectiveZone2.AutoSize = true;
            this.checkBoxSelectiveZone2.BackColor = System.Drawing.Color.Gray;
            this.checkBoxSelectiveZone2.CheckAlign = System.Drawing.ContentAlignment.TopCenter;
            this.checkBoxSelectiveZone2.Location = new System.Drawing.Point(18, 111);
            this.checkBoxSelectiveZone2.Name = "checkBoxSelectiveZone2";
            this.checkBoxSelectiveZone2.Size = new System.Drawing.Size(72, 41);
            this.checkBoxSelectiveZone2.TabIndex = 1;
            this.checkBoxSelectiveZone2.Text = "Zone 2";
            this.checkBoxSelectiveZone2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBoxSelectiveZone2.UseVisualStyleBackColor = false;
            this.checkBoxSelectiveZone2.CheckedChanged += new System.EventHandler(this.checkBoxSelectiveZone_CheckedChanged);
            // 
            // checkBoxSelectiveZone1
            // 
            this.checkBoxSelectiveZone1.AutoSize = true;
            this.checkBoxSelectiveZone1.BackColor = System.Drawing.Color.LightSeaGreen;
            this.checkBoxSelectiveZone1.CheckAlign = System.Drawing.ContentAlignment.TopCenter;
            this.checkBoxSelectiveZone1.Checked = true;
            this.checkBoxSelectiveZone1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxSelectiveZone1.Location = new System.Drawing.Point(18, 55);
            this.checkBoxSelectiveZone1.Name = "checkBoxSelectiveZone1";
            this.checkBoxSelectiveZone1.Size = new System.Drawing.Size(72, 41);
            this.checkBoxSelectiveZone1.TabIndex = 0;
            this.checkBoxSelectiveZone1.Text = "Zone 1";
            this.checkBoxSelectiveZone1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBoxSelectiveZone1.UseVisualStyleBackColor = false;
            this.checkBoxSelectiveZone1.CheckedChanged += new System.EventHandler(this.checkBoxSelectiveZone_CheckedChanged);
            // 
            // labelLeftLaser
            // 
            this.labelLeftLaser.BackColor = System.Drawing.Color.Black;
            this.labelLeftLaser.Font = new System.Drawing.Font("Microsoft Sans Serif", 26.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelLeftLaser.ForeColor = System.Drawing.Color.Lime;
            this.labelLeftLaser.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.labelLeftLaser.Location = new System.Drawing.Point(199, 28);
            this.labelLeftLaser.Name = "labelLeftLaser";
            this.labelLeftLaser.Size = new System.Drawing.Size(173, 38);
            this.labelLeftLaser.TabIndex = 36;
            this.labelLeftLaser.Text = "-FFFFFFF";
            this.labelLeftLaser.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelRightLaser
            // 
            this.labelRightLaser.BackColor = System.Drawing.Color.Black;
            this.labelRightLaser.Font = new System.Drawing.Font("Microsoft Sans Serif", 26.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelRightLaser.ForeColor = System.Drawing.Color.Lime;
            this.labelRightLaser.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.labelRightLaser.Location = new System.Drawing.Point(549, 28);
            this.labelRightLaser.Name = "labelRightLaser";
            this.labelRightLaser.Size = new System.Drawing.Size(173, 38);
            this.labelRightLaser.TabIndex = 37;
            this.labelRightLaser.Text = "-FFFFFFF";
            this.labelRightLaser.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panelKeyenceLasers
            // 
            this.panelKeyenceLasers.BackColor = System.Drawing.SystemColors.ControlDark;
            this.panelKeyenceLasers.Controls.Add(this.labelLeftLaser);
            this.panelKeyenceLasers.Controls.Add(this.labelRightLaser);
            this.panelKeyenceLasers.Controls.Add(this.label3);
            this.panelKeyenceLasers.Controls.Add(this.label4);
            this.panelKeyenceLasers.Controls.Add(this.labelCurProgram);
            this.panelKeyenceLasers.Location = new System.Drawing.Point(138, 433);
            this.panelKeyenceLasers.Name = "panelKeyenceLasers";
            this.panelKeyenceLasers.Size = new System.Drawing.Size(778, 73);
            this.panelKeyenceLasers.TabIndex = 38;
            this.panelKeyenceLasers.Visible = false;
            // 
            // label3
            // 
            this.label3.ForeColor = System.Drawing.Color.Yellow;
            this.label3.Location = new System.Drawing.Point(57, 31);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(145, 33);
            this.label3.TabIndex = 110;
            this.label3.Text = "Left Laser:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label4
            // 
            this.label4.ForeColor = System.Drawing.Color.Yellow;
            this.label4.Location = new System.Drawing.Point(385, 31);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(165, 33);
            this.label4.TabIndex = 111;
            this.label4.Text = "Right Laser:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.label4.Visible = false;
            // 
            // labelCurProgram
            // 
            this.labelCurProgram.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelCurProgram.ForeColor = System.Drawing.Color.Yellow;
            this.labelCurProgram.Location = new System.Drawing.Point(309, 3);
            this.labelCurProgram.Name = "labelCurProgram";
            this.labelCurProgram.Size = new System.Drawing.Size(161, 25);
            this.labelCurProgram.TabIndex = 112;
            this.labelCurProgram.Text = "Current Program: -";
            this.labelCurProgram.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tabControlLoadUnload
            // 
            this.tabControlLoadUnload.Controls.Add(this.tabPageCoatingChuck);
            this.tabControlLoadUnload.Controls.Add(this.tabPagePrimingChuck);
            this.tabControlLoadUnload.Location = new System.Drawing.Point(138, 12);
            this.tabControlLoadUnload.Name = "tabControlLoadUnload";
            this.tabControlLoadUnload.SelectedIndex = 0;
            this.tabControlLoadUnload.Size = new System.Drawing.Size(778, 125);
            this.tabControlLoadUnload.TabIndex = 39;
            // 
            // tabPageCoatingChuck
            // 
            this.tabPageCoatingChuck.Controls.Add(this.buttonPrepareToLoad);
            this.tabPageCoatingChuck.Controls.Add(this.buttonLoad);
            this.tabPageCoatingChuck.Controls.Add(this.buttonAligners);
            this.tabPageCoatingChuck.Controls.Add(this.buttonUnload);
            this.tabPageCoatingChuck.Controls.Add(this.buttonLiftPins);
            this.tabPageCoatingChuck.Location = new System.Drawing.Point(4, 42);
            this.tabPageCoatingChuck.Name = "tabPageCoatingChuck";
            this.tabPageCoatingChuck.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageCoatingChuck.Size = new System.Drawing.Size(770, 79);
            this.tabPageCoatingChuck.TabIndex = 0;
            this.tabPageCoatingChuck.Text = "Coating Chuck";
            this.tabPageCoatingChuck.UseVisualStyleBackColor = true;
            // 
            // tabPagePrimingChuck
            // 
            this.tabPagePrimingChuck.Controls.Add(this.buttonPrepareToLoadPriming);
            this.tabPagePrimingChuck.Controls.Add(this.buttonLoadPriming);
            this.tabPagePrimingChuck.Controls.Add(this.buttonUnloadPriming);
            this.tabPagePrimingChuck.Controls.Add(this.buttonLiftPinsPriming);
            this.tabPagePrimingChuck.Location = new System.Drawing.Point(4, 42);
            this.tabPagePrimingChuck.Name = "tabPagePrimingChuck";
            this.tabPagePrimingChuck.Padding = new System.Windows.Forms.Padding(3);
            this.tabPagePrimingChuck.Size = new System.Drawing.Size(770, 79);
            this.tabPagePrimingChuck.TabIndex = 1;
            this.tabPagePrimingChuck.Text = "Priming Chuck";
            this.tabPagePrimingChuck.UseVisualStyleBackColor = true;
            // 
            // buttonPrepareToLoadPriming
            // 
            this.buttonPrepareToLoadPriming.BackColor = System.Drawing.SystemColors.Control;
            this.buttonPrepareToLoadPriming.Location = new System.Drawing.Point(3, 3);
            this.buttonPrepareToLoadPriming.Name = "buttonPrepareToLoadPriming";
            this.buttonPrepareToLoadPriming.Size = new System.Drawing.Size(150, 74);
            this.buttonPrepareToLoadPriming.TabIndex = 18;
            this.buttonPrepareToLoadPriming.Text = "Prepare To Load";
            this.buttonPrepareToLoadPriming.UseVisualStyleBackColor = true;
            this.buttonPrepareToLoadPriming.Click += new System.EventHandler(this.buttonPrepareToLoadPriming_Click);
            // 
            // buttonLoadPriming
            // 
            this.buttonLoadPriming.BackColor = System.Drawing.SystemColors.Control;
            this.buttonLoadPriming.Location = new System.Drawing.Point(156, 3);
            this.buttonLoadPriming.Name = "buttonLoadPriming";
            this.buttonLoadPriming.Size = new System.Drawing.Size(150, 74);
            this.buttonLoadPriming.TabIndex = 19;
            this.buttonLoadPriming.Text = "Load";
            this.buttonLoadPriming.UseVisualStyleBackColor = true;
            this.buttonLoadPriming.Click += new System.EventHandler(this.buttonLoadPriming_Click);
            // 
            // buttonUnloadPriming
            // 
            this.buttonUnloadPriming.BackColor = System.Drawing.SystemColors.Control;
            this.buttonUnloadPriming.Location = new System.Drawing.Point(309, 3);
            this.buttonUnloadPriming.Name = "buttonUnloadPriming";
            this.buttonUnloadPriming.Size = new System.Drawing.Size(150, 74);
            this.buttonUnloadPriming.TabIndex = 20;
            this.buttonUnloadPriming.Text = "Unload";
            this.buttonUnloadPriming.UseVisualStyleBackColor = true;
            this.buttonUnloadPriming.Click += new System.EventHandler(this.buttonUnloadPriming_Click);
            // 
            // buttonLiftPinsPriming
            // 
            this.buttonLiftPinsPriming.BackColor = System.Drawing.SystemColors.Control;
            this.buttonLiftPinsPriming.Location = new System.Drawing.Point(462, 3);
            this.buttonLiftPinsPriming.Name = "buttonLiftPinsPriming";
            this.buttonLiftPinsPriming.Size = new System.Drawing.Size(150, 74);
            this.buttonLiftPinsPriming.TabIndex = 21;
            this.buttonLiftPinsPriming.Text = "Raise\r\nPins";
            this.buttonLiftPinsPriming.UseVisualStyleBackColor = true;
            this.buttonLiftPinsPriming.Click += new System.EventHandler(this.buttonLiftPinsPriming_Click);
            // 
            // FormChuck
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.SteelBlue;
            this.ClientSize = new System.Drawing.Size(1024, 514);
            this.Controls.Add(this.panelKeyenceLasers);
            this.Controls.Add(this.groupBoxSelectiveZones);
            this.Controls.Add(this.panelZone3Status);
            this.Controls.Add(this.panelMainVac);
            this.Controls.Add(this.panelZone1Status);
            this.Controls.Add(this.panelZone2Status);
            this.Controls.Add(this.panelPrimingStatuspanelPrimingStatus);
            this.Controls.Add(this.panelChuckTempController);
            this.Controls.Add(this.tabControlLoadUnload);
            this.Font = new System.Drawing.Font("Tahoma", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(5);
            this.Name = "FormChuck";
            this.Text = "Chuck Control";
            this.Load += new System.EventHandler(this.FormChuck_Load);
            this.panelChuckTempController.ResumeLayout(false);
            this.panelChuckTempController.PerformLayout();
            this.panelMainVac.ResumeLayout(false);
            this.panelZone1Status.ResumeLayout(false);
            this.panelZone2Status.ResumeLayout(false);
            this.panelPrimingStatuspanelPrimingStatus.ResumeLayout(false);
            this.panelZone3Status.ResumeLayout(false);
            this.groupBoxSelectiveZones.ResumeLayout(false);
            this.groupBoxSelectiveZones.PerformLayout();
            this.panelKeyenceLasers.ResumeLayout(false);
            this.tabControlLoadUnload.ResumeLayout(false);
            this.tabPageCoatingChuck.ResumeLayout(false);
            this.tabPagePrimingChuck.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelChuckTempController;
        private System.Windows.Forms.Label labelChuckSetPointChanges;
        private System.Windows.Forms.Button buttonChuckSetPointApply;
        private System.Windows.Forms.Button buttonChuckTempSetPoint;
        private System.Windows.Forms.Label labelChuckTempSetPoint;
        private System.Windows.Forms.Label labelChuckTempController;
        private System.Windows.Forms.Label labelChuckTempOutput;
        private System.Windows.Forms.Panel panelMainVac;
        private System.Windows.Forms.Label labelMainVacStatus;
        private System.Windows.Forms.Label labelMainVac;
        private System.Windows.Forms.Panel panelZone1Status;
        private System.Windows.Forms.Label labelZone1;
        private System.Windows.Forms.Button buttonZone1Vacuum;
        private System.Windows.Forms.Panel panelZone2Status;
        private System.Windows.Forms.Label labelZone2;
        private System.Windows.Forms.Button buttonZone2Vacuum;
        private System.Windows.Forms.Panel panelPrimingStatuspanelPrimingStatus;
        private System.Windows.Forms.Label labelPrimingZone;
        private System.Windows.Forms.Button buttonPrimingVacuum;
        private System.Windows.Forms.Panel panelZone3Status;
        private System.Windows.Forms.Label labelZone3;
        private System.Windows.Forms.Button buttonZone3Vacuum;
        private System.Windows.Forms.Button buttonPrepareToLoad;
        private System.Windows.Forms.Button buttonLoad;
        private System.Windows.Forms.Button buttonUnload;
        private System.Windows.Forms.Button buttonLiftPins;
        private System.Windows.Forms.Button buttonAligners;
		private System.Windows.Forms.GroupBox groupBoxSelectiveZones;
		private System.Windows.Forms.CheckBox checkBoxSelectiveZone3;
		private System.Windows.Forms.CheckBox checkBoxSelectiveZone2;
		private System.Windows.Forms.CheckBox checkBoxSelectiveZone1;
        private System.Windows.Forms.Label labelLeftLaser;
        private System.Windows.Forms.Label labelRightLaser;
        private System.Windows.Forms.Panel panelKeyenceLasers;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label labelCurProgram;
        private System.Windows.Forms.TabControl tabControlLoadUnload;
        private System.Windows.Forms.TabPage tabPageCoatingChuck;
        private System.Windows.Forms.TabPage tabPagePrimingChuck;
        private System.Windows.Forms.Button buttonPrepareToLoadPriming;
        private System.Windows.Forms.Button buttonLoadPriming;
        private System.Windows.Forms.Button buttonUnloadPriming;
        private System.Windows.Forms.Button buttonLiftPinsPriming;
    }
}