using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;


namespace DeathWish.Game.MsgServer
{
    public static class MsgCrossArenaServer
    {
        [ProtoContract]
        public class CrossArena
        {
            [ProtoMember(1, IsRequired = true)]
            public uint ServerID; // it id server cross arena ! by mo ali - rankos 
            public CrossArenaServer[] Servers;
        }
        [ProtoContract]
        public class CrossArenaServer
        {
            [ProtoMember(1, IsRequired = false)]
            public uint ServerID;
            [ProtoMember(2, IsRequired = false)]
            public uint dwparam;
            [ProtoMember(3, IsRequired = false)]
            public string Name;
            [ProtoMember(4, IsRequired = false)]
            public uint MapID;
            [ProtoMember(5, IsRequired = false)]
            public uint X;
            [ProtoMember(6, IsRequired = false)]
            public uint Y;
            [ProtoMember(7, IsRequired = false)]
            public uint GroupID;
        }
        public static unsafe ServerSockets.Packet CreateCrossArenaServer(this ServerSockets.Packet stream, CrossArena obj)
        {
            stream.InitWriter();
            stream.ProtoBufferSerialize(obj);
            stream.Finalize(GamePackets.MsgCrossArenaServer); 
            return stream;
        }

        public static unsafe void GetCrossArenaServer(this ServerSockets.Packet stream, out CrossArena pQuery)
        {
            pQuery = new CrossArena(); 
            pQuery = stream.ProtoBufferDeserialize<CrossArena>(pQuery);
        }
    }
}