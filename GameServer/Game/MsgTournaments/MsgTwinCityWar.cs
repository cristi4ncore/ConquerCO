//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Extensions;

//namespace DeathWish.Game.MsgTournaments
//{
//    public class MsgTwinCityWar
//    {
//        public static System.Collections.Generic.SafeDictionary<uint, Info> MsgTwinCityWarList = new System.Collections.Generic.SafeDictionary<uint, Info>();

//        public static Info RoundOwner;

//        public static ProcesType Proces = ProcesType.Dead;

//        public static Dictionary<Role.SobNpc.StaticMesh, Role.SobNpc> Furnitures = new Dictionary<Role.SobNpc.StaticMesh, Role.SobNpc>();
//        /// <summary>
//        /// SobNpc 
//        /// 6525,VeteransWar,10,1137,6526,95,113,20000000,20000000,17,1
//        /// Npc 1002, 283, 290
//        /// 6525,2,9696,1002,283,290
//        /// </summary>
//        public static ushort MapID = 2002;

//        public class Info
//        {
//            public uint GuildID;
//            public uint Score;
//            public string GuildName;
//        }

//        public static void Load()
//        {
//            Furnitures.Add(Role.SobNpc.StaticMesh.Pole, Database.Server.ServerMaps[MapID].View.GetMapObject<Role.SobNpc>(Role.MapObjectType.SobNpc, 2002655));
//        }

//        internal unsafe void Start()
//        {
//            RoundOwner = null;
//            MsgTwinCityWarList = new System.Collections.Generic.SafeDictionary<uint, Info>();
//            Proces = ProcesType.Alive;
//            MsgSchedules.SendInvitation("TwinWar-Pole ", "Special Reward ", 449, 357, 1002, 0, 300);
//            MsgSchedules.SendSysMesage("TwinWar-Pole  has been begun..");
//            using (var rec = new ServerSockets.RecycledPacket())
//            {
//                var stream = rec.GetStream();
//                ResetFurnitures(stream);
//            }
//        }

//        public static void End()
//        {
//            if (Proces == ProcesType.Alive)
//            {
//                Proces = ProcesType.Dead;
//                MsgSchedules.SendSysMesage(RoundOwner == null ? "TwinWar-Pole  has ended and there is no winner" : " Guild " + RoundOwner.GuildName + ", is the winner of TwinWar-Pole   .", MsgServer.MsgMessage.ChatMode.Center);
//                MsgTwinCityWarList.Clear();
//            }
//        }
//        public unsafe void CreateFurnitures()
//        {
//            Furnitures.Add(Role.SobNpc.StaticMesh.LeftGate, Database.Server.ServerMaps[2002].View.GetMapObject<Role.SobNpc>(Role.MapObjectType.SobNpc, 516974));
//            Furnitures.Add(Role.SobNpc.StaticMesh.RightGate, Database.Server.ServerMaps[2002].View.GetMapObject<Role.SobNpc>(Role.MapObjectType.SobNpc, 516975));
//            Furnitures.Add(Role.SobNpc.StaticMesh.Pole, Database.Server.ServerMaps[2002].View.GetMapObject<Role.SobNpc>(Role.MapObjectType.SobNpc, 2002655));
//        }
//        internal unsafe void ResetFurnitures(ServerSockets.Packet stream)
//        {
//            Furnitures[Role.SobNpc.StaticMesh.LeftGate].Mesh = Role.SobNpc.StaticMesh.LeftGate;
//            Furnitures[Role.SobNpc.StaticMesh.RightGate].Mesh = Role.SobNpc.StaticMesh.RightGate;

//            foreach (var npc in Furnitures.Values)
//                npc.HitPoints = npc.MaxHitPoints;

//            foreach (var client in Database.Server.GamePoll.Values)
//            {
//                if (client.Player.Map == 2002)
//                {
//                    foreach (var npc in Furnitures.Values)
//                    {
//                        if (Role.Core.GetDistance(client.Player.X, client.Player.Y, npc.X, npc.Y) <= Role.SobNpc.SeedDistrance)
//                        {
//                            MsgServer.MsgUpdate upd = new MsgServer.MsgUpdate(stream, npc.UID, 2);
//                            stream = upd.Append(stream, MsgServer.MsgUpdate.DataType.Mesh, (long)npc.Mesh);
//                            stream = upd.Append(stream, MsgServer.MsgUpdate.DataType.Hitpoints, npc.HitPoints);
//                            stream = upd.GetArray(stream);
//                            client.Send(stream);
//                            if ((Role.SobNpc.StaticMesh)npc.Mesh == Role.SobNpc.StaticMesh.Pole)
//                                client.Send(npc.GetArray(stream, false));
//                        }
//                    }
//                }
//            }
//        }
//        public static void Reset(ServerSockets.Packet stream)
//        {
//            MsgTwinCityWarList = new System.Collections.Generic.SafeDictionary<uint, Info>();

//            foreach (var npc in Furnitures.Values)
//                npc.HitPoints = npc.MaxHitPoints;

//            var Pole = Furnitures[Role.SobNpc.StaticMesh.Pole];
//            var users = Database.Server.GamePoll.Values.Where(u => Role.Core.GetDistance(u.Player.X, u.Player.Y, Pole.X, Pole.Y) <= Role.SobNpc.SeedDistrance).ToArray();
//            if (users != null)
//            {
//                foreach (var user in users)
//                {
//                    MsgServer.MsgUpdate upd = new MsgServer.MsgUpdate(stream, Pole.UID, 2);
//                    stream = upd.Append(stream, MsgServer.MsgUpdate.DataType.Mesh, (long)Pole.Mesh);
//                    stream = upd.Append(stream, MsgServer.MsgUpdate.DataType.Hitpoints, Pole.HitPoints);
//                    stream = upd.GetArray(stream);
//                    user.Send(stream);
//                    if ((Role.SobNpc.StaticMesh)Pole.Mesh == Role.SobNpc.StaticMesh.Pole)
//                        user.Send(Pole.GetArray(stream, false));
//                }
//            }
//        }
//        internal unsafe void FinishRound()
//        {
//            SortScores(true);
//            Furnitures[Role.SobNpc.StaticMesh.Pole].Name = RoundOwner.GuildName;
//            MsgSchedules.SendSysMesage("Guilds-Pole The Round Ownered by " + RoundOwner.GuildName + ".", MsgServer.MsgMessage.ChatMode.Center);
//            using (var rec = new ServerSockets.RecycledPacket())
//            {
//                var stream = rec.GetStream();
//                Reset(stream);
//                ResetFurnitures(stream);
//            }
//        }
//        internal void UpdateScore(Role.Player client, uint Damage)
//        {
//            if (!MsgTwinCityWarList.ContainsKey(client.GuildID))
//                MsgTwinCityWarList.Add(client.GuildID, new Info() { GuildID = client.MyGuild.Info.GuildID, GuildName = client.MyGuild.GuildName, Score = Damage });
//            else
//                MsgTwinCityWarList[client.GuildID].Score += Damage;

//            SortScores();

//            if (Furnitures[Role.SobNpc.StaticMesh.Pole].HitPoints < 1)
//            {
//                FinishRound();
//                return;
//            }
//        }
//        internal unsafe void SortScores(bool getwinner = false)
//        {
//            if (Proces != ProcesType.Dead)
//            {
//                var Array = MsgTwinCityWarList.Values.ToArray();
//                var DescendingList = Array.OrderByDescending(p => p.Score).ToArray();
//                for (int x = 0; x < DescendingList.Length; x++)
//                {
//                    var element = DescendingList[x];
//                    if (x == 0 && getwinner)
//                    {
//                        RoundOwner = element;
//                    }
//                    using (var rec = new ServerSockets.RecycledPacket())
//                    {
//                        Game.MsgServer.MsgMessage msg = new MsgServer.MsgMessage("No " + (x + 1).ToString() + ". " + element.GuildName + " (" + element.Score + ")"
//                           , MsgServer.MsgMessage.MsgColor.yellow, x == 0 ? MsgServer.MsgMessage.ChatMode.FirstRightCorner : MsgServer.MsgMessage.ChatMode.ContinueRightCorner);

//                        SendMapPacket(msg.GetArray(rec.GetStream()));
//                    }
//                    if (x == 4)
//                        break;
//                }
//            }
//        }
//        public static void SendMapPacket(ServerSockets.Packet packet)
//        {
//            foreach (var client in Database.Server.ServerMaps[MapID].Values)
//                client.Send(packet);
//        }

//    }
//}
