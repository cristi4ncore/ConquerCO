using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace DeathWish.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {
        public static unsafe ServerSockets.Packet CreateTexasMatchInfo(this ServerSockets.Packet stream, DeathWish.Game.MsgServer.MsgTexasExMatchFieldList.TexasMatchInfo obj)
        {
            stream.InitWriter();
            stream.ProtoBufferSerialize(obj);
            stream.Finalize(GamePackets.MsgTexasExMatchFieldList);

            return stream;
        }
        public static unsafe ServerSockets.Packet CreateTexasMatchInfo(this ServerSockets.Packet stream, DeathWish.Game.MsgServer.MsgTexasExMatchFieldList.TexasMatchInfoArray obj)
        {
            stream.InitWriter();
            stream.ProtoBufferSerialize(obj);
            stream.Finalize(GamePackets.MsgTexasExMatchFieldList);
            return stream;
        }
    }
    public unsafe class MsgTexasExMatchFieldList
    {
        [ProtoContract]
        public class TexasMatchInfoArray
        {
            [ProtoMember(1, IsRequired = true)]
            public TexasMatchInfo[] Matchs;
        }
        [ProtoContract]
        public class TexasMatchInfo
        {
            [ProtoMember(1, IsRequired = true)]
            public int ID;
            [ProtoMember(2, IsRequired = true)]
            public int PlayersCount;
        }
        
    }
}
