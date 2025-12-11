using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeathWish.Game.MsgTournaments
{
    public class MsgBettingCompetition : ITournament
    {

        public static uint RewardConquerPoints = 0, MinimumBet = 25000;
        public ProcesType Process { get; set; }
        public DateTime StartTimer = new DateTime();
        public DateTime InfoTimer = new DateTime();
        public uint Seconds = 0;
        public Role.GameMap Map;
        public uint DinamicMap = 0;
        public KillerSystem KillSystem;
        public TournamentType Type { get; set; }
        public MsgBettingCompetition(TournamentType _type)
        {
            Type = _type;
            Process = ProcesType.Dead;
        }

        public void Open()
        {
            if (Process == ProcesType.Dead)
            {
                RewardConquerPoints = 0;
                KillSystem = new KillerSystem();
                StartTimer = DateTime.Now;

                MsgSchedules.SendInvitation("BettingCompetition", "Every One Show Bet 25K CPs", 452, 354, 1002, 0, 120);

                if (Map == null)
                {
                    Map = Database.Server.ServerMaps[7272];
                    DinamicMap = Map.GenerateDynamicID();
                }
                InfoTimer = DateTime.Now;
                Seconds = 120;
                Process = ProcesType.Idle;
            }
        }
        public bool Join(Client.GameClient user, ServerSockets.Packet stream)
        {
            if (Process == ProcesType.Idle)
            {
                if (user.Player.ConquerPoints >= MinimumBet)
                {
                    user.Player.ConquerPoints -= MinimumBet;
                    RewardConquerPoints += MinimumBet;
                    user.Player.MessageBox("You`ve betted " + MinimumBet + " CPs and current total bet is " + RewardConquerPoints, null, null);

                }
                else
                {
                    user.Player.MessageBox("Minimum bet is " + MinimumBet + " CPs and you dont have that amount.", null, null);
                    return false;
                }
                ushort x = 0;
                ushort y = 0;
                Map.GetRandCoord(ref x, ref y);
                user.Teleport(x, y, Map.ID, DinamicMap);
                user.Player.SetPkMode(DeathWish.Role.Flags.PKMode.PK);
                return true;
            }
            return false;
        }
        public void CheckUp()
        {
            if (Process == ProcesType.Idle)
            {
                if (DateTime.Now > StartTimer.AddSeconds(120))
                {

                    MsgSchedules.SendSysMesage("BettingCompetition has started! signup are now closed.", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);

                    Process = ProcesType.Alive;
                    StartTimer = DateTime.Now;
                }
                else if (DateTime.Now > InfoTimer.AddSeconds(10))
                {
                    Seconds -= 10;
                    MsgSchedules.SendSysMesage("[BettingCompetition] Fight starts in " + Seconds.ToString() + " Seconds.", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);
                    InfoTimer = DateTime.Now;
                }
            }
            if (Process == ProcesType.Alive)
            {
                if (DateTime.Now > StartTimer.AddMinutes(3))
                {
                    foreach (var user in MapPlayers())
                    {
                        user.Teleport(300, 278, 1002);
                        user.Player.SetPkMode(DeathWish.Role.Flags.PKMode.Capture);
                    }
                    MsgSchedules.SendSysMesage("BettingCompetition has ended. All Players of BettingCompetition has teleported to TwinCity.", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);

                    Process = ProcesType.Dead;
                }
                if (MapPlayers().Length == 1)
                {
                    var winner = MapPlayers().First();

                    MsgSchedules.SendSysMesage("" + winner.Player.Name + " has Won  BettingCompetition , he received " + RewardConquerPoints.ToString() + " Cps & 2 DIABLO Spin & + 10 E-P.", MsgServer.MsgMessage.ChatMode.BroadcastMessage, MsgServer.MsgMessage.MsgColor.white);
                    winner.Player.ConquerPoints += RewardConquerPoints;
                    winner.Player.PIKAPoint += 10;
                   using (var rec = new ServerSockets.RecycledPacket())
                    {
                       var stream = rec.GetStream();
                       winner.Inventory.Add(stream, 3008733, 2);
                   }
                    winner.Teleport(300, 278, 1002, 0);
                    winner.Player.SetPkMode(DeathWish.Role.Flags.PKMode.Capture);

                    Process = ProcesType.Dead;
                }

                Extensions.Time32 Timer = Extensions.Time32.Now;
                foreach (var user in MapPlayers())
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
            }


        }

        public Client.GameClient[] MapPlayers()
        {
            return Map.Values.Where(p => p.Player.DynamicID == DinamicMap && p.Player.Map == Map.ID).ToArray();
        }

        public bool InTournament(Client.GameClient user)
        {
            if (Map == null) return false;
            return user.Player.Map == Map.ID && user.Player.DynamicID == DinamicMap;
        }
    }
}
