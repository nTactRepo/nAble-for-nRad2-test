using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using DevComponents.DotNetBar;

namespace nAble
{
    public partial class nRadMessageBox : Form
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool MessageBeep(uint type);

        
        static public LogEntry Logger { get; set; } = null;
        
        
        static private nRadMessageBox newMessageBox;
        static private Image imageIcon;

        static private DialogResult ReturnButton;

        public nRadMessageBox()
        {
            InitializeComponent();
        }

        static private void BuildMessageBox(string title)
        {
            newMessageBox = new nRadMessageBox();
            newMessageBox.labelTitle.Text = title;
            newMessageBox.StartPosition = FormStartPosition.CenterParent;
        }

        /// <summary>
        /// MIcon: Display Icon on the message box.
        /// </summary>
        static public DialogResult Show(IWin32Window owner, string Message, string Title, MessageBoxButtons MButtons, MessageBoxIcon MIcon, bool calibMsg = false)
        {
            Logger.log(LogType.ACTIVITY, Category.INFO, $"MessageBox: '{Message}'");
			BuildMessageBox(Title);
            newMessageBox.labelMessage.Text = Message;
            ButtonStatements(MButtons);
            IconStatements(MIcon);
            //Image imageIcon = new Bitmap(frmIcon.ToBitmap(), 86, 86);
            newMessageBox.pictureBoxIcon.Image = imageIcon;
            newMessageBox.pictureBoxIcon.SizeMode = PictureBoxSizeMode.StretchImage;
            newMessageBox.pictureBoxIcon.Image = imageIcon;
            if (calibMsg)
                ShowOpenLKNavButton(newMessageBox.buttonOpenLKNav);
            newMessageBox.Owner = owner is null ? null : (Form)FromHandle(owner.Handle);
            newMessageBox.ShowDialog();
			
            return ReturnButton;
        }

        static public void ShowIf(bool show, IWin32Window owner, string Message, string Title, MessageBoxButtons MButtons, MessageBoxIcon MIcon, bool calibMsg = false)
        {
            if (show)
            {
                Show(owner, Message, Title, MButtons, MIcon, calibMsg);
            }
        }

        static void btnOK_Click(object sender, EventArgs e)
        {
            ReturnButton = DialogResult.OK;
            newMessageBox.Dispose();
        }

        static void btnAbort_Click(object sender, EventArgs e)
        {
            ReturnButton = DialogResult.Abort;
            newMessageBox.Dispose();
        }

        static void btnRetry_Click(object sender, EventArgs e)
        {
            ReturnButton = DialogResult.Retry;
            newMessageBox.Dispose();
        }

        static void btnIgnore_Click(object sender, EventArgs e)
        {
            ReturnButton = DialogResult.Ignore;
            newMessageBox.Dispose();
        }

        static void btnCancel_Click(object sender, EventArgs e)
        {
            ReturnButton = DialogResult.Cancel;
            newMessageBox.Dispose();
        }

        static void btnYes_Click(object sender, EventArgs e)
        {
            ReturnButton = DialogResult.Yes;
            newMessageBox.Dispose();
        }

        static void btnNo_Click(object sender, EventArgs e)
        {
            ReturnButton = DialogResult.No;
            newMessageBox.Dispose();
        }
        static void btnOpenLKNav_Click(object sender, EventArgs e)
        {
            ProcessHelpers.OpenLKNavigator2();
        }

        static private void ShowOKButton(Button btnOK)
        {
            btnOK.Text = "OK";
            btnOK.Visible = true;
            btnOK.Click += new EventHandler(btnOK_Click);
        }

        static private void ShowOpenLKNavButton(ButtonX btnOpenLKNav)
        {
            btnOpenLKNav.Visible = true;
            btnOpenLKNav.Click += new EventHandler(btnOpenLKNav_Click);
        }
        static private void ShowAbortButton(Button btnAbort)
        {
            btnAbort.Text = "Abort";
            btnAbort.Visible = true;
            btnAbort.Click += new EventHandler(btnAbort_Click);
        }

        static private void ShowRetryButton(Button btnRetry)
        {
            btnRetry.Text = "Retry";
            btnRetry.Visible = true;
            btnRetry.Click += new EventHandler(btnRetry_Click);
        }

        static private void ShowIgnoreButton(Button btnIgnore)
        {
            btnIgnore.Text = "Ignore";
            btnIgnore.Visible = true;
            btnIgnore.Click += new EventHandler(btnIgnore_Click);
        }

        static private void ShowCancelButton(Button btnCancel)
        {
            btnCancel.Text = "Cancel";
            btnCancel.Visible = true;
            btnCancel.Click += new EventHandler(btnCancel_Click);
        }

        static private void ShowYesButton(Button btnYes)
        {
            btnYes.Text = "Yes";
            btnYes.Visible = true;
            btnYes.Click += new EventHandler(btnYes_Click);
        }

        static private void ShowNoButton(Button btnNo)
        {
            btnNo.Text = "No";
            btnNo.Visible = true;
            btnNo.Click += new EventHandler(btnNo_Click);
        }

        static private void ButtonStatements(MessageBoxButtons MButtons)
        {
            if (MButtons == MessageBoxButtons.AbortRetryIgnore)
            {
                ShowIgnoreButton(newMessageBox.button1);
                ShowRetryButton(newMessageBox.button2);
                ShowAbortButton(newMessageBox.button3);
            }

            if (MButtons == MessageBoxButtons.OK)
            {
                ShowOKButton(newMessageBox.button1);
            }

            if (MButtons == MessageBoxButtons.OKCancel)
            {
                ShowCancelButton(newMessageBox.button1);
                ShowOKButton(newMessageBox.button2);
            }

            if (MButtons == MessageBoxButtons.RetryCancel)
            {
                ShowCancelButton(newMessageBox.button1);
                ShowRetryButton(newMessageBox.button2);
            }

            if (MButtons == MessageBoxButtons.YesNo)
            {
                ShowNoButton(newMessageBox.button1);
                ShowYesButton(newMessageBox.button2);
            }

            if (MButtons == MessageBoxButtons.YesNoCancel)
            {
                ShowCancelButton(newMessageBox.button1);
                ShowNoButton(newMessageBox.button2);
                ShowYesButton(newMessageBox.button3);
            }
        }

        static private void IconStatements(MessageBoxIcon MIcon)
        {
            if (MIcon == MessageBoxIcon.Error || MIcon == MessageBoxIcon.Exclamation)
            {
                MessageBeep(30);
                imageIcon = Resources.error;
            }

            if (MIcon == MessageBoxIcon.Information)
            {
                MessageBeep(0);
                imageIcon = Resources.info;
            }

            if (MIcon == MessageBoxIcon.Question)
            {
                MessageBeep(0);
                imageIcon = Resources.help;
            }

            if (MIcon == MessageBoxIcon.Warning || MIcon == MessageBoxIcon.Hand)
            {
                MessageBeep(30);
                imageIcon = Resources.Warning;
            }
        }
    }
}
