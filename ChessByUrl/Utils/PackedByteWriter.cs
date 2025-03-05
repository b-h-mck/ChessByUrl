namespace ChessByUrl.Utils
{
    /// <summary>
    /// Writes a sequence of integers to a byte array, packing them as tightly as possible.
    /// </summary>
    /// <remarks>
    /// This reserves the first three bits of the first byte for the final bit index, so that 
    /// we can read the sequence back without needing to know the number of reads in advance.
    /// </remarks>
    public class PackedByteWriter
    {
        public PackedByteWriter()
        {
        }

        private byte[] _bytes = [3];
        public byte[] Bytes { get { return _bytes; } }
        public int ByteIndex { get; private set; } = 0;
        public int BitIndex { get; private set; } = 3; // Reserve 3 bits

        public string ToBase64()
        {
            var base64 = Convert.ToBase64String(Bytes);
            return base64.Replace('+', '-').Replace('/', '_').TrimEnd('=');
        }

        public void Write(int value, int minValue, int maxValue)
        {
            if (minValue >= maxValue)
            {
                throw new ArgumentException("minValue must be less than maxValue");
            }
            if (value < minValue || value > maxValue)
            {
                throw new ArgumentException("value must be between minValue and maxValue");
            }

            int range = maxValue - minValue;
            int valueToWrite = value - minValue;
            int numberOfBits = (int)Math.Ceiling(Math.Log2(range + 1));

            while (numberOfBits > 0)
            {
                if (BitIndex == 0 && ByteIndex >= Bytes.Length)
                {
                    Array.Resize(ref _bytes, _bytes.Length + 1);
                }

                int bitsAvailableInCurrentByte = 8 - BitIndex;
                int bitsToWrite = Math.Min(bitsAvailableInCurrentByte, numberOfBits);

                byte partialByte = GetPartialByte(valueToWrite, numberOfBits - bitsToWrite, bitsToWrite);
                Bytes[ByteIndex] |= (byte)(partialByte << BitIndex);

                BitIndex += bitsToWrite;
                if (BitIndex == 8)
                {
                    BitIndex = 0;
                    ByteIndex++;
                }

                numberOfBits -= bitsToWrite;
            }

            // Write the final bit index
            _bytes[0] = (byte)((_bytes[0] & 0b11111000) | BitIndex);
        }

        private byte GetPartialByte(int valueToWrite, int bitStartIndex, int bitCount)
        {
            int mask = (1 << bitCount) - 1;
            int shiftedValue = valueToWrite >> bitStartIndex;
            int maskedValue = shiftedValue & mask;
            return (byte)maskedValue;
        }
    }
}
