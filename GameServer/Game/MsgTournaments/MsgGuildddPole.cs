using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Extensions;

namespace DeathWish.Game.MsgTournaments
{
    public class MsgVeteransWar
    {
        public static System.Collections.Generic.SafeDictionary<uint, Info> VeteransList = new System.Collections.Generic.SafeDictionary<uint, Info>();

        public static Info RoundOwner;

        public static ProcesType Proces = ProcesType.Dead;

        public static Dictionary<Role.SobNpc.StaticMesh, Role.SobNpc> Furnitures = new Dictionary<Role.SobNpc.StaticMesh, Role.SobNpc>();
        /// <summary>
        /// SobNpc 
        /// 6525,VeteransWar,10,1137,6526,95,113,20000000,20000000,17,1
        /// Npc 1002, 283, 290
        /// 6525,2,9696,1002,283,290
        /// </summary>
        public static ushort MapID = 6979;

        public class Info
        {
            public uint GuildID;
            public uint Score;
            public string GuildName;
        }

        public static void Load()
        {
            Furnitures.Add(Role.SobNpc.StaticMesh.Pole, Database.Server.ServerMaps[MapID].View.GetMapObject<Role.SobNpc>(Role.MapObjectType.SobNpc, 65255));
        }

        public static void Start()
        {
            RoundOwner = null;
            VeteransList = new System.Collections.Generic.SafeDictionary<uint, Info>();
            Proces = ProcesType.Alive;
            MsgSchedules.SendInvitation("Guilds-Pole ", "Special Reward ", 449, 357, 1002, 0, 300);
            MsgSchedules.SendSysMesage("Guilds-Pole  has been begun..");
        }

        public static void End()
        {
            if (Proces == ProcesType.Alive)
            {
                Proces = ProcesType.Dead;
                MsgSchedules.SendSysMesage(RoundOwner == null ? "Guilds-Pole  has ended and there is no winner" : " Guild " + RoundOwner.GuildName + ", is the winner of Guilds-Pole   .", MsgServer.MsgMessage.ChatMode.Center);
                VeteransList.Clear();
            }
        }

        public static void CheckUP()
        {
            DateTime Now = DateTime.Now;

            if (Proces == ProcesType.Dead && Now.Hour != 20 && Now.Hour != 23 && Now.Minute == 50 && Now.Second == 30)
                Start();

            if (Proces == ProcesType.Alive)
            {
                if (Now.Minute == 53 && Now.Second == 01)
                    MsgSchedules.SendSysMesage("Guilds-Pole  will ended after 2 Minutes.", MsgServer.MsgMessage.ChatMode.Center);

                if (Now.Minute == 55 && Now.Second >= 00)
                    End();
            }
        }

        public static void Reset(ServerSockets.Packet stream)
        {
            VeteransList = new System.Collections.Generic.SafeDictionary<uint, Info>();

            foreach (var npc in Furnitures.Values)
                npc.HitPoints = npc.MaxHitPoints;

            var Pole = Furnitures[Role.SobNpc.StaticMesh.Pole];
            var users = Database.Server.GamePoll.Values.Where(u => Role.Core.GetDistance(u.Player.X, u.Player.Y, Pole.X, Pole.Y) <= Role.SobNpc.SeedDistrance).ToArray();
            if (users != null)
            {
                foreach (var user in users)
                {
                    MsgServer.MsgUpdate upd = new MsgServer.MsgUpdate(stream, Pole.UID, 2);
                    stream = upd.Append(stream, MsgServer.MsgUpdate.DataType.Mesh, (long)Pole.Mesh);
                    stream = upd.Append(stream, MsgServer.MsgUpdate.DataType.Hitpoints, Pole.HitPoints);
                    stream = upd.GetArray(stream);
                    user.Send(stream);
                    if ((Role.SobNpc.StaticMesh)Pole.Mesh == Role.SobNpc.StaticMesh.Pole)
                        user.Send(Pole.GetArray(stream, false));
                }
            }
        }
        public static void FinishRound(ServerSockets.Packet stream)
        {
            SortScores(true);
            Furnitures[Role.SobNpc.StaticMesh.Pole].Name = RoundOwner.GuildName;
            MsgSchedules.SendSysMesage("Guilds-Pole The Round Ownered by " + RoundOwner.GuildName + ".", MsgServer.MsgMessage.ChatMode.Center);
            Reset(stream);
        }
        public static void UpdateScore(ServerSockets.Packet stream, uint Score, Role.Instance.Guild Guild)
        {
            if (!VeteransList.ContainsKey(Guild.Info.GuildID))
                VeteransList.Add(Guild.Info.GuildID, new Info() { GuildName = Guild.GuildName, GuildID = Guild.Info.GuildID, Score = Score });
            else
                VeteransList[Guild.Info.GuildID].Score += Score;

            SortScores();

            if (Furnitures[Role.SobNpc.StaticMesh.Pole].HitPoints < 1)
            {
                FinishRound(stream);
                return;
            }
        }
        private static void SortScores(bool getwinner = false)
        {
            if (Proces != ProcesType.Dead)
            {
                var Array = VeteransList.Values.ToArray();
                var DescendingList = Array.OrderByDescending(p => p.Score).ToArray();
                for (int x = 0; x < DescendingList.Length; x++)
                {
                    var element = DescendingList[x];
                    if (x == 0 && getwinner)
                    {
                        RoundOwner = element;
                    }
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        Game.MsgServer.MsgMessage msg = new MsgServer.MsgMessage("No " + (x + 1).ToString() + ". " + element.GuildName + " (" + element.Score + ")"
                           , MsgServer.MsgMessage.MsgColor.yellow, x == 0 ? MsgServer.MsgMessage.ChatMode.FirstRightCorner : MsgServer.MsgMessage.ChatMode.ContinueRightCorner);

                        SendMapPacket(msg.GetArray(rec.GetStream()));
                    }
                    if (x == 4)
                        break;
                }
            }
        }
        public static void SendMapPacket(ServerSockets.Packet packet)
        {
            foreach (var client in Database.Server.ServerMaps[MapID].Values)
                client.Send(packet);
        }

    }
}
