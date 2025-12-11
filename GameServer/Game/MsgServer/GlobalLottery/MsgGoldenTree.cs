using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeathWish.Game.MsgServer
{
    public static class MsgGoldenTree
    {
        public static List<Tuple<string, uint>> GoldenTreeRank = new List<Tuple<string, uint>>();
        public static bool Contains(string Name)
        {
            foreach (var item in GoldenTreeRank)
                if (item.Item1 == Name)
                    return true;
            return false;
        }
        public static void Update(string Name, uint Value)
        {
            if (Contains(Name))
            {
                foreach (var item in GoldenTreeRank)
                    if (item.Item1 == Name)
                        GoldenTreeRank.Remove(item);
                GoldenTreeRank.Add(Tuple.Create(Name, Value));
            }
            else
            {
                GoldenTreeRank.Add(Tuple.Create(Name, Value));
            }
        }


        public static void SendWorldMessage(DeathWish.ServerSockets.Packet stream)
        {
            foreach (var player in Database.Server.GamePoll.Values)
                player.Send(stream);
        }
        public static void Ready(Client.GameClient client, ServerSockets.Packet stream)
        {
            MsgGlobalLottery Lottery = new MsgGlobalLottery();
            Lottery.Time = 5;
            Lottery.GUI = 1;
            Lottery.PrizesType = Database.GlobalLotteryTable.TodayCondition.ID;
            Lottery.TotalCount = Database.Server.MaxAvaliableGoldenTreeClaim;
            client.Send(stream.CreateGlobalLottery(Lottery));
        }
        public static void BlossomEvaluation(Client.GameClient client, ServerSockets.Packet stream)
        {
            MsgGlobalLottery Lottery = new MsgGlobalLottery();
            Lottery.Time = Database.GlobalLotteryTable.TickCount;
            Lottery.GUI = 2;
            Lottery.PrizesType = Database.GlobalLotteryTable.TodayCondition.ID;
            Lottery.TotalCount = Database.Server.MaxAvaliableGoldenTreeClaim;
            client.Send(stream.CreateGlobalLottery(Lottery));
        }
        public static void LoginEvaluation(Client.GameClient client, ServerSockets.Packet stream)
        {
        //  //  if (DateTime.Now >= Database.Server.GoldenTreeExpirationDate)
        ////    {
        //        MsgGlobalLottery Lottery = new MsgGlobalLottery();
        //        Lottery.GUI = (uint)(GoldenTreeRank.Count == 0 ? 0 : 3);
        //        Lottery.PrizesType = Database.GlobalLotteryTable.TodayCondition.ID;
        //        client.Send(stream.CreateGlobalLottery(Lottery));
        //        return;
        //  //  }
            BlossomEvaluation(client, stream);
        }
        public static void EndEvent()
        {
            using (var recycle = new ServerSockets.RecycledPacket())
            {
                var stream = recycle.GetStream();
                MsgGlobalLottery Lottery = new MsgGlobalLottery();
                Lottery.GUI = (uint)(GoldenTreeRank.Count == 0 ? 0 : 3);
                Lottery.PrizesType = Database.GlobalLotteryTable.TodayCondition.ID;
                SendWorldMessage(stream.CreateGlobalLottery(Lottery));
            }
        }
    }
}
