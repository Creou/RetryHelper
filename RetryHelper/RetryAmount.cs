using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RetryHelper
{
    public class RetryAmount
    {
        public static readonly RetryAmount Infinite = new RetryAmount();

        public RetryAmount(int count)
        {
            Count = count;
        }

        private RetryAmount()
        {
            this.IsInfinite = true;
        }

        public bool IsInfinite { get; private set; }
        public int Count { get; private set; }
    }
}
