using System;

namespace Ordina.Unite.Api
{
    public class PartitionKeyGenerator
    {
        private static readonly Random rnd = new Random(DateTime.UtcNow.Second);

        public static Int64 GetPartitionKey()
        {
            byte[] buf = new byte[8];
            rnd.NextBytes(buf);
            long longRand = BitConverter.ToInt64(buf, 0);
            return longRand;
        }
    }
}
