using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary.Utils
{
    public class FileWatcher
    {
        #region Events

        public event Action Changed;

        #endregion

        #region Constants

        #endregion

        #region Properties

        public string FilePath { get; }

        public bool Enabled { get; set; } = true;

        #endregion

        #region Member Data

        private FileSystemWatcher _watcher = null;

        private string _directory = "";
        private string _file = "";

        #endregion

        #region Functions

        #region Constructors

        public FileWatcher(string filePath)
        {
            FilePath = filePath;
            _directory = Path.GetDirectoryName(filePath);

            if (string.IsNullOrEmpty(_directory))
            {
                throw new ArgumentException($"FileWatcher was set to watch a file in a non-existent directory:  {_directory}");
            }

            _file = Path.GetFileName(filePath);

            if (string.IsNullOrEmpty(_file))
            {
                throw new ArgumentException($"FileWatcher was set to watch a non-existent file:  {_file}");
            }

            CreateWatcher();
        }

        #endregion

        #region Event Handlers

        private void HandleWatcherRenamed(object sender, RenamedEventArgs e)
        {
            TellChanged();
        }

        private void HandleWatcherChanged(object sender, FileSystemEventArgs e)
        {
            TellChanged();
        }

        #endregion

        #region Private Functions

        private void CreateWatcher()
        {
            if (_watcher != null)
            {
                _watcher.Dispose();
            }

            _watcher = new FileSystemWatcher(_directory, _file)
            {
                IncludeSubdirectories = false,
                EnableRaisingEvents = true
            };

            _watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.Size;

            _watcher.Changed += HandleWatcherChanged;
            _watcher.Created += HandleWatcherChanged;
            _watcher.Deleted += HandleWatcherChanged;
            _watcher.Renamed += HandleWatcherRenamed;
        }

        private void TellChanged()
        {
            if (Enabled)
            {
                Changed?.Invoke();
            }
        }

        #endregion

        #endregion
    }
}
