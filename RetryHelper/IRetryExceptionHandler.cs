using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RetryHelper
{
    public interface IRetryExceptionHandler
    {
        Type ExceptionType { get; set; }
        RetryRequired Handle(Exception ex);
    }
}
