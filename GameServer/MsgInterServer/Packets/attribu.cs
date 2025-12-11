using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeathWish.MsgInterServer.Packets
{
    public static class attribu
    {
        public static unsafe ServerSockets.Packet Sentattribu(this ServerSockets.Packet stream, uint attribu)
        {
            stream.InitWriter();
            stream.Write(attribu);
            stream.Finalize(Game.GamePackets.donatepo);
            return stream;
        }
        public static unsafe void Getattribu(this ServerSockets.Packet stream, out uint attribu)
        {
            attribu = stream.ReadUInt32();
        }
    }
}
