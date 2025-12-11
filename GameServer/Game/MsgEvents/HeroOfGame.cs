using DeathWish.Client;
using DeathWish.Game.MsgServer;
using DeathWish.Role;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DeathWish.Game.MsgServer.MsgMessage;

namespace DeathWish.Game.MsgEvents
{
    public class HeroOfGame : Events
    {
        public static Statue StaticStatue = null;
        public static SobNpc StaticSobNpc = null;

        public HeroOfGame()
        {
            IDEvent = 9;
            EventTitle = "HeroOfGame";
            Duration = 10;
            IDMessage = MsgStaticMessage.Messages.HeroOfGame;
            NoDamage = false;
            MagicAllowed = true;
            MeleeAllowed = true;
            FriendlyFire = true;
            FlyAllowed = false;
            BaseMap = 1507;
            Duration = 180;
        }

        public override void Reward(GameClient client)
        {
            HeroStatue(client);
            base.Reward(client);
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

        public static void HeroStatue(Client.GameClient user)
        {
            if (StaticStatue == null && StaticSobNpc == null)
            {
                CreateStatue(user, 439, 368, 0, 0, true);
            }
            else
            {
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    RemoveStatue(stream, user, StaticSobNpc.UID, StaticSobNpc);
                    CreateStatue(user, 439, 368, 0, 0, true);
                }
            }
        }

        public static unsafe void CreateStatue(Client.GameClient client, ushort x, ushort y, int Action, int action2, bool Static = false)
        {
            try
            {
                Statue stat = new Statue();
                stat.user = client;
                stat.UID = Statue.CounterUID.Next;
                stat.HitPotion = client.Player.HitPoints / 100;
                stat.Action = Role.Flags.ConquerAction.Cool;
                stat.Angle = Flags.ConquerAngle.East;
                stat.Static = Static;
                StaticStatue = stat;
                using (var rec = new ServerSockets.RecycledPacket())
                {
                    var stream = rec.GetStream();
                    SobNpc npc = new SobNpc(stat);
                    npc.ObjType = MapObjectType.SobNpc;
                    npc.UID = stat.UID;
                    npc.X = x;
                    npc.Y = y;
                    npc.Map = 1002;
                    npc.Name = client.Player.Name;
                    npc.MaxHitPoints = 0;
                    npc.HitPoints = 0;
                    client.Player.View.SendView(npc.GetArray(stream, false), true);
                    client.Map.View.EnterMap<Role.IMapObj>(npc);
                    if (Static)
                        StaticSobNpc = npc;
                }
            }
            catch (Exception e) { MyConsole.SaveException(e); }
        }

        public static unsafe void RemoveStatue(ServerSockets.Packet stream, Client.GameClient killer, uint UID, Role.IMapObj obj)
        {
            Role.GameMap map;
            if (killer.Map != null)
                map = killer.Map;
            else
                map = Database.Server.ServerMaps[1002];

            map.View.LeaveMap<Role.IMapObj>(obj);

            ActionQuery action = new ActionQuery()
            {
                ObjId = UID,
                Type = ActionType.RemoveEntity
            };
            killer.Player.View.SendView(stream.ActionCreate(&action), true);
        }
    }
}