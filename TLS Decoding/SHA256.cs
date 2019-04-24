using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLS_Decoding
{
    class SHA256
    {
        private static readonly uint[] PRIME_ROOT_BITS = new uint[]
        {
            0x6a09e667,
            0xbb67ae85,
            0x3c6ef372,
            0xa54ff53a,
            0x510e527f,
            0x9b05688c,
            0x1f83d9ab,
            0x5be0cd19
        };

        private static readonly uint[] PRIME_CUBE_ROOT_BITS = new uint[]
        {
            0x428a2f98, 0x71374491, 0xb5c0fbcf, 0xe9b5dba5, 0x3956c25b, 0x59f111f1, 0x923f82a4, 0xab1c5ed5,
            0xd807aa98, 0x12835b01, 0x243185be, 0x550c7dc3, 0x72be5d74, 0x80deb1fe, 0x9bdc06a7, 0xc19bf174,
            0xe49b69c1, 0xefbe4786, 0x0fc19dc6, 0x240ca1cc, 0x2de92c6f, 0x4a7484aa, 0x5cb0a9dc, 0x76f988da,
            0x983e5152, 0xa831c66d, 0xb00327c8, 0xbf597fc7, 0xc6e00bf3, 0xd5a79147, 0x06ca6351, 0x14292967,
            0x27b70a85, 0x2e1b2138, 0x4d2c6dfc, 0x53380d13, 0x650a7354, 0x766a0abb, 0x81c2c92e, 0x92722c85,
            0xa2bfe8a1, 0xa81a664b, 0xc24b8b70, 0xc76c51a3, 0xd192e819, 0xd6990624, 0xf40e3585, 0x106aa070,
            0x19a4c116, 0x1e376c08, 0x2748774c, 0x34b0bcb5, 0x391c0cb3, 0x4ed8aa4a, 0x5b9cca4f, 0x682e6ff3,
            0x748f82ee, 0x78a5636f, 0x84c87814, 0x8cc70208, 0x90befffa, 0xa4506ceb, 0xbef9a3f7, 0xc67178f2
        };

        private static UInt32 ROTL(UInt32 x, byte n) => (x << n) | (x >> (32 - n));

        private static UInt32 ROTR(UInt32 x, byte n) => (x >> n) | (x << (32 - n));

        private static UInt32 Ch(UInt32 x, UInt32 y, UInt32 z) => (x & y) ^ ((~x) & z);

        private static UInt32 Maj(UInt32 x, UInt32 y, UInt32 z) => (x & y) ^ (x & z) ^ (y & z);

        private static UInt32 Sigma0(UInt32 x) => ROTR(x, 2) ^ ROTR(x, 13) ^ ROTR(x, 22);

        private static UInt32 Sigma1(UInt32 x) => ROTR(x, 6) ^ ROTR(x, 11) ^ ROTR(x, 25);

        private static UInt32 sigma0(UInt32 x) => ROTR(x, 7) ^ ROTR(x, 18) ^ (x >> 3);

        private static UInt32 sigma1(UInt32 x) => ROTR(x, 17) ^ ROTR(x, 19) ^ (x >> 10);

        public static byte[] Encrypt(byte[] bytes)
        {
            List<byte> encryptedBytes = new List<byte>(bytes);
            int length = bytes.Length;

            // Pre-processing (Padding)
            int k = (length * 8) + 65;
            while (k % 512 != 0)
            {
                ++k;
            }
            k -= length + 65;
            encryptedBytes.Add(0x80);
            encryptedBytes.AddRange(
                Enumerable.Repeat<byte>(
                    0x00, k / 8
                ).Concat(
                    BitConverter.GetBytes((long)length)
                )
            );

            uint[] intermediateHash = new uint[8];
            Array.Copy(PRIME_ROOT_BITS, intermediateHash, 8);

            //Process the message in successive 512-bit chunks
            for (int chunkStart = 0; chunkStart < length; chunkStart += 64)
            {
                uint[] messageSchedule = new uint[64];
                
                // Copy chunk into first 16 slots of array
                for (int bytePointer = chunkStart; bytePointer < chunkStart + 64; chunkStart += 4)
                {
                    messageSchedule[bytePointer] = BitConverter.ToUInt32(bytes, bytePointer);
                }

                for (int i = 16; i <= 63; ++i)
                {
                    messageSchedule[i] = sigma1(messageSchedule[i - 2]) + messageSchedule[i - 7] + sigma0(messageSchedule[i - 15]) + messageSchedule[i - 16];
                }

                uint a = intermediateHash[0],
                   b = intermediateHash[1],
                   c = intermediateHash[2],
                   d = intermediateHash[3],
                   e = intermediateHash[4],
                   f = intermediateHash[5],
                   g = intermediateHash[6],
                   h = intermediateHash[7];

                for (int i = 0; i < 64; ++i)
                {
                    uint temp1 = h + Sigma1(e) + Ch(e, f, g) + PRIME_CUBE_ROOT_BITS[t] + messageSchedule[t];
                    uint temp2 = Sigma0(a) + Maj(a, b, c);
                    h = g;
                    g = f;
                    f = e;
                    e = d + temp1;
                    d = c;
                    c = b;
                    b = a;
                    a = temp1 + temp2;
                }

                intermediateHash[0] += a;
                intermediateHash[1] += b;
                intermediateHash[2] += c;
                intermediateHash[3] += d;
                intermediateHash[4] += e;
                intermediateHash[5] += f;
                intermediateHash[6] += g;
                intermediateHash[7] += h;
            }
        }
    }
}
