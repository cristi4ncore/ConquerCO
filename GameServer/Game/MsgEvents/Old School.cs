using DeathWish.Client;
using DeathWish.Game.MsgServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DeathWish.Role.Flags;

namespace DeathWish.Game.MsgEvents
{
    class KungfuSchool : Events
    {
        int ScoreMars = 0, ScoreSaturn = 0;
        Dictionary<uint, GameClient> Mars = new Dictionary<uint, GameClient>();
        Dictionary<uint, GameClient> Saturn = new Dictionary<uint, GameClient>();
        public KungfuSchool()
        {
            IDEvent = 3;
            EventTitle = "Old School";
            IDMessage = MsgStaticMessage.Messages.KungfuSchool;
            BaseMap = 9573;
            NoDamage = true;
            MagicAllowed = false;
            MeleeAllowed = false;
            FriendlyFire = false;
            AllowedSkills = new List<ushort> { (ushort)1045, (ushort)1046, (ushort)1047 };
            Duration = 180;
            PotionsAllowed = false;
        }
        public override void BeginTournament()
        {
            Teams = new Dictionary<uint, Dictionary<uint, GameClient>>();
            base.BeginTournament();
        }
        public override void TeleportPlayersToMap()
        {
            base.TeleportPlayersToMap();

            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                var counter = 0;
                foreach (GameClient client in PlayerList.Values.ToArray())
                {
                    client.Player.Kungfu = 0;
                    client.Equipment.Remove(Role.Flags.ConquerItem.Garment, stream);
                    if (counter % 2 == 0)
                    {
                        Mars.Add(client.EntityID, client);
                        client.Player.AddSpecialGarment(stream, 191305);
                        client.SendSysMesage($"Welcome to {EventTitle} you're a member of team [RED]!");
                    }
                    else
                    {
                        Saturn.Add(client.EntityID, client);
                        client.Player.AddSpecialGarment(stream, 191405);
                        client.SendSysMesage($"Welcome to {EventTitle} you're a member of team [YELLOW]!");
                    }
                    counter++;
                }
                Teams.Add(191305, Mars);//[RED]
                Teams.Add(191405, Saturn);//[YELLOW]
            }
        }

        public void TeleafterRev(GameClient client)
        {
            ushort x = 0;
            ushort y = 0;
            Map.GetRandCoord(ref x, ref y);
            if (Teams[191305].ContainsKey(client.EntityID))
                client.Teleport(x, y, Map.ID, DinamicID, true, true);
            else
                client.Teleport(x, y, Map.ID, DinamicID, true, true);
        }

        public override void Hit(GameClient Attacker, GameClient Victim)
        {
            if (Stage == EventStage.Fighting)
            {
                Victim.Player.Dead(Attacker.Player, Victim.Player.X, Victim.Player.Y, Attacker.EntityID);
                if (Teams[191305].ContainsKey(Attacker.EntityID))
                    ScoreMars++;
                else
                    ScoreSaturn++;
                //PlayerScores[Attacker.EntityID]++;
                //RevivePlayer(Victim);
                //TeleafterRev(Victim);
            }
        }

        public override void DisplayScore()
        {
            DisplayScores = DateTime.Now;
            foreach (var player in PlayerList.Values.ToArray())
                player.SendSysMesage($"---------{EventTitle}---------", MsgServer.MsgMessage.ChatMode.FirstRightCorner);

            foreach (var player in PlayerList.Values.ToArray())
            {
                if (ScoreMars > ScoreSaturn)
                {
                    player.SendSysMesage($"Team [RED] - {ScoreMars}", MsgServer.MsgMessage.ChatMode.ContinueRightCorner);
                    player.SendSysMesage($"Team [YELLOW] - {ScoreSaturn}", MsgServer.MsgMessage.ChatMode.ContinueRightCorner);
                }
                else
                {
                    player.SendSysMesage($"Team [YELLOW] - {ScoreSaturn}", MsgServer.MsgMessage.ChatMode.ContinueRightCorner);
                    player.SendSysMesage($"Team [RED] - {ScoreMars}", MsgServer.MsgMessage.ChatMode.ContinueRightCorner);
                }
                player.SendSysMesage($"My Score - {PlayerScores[player.EntityID]}", MsgServer.MsgMessage.ChatMode.ContinueRightCorner);
            }
            TimeSpan T = TimeSpan.FromSeconds(Duration);
            Broadcast($"Time left {T.ToString(@"mm\:ss")}", BroadCastLoc.Score);
            if (Duration > 0)
                --Duration;
        }
        public override void WaitForWinner()
        {
            foreach (var P in PlayerList.Values.Where(x => !x.Socket.Alive || x.Player.Map != Map.ID).ToArray())
            {
                if (P.Socket.Alive)
                    P.EventBase = null;

                PlayerScores.Remove(P.Player.UID);
                PlayerList.Remove(P.Player.UID);

                foreach (KeyValuePair<uint, Dictionary<uint, GameClient>> T in Teams.ToArray())
                    if (T.Value.ContainsKey(P.Player.UID))
                        T.Value.Remove(P.Player.UID);
            }
            if (Duration < 100)
            {
                if (Teams[191405].Count == 0)
                    ScoreSaturn = 0;
                if (Teams[191305].Count == 0)
                    ScoreMars = 0;

                if (DateTime.Now >= EndTime || Duration <= 0 || Teams[191305].Count == 0 || Teams[191405].Count == 0)
                    Finish();
            }
            if (DateTime.Now >= LastScores.AddMilliseconds(3000))
                DisplayScore();
        }
        public override void End()
        {
            DisplayScore();
            if (ScoreMars == ScoreSaturn)
            {
                Broadcast("It's a tie! event Minutes have passed and the teams scored the same points! Better luck next time!", BroadCastLoc.World);
                foreach (GameClient C in PlayerList.Values.ToArray())
                    C.EventBase?.RemovePlayer(C);
            }
            else
            {
                if (ScoreMars > ScoreSaturn)
                {
                    Broadcast("The [[RED]] Team has won the " + EventTitle + "! Congratulations to all their members!", BroadCastLoc.World);
                    foreach (GameClient C2 in Teams[191405].Values.ToArray())
                        C2.EventBase?.RemovePlayer(C2);
                }
                else if (ScoreSaturn > ScoreMars)
                {
                    Broadcast("The [[YELLOW]] Team has won the " + EventTitle + "! Congratulations to all their members!", BroadCastLoc.World);
                    foreach (GameClient C in Teams[191305].Values.ToArray())
                        C.EventBase?.RemovePlayer(C);
                }
                foreach (var c in PlayerList.Values.ToArray())
                {
                    Reward(c);
                    RemovePlayer(c);
                }
            }
            PlayerList.Clear();
            PlayerScores.Clear();
            Teams.Clear();
            return;
        }
        public uint Win_TopKungfuSchool = 0;
        public void Reward(Client.GameClient client, ServerSockets.Packet stream)
        {
            Win_TopKungfuSchool = client.Player.UID;
            client.Player.Kungfu += 1;
            client.Player.RemoveFlag(MsgServer.MsgUpdate.Flags.GodlyShield);
            base.Reward(client);
            AddTop(client);
        }
        public void AddTop(Client.GameClient client)
        {
            if (Win_TopKungfuSchool == client.Player.UID)
                client.Player.AddFlag(MsgServer.MsgUpdate.Flags.TopKungfuSchool, Role.StatusFlagsBigVector32.PermanentFlag, false);
        }
    }
}
