using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RetryHelper.Tests
{
    [TestClass]
    public class RetryTests
    {
        [TestMethod]
        public void TestFailureRetries()
        {
            int attemptCount = 0;
            int retryRequest = 3;

            Retry.Attempt(retryRequest,
                () =>
                {
                    attemptCount++;
                    return false; // This function fails every time, so the helper will retry it.
                });

            Assert.IsTrue(attemptCount == retryRequest, "Attempt count should match retry request.");
        }

        [TestMethod]
        public void TestSuccessRetries()
        {
            int attemptCount = 0;
            int retryRequest = 3;

            Retry.Attempt(retryRequest,
                () =>
                {
                    attemptCount++;
                    return true; // This function succeeds, so should not be retried;
                });

            Assert.IsTrue(attemptCount == 1, "Attempt count should only be one as the function succeeded.");
        }

        [TestMethod]
        public void TestFailureExceptionHandled()
        {
            int attemptCount = 0;
            int exceptionHandledCount = 0;
            int retryRequest = 3;

            Retry.Attempt(retryRequest,
                () =>
                {
                    attemptCount++;
                    throw new InvalidOperationException();
                },
                UnhandledException.Throw,
                new RetryExceptionHandler<InvalidOperationException>(
                    (ex) =>
                    {
                        exceptionHandledCount++;
                        return RetryRequired.DoRetry;
                    }));

            Assert.IsTrue(attemptCount == retryRequest, "Attempt count should match retry request.");
            Assert.IsTrue(exceptionHandledCount == retryRequest, "Attempt count should match retry request.");
        }

        [TestMethod]
        public void TestFailureExceptionHandlerMatched()
        {
            bool handled = false;
            int retryRequest = 3;

            Retry.Attempt(retryRequest,
                () =>
                {
                    throw new InvalidOperationException();
                },
                UnhandledException.Throw,
                new RetryExceptionHandler<InvalidOperationException>(
                    (ex) =>
                    {
                        handled = true;
                        return RetryRequired.NotRetry;
                    }));

            Assert.IsTrue(handled, "Exception types match, should be handled");
        }

        [TestMethod]
        public void TestFailureExceptionHandlerNotMatched()
        {
            bool handled = false;
            int retryRequest = 3;

            Retry.Attempt(retryRequest,
                () =>
                {
                    throw new InvalidOperationException();
                },
                UnhandledException.TreatAsFunctionSuccess_NotRetry,
                new RetryExceptionHandler<ArgumentNullException>(
                    (ex) =>
                    {
                        handled = true;
                        return RetryRequired.NotRetry;
                    }));

            Assert.IsFalse(handled, "Exception types don't match, should not be handled");
        }

        [TestMethod]
        public void TestFailureExceptionHandlerSuperclass()
        {
            bool handled = false;
            int retryRequest = 3;

            Retry.Attempt(retryRequest,
                () =>
                {
                    throw new InvalidOperationException();
                },
                UnhandledException.Throw,
                new RetryExceptionHandler<Exception>(
                    (ex) =>
                    {
                        handled = true;
                        return RetryRequired.NotRetry;
                    }));

            Assert.IsTrue(handled, "Exception handler type is superclass of exception thrown, should be handled");
        }

        [TestMethod]
        public void TestFailureExceptionHandlerSubclass()
        {
            bool handled = false;
            int retryRequest = 3;

            Retry.Attempt(retryRequest,
                () =>
                {
                    throw new Exception();
                },
                UnhandledException.TreatAsFunctionSuccess_NotRetry,
                new RetryExceptionHandler<InvalidOperationException>(
                    (ex) =>
                    {
                        handled = true;
                        return RetryRequired.NotRetry;
                    }));

            Assert.IsFalse(handled, "Exception handler type is subclass of exception thrown, should not be handled");
        }

        [TestMethod]
        public void TestFailureExceptionHandlerChainedMatchFirst()
        {
            bool handledByFirst = false;
            bool handledBySecond = false;
            int retryRequest = 3;

            Retry.Attempt(retryRequest,
                () =>
                {
                    throw new InvalidOperationException(); // Matches first handler.
                },
                UnhandledException.Throw,
                new RetryExceptionHandler<InvalidOperationException>(
                    (ex) =>
                    {
                        handledByFirst = true;
                        return RetryRequired.NotRetry;
                    }),
                new RetryExceptionHandler<Exception>(
                    (ex) =>
                    {
                        handledBySecond = true;
                        return RetryRequired.NotRetry;
                    })
                );

            Assert.IsTrue(handledByFirst, "Exception matches first handler, first should have handled");
            Assert.IsFalse(handledBySecond, "Exception matches first handler, second should not have handled");
        }

        [TestMethod]
        public void TestFailureExceptionHandlerChainedMatchSecond()
        {
            bool handledByFirst = false;
            bool handledBySecond = false;
            int retryRequest = 3;

            Retry.Attempt(retryRequest,
                () =>
                {
                    throw new Exception(); // Matches second handler.
                },
                UnhandledException.Throw,
                new RetryExceptionHandler<InvalidOperationException>(
                    (ex) =>
                    {
                        handledByFirst = true;
                        return RetryRequired.NotRetry;
                    }),
                new RetryExceptionHandler<Exception>(
                    (ex) =>
                    {
                        handledBySecond = true;
                        return RetryRequired.NotRetry;
                    })
                );

            Assert.IsFalse(handledByFirst, "Exception matches second handler, first should not have handled");
            Assert.IsTrue(handledBySecond, "Exception matches second handler, second should have handled");
        }


        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestFailureExceptionUnHandled()
        {
            int retryRequest = 3;

            Retry.Attempt(retryRequest,
                () =>
                {
                    throw new InvalidOperationException();
                    return RetryRequired.NotRetry;
                });
        }
    }
}
