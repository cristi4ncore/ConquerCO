using DeathWish.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DeathWish.Role.Flags;
using static DeathWish.Game.MsgServer.MsgMessage;
using DeathWish.Game.MsgServer;

namespace DeathWish.Game.MsgEvents
{
    class VALORANT : Events
    {
        public VALORANT()
        {
            IDEvent = 4;
            EventTitle = "VALORANT";
            IDMessage = MsgStaticMessage.Messages.VALORANT;
            BaseMap = 7777;
            Reflect = false;
            NoDamage = false;
            MagicAllowed = false;
            MeleeAllowed = false;
            FriendlyFire = true;
            AllowedSkills = new System.Collections.Generic.List<ushort> { (ushort)1045, (ushort)1046, (ushort)1047 };
            Duration = 300;
            PotionsAllowed = false;
        }
        public override void TeleportPlayersToMap()
        {
            base.TeleportPlayersToMap();
            foreach (GameClient client in PlayerList.Values.ToArray())
            {
                client.Player.HitPoints = (int)client.Status.MaxHitpoints;
            }
        }

        public override void CharacterChecks(GameClient C)
        {
            base.CharacterChecks(C);
            if (!C.Player.Alive)
            {
                if (C.Player.DeadStamp.AddSeconds(3) < Extensions.Time32.Now)
                {
                    C.EventBase?.RemovePlayer(C);
                }
            }
        }

        public override void Kill(GameClient Attacker, GameClient Victim)
        {
            //RemovePlayer(Victim);
        }

        public override void End()
        {
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
        public uint Win_TopLastMan = 0;

        public void Reward(Client.GameClient client, ServerSockets.Packet stream)
        {
            base.Reward(client);
        }
        public override void Inviting()
        {
            base.Inviting();
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