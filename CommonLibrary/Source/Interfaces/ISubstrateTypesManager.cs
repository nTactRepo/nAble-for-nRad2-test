using System;
using CommonLibrary.Model;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nAble.Utils
{
    public interface ISubstrateTypesManager
    {
        #region Events

        event Action NewTypesAvailable;

        #endregion

        #region Properties

        SubstrateTypes CurrentTypes { get; }

        bool AllowUpdates { get; set; }

        #endregion

        #region Functions

        void Refresh();

        #endregion
    }
}
