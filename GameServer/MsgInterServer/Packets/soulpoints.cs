using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeathWish.MsgInterServer.Packets
{
    public static class soulpoints
    {
        public static unsafe ServerSockets.Packet Sentsoulpoints1(this ServerSockets.Packet stream, uint soulpoints1)
        {
            stream.InitWriter();
            stream.Write(soulpoints1);
            stream.Finalize(Game.GamePackets.soulpoints);
            return stream;
        }
        public static unsafe void Getsoulpoints1(this ServerSockets.Packet stream, out uint soulpoints1)
        {
            soulpoints1 = stream.ReadUInt32();
        }
    }
}
