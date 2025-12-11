using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeathWish.Game.MsgServer;

namespace DeathWish.Game.MsgTournaments
{
    public class MsgTopFight : ITournament
    {

        public const uint King = 645;
        public const uint Prince = 500;
        public const uint Duke = 430;
        public const uint Earl = 400;
        public ProcesType Process { get; set; }
        public DateTime StartTimer = new DateTime();
        public DateTime InfoTimer = new DateTime();
        public uint Secounds = 120;
        public Role.GameMap Map1;
        public Role.GameMap Map2;
        public Role.GameMap Map3;
        public Role.GameMap Map4;
        public uint DinamicMap1 = 0;
        public uint DinamicMap2 = 0;
        public uint DinamicMap3 = 0;
        public uint DinamicMap4 = 0;
        public KillerSystem KillSystem;
        public TournamentType Type { get; set; }
        public MsgTopFight(TournamentType _type)
        {
            Type = _type;
            Process = ProcesType.Dead;
        }

        public void Open()
        {
            if (Process == ProcesType.Dead)
            {
                KillSystem = new KillerSystem();
                StartTimer = DateTime.Now;
#if Arabic
                MsgSchedules.SendInvitation("MsgTopFight", "ConquerPoints,YinYangFruit", 295, 145, 1002, 0, 60);
#else
                MsgSchedules.SendInvitation("Nobility-Fight", "Special Reward", 444,357, 1002, 0, 120);
#endif


                if (Map1 == null)
                {
                    Map1 = Database.Server.ServerMaps[8893];
                    DinamicMap1 = Map1.GenerateDynamicID();
                }
                if (Map2 == null)
                {
                    Map2 = Database.Server.ServerMaps[8894];
                    DinamicMap2 = Map2.GenerateDynamicID();
                }
                if (Map3 == null)
                {
                    Map3 = Database.Server.ServerMaps[8895];
                    DinamicMap3 = Map3.GenerateDynamicID();
                }
                if (Map4 == null)
                {
                    Map4 = Database.Server.ServerMaps[8896];
                    DinamicMap4 = Map4.GenerateDynamicID();
                }
                InfoTimer = DateTime.Now;
                Secounds = 120;
                Process = ProcesType.Idle;
            }
        }
        public bool Join(Client.GameClient user, ServerSockets.Packet stream)
        {
            if (Process == ProcesType.Idle)
            {
                if (user.Player.NobilityRank == Role.Instance.Nobility.NobilityRank.King)
                {
                    ushort x = 0;
                    ushort y = 0;
                    Map1.GetRandCoord(ref x, ref y);
                    user.Teleport(x, y, Map1.ID, DinamicMap1);
                    user.Player.SetPkMode(DeathWish.Role.Flags.PKMode.PK);
                    return true;

                }
                else if (user.Player.NobilityRank == Role.Instance.Nobility.NobilityRank.Prince)
                {
                    ushort x = 0;
                    ushort y = 0;
                    Map2.GetRandCoord(ref x, ref y);
                    user.Teleport(x, y, Map2.ID, DinamicMap2);
                    user.Player.SetPkMode(DeathWish.Role.Flags.PKMode.PK);
                    return true;
                }
                else if (user.Player.NobilityRank == Role.Instance.Nobility.NobilityRank.Duke)
                {
                    ushort x = 0;
                    ushort y = 0;
                    Map3.GetRandCoord(ref x, ref y);
                    user.Teleport(x, y, Map3.ID, DinamicMap3);
                    user.Player.SetPkMode(DeathWish.Role.Flags.PKMode.PK);
                    return true;
                }
                else if (user.Player.NobilityRank == Role.Instance.Nobility.NobilityRank.Earl)
                {
                    ushort x = 0;
                    ushort y = 0;
                    Map4.GetRandCoord(ref x, ref y);
                    user.Teleport(x, y, Map4.ID, DinamicMap4);
                    user.Player.SetPkMode(DeathWish.Role.Flags.PKMode.PK);
                    return true;
                }
            }
            return false;
        }
        public void CheckUp()
        {
            if (Process == ProcesType.Idle)
            {
                if (DateTime.Now > StartTimer.AddSeconds(120))
                {

                    MsgSchedules.SendSysMesage("TopNobility has started! signup are now closed.", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);
                    StartTimer = DateTime.Now;
                    Process = ProcesType.Alive;
                }
                else if (DateTime.Now > InfoTimer.AddSeconds(10))
                {
                    Secounds -= 10;
                    MsgSchedules.SendSysMesage("Fight starts in " + Secounds.ToString() + " seconds.", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);
                    InfoTimer = DateTime.Now;
                }
            }
            if (Process == ProcesType.Alive)
            {
                if (DateTime.Now > StartTimer.AddMinutes(3))
                {
                    foreach (var user in MapPlayers1())
                    {
                        MsgSchedules.SendSysMesage("TopNobility has ends no one win.", MsgServer.MsgMessage.ChatMode.BroadcastMessage, MsgServer.MsgMessage.MsgColor.red);

                    }
                    foreach (var user in MapPlayers2())
                    {
                        MsgSchedules.SendSysMesage("TopNobility has ends no one win.", MsgServer.MsgMessage.ChatMode.BroadcastMessage, MsgServer.MsgMessage.MsgColor.red);

                    }
                    foreach (var user in MapPlayers3())
                    {
                        MsgSchedules.SendSysMesage("TopNobility has ends no one win.", MsgServer.MsgMessage.ChatMode.BroadcastMessage, MsgServer.MsgMessage.MsgColor.red);

                    }
                    foreach (var user in MapPlayers4())
                    {
                        MsgSchedules.SendSysMesage("TopNobility has ends no one win.", MsgServer.MsgMessage.ChatMode.BroadcastMessage, MsgServer.MsgMessage.MsgColor.red);

                    }

                    Process = ProcesType.Dead;
                }
                if (MapPlayers1().Length == 1)
                {
                    var winner = MapPlayers1().First();
                    MsgSchedules.SendSysMesage("" + winner.Player.Name + " has Won TopNobility~King#52 , he received [ " + King.ToString() + " Cps + DIABLO Spin + 5 Coins + 1 EP]", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.white);
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        if (winner.Inventory.HaveSpace(1))
                        {
                            winner.Inventory.Add(stream, 3008733, 1);
                            winner.Player.ConquerPoints += King;
                            winner.Player.BoundConquerPoints += 5;
                            winner.Player.SendUpdate(stream, winner.Player.ConquerPoints, MsgUpdate.DataType.ConquerPoints);
                            winner.Player.SendUpdate(stream, winner.Player.BoundConquerPoints, MsgUpdate.DataType.BoundConquerPoints);
                        }
                        else
                        {
                            winner.Inventory.AddReturnedItem(stream, 3008733);
                            winner.Player.ConquerPoints += King;
                            winner.Player.BoundConquerPoints += 5;
                            winner.Player.SendUpdate(stream, winner.Player.ConquerPoints, MsgUpdate.DataType.ConquerPoints);
                            winner.Player.SendUpdate(stream, winner.Player.BoundConquerPoints, MsgUpdate.DataType.BoundConquerPoints);
                        }
                    } 
                    winner.Teleport(300, 278, 1002, 0);
                    winner.Player.SetPkMode(DeathWish.Role.Flags.PKMode.Capture);

                }
                if (MapPlayers2().Length == 1)
                {
                    var winner = MapPlayers2().First();
                    MsgSchedules.SendSysMesage("" + winner.Player.Name + " has Won TopNobility~Prince#52  he received [" + Prince.ToString() + " Cps + DIABLO Spin + 5 Coins + 1 EP]", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.white);
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        if (winner.Inventory.HaveSpace(1))
                        {
                            winner.Inventory.Add(stream, 3008733, 1);
                            winner.Player.ConquerPoints += Prince;
                            winner.Player.BoundConquerPoints += 5;
                            winner.Player.SendUpdate(stream, winner.Player.ConquerPoints, MsgUpdate.DataType.ConquerPoints);
                            winner.Player.SendUpdate(stream, winner.Player.BoundConquerPoints, MsgUpdate.DataType.BoundConquerPoints);
                        }
                        else
                        {
                            winner.Inventory.AddReturnedItem(stream, 3008733);
                            winner.Player.ConquerPoints += Prince;
                            winner.Player.BoundConquerPoints += 5;
                            winner.Player.SendUpdate(stream, winner.Player.ConquerPoints, MsgUpdate.DataType.ConquerPoints);
                            winner.Player.SendUpdate(stream, winner.Player.BoundConquerPoints, MsgUpdate.DataType.BoundConquerPoints);
                        }
                    }
                    winner.Teleport(300, 278, 1002, 0);
                    winner.Player.SetPkMode(DeathWish.Role.Flags.PKMode.Capture);
                }
                if (MapPlayers3().Length == 1)
                {
                    var winner = MapPlayers3().First();
                    MsgSchedules.SendSysMesage("" + winner.Player.Name + " has Won TopNobility~Duke#52 , he received [" + Duke.ToString() + " Cps + DIABLO Spin + 5 Coins + 1 EP]", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.white);
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        if (winner.Inventory.HaveSpace(1))
                        {
                            winner.Inventory.Add(stream, 3008733, 1);
                            winner.Player.ConquerPoints += Duke;
                            winner.Player.BoundConquerPoints += 5;
                            winner.Player.SendUpdate(stream, winner.Player.ConquerPoints, MsgUpdate.DataType.ConquerPoints);
                            winner.Player.SendUpdate(stream, winner.Player.BoundConquerPoints, MsgUpdate.DataType.BoundConquerPoints);
                        }
                        else
                        {
                            winner.Inventory.AddReturnedItem(stream, 3008733);
                            winner.Player.ConquerPoints += Duke;
                            winner.Player.BoundConquerPoints += 5;
                            winner.Player.SendUpdate(stream, winner.Player.ConquerPoints, MsgUpdate.DataType.ConquerPoints);
                            winner.Player.SendUpdate(stream, winner.Player.BoundConquerPoints, MsgUpdate.DataType.BoundConquerPoints);
                        }
                    }
                    winner.Teleport(300, 278, 1002, 0);
                    winner.Player.SetPkMode(DeathWish.Role.Flags.PKMode.Capture);
                }
                if (MapPlayers4().Length == 1)
                {
                    var winner = MapPlayers4().First();
                    MsgSchedules.SendSysMesage("" + winner.Player.Name + " has Won TopNobility~Earl#52 , he received [" + Earl.ToString() + " Cps + DIABLO Spin + 5 Coins + 1 EP]", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.white);
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        if (winner.Inventory.HaveSpace(1))
                        {
                            winner.Inventory.Add(stream, 3008733, 1);
                            winner.Player.ConquerPoints += Earl;
                            winner.Player.BoundConquerPoints += 5;
                            winner.Player.SendUpdate(stream, winner.Player.ConquerPoints, MsgUpdate.DataType.ConquerPoints);
                            winner.Player.SendUpdate(stream, winner.Player.BoundConquerPoints, MsgUpdate.DataType.BoundConquerPoints);
                        }
                        else
                        {
                            winner.Inventory.AddReturnedItem(stream, 3008733);
                            winner.Player.ConquerPoints += Earl;
                            winner.Player.BoundConquerPoints += 5;
                            winner.Player.SendUpdate(stream, winner.Player.ConquerPoints, MsgUpdate.DataType.ConquerPoints);
                            winner.Player.SendUpdate(stream, winner.Player.BoundConquerPoints, MsgUpdate.DataType.BoundConquerPoints);
                        }
                    }
                    winner.Teleport(300, 278, 1002, 0);
                    winner.Player.SetPkMode(DeathWish.Role.Flags.PKMode.Capture);

                }
                Extensions.Time32 Timer = Extensions.Time32.Now;
                foreach (var user in MapPlayers1())
                {
                    if (user.Player.Alive == false)
                    {
                        if (user.Player.DeadStamp.AddSeconds(2) < Timer)
                        {
                            user.Teleport(300, 278, 1002);
                            user.Player.SetPkMode(DeathWish.Role.Flags.PKMode.Capture);

                        }
                    }
                }
                foreach (var user in MapPlayers2())
                {
                    if (user.Player.Alive == false)
                    {
                        if (user.Player.DeadStamp.AddSeconds(2) < Timer)
                        {
                            user.Teleport(300, 278, 1002);
                            user.Player.SetPkMode(DeathWish.Role.Flags.PKMode.Capture);

                        }
                    }
                }
                foreach (var user in MapPlayers3())
                {
                    if (user.Player.Alive == false)
                    {
                        if (user.Player.DeadStamp.AddSeconds(2) < Timer)
                        {
                            user.Teleport(300, 278, 1002);
                            user.Player.SetPkMode(DeathWish.Role.Flags.PKMode.Capture);

                        }
                    }
                }
                foreach (var user in MapPlayers4())
                {
                    if (user.Player.Alive == false)
                    {
                        if (user.Player.DeadStamp.AddSeconds(2) < Timer)
                        {
                            user.Teleport(300, 278, 1002);
                            user.Player.SetPkMode(DeathWish.Role.Flags.PKMode.Capture);

                        }
                    }
                }
                if (MapPlayers1().Length == 0 && MapPlayers2().Length == 0 && MapPlayers3().Length == 0 && MapPlayers4().Length == 0)
                {
                    Process = ProcesType.Dead;
                }

            }
        }

        public Client.GameClient[] MapPlayers1()
        {
            return Map1.Values.Where(p => p.Player.DynamicID == DinamicMap1 && p.Player.Map == Map1.ID).ToArray();
        }
        public Client.GameClient[] MapPlayers2()
        {
            return Map2.Values.Where(p => p.Player.DynamicID == DinamicMap2 && p.Player.Map == Map2.ID).ToArray();
        }
        public Client.GameClient[] MapPlayers3()
        {
            return Map3.Values.Where(p => p.Player.DynamicID == DinamicMap3 && p.Player.Map == Map3.ID).ToArray();
        }
        public Client.GameClient[] MapPlayers4()
        {
            return Map4.Values.Where(p => p.Player.DynamicID == DinamicMap4 && p.Player.Map == Map4.ID).ToArray();
        }
        public bool InTournament(Client.GameClient user)
        {
            if (Map1 == null) return false;
            if (Map2 == null) return false;
            if (Map3 == null) return false;
            if (Map4 == null) return false;
            return user.Player.Map == Map1.ID && user.Player.DynamicID == DinamicMap1 || user.Player.Map == Map2.ID && user.Player.DynamicID == DinamicMap2 || user.Player.Map == Map3.ID && user.Player.DynamicID == DinamicMap3 || user.Player.Map == Map4.ID && user.Player.DynamicID == DinamicMap4;
        }
    }
}
