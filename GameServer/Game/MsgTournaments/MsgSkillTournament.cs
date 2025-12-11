using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeathWish.Game.MsgTournaments
{
    public class MsgSkillTournament : ITournament
    {

        public const uint MapID = 2575, RewardConquerPoints = 200000, MaxLifes = 4;

        public ProcesType Process { get; set; }
        public TournamentType Type { get; set; }
        public KillerSystem KillSystem;
        public DateTime StartTimer = new DateTime();
        public DateTime ScoreStamp = new DateTime();
        public DateTime InfoTimer = new DateTime();
        public Role.GameMap Map;
        public uint DinamicID, Secounds = 0;

        public MsgSkillTournament(TournamentType _Type)
        {
            Process = ProcesType.Dead;
            Type = _Type;
        }
        public void Open()
        {
            if (Process == ProcesType.Dead)
            {

                KillSystem = new KillerSystem();
                Map = Database.Server.ServerMaps[MapID];
                DinamicID = Map.GenerateDynamicID();
#if Arabic
                 MsgSchedules.SendInvitation("SkillTournament", "ConquerPoints", 448, 353, 1002, 0, 60);
#else
                MsgSchedules.SendInvitation("SkillTournament[SS/FB]", "200K Cps + 2 DIABLO[Spin] + 10 E-P", 452, 354, 1002, 0, 120);
#endif

                StartTimer = DateTime.Now;
                InfoTimer = DateTime.Now;
                Secounds = 120;
                Process = ProcesType.Idle;
            }
        }
        public void Revive(Extensions.Time32 Timer, Client.GameClient user)
        {
            if (user.Player.Alive == false && Process != ProcesType.Dead)
            {
                if (InTournament(user))
                {
                    if (user.Player.DeadStamp.AddSeconds(3) < Timer)
                    {
                        user.Teleport(428, 379, 1002);
                        user.Player.SetPkMode(DeathWish.Role.Flags.PKMode.Capture);
                        user.CreateBoxDialog("You've been eliminated , good luck next time.");
                    }
                }
            }
        }
        public bool Join(Client.GameClient user, ServerSockets.Packet stream)
        {
            if (Process == ProcesType.Idle)
            {
                ushort x = 0;
                ushort y = 0;

                user.Player.SkillTournamentLifes = MaxLifes;
                user.Player.SetPkMode(DeathWish.Role.Flags.PKMode.PK);
                Map.GetRandCoord(ref x, ref y);
                user.Teleport(x, y, Map.ID, DinamicID);
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
                    MsgSchedules.SendSysMesage("SkillTournament[SS/FB] has started! signup are now closed.", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);
                    Process = ProcesType.Alive;
                    StartTimer = DateTime.Now;
                }
                else if (DateTime.Now > InfoTimer.AddSeconds(10))
                {
                    Secounds -= 10;
                    MsgSchedules.SendSysMesage("SkillTournament[SS/FB] Fight starts in " + Secounds.ToString() + " Secounds.", MsgServer.MsgMessage.ChatMode.Center, MsgServer.MsgMessage.MsgColor.red);
                    InfoTimer = DateTime.Now;
                }
            }
            else if (Process == ProcesType.Alive)
            {
                DateTime Now = DateTime.Now;

                if (DateTime.Now > StartTimer.AddMinutes(3))
                {

                    if (MapCount() == 1)
                    {
                        var winner = Map.Values.Where(p => p.Player.DynamicID == DinamicID && p.Player.Map == Map.ID).FirstOrDefault();
                        if (winner != null)
                        {
#if Arabic
  MsgSchedules.SendSysMesage("" + winner.Player.Name + " has Won  SkillTournament. ", MsgServer.MsgMessage.ChatMode.BroadcastMessage, MsgServer.MsgMessage.MsgColor.white);

#else
                            MsgSchedules.SendSysMesage("" + winner.Player.Name + " has Won  SkillTournament-War. 200.000 Cps & 2 DIABLO Spin & 10 EventPoint  ", MsgServer.MsgMessage.ChatMode.BroadcastMessage, MsgServer.MsgMessage.MsgColor.white);
#endif

                            winner.Player.ConquerPoints += (int)RewardConquerPoints;
                            winner.Player.PIKAPoint += 10;

                            using (var rec = new ServerSockets.RecycledPacket())
                            {
                                var stream = rec.GetStream();
                                winner.Inventory.Add(stream, 3008733,2);
                                winner.Teleport(300, 278, 1002);
                                winner.Player.SetPkMode(DeathWish.Role.Flags.PKMode.Capture);
                                Process = ProcesType.Dead;
                            }

                        }
                    }
                    else if (MapCount() == 0)
                    {
                        Process = ProcesType.Dead;
                    }
                }
            }

        }
        public int MapCount()
        {
            return MapPlayers().Length;
        }
        public Client.GameClient[] MapPlayers()
        {
            return Map.Values.Where(p => p.Player.DynamicID == DinamicID && p.Player.Map == Map.ID).ToArray();
        }
        public bool InTournament(Client.GameClient user)
        {
            if (Map == null)
                return false;
            return user.Player.Map == Map.ID && user.Player.DynamicID == DinamicID;
        }

    }
}
