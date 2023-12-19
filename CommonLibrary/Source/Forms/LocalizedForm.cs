using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CommonLibrary.Source.Forms
{
    public class LocalizedForm : Form
    {
        #region Events

        [Browsable(true)]
        [Description("Occurs when current UI culture is changed")]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [Category("Property Changed")]
        public event EventHandler CultureChanged;

        #endregion

        #region Data Members

        protected CultureInfo culture;
        protected ComponentResourceManager resManager;

        #endregion

        #region Properties

        /// <summary>
        /// Current culture of this form
        /// </summary>
        [Browsable(false)]
        [Description("Current culture of this form")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public CultureInfo Culture
        {
            get => culture;

            set
            {
                if (culture != value)
                {
                    ApplyResources(this, value);

                    culture = value;
                    OnCultureChanged();
                }
            }
        }

        #endregion

        #region Functions

        public LocalizedForm()
        {
            resManager = new ComponentResourceManager(GetType());
            culture = CultureInfo.CurrentUICulture;
        }

        private void ApplyResources(Control parent, CultureInfo culture)
        {
            resManager.ApplyResources(parent, parent.Name, culture);

            foreach (Control ctl in parent.Controls)
            {
                ApplyResources(ctl, culture);
            }
        }

        protected void OnCultureChanged()
        {
            CultureChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion
    }
}
