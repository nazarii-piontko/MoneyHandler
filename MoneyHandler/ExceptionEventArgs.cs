using System;

namespace MoneyHandler
{
    public class ExceptionEventArgs : EventArgs
    {
        private readonly Exception _exception;

        public ExceptionEventArgs(Exception exception)
        {
            _exception = exception;
        }

        public Exception Exception1
        {
            get { return _exception; }
        }
    }
}