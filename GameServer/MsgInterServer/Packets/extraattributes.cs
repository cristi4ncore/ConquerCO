using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeathWish.MsgInterServer.Packets
{
    public static class extraattributes
    {
        public static unsafe ServerSockets.Packet Sentextra(this ServerSockets.Packet stream, uint extrapoints)
        {
            stream.InitWriter();
            stream.Write(extrapoints);
            stream.Finalize(Game.GamePackets.extra);
            return stream;
        }
        public static unsafe void Getextra(this ServerSockets.Packet stream, out uint extrapoints)
        {
            extrapoints = stream.ReadUInt32();
        }
    }
}
