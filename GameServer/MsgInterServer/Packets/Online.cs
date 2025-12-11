using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeathWish.MsgInterServer.Packets
{
    public static class Online
    {
        public static unsafe ServerSockets.Packet SentOnline(this ServerSockets.Packet stream, uint OnlinePoints)
        {
            stream.InitWriter();
            stream.Write(OnlinePoints);
            stream.Finalize(Game.GamePackets.Online);
            return stream;
        }
        public static unsafe void GetOnline(this ServerSockets.Packet stream, out uint OnlinePoints)
        {
            OnlinePoints = stream.ReadUInt32();
        }
    }
}
