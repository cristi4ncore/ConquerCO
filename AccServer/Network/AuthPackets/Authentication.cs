using System;
using System.IO;
using System.Text;

namespace AccServer.Network.AuthPackets
{
    public unsafe class LoaderEncryption
    {
        private static byte[] Key = { 89, 80, 40, 35, 33, 90, 44, 15, 60, 91, 23, 46, 40, 54, 99, 33, 142, 4, 19, 113, 106, 59, 215, 210, 169, 232, 121, 79, 241, 95, 193, 149 };
        private static byte[] Key2 = { 70, 28, 28, 59, 10, 18, 18, 13, 154, 52, 11, 31, 84, 99, 83, 190, 57, 26, 89, 218, 121, 217, 232, 36, 55, 224, 48, 169, 254, 163, 166, 43 };

        public static void Decrypt(byte[] arr, ushort len)
        {
            for (int i = 0; i < len; ++i)
            {
                arr[i] ^= Key[(44 * i) & 28];
                arr[i] ^= Key2[(99 * i) & 31];
            }
        }
    }
    public unsafe class Authentication : Interfaces.IPacket
    {
        public ushort Length;//0
        public ushort ID;//2
        public string Username;//4
        public string Password;//36
        public string Server;//68
        public string Mac;
        public Authentication()
        {

        }
        public void Deserialize(byte[] buffer)
        {
            try
            {
                ushort Length = BitConverter.ToUInt16(buffer, 0);
                ushort ID = BitConverter.ToUInt16(buffer, 2);
                if (ID == 1542)
                {
                    MemoryStream MS = new MemoryStream(buffer);
                    BinaryReader BR = new BinaryReader(MS);

                    BR.ReadUInt16();
                    BR.ReadUInt16();

                    Username = Encoding.Default.GetString(BR.ReadBytes(32));
                    Username = Username.Replace("\0", "");

                    BR.ReadBytes(36);

                    byte PassLen = BR.ReadByte();
                    var PasswordArray = BR.ReadBytes(32);
                    LoaderEncryption.Decrypt(PasswordArray, PassLen);
                    Password = Encoding.Default.GetString(PasswordArray);
                    Password = Password.Replace("\0", "");

                    BR.ReadBytes(31);

                    Server = Encoding.Default.GetString(BR.ReadBytes(16));
                    Server = Server.Replace("\0", "");

                    Mac = Encoding.Default.GetString(BR.ReadBytes(16));
                    Mac = Mac.Replace("\0", "");

                    BR.Close();
                    MS.Close();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        public static string Decrypt(byte[] data)
        {
            return Encoding.Default.GetString(data);
        }
        public byte[] ToArray()
        {
            return null;
        }
    }
}