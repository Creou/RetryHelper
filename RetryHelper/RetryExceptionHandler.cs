using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RetryHelper
{
    public enum RetryRequired
    {
        DoRetry = 1,
        NotRetry = 2
    }

    public class RetryExceptionHandler<T> : IRetryExceptionHandler where T : Exception
    {
        public Type ExceptionType { get; set; }
        private Func<T, RetryRequired> Handler { get; set; }

        public RetryExceptionHandler(Func<T, RetryRequired> handler)
        {
            this.Handler = handler;
            this.ExceptionType = typeof(T);
        }

        public RetryRequired Handle(T ex)
        {
            return this.Handler(ex);
        }

        public RetryRequired Handle(Exception ex)
        {
            return this.Handle((T)ex);
        }
    }
}
