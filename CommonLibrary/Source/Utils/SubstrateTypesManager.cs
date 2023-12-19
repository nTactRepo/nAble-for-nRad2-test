using CommonLibrary.Utils;
using CommonLibrary.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nAble.Utils
{
    public class SubstrateTypesManager : ISubstrateTypesManager, IUpdate
    {
        #region Events

        public event Action NewTypesAvailable;

        #endregion

        #region Constants

        private const int RefreshTimeinSecs = 20;

        #endregion

        #region Properties

        public SubstrateTypes CurrentTypes { get; private set; } = new SubstrateTypes();

        public bool AllowUpdates { get; set; } = true;

        #endregion

        #region Data Members

        private DateTime _nextUpdate = DateTime.MinValue;
        //private FileWatcher _watcher = null;

        #endregion

        #region Functions

        #region Constructors

        public SubstrateTypesManager()
        {
            //_watcher = new FileWatcher(SubstrateTypes.SubstrateTypesPath);
            //_watcher.Changed += HandleSubstrateTypesChanged;
        }

        private void HandleSubstrateTypesChanged()
        {
            Refresh();
        }

        #endregion

        #region ISubstrateTypesManager

        public void Refresh()
        {
            //if (!AllowUpdates)
            //{
            //    return;
            //}

            var newTypes = SubstrateTypes.Load();
            //CurrentTypes = newTypes is null ? new SubstrateTypes() : newTypes;

            if (newTypes != null)
            {
                CurrentTypes = newTypes;
                NewTypesAvailable?.Invoke();
            }
        }

        #endregion

        #region IUpdate

        public void UpdateStatus()
        {
            //var now = DateTime.Now;

            //if (now > _nextUpdate)
            //{
            //    _nextUpdate = now.AddSeconds(RefreshTimeinSecs);
            //    Refresh();
            //}
        }

        #endregion

        #endregion
    }
}
