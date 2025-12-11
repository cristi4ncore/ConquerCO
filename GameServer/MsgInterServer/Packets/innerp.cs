using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeathWish.MsgInterServer.Packets
{
    public static class innerp
    {
        public static unsafe ServerSockets.Packet Sentinnerp1(this ServerSockets.Packet stream, uint innerp1)
        {
            stream.InitWriter();
            stream.Write(innerp1);
            stream.Finalize(Game.GamePackets.innerp);
            return stream;
        }
        public static unsafe void Getinnerp1(this ServerSockets.Packet stream, out uint innerp1)
        {
            innerp1 = stream.ReadUInt32();
        }
    }
}
