using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Extensions;

namespace DeathWish.Game.MsgTournaments
{
    public class MsgWarOfPlayers
    {
        public static System.Collections.Generic.SafeDictionary<uint, Info> WarOfPlayersList = new System.Collections.Generic.SafeDictionary<uint, Info>();

        public static Info RoundOwner;

        public static ProcesType Proces = ProcesType.Dead;

        public static Dictionary<Role.SobNpc.StaticMesh, Role.SobNpc> Furnitures = new Dictionary<Role.SobNpc.StaticMesh, Role.SobNpc>();
        public static ushort MapID = 7979;

        public class Info
        {
            public uint ClanID;
            public uint Score;
            public string ClanName;
        }

        public static void Load()
        {
            Furnitures.Add(Role.SobNpc.StaticMesh.Pole, Database.Server.ServerMaps[MapID].View.GetMapObject<Role.SobNpc>(Role.MapObjectType.SobNpc, 2515));
        }

        public static void Start()
        {
            RoundOwner = null;
            WarOfPlayersList = new System.Collections.Generic.SafeDictionary<uint, Info>();
            Proces = ProcesType.Alive;
            MsgSchedules.SendInvitation("Clans-Pole has been begun ", "100K Cps + 2 DIABLO[Spin] + 1 E-P", 448, 353, 1002, 0, 300);
            MsgSchedules.SendSysMesage("Clans-Pole  has been begun..");
        }

        public static void End()
        {
            if (Proces == ProcesType.Alive)
            {
                Proces = ProcesType.Dead;
                MsgSchedules.SendSysMesage(RoundOwner == null ? "Clans-Pole  has ended and there is no winner" : " Clan " + RoundOwner.ClanName + ",  is the winner of Clans-Pole .", MsgServer.MsgMessage.ChatMode.BroadcastMessage);
                WarOfPlayersList.Clear();
            }
        }

        public static void CheckUP()
        {
           // DateTime Now = DateTime.Now;

            //if (Proces == ProcesType.Dead && Now.Hour != 20 && Now.Hour != 21 && Now.Hour != 22 && Now.Minute == 55 && Now.Second == 05)
            //    Start();

            //if (Proces == ProcesType.Alive)
            //{
            //    if (Now.Minute == 58 && Now.Second == 01)
            //        MsgSchedules.SendSysMesage("Clans-Pole  will ended after 2 Minutes .", MsgServer.MsgMessage.ChatMode.Center);


            //    if (Now.Minute == 59 && Now.Second == 58)
            //        End();
            //}
        }

        public static void Reset(ServerSockets.Packet stream)
        {
            WarOfPlayersList = new System.Collections.Generic.SafeDictionary<uint, Info>();

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
            Furnitures[Role.SobNpc.StaticMesh.Pole].Name = RoundOwner.ClanName;
            MsgSchedules.SendSysMesage("Clans-Pole The Round Ownered by " + RoundOwner.ClanName + ".", MsgServer.MsgMessage.ChatMode.Center);
            Reset(stream);
        }
        public static void UpdateScore(ServerSockets.Packet stream, Role.Player client, uint Damage)
        {
            if (!WarOfPlayersList.ContainsKey(client.ClanUID))
                WarOfPlayersList.Add(client.ClanUID, new Info() { ClanID = client.MyClan.ID, ClanName = client.MyClan.Name, Score = Damage });
            else
                WarOfPlayersList[client.ClanUID].Score += Damage;

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
                var Array = WarOfPlayersList.Values.ToArray();
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
                        Game.MsgServer.MsgMessage msg = new MsgServer.MsgMessage("No " + (x + 1).ToString() + ". " + element.ClanName + " (" + element.Score + ")"
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
