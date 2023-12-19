using System;
using System.Windows.Forms;

namespace nAble.Utils
{
    public static class TabControlExtensions
    {
        /// <summary>
        /// Shows or hides a tab correctly
        /// </summary>
        /// <param name="tabControl">The control in question</param>
        /// <param name="page">The tab in question</param>
        /// <param name="show">If true, show the tabPage, else hide it</param>
        /// <param name="index">If -1, add at the end, else insert at the index.</param>
        public static void ShowOrHideTab(this TabControl tabControl, TabPage page, bool show, int index = -1)
        {
            if (show)
            {
                tabControl.ShowTab(page, index);
            }
            else
            {
                tabControl.HideTab(page);
            }
        }

        /// <summary>
        /// Shows or hides a tab correctly
        /// </summary>
        /// <param name="tabControl">The control in question</param>
        /// <param name="page">The tab in question</param>
        /// <param name="index">If -1, add at the end, else insert at the index.</param>
        public static void ShowTab(this TabControl tabControl, TabPage page, int index = -1)
        {
            if (page is null)
            {
                throw new ArgumentNullException(nameof(page));
            }

            if (!tabControl.TabPages.Contains(page))
            {
                if (index == -1)
                {
                    tabControl.TabPages.Add(page);
                }
                else
                {
                    tabControl.TabPages.Insert(index, page);
                }
            }
        }

        /// <summary>
        /// Hides a tab correctly
        /// </summary>
        /// <param name="tabControl">The control in question</param>
        /// <param name="page">The tab in question</param>
        public static void HideTab(this TabControl tabControl, TabPage page)
        {
            if (page is null)
            {
                throw new ArgumentNullException(nameof(page));
            }

            if (tabControl.TabPages.Contains(page))
            {
                tabControl.TabPages.Remove(page);
            }
        }

        /// <summary>
        /// Climbs up the parent list to find the Form that owns this control
        /// </summary>
        /// <param name="tabControl">The control in question</param>
        /// <returns>The owning form, or null if not found</returns>
        public static Form GetOwningForm(this TabControl tabControl)
        {
            Control currentCtrl = tabControl.Parent;

            while (currentCtrl != null)
            {
                if (currentCtrl is Form form)
                {
                    return form;
                }

                currentCtrl = currentCtrl.Parent;
            }

            return null;
        }
    }
}
