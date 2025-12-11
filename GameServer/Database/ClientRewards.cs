using DeathWish.Game.MsgServer.Reward;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeathWish.Database
{
    public class ClientRewards
    {
        public struct Rewards
        {
            public Mode Mode;
            public uint Claim;
            public uint Points;
        }
    }
}
