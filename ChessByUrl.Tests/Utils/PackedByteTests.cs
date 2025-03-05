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

        public TestContext TestContext { get; set; }


        [TestMethod]
        [DataRow("01: Single 8-bit", new[] { 0 }, 0, 255)]
        [DataRow("02: Single 8-bit", new[] { 123 }, 0, 255)]
        [DataRow("03: Single 8-bit", new[] { 255 }, 0, 255)]
        [DataRow("04: Single 8-bit reduced range", new[] { 37 }, 0, 217)]
        [DataRow("05: Single 8-bit offset range", new[] { 260 }, 230, 430)]
        [DataRow("06: Single 1-bit", new[] { 0 }, 0, 1)]
        [DataRow("07: Single 1-bit", new[] { 1 }, 0, 1)]
        [DataRow("08: Single 7-bit", new[] { 76 }, 0, 120)]
        [DataRow("09: Single 9-bit", new[] { 0 }, 0, 511)]
        [DataRow("10: Single 9-bit", new[] { 1 }, 0, 511)]
        [DataRow("11: Single 9-bit", new[] { 2 }, 0, 511)]
        [DataRow("12: Single 9-bit reduced range", new[] { 127 }, 0, 256)]
        [DataRow("13: Single 9-bit reduced range", new[] { 128 }, 0, 500)]
        [DataRow("14: Single 9-bit", new[] { 255 }, 0, 511)]
        [DataRow("15: Single 9-bit", new[] { 256 }, 0, 511)]
        [DataRow("16: Single 9-bit", new[] { 511 }, 0, 511)]
        [DataRow("17: Double 1-bit", new[] { 0, 0 }, 0, 1)]
        [DataRow("18: Double 1-bit", new[] { 1, 0 }, 0, 1)]
        [DataRow("19: Double 1-bit", new[] { 0, 1 }, 0, 1)]
        [DataRow("20: Double 1-bit", new[] { 1, 1 }, 0, 1)]
        [DataRow("21: Double 4-bit", new[] { 0, 0 }, 0, 15)]
        [DataRow("22: Double 4-bit", new[] { 1, 0 }, 0, 15)]
        [DataRow("23: Double 4-bit", new[] { 15, 0 }, 0, 15)]
        [DataRow("24: Double 4-bit reduced range", new[] { 0, 1 }, 0, 8)]
        [DataRow("25: Double 4-bit reduced offset range", new[] { 2, 2 }, 1, 14)]
        [DataRow("26: Double 6-bit", new[] { 0, 0 }, 0, 60)]
        [DataRow("27: Triple 5-bit", new[] { 0, 0, 0 }, 0, 31)]
        [DataRow("28: Triple 5-bit offset range", new[] { 18, 7, 15 }, 3, 34)]
        [DataRow("29: Single 4-bit", new[] { 11 }, 0, 15)]
        [DataRow("30: Single 4-bit reduced range", new[] { 11 }, 0, 14)]
        [DataRow("31: Single 5-bit", new[] { 0 }, 0, 31)]
        [DataRow("32: Single 5-bit", new[] { 14 }, 0, 31)]
        [DataRow("33: Single 5-bit", new[] { 31 }, 0, 31)]
        [DataRow("34: Single 5-bit reduced range", new[] { 14 }, 0, 20)]
        [DataRow("34: Single 13-bit", new[] { 14 }, 0, 8191)]
        public void RoundTrip(string caseDescription, int[] valuesToWrite, int min, int max)
        {
            var writer = new PackedByteWriter();
            foreach (var value in valuesToWrite)
            {
                writer.Write(value, min, max);
            }
            var base64 = writer.ToBase64();
            TestContext.WriteLine($"{caseDescription}: {base64}");

            var reader = new PackedByteReader(base64);
            var actualValues = new List<int>();
            var readValue = reader.Read(min, max);
            while (readValue != null)
            {
                actualValues.Add(readValue!.Value);
                readValue = reader.Read(min, max);
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

            var base64 = writer.ToBase64();
            TestContext.WriteLine($"{caseDescription}: {base64}");

            var reader = new PackedByteReader(bytes);
            var actualValues = new List<int>();

            var readIndex = 0;
            var readValue = reader.Read(mins[0], maxs[0]);
            while (readValue != null)
            {
                actualValues.Add(readValue!.Value);
                readIndex++;
                if (readIndex >= mins.Length || readIndex >= maxs.Length)
                {
                    break;
                }
                readValue = reader.Read(mins[readIndex], maxs[readIndex]);
            }
            CollectionAssert.AreEqual(valuesToWrite, actualValues, $"Case: {caseDescription} [{string.Join(",", valuesToWrite)}] (mins: {string.Join(",", mins)}, maxs: {string.Join(",", maxs)})");
        }


    }
}
