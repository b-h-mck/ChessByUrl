using ChessByUrl.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessByUrl.Tests.Utils
{
    [TestClass]
    public class PackedByteTests
    {
        [TestMethod]
        [DataRow("Single 8-bit", new[] { 0 }, 0, 255, new byte[] { 0 })]
        [DataRow("Single 8-bit", new[] { 123 }, 0, 255, new byte[] { 123 })]
        [DataRow("Single 8-bit", new[] { 255 }, 0, 255, new byte[] { 255 })]
        [DataRow("Single 8-bit reduced range", new[] { 37 }, 0, 217, new byte[] { 37 })]
        [DataRow("Single 8-bit offset range", new[] { 260 }, 230, 430, new byte[] { 30 })]
        [DataRow("Single 1-bit", new[] { 0 }, 0, 1, new byte[] { 0 })]
        [DataRow("Single 1-bit", new[] { 1 }, 0, 1, new byte[] { 1 })]
        [DataRow("Single 7-bit", new[] { 76 }, 0, 120, new byte[] { 76 })]

        [DataRow("Single 9-bit", new[] { 0 }, 0, 511, new byte[] { 0, 0 })]
        [DataRow("Single 9-bit", new[] { 1 }, 0, 511, new byte[] { 0, 1 })]
        [DataRow("Single 9-bit", new[] { 2 }, 0, 511, new byte[] { 1, 0 })]
        [DataRow("Single 9-bit reduced range", new[] { 127 }, 0, 256, new byte[] { 63, 1 })]
        [DataRow("Single 9-bit reduced range", new[] { 128 }, 0, 500, new byte[] { 64, 0 })]
        [DataRow("Single 9-bit", new[] { 255 }, 0, 511, new byte[] { 127, 1 })]
        [DataRow("Single 9-bit", new[] { 256 }, 0, 511, new byte[] { 128, 0 })]
        [DataRow("Single 9-bit", new[] { 511 }, 0, 511, new byte[] { 255, 1 })]

        [DataRow("Double 1-bit", new[] { 0, 0 }, 0, 1, new byte[] { 0 })]
        [DataRow("Double 1-bit", new[] { 1, 0 }, 0, 1, new byte[] { 1 })]
        [DataRow("Double 1-bit", new[] { 0, 1 }, 0, 1, new byte[] { 2 })]
        [DataRow("Double 1-bit", new[] { 1, 1 }, 0, 1, new byte[] { 3 })]

        [DataRow("Double 4-bit", new[] { 0, 0 }, 0, 15, new byte[] { 0 })]
        [DataRow("Double 4-bit", new[] { 1, 0 }, 0, 15, new byte[] { 1 })]
        [DataRow("Double 4-bit", new[] { 15, 0 }, 0, 15, new byte[] { 15 })]
        [DataRow("Double 4-bit reduced range", new[] { 0, 1 }, 0, 8, new byte[] { 16 })]
        [DataRow("Double 4-bit reduced offset range", new[] { 2, 2 }, 1, 14, new byte[] { 17 })]

        [DataRow("Double 6-bit", new[] { 0, 0 }, 0, 63, new byte[] { 0, 0 })]
        [DataRow("Triple 5-bit", new[] { 0, 0, 0 }, 0, 31, new byte[] { 0, 0 })]

        public void Write(string caseDescription, int[] valuesToWrite, int min, int max, byte[] expectedBytes)
        {
            var writer = new PackedByteWriter();
            foreach (var value in valuesToWrite)
            {
                writer.Write(value, min, max);
            }
            CollectionAssert.AreEqual(expectedBytes, writer.Bytes, $"Case: {caseDescription} [{string.Join(",", valuesToWrite)}] ({min}-{max})");
        }

        [TestMethod]
        [DataRow("Single 8-bit", new byte[] { 0 }, 0, 255, new[] { 0 })]
        [DataRow("Single 8-bit", new byte[] { 123 }, 0, 255, new[] { 123 })]
        [DataRow("Single 8-bit", new byte[] { 255 }, 0, 255, new[] { 255 })]
        [DataRow("Single 8-bit reduced range", new byte[] { 37 }, 0, 217, new[] { 37 })]
        [DataRow("Single 8-bit offset range", new byte[] { 30 }, 230, 430, new[] { 260 })]
        [DataRow("Single 1-bit", new byte[] { 0 }, 0, 1, new[] { 0 })]
        [DataRow("Single 1-bit", new byte[] { 1 }, 0, 1, new[] { 1 })]
        [DataRow("Single 7-bit", new byte[] { 76 }, 0, 120, new[] { 76 })]

        [DataRow("Single 9-bit", new byte[] { 0, 0 }, 0, 511, new[] { 0 })]
        [DataRow("Single 9-bit", new byte[] { 0, 1 }, 0, 511, new[] { 1 })]
        [DataRow("Single 9-bit", new byte[] { 1, 0 }, 0, 511, new[] { 2 })]
        [DataRow("Single 9-bit reduced range", new byte[] { 63, 1 }, 0, 256, new[] { 127 })]
        [DataRow("Single 9-bit reduced range", new byte[] { 64, 0 }, 0, 500, new[] { 128 })]
        [DataRow("Single 9-bit", new byte[] { 127, 1 }, 0, 511, new[] { 255 })]
        [DataRow("Single 9-bit", new byte[] { 128, 0 }, 0, 511, new[] { 256 })]
        [DataRow("Single 9-bit", new byte[] { 255, 1 }, 0, 511, new[] { 511 })]

        [DataRow("Double 1-bit", new byte[] { 0 }, 0, 1, new[] { 0, 0 })]
        [DataRow("Double 1-bit", new byte[] { 1 }, 0, 1, new[] { 1, 0 })]
        [DataRow("Double 1-bit", new byte[] { 2 }, 0, 1, new[] { 0, 1 })]
        [DataRow("Double 1-bit", new byte[] { 3 }, 0, 1, new[] { 1, 1 })]

        [DataRow("Double 4-bit", new byte[] { 0 }, 0, 15, new[] { 0, 0 })]
        [DataRow("Double 4-bit", new byte[] { 1 }, 0, 15, new[] { 1, 0 })]
        [DataRow("Double 4-bit", new byte[] { 15 }, 0, 15, new[] { 15, 0 })]
        [DataRow("Double 4-bit reduced range", new byte[] { 16 }, 0, 8, new[] { 0, 1 })]
        [DataRow("Double 4-bit reduced offset range", new byte[] { 17 }, 1, 14, new[] { 2, 2 })]

        [DataRow("Double 6-bit", new byte[] { 0, 0 }, 0, 60, new[] { 0, 0 })]
        public void Read(string caseDescription, byte[] bytes, int min, int max, int[] expectedValues)
        {
            var reader = new PackedByteReader(bytes);
            var actualValues = new List<int>();
            for (int i = 0; i < expectedValues.Length; i++)
            {
                actualValues.Add(reader.Read(min, max)!.Value);
            }
            CollectionAssert.AreEqual(expectedValues, actualValues, $"Case: {caseDescription} [{string.Join(",", expectedValues)}] ({min}-{max})");
        }

        [TestMethod]
        [DataRow("Single 8-bit", new[] { 0 }, 0, 255)]
        [DataRow("Single 8-bit", new[] { 123 }, 0, 255)]
        [DataRow("Single 8-bit", new[] { 255 }, 0, 255)]
        [DataRow("Single 8-bit reduced range", new[] { 37 }, 0, 217)]
        [DataRow("Single 8-bit offset range", new[] { 260 }, 230, 430)]
        [DataRow("Single 1-bit", new[] { 0 }, 0, 1)]
        [DataRow("Single 1-bit", new[] { 1 }, 0, 1)]
        [DataRow("Single 7-bit", new[] { 76 }, 0, 120)]
        [DataRow("Single 9-bit", new[] { 0 }, 0, 511)]
        [DataRow("Single 9-bit", new[] { 1 }, 0, 511)]
        [DataRow("Single 9-bit", new[] { 2 }, 0, 511)]
        [DataRow("Single 9-bit reduced range", new[] { 127 }, 0, 256)]
        [DataRow("Single 9-bit reduced range", new[] { 128 }, 0, 500)]
        [DataRow("Single 9-bit", new[] { 255 }, 0, 511)]
        [DataRow("Single 9-bit", new[] { 256 }, 0, 511)]
        [DataRow("Single 9-bit", new[] { 511 }, 0, 511)]
        [DataRow("Double 1-bit", new[] { 0, 0 }, 0, 1)]
        [DataRow("Double 1-bit", new[] { 1, 0 }, 0, 1)]
        [DataRow("Double 1-bit", new[] { 0, 1 }, 0, 1)]
        [DataRow("Double 1-bit", new[] { 1, 1 }, 0, 1)]
        [DataRow("Double 4-bit", new[] { 0, 0 }, 0, 15)]
        [DataRow("Double 4-bit", new[] { 1, 0 }, 0, 15)]
        [DataRow("Double 4-bit", new[] { 15, 0 }, 0, 15)]
        [DataRow("Double 4-bit reduced range", new[] { 0, 1 }, 0, 8)]
        [DataRow("Double 4-bit reduced offset range", new[] { 2, 2 }, 1, 14)]
        [DataRow("Double 6-bit", new[] { 0, 0 }, 0, 60)]
        [DataRow("Triple 5-bit", new[] { 0, 0, 0 }, 0, 31)]
        [DataRow("Triple 5-bit offset range", new[] { 18, 7, 15 }, 3, 34)]
        public void RoundTrip(string caseDescription, int[] valuesToWrite, int min, int max)
        {
            var writer = new PackedByteWriter();
            foreach (var value in valuesToWrite)
            {
                writer.Write(value, min, max);
            }
            var bytes = writer.Bytes;

            var reader = new PackedByteReader(bytes);
            var actualValues = new List<int>();
            for (int i = 0; i < valuesToWrite.Length; i++)
            {
                actualValues.Add(reader.Read(min, max)!.Value);
            }
            CollectionAssert.AreEqual(valuesToWrite, actualValues, $"Case: {caseDescription} [{string.Join(",", valuesToWrite)}] ({min}-{max})");
        }

        [TestMethod]
        [DataRow("Mixed ranges", new[] { 0, 123, 255, 37, 260, 1, 76 }, new[] { 0, 0, 0, 0, 230, 0, 0 }, new[] { 255, 255, 255, 217, 430, 1, 120 })]
        [DataRow("Mixed ranges", new[] { 511, 1, 0, 15, 2, 0, 60 }, new[] { 0, 0, 0, 0, 1, 0, 0 }, new[] { 511, 1, 1, 15, 14, 1, 60 })]
        public void ComplexRoundTrip(string caseDescription, int[] valuesToWrite, int[] mins, int[] maxs)
        {
            var writer = new PackedByteWriter();
            for (int i = 0; i < valuesToWrite.Length; i++)
            {
                writer.Write(valuesToWrite[i], mins[i], maxs[i]);
            }
            var bytes = writer.Bytes;

            var reader = new PackedByteReader(bytes);
            var actualValues = new List<int>();
            for (int i = 0; i < valuesToWrite.Length; i++)
            {
                actualValues.Add(reader.Read(mins[i], maxs[i])!.Value);
            }
            CollectionAssert.AreEqual(valuesToWrite, actualValues, $"Case: {caseDescription} [{string.Join(",", valuesToWrite)}] (mins: {string.Join(",", mins)}, maxs: {string.Join(",", maxs)})");
        }


    }
}
