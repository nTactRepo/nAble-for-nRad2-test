using System;

namespace CommonLibrary.Collections
{
    public class QueueEmptyException : Exception
    {
        public QueueEmptyException() : base() { }
        public QueueEmptyException(string msg) : base(msg) { }
    }
}
