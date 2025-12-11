/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace DeathWish.Game.MsgServer
{
    class MsgCrossElitePkServer
    {
         public static class MsgCrossElitePkServer
    {
        [ProtoContract]
        public class CrossElitePkServer
        {
            [ProtoMember(1, IsRequired = true)]
            public uint ServerID;
            [ProtoMember(2, IsRequired = true)]
            public uint Name;
            [ProtoMember(3, IsRequired = true)]
            public uint MapID;
            [ProtoMember(4, IsRequired = true)]
            public uint X;
            [ProtoMember(5, IsRequired = true)]
            public uint Y;
            [ProtoMember(6, IsRequired = true)]
            public uint TransferType;
        }
        public static unsafe ServerSockets.Packet CreateCrossElitePkServer(this ServerSockets.Packet stream, CrossElitePkServer obj)
        {
            stream.InitWriter();
            stream.ProtoBufferSerialize(obj);
            stream.Finalize(GamePackets.CrossElitePkServer);
            return stream;
        }
        public static unsafe ServerSockets.Packet ConnectionInfoCreate(this ServerSockets.Packet stream, uint MapID, uint Type, uint ServerID, uint Name, uint TransferType)
        {
            stream.InitWriter();
            stream.Write(Type);
            stream.Write(TransferType);
            stream.Write(ServerID);
            stream.Write(MapID);
            stream.Write(Name);
            stream.ZeroFill(8);
 
            stream.Finalize(GamePackets.CrossElitePk);
            return stream;
        }
    }
}
    }
}*/
