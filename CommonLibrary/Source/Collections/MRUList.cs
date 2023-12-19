using CommonLibrary.Collections;
using CommonLibrary.Source.Utils;
using CommonLibrary.Source.Utils.Interfaces;
using System;
using System.Collections.Generic;

namespace CommonLibrary.Source.Collections
{
    public class MRUList<T> : ILoadSave
    {
        #region Events

        public event Action Changed;

        #endregion

        #region Constants

        public const int DefaultMaxNumItems = 10;

        #endregion

        #region Properties

        public int MaxNumItems { get; set; } = DefaultMaxNumItems;

        public List<T> MRUItems { get; set; } = new List<T>();

        public string Filename { get; set; } = "";

        #endregion

        #region Functions

        #region Constructors

        // Used mostly for XML serialization
        public MRUList() { }

        public MRUList(string listFileLocation)
        {
            Filename = listFileLocation;

            LoadMRUFile(Filename);
        }

        #endregion

        #region Public Functions

        public void UseItem(T item)
        {
            MRUItems.Remove(item);
            MRUItems.Insert(0, item);

            if (MRUItems.Count > MaxNumItems)
            {
                MRUItems.RemoveAt(MRUItems.Count - 1);
            }
            
            LoaderSaver<MRUList<T>>.Save(this, Filename);
            Changed?.Invoke();
        }

        public void Clear()
        {
            MRUItems.Clear();

            LoaderSaver<MRUList<T>>.Save(this, Filename);
            Changed?.Invoke();
        }

        #endregion

        #region Private Functions

        private bool LoadMRUFile(string filename)
        {
            bool loaded = false;

            try
            {
                var mruFile = LoaderSaver<MRUList<T>>.Load(filename);
                
                Filename = filename;
                MRUItems = mruFile.MRUItems;
                MaxNumItems = mruFile.MaxNumItems;

                loaded = true;
            }
            catch (Exception)
            {
            }

            return loaded;
        }

        #endregion

        #endregion
    }
}
