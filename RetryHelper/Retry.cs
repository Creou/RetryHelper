using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RetryHelper
{
    public static class Retry
    {
        public static void Attempt(int retries, Func<bool> retryFunction)
        {
            Attempt(retries, () => retryFunction() ? RetryRequired.NotRetry : RetryRequired.DoRetry);
        }

        public static void Attempt(int retries, Func<RetryRequired> retryFunction)
        {
            Attempt(retries, retryFunction, UnhandledException.Throw, null);
        }

        public static void Attempt(int retries, Func<RetryRequired> retryFunction, UnhandledException unhandledException, params IRetryExceptionHandler[] exceptionHandlers)
        {
            Attempt(new RetryAmount(retries), retryFunction, unhandledException, exceptionHandlers);
        }

        public static void Attempt(RetryAmount retries, Func<RetryRequired> retryFunction, UnhandledException unhandledException, params IRetryExceptionHandler[] exceptionHandlers)
        {
            int retryCount = 0;
            RetryRequired retry = RetryRequired.DoRetry;

            do
            {
                try
                {
                    retryCount++;
                    retry = retryFunction();
                }
                catch (Exception ex)
                {
                    bool handled = false;
                    if (exceptionHandlers != null)
                    {
                        var handler = exceptionHandlers.FirstOrDefault(h => h.ExceptionType.IsAssignableFrom(ex.GetType()));
                        if (handler != null)
                        {
                            handled = true;
                            retry = handler.Handle(ex);
                        }
                    }

                    if(!handled)
                    {
                        switch (unhandledException)
                        {
                            default:
                            case UnhandledException.Throw:
                                throw;

                            case UnhandledException.TreatAsFunctionFailure_DoRetry:
                                retry = RetryRequired.DoRetry;
                                break;

                            case UnhandledException.TreatAsFunctionSuccess_NotRetry:
                                retry = RetryRequired.NotRetry;
                                break;
                        }
                    }
                }
            }
            while (retry == RetryRequired.DoRetry && (retries.IsInfinite || retryCount < retries.Count));
        }
    }
}
