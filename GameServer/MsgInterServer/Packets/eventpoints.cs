using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeathWish.MsgInterServer.Packets
{
    public static class eventpoints
    {
        public static unsafe ServerSockets.Packet Senteventpoints(this ServerSockets.Packet stream, uint eventpoints1)
        {
            stream.InitWriter();
            stream.Write(eventpoints1);
            stream.Finalize(Game.GamePackets.eventpoints);
            return stream;
        }
        public static unsafe void Geteventpoints(this ServerSockets.Packet stream, out uint eventpoints1)
        {
            eventpoints1 = stream.ReadUInt32();
        }
    }
}
