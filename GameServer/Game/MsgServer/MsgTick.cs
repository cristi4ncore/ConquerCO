using DeathWish.Role;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace DeathWish.Game.MsgServer
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct MsgInfo
    {
        public uint UniqId;
        public int Timestamp;
        public int Junk1;
        public int Junk2;
        public int Junk3;
        public int Junk4;
        public uint Hash;
    };
    public unsafe static class MsgTick
    {
        public static unsafe ServerSockets.Packet TickCreate(this ServerSockets.Packet stream, MsgInfo* pQuery)
        {

            stream.InitWriter();

            stream.WriteUnsafe(pQuery, sizeof(MsgInfo));
            stream.Finalize(GamePackets.MsgTick);

            return stream;
        }
        public static unsafe void GetTick(this ServerSockets.Packet stream, MsgInfo* pQuery)
        {
            stream.ReadUnsafe(pQuery, sizeof(MsgInfo));
        }
        public static unsafe ServerSockets.Packet CreateTickData(this ServerSockets.Packet stream, Player role)
        {
            MsgInfo Info = new MsgInfo()
            {
                UniqId = role.UID,
                Timestamp = 0x00,
                Junk1 =  MyMath.Generate(0x1FFFFFFF, 0x7FFFFFFF),
                Junk2 = MyMath.Generate(0x1FFFFFFF, 0x7FFFFFFF),
                Junk3 = MyMath.Generate(0x1FFFFFFF, 0x7FFFFFFF),
                Junk4 = MyMath.Generate(0x1FFFFFFF, 0x7FFFFFFF),
                Hash = 0x00,
            };
            return stream.TickCreate(&Info);
        }
        [PacketAttribute(GamePackets.MsgTick)]
        public static unsafe void TickHandler(Client.GameClient client, ServerSockets.Packet stream)
        {
#if msgtick
            MsgInfo Packet;
            stream.GetTick(&Packet);
            if (Packet.UniqId != client.Player.UID)
            {
                client.Socket.Disconnect();
                //disconnect
                return;
            }
            if (Packet.Hash != HashName(client.Player.Name))
            {
                client.Socket.Disconnect();
                //disconnect
                return;
            }
            int Timestamp = Packet.Timestamp ^ (int)Packet.UniqId;
            if (client.Player.LastClientTick == 0)
            {
                client.Player.LastClientTick = Timestamp;
            }
            if (client.Player.LastClientTick > Timestamp)
            {
                client.SendSysMesage("Your session has expired.");
                MyConsole.WriteLine("Your session has expired.");
                client.Socket.Disconnect();
                //disconnect
                return;
            }
            client.Player.LastClientTick = Timestamp;
            client.Player.LastRcvClientTick = Environment.TickCount;
#endif
        }
        private static uint HashName(string Name)
        {
            if (string.IsNullOrEmpty(Name) || Name.Length < 4)
                return 0x9D4B5703;
            else
            {
                byte[] Buffer = Encoding.Default.GetBytes(Name);
                fixed (byte* pBuf = Buffer)
                    return ((ushort*)pBuf)[0] ^ 0x9823U;
            }
        }
    }
}
