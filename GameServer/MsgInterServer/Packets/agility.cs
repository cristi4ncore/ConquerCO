using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeathWish.MsgInterServer.Packets
{
    public static class agility
    {
        public static unsafe ServerSockets.Packet Sentagility1(this ServerSockets.Packet stream, uint agility1)
        {
            stream.InitWriter();
            stream.Write(agility1);
            stream.Finalize(Game.GamePackets.agility);
            return stream;
        }
        public static unsafe void Getagility1(this ServerSockets.Packet stream, out uint agility1)
        {
            agility1 = stream.ReadUInt32();
        }
    }
}
