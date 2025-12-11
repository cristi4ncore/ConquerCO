using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeathWish.Cryptography
{
    public unsafe class WebServerEncryption
    {
        private static byte[] Key = { 92, 252, 229, 121, 110, 183, 197, 106, 230, 188, 31, 206, 185, 191, 195, 65, 156, 35, 220, 18, 255, 65, 200, 78, 35, 219, 217, 100, 226, 50, 124, 251, 152, 244, 191, 142, 171, 186, 41, 47 };
        public static void Encrypt(ref ServerSockets.Packet stream)
        {
            int length = Key.Length;
            for (int i = 0; i < stream.Size; i++)
            {
                stream.Memory[i] ^= Key[i % length];
                stream.Memory[i] ^= Key[(i + 1) % length];
            }
        }
        public static void Decrypt(ref ServerSockets.Packet stream)
        {
            int length = Key.Length;
            for (int i = 0; i < stream.Size; i++)
            {
                stream.Memory[i] ^= Key[(i + 1) % length];
                stream.Memory[i] ^= Key[i % length];
            }
        }
    }
}
