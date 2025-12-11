using DeathWish.Client;
using DeathWish.Game.MsgServer;
using DeathWish.Role;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DeathWish.Game.MsgServer.MsgMessage;
using static DeathWish.Role.Flags;

namespace DeathWish.Game.MsgEvents
{
    public class CycloneRace : Events
    {
        public Role.SobNpc npc;
        public uint Rank = 0;

        public CycloneRace()
        {
            IDEvent = 5;
            EventTitle = "CycloneRace";
            IDMessage = MsgStaticMessage.Messages.CycloneRace;
            NoDamage = true;
            MagicAllowed = false;
            MeleeAllowed = false;
            FriendlyFire = false;
            FlyAllowed = false;
            BaseMap = 1645;
            Duration = 120;
            Rank = 0;
            RewardCps = 430;
        }

        public override void TeleportPlayersToMap()
        {
            if (Map != null)
            {
                if (npc == null)
                {
                    npc = new Role.SobNpc();
                    npc.ObjType = Role.MapObjectType.SobNpc;
                    npc.UID = 465221;
                    npc.Name = "Exit";
                    npc.Type = NpcType.Talker;
                    npc.Mesh = (SobNpc.StaticMesh)35920;
                    npc.Map = Map.ID;                
                    npc.AddFlag(Game.MsgServer.MsgUpdate.Flags.Praying, StatusFlagsBigVector32.PermanentFlag, false);
                    npc.X = 218;
                    npc.Y = 199;
                    npc.HitPoints = 0;
                    npc.MaxHitPoints = 0;
                    npc.Sort = 1;
                    Map.View.EnterMap<Role.IMapObj>(npc);
                }
            }
            foreach (GameClient client in PlayerList.Values.ToArray())
            {
                ChangePKMode(client, PKMode.PK);
                ushort x = 303;
                ushort y = 233;
                Map.GetRandCoord(ref x, ref y);
                client.Teleport(x, y, Map.ID, DinamicID);
                if (client.Player.ContainFlag(MsgServer.MsgUpdate.Flags.Ride))
                    client.Player.RemoveFlag(MsgServer.MsgUpdate.Flags.Ride);
                if (client.Player.ContainFlag(MsgServer.MsgUpdate.Flags.Cyclone))
                    client.Player.RemoveFlag(MsgServer.MsgUpdate.Flags.Cyclone);
                if (client.Player.ContainFlag(MsgServer.MsgUpdate.Flags.Fly))
                    client.Player.RemoveFlag(MsgServer.MsgUpdate.Flags.Fly);
                if (client.Player.ContainFlag(MsgServer.MsgUpdate.Flags.Superman))
                    client.Player.RemoveFlag(MsgServer.MsgUpdate.Flags.Superman);
                client.Player.AddFlag(MsgServer.MsgUpdate.Flags.Freeze, 11, true);
                client.Player.HitPoints = (int)client.Status.MaxHitpoints;
            }
        }

        public override void CharacterChecks(GameClient C)
        {
            base.CharacterChecks(C);
            if (!C.Player.ContainFlag(MsgServer.MsgUpdate.Flags.Cyclone))
                C.Player.AddFlag(MsgServer.MsgUpdate.Flags.Cyclone, StatusFlagsBigVector32.PermanentFlag, true);
            if (C.Player.TransformInfo != null && C.Player.TransformationID != 0)
                C.Player.TransformInfo.FinishTransform();
            if (C.Player.ContainFlag(MsgServer.MsgUpdate.Flags.Ride))
                C.Player.RemoveFlag(MsgServer.MsgUpdate.Flags.Ride);
            if (!C.Player.Alive)
            {
                if (C.Player.DeadStamp.AddSeconds(3) < Extensions.Time32.Now)
                {
                    C.EventBase?.RemovePlayer(C);
                }
            }
        }

        public override void End()
        {
            DisplayScore();
            foreach (var player in PlayerScores.OrderByDescending(s => s.Value).ToArray())
            {
                Reward(PlayerList[player.Key]);
                RemovePlayer(PlayerList[player.Key]);
            }
            PlayerList.Clear();
            PlayerScores.Clear();
            Program.Events.Remove(this);
            return;
        }

        public override void Reward(GameClient client)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                Rank++;
                switch (Rank)
                {
                    case 1:
                        {
                            client.IncreaseExperience(stream, 10000, Role.Flags.ExperienceEffect.angelwing);
                            uint value = RewardCps;
                            client.Player.ConquerPoints += value;
                            client.CreateBoxDialog($"You've received {value} ConquerPoints for Rank {Rank} in {EventTitle} Tournament!.");
                            string msg = $"{client.Player.Name} has won {value} ConquerPoints from Rank {Rank} in the hourly {EventTitle} Tournament!";
                            Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage(msg, MsgColor.red, ChatMode.System).GetArray(stream));
                            string reward = "[EVENT]" + msg;
                            Database.ServerDatabase.LoginQueue.Enqueue(reward);
                            break;
                        }
                    default:
                        {
                            client.IncreaseExperience(stream, 5000, Role.Flags.ExperienceEffect.angelwing);
                            uint value = RewardCps - (Rank * 10);
                            if (value < 5)
                                value = 5;
                            client.Player.ConquerPoints += value;
                            client.CreateBoxDialog($"You've received {value} ConquerPoints for Rank {Rank} in {EventTitle} Tournament!.");
                            string msg = $"{client.Player.Name} has won {value} ConquerPoints from Rank {Rank} in the hourly {EventTitle} Tournament!";
                            Program.SendGlobalPackets.Enqueue(new Game.MsgServer.MsgMessage(msg, MsgColor.red, ChatMode.System).GetArray(stream));
                            string reward = "[EVENT]" + msg;
                            Database.ServerDatabase.LoginQueue.Enqueue(reward);
                            break;
                        }
                }
            }
        }

        public override void DisplayScore()
        {
            DisplayScores = DateTime.Now;
            foreach (var player in PlayerList.Values.ToArray())
            {
                player.SendSysMesage($"---------{EventTitle}---------", ChatMode.FirstRightCorner);
            }
            TimeSpan T = TimeSpan.FromSeconds(Duration);
            Broadcast($"Time left {T.ToString(@"mm\:ss")}", BroadCastLoc.Score);
            Broadcast($"Players left: {PlayerList.Count}", BroadCastLoc.Score);
            if (Duration > 0)
                --Duration;
        }
    }
}