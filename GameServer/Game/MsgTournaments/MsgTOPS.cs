using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeathWish.Game.MsgTournaments
{
    public class MsgTOPS
    {
        public uint TopSpouse = 0;
        public uint MRConquerHost = 0;
        public uint MSConquerHostess = 0;
        public uint rygh_hglx = 0;
        public uint rygh_syzs = 0;
        public uint WinnerUID = 0;
        public uint WinnerMonthlyUID = 0;
        public uint bdeltoid_cyc = 0;
        public uint _p_6_targst = 0;
        public uint GoldBrickSuper = 0;
        public uint GoldBrickElite = 0;
        public uint GoldBrickUnique = 0;
        public uint VikingPk = 0;
        public uint GuildLeaderT = 0;
        public uint DeputyLeaderT = 0;

        public void Weekly(Client.GameClient client, ServerSockets.Packet stream)
        {

            WinnerUID = client.Player.UID;
            AddTop(client);
        }

        public void Monthly(Client.GameClient client, ServerSockets.Packet stream)
        {
            WinnerMonthlyUID = client.Player.UID;


            AddTop(client);

        }
        public void MRConquer(Client.GameClient client, ServerSockets.Packet stream)
        {
            MRConquerHost = client.Player.UID;


            AddTop(client);

        }
        public void TopSpouse1(Client.GameClient client, ServerSockets.Packet stream)
        {
            TopSpouse = client.Player.UID;


            AddTop(client);

        }
        public void MSConquer(Client.GameClient client, ServerSockets.Packet stream)
        {
            MSConquerHostess = client.Player.UID;


            AddTop(client);

        }
        public void MRBoy(Client.GameClient client, ServerSockets.Packet stream)
        {
            rygh_hglx = client.Player.UID;


            AddTop(client);

        }
        public void MSGirl(Client.GameClient client, ServerSockets.Packet stream)
        {
            rygh_syzs = client.Player.UID;


            AddTop(client);

        }

        public void QueenWorld(Client.GameClient client, ServerSockets.Packet stream)
        {
            bdeltoid_cyc = client.Player.UID;


            AddTop(client);

        }
        public void KingWorld(Client.GameClient client, ServerSockets.Packet stream)
        {
            _p_6_targst = client.Player.UID;


            AddTop(client);

        }

        public void GoldBrickSuper1(Client.GameClient client, ServerSockets.Packet stream)
        {
            GoldBrickSuper = client.Player.UID;


            AddTop(client);

        }
        public void GoldBrickElite1(Client.GameClient client, ServerSockets.Packet stream)
        {
            GoldBrickElite = client.Player.UID;


            AddTop(client);

        }
        public void GoldBrickUnique1(Client.GameClient client, ServerSockets.Packet stream)
        {
            GoldBrickUnique = client.Player.UID;


            AddTop(client);

        }
        public void Vikings(Client.GameClient client, ServerSockets.Packet stream)
        {
            VikingPk = client.Player.UID;


            AddTop(client);

        }
        public void GuildLeader(Client.GameClient client, ServerSockets.Packet stream)
        {
            GuildLeaderT = client.Player.UID;


            AddTop(client);

        }
        public void DeputyLeader(Client.GameClient client, ServerSockets.Packet stream)
        {
            DeputyLeaderT = client.Player.UID;


            AddTop(client);

        }
        public void AddTop(Client.GameClient client)
        {
            if (GuildLeaderT == client.Player.UID)
                client.Player.AddFlag(MsgServer.MsgUpdate.Flags.TopGuildLeader, Role.StatusFlagsBigVector32.PermanentFlag, false);
            if (DeputyLeaderT == client.Player.UID)
                client.Player.AddFlag(MsgServer.MsgUpdate.Flags.TopDeputyLeader, Role.StatusFlagsBigVector32.PermanentFlag, false);

            if (VikingPk == client.Player.UID)
                client.Player.AddFlag(MsgServer.MsgUpdate.Flags.rygh_hglx1, Role.StatusFlagsBigVector32.PermanentFlag, false);
            if (GoldBrickSuper == client.Player.UID)
                client.Player.AddFlag(MsgServer.MsgUpdate.Flags.GoldBrickSuper, Role.StatusFlagsBigVector32.PermanentFlag, false);
            if (GoldBrickElite == client.Player.UID)
                client.Player.AddFlag(MsgServer.MsgUpdate.Flags.GoldBrickElite, Role.StatusFlagsBigVector32.PermanentFlag, false);
            if (GoldBrickUnique == client.Player.UID)
                client.Player.AddFlag(MsgServer.MsgUpdate.Flags.GoldBrickUnique, Role.StatusFlagsBigVector32.PermanentFlag, false);
            if (bdeltoid_cyc == client.Player.UID)
                client.Player.AddFlag(MsgServer.MsgUpdate.Flags.bdeltoid_cyc, Role.StatusFlagsBigVector32.PermanentFlag, false);
            if (_p_6_targst == client.Player.UID)
                client.Player.AddFlag(MsgServer.MsgUpdate.Flags._p_6_targst, Role.StatusFlagsBigVector32.PermanentFlag, false);
            if (rygh_hglx == client.Player.UID)
                client.Player.AddFlag(MsgServer.MsgUpdate.Flags.rygh_hglx, Role.StatusFlagsBigVector32.PermanentFlag, false);
            if (rygh_syzs == client.Player.UID)
                client.Player.AddFlag(MsgServer.MsgUpdate.Flags.rygh_syzs, Role.StatusFlagsBigVector32.PermanentFlag, false);
            if (TopSpouse == client.Player.UID)
                client.Player.AddFlag(MsgServer.MsgUpdate.Flags.TopSpouse, Role.StatusFlagsBigVector32.PermanentFlag, false);
            if (MSConquerHostess == client.Player.UID)
                client.Player.AddFlag(MsgServer.MsgUpdate.Flags.MSConquerHostess, Role.StatusFlagsBigVector32.PermanentFlag, false);
            if (MRConquerHost == client.Player.UID)
                client.Player.AddFlag(MsgServer.MsgUpdate.Flags.MRConquerHost, Role.StatusFlagsBigVector32.PermanentFlag, false);
            if (WinnerUID == client.Player.UID)
                client.Player.AddFlag(MsgServer.MsgUpdate.Flags.WeeklyPKChampion, Role.StatusFlagsBigVector32.PermanentFlag, false);
            if (WinnerMonthlyUID == client.Player.UID)
                client.Player.AddFlag(MsgServer.MsgUpdate.Flags.MonthlyPKChampion, Role.StatusFlagsBigVector32.PermanentFlag, false);
        }
    }
}