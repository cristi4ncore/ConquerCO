//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace DeathWish.Game.MsgTournaments
//{
//    public class MsgDragonWar : ITournament
//    {
//        public const uint MapID = 2571, RewardConquerPoints = 200000;

//        public ProcesType Process { get; set; }

//        public DateTime StartTimer = new DateTime();
//        public DateTime InfoTimer = new DateTime();
//        public DateTime ScoreStamp = new DateTime();
//        public KillerSystem KillSystem;
//        public uint Secounds = 120;
//        public uint DinamicID = 0;
//        public Role.GameMap Map;
//        public TournamentType Type { get; set; }
//        public MsgDragonWar(TournamentType _type)
//        {
//            Type = _type;
//            Process = ProcesType.Dead;

//        }
//        public void Open()
//        {
//            if (Process == ProcesType.Dead)
//            {
//                KillSystem = new KillerSystem();
//                if (Map == null)
//                {
//                    Map = Database.Server.ServerMaps[MapID];
//                    DinamicID = Map.GenerateDynamicID();
//                }

//                StartTimer = DateTime.Now;
//                Process = ProcesType.Idle;
//#if Arabic
//                MsgSchedules.SendInvitation("DragonWar[SS/FB]", "ConquerPoints, 4-PowerExpBalls", 448, 353, 1002, 0, 60);
//#else
//                MsgSchedules.SendInvitation("DragonWar[SS/FB]", "200K Cps + 2 DIABLO[Spin] + 10 E-P", 452, 354, 1002, 0, 120);
//#endif

//                InfoTimer = DateTime.Now.AddSeconds(10);
//                Secounds = 120;
//            }
//        }

//        public bool Join(Client.GameClient client, ServerSockets.Packet stream)
//        {
//            if (Process == ProcesType.Idle)
//            {
//                client.Player.DragonWarScore = 0;
//                client.Player.DragonWarHits = 0;
//                client.Player.TournamentKills = 0;
//                client.Player.SetPkMode(DeathWish.Role.Flags.PKMode.PK);
//                ushort x = 0;
//                ushort y = 0;
//                Map.GetRandCoord(ref x, ref y);
//                client.Teleport(x, y, Map.ID, DinamicID);
//                if (client.Player.ContainFlag(MsgServer.MsgUpdate.Flags.DragonSwing))
//                    client.Player.RemoveFlag(MsgServer.MsgUpdate.Flags.DragonSwing);
//                return true;
//            }
//            return false;
//        }
//        public bool InTournament(Client.GameClient user)
//        {
//            if (Map == null)
//                return false;
//            return user.Player.Map == Map.ID && user.Player.DynamicID == DinamicID;
//        }
//        public void DragonWarRevive(Extensions.Time32 Timer, Client.GameClient user)
//        {
//            if (user.Player.Alive == false && Process != ProcesType.Dead)
//            {
//                if (InTournament(user))
//                {
//                    if (user.Player.DeadStamp.AddSeconds(4) < Timer)
//                    {
//                        ushort x = 0;
//                        ushort y = 0;
//                        Map.GetRandCoord(ref x, ref y);
//                        user.Teleport(x, y, Map.ID, DinamicID);
//                    }
//                }
//            }
//        }
//        public void CheckUp()
//        {

//            if (Process == ProcesType.Idle)
//            {
//                if (DateTime.Now > StartTimer.AddMinutes(2))
//                {
//#if Arabic
//                     MsgSchedules.SendSysMesage("DragonWar has started! signup are now closed.", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);
                   
//#else
//                    MsgSchedules.SendSysMesage("DragonWar has started! signup are now closed.", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);

//#endif
//                    StartTimer = DateTime.Now;
//                    Process = ProcesType.Alive;
//                    if (MapPlayers().Length > 0)
//                    {
//                        MapPlayers().First().Player.DragonWarHits = 2;
//                        MapPlayers().First().Player.AddFlag(MsgServer.MsgUpdate.Flags.DragonSwing, Role.StatusFlagsBigVector32.PermanentFlag, true);

//                    }
//                }
//                else if (DateTime.Now > InfoTimer)
//                {
//                    Secounds -= 10;
//#if Arabic
//                      MsgSchedules.SendSysMesage("Fight starts in " + Secounds.ToString() + " Secounds.", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);
                   
//#else
//                    MsgSchedules.SendSysMesage("Fight starts in " + Secounds.ToString() + " Secounds.", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);

//#endif
//                    InfoTimer = DateTime.Now.AddSeconds(10);
//                }
//            }
//            if (Process == ProcesType.Alive)
//            {
//                if (DateTime.Now > StartTimer.AddMinutes(3))
//                {
//#if Arabic
//   MsgSchedules.SendSysMesage("DragonWar has ended. All Players of DragonWar has teleported to TwinCity.", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);

//#else
//                    MsgSchedules.SendSysMesage("DragonWar has ended. All Players of DragonWar has teleported to TwinCity.", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);

//#endif

//                    var array = MapPlayers().OrderByDescending(p => p.Player.DragonWarScore).ToArray();
//                    if (array.Length > 0)
//                    {
//                        var Winner = array.First();

//#if Arabic
//                         MsgSchedules.SendSysMesage("" + Winner.Player.Name + " has Won  DragonWar. ", MsgServer.MsgMessage.ChatMode.BroadcastMessage, MsgServer.MsgMessage.MsgColor.white);
//#else

//                        MsgSchedules.SendSysMesage("" + Winner.Player.Name + " has Won  DragonWar 200.000 Cps & 2 DIABLO Spin & 10 Event Point. ", MsgServer.MsgMessage.ChatMode.BroadcastMessage, MsgServer.MsgMessage.MsgColor.white);
//#endif


//                        Winner.Player.ConquerPoints += RewardConquerPoints;

//                        Winner.Player.PIKAPoint += 10;
//                        using (var rec = new ServerSockets.RecycledPacket())
//                        {
//                            var stream = rec.GetStream();
//                            Winner.Inventory.Add(stream, 3008733, 2);
//                            if (Winner.Player.ContainFlag(MsgServer.MsgUpdate.Flags.DragonSwing))
//                                Winner.Player.RemoveFlag(MsgServer.MsgUpdate.Flags.DragonSwing);
//                        }
//                    }
//                    int x = 1;
//                    foreach (var user in array)
//                    {
//                        if (x > 1)
//                        {
//                            //  user.Player.ConquerPoints += (uint)(RewardConquerPoints / x);
//#if Arabic
//                                   user.SendSysMesage("You received " + (RewardConquerPoints / x).ToString() + " ConquerPoints. ", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
                     
//#else
//                            // user.SendSysMesage("You received " + (RewardConquerPoints / x).ToString() + " ConquerPoints. ", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);

//#endif
//                        }
//                        x++;
//                        user.Teleport(428, 379, 1002);
//                        user.Player.SetPkMode(DeathWish.Role.Flags.PKMode.Capture);
//                        if (user.Player.ContainFlag(MsgServer.MsgUpdate.Flags.DragonSwing))
//                            user.Player.RemoveFlag(MsgServer.MsgUpdate.Flags.DragonSwing);
//                    }
//                    Process = ProcesType.Dead;
//                }
//                if (!ExistDragonWarEffect())
//                {
//                    if (MapPlayers().Length > 0)
//                    {
//                        MapPlayers().First().Player.DragonWarHits = 3;
//                        MapPlayers().First().Player.AddFlag(MsgServer.MsgUpdate.Flags.DragonSwing, Role.StatusFlagsBigVector32.PermanentFlag, true);
//                    }
//                }
//                if (DateTime.Now > ScoreStamp)
//                {

//                    using (var rec = new ServerSockets.RecycledPacket())
//                    {
//                        var stream = rec.GetStream();
//                        var array = MapPlayers().OrderByDescending(p => p.Player.DragonWarScore).ToArray();

//#if Arabic
//                          Game.MsgServer.MsgMessage msg = new MsgServer.MsgMessage("DragonWar Score: ", MsgServer.MsgMessage.MsgColor.yellow, MsgServer.MsgMessage.ChatMode.FirstRightCorner);
                      
//#else
//                        Game.MsgServer.MsgMessage msg = new MsgServer.MsgMessage("DragonWar Score: ", MsgServer.MsgMessage.MsgColor.yellow, MsgServer.MsgMessage.ChatMode.FirstRightCorner);

//#endif
//                        SendMapPacket(msg.GetArray(stream));

//                        int x = 0;
//                        foreach (var obj in array)
//                        {
//                            if (x == 4)
//                                break;
//#if Arabic
//                               Game.MsgServer.MsgMessage amsg = new MsgServer.MsgMessage("No " + (x + 1).ToString() + ". " + obj.Player.Name + " (" + obj.Player.DragonWarScore.ToString() + ")", MsgServer.MsgMessage.MsgColor.yellow, MsgServer.MsgMessage.ChatMode.ContinueRightCorner);
                         
//#else
//                            Game.MsgServer.MsgMessage amsg = new MsgServer.MsgMessage("No " + (x + 1).ToString() + ". " + obj.Player.Name + " (" + obj.Player.DragonWarScore.ToString() + ")", MsgServer.MsgMessage.MsgColor.yellow, MsgServer.MsgMessage.ChatMode.ContinueRightCorner);

//#endif
//                            SendMapPacket(amsg.GetArray(stream));

//                            x++;
//                        }
//                        foreach (var user in MapPlayers())
//                        {
//#if Arabic
//                              msg = new MsgServer.MsgMessage("My tournament Kills: " + user.Player.TournamentKills.ToString() + "" , MsgServer.MsgMessage.MsgColor.yellow, MsgServer.MsgMessage.ChatMode.ContinueRightCorner);
                          
//#else
//                            msg = new MsgServer.MsgMessage("My Kills: " + user.Player.TournamentKills.ToString() + "", MsgServer.MsgMessage.MsgColor.yellow, MsgServer.MsgMessage.ChatMode.ContinueRightCorner);

//#endif
//                            user.Send(msg.GetArray(stream));
//                        }
//                    }
//                    ScoreStamp = DateTime.Now.AddSeconds(1);
//                }


//            }
//        }

//        public void SendMapPacket(ServerSockets.Packet stream)
//        {
//            foreach (var user in MapPlayers())
//                user.Send(stream);
//        }
//        public Client.GameClient[] MapPlayers()
//        {
//            return Map.Values.Where(p => InTournament(p)).ToArray();
//        }
//        public bool ExistDragonWarEffect()
//        {
//            return MapPlayers().Where(p => p.Player.ContainFlag(MsgServer.MsgUpdate.Flags.DragonSwing) && InTournament(p.Player.Owner)).Count() != 0;
//        }


//    }
//}
