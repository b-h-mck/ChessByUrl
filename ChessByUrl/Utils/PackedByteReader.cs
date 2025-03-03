namespace ChessByUrl.Utils
{
    public class PackedByteReader
    {
        public PackedByteReader(byte[] bytes)
        {
            Bytes = bytes;
        }

        public PackedByteReader(string base64)
        {
            base64 = base64.Replace('-', '+').Replace('_', '/');
            int paddingNeeded = 4 - (base64.Length % 4);
            if (paddingNeeded < 4)
            {
                base64 = base64.PadRight(base64.Length + paddingNeeded, '=');
            }
            Convert.TryFromBase64Chars(base64, new Span<byte>(new byte[base64.Length]), out int bytesWritten);
            Bytes = Convert.FromBase64String(base64);
        }

        public byte[] Bytes { get; }
        public int ByteIndex { get; private set; } = 0;
        public int BitIndex { get; private set; } = 0;


        public int? Read(int minValue, int maxValue)
        {
            if (minValue >= maxValue)
            {
                throw new ArgumentException("minValue must be less than maxValue");
            }
            int range = maxValue - minValue;
            int numberOfBits = (int)Math.Ceiling(Math.Log2(range + 1));

            int value = 0;
            while (numberOfBits > 0)
            {
                if (BitIndex == 0)
                {
                    if (ByteIndex >= Bytes.Length)
                    {
                        return null;
                    }
                }
                int bitsToRead = Math.Min(8 - BitIndex, numberOfBits);
                int mask = (1 << bitsToRead) - 1;
                value <<= bitsToRead;
                value |= (Bytes[ByteIndex] >> BitIndex) & mask;
                BitIndex += bitsToRead;
                if (BitIndex == 8)
                {
                    BitIndex = 0;
                    ByteIndex++;
                }
                numberOfBits -= bitsToRead;
            }
            return value + minValue;
        }
    }
}
