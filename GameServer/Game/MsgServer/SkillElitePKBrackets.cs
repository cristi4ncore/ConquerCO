using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace DeathWish.Game.MsgServer
{
    public static unsafe partial class MsgBuilder
    {
        public static unsafe void GetSkillElitePKBrackets(this ServerSockets.Packet stream, out SkillElitePKBrackets.Action mode, out ushort Page, out ushort packetno
            , out uint MatchCount, out MsgTournaments.MsgEliteTournament.GroupTyp Group, out SkillElitePKBrackets.GuiTyp guityp
            , out ushort TimeLeft, out uint TotalMatches)
        {
            mode = (SkillElitePKBrackets.Action)stream.ReadUInt16();
            Page = stream.ReadUInt16();
            packetno = stream.ReadUInt16();
            MatchCount = stream.ReadUInt32();
            Group = (MsgTournaments.MsgEliteTournament.GroupTyp)stream.ReadUInt16();
            guityp = (SkillElitePKBrackets.GuiTyp)stream.ReadUInt16();
            TimeLeft = stream.ReadUInt16();
            TotalMatches = stream.ReadUInt32();
        }
        public static unsafe ServerSockets.Packet AddItemSkillElitePKBrackets(this ServerSockets.Packet stream, ushort type, MsgTournaments.SkillEliteGroup.Match match)
        {
            stream.Write(match.ID);
            stream.Write((ushort)match.Teams.Count);
            stream.Write(match.Index);
            stream.Write((ushort)match.Flag);

            var array = match.GetMatchStats;
            for (int x = 0; x < array.Length; x++)
            {
                var element = array[x];
                if (type == 2232)
                    stream.Write((uint)0); //server id
                stream.Write(element.UID);
                stream.Write(element.LeaderUID);
                stream.Write(element.LeaderMesh);
                stream.Write(element.Name, 32);

                if ((byte)element.Flag == 0)//1)
                    stream.Write((uint)1);
                else
                    stream.Write((uint)element.Flag);
            }

            if (array.Length == 0)
                stream.ZeroFill(144 + ((type == 2232) ? 16 : 0));
            else if (array.Length == 1)
                stream.ZeroFill(96 + ((type == 2232) ? 8 : 0));
            else if (array.Length == 2)
                stream.ZeroFill(48 + ((type == 2232) ? 4 : 0));

            return stream;
        }
        public static unsafe ServerSockets.Packet SkillEliteBracketsCreate(this ServerSockets.Packet stream, SkillElitePKBrackets.Action mode, ushort Page,
            ushort PacketNo,
             uint MatchCount, MsgTournaments.MsgEliteTournament.GroupTyp Group, SkillElitePKBrackets.GuiTyp guityp
            , ushort TimeLeft, uint TotalMatches)
        {
            stream.InitWriter();

            stream.Write((ushort)mode);//4 5

            stream.Write(Page);//6 7
            stream.Write((ushort)PacketNo);//8 9

            stream.Write(MatchCount);//10 11 12 13

            stream.Write((ushort)Group);//14 15

            stream.Write((ushort)guityp);//16

            stream.Write(TimeLeft);//18
            stream.Write(TotalMatches);//20

            return stream;
        }
        public static unsafe ServerSockets.Packet SkillElitePKBracketsFinalize(this ServerSockets.Packet stream, ushort ID)
        {
            stream.ZeroFill(154);
            stream.Finalize(ID);//GamePackets.TeamElitePkBrackets);
            return stream;
        }
    }

    public unsafe struct SkillElitePKBrackets
    {
        public enum GuiTyp : ushort
        {
            GUI_Top8Ranking = 0,
            GUI_Knockout = 3,
            GUI_Top8Qualifier = 4,
            GUI_Top4Qualifier = 5,
            GUI_Top2Qualifier = 6,
            GUI_Top3 = 7,
            GUI_Top1 = 8,
            GUI_ReconstructTop = 9
        }
        public enum Action : ushort
        {
            InitialList = 0,
            StaticUpdate = 1,
            GUIEdit = 2,
            UpdateList = 3,
            RequestInformation = 4,
            EPK_State = 6
        }

        [PacketAttribute(GamePackets.SkillElitePkBrackets)]
        private static void PorocesSkillTeam(Client.GameClient user, ServerSockets.Packet stream)
        {
            try
            {
                SkillElitePKBrackets.Action mode;
                ushort Page;
                ushort packetno;
                uint MatchCount;

                MsgTournaments.MsgEliteTournament.GroupTyp Group;
                SkillElitePKBrackets.GuiTyp guityp;
                ushort TimeLeft;
                uint TotalMatches;

                stream.GetSkillElitePKBrackets(out mode, out Page, out packetno, out MatchCount, out Group, out guityp, out TimeLeft, out TotalMatches);

                stream.SkillEliteBracketsCreate(mode, Page, packetno, MatchCount, Group, guityp, TimeLeft, TotalMatches);

                user.Send(stream.SkillElitePKBracketsFinalize(GamePackets.SkillElitePkBrackets));

                if (mode == Action.RequestInformation || mode == Action.InitialList || mode == Action.UpdateList)
                {
                    var tournament = MsgTournaments.MsgSkillTeamPkTournament.EliteGroups[Math.Min(3, (int)Group)];
                    if (tournament.Proces != MsgTournaments.ProcesType.Dead)
                    {
                        if (tournament.State1 >= GuiTyp.GUI_Top4Qualifier)
                        {
                            if (tournament.Matches != null)
                            {
                                tournament.SendBrackets1(tournament.Matches.GetValues(), user, false, Page, Action.GUIEdit, true);
                                if (tournament.State1 >= GuiTyp.GUI_Top2Qualifier)
                                    tournament.SendBrackets1(tournament.ArrayMatchesTop3(), user, false, 1, Action.GUIEdit, true);
                            }
                            else
                                tournament.SendBrackets1(tournament.ArrayMatchesTop3(), user, false, 0, Action.GUIEdit, true);
                        }
                        else
                        {
                            if (tournament.Matches != null)
                                tournament.SendBrackets1(tournament.Matches.GetValues(), user, false, Page, Action.UpdateList, true);
                        }
                    }
                }
            }
            catch (Exception e) { MyConsole.WriteLine(e.ToString()); }

        }
    }
}
