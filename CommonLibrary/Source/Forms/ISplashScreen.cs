using CommonLibrary.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary.Forms
{
    public interface ISplashScreen : IUpdate
    {
        #region Properties

        string Header { get; set; }

        int MinimumDurationInMs { get; set; }

        #endregion

        #region Functions

        void AddMessage(string message);

        void CloseSplash();
        #endregion
    }
}
