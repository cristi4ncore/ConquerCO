using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeathWish.Game.MsgServer
{
    public static unsafe partial class MsgBuilder
    {
        public static unsafe ServerSockets.Packet SkillElitePkRankingCreate(this ServerSockets.Packet stream, SkillElitePkRanking.RankType rank
            , uint Group, SkillElitePKBrackets.GuiTyp GroupStatus, uint Count, uint UID)
        {
            stream.InitWriter();
            stream.Write((uint)rank);//4
            stream.Write(Group);//8
            stream.Write((uint)GroupStatus);//12
            stream.Write(Count);//16
            return stream;
        }

        public static unsafe ServerSockets.Packet AdditemSkillElitePkRanking(this ServerSockets.Packet stream, MsgTournaments.SkillEliteGroup.FighterStats status, uint Rank)
        {
            stream.Write(status.LeaderUID);
            stream.Write(Rank);
            stream.Write(status.Name, 32);
            stream.Write(status.LeaderMesh);
            return stream;
        }
        public static unsafe ServerSockets.Packet SkillElitePkRankingFinalize(this ServerSockets.Packet stream, ushort ID)
        {
            stream.ZeroFill(420 - stream.Position);
            stream.Finalize(ID);
            return stream;
        }
        public static unsafe void GetSkillElitePkRanking(this ServerSockets.Packet stream, out uint Group)
        {
            uint first = stream.ReadUInt32();
            Group = stream.ReadUInt32();
        }
    }
    public class SkillElitePkRanking
    {
        public enum RankType : uint
        {
            Top8 = 0,
            Top3 = 2
        }
        [PacketAttribute(GamePackets.SkillElitePkTop)]
        private static void PorocesSkillTeamPkRanking(Client.GameClient user, ServerSockets.Packet stream)
        {
            //return;
            try
            {
                uint Group;

                stream.GetSkillElitePkRanking(out Group);

                var tournament = MsgTournaments.MsgSkillTeamPkTournament.EliteGroups[Math.Min(3, (int)Group)];

                if (tournament.Top8 == null)
                    return;
                if (tournament.Top8.Length == 0)
                    return;

                //if (tournament.Top8[0] == null)
                //    return;

                if (tournament.State1 >= SkillElitePKBrackets.GuiTyp.GUI_Top1)
                {
                    if (tournament.State1 == SkillElitePKBrackets.GuiTyp.GUI_Top1)
                    {
                        if (tournament.Top8[2] != null)
                        {

                            stream.SkillElitePkRankingCreate(RankType.Top3, Group, tournament.State1, 1, user.Player.UID);

                            stream.AdditemSkillElitePkRanking(tournament.Top8[2], 3);
                            user.Send(stream.SkillElitePkRankingFinalize(GamePackets.SkillElitePkTop));

                        }
                    }
                    else
                    {
                        stream.SkillElitePkRankingCreate(RankType.Top3, Group, tournament.State1, 3, user.Player.UID);

                        for (int i = 0; i < 3; i++)
                            stream.AdditemSkillElitePkRanking(tournament.Top8[i], (uint)(i + 1));

                        user.Send(stream.SkillElitePkRankingFinalize(GamePackets.SkillElitePkTop));

                    }
                }
                else
                {

                    stream.SkillElitePkRankingCreate(RankType.Top8, Group, tournament.State1, 8, user.Player.UID);

                    for (int i = 0; i < 8; i++)
                    {
                        if (tournament.Top8[i] == null)
                            break;
                        stream.AdditemSkillElitePkRanking(tournament.Top8[i], (uint)(i + 1));
                    }
                    user.Send(stream.SkillElitePkRankingFinalize(GamePackets.SkillElitePkTop));
                }
            }
            catch (Exception e) { MyConsole.WriteLine(e.ToString()); }
        }
    }
}
