using DeathWish.Client;
using DeathWish.Database;
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
    public class SkyFight : Events
    {
        public SkyFight()
        {
            IDEvent = 8;
            EventTitle = "SkyFight";
            IDMessage = MsgStaticMessage.Messages.SkyFight;
            NoDamage = false;
            MagicAllowed = true;
            MeleeAllowed = true;
            FriendlyFire = true;
            FlyAllowed = false;
            BaseMap = 700;
            ReviveAllowed = false;
            Duration = 180;
        }

        public override void TeleportPlayersToMap()
        {
            base.TeleportPlayersToMap();
        }

        public override void CharacterChecks(GameClient C)
        {
            base.CharacterChecks(C);
        }

        public override void Kill(GameClient Attacker, GameClient Victim)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                Victim.Player.Revive(stream);
            }
            Victim.Player.ProtectAttack(13 * 1000);
            Victim.Player.AddFlag(MsgUpdate.Flags.Freeze, 10, true);
            Victim.Player.AddFlag(MsgUpdate.Flags.Fly, 10, true);
            PlayerScores[Attacker.EntityID] += 1;
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
            Program.Events.Remove(this);
            return;
        }

        public override void Reward(GameClient client)
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
                player.SendSysMesage($"---------{EventTitle}---------", MsgServer.MsgMessage.ChatMode.FirstRightCorner);
            }
            byte Score = 2;
            foreach (var kvp in PlayerScores.OrderByDescending((s => s.Value)).ToArray())
            {
                if (Score == 7)
                    break;
                //Broadcast($"Nยบ {Score - 1}: {PlayerList[kvp.Key].Name} - {kvp.Value}", BroadCastLoc.Score, Score);
                Broadcast($"Rank {Score - 1}: {PlayerList[kvp.Key].Player.Name} - Score  : {kvp.Value}", BroadCastLoc.Score, Score);
                Score++;
            }
            TimeSpan T = TimeSpan.FromSeconds(Duration);
            Broadcast($"Time left {T.ToString(@"mm\:ss")}", BroadCastLoc.Score);
            if (Duration > 0)
                --Duration;
        }
    }
}