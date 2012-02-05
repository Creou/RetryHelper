using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RetryHelper
{
    public enum UnhandledException
    {
        Throw = 1,
        TreatAsFunctionFailure_DoRetry = 2,
        TreatAsFunctionSuccess_NotRetry = 3
    }
}
