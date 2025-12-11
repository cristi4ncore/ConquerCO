using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeathWish.MsgInterServer.Packets
{
    public static class vippoints
    {
        public static unsafe ServerSockets.Packet Sentvip(this ServerSockets.Packet stream, uint vippoints)
        {
            stream.InitWriter();
            stream.Write(vippoints);
            stream.Finalize(Game.GamePackets.vippoints);
            return stream;
        }
        public static unsafe void Getvip(this ServerSockets.Packet stream, out uint vippoints)
        {
            vippoints = stream.ReadUInt32();
        }
    }
}
