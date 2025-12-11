using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeathWish.Game.MsgTournaments
{
    public class MsgToPMan : ITournament
    {

        public const uint RewardConquerPoints = 76000;

        public ProcesType Process { get; set; }
        public DateTime StartTimer = new DateTime();
        public DateTime InfoTimer = new DateTime();
        public uint Secounds = 120;
        public Role.GameMap Map;
        public uint DinamicMap = 0;
        public KillerSystem KillSystem;
        public TournamentType Type { get; set; }
        public MsgToPMan(TournamentType _type)
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
                MsgSchedules.SendInvitation("ToPMan", "ConquerPoints,YinYangFruit", 333, 279 1002, 0, 60);
#else
     //           MsgSchedules.SendInvitation("ToPMan", "96.000 CPS  &&  20m Gold", 329, 298, 1002, 0, 60);
#endif


                if (Map == null)
                {
                    Map = Database.Server.ServerMaps[700];
                    DinamicMap = Map.GenerateDynamicID();
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
                ushort x = 0;
                ushort y = 0;
                Map.GetRandCoord(ref x, ref y);
                user.Teleport(x, y, Map.ID, DinamicMap);
                return true;
            }
            return false;
        }
        public void CheckUp()
        {
            if (Process == ProcesType.Idle)
            {
                if (DateTime.Now > StartTimer.AddMinutes(2))
                {
#if Arabic
                    MsgSchedules.SendSysMesage("ToPMan has started! signup are now closed.", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);
#else
                //    MsgSchedules.SendSysMesage("ToPMan has started! signup are now closed.", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);
#endif

                    Process = ProcesType.Alive;
                    StartTimer = DateTime.Now;
                }
                else if (DateTime.Now > InfoTimer.AddSeconds(10))
                {
                    Secounds -= 10;
#if Arabic
                      MsgSchedules.SendSysMesage("[ToPMan] Fight starts in " + Secounds.ToString() + " Secounds.", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);
                  
#else
               //     MsgSchedules.SendSysMesage("[ToPMan] Fight starts in " + Secounds.ToString() + " Secounds.", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);

#endif
                    InfoTimer = DateTime.Now;
                }
            }
            if (Process == ProcesType.Alive)
            {
                if (DateTime.Now > StartTimer.AddMinutes(1))
                {
                    foreach (var user in MapPlayers())
                    {
                        user.Teleport(428, 379, 1002);
                    }
#if Arabic
                      MsgSchedules.SendSysMesage("ToPMan has ended. All Players of ToPMan has teleported to TwinCity.", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);
                  
#else
                   // MsgSchedules.SendSysMesage("ToPMan has ended. All Players of ToPMan has teleported to TwinCity.", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);

#endif

                    Process = ProcesType.Dead;
                }
                if (MapPlayers().Length == 1)
                {
                    var winner = MapPlayers().First();

#if Arabic
                       MsgSchedules.SendSysMesage("" + winner.Player.Name + " has Won  ToPMan , he received " + RewardConquerPoints.ToString() + " ConquerPoints and 1 YinYangFruit.", MsgServer.MsgMessage.ChatMode.TopLeftSystem, MsgServer.MsgMessage.MsgColor.white);
                 
#else
                //    MsgSchedules.SendSysMesage("" + winner.Player.Name + " has Won  ToPMan , he received " + RewardConquerPoints.ToString() + " ConquerPoints .", MsgServer.MsgMessage.ChatMode.TopLeftSystem, MsgServer.MsgMessage.MsgColor.white);

#endif
                    winner.Player.ConquerPoints += RewardConquerPoints;
                    winner.Player.Money += 10000000;
               

#if Arabic
                        winner.SendSysMesage("You received " + RewardConquerPoints.ToString() + " ConquerPoints. ", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
                 
#else
                    winner.Player.WarPoints += 1000;
                    winner.SendSysMesage("You received " + RewardConquerPoints.ToString() + " ConquerPoints + 10.000.000 .", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);

#endif
                    winner.Teleport(298, 279, 1002, 0);

                    Process = ProcesType.Dead;
                }

                Extensions.Time32 Timer = Extensions.Time32.Now;
                foreach (var user in MapPlayers())
                {
                    if (user.Player.Alive == false)
                    {
                        if (user.Player.DeadStamp.AddSeconds(4) < Timer)
                            user.Teleport(428, 379, 1002);
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
