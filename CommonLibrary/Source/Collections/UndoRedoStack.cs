using System;
using CommonLibrary.Collections;

namespace CommonLibrary.Collections
{
    public class UndoRedoStack<T>
    {
        #region Data Members

        private readonly DropoutStack<T> _undo;
        private readonly DropoutStack<T> _redo;

        #endregion

        #region Properties

        public int Size { get; private set; }

        public int UndoCount => _undo.Count;
        public bool HasUndo => _undo.Count > 0;

        public int RedoCount => _redo.Count;
        public bool HasRedo => _redo.Count > 0;

        #endregion

        #region Functions

        #region Constructors

        public UndoRedoStack(int size)
        {
            if (size <= 0)
            {
                throw new ArgumentOutOfRangeException($"Stack size was set to {size} -- illegal!");
            }

            Size = size;
            _undo = new DropoutStack<T>(size);
            _redo = new DropoutStack<T>(size);

            Reset();
        }

        #endregion

        #region Public Functions

        public void Reset()
        {
            _undo.Clear();
            _redo.Clear();
        }

        public void Do(T input)
        {
            // Push To the undo stack, making room as required
            _undo.Push(input);
            // If we issue a new action (not a re-do), then the re-do stack is now obsolete
            _redo.Clear();
        }

        public bool Undo(T currentValue, out T undoValue)
        {
            if (_undo.Count > 0)
            {
                undoValue = _undo.Pop();
                _redo.Push(currentValue);
                return true;
            }
            else
            {
                undoValue = default;
                return false;
            }
        }

        public bool Redo(T currentValue, out T redoValue)
        {
            if (_redo.Count > 0)
            {
                redoValue = _redo.Pop();
                _undo.Push(currentValue);
                return true;
            }
            else
            {
                redoValue = default;
                return false;
            }
        }

        #endregion

        #endregion
    }
}
