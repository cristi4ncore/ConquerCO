using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeathWish.Game.MsgServer
{
    public static unsafe partial class MsgBuilder
    {
        public static unsafe ServerSockets.Packet CreateGlobalLotteryRank(this ServerSockets.Packet stream, MsgGlobalLotteryRankList obj)
        {
            stream.InitWriter();
            stream.ProtoBufferSerialize(obj);
            stream.Finalize(GamePackets.MsgGlobalLotteryRankList);

            return stream;
        }
        public static unsafe void GetGlobalLotteryRank(this ServerSockets.Packet stream, out MsgGlobalLotteryRankList pQuery)
        {
            pQuery = new MsgGlobalLotteryRankList();
            pQuery = stream.ProtoBufferDeserialize<MsgGlobalLotteryRankList>(pQuery);
        }
    }
    [ProtoContract]
    public class MsgGlobalLotteryRankList
    {
        [ProtoMember(1, IsRequired = true)]
        public uint Page;
        [ProtoMember(2, IsRequired = true)]
        public uint TotalPages;
        [ProtoMember(3, IsRequired = true)]
        public uint RegistedCount;
        [ProtoMember(4, IsRequired = true)]
        public Picker[] Pickers;
        [PacketAttribute(GamePackets.MsgGlobalLotteryRankList)]
        public void ExcuteMsgGlobalLotteryRankingList(DeathWish.ServerSockets.Packet stream, Client.GameClient client)
        {
            if (MsgGoldenTree.GoldenTreeRank.Count == 0)
                return;
            MsgGlobalLotteryRankList Rank;
            stream.GetGlobalLotteryRank(out Rank);
            uint Page = Rank.Page;
            var selected = MsgGoldenTree.GoldenTreeRank.OrderByDescending(p => p.Item2).Skip((int)(10 * Page)).Take(10);
            Rank.TotalPages = (uint)(MsgGoldenTree.GoldenTreeRank.Count / 10) == 0 ? 1 : (uint)(MsgGoldenTree.GoldenTreeRank.Count / 10);
            Rank.RegistedCount = (uint)MsgGoldenTree.GoldenTreeRank.Count;
            Rank.Pickers = new Picker[selected.Count<Tuple<string, uint>>()];
            int index = 0;
            foreach (var entity in selected)
            {
                Rank.Pickers[index] = new Picker() { Name = entity.Item1, PickItem = entity.Item2 };
                index++;
            }
            client.Send(stream.CreateGlobalLotteryRank(Rank));
        }
    }
    [ProtoContract]
    public class Picker
    {
        [ProtoMember(1, IsRequired = true)]
        public uint PickItem;
        [ProtoMember(2, IsRequired = true)]
        public string Name;
    }
}
