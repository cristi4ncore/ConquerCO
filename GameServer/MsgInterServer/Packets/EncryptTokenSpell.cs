using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeathWish.MsgInterServer.Packets
{
    public static class EncryptTokenSpell
    {
        public static unsafe ServerSockets.Packet CreateEncryptTokenSpell(this ServerSockets.Packet stream, uint EncryptTokenSpell)
        {
            stream.InitWriter();
            stream.Write(EncryptTokenSpell);
            stream.Finalize((ushort)Game.GamePackets.CrossPackets);
            return stream;
        }
        public static unsafe void GetEncryptTokenSpell(this ServerSockets.Packet stream, out uint EncryptTokenSpell)
        {
            EncryptTokenSpell = stream.ReadUInt32();
        }
    }
}

