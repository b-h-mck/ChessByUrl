using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessByUrl.Tests
{
    public static class CollectionAssert
    {
        [DebuggerHidden]
        public static void AreEqual<T>(IEnumerable<T>? expected, IEnumerable<T>? actual, string? message = null)
        {
            if (expected == null && actual == null)
            {
                return;
            }
            else if (expected == null || actual == null || !expected.SequenceEqual(actual))
            {
                string expectedString = expected == null ? "null" : $"[{string.Join(",", expected)}]";
                string actualString = actual == null ? "null" : $"[{string.Join(",", actual)}]";
                Assert.Fail($"Expected: {expectedString}. Actual: {actualString}. {message}");
            }
        }
    }
}
