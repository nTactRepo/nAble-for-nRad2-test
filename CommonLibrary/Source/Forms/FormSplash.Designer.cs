
namespace CommonLibrary.Forms
{
    partial class FormSplash
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
            components=new System.ComponentModel.Container();
            splitContainerRegions=new SplitContainer();
            labelHeader=new Label();
            panelBody=new Panel();
            flowLayoutPanelMessages=new FlowLayoutPanel();
            timerDelayedClose=new System.Windows.Forms.Timer(components);
            ((System.ComponentModel.ISupportInitialize)splitContainerRegions).BeginInit();
            splitContainerRegions.Panel1.SuspendLayout();
            splitContainerRegions.Panel2.SuspendLayout();
            splitContainerRegions.SuspendLayout();
            panelBody.SuspendLayout();
            SuspendLayout();
            // 
            // splitContainerRegions
            // 
            splitContainerRegions.BackColor=Color.Transparent;
            splitContainerRegions.Dock=DockStyle.Fill;
            splitContainerRegions.FixedPanel=FixedPanel.Panel1;
            splitContainerRegions.Location=new Point(0, 0);
            splitContainerRegions.Name="splitContainerRegions";
            splitContainerRegions.Orientation=Orientation.Horizontal;
            // 
            // splitContainerRegions.Panel1
            // 
            splitContainerRegions.Panel1.Controls.Add(labelHeader);
            // 
            // splitContainerRegions.Panel2
            // 
            splitContainerRegions.Panel2.Controls.Add(panelBody);
            splitContainerRegions.Panel2.Margin=new Padding(3);
            splitContainerRegions.Panel2.Padding=new Padding(3);
            splitContainerRegions.Size=new Size(421, 500);
            splitContainerRegions.SplitterDistance=54;
            splitContainerRegions.SplitterWidth=1;
            splitContainerRegions.TabIndex=0;
            // 
            // labelHeader
            // 
            labelHeader.Dock=DockStyle.Fill;
            labelHeader.Font=new Font("Tahoma", 15.75F, FontStyle.Italic, GraphicsUnit.Point);
            labelHeader.Location=new Point(0, 0);
            labelHeader.Name="labelHeader";
            labelHeader.Size=new Size(421, 54);
            labelHeader.TabIndex=0;
            labelHeader.Text="{{Header}}";
            labelHeader.TextAlign=ContentAlignment.MiddleCenter;
            // 
            // panelBody
            // 
            panelBody.Controls.Add(flowLayoutPanelMessages);
            panelBody.Dock=DockStyle.Fill;
            panelBody.Location=new Point(3, 3);
            panelBody.Name="panelBody";
            panelBody.Size=new Size(415, 439);
            panelBody.TabIndex=0;
            // 
            // flowLayoutPanelMessages
            // 
            flowLayoutPanelMessages.AutoScroll=true;
            flowLayoutPanelMessages.Dock=DockStyle.Fill;
            flowLayoutPanelMessages.Font=new Font("Tahoma", 12F, FontStyle.Regular, GraphicsUnit.Point);
            flowLayoutPanelMessages.Location=new Point(0, 0);
            flowLayoutPanelMessages.Name="flowLayoutPanelMessages";
            flowLayoutPanelMessages.Size=new Size(415, 439);
            flowLayoutPanelMessages.TabIndex=0;
            // 
            // timerDelayedClose
            // 
            timerDelayedClose.Tick+=timerDelayedClose_Tick;
            // 
            // FormSplash
            // 
            AutoScaleMode=AutoScaleMode.None;
            BackgroundImage=Resources.Resources.Splash3;
            BackgroundImageLayout=ImageLayout.Stretch;
            ClientSize=new Size(421, 500);
            ControlBox=false;
            Controls.Add(splitContainerRegions);
            DoubleBuffered=true;
            FormBorderStyle=FormBorderStyle.None;
            Name="FormSplash";
            StartPosition=FormStartPosition.CenterScreen;
            Text="Cell Manager";
            TopMost=true;
            FormClosing+=FormSplash_FormClosing;
            Load+=FormSplash_Load;
            splitContainerRegions.Panel1.ResumeLayout(false);
            splitContainerRegions.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerRegions).EndInit();
            splitContainerRegions.ResumeLayout(false);
            panelBody.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private SplitContainer splitContainerRegions;
        private Panel panelBody;
        private Label labelHeader;
        private FlowLayoutPanel flowLayoutPanelMessages;
        private System.Windows.Forms.Timer timerDelayedClose;
    }
}