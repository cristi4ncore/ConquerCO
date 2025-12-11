using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeathWish.Game.MsgTournaments
{
    public class MsgKingOfTheHill : ITournament
    {
        public ProcesType Process { get; set; }
        public TournamentType Type { get; set; }
        public DateTime StartTimer = new DateTime();
        public DateTime InfoTimer = new DateTime();
        public DateTime ScoreStamp = new DateTime();
        public KillerSystem KillSystem;
        public uint Secounds = 0;
        public uint DinamicID = 0;
        public Role.GameMap Map;

        public MsgKingOfTheHill(TournamentType _type)
        {
            Type = _type;
            Process = ProcesType.Dead;
        }
        public void Open()
        {
            if (Process == ProcesType.Dead)
            {
                KillSystem = new KillerSystem();
                if (Map == null)
                {
                    Map = Database.Server.ServerMaps[7273];
                    DinamicID = Map.GenerateDynamicID();
                }

                Process = ProcesType.Idle;
                MsgSchedules.SendInvitation("KingOfTheHill", "200K Cps + 2 DIABLO[Spin] + 10 E-P", 452, 354, 1002, 0, 120);

                StartTimer = DateTime.Now;
                InfoTimer = DateTime.Now;
                Secounds = 120;
            }
        }
        public bool Join(Client.GameClient client, ServerSockets.Packet stream)
        {
            if (Process == ProcesType.Idle)
            {
                client.Player.KingOfTheHillScore = 0;
                ushort x = 0;
                ushort y = 0;
                Map.GetRandCoord(ref x, ref y);
                client.Teleport(x, y, Map.ID, DinamicID);
                client.Player.SetPkMode(DeathWish.Role.Flags.PKMode.PK);
                client.Player.AddFlag(MsgServer.MsgUpdate.Flags.Freeze, 120, true);
                return true;
            }
            return false;
        }
        public bool InTournament(Client.GameClient user)
        {
            if (Map == null)
                return false;
            return user.Player.Map == Map.ID && user.Player.DynamicID == DinamicID;
        }
        public void Revive(Client.GameClient user)
        {
            if (user.Player.Alive == false && Process != ProcesType.Dead)
            {
                if (InTournament(user))
                {
                    ushort x = 0;
                    ushort y = 0;
                    Map.GetRandCoord(ref x, ref y);
                    user.Teleport(x, y, Map.ID, DinamicID);
                }
            }
        }
        public void GetPoints(Client.GameClient user)
        {
            if (Process == ProcesType.Alive)
            {
                if (DateTime.Now > user.Player.KingOfTheHillStamp.AddSeconds(8))
                {
                    short distance = Role.Core.GetDistance(user.Player.X, user.Player.Y, 50, 50);
                    if (distance <= 12)
                    {
                        user.Player.KingOfTheHillScore += 2;
                        user.Player.KingOfTheHillStamp = DateTime.Now;
                    }
                }
            }
        }
        public Client.GameClient[] MapPlayers()
        {
            return Map.Values.Where(p => InTournament(p)).ToArray();
        }
        public void SendMapPacket(ServerSockets.Packet stream)
        {
            foreach (var user in MapPlayers())
                user.Send(stream);
        }
        public void CheckUp()
        {
            if (Process == ProcesType.Idle)
            {
                if (DateTime.Now > StartTimer.AddSeconds(120))
                {
#if Arabic
                     MsgSchedules.SendSysMesage("KingOfTheHill has started! signup are now closed.", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);
                   
#else
                    MsgSchedules.SendSysMesage("KingOfTheHill has started! signup are now closed.", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);

#endif
                    StartTimer = DateTime.Now;
                    foreach (var user in MapPlayers())
                        user.Player.RemoveFlag(MsgServer.MsgUpdate.Flags.Freeze);
                    Process = ProcesType.Alive;
                }
                else if (DateTime.Now > InfoTimer.AddSeconds(10))
                {
                    Secounds -= 10;
#if Arabic
                      MsgSchedules.SendSysMesage("Fight starts in " + Secounds.ToString() + " Secounds.", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);
                   
#else
                    MsgSchedules.SendSysMesage("Fight starts in " + Secounds.ToString() + " seconds.", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);

#endif
                    InfoTimer = DateTime.Now;
                }
            }
            if (Process == ProcesType.Alive)
            {
                if (DateTime.Now > StartTimer.AddMinutes(3))
                {

                    var array = MapPlayers().OrderByDescending(p => p.Player.KingOfTheHillScore).ToArray();
                    if (array.Length > 0)
                    {
                        #region Rewards
                        var Winner = array.FirstOrDefault();
                        if (Winner != null && (Winner.Player.KingOfTheHillScore >= 150 || DateTime.Now > StartTimer.AddMinutes(5)))
                        {

                            MsgSchedules.SendSysMesage("KingOfTheHill has ended. All Players of KingOfTheHill has teleported to TwinCity.", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);
                            MsgSchedules.SendSysMesage("" + Winner.Player.Name + " has won the king of hill.[ 200.000 Cps & 2 DIABLO Spin & + 10 E-P ] ", MsgServer.MsgMessage.ChatMode.BroadcastMessage, MsgServer.MsgMessage.MsgColor.white);
                            Winner.Player.ConquerPoints += 200000;
                            Winner.Player.PIKAPoint += 10;
                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                Winner.Inventory.Add(stream, 3008733, 2);
                            }
                            Winner.Teleport(300, 278, 1002);
                            Winner.Player.SetPkMode(DeathWish.Role.Flags.PKMode.Capture);
                            foreach (var user in array)
                            {
                                user.Teleport(300, 278, 1002);//to do
                                user.Player.SetPkMode(DeathWish.Role.Flags.PKMode.Capture);
                            }
                            Process = ProcesType.Dead;
                        }
                        #endregion


                    }
                    else
                        Process = ProcesType.Dead;
                }

                if (DateTime.Now > ScoreStamp)
                {

                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();
                        var array = MapPlayers().OrderByDescending(p => p.Player.KingOfTheHillScore).ToArray();
                        Extensions.Time32 Timer = Extensions.Time32.Now;
                        foreach (var user in MapPlayers())
                        {
                            if (user.Player.Alive)
                                GetPoints(user);
                            else
                            {
                                if (user.Player.DeadStamp.AddSeconds(2) < Timer)
                                    Revive(user);
                            }
                        }
                        Game.MsgServer.MsgMessage msg = new MsgServer.MsgMessage("ScoreBoard: (1st Collect 150--- wins)", MsgServer.MsgMessage.MsgColor.yellow, MsgServer.MsgMessage.ChatMode.FirstRightCorner);
                        SendMapPacket(msg.GetArray(stream));

                        int x = 0;
                        foreach (var obj in array)
                        {
                            if (x == 4)
                                break;
                            Game.MsgServer.MsgMessage amsg = new MsgServer.MsgMessage("No " + (x + 1).ToString() + ". " + obj.Player.Name + " (" + obj.Player.KingOfTheHillScore.ToString() + ")", MsgServer.MsgMessage.MsgColor.yellow, MsgServer.MsgMessage.ChatMode.ContinueRightCorner);
                            SendMapPacket(amsg.GetArray(stream));

                            x++;
                        }
                        foreach (var user in MapPlayers())
                        {
                            msg = new MsgServer.MsgMessage("My Score: " + user.Player.KingOfTheHillScore.ToString() + "", MsgServer.MsgMessage.MsgColor.yellow, MsgServer.MsgMessage.ChatMode.ContinueRightCorner);
                            user.Send(msg.GetArray(stream));
                        }
                    }
                    ScoreStamp = DateTime.Now.AddSeconds(3);
                }
            }
        }
    }
}
