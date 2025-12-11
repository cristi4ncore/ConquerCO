using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Extensions;

namespace DeathWish.Game.MsgTournaments
{
    public class MsgEmperorWar
    {

        public static System.Collections.Generic.SafeDictionary<uint, Info> EmperrosList = new System.Collections.Generic.SafeDictionary<uint, Info>();

        public static Info RoundOwner;

        public static ProcesType Proces = ProcesType.Dead;

        public static Dictionary<Role.SobNpc.StaticMesh, Role.SobNpc> Furnitures = new Dictionary<Role.SobNpc.StaticMesh, Role.SobNpc>();
        /// <summary>
        /// SobNpc 
        /// 6525,Emperrors,10,1137,6521,95,113,20000000,20000000,17,1
        /// Npc 1002, 283, 290
        /// 6525,2,9696,1002,283,290
        /// </summary>
        public static ushort MapID = 5979;

        public class Info
        {
            public uint UID;
            public uint Score;
            public string Name;
        }

        public static void Load()
        {
            Furnitures.Add(Role.SobNpc.StaticMesh.Pole, Database.Server.ServerMaps[MapID].View.GetMapObject<Role.SobNpc>(Role.MapObjectType.SobNpc, 6525));
        }

        public static void Start()
        {
            RoundOwner = null;
            EmperrosList = new System.Collections.Generic.SafeDictionary<uint, Info>();
            Proces = ProcesType.Alive;
            MsgSchedules.SendInvitation("Players-Pole ", "Special Reward ", 449, 357, 1002, 0, 300);
            MsgSchedules.SendSysMesage("Players-Pole  has been begun..");
        }

        public static void End()
        {
            if (Proces == ProcesType.Alive)
            {
                Proces = ProcesType.Dead;
                MsgSchedules.SendSysMesage(RoundOwner == null ? "Players-Pole  has ended and there is no winner" : "" + RoundOwner.Name + ",  is the winner of Players-Pole  .", MsgServer.MsgMessage.ChatMode.Center);
                EmperrosList.Clear();
            }
        }

        public static void CheckUP()
        {
            DateTime Now = DateTime.Now;

            if (Proces == ProcesType.Dead && Now.Hour != 20 && Now.Hour != 23 && Now.Minute == 45 && Now.Second == 01)
                Start();

            if (Proces == ProcesType.Alive)
            {
                    if (Now.Minute == 48 && Now.Second == 00)
                   MsgSchedules.SendSysMesage("Players-Pole  will ended after 2 Minutes.", MsgServer.MsgMessage.ChatMode.Center);

                if (Now.Minute == 50 && Now.Second >= 00)
                    End();
            }
        }

        public static void Reset(ServerSockets.Packet stream)
        {
            EmperrosList = new System.Collections.Generic.SafeDictionary<uint, Info>();

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
            Furnitures[Role.SobNpc.StaticMesh.Pole].Name = RoundOwner.Name;
            MsgSchedules.SendSysMesage("Players-Pole The Round Ownered by " + RoundOwner.Name + ".", MsgServer.MsgMessage.ChatMode.Center);
            Reset(stream);
        }
        public static void UpdateScore(ServerSockets.Packet stream, uint Score, Role.Player Player)
        {
            if (!EmperrosList.ContainsKey(Player.UID))
                EmperrosList.Add(Player.UID, new Info() { Name = Player.Name, UID = Player.UID, Score = Score });
            else
                EmperrosList[Player.UID].Score += Score;

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
                var Array = EmperrosList.Values.ToArray();
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
                        Game.MsgServer.MsgMessage msg = new MsgServer.MsgMessage("No " + (x + 1).ToString() + ". " + element.Name + " (" + element.Score + ")"
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
