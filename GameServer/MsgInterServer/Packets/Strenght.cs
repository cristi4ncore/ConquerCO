using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeathWish.MsgInterServer.Packets
{
    public static class Strenght
    {
        public static unsafe ServerSockets.Packet SentStrenght(this ServerSockets.Packet stream, uint Strenght1)
        {
            stream.InitWriter();
            stream.Write(Strenght1);
            stream.Finalize(Game.GamePackets.Strengh);
            return stream;
        }
        public static unsafe void GetStrenght(this ServerSockets.Packet stream, out uint Strenght1)
        {
            Strenght1 = stream.ReadUInt32();
        }
    }
}
