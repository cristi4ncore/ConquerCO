using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeathWish.Game.MsgServer
{
    public static unsafe partial class MsgBuilder
    {
        public static unsafe ServerSockets.Packet SkillElitePKMatchStatsCreate(this ServerSockets.Packet stream, ushort ID, MsgTournaments.SkillEliteGroup.Match match)
        {
            stream.InitWriter();

            if (match.TeamsFighting.Length > 0)
            {
                var team = match.TeamsFighting[0];
                stream.Write(team.Leader.Player.UID);
                stream.Write(team.UID);
                stream.Write(team.Leader.Player.Name, 16);
                stream.Write(team.TeamName, 32);
                stream.Write(team.PKStats1.Points);
            }
            else
                stream.ZeroFill(60);

            if (match.TeamsFighting.Length > 1)
            {
                var team = match.TeamsFighting[1];
                stream.Write(team.Leader.Player.UID);
                stream.Write(team.UID);
                stream.Write(team.Leader.Player.Name, 16);
                stream.Write(team.TeamName, 32);
                stream.Write(team.PKStats1.Points);
            }
            else
                stream.ZeroFill(60);

            stream.Write((uint)0);//unknow

            stream.Finalize(ID);

            return stream;
        }
    }
}
