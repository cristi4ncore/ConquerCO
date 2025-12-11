using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeathWish.Game.MsgServer.RealmPoker
{
    public class MsgTexasExInteractive
    {
        [Flags]
        public enum TexasFlag : uint
        {
            ShowMatchInfo,
            ShowTournmentInfos = 2,
            JoinTournement = 3,
            Wheel = 11,
        }
        [ProtoContract]
        public class TexasAction
        {
            [ProtoMember(1, IsRequired = true)]
            public TexasFlag Action;
            [ProtoMember(2, IsRequired = true)]
            public uint dwparam;
            [ProtoMember(3, IsRequired = true)]
            public uint MatchID;
            [ProtoMember(4, IsRequired = true)]
            public uint dwparam2;
            [ProtoMember(5, IsRequired = true)]
            public uint dwparam3;
            [ProtoMember(6, IsRequired = true)]
            public uint dwparam4;
            [ProtoMember(7, IsRequired = true)]
            public uint dwparam5;
        }
    }
}
