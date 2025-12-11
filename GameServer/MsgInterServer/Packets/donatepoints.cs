using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeathWish.MsgInterServer.Packets
{
    public static class donatepoints
    {
        public static unsafe ServerSockets.Packet Sentdonate(this ServerSockets.Packet stream, uint donatepoints)
        {
            stream.InitWriter();
            stream.Write(donatepoints);
            stream.Finalize(Game.GamePackets.donatepo);
            return stream;
        }
        public static unsafe void Getdonate(this ServerSockets.Packet stream, out uint donatepoints)
        {
            donatepoints = stream.ReadUInt32();
        }
    }
}
