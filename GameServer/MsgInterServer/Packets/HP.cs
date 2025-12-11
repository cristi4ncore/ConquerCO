using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeathWish.MsgInterServer.Packets
{
    public static class HP
    {
        public static unsafe ServerSockets.Packet SentHPVI(this ServerSockets.Packet stream, uint HPVI)
        {
            stream.InitWriter();
            stream.Write(HPVI);
            stream.Finalize(Game.GamePackets.HPV);
            return stream;
        }
        public static unsafe void GetHPVI(this ServerSockets.Packet stream, out uint HPVI)
        {
            HPVI = stream.ReadUInt32();
        }
    }
}
