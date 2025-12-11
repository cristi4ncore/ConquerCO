using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeathWish.MsgInterServer.Packets
{
    public static class viplevel
    {
        public static unsafe ServerSockets.Packet Sentviplevel(this ServerSockets.Packet stream, uint viplevel)
        {
            stream.InitWriter();
            stream.Write(viplevel);
            stream.Finalize(Game.GamePackets.viple);
            return stream;
        }
        public static unsafe void Getviplevel(this ServerSockets.Packet stream, out uint viplevel)
        {
            viplevel = stream.ReadUInt32();
        }
    }
}
