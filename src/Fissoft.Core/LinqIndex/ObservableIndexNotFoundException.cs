using System;
namespace Fissoft.Framework.Systems.Data
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