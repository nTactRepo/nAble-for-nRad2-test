namespace nAble
{
    partial class FormFluidFlow
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
            System.Windows.Forms.Label labelValveHeadVent;
            System.Windows.Forms.Label labelValveDispense;
            System.Windows.Forms.Label labelValveDieVent;
            System.Windows.Forms.Label labelValveRecharge;
            this.buttonFluidHeadPurge = new System.Windows.Forms.Button();
            this.buttonFluidStopPrime = new System.Windows.Forms.Button();
            this.buttonFluidHeadPrime = new System.Windows.Forms.Button();
            this.buttonFluidPumpPrime = new System.Windows.Forms.Button();
            this.panelFluidParams = new System.Windows.Forms.Panel();
            this.labelPumpSpec = new System.Windows.Forms.Label();
            this.labelValveStatus = new System.Windows.Forms.Label();
            this.buttonRecirc = new System.Windows.Forms.Button();
            this.buttonFluidPumpPurge = new System.Windows.Forms.Button();
            this.buttonRecircTime = new System.Windows.Forms.Button();
            this.buttonRecircCount = new System.Windows.Forms.Button();
            this.labelSaveWarning = new System.Windows.Forms.Label();
            this.labelDiePressureUnits = new System.Windows.Forms.Label();
            this.labelDiePressure = new System.Windows.Forms.Label();
            this.labelPumpBRatio = new System.Windows.Forms.Label();
            this.comboBoxSelectedPump = new System.Windows.Forms.ComboBox();
            this.labelSelectedPumpTitle = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.buttonPumpARatio = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.buttonExpandRatioPanel = new System.Windows.Forms.Button();
            this.panelRatioMixing = new System.Windows.Forms.Panel();
            this.labelRecircTimeUnits = new System.Windows.Forms.Label();
            this.labelRecircCountUnits = new System.Windows.Forms.Label();
            this.panelRecirc = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.labelPumpAVolDispensed = new System.Windows.Forms.Label();
            this.labelPumpBVolDispensed = new System.Windows.Forms.Label();
            this.pictureBoxValveHeadVent = new System.Windows.Forms.PictureBox();
            this.pictureBoxValveDieVent = new System.Windows.Forms.PictureBox();
            this.pictureBoxValveDispense = new System.Windows.Forms.PictureBox();
            this.pictureBoxValveRecharge = new System.Windows.Forms.PictureBox();
            this.panelValveStatus = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.labelValveBPosition = new System.Windows.Forms.Label();
            this.labelValveAPosition = new System.Windows.Forms.Label();
            this.panelValvePosition = new System.Windows.Forms.Panel();
            this.labelDispenseRateB = new System.Windows.Forms.Label();
            this.labelDispenseRateA = new System.Windows.Forms.Label();
            this.groupBoxDiePressure = new DevComponents.DotNetBar.Controls.GroupPanel();
            this.groupBoxDispenseRate = new DevComponents.DotNetBar.Controls.GroupPanel();
            labelValveHeadVent = new System.Windows.Forms.Label();
            labelValveDispense = new System.Windows.Forms.Label();
            labelValveDieVent = new System.Windows.Forms.Label();
            labelValveRecharge = new System.Windows.Forms.Label();
            this.panelRatioMixing.SuspendLayout();
            this.panelRecirc.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxValveHeadVent)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxValveDieVent)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxValveDispense)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxValveRecharge)).BeginInit();
            this.panelValveStatus.SuspendLayout();
            this.panelValvePosition.SuspendLayout();
            this.groupBoxDiePressure.SuspendLayout();
            this.groupBoxDispenseRate.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelValveHeadVent
            // 
            labelValveHeadVent.AutoSize = true;
            labelValveHeadVent.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            labelValveHeadVent.ForeColor = System.Drawing.Color.Black;
            labelValveHeadVent.Location = new System.Drawing.Point(1, 131);
            labelValveHeadVent.Name = "labelValveHeadVent";
            labelValveHeadVent.Size = new System.Drawing.Size(66, 16);
            labelValveHeadVent.TabIndex = 153;
            labelValveHeadVent.Text = "Head Vent";
            labelValveHeadVent.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // labelValveDispense
            // 
            labelValveDispense.AutoSize = true;
            labelValveDispense.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            labelValveDispense.ForeColor = System.Drawing.Color.Black;
            labelValveDispense.Location = new System.Drawing.Point(1, 51);
            labelValveDispense.Name = "labelValveDispense";
            labelValveDispense.Size = new System.Drawing.Size(58, 16);
            labelValveDispense.TabIndex = 149;
            labelValveDispense.Text = "Dispense";
            labelValveDispense.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // labelValveDieVent
            // 
            labelValveDieVent.AutoSize = true;
            labelValveDieVent.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            labelValveDieVent.ForeColor = System.Drawing.Color.Black;
            labelValveDieVent.Location = new System.Drawing.Point(1, 11);
            labelValveDieVent.Name = "labelValveDieVent";
            labelValveDieVent.Size = new System.Drawing.Size(55, 16);
            labelValveDieVent.TabIndex = 147;
            labelValveDieVent.Text = "Die Vent";
            labelValveDieVent.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // labelValveRecharge
            // 
            labelValveRecharge.AutoSize = true;
            labelValveRecharge.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            labelValveRecharge.ForeColor = System.Drawing.Color.Black;
            labelValveRecharge.Location = new System.Drawing.Point(1, 91);
            labelValveRecharge.Name = "labelValveRecharge";
            labelValveRecharge.Size = new System.Drawing.Size(61, 16);
            labelValveRecharge.TabIndex = 151;
            labelValveRecharge.Text = "Recharge";
            labelValveRecharge.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // buttonFluidHeadPurge
            // 
            this.buttonFluidHeadPurge.BackColor = System.Drawing.SystemColors.Control;
            this.buttonFluidHeadPurge.Location = new System.Drawing.Point(242, 152);
            this.buttonFluidHeadPurge.Name = "buttonFluidHeadPurge";
            this.buttonFluidHeadPurge.Size = new System.Drawing.Size(215, 87);
            this.buttonFluidHeadPurge.TabIndex = 13;
            this.buttonFluidHeadPurge.Text = "Head Purge";
            this.buttonFluidHeadPurge.UseVisualStyleBackColor = true;
            this.buttonFluidHeadPurge.Click += new System.EventHandler(this.buttonFluidHeadPurge_Click);
            // 
            // buttonFluidStopPrime
            // 
            this.buttonFluidStopPrime.BackColor = System.Drawing.SystemColors.Control;
            this.buttonFluidStopPrime.Enabled = false;
            this.buttonFluidStopPrime.Location = new System.Drawing.Point(22, 408);
            this.buttonFluidStopPrime.Name = "buttonFluidStopPrime";
            this.buttonFluidStopPrime.Size = new System.Drawing.Size(189, 83);
            this.buttonFluidStopPrime.TabIndex = 12;
            this.buttonFluidStopPrime.Text = "Stop";
            this.buttonFluidStopPrime.UseVisualStyleBackColor = true;
            this.buttonFluidStopPrime.Click += new System.EventHandler(this.buttonFluidStopPrime_Click);
            // 
            // buttonFluidHeadPrime
            // 
            this.buttonFluidHeadPrime.BackColor = System.Drawing.SystemColors.Control;
            this.buttonFluidHeadPrime.Location = new System.Drawing.Point(21, 152);
            this.buttonFluidHeadPrime.Name = "buttonFluidHeadPrime";
            this.buttonFluidHeadPrime.Size = new System.Drawing.Size(215, 87);
            this.buttonFluidHeadPrime.TabIndex = 11;
            this.buttonFluidHeadPrime.Text = "Head Prime";
            this.buttonFluidHeadPrime.UseVisualStyleBackColor = true;
            this.buttonFluidHeadPrime.Click += new System.EventHandler(this.buttonFluidHeadPrime_Click);
            // 
            // buttonFluidPumpPrime
            // 
            this.buttonFluidPumpPrime.BackColor = System.Drawing.SystemColors.Control;
            this.buttonFluidPumpPrime.Location = new System.Drawing.Point(22, 63);
            this.buttonFluidPumpPrime.Name = "buttonFluidPumpPrime";
            this.buttonFluidPumpPrime.Size = new System.Drawing.Size(215, 87);
            this.buttonFluidPumpPrime.TabIndex = 10;
            this.buttonFluidPumpPrime.Text = "Pump Prime";
            this.buttonFluidPumpPrime.UseVisualStyleBackColor = true;
            this.buttonFluidPumpPrime.Click += new System.EventHandler(this.buttonFluidPumpPrime_Click);
            // 
            // panelFluidParams
            // 
            this.panelFluidParams.Location = new System.Drawing.Point(566, 52);
            this.panelFluidParams.Name = "panelFluidParams";
            this.panelFluidParams.Size = new System.Drawing.Size(420, 310);
            this.panelFluidParams.TabIndex = 14;
            // 
            // labelPumpSpec
            // 
            this.labelPumpSpec.BackColor = System.Drawing.Color.Transparent;
            this.labelPumpSpec.Location = new System.Drawing.Point(566, 12);
            this.labelPumpSpec.Name = "labelPumpSpec";
            this.labelPumpSpec.Size = new System.Drawing.Size(420, 34);
            this.labelPumpSpec.TabIndex = 15;
            this.labelPumpSpec.Text = "Pump: Syringe";
            this.labelPumpSpec.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelValveStatus
            // 
            this.labelValveStatus.BackColor = System.Drawing.Color.LightSalmon;
            this.labelValveStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelValveStatus.Location = new System.Drawing.Point(566, 422);
            this.labelValveStatus.Name = "labelValveStatus";
            this.labelValveStatus.Size = new System.Drawing.Size(420, 80);
            this.labelValveStatus.TabIndex = 16;
            this.labelValveStatus.Text = "Ready";
            this.labelValveStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // buttonRecirc
            // 
            this.buttonRecirc.BackColor = System.Drawing.SystemColors.Control;
            this.buttonRecirc.Location = new System.Drawing.Point(0, 7);
            this.buttonRecirc.Name = "buttonRecirc";
            this.buttonRecirc.Size = new System.Drawing.Size(215, 87);
            this.buttonRecirc.TabIndex = 17;
            this.buttonRecirc.Text = "Recirculate";
            this.buttonRecirc.UseVisualStyleBackColor = true;
            this.buttonRecirc.Click += new System.EventHandler(this.buttonRecirc_Click);
            // 
            // buttonFluidPumpPurge
            // 
            this.buttonFluidPumpPurge.BackColor = System.Drawing.SystemColors.Control;
            this.buttonFluidPumpPurge.Location = new System.Drawing.Point(242, 63);
            this.buttonFluidPumpPurge.Name = "buttonFluidPumpPurge";
            this.buttonFluidPumpPurge.Size = new System.Drawing.Size(215, 87);
            this.buttonFluidPumpPurge.TabIndex = 18;
            this.buttonFluidPumpPurge.Text = "Pump Purge";
            this.buttonFluidPumpPurge.UseVisualStyleBackColor = true;
            this.buttonFluidPumpPurge.Click += new System.EventHandler(this.buttonPumpPurge_Click);
            // 
            // buttonRecircTime
            // 
            this.buttonRecircTime.Location = new System.Drawing.Point(221, 7);
            this.buttonRecircTime.Name = "buttonRecircTime";
            this.buttonRecircTime.Size = new System.Drawing.Size(108, 40);
            this.buttonRecircTime.TabIndex = 129;
            this.buttonRecircTime.Text = "5";
            this.buttonRecircTime.UseVisualStyleBackColor = true;
            this.buttonRecircTime.Click += new System.EventHandler(this.buttonRecircTime_Click);
            // 
            // buttonRecircCount
            // 
            this.buttonRecircCount.Location = new System.Drawing.Point(221, 54);
            this.buttonRecircCount.Name = "buttonRecircCount";
            this.buttonRecircCount.Size = new System.Drawing.Size(108, 40);
            this.buttonRecircCount.TabIndex = 127;
            this.buttonRecircCount.Text = "1";
            this.buttonRecircCount.UseVisualStyleBackColor = true;
            this.buttonRecircCount.Click += new System.EventHandler(this.buttonRecircCount_Click);
            // 
            // labelSaveWarning
            // 
            this.labelSaveWarning.BackColor = System.Drawing.Color.Yellow;
            this.labelSaveWarning.ForeColor = System.Drawing.Color.Black;
            this.labelSaveWarning.Location = new System.Drawing.Point(45, 363);
            this.labelSaveWarning.Name = "labelSaveWarning";
            this.labelSaveWarning.Size = new System.Drawing.Size(395, 33);
            this.labelSaveWarning.TabIndex = 131;
            this.labelSaveWarning.Text = "Toggle Enable to Save Changes";
            this.labelSaveWarning.Visible = false;
            // 
            // labelDiePressureUnits
            // 
            this.labelDiePressureUnits.BackColor = System.Drawing.Color.Transparent;
            this.labelDiePressureUnits.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelDiePressureUnits.ForeColor = System.Drawing.Color.Black;
            this.labelDiePressureUnits.Location = new System.Drawing.Point(2, 30);
            this.labelDiePressureUnits.Name = "labelDiePressureUnits";
            this.labelDiePressureUnits.Size = new System.Drawing.Size(128, 30);
            this.labelDiePressureUnits.TabIndex = 32;
            this.labelDiePressureUnits.Text = "PSI";
            this.labelDiePressureUnits.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelDiePressureUnits.Click += new System.EventHandler(this.labelDiePressure_Click);
            // 
            // labelDiePressure
            // 
            this.labelDiePressure.BackColor = System.Drawing.Color.Transparent;
            this.labelDiePressure.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelDiePressure.ForeColor = System.Drawing.Color.Black;
            this.labelDiePressure.Location = new System.Drawing.Point(2, 0);
            this.labelDiePressure.Name = "labelDiePressure";
            this.labelDiePressure.Size = new System.Drawing.Size(128, 30);
            this.labelDiePressure.TabIndex = 32;
            this.labelDiePressure.Text = "00.00";
            this.labelDiePressure.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelDiePressure.Click += new System.EventHandler(this.labelDiePressure_Click);
            // 
            // labelPumpBRatio
            // 
            this.labelPumpBRatio.AutoSize = true;
            this.labelPumpBRatio.ForeColor = System.Drawing.Color.White;
            this.labelPumpBRatio.Location = new System.Drawing.Point(323, 91);
            this.labelPumpBRatio.Name = "labelPumpBRatio";
            this.labelPumpBRatio.Size = new System.Drawing.Size(98, 33);
            this.labelPumpBRatio.TabIndex = 140;
            this.labelPumpBRatio.Text = "50.000";
            this.labelPumpBRatio.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // comboBoxSelectedPump
            // 
            this.comboBoxSelectedPump.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSelectedPump.FormattingEnabled = true;
            this.comboBoxSelectedPump.Items.AddRange(new object[] {
            "Pump-A",
            "Pump-B",
            "Mixing"});
            this.comboBoxSelectedPump.Location = new System.Drawing.Point(219, 10);
            this.comboBoxSelectedPump.Name = "comboBoxSelectedPump";
            this.comboBoxSelectedPump.Size = new System.Drawing.Size(215, 41);
            this.comboBoxSelectedPump.TabIndex = 139;
            this.comboBoxSelectedPump.SelectedIndexChanged += new System.EventHandler(this.comboBoxSelectedPump_SelectedIndexChanged);
            // 
            // labelSelectedPumpTitle
            // 
            this.labelSelectedPumpTitle.AutoSize = true;
            this.labelSelectedPumpTitle.Location = new System.Drawing.Point(13, 14);
            this.labelSelectedPumpTitle.Name = "labelSelectedPumpTitle";
            this.labelSelectedPumpTitle.Size = new System.Drawing.Size(200, 33);
            this.labelSelectedPumpTitle.TabIndex = 138;
            this.labelSelectedPumpTitle.Text = "Selected Pump:";
            this.labelSelectedPumpTitle.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(5, 91);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(302, 33);
            this.label5.TabIndex = 136;
            this.label5.Text = "Pump-B Dispense Ratio:";
            this.label5.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ForeColor = System.Drawing.Color.White;
            this.label6.Location = new System.Drawing.Point(438, 91);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(41, 33);
            this.label6.TabIndex = 137;
            this.label6.Text = "%";
            this.label6.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // buttonPumpARatio
            // 
            this.buttonPumpARatio.Location = new System.Drawing.Point(313, 27);
            this.buttonPumpARatio.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.buttonPumpARatio.Name = "buttonPumpARatio";
            this.buttonPumpARatio.Size = new System.Drawing.Size(119, 48);
            this.buttonPumpARatio.TabIndex = 135;
            this.buttonPumpARatio.Text = "50.000";
            this.buttonPumpARatio.UseVisualStyleBackColor = true;
            this.buttonPumpARatio.TextChanged += new System.EventHandler(this.buttonPumpARatio_TextChanged);
            this.buttonPumpARatio.Click += new System.EventHandler(this.buttonPumpARatio_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(5, 35);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(302, 33);
            this.label1.TabIndex = 133;
            this.label1.Text = "Pump-A Dispense Ratio:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(438, 35);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 33);
            this.label4.TabIndex = 134;
            this.label4.Text = "%";
            this.label4.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // buttonExpandRatioPanel
            // 
            this.buttonExpandRatioPanel.Location = new System.Drawing.Point(440, 10);
            this.buttonExpandRatioPanel.Name = "buttonExpandRatioPanel";
            this.buttonExpandRatioPanel.Size = new System.Drawing.Size(56, 41);
            this.buttonExpandRatioPanel.TabIndex = 141;
            this.buttonExpandRatioPanel.Text = "...";
            this.buttonExpandRatioPanel.UseVisualStyleBackColor = true;
            this.buttonExpandRatioPanel.Visible = false;
            this.buttonExpandRatioPanel.Click += new System.EventHandler(this.buttonExpandRatioPanel_Click);
            // 
            // panelRatioMixing
            // 
            this.panelRatioMixing.BackColor = System.Drawing.Color.SteelBlue;
            this.panelRatioMixing.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelRatioMixing.Controls.Add(this.label5);
            this.panelRatioMixing.Controls.Add(this.label4);
            this.panelRatioMixing.Controls.Add(this.labelPumpBRatio);
            this.panelRatioMixing.Controls.Add(this.label1);
            this.panelRatioMixing.Controls.Add(this.buttonPumpARatio);
            this.panelRatioMixing.Controls.Add(this.label6);
            this.panelRatioMixing.Location = new System.Drawing.Point(19, 57);
            this.panelRatioMixing.Name = "panelRatioMixing";
            this.panelRatioMixing.Size = new System.Drawing.Size(477, 439);
            this.panelRatioMixing.TabIndex = 146;
            this.panelRatioMixing.Visible = false;
            // 
            // labelRecircTimeUnits
            // 
            this.labelRecircTimeUnits.AutoSize = true;
            this.labelRecircTimeUnits.Location = new System.Drawing.Point(335, 11);
            this.labelRecircTimeUnits.Name = "labelRecircTimeUnits";
            this.labelRecircTimeUnits.Size = new System.Drawing.Size(109, 33);
            this.labelRecircTimeUnits.TabIndex = 150;
            this.labelRecircTimeUnits.Text = "minutes";
            // 
            // labelRecircCountUnits
            // 
            this.labelRecircCountUnits.AutoSize = true;
            this.labelRecircCountUnits.Location = new System.Drawing.Point(335, 58);
            this.labelRecircCountUnits.Name = "labelRecircCountUnits";
            this.labelRecircCountUnits.Size = new System.Drawing.Size(100, 33);
            this.labelRecircCountUnits.TabIndex = 149;
            this.labelRecircCountUnits.Text = "strokes";
            // 
            // panelRecirc
            // 
            this.panelRecirc.BackColor = System.Drawing.Color.Transparent;
            this.panelRecirc.Controls.Add(this.labelRecircTimeUnits);
            this.panelRecirc.Controls.Add(this.buttonRecirc);
            this.panelRecirc.Controls.Add(this.labelRecircCountUnits);
            this.panelRecirc.Controls.Add(this.buttonRecircCount);
            this.panelRecirc.Controls.Add(this.buttonRecircTime);
            this.panelRecirc.Location = new System.Drawing.Point(21, 251);
            this.panelRecirc.Name = "panelRecirc";
            this.panelRecirc.Size = new System.Drawing.Size(464, 100);
            this.panelRecirc.TabIndex = 141;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(566, 366);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(171, 23);
            this.label2.TabIndex = 138;
            this.label2.Text = "Dispensed Volume:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            // 
            // labelPumpAVolDispensed
            // 
            this.labelPumpAVolDispensed.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelPumpAVolDispensed.Location = new System.Drawing.Point(738, 366);
            this.labelPumpAVolDispensed.Name = "labelPumpAVolDispensed";
            this.labelPumpAVolDispensed.Size = new System.Drawing.Size(116, 23);
            this.labelPumpAVolDispensed.TabIndex = 138;
            this.labelPumpAVolDispensed.Text = "A: 00.000 ml";
            this.labelPumpAVolDispensed.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelPumpBVolDispensed
            // 
            this.labelPumpBVolDispensed.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelPumpBVolDispensed.Location = new System.Drawing.Point(871, 366);
            this.labelPumpBVolDispensed.Name = "labelPumpBVolDispensed";
            this.labelPumpBVolDispensed.Size = new System.Drawing.Size(116, 23);
            this.labelPumpBVolDispensed.TabIndex = 138;
            this.labelPumpBVolDispensed.Text = "B: 00.000 ml";
            this.labelPumpBVolDispensed.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pictureBoxValveHeadVent
            // 
            this.pictureBoxValveHeadVent.BackColor = System.Drawing.Color.Gray;
            this.pictureBoxValveHeadVent.Location = new System.Drawing.Point(21, 150);
            this.pictureBoxValveHeadVent.Name = "pictureBoxValveHeadVent";
            this.pictureBoxValveHeadVent.Size = new System.Drawing.Size(18, 18);
            this.pictureBoxValveHeadVent.TabIndex = 154;
            this.pictureBoxValveHeadVent.TabStop = false;
            // 
            // pictureBoxValveDieVent
            // 
            this.pictureBoxValveDieVent.BackColor = System.Drawing.Color.Red;
            this.pictureBoxValveDieVent.Location = new System.Drawing.Point(21, 30);
            this.pictureBoxValveDieVent.Name = "pictureBoxValveDieVent";
            this.pictureBoxValveDieVent.Size = new System.Drawing.Size(18, 18);
            this.pictureBoxValveDieVent.TabIndex = 148;
            this.pictureBoxValveDieVent.TabStop = false;
            // 
            // pictureBoxValveDispense
            // 
            this.pictureBoxValveDispense.BackColor = System.Drawing.Color.Gray;
            this.pictureBoxValveDispense.Location = new System.Drawing.Point(21, 70);
            this.pictureBoxValveDispense.Name = "pictureBoxValveDispense";
            this.pictureBoxValveDispense.Size = new System.Drawing.Size(18, 18);
            this.pictureBoxValveDispense.TabIndex = 150;
            this.pictureBoxValveDispense.TabStop = false;
            // 
            // pictureBoxValveRecharge
            // 
            this.pictureBoxValveRecharge.BackColor = System.Drawing.Color.Gray;
            this.pictureBoxValveRecharge.Location = new System.Drawing.Point(21, 110);
            this.pictureBoxValveRecharge.Name = "pictureBoxValveRecharge";
            this.pictureBoxValveRecharge.Size = new System.Drawing.Size(18, 18);
            this.pictureBoxValveRecharge.TabIndex = 152;
            this.pictureBoxValveRecharge.TabStop = false;
            // 
            // panelValveStatus
            // 
            this.panelValveStatus.Controls.Add(this.pictureBoxValveHeadVent);
            this.panelValveStatus.Controls.Add(this.pictureBoxValveDieVent);
            this.panelValveStatus.Controls.Add(labelValveRecharge);
            this.panelValveStatus.Controls.Add(this.pictureBoxValveDispense);
            this.panelValveStatus.Controls.Add(labelValveDieVent);
            this.panelValveStatus.Controls.Add(this.pictureBoxValveRecharge);
            this.panelValveStatus.Controls.Add(labelValveDispense);
            this.panelValveStatus.Controls.Add(labelValveHeadVent);
            this.panelValveStatus.Location = new System.Drawing.Point(498, 125);
            this.panelValveStatus.Name = "panelValveStatus";
            this.panelValveStatus.Size = new System.Drawing.Size(67, 202);
            this.panelValveStatus.TabIndex = 155;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(41, 3);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(130, 23);
            this.label3.TabIndex = 156;
            this.label3.Text = "Valve Position:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            // 
            // labelValveBPosition
            // 
            this.labelValveBPosition.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelValveBPosition.Location = new System.Drawing.Point(305, 3);
            this.labelValveBPosition.Name = "labelValveBPosition";
            this.labelValveBPosition.Size = new System.Drawing.Size(116, 23);
            this.labelValveBPosition.TabIndex = 157;
            this.labelValveBPosition.Text = "Unknown";
            this.labelValveBPosition.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelValveAPosition
            // 
            this.labelValveAPosition.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelValveAPosition.Location = new System.Drawing.Point(172, 3);
            this.labelValveAPosition.Name = "labelValveAPosition";
            this.labelValveAPosition.Size = new System.Drawing.Size(116, 23);
            this.labelValveAPosition.TabIndex = 158;
            this.labelValveAPosition.Text = "Unknown";
            this.labelValveAPosition.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panelValvePosition
            // 
            this.panelValvePosition.Controls.Add(this.labelValveAPosition);
            this.panelValvePosition.Controls.Add(this.labelValveBPosition);
            this.panelValvePosition.Controls.Add(this.label3);
            this.panelValvePosition.Location = new System.Drawing.Point(566, 392);
            this.panelValvePosition.Name = "panelValvePosition";
            this.panelValvePosition.Size = new System.Drawing.Size(421, 29);
            this.panelValvePosition.TabIndex = 159;
            this.panelValvePosition.Visible = false;
            // 
            // labelDispenseRateB
            // 
            this.labelDispenseRateB.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelDispenseRateB.ForeColor = System.Drawing.Color.Black;
            this.labelDispenseRateB.Location = new System.Drawing.Point(9, 29);
            this.labelDispenseRateB.Name = "labelDispenseRateB";
            this.labelDispenseRateB.Size = new System.Drawing.Size(104, 30);
            this.labelDispenseRateB.TabIndex = 32;
            this.labelDispenseRateB.Text = "B:00.00 ml/m";
            this.labelDispenseRateB.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelDispenseRateB.Click += new System.EventHandler(this.labelDiePressure_Click);
            // 
            // labelDispenseRateA
            // 
            this.labelDispenseRateA.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelDispenseRateA.ForeColor = System.Drawing.Color.Black;
            this.labelDispenseRateA.Location = new System.Drawing.Point(9, -1);
            this.labelDispenseRateA.Name = "labelDispenseRateA";
            this.labelDispenseRateA.Size = new System.Drawing.Size(102, 30);
            this.labelDispenseRateA.TabIndex = 32;
            this.labelDispenseRateA.Text = "A:00.00 ml/m";
            this.labelDispenseRateA.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelDispenseRateA.Click += new System.EventHandler(this.labelDiePressure_Click);
            // 
            // groupBoxDiePressure
            // 
            this.groupBoxDiePressure.BackColor = System.Drawing.Color.LightSkyBlue;
            this.groupBoxDiePressure.CanvasColor = System.Drawing.SystemColors.Control;
            this.groupBoxDiePressure.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007;
            this.groupBoxDiePressure.Controls.Add(this.labelDiePressureUnits);
            this.groupBoxDiePressure.Controls.Add(this.labelDiePressure);
            this.groupBoxDiePressure.DisabledBackColor = System.Drawing.Color.Empty;
            this.groupBoxDiePressure.DrawTitleBox = false;
            this.groupBoxDiePressure.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBoxDiePressure.Location = new System.Drawing.Point(214, 399);
            this.groupBoxDiePressure.Name = "groupBoxDiePressure";
            this.groupBoxDiePressure.Size = new System.Drawing.Size(140, 92);
            // 
            // 
            // 
            this.groupBoxDiePressure.Style.BackColor = System.Drawing.Color.LightSkyBlue;
            this.groupBoxDiePressure.Style.BackColor2 = System.Drawing.Color.LightSkyBlue;
            this.groupBoxDiePressure.Style.BackColorGradientAngle = 90;
            this.groupBoxDiePressure.Style.BorderBottom = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.groupBoxDiePressure.Style.BorderBottomWidth = 1;
            this.groupBoxDiePressure.Style.BorderColor = System.Drawing.Color.Black;
            this.groupBoxDiePressure.Style.BorderLeft = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.groupBoxDiePressure.Style.BorderLeftWidth = 1;
            this.groupBoxDiePressure.Style.BorderRight = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.groupBoxDiePressure.Style.BorderRightWidth = 1;
            this.groupBoxDiePressure.Style.BorderTop = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.groupBoxDiePressure.Style.BorderTopWidth = 1;
            this.groupBoxDiePressure.Style.CornerDiameter = 4;
            this.groupBoxDiePressure.Style.CornerType = DevComponents.DotNetBar.eCornerType.Rounded;
            this.groupBoxDiePressure.Style.TextAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Center;
            this.groupBoxDiePressure.Style.TextColor = System.Drawing.Color.Black;
            this.groupBoxDiePressure.Style.TextLineAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Near;
            // 
            // 
            // 
            this.groupBoxDiePressure.StyleMouseDown.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            // 
            // 
            // 
            this.groupBoxDiePressure.StyleMouseOver.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.groupBoxDiePressure.TabIndex = 141;
            this.groupBoxDiePressure.Text = "Die Pressure Avg";
            // 
            // groupBoxDispenseRate
            // 
            this.groupBoxDispenseRate.BackColor = System.Drawing.Color.LightSkyBlue;
            this.groupBoxDispenseRate.CanvasColor = System.Drawing.SystemColors.Control;
            this.groupBoxDispenseRate.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007;
            this.groupBoxDispenseRate.Controls.Add(this.labelDispenseRateB);
            this.groupBoxDispenseRate.Controls.Add(this.labelDispenseRateA);
            this.groupBoxDispenseRate.DisabledBackColor = System.Drawing.Color.Empty;
            this.groupBoxDispenseRate.DrawTitleBox = false;
            this.groupBoxDispenseRate.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBoxDispenseRate.Location = new System.Drawing.Point(357, 399);
            this.groupBoxDispenseRate.Name = "groupBoxDispenseRate";
            this.groupBoxDispenseRate.Size = new System.Drawing.Size(140, 92);
            // 
            // 
            // 
            this.groupBoxDispenseRate.Style.BackColor = System.Drawing.Color.LightSkyBlue;
            this.groupBoxDispenseRate.Style.BackColor2 = System.Drawing.Color.LightSkyBlue;
            this.groupBoxDispenseRate.Style.BackColorGradientAngle = 90;
            this.groupBoxDispenseRate.Style.BorderBottom = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.groupBoxDispenseRate.Style.BorderBottomWidth = 1;
            this.groupBoxDispenseRate.Style.BorderColor = System.Drawing.Color.Black;
            this.groupBoxDispenseRate.Style.BorderLeft = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.groupBoxDispenseRate.Style.BorderLeftWidth = 1;
            this.groupBoxDispenseRate.Style.BorderRight = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.groupBoxDispenseRate.Style.BorderRightWidth = 1;
            this.groupBoxDispenseRate.Style.BorderTop = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.groupBoxDispenseRate.Style.BorderTopWidth = 1;
            this.groupBoxDispenseRate.Style.CornerDiameter = 4;
            this.groupBoxDispenseRate.Style.CornerType = DevComponents.DotNetBar.eCornerType.Rounded;
            this.groupBoxDispenseRate.Style.TextAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Center;
            this.groupBoxDispenseRate.Style.TextColor = System.Drawing.Color.Black;
            this.groupBoxDispenseRate.Style.TextLineAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Near;
            // 
            // 
            // 
            this.groupBoxDispenseRate.StyleMouseDown.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            // 
            // 
            // 
            this.groupBoxDispenseRate.StyleMouseOver.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.groupBoxDispenseRate.TabIndex = 141;
            this.groupBoxDispenseRate.Text = "Dispense Rate";
            // 
            // FormFluidFlow
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.LightSkyBlue;
            this.ClientSize = new System.Drawing.Size(1024, 514);
            this.Controls.Add(this.panelValvePosition);
            this.Controls.Add(this.buttonExpandRatioPanel);
            this.Controls.Add(this.groupBoxDiePressure);
            this.Controls.Add(this.groupBoxDispenseRate);
            this.Controls.Add(this.comboBoxSelectedPump);
            this.Controls.Add(this.labelPumpBVolDispensed);
            this.Controls.Add(this.labelPumpAVolDispensed);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.labelSelectedPumpTitle);
            this.Controls.Add(this.buttonFluidHeadPurge);
            this.Controls.Add(this.labelSaveWarning);
            this.Controls.Add(this.buttonFluidPumpPurge);
            this.Controls.Add(this.labelValveStatus);
            this.Controls.Add(this.labelPumpSpec);
            this.Controls.Add(this.panelFluidParams);
            this.Controls.Add(this.buttonFluidStopPrime);
            this.Controls.Add(this.buttonFluidHeadPrime);
            this.Controls.Add(this.buttonFluidPumpPrime);
            this.Controls.Add(this.panelRecirc);
            this.Controls.Add(this.panelValveStatus);
            this.Controls.Add(this.panelRatioMixing);
            this.Font = new System.Drawing.Font("Tahoma", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(5);
            this.Name = "FormFluidFlow";
            this.Text = "Fluid Flow";
            this.Load += new System.EventHandler(this.FormFluidFlow_Load);
            this.Enter += new System.EventHandler(this.FormFluidFlow_Enter);
            this.panelRatioMixing.ResumeLayout(false);
            this.panelRatioMixing.PerformLayout();
            this.panelRecirc.ResumeLayout(false);
            this.panelRecirc.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxValveHeadVent)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxValveDieVent)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxValveDispense)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxValveRecharge)).EndInit();
            this.panelValveStatus.ResumeLayout(false);
            this.panelValveStatus.PerformLayout();
            this.panelValvePosition.ResumeLayout(false);
            this.panelValvePosition.PerformLayout();
            this.groupBoxDiePressure.ResumeLayout(false);
            this.groupBoxDispenseRate.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonFluidHeadPurge;
        private System.Windows.Forms.Button buttonFluidStopPrime;
        private System.Windows.Forms.Button buttonFluidHeadPrime;
        private System.Windows.Forms.Button buttonFluidPumpPrime;
        private System.Windows.Forms.Panel panelFluidParams;
        private System.Windows.Forms.Label labelPumpSpec;
        private System.Windows.Forms.Label labelReclabelRecircTimeUnits;
        private System.Windows.Forms.Label labelValveStatus;
        private System.Windows.Forms.Button buttonRecirc;
        private System.Windows.Forms.Button buttonFluidPumpPurge;
		private System.Windows.Forms.Label labelSaveWarning;
		internal System.Windows.Forms.Button buttonRecircTime;
		internal System.Windows.Forms.Button buttonRecircCount;
		private System.Windows.Forms.Label labelDiePressure;
        private System.Windows.Forms.Label labelPumpBRatio;
        private System.Windows.Forms.ComboBox comboBoxSelectedPump;
        private System.Windows.Forms.Label labelSelectedPumpTitle;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button buttonPumpARatio;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        internal System.Windows.Forms.Button buttonExpandRatioPanel;
        private System.Windows.Forms.Panel panelRatioMixing;
        private System.Windows.Forms.Label labelRecircTimeUnits;
        private System.Windows.Forms.Label labelRecircCountUnits;
        private System.Windows.Forms.Panel panelRecirc;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labelPumpAVolDispensed;
        private System.Windows.Forms.Label labelPumpBVolDispensed;
        private System.Windows.Forms.PictureBox pictureBoxValveHeadVent;
        private System.Windows.Forms.PictureBox pictureBoxValveDieVent;
        private System.Windows.Forms.PictureBox pictureBoxValveDispense;
        private System.Windows.Forms.PictureBox pictureBoxValveRecharge;
        private System.Windows.Forms.Panel panelValveStatus;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label labelValveBPosition;
        private System.Windows.Forms.Label labelValveAPosition;
        private System.Windows.Forms.Panel panelValvePosition;
        private System.Windows.Forms.Label labelDiePressureUnits;
        private System.Windows.Forms.Label labelDispenseRateB;
        private System.Windows.Forms.Label labelDispenseRateA;
        private DevComponents.DotNetBar.Controls.GroupPanel groupBoxDiePressure;
        private DevComponents.DotNetBar.Controls.GroupPanel groupBoxDispenseRate;
    }
}