using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeathWish.MsgInterServer.Packets
{
    public static class sprit
    {
        public static unsafe ServerSockets.Packet Sentsprit1(this ServerSockets.Packet stream, uint sprit1)
        {
            stream.InitWriter();
            stream.Write(sprit1);
            stream.Finalize(Game.GamePackets.spri);
            return stream;
        }
        public static unsafe void Getsprit1(this ServerSockets.Packet stream, out uint sprit1)
        {
            sprit1 = stream.ReadUInt32();
        }
    }
}
