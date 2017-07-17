using System;

namespace Fissoft.LinqIndex
{

    public class ObservableIndexNotFoundException : Exception
    {
        public ObservableIndexNotFoundException()
        {
        }

        public ObservableIndexNotFoundException(string message)
            : base(message)
        {
        }

        public ObservableIndexNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

}