using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeathWish.Client;
using DeathWish.Game.MsgServer;
using DeathWish.Role;
using static DeathWish.Role.Flags;

namespace DeathWish.Game.MsgEvents
{
    class DragonWar : Events
    {
        public uint Win_TopDragonWar = 0;
        DateTime LastScore;

        public DragonWar()
        {
            IDEvent = 2;
            EventTitle = "DragonWar";
            IDMessage = MsgStaticMessage.Messages.DragonWar;
            BaseMap = 2571;
            Reflect = false;
            NoDamage = true;
            MagicAllowed = false;
            MeleeAllowed = false;
            FriendlyFire = true;
            AllowedSkills = new System.Collections.Generic.List<ushort> { (ushort)1045, (ushort)1046, (ushort)1047 };
            Duration = 180;
            PotionsAllowed = false;
        }

        public override void TeleportPlayersToMap()
        {
            base.TeleportPlayersToMap();
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                foreach (GameClient client in PlayerList.Values.ToArray())
                {
                    client.Player.DragonWar = 0;
                    if (!DW)
                    {
                        client.Player.AddFlag(MsgServer.MsgUpdate.Flags.GodlyShield, StatusFlagsBigVector32.PermanentFlag, true);
                        DW = true;
                    }
                }
                LastScore = DateTime.Now;
                DisplayScores = DateTime.Now;
            }
        }
        public override void WaitForWinner()
        {
            base.WaitForWinner();
            if (PlayerScores.ContainsValue(300))
                Finish();
            else
            {
                bool victim = false;
                foreach (var v in PlayerList.Values.ToArray())
                {
                    if (v.Player.ContainFlag(MsgServer.MsgUpdate.Flags.GodlyShield))
                    {
                        victim = true;
                    }
                }
                if (!victim)
                {
                    var vic = PlayerScores.OrderByDescending((s => s.Value)).ToArray().FirstOrDefault();
                    PlayerList[vic.Key].Player.AddFlag(MsgServer.MsgUpdate.Flags.GodlyShield, StatusFlagsBigVector32.PermanentFlag, true);
                    DW = true;
                }
                if (DateTime.Now >= DisplayScores.AddMilliseconds(3000))
                    DisplayScore();
            }
        }
        public override void CharacterChecks(GameClient client)
        {
            base.CharacterChecks(client);
            if (client.Player.ContainFlag(MsgServer.MsgUpdate.Flags.GodlyShield))
            {
                if (DateTime.Now >= LastScore.AddMilliseconds(2000))
                {
                    LastScore = DateTime.Now;
                    if (PlayerScores[client.EntityID] + 3 > 300)
                        PlayerScores[client.EntityID] = 300;
                    else
                        PlayerScores[client.EntityID] += 3;
                }
            }
        }
        public override void RemovePlayer(GameClient client, bool diss = false, bool CanTeleport = false)
        {
            base.RemovePlayer(client);
            if (client.Player.ContainFlag(MsgServer.MsgUpdate.Flags.GodlyShield))
            {
                DW = false;
                client.Player.RemoveFlag(MsgServer.MsgUpdate.Flags.GodlyShield);
            }
        }
        public override void End()
        {
            foreach (GameClient client in PlayerList.Values.ToArray())
                client.Player.RemoveFlag(MsgServer.MsgUpdate.Flags.GodlyShield);

            DisplayScore();
            byte NO = 1;
            foreach (var player in PlayerScores.OrderByDescending(s => s.Value).ToArray())
            {
                if (NO == 1)
                {
                    Reward(PlayerList[player.Key]);
                    RemovePlayer(PlayerList[player.Key]);
                    NO++;
                }
                else
                {
                    if (PlayerList.ContainsKey(player.Key))
                    {
                        RemovePlayer(PlayerList[player.Key]);
                        NO++;
                    }
                }
            }

            PlayerList.Clear();
            PlayerScores.Clear();
            return;
        }
        public void Reward(Client.GameClient client, ServerSockets.Packet stream)
        {
            Win_TopDragonWar = client.Player.UID;
            client.Player.DragonWar += 1;
            client.Player.RemoveFlag(MsgServer.MsgUpdate.Flags.GodlyShield);
            base.Reward(client);
            AddTop(client);
        }
        public void AddTop(Client.GameClient client)
        {
            if (Win_TopDragonWar == client.Player.UID)
                client.Player.AddFlag(MsgServer.MsgUpdate.Flags.TopDragonWar, Role.StatusFlagsBigVector32.PermanentFlag, false);
        }
        public override uint GetDamage(GameClient User, GameClient C)
        {
            if (User.Player.ContainFlag(MsgServer.MsgUpdate.Flags.GodlyShield))
                return Convert.ToUInt32(C.Status.MaxHitpoints);
            else if (!DW)
                return Convert.ToUInt32(C.Status.MaxHitpoints * 0.4);
            else if (C.Player.ContainFlag(MsgServer.MsgUpdate.Flags.GodlyShield))
                return Convert.ToUInt32(C.Status.MaxHitpoints * 0.4);
            return 1;
        }

        public override void Hit(GameClient Attacker, GameClient Victim)
        {
            //if (PlayerScores[Attacker.EntityID] + 5 > 300)
            //    PlayerScores[Attacker.EntityID] = 300;
            //else
            //    PlayerScores[Attacker.EntityID] += 5;
        }
        public override void Kill(GameClient Attacker, GameClient Victim)
        {
            if (Victim.Player.ContainFlag(MsgUpdate.Flags.GodlyShield))
            {
                Victim.Player.RemoveFlag(MsgUpdate.Flags.GodlyShield);
                Attacker.Player.AddFlag(MsgUpdate.Flags.GodlyShield, StatusFlagsBigVector32.PermanentFlag, true);
                Attacker.Player.Stamina = 100;
                DW = true;
                if (PlayerScores[Attacker.EntityID] + 5 > 300)
                    PlayerScores[Attacker.EntityID] = 300;
                else
                    PlayerScores[Attacker.EntityID] += 5;
            }
            else if (!DW)
            {
                Attacker.Player.AddFlag(MsgUpdate.Flags.GodlyShield, StatusFlagsBigVector32.PermanentFlag, true);
                PlayerScores[Attacker.EntityID]++;
                DW = true;
            }
            else if (Attacker.Player.ContainFlag(MsgUpdate.Flags.GodlyShield))
            {
                if (PlayerScores[Attacker.EntityID] + 5 > 300)
                    PlayerScores[Attacker.EntityID] = 300;
                else
                    PlayerScores[Attacker.EntityID] += 5;
            }
            else
            {
                if (PlayerScores[Attacker.EntityID]++ > 300)
                    PlayerScores[Attacker.EntityID] = 300;
                else
                    PlayerScores[Attacker.EntityID]++;
            }
        }

        public override void DisplayScore()
        {
            DisplayScores = DateTime.Now;
            foreach (var player in PlayerList.Values.ToArray())
            {
                player.SendSysMesage($"---------{EventTitle}---------", MsgServer.MsgMessage.ChatMode.FirstRightCorner);
            }
            byte Score = 2;
            foreach (var kvp in PlayerScores.OrderByDescending((s => s.Value)).ToArray())
            {
                if (Score == 7)
                    break;
                if (Score == PlayerScores.Count + 2)
                    break;
                Broadcast($"Rank {Score - 1}: {PlayerList[kvp.Key].Player.Name} - Score  : {kvp.Value}", BroadCastLoc.Score, Score);
                Score++;
            }
            TimeSpan T = TimeSpan.FromSeconds(Duration);
            Broadcast("----------------------------------", BroadCastLoc.Score);

            Broadcast("First One Get 300 Points will win", BroadCastLoc.Score);

            Broadcast("----------------------------------", BroadCastLoc.Score);

            Broadcast($"Time left {T.ToString(@"mm\:ss")}", BroadCastLoc.Score);
            if (Duration > 0)
                --Duration;
        }
    }
}